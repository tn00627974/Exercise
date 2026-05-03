"""批量推送隊列模組。

職責：管理消息隊列與雙觸發條件（數量 / 超時），不包含任何 I/O 邏輯。
"""

import asyncio
import logging
from dataclasses import dataclass, field
from datetime import datetime
from typing import Awaitable, Callable, Dict, List, Optional

# 刷新回調型別：接收 target_id，回傳 awaitable
FlushCallback = Callable[[str], Awaitable[None]]


@dataclass
class QueuedMessage:
    """隊列中的待推送消息。"""

    target_id: str
    content: str
    enqueued_at: datetime = field(default_factory=datetime.now)


class BatchQueue:
    """單一 target 的批量隊列，支援數量與超時雙觸發。

    觸發條件（任一滿足即推送）：
    - 累積消息數達到 max_batch_size
    - 距首條消息超過 max_wait_seconds
    """

    def __init__(
        self,
        target_id: str,
        flush_callback: FlushCallback,
        max_batch_size: int = 10,
        max_wait_seconds: int = 3600,
    ) -> None:
        self.target_id = target_id
        self._flush_callback = flush_callback
        self.max_batch_size = max_batch_size
        self.max_wait_seconds = max_wait_seconds
        self._queue: List[QueuedMessage] = []
        self._timeout_task: Optional[asyncio.Task] = None

    async def enqueue(self, content: str) -> None:
        """加入一條消息，滿足觸發條件時自動推送。"""
        self._queue.append(QueuedMessage(target_id=self.target_id, content=content))

        # 首條消息：啟動超時計時器
        if len(self._queue) == 1:
            self._arm_timeout()

        # 達到數量上限：取消計時器並立即推送
        if len(self._queue) >= self.max_batch_size:
            logging.info(
                "[BatchQueue] %s 達到批量上限 %d 條，立即推送",
                self.target_id,
                self.max_batch_size,
            )
            self._disarm_timeout()
            await self._do_flush()

    def drain(self) -> List[str]:
        """取出所有消息並清空隊列（同步，無副作用）。"""
        contents = [msg.content for msg in self._queue]
        self._queue.clear()
        return contents

    def __len__(self) -> int:
        return len(self._queue)

    def _arm_timeout(self) -> None:
        """啟動超時計時器，到期後觸發推送。"""
        self._disarm_timeout()

        async def _on_timeout() -> None:
            await asyncio.sleep(self.max_wait_seconds)
            if self._queue:
                logging.info(
                    "[BatchQueue] %s 超時 %ds，觸發推送",
                    self.target_id,
                    self.max_wait_seconds,
                )
                await self._do_flush()
            # 計時器自然完成，清除自身參照
            self._timeout_task = None

        self._timeout_task = asyncio.create_task(_on_timeout())

    def _disarm_timeout(self) -> None:
        """取消計時器（勿從計時器回調本身呼叫，避免自我取消）。"""
        if self._timeout_task is not None and not self._timeout_task.done():
            self._timeout_task.cancel()
        self._timeout_task = None

    async def _do_flush(self) -> None:
        """呼叫外部刷新回調，例外只記錄不拋出（避免中斷隊列邏輯）。"""
        try:
            await self._flush_callback(self.target_id)
        except Exception:
            logging.exception("[BatchQueue] 推送失敗: %s", self.target_id)

    async def shutdown(self) -> None:
        """強制刷新剩餘消息並清理計時器。"""
        self._disarm_timeout()
        if self._queue:
            await self._do_flush()


class BatchQueueManager:
    """管理多個 target_id 各自對應的 BatchQueue。

    職責：自動建立隊列、路由入隊請求、統一關閉。
    """

    def __init__(
        self,
        flush_callback: FlushCallback,
        max_batch_size: int = 10,
        max_wait_seconds: int = 3600,
    ) -> None:
        self._flush_callback = flush_callback
        self.max_batch_size = max_batch_size
        self.max_wait_seconds = max_wait_seconds
        self._queues: Dict[str, BatchQueue] = {}

    def _get_or_create(self, target_id: str) -> BatchQueue:
        if target_id not in self._queues:
            self._queues[target_id] = BatchQueue(
                target_id=target_id,
                flush_callback=self._flush_callback,
                max_batch_size=self.max_batch_size,
                max_wait_seconds=self.max_wait_seconds,
            )
        return self._queues[target_id]

    async def enqueue(self, target_id: str, content: str) -> None:
        """將消息加入指定 target 的隊列。"""
        await self._get_or_create(target_id).enqueue(content)

    def drain(self, target_id: str) -> List[str]:
        """取出並清空指定 target 的隊列。"""
        if target_id not in self._queues:
            return []
        return self._queues[target_id].drain()

    async def shutdown(self) -> None:
        """關閉所有隊列並刷新剩餘消息。"""
        for queue in list(self._queues.values()):
            await queue.shutdown()
        self._queues.clear()

    def stats(self) -> Dict[str, int]:
        """回傳各 target 目前的待推送消息數。"""
        return {tid: len(q) for tid, q in self._queues.items()}
