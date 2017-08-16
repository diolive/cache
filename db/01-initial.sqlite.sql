CREATE TABLE `Users` (
    `Id`          INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `DisplayName` TEXT NOT NULL
);

CREATE TABLE `Currencies` (
    `Code`   TEXT NOT NULL,
    `Name`   TEXT NOT NULL,
    `Format` TEXT NOT NULL,
    PRIMARY KEY (`Code` ASC)
) WITHOUT ROWID;

CREATE TABLE `UserIdentities` (
    `NameIdentity` TEXT NOT NULL,
    `UserId`       INTEGER NOT NULL,
    PRIMARY KEY (`NameIdentity` ASC),
    FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`)
) WITHOUT ROWID;

CREATE TABLE `Books` (
    `Id`       INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `Name`     TEXT NOT NULL,
    `AuthorId` INTEGER NOT NULL,
    FOREIGN KEY (`AuthorId`) REFERENCES `Users` (`Id`)
);

CREATE TABLE `BookAccess` (
    `BookId` INTEGER NOT NULL,
    `UserId` INTEGER NOT NULL,
    `Role`   INTEGER NOT NULL,
    PRIMARY KEY (`BookId` ASC, `UserId` ASC),
    FOREIGN KEY (`BookId`) REFERENCES `Books` (`Id`),
    FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`)
) WITHOUT ROWID;

CREATE TABLE `Plans` (
    `Id`          INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `Name`        TEXT NOT NULL,
    `BookId`      INTEGER NOT NULL,
    `TargetSave`  REAL NOT NULL,
    `CurrentSave` REAL NOT NULL,
    `Currency`    TEXT NOT NULL,
    `Done`        INTEGER NOT NULL,
    FOREIGN KEY (`Currency`) REFERENCES `Currencies` (`Code`),
    FOREIGN KEY (`BookId`) REFERENCES `Books` (`Id`)
);

CREATE TABLE `Purchases` (
    `Id`        INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `BookId`    INTEGER NOT NULL,
    `Seller`    TEXT NOT NULL,
    `Date`      INTEGER NOT NULL,
    `TotalCost` REAL NOT NULL,
    `Currency`  TEXT NOT NULL,
    `Comments`  TEXT,
    FOREIGN KEY (`Currency`) REFERENCES `Currencies` (`Code`),
    FOREIGN KEY (`BookId`) REFERENCES `Books` (`Id`)
);

CREATE TABLE `Incomes` (
    `Id`       INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `BookId`   INTEGER NOT NULL,
    `Source`   TEXT NOT NULL,
    `Date`     INTEGER NOT NULL,
    `Value`    REAL NOT NULL,
    `Currency` TEXT NOT NULL,
    `Comments` TEXT,
    FOREIGN KEY (`BookId`) REFERENCES `Books` (`Id`),
    FOREIGN KEY (`Currency`) REFERENCES `Currencies` (`Code`)
);

CREATE TABLE `PurchasesLog` (
    `PurchaseId` INTEGER NOT NULL,
    `TimeStamp`  INTEGER NOT NULL,
    `UserId`     INTEGER NOT NULL,
    PRIMARY KEY (`PurchaseId` ASC, `TimeStamp` ASC),
    FOREIGN KEY (`PurchaseId`) REFERENCES `Purchases` (`Id`),
    FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`)
) WITHOUT ROWID;

CREATE TABLE `IncomesLog` (
    `IncomeId`	INTEGER NOT NULL,
    `TimeStamp`	INTEGER NOT NULL,
    `UserId`	INTEGER NOT NULL,
    PRIMARY KEY (`IncomeId` ASC, `TimeStamp` ASC),
    FOREIGN KEY (`IncomeId`) REFERENCES `Incomes` (`Id`),
    FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`)
) WITHOUT ROWID;

CREATE TABLE `PurchaseItems` (
    `Id`         INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    `PurchaseId` INTEGER NOT NULL,
    `Name`       TEXT NOT NULL,
    `Price`      REAL NOT NULL,
    `Count`      INTEGER NOT NULL,
    FOREIGN KEY (`PurchaseId`) REFERENCES `Purchases` (`Id`)
);

CREATE TABLE `PurchaseTags` (
    `PurchaseItemId` INTEGER NOT NULL,
    `Tag`            TEXT NOT NULL,
    PRIMARY KEY (`PurchaseItemId` ASC, `Tag` ASC),
    FOREIGN KEY (`PurchaseItemId`) REFERENCES `PurchaseItems` (`Id`)
) WITHOUT ROWID;

CREATE TABLE `IncomeTags` (
    `IncomeId` INTEGER NOT NULL,
    `Tag`      TEXT NOT NULL,
    PRIMARY KEY (`IncomeId` ASC, `Tag` ASC),
    FOREIGN KEY (`IncomeId`) REFERENCES `Incomes` (`Id`)
) WITHOUT ROWID;

CREATE TABLE `Log` (
	`TimeStamp`		INTEGER NOT NULL,
	`Description`	TEXT NOT NULL
);

INSERT INTO `Currencies` (`Code`, `Name`, `Format`)
VALUES
	('RUB', 'Russian Ruble', '#,###.## ₽'),
	('USD', 'US Dollar', '$#,###.##;($#,###.##)'),
	('EUR', 'Euro', '€#,###.##');