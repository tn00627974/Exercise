"""Pytest root conftest — 確保專案根目錄在 sys.path 中。"""

import sys
from pathlib import Path

# 將專案根目錄（此檔所在）加入 sys.path，讓 rss_center 可以被找到
ROOT = Path(__file__).parent
if str(ROOT) not in sys.path:
    sys.path.insert(0, str(ROOT))
