
-- Publishers 資料表 
CREATE TABLE Publishers (
Pub_ID INT PRIMARY KEY IDENTITY(1,1), -- 出版商編碼
pub_name CHAR(50) NOT NULL ,  -- 出版商名稱
contact CHAR(20), -- 聯絡人
tel CHAR(20), -- 電話
address CHAR(50) -- 地址
);



ALTER TABLE Books
ALTER COLUMN title CHAR(50) NOT NULL;


EXEC sp_help 'Books';
