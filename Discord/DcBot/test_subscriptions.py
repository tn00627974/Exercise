#!/usr/bin/env python3
"""Test SUBSCRIPTIONS JSON parsing"""

import json
import os
from dotenv import load_dotenv

load_dotenv()

subs_env = os.getenv("SUBSCRIPTIONS", "").strip()
print("SUBSCRIPTIONS raw string:")
print(subs_env)
print("\n" + "="*60 + "\n")

try:
    parsed = json.loads(subs_env)
    print(f"✅ JSON 解析成功！")
    print(f"總訂閱數：{len(parsed)}\n")
    
    for i, sub in enumerate(parsed, 1):
        print(f"訂閱 #{i}:")
        print(f"  - 頻道 ID: {sub.get('channel_id')}")
        print(f"  - RSS URL: {sub.get('rss_url')}")
        print(f"  - 提及用戶: {sub.get('mention_user_id', '(無)')}")
        print()
        
except json.JSONDecodeError as e:
    print(f"❌ JSON 解析失敗：{e}")
