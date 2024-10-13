CREATE TABLE Records (
    Guest_ID INT NOT NULL,  -- NOT NULL 但無需自增
    Book_ID INT NOT NULL,   -- NOT NULL 但無需自增
    date DATE,
    PRIMARY KEY (Guest_ID, Book_ID) -- 複合主鍵
);


ALTER TABLE Records
ALTER COLUMN Guest_ID CHAR(20) NOT NULL ;
