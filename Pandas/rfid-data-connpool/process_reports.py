import re
import os
from datetime import datetime
import pandas as pd
import matplotlib.pyplot as plt
import matplotlib.dates as mdates
from pathlib import Path

# 設置字體支持中文
plt.rcParams["font.sans-serif"] = ["Microsoft YaHei", "SimHei", "DejaVu Sans"]
plt.rcParams["axes.unicode_minus"] = False

# 獲取所有 .txt 文件
current_dir = Path(__file__).parent
txt_files = sorted(current_dir.glob("ConnPool/Performance_Report_*.txt"))

print(f"找到 {len(txt_files)} 個 .txt 文件")

# 提取數據
data = []

for txt_file in txt_files:
    try:
        with open(txt_file, "r", encoding="utf-8") as f:
            content = f.read()

        # 提取統計時間
        time_match = re.search(
            r"統計時間:\s*(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2})", content
        )
        # 提取連線池數
        pool_match = re.search(r"連線池數:\s*(\d+)", content)

        if time_match and pool_match:
            time_str = time_match.group(1)
            pool_count = int(pool_match.group(1))

            time_obj = datetime.strptime(time_str, "%Y-%m-%d %H:%M:%S")
            data.append(
                {"時間": time_obj, "連線池數": pool_count, "時間字串": time_str}
            )
            print(f"✓ {txt_file.name}: {time_str} | 連線池數: {pool_count}")
    except Exception as e:
        print(f"✗ {txt_file.name}: 讀取失敗 - {e}")

# 創建 DataFrame
df = pd.DataFrame(data)
df = df.sort_values("時間").reset_index(drop=True)

print(f"\n總共提取: {len(df)} 筆數據")
print("\n數據摘要:")
print(df.to_string())

# 保存為 CSV
csv_file = current_dir / "connection_pool_data.csv"
df.to_csv(csv_file, index=False, encoding="utf-8")
print(f"\n已保存CSV: {csv_file}")

# 創建曲線圖
fig, ax = plt.subplots(figsize=(14, 7))

ax.plot(
    df["時間"],
    df["連線池數"],
    marker="o",
    linestyle="-",
    linewidth=2,
    markersize=6,
    color="#2E86AB",
    markerfacecolor="#A23B72",
    markeredgewidth=2,
    markeredgecolor="#A23B72",
)

# 添加網格
ax.grid(True, alpha=0.3, linestyle="--")

# 設置標籤和標題
ax.set_xlabel("時間", fontsize=12, fontweight="bold")
ax.set_ylabel("連線池數 (個)", fontsize=12, fontweight="bold")
ax.set_title("RFID 連線池數變化趨勢", fontsize=14, fontweight="bold")

# 格式化 X 軸時間
ax.xaxis.set_major_formatter(mdates.DateFormatter("%m-%d %H:%M"))
ax.xaxis.set_major_locator(mdates.HourLocator(interval=6))
plt.xticks(rotation=45, ha="right")

# 添加數值標籤
for i, (time, pool) in enumerate(zip(df["時間"], df["連線池數"])):
    if i % max(1, len(df) // 10) == 0 or i == len(df) - 1:  # 只顯示部分標籤
        ax.annotate(
            f"{pool}",
            xy=(time, pool),
            xytext=(0, 8),
            textcoords="offset points",
            ha="center",
            fontsize=9,
        )

# 調整布局
plt.tight_layout()

# 保存圖表
png_file = current_dir / "connection_pool_trend.png"
plt.savefig(png_file, dpi=300, bbox_inches="tight")
print(f"已保存圖表: {png_file}")

plt.show()

# 統計信息
print("\n=== 統計信息 ===")
print(f"時間範圍: {df['時間'].min()} 到 {df['時間'].max()}")
print(f"連線池數最小值: {df['連線池數'].min()}")
print(f"連線池數最大值: {df['連線池數'].max()}")
print(f"連線池數平均值: {df['連線池數'].mean():.2f}")
print(f"連線池數標準差: {df['連線池數'].std():.2f}")
