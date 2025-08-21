
[2325.解碼訊息](https://leetcode.com/problems/decode-the-message/)

Easy 簡單的

Topics 主題

Companies 公司

Hint 暗示

You are given the strings `key` and `message`, which represent a cipher key and a secret message, respectively. The steps to decode `message` are as follows:  
您將獲得字串 `key` 和 `message` ，它們分別代表金鑰和秘密訊息。解碼 `message` 的步驟如下：

1. Use the **first** appearance of all 26 lowercase English letters in `key` as the **order** of the substitution table.  
    以 `key` 中全部26個小寫英文字母第一次出現的順序作為替換錶的順序。
2. Align the substitution table with the regular English alphabet.  
    將替換錶與常規英文字母對齊。
3. Each letter in `message` is then **substituted** using the table.  
    然後使用表格替換 `message` 中的每個字母。
4. Spaces `' '` are transformed to themselves.  
    空間 `' '` 會轉變為自身。

- For example, given `key = "**hap**p**y** **bo**y"` (actual key would have **at least one** instance of each letter in the alphabet), we have the partial substitution table of (`'h' -> 'a'`, `'a' -> 'b'`, `'p' -> 'c'`, `'y' -> 'd'`, `'b' -> 'e'`, `'o' -> 'f'`).  
    例如，給定 `key = "**hap**p**y** **bo**y"` （實際鍵將至少有字母表中每個字母的一個實例），我們有 ( `'h' -> 'a'` , `'a' -> 'b'` 、 `'p' -> 'c'` 、 `'y' -> 'd'` 、 `'b' -> 'e'` 、 `'o' -> 'f'` ）。

Return _the decoded message_.  
返回解碼後的訊息。

**Example 1: 範例1：**

![](https://assets.leetcode.com/uploads/2022/05/08/ex1new4.jpg)

**Input:** key = "the quick brown fox jumps over the lazy dog", message = "vkbs bs t suepuv"
**Output:** "this is a secret"
**Explanation:** The diagram above shows the substitution table.
It is obtained by taking the first appearance of each letter in "**the** **quick** **brown** **f**o**x** **j**u**mps** o**v**er the **lazy** **d**o**g**".

**Example 2: 範例2：**

![](https://assets.leetcode.com/uploads/2022/05/08/ex2new.jpg)

**Input:** key = "eljuxhpwnyrdgtqkviszcfmabo", message = "zwx hnfx lqantp mnoeius ycgk vcnjrdb"
**Output:** "the five boxing wizards jump quickly"
**Explanation:** The diagram above shows the substitution table.
It is obtained by taking the first appearance of each letter in "**eljuxhpwnyrdgtqkviszcfmabo**".

**Constraints: 限制條件：**

- `26 <= key.length <= 2000`
- `key` consists of lowercase English letters and `' '`.  
    `key` 由小寫英文字母和 `' '` 組成。
- `key` contains every letter in the English alphabet (`'a'` to `'z'`) **at least once**.  
    `key` 包含英文字母表中的每個字母（ `'a'` 到 `'z'` ）至少一次。
- `1 <= message.length <= 2000`
- `message` consists of lowercase English letters and `' '`.  
    `message` 由小寫英文字母和 `' '` 組成。


### 解題思路總結

1. **初始化映射字典**，確保空格映射到自身。
2. **遍歷 `key` 字符串**，將每個唯一字符映射到字母表中的字母。
3. **遍歷 `message` 字符串**，使用建立的映射字典將每個字符轉換為對應的字母。
4. **輸出解碼後的訊息**。



![[Ulysses/成長筆記本/資料工程師/附件圖片檔/Pasted image 20240630232251.png]]

# Python 解法
```python
# python
class Solution:
    def decodeMessage(self, key: str, message: str) -> str:
        mapping = {' ':' '}
        i = 0 
        res = ''
        letters = 'abcdefghijklmnopqrstuvwxyz'
        print(mapping)

        for char in key :
            if char not in mapping :
                mapping[char] = letters[i]  # t : a | h : b 
                i += 1 

        for char in message : 
            res += mapping[char] # v : t | k : h | b : i |
        return res 
```

- 時間複雜`O(n)`
- 程式碼需要遍歷 `key` 中的每個字符，時間複雜度為 O(n)O(n)O(n)，其中 nnn 是 `key` 的長度。
- 對於每個字符，都會執行一個字典查詢和可能的插入操作。由於字典的查詢和插入操作平均為 O(1)O(1)O(1)，因此這部分的時間複雜度為 O(n)O(n)O(n)。

- 空間複雜`O(n)`
- - 這段程式碼需要遍歷 `message` 中的每個字符，時間複雜度為 O(m)O(m)O(m)，其中 mmm 是 `message` 的長度。
- 對於每個字符，都會執行一次字典查詢操作，平均為 O(1)O(1)O(1)。因此這部分的時間複雜度為 O(m)O(m)O(m)。
- 總時間複雜度是這兩部分的和，即 O(n+m)O(n + m)O(n+m)。

### 空間複雜度

1. **映射字典**：
    
    - 映射字典的大小取決於 `key` 中不同字符的數量。最壞情況下，所有26個字母都在 `key` 中，字典的大小為 O(1)O(1)O(1)（因為英文字母的數量是固定的，不超過26個字母，空間複雜度不隨輸入大小而變化）。
2. **結果字符串**：
    
    - 結果字符串 `res` 的大小與 `message` 的長度成正比，為 O(m)O(m)O(m)。

總空間複雜度為 O(1)+O(m)O(1) + O(m)O(1)+O(m)，簡化為 O(m)O(m)O(m)。

### 總結

- **時間複雜度**：O(n+m)O(n + m)O(n+m)，其中 nnn 是 `key` 的長度，mmm 是 `message` 的長度。
- **空間複雜度**：O(m)O(m)O(m)，其中 mmm 是 `message` 的長度。


# Cshrap 解法
```C#
// C#
public class Solution 
{
    public string DecodeMessage(string key, string message) 
    {
        Dictionary<char,char> mapping = new Dictionary<char,char> 

        {
            {' ',' '}
        };
        int i = 0 ;
        string res = "" ; 
        string letters = "abcdefghijklmnopqrstuvwxyz" ;

        foreach (char charKey  in key)
        {
            if(!mapping.ContainsKey(charKey ))
            {
                mapping[charKey ] = letters[i] ; 
                i ++ ; 
            }
        }
        foreach (char charMsg in message)
        {
            res += mapping[charMsg] ;
        }
        return res ;
    }
}
```

- 時間複雜`O(n)`
- 空間複雜`O(n)`
