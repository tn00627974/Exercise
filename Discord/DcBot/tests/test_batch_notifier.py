"""Tests for rss_center/batch_notifier.py

涵蓋範圍：
- BaseBatchNotifier._merge()          訊息合併邏輯
- BaseBatchNotifier send → flush 流程  enqueue 到 _push 的完整路徑
- BaseBatchNotifier.shutdown()         強制刷新剩餘隊列
- DiscordBatchNotifier._target_key()   channel_id 驗證與 mention_map
- DiscordBatchNotifier._push()         channel fetch 快取、mention 格式
- LineBatchNotifier._target_key()      target_id 驗證
- LineBatchNotifier._push()            LINE API 呼叫、4xx 例外
- FeishuBatchNotifier._target_key()    webhook_url 優先順序
- FeishuBatchNotifier._push()          飛書 webhook 呼叫、4xx 例外
"""

import sys
import unittest
from pathlib import Path
from unittest.mock import AsyncMock, MagicMock, patch

# 直接執行 (python tests/test_batch_notifier.py) 時確保根目錄在 sys.path
ROOT = Path(__file__).parent.parent
if str(ROOT) not in sys.path:
    sys.path.insert(0, str(ROOT))

from rss_center.batch_notifier import (
    BaseBatchNotifier,
    DiscordBatchNotifier,
    FeishuBatchNotifier,
    LineBatchNotifier,
)
from rss_center.models import Subscription

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------


def _discord_sub(channel_id=111, mention_user_id=None) -> Subscription:
    return Subscription(
        rss_url="https://example.com/rss",
        platform="discord",
        channel_id=channel_id,
        mention_user_id=mention_user_id,
    )


def _line_sub(target_id="Uabc123") -> Subscription:
    return Subscription(
        rss_url="https://example.com/rss",
        platform="line",
        target_id=target_id,
    )


def _feishu_sub(webhook_url=None) -> Subscription:
    return Subscription(
        rss_url="https://example.com/rss",
        platform="feishu",
        webhook_url=webhook_url,
    )


def _aiohttp_mock(status: int = 200, body: str = "OK"):
    """建立模擬 aiohttp async context manager response。"""
    resp = MagicMock()
    resp.status = status
    resp.text = AsyncMock(return_value=body)
    cm = MagicMock()
    cm.__aenter__ = AsyncMock(return_value=resp)
    cm.__aexit__ = AsyncMock(return_value=False)
    return cm


class _ConcreteNotifier(BaseBatchNotifier):
    """最簡具體子類，用於測試 BaseBatchNotifier 邏輯。"""

    _MAX_CONTENT_LENGTH = 100

    def __init__(self, max_batch_size: int = 10, max_wait_seconds: int = 9999):
        super().__init__(max_batch_size, max_wait_seconds)
        self.pushed: list[tuple[str, str]] = []

    def _target_key(self, sub: Subscription) -> str:
        return sub.target_id or "test-key"

    async def _push(self, target_id: str, content: str) -> None:
        self.pushed.append((target_id, content))


# ---------------------------------------------------------------------------
# BaseBatchNotifier._merge()
# ---------------------------------------------------------------------------


class TestMerge(unittest.TestCase):

    def setUp(self):
        self.notifier = _ConcreteNotifier()

    def test_single_message_returned_as_is(self):
        self.assertEqual(self.notifier._merge(["Hello"]), "Hello")

    def test_two_messages_get_numbered_headers(self):
        result = self.notifier._merge(["Msg A", "Msg B"])
        self.assertIn("【1/2】", result)
        self.assertIn("【2/2】", result)
        self.assertIn("Msg A", result)
        self.assertIn("Msg B", result)

    def test_three_messages_get_numbered_headers(self):
        result = self.notifier._merge(["A", "B", "C"])
        self.assertIn("【1/3】", result)
        self.assertIn("【2/3】", result)
        self.assertIn("【3/3】", result)

    def test_merged_content_truncated_when_exceeds_limit(self):
        # 每條 60 字，合併後加標頭必然超過 _MAX_CONTENT_LENGTH=100
        long_msg = "x" * 60
        result = self.notifier._merge([long_msg, long_msg])
        self.assertLessEqual(len(result), 100)
        self.assertTrue(result.endswith("⋯（已截斷）"))

    def test_content_within_limit_not_truncated(self):
        result = self.notifier._merge(["Hi", "Lo"])
        self.assertNotIn("⋯（已截斷）", result)


# ---------------------------------------------------------------------------
# BaseBatchNotifier send → flush cycle
# ---------------------------------------------------------------------------


class TestSendFlushCycle(unittest.IsolatedAsyncioTestCase):

    async def test_immediate_push_when_batch_size_1(self):
        notifier = _ConcreteNotifier(max_batch_size=1)
        sub = _line_sub("U1")
        await notifier.send(sub, "Hello")
        self.assertEqual(notifier.pushed, [("U1", "Hello")])

    async def test_no_push_before_batch_full(self):
        notifier = _ConcreteNotifier(max_batch_size=5)
        sub = _line_sub("U1")
        await notifier.send(sub, "Msg 1")
        await notifier.send(sub, "Msg 2")
        self.assertEqual(notifier.pushed, [])

    async def test_push_fires_when_batch_full(self):
        notifier = _ConcreteNotifier(max_batch_size=2)
        sub = _line_sub("U1")
        await notifier.send(sub, "Msg 1")
        await notifier.send(sub, "Msg 2")
        self.assertEqual(len(notifier.pushed), 1)
        self.assertIn("【1/2】", notifier.pushed[0][1])

    async def test_shutdown_flushes_pending_messages(self):
        notifier = _ConcreteNotifier(max_batch_size=99)
        sub = _line_sub("U1")
        await notifier.send(sub, "Pending")
        self.assertEqual(notifier.pushed, [])
        await notifier.shutdown()
        self.assertEqual(len(notifier.pushed), 1)
        self.assertEqual(notifier.pushed[0][1], "Pending")

    async def test_shutdown_with_empty_queue_is_noop(self):
        notifier = _ConcreteNotifier(max_batch_size=99)
        await notifier.shutdown()  # should not raise
        self.assertEqual(notifier.pushed, [])

    async def test_different_targets_use_separate_queues(self):
        notifier = _ConcreteNotifier(max_batch_size=99)
        sub_a = _line_sub("UA")
        sub_b = _line_sub("UB")
        await notifier.send(sub_a, "Msg for A")
        await notifier.send(sub_b, "Msg for B")
        await notifier.shutdown()
        targets = {p[0] for p in notifier.pushed}
        self.assertEqual(targets, {"UA", "UB"})


# ---------------------------------------------------------------------------
# DiscordBatchNotifier._target_key()
# ---------------------------------------------------------------------------


class TestDiscordTargetKey(unittest.TestCase):

    def _make(self) -> DiscordBatchNotifier:
        return DiscordBatchNotifier(MagicMock())

    def test_raises_if_channel_id_missing(self):
        notifier = self._make()
        sub = Subscription(rss_url="x", platform="discord", channel_id=None)
        with self.assertRaises(RuntimeError):
            notifier._target_key(sub)

    def test_returns_string_channel_id(self):
        notifier = self._make()
        self.assertEqual(
            notifier._target_key(_discord_sub(channel_id=123456)), "123456"
        )

    def test_stores_mention_user_id_on_first_call(self):
        notifier = self._make()
        notifier._target_key(_discord_sub(channel_id=999, mention_user_id=777))
        self.assertEqual(notifier._mention_map["999"], 777)

    def test_mention_map_not_overwritten_by_subsequent_call(self):
        notifier = self._make()
        notifier._target_key(_discord_sub(channel_id=999, mention_user_id=777))
        notifier._target_key(_discord_sub(channel_id=999, mention_user_id=888))
        # setdefault: first writer wins
        self.assertEqual(notifier._mention_map["999"], 777)


# ---------------------------------------------------------------------------
# DiscordBatchNotifier._push()
# ---------------------------------------------------------------------------


class TestDiscordPush(unittest.IsolatedAsyncioTestCase):

    def _make(self) -> tuple[DiscordBatchNotifier, MagicMock]:
        channel = AsyncMock()
        client = MagicMock()
        client.fetch_channel = AsyncMock(return_value=channel)
        notifier = DiscordBatchNotifier(client)
        return notifier, channel

    async def test_push_sends_plain_message(self):
        notifier, channel = self._make()
        notifier._mention_map["42"] = None
        await notifier._push("42", "Hello Discord")
        channel.send.assert_called_once_with("Hello Discord")

    async def test_push_prepends_mention(self):
        notifier, channel = self._make()
        notifier._mention_map["42"] = 123
        await notifier._push("42", "Hello")
        channel.send.assert_called_once_with("<@123> Hello")

    async def test_channel_cached_after_first_fetch(self):
        notifier, channel = self._make()
        notifier._mention_map["42"] = None
        await notifier._push("42", "First")
        await notifier._push("42", "Second")
        notifier.client.fetch_channel.assert_called_once()


# ---------------------------------------------------------------------------
# LineBatchNotifier._target_key()
# ---------------------------------------------------------------------------


class TestLineTargetKey(unittest.TestCase):

    def _make(self) -> LineBatchNotifier:
        return LineBatchNotifier(MagicMock())

    def test_raises_if_target_id_missing(self):
        notifier = self._make()
        sub = Subscription(rss_url="x", platform="line", target_id=None)
        with self.assertRaises(RuntimeError):
            notifier._target_key(sub)

    def test_returns_target_id(self):
        notifier = self._make()
        self.assertEqual(notifier._target_key(_line_sub("Uabc")), "Uabc")


# ---------------------------------------------------------------------------
# LineBatchNotifier._push()
# ---------------------------------------------------------------------------


class TestLinePush(unittest.IsolatedAsyncioTestCase):

    async def test_push_calls_line_api_url(self):
        session = MagicMock()
        session.post = MagicMock(return_value=_aiohttp_mock(200))
        notifier = LineBatchNotifier(session)
        with patch("rss_center.batch_notifier.get_env_var", return_value="fake-token"):
            await notifier._push("Uabc", "Hello LINE")
        url = session.post.call_args.args[0]
        self.assertEqual(url, "https://api.line.me/v2/bot/message/push")

    async def test_push_sends_correct_payload(self):
        session = MagicMock()
        session.post = MagicMock(return_value=_aiohttp_mock(200))
        notifier = LineBatchNotifier(session)
        with patch("rss_center.batch_notifier.get_env_var", return_value="fake-token"):
            await notifier._push("Uabc", "Hello LINE")
        payload = session.post.call_args.kwargs["json"]
        self.assertEqual(payload["to"], "Uabc")
        self.assertEqual(payload["messages"][0]["text"], "Hello LINE")

    async def test_push_raises_on_4xx(self):
        session = MagicMock()
        session.post = MagicMock(return_value=_aiohttp_mock(400, "Bad Request"))
        notifier = LineBatchNotifier(session)
        with patch("rss_center.batch_notifier.get_env_var", return_value="fake-token"):
            with self.assertRaises(RuntimeError):
                await notifier._push("Uabc", "Hello")


# ---------------------------------------------------------------------------
# FeishuBatchNotifier._target_key()
# ---------------------------------------------------------------------------


class TestFeishuTargetKey(unittest.TestCase):

    def _make(self) -> FeishuBatchNotifier:
        return FeishuBatchNotifier(MagicMock())

    def test_uses_sub_webhook_url_when_present(self):
        notifier = self._make()
        sub = _feishu_sub(webhook_url="https://feishu.example.com/hook")
        self.assertEqual(notifier._target_key(sub), "https://feishu.example.com/hook")

    def test_falls_back_to_env_var_when_sub_has_no_url(self):
        notifier = self._make()
        with patch(
            "rss_center.batch_notifier.get_env_var", return_value="https://env-hook.com"
        ):
            key = notifier._target_key(_feishu_sub(webhook_url=None))
        self.assertEqual(key, "https://env-hook.com")

    def test_raises_when_both_sub_and_env_var_missing(self):
        notifier = self._make()
        with patch(
            "rss_center.batch_notifier.get_env_var", side_effect=RuntimeError("Missing")
        ):
            with self.assertRaises(RuntimeError):
                notifier._target_key(_feishu_sub(webhook_url=None))


# ---------------------------------------------------------------------------
# FeishuBatchNotifier._push()
# ---------------------------------------------------------------------------


class TestFeishuPush(unittest.IsolatedAsyncioTestCase):

    async def test_push_calls_webhook_url(self):
        session = MagicMock()
        session.post = MagicMock(return_value=_aiohttp_mock(200))
        notifier = FeishuBatchNotifier(session)
        await notifier._push("https://hook.example.com", "Hello Feishu")
        session.post.assert_called_once_with(
            "https://hook.example.com",
            json={"msg_type": "text", "content": {"text": "Hello Feishu"}},
        )

    async def test_push_raises_on_4xx(self):
        session = MagicMock()
        session.post = MagicMock(return_value=_aiohttp_mock(403, "Forbidden"))
        notifier = FeishuBatchNotifier(session)
        with self.assertRaises(RuntimeError):
            await notifier._push("https://hook.example.com", "Hello")


if __name__ == "__main__":
    unittest.main()
