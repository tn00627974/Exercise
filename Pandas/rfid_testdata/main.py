import pandas as pd
import glob
import os
from tabulate import tabulate


# 定義要讀取的 CSV 文件路徑
# file_paths = [
#     "inventory_20250303_103123_807_人天線右_右手.csv",
#     "inventory_20250303_103009_232_右手.csv",
#     "inventory_20250303_104219_470.csv",
#     "inventory_20250303_104121_511.csv",
#     "inventory_20250303_104027_102.csv",
#     "inventory_20250303_103908_193.csv",
#     "inventory_20250303_103726_023.csv",
#     "inventory_20250303_103622_002.csv",
#     "inventory_20250303_103419_494.csv",
#     "inventory_20250303_103319_265.csv"
# ]

# 定義要讀取的 CSV 文件資料夾路徑
folder_path = r"C:\Users\Diyi-IDD-003\Desktop\Python\pd\files"

# 使用 glob 模組撈取資料夾底下的所有 CSV 檔案
file_paths = glob.glob(os.path.join(folder_path, "*.csv"))
print(file_paths)

# 初始化一個空的 DataFrame 用於累積數據
total_counts = pd.DataFrame()
a1_to_a4_counts = pd.DataFrame()

# 逐個讀取 CSV 文件並累加數據
for file in file_paths:
    full_path = os.path.join(folder_path, file)
    # print(full_path)
    df = pd.read_csv(full_path)
    
# 將 A1 到 A4 的計數填充為 0
    for col in ['A1Count', 'A2Count', 'A3Count', 'A4Count']:
        df[col] = df[col].fillna(0) # 將 NaN 填充為 0

    # 計算 A1 到 A4 的累積 COUNT
    df['A1Count'] = df['A1Count'].fillna(0)
    df['A2Count'] = df['A2Count'].fillna(0)
    df['A3Count'] = df['A3Count'].fillna(0)
    df['A4Count'] = df['A4Count'].fillna(0)
    
    # 計算每個 EPC 的 A1 到 A4 的總 COUNT
    df['TotalA'] = df[['A1Count', 'A2Count', 'A3Count', 'A4Count']].sum(axis=1)
    
    # 將結果合併到總數據中
    total_counts = pd.concat([total_counts, df[['EPC', 'TotalA']]], ignore_index=True)
    a1_to_a4_counts = pd.concat([a1_to_a4_counts, df[['EPC','A1Count', 'A2Count', 'A3Count', 'A4Count']]], ignore_index=True)

# 計算每個 EPC 的累積 COUNT
final_counts = total_counts.groupby('EPC')['TotalA'].sum().reset_index()
a1_to_a4_grouped = a1_to_a4_counts.groupby('EPC').sum().reset_index()
print("每個 EPC 的總和\n", tabulate(final_counts, headers='keys', tablefmt='psql'))
print("每個 EPC 的 A1Count、A2Count、A3Count 和 A4Count 的總和\n", tabulate(a1_to_a4_grouped, headers='keys', tablefmt='psql'))

# 計算每個 A1Count、A2Count、A3Count 和 A4Count 的總和
a1_total = a1_to_a4_counts['A1Count'].sum()
a2_total = a1_to_a4_counts['A2Count'].sum()
a3_total = a1_to_a4_counts['A3Count'].sum()
a4_total = a1_to_a4_counts['A4Count'].sum()

# # # 顯示各別的總和
all_total = a1_total + a2_total + a3_total + a4_total
print(f"A1Count 總和: {a1_total}")
print(f"A2Count 總和: {a2_total}")
print(f"A3Count 總和: {a3_total}")
print(f"A4Count 總和: {a4_total}")
print(f"總和: {all_total}")
# 顯示結果


