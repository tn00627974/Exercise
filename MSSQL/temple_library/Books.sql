CREATE TABLE Books(
Book_ID INT PRIMARY KEY IDENTITY(1,1),
title CHAR(50) NOT NULL ,
author CHAR(20),
Pub_ID INT,
remark CHAR(50),
price INT,
place CHAR(20)
);



ALTER TABLE Books
ALTER COLUMN title CHAR(50) NOT NULL;


EXEC sp_help 'Books';
