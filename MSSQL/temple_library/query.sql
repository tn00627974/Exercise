-- 查詢某位作者有哪些藏書
SELECT b.title ,p.pub_name ,b.price 
FROM Books b 
JOIN Publishers p ON b.Pub_ID = p.Pub_ID
WHERE b.author = '粘添壽';

-- 查詢書本出借狀況
SELECT r.date , g.name , b.title 
FROM Records r 
JOIN Books b on r.Book_ID = b.Book_ID 
JOIN Guests g on r.Guest_ID = g.Guest_ID
WHERE b.title = '資料庫程式設計';

-- 查詢翻轉工作室有哪些書
select title, author, price
from books
where Pub_ID = (select Pub_ID
                            from publishers
                            where pub_name = '翻轉工作室');

-- 查詢郭大豪借用那些書，以及借書日期
SELECT b.title ,r.date
FROM Books b 
JOIN Records r on b.Book_ID = r.Book_ID 
JOIN Guests g on g.Guest_ID = r.Guest_ID 
WHERE g.name = '郭大豪';                 
                           
-- 更新 Records 資料表
INSERT INTO Records
VALUES ('A013', 12, GETDATE());
--  Records 資料總表
SELECT COUNT(*)AS 總共筆數 FROM Records 
WHERE Guest_ID ='A013';


--  更新 Records 的 Book_ID書籍
UPDATE Records
SET Book_ID = '11'
WHERE 
Guest_ID = 'A013' and Book_ID = '14';


