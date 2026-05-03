from types import SimpleNamespace
from unittest import TestCase
from unittest.mock import Mock, patch

import bot


class BotCliTests(TestCase):
    @patch("bot.load_dotenv")
    @patch("bot.get_env_var", return_value="fake-token")
    @patch("bot.argparse.ArgumentParser.parse_args")
    @patch("bot._async_main", new_callable=Mock, return_value=object())
    @patch("bot.asyncio.run")
    def test_main_supports_test_line_flag(
        self,
        mock_asyncio_run,
        mock_async_main,
        mock_parse_args,
        mock_get_env_var,
        mock_load_dotenv,
    ):
        mock_parse_args.return_value = SimpleNamespace(
            test=None,
            test_yt=False,
            test_line="LINE 測試成功",
            debug=False,
        )

        bot.main()

        mock_async_main.assert_called_once_with(
            "fake-token",
            test_message=None,
            test_yt=False,
            test_line="LINE 測試成功",
        )
        mock_asyncio_run.assert_called_once()
