
-- Publishers ��ƪ� 
CREATE TABLE Publishers (
Pub_ID INT PRIMARY KEY IDENTITY(1,1), -- �X���ӽs�X
pub_name CHAR(50) NOT NULL ,  -- �X���ӦW��
contact CHAR(20), -- �p���H
tel CHAR(20), -- �q��
address CHAR(50) -- �a�}
);



ALTER TABLE Books
ALTER COLUMN title CHAR(50) NOT NULL;


EXEC sp_help 'Books';
