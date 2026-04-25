import unittest
from unittest.mock import patch

import line_userid_webhook


class DummyRequest:
    def __init__(self, payload):
        self._payload = payload

    async def json(self):
        return self._payload


class LineUserIdWebhookTests(unittest.IsolatedAsyncioTestCase):
    @patch("builtins.print")
    async def test_handle_callback_prints_user_id(self, mock_print):
        request = DummyRequest(
            {
                "events": [
                    {
                        "type": "message",
                        "replyToken": "reply-token",
                        "source": {"type": "user", "userId": "U1234567890"},
                    }
                ]
            }
        )

        response = await line_userid_webhook.handle_callback(request)

        self.assertEqual(response.status, 200)
        mock_print.assert_any_call("LINE source.userId: U1234567890", flush=True)


if __name__ == "__main__":
    unittest.main()
