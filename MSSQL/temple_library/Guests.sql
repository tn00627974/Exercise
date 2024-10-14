
-- Guests 資料表 
CREATE TABLE Guests (
Guest_ID CHAR(20) PRIMARY KEY, -- 出版商編碼
name CHAR(20) NOT NULL ,  -- 出版商名稱
sex CHAR(20), -- 聯絡人
tel CHAR(20), -- 電話
address CHAR(50) -- 地址
);



--ALTER TABLE Books
--ALTER COLUMN title CHAR(50) NOT NULL;


--EXEC sp_help 'Books';
