#!/usr/bin/env python3
"""Test RSS Feed parsing"""

import feedparser

rss_url = "https://tw.stock.yahoo.com/rss?category=personal-finance"

print(f"Testing RSS URL: {rss_url}")
print("-" * 60)

feed = feedparser.parse(rss_url)

print(f"Feed Title: {feed.feed.get('title', '(no title)')}")
print(f"Feed Link: {feed.feed.get('link', '(no link)')}")
print(f"Feed Description: {feed.feed.get('description', '(no description)')}")
print(f"Total Entries: {len(feed.entries)}")
print("-" * 60)

if feed.entries:
    print(f"\n✅ Successfully fetched {len(feed.entries)} entries!\n")
    print("First 3 entries:")
    for i, entry in enumerate(feed.entries[:3], 1):
        print(f"\n{i}. {entry.get('title', '(no title)')}")
        print(f"   Link: {entry.get('link', '(no link)')}")
        print(f"   ID: {entry.get('id', entry.get('link', '(no id)'))}")
        print(f"   Published: {entry.get('published', '(no date)')}")
else:
    print("❌ No entries found in this RSS feed!")
    if feed.bozo:
        print(f"⚠️  Feed parsing warning: {feed.bozo_exception}")
