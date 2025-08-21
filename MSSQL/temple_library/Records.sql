CREATE TABLE Records (
    Guest_ID CHAR(20) PRIMARY KEY , -- 出版商編碼
    Book_ID INT NOT NULL,   
    date DATE,
    PRIMARY KEY (Guest_ID, Book_ID) -- 複合主鍵
);


ALTER TABLE Records
ALTER COLUMN Guest_ID CHAR(20) NOT NULL ;
