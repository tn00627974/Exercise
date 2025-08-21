-- 1.1 Select the names of all the products in the store.

SELECT * FROM Products ;

-- 1.2 Select the names and the prices of all the products in the store.

SELECT name , Price from products ;

-- 1.3 Select the name of the products with a price less than or equal to $200.

SELECT name FROM Products 
WHERE price <= 200;

-- 1.4 Select all the products with a price between $60 and $120.

SELECT price FROM Products 
WHERE price BETWEEN 60 AND 120 ;

-- 1.5 Select the name and price in cents (i.e., the price must be multiplied by 100).

SELECT name , price*100 as PriceCents  FROM Products ;

-- 1.6 Compute the average price of all the products.

SELECT AVG(price) FROM Products; 

-- 1.7 Compute the average price of all products with manufacturer code equal to 2.

SELECT AVG(price) FROM Products
WHERE Manufacturer = '2';

-- 1.8 Compute the number of products with a price larger than or equal to $180.

SELECT COUNT(*) FROM Products 
WHERE price >= 180 ;

-- 1.9 Select the name and price of all products with a price larger than or equal to $180, 
-- and sort first by price (in descending order), and then by name (in ascending order).

SELECT name , price FROM Products 
WHERE price >= 180 
ORDER BY price desc ,name asc;

-- 1.10 Select all the data from the products, including all the data for each product's manufacturer.

SELECT * FROM Products p , Manufacturers m
WHERE p.Manufacturer = m.Code;

-- 1.11 Select the product name, price, and manufacturer name of all the products.

SELECT p.name, p.price , m.name 
FROM Products p , Manufacturers m 
WHERE p.Manufacturer = m.Code;

-- 1.12 Select the average price of each manufacturer's products, showing only the manufacturer's code.

SELECT manufacturer , AVG(price)  FROM Products
GROUP BY manufacturer 
HAVING AVG(price) 
ORDER BY AVG(price) DESC ;

-- 1.13 Select the average price of each manufacturer's products, showing the manufacturer's name.
-- 選擇每個製造商產品的平均價格，顯示製造商的名稱。

SELECT AVG(price) , M.Name FROM products P, Manufacturers M
WHERE p.Manufacturer = M.Code 
GROUP BY M.Name ;


-- 1.14 Select the names of manufacturer whose products have an average price larger than or equal to $150.

SELECT AVG(price) , M.name  FROM Products P , Manufacturers M
WHERE P.Manufacturer = M.code
GROUP BY M.name
HAVING AVG(price) >= 150 ;

-- 1.15 Select the name and price of the cheapest product.

SELECT name , price FROM Products 
ORDER BY price
LIMIT 1 ;

-- 1.16 Select the name of each manufacturer along with the name and price of its most expensive product.
-- 選擇每個製造商的名稱及其最昂貴產品的名稱和價格。

SELECT M.name , P.price  
FROM Products P 
JOIN Manufacturers M 
ON P.Manufacturer = M.code 
WHERE P.Price = (
	SELECT MAX(P.price) FROM Products P
	WHERE P.Manufacturer = M.Code
);

-- 1.17 Add a new product: Loudspeakers, $70, manufacturer 2.
-- 新增產品：揚聲器，70 美元，製造商 2。

INSERT INTO Products( Code, Name , Price , Manufacturer)
VALUES ( 11, 'Loudspeakers' , 70 , 2 );


-- 1.18 Update the name of product 8 to "Laser Printer".
-- 將產品8的名稱更新為「雷射印表機」。

UPDATE Products
SET name = 'Laser Printer'
WHERE CODE = 8 ;

-- 1.19 Apply a 10% discount to all products.
-- 對所有產品應用 10% 的折扣。

UPDATE Products
SET price = price - (price * 0.1);

-- 1.20 Apply a 10% discount to all products with a price larger than or equal to $120.
-- 對所有價格大於或等於 120 美元的產品應用 10% 的折扣。

UPDATE Products
SET price = price - (price * 0.1)
WHERE price >= 120 ;

-- 1.21 Select the name of each manufacturer which have an average price above $145 and contain at least 2 different products.
-- 選擇平均價格高於 145 美元且包含至少 2 種不同產品的每個製造商的名稱。

-- Select m.Name, Avg(p.price) as p_price, COUNT(p.Manufacturer) as m_count
-- FROM Manufacturers m, Products p
-- WHERE p.Manufacturer = m.code
-- GROUP BY m.Name , p.Manufacturer
-- HAVING Avg(p.price) >= 150 and COUNT(p.Manufacturer) >= 2;