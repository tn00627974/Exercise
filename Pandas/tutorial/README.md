# Practise

# PANDAS 常用指令如下
1. 建立 DataFrame:
    - `pd.DataFrame(data, index, columns)` : 從資料建立 DataFrame。
2. 讀取/寫入資料 :
    -  `pd.to_csv(filename)` : 將資料寫入 CSV 檔案。
	- `pd.read_csv(filename)` : 從 CSV 檔案讀取資料。
    -  `pd.to_excel(filename)` : 將資料寫入 Excel 檔案。
    - `pd.read_excel(filename)` : 從 Excel 檔案讀取資料。
    - `pd.to_json(filename)` : 將資料寫入 JSON 檔案。
    - `pd.read_json(filename)` : 從 JSON 檔案讀取資料。
    - `pd.to_html(filename)` : 將資料寫入 HTML 檔案。
    - `pd.read_html(filename)` :從 HTML 檔案讀取資料。
1. 查看數據 :
    - `df.head(n)`: 查看 DataFrame 的前 n 行。
    - `df.tail(n)` : 查看 DataFrame 的後 n 行。
2. 資料選擇與修改:
    - `df['column']` 或 `df.column` : 選擇 DataFrame 中的一列。df['name']
    - `df.loc[index]` : 透過索引選擇行。df[[0],['population','city']]
    - `df.iloc[row_index, column_index]` : 透過位置選擇行和列。**以索引及欄位'編號'取得資料使用df[0,1] 
    - `df.at[label, column]` : 透過標籤選擇單一值。**用於根據行標籤和列標籤提取單個元素, 標籤必須是數字
    - `df.iat[row_index, column_index]` : 透過位置選擇單一值。**只能從內容取得單行值
    - `df['column'] = value`: 修改列的值。
    - `df.drop(labels, axis)` : 刪除行或列。 axis=0 橫列  ,axis=1  直列
    - `df.rename(columns={'old_name': 'new_name'})` : 重新命名列名。
3. 資料處理 :
    - `df.groupby(by)` : 將資料分組。
    - `df.sort_values(by)` : 依照指定列進行排序。
    - `df.merge(df2)` : 合併兩個 DataFrame。
    - `df.pivot_table()` : 建立透視表。
    - `df.apply(func)` : 對 DataFrame 應用函數。
4. 統計計算 :
    - `df.mean()` : 計算平均值。
    - `df.median()` : 計算中位數。
    - `df.sum()` : 計算總和。
    - `df.max()` : 計算最大值。
    - `df.min()` : 計算最小值。
5. 數據視覺化 :
    - `df.plot()` : 繪製 DataFrame 中的資料。
    - `df.hist()` : 繪製直方圖。
    - `df.boxplot()` : 繪製箱型圖。
6. 缺失值處理 :
    - `df.isnull()` : 檢查缺失值。
    - `df.dropna()` : 刪除包含缺失值的行或列。
    - `df.fillna(value)` : 使用指定值填入缺失值。
7.   索引功能 :
	- `set_index()` :  將 DataFrame 中的一列或多列設置為新的索引。# city 和 name 為索引
	- `sort_index()` : 對 DataFrame 根據索引進行排序。(ascending=False,inplace=True)  **ascending參數為 False**  代表重新排序索引 (由大到小) , **inplace參數為 True** 參數為 True 代表直接修改原本的 DataFrame
