#### Sql

```SQL
-- 員工卡管理 
CREATE TABLE EmployeeCard (
	Id Int IDENTITY(1,1) PRIMARY KEY, 
    NationalId CHAR(10) NULL, -- 身分證字號（A123456789）
	EmployeeName NVARCHAR(50) NOT NULL, -- 員工姓名（高秀英）
    Description NVARCHAR(200) NULL,-- 備註
    CardMasterId INT NULL,-- FK CardMaster 表 Id
    StationId INT NULL, -- FK 院區/案點名稱（仁愛院區）
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()  -- 創建時間
	CONSTRAINT PK_EmployeeCard PRIMARY KEY (Id)
);
```

新增 EmployeeCard 表上 FK CardMasterId
	ALTER TABLE EmployeeCard
	ADD CONSTRAINT FK_EmployeeCard_CardMaster
	FOREIGN KEY (CardMasterId)
	REFERENCES CardMaster(Id);

```SQL
-- 卡片管理 
CREATE TABLE CardMaster (
    Id INT IDENTITY(1,1) PRIMARY KEY,  
	CardId VARCHAR(32) NULL , -- 312be82e（實體ID，可後綁）
    CardName VARCHAR(20) NULL UNIQUE, -- diyisn01042（流水號）
    CardStatus TINYINT NOT NULL DEFAULT 0,
    -- 0 = 未啟用 / 庫存
    -- 1 = 使用中
    -- 2 = 停用 / 作廢
	Description NVARCHAR(200) NULL,-- 備註
    StationId INT NULL, -- 院區/案點名稱（仁愛院區）
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE() -- 創建時間
	CONSTRAINT PK_CardMaster PRIMARY KEY (Id)
);
```

```SQL
-- 案點管理(電子打卡)
CREATE TABLE StationMaster (
    Id INT IDENTITY(1,1),
    StationCode NVARCHAR(20),-- AWJD04170054JD (KKEY)
    StationName NVARCHAR(50) NOT NULL UNIQUE, -- 案點名稱 : 仁愛院區
	StationShortCode NVARCHAR(10), -- 短碼 zxhp 
    ContractStatus TINYINT NOT NULL DEFAULT 1, -- 合約狀態 > 合約到期:0/合約啟用:1 
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- 創建時間
	CONSTRAINT PK_StationMaster PRIMARY KEY (Id)
);
```

```SQL
CREATE TABLE EmployeeCardLog (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeCardId INT NOT NULL,   -- FK -> EmployeeCard.Id
    CardMasterId INT NOT NULL,     -- FK -> CardMaster.Id
    StationId INT NOT NULL,        -- FK -> StationMaster.Id
    ActionType TINYINT NOT NULL,
    -- 1 = 發卡
    -- 2 = 退卡
    -- 3 = 換卡
    -- 4 = 補發
    ActionAt DATETIME NOT NULL DEFAULT GETDATE(),
    Description NVARCHAR(200) NULL -- 小夜班 , 大夜班 ,等敘述
    CONSTRAINT PK_EmployeeCardLog PRIMARY KEY (Id)    
);
```
