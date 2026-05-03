import json
import tempfile
import unittest
from pathlib import Path

from bot import load_subscriptions


class SubscriptionCenterTests(unittest.TestCase):
    def _write_temp_json(self, payload):
        tmp = tempfile.NamedTemporaryFile("w", delete=False, suffix=".json", encoding="utf-8")
        with tmp:
            json.dump(payload, tmp, ensure_ascii=False)
        return Path(tmp.name)

    def test_legacy_discord_subscription_is_supported(self):
        path = self._write_temp_json(
            [
                {
                    "channel_id": 123456,
                    "rss_url": "https://example.com/rss",
                    "mention_user_id": 999,
                }
            ]
        )

        subscriptions = load_subscriptions(str(path))

        self.assertEqual(len(subscriptions), 1)
        self.assertEqual(subscriptions[0].platform, "discord")
        self.assertEqual(subscriptions[0].channel_id, 123456)
        self.assertEqual(subscriptions[0].mention_user_id, 999)

    def test_multi_platform_targets_are_expanded(self):
        path = self._write_temp_json(
            [
                {
                    "rss_url": "https://example.com/rss",
                    "display_name": "測試來源",
                    "targets": [
                        {"platform": "discord", "channel_id": 111},
                        {"platform": "line", "target_id": "U123"},
                        {"platform": "feishu", "webhook_url": "https://example.com/hook"},
                    ],
                }
            ]
        )

        subscriptions = load_subscriptions(str(path))
        platforms = sorted(sub.platform for sub in subscriptions)

        self.assertEqual(len(subscriptions), 3)
        self.assertEqual(platforms, ["discord", "feishu", "line"])
        self.assertTrue(all(sub.display_name == "測試來源" for sub in subscriptions))


if __name__ == "__main__":
    unittest.main()
