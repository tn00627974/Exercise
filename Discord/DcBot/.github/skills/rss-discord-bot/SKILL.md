---
name: rss-discord-bot
description: 'Maintain and extend this RSS-to-Discord bot. Use when working on bot.py, subscriptions.json, Yahoo or YouTube feeds, Discord test delivery, duplicate-post prevention, or Railway/Render deployment issues.'
argument-hint: 'Describe the bot task, such as add a feed, test Discord posting, or fix deployment.'
user-invocable: true
---

# RSS Discord Bot

Use this skill when modifying, testing, or troubleshooting the stock RSS Discord bot in this workspace.

## When to Use
- Add or change RSS or YouTube subscriptions
- Adjust message formatting or mention behavior
- Debug duplicate posting, feed parsing, or channel delivery
- Validate deployment behavior for Railway, Render, or Docker
- Run safe local checks before shipping changes

## Project Facts
- Main app entry: [bot.py](../../../bot.py)
- Subscriptions source: [subscriptions.json](../../../subscriptions.json)
- Dependencies: discord.py, feedparser, python-dotenv, aiohttp
- Health check server is required for Render-style hosting
- Poll interval defaults to 300 seconds

## Standard Workflow
1. Read [bot.py](../../../bot.py) and the test utilities if the request affects delivery logic.
2. Check required environment variables:
   - DISCORD_TOKEN
   - SUBSCRIPTIONS_FILE
3. Prefer the smallest root-cause fix.
4. Verify behavior with the commands in [workflow.md](./references/workflow.md).
5. If deployment is involved, keep the HTTP health endpoint working.

## Guardrails
- Do not remove duplicate-post protection unless explicitly requested.
- Keep both normal RSS and YouTube feed behavior intact.
- Preserve test modes such as --test and --test-yt.
- For hosted environments, avoid changes that block the event loop or remove the health server.

## Success Criteria
- Bot starts without syntax errors
- Test message mode still works
- Subscription parsing still works
- No regression in RSS polling or Discord send flow
