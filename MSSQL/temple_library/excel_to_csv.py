import openpyxl
import csv

def excel_to_csv(excel_file, csv_file):
    # 打開Excel文件
    workbook = openpyxl.load_workbook(excel_file)
    
    # 選擇第一個工作表
    sheet = workbook.active
    
    # 打開CSV文件準備寫入
    with open(csv_file, 'w', newline='', encoding='utf-8') as file:
        writer = csv.writer(file)
        
        # 遍歷Excel的每一行並寫入CSV
        for row in sheet.rows:
            writer.writerow([cell.value for cell in row])

    print(f"已將 {excel_file} 轉換為 {csv_file}")

# 使用示例
excel_file = 'Records.xlsx'
csv_file = 'Records.csv'
excel_to_csv(excel_file, csv_file)

