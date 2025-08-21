-- 2.1 Select the last name of all employees.
-- 2.1 選擇所有員工的姓氏。

SELECT LastName FROM Employees ;

-- 2.2 Select the last name of all employees, without duplicates.
-- 2.2 選擇所有員工的姓氏，不要重複。

SELECT distinct LastName  FROM Employees ;

-- 2.3 Select all the data of employees whose last name is "Smith".
-- 2.3 選擇姓氏為「Smith」的員工的所有資料。

SELECT name  FROM Employees 
WHERE LastName = 'Smith' ;

-- 2.4 Select all the data of employees whose last name is "Smith" or "Doe".
-- 2.4 選擇姓氏為「Smith」或「Doe」的員工的所有資料。

SELECT name FROM Employees 
WHERE LastName in ('Doe','Smith')  ;

select * from Employees where lastname = 'Smith' or lastname = 'Doe';


-- 2.5 Select all the data of employees that work in department 14.
-- 2.5 選擇部門14員工的所有數據。

SELECT * FROM employees 
WHERE Department = 14 ; 

-- 2.6 Select all the data of employees that work in department 37 or department 77.
-- 2.6 選取部門37或部門77的員工的所有資料。

SELECT * FROM employees 
WHERE Department in ( 37,77 ) ; 

select * from employees where department = 37 or department = 77;

-- 2.7 Select all the data of employees whose last name begins with an "S".
-- 2.7 選擇姓氏以「S」開頭的所有員工資料。

SELECT * FROM Employees 
WHERE LastName LIKE 'S%' ;

-- 2.8 Select the sum of all the departments' budgets.
-- 2.8 選擇所有部門預算的總和。

SELECT sum(budget) FROM Departments ;

select Name, sum(Budget) from Departments group by Name;

-- 2.9 Select the number of employees in each department (you only need to show the department code and the number of employees).
-- 2.9 選擇每個部門的員工人數（只需顯示部門代碼和員工人數）。

SELECT Department , COUNT(Department) as '每部門人數' FROM Employees 
GROUP BY Department ;

-- 2.10 Select all the data of employees, including each employee's department's data.
-- 2.10 選擇員工的所有數據，包括每位員工所在部門的數據。

SELECT * FROM Employees E
JOIN Departments D
ON E.Department = D.Code ;

-- 2.11 Select the name and last name of each employee, along with the name and budget of the employee's department.
-- 2.11 選擇每位員工的姓名，以及員工所在部門的名稱和預算。

SELECT e.NAME , d.Name , d.Budget FROM Employees e
JOIN Departments d
ON e.Department = d.Code ;

-- 2.12 Select the name and last name of employees working for departments with a budget greater than $60,000.
-- 2.12 選擇預算超過 60,000 美元的部門的員工姓名。

SELECT e.NAME, d.Budget FROM Employees e
JOIN Departments d
ON e.Department = d.Code 
WHERE d.Budget >= '60000' ; 

-- 2.13 Select the departments with a budget larger than the average budget of all the departments.
-- 2.13 選擇預算大於所有部門平均預算的部門。

SELECT Name FROM Departments 
WHERE Budget >
(SELECT avg(Budget) FROM Departments) ; 


-- 2.14 Select the names of departments with more than two employees.
-- 2.14 選擇擁有兩名以上員工的部門名稱。

SELECT d.Name FROM Employees e
JOIN Departments d
ON e.Department = d.Code 
GROUP BY d.Name , d.Budget
HAVING COUNT(*) > 2 ;

-- 2.15 Very Important - Select the name and last name of employees working for departments with second lowest budget.
-- 2.15 非常重要 - 選擇在預算第二低的部門工作的員工的姓名。

SELECT e.name , e.LastName 
FROM Employees e 
WHERE e.Department = (
		SELECT d.Code 
        FROM Departments d
		ORDER BY d.budget 
		LIMIT 1 OFFSET 1 ) ;

SELECT e.Name, e.LastName
FROM Employees e 
WHERE e.Department = (
       SELECT sub.Code 
       FROM (SELECT * FROM Departments d ORDER BY d.budget LIMIT 2) sub 
       ORDER BY budget DESC LIMIT 1);

-- 2.16 Add a new department called "Quality Assurance", with a budget of $40,000 and departmental code 11. 
-- And Add an employee called "Mary Moore" in that department, with SSN 847-21-9811.
-- 2.16 新增一個名為「品質保證」的新部門，預算為 40,000 美元，部門代碼為 11. 在該部門新增一名名為「Mary Moore」的員工，SSN 為 847-21-9811。

INSERT INTO Departments (Code,Name,Budget) VALUES(11,'Quality Assurance',40000);

INSERT INTO Employees (SSN,Name,LastName,Department) VALUES('847219811','Mary','Moore',11);

SELECT * FROM Employees ;

-- 2.17 Reduce the budget of all departments by 10%.
-- 2.17 各部門預算削減10%。

SELECT Budget*0.9 FROM Departments ; 

UPDATE Departments 
SET Budget = Budget * 0.9 ;

-- 2.18 Reassign all employees from the Research department (code 77) to the IT department (code 14).
-- 2.18 將所有員工從研究部門（代碼 77）重新分配到 IT 部門（代碼 14）。

UPDATE Employees
SET Department = '14' -- 更新數字
WHERE Department = '77';  -- 舊有數字

SELECT * FROM Employees ;

-- 2.19 Delete from the table all employees in the IT department (code 14).
-- 2.19 從表格中刪除 IT 部門的所有員工（代碼 14）。

DELETE FROM Employees
WHERE Department = '14' ;

-- 2.20 Delete from the table all employees who work in departments with a budget greater than or equal to $60,000.
-- 2.20 從表中刪除在預算大於或等於 60,000 美元的部門工作的所有員工。

DELETE FROM Employees
WHERE Department IN
( SELECT Code FROM Departments 
WHERE Budget >= '60000' );

SELECT * FROM Employees ;

-- 2.21 Delete from the table all employees.
-- 2.21 從表中刪除所有員工。

DELETE FROM  employees;