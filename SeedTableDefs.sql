CREATE TABLE UserTable (
	UserID INT IDENTITY(1,1) PRIMARY KEY,
	UserName VARCHAR(255),
	UserSurname VARCHAR(255),
	UserEmail VARCHAR(255),
	UserPassword VARCHAR(255),
	UserBalance FLOAT
);

CREATE TABLE ProductTable (
	ProductID INT IDENTITY(1,1) PRIMARY KEY,
	ProductName VARCHAR(255),
	ProductPrice FLOAT NOT NULL,
	ProductAvailability INT NOT NULL,
	ProductDescription VARCHAR(255),
	ProductCategory VARCHAR(255),
	ProductImageURL VARCHAR(255),
	UserID INT NOT NULL FOREIGN KEY REFERENCES UserTable(UserID)
);

CREATE TABLE OrderTable (
	OrderID INT IDENTITY(1,1) PRIMARY KEY,
	UserID INT NOT NULL FOREIGN KEY REFERENCES UserTable(UserID),
	ProductID INT NOT NULL FOREIGN KEY REFERENCES ProductTable(ProductID),
	OrderQuantity INT,
	OrderAddress VARCHAR(255),
	OrderProcessed BIT
);

