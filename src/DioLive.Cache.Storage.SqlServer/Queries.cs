﻿namespace DioLive.Cache.Storage.SqlServer
{
	internal static class Queries
	{
		internal static class Budgets
		{
			internal const string CheckRights = @"
SELECT CASE 
		WHEN NOT EXISTS (
				SELECT 1
				FROM dbo.Budget
				WHERE Id = @BudgetId
				)
			THEN 0
		WHEN (
				SELECT AuthorId
				FROM dbo.Budget
				WHERE Id = @BudgetId
				) = @UserId
			OR (
				SELECT Access
				FROM dbo.Share
				WHERE BudgetId = @BudgetId
					AND UserId = @UserId
				) & @Access = @Access
			THEN 1
		ELSE 2
		END
";

			internal const string Delete = @"
DELETE
FROM dbo.[Budget]
WHERE Id = @Id
";

			internal const string GetCurrency = @"
SELECT TOP 1 c.[Sign]
FROM dbo.[Currency] c
INNER JOIN dbo.[Budget] b ON (
		b.[CurrencyId] = c.[Id]
		AND b.Id = @Id
		)
";

			internal const string GetShares = @"
SELECT u.Name AS [UserName]
	,s.Access
FROM dbo.[Share] s
INNER JOIN dbo.[Users] u ON s.UserId = u.Id
WHERE BudgetId = @BudgetId
";

			internal const string GetVersion = @"
SELECT TOP 1 Version
FROM dbo.[Budget]
WHERE Id = @Id
";

			internal const string Insert = @"
INSERT INTO dbo.[Budget] (
	Id
	,AuthorId
	,Name
	,CurrencyId
	)
VALUES (
	@Id
	,@AuthorId
	,@Name
	,@CurrencyId
	)
";

			internal const string Rename = @"
UPDATE dbo.[Budget]
SET [Name] = @Name
WHERE Id = @Id
";

			internal const string Select = @"
SELECT TOP 1 *
FROM dbo.[Budget]
WHERE Id = @Id
";

			internal const string SelectAvailable = @"
SELECT DISTINCT b.*
FROM dbo.[Budget] b
INNER JOIN dbo.[Share] s ON (
		b.AuthorId = @UserId
		OR b.[Id] = s.[BudgetId]
		)
";

			internal const string SetVersion = @"
UPDATE dbo.[Budget]
SET Version = @Version
WHERE Id = @Id
";

			internal const string Share = @"
IF EXISTS (
		SELECT 1
		FROM dbo.[Share]
		WHERE BudgetId = @BudgetId
			AND UserId = @UserId
		)
	UPDATE dbo.[Share]
	SET Access = @Access
	WHERE BudgetId = @BudgetId
		AND UserId = @UserId
ELSE
	INSERT INTO dbo.[Share] (
		BudgetId
		,UserId
		,Access
		)
	VALUES (
		@BudgetId
		,@UserId
		,@Access
		)
";
		}

		internal static class Categories
		{
			internal const string CheckRights = @"
SELECT CASE 
		WHEN NOT EXISTS (
				SELECT 1
				FROM dbo.Category
				WHERE Id = @CategoryId
				)
			THEN 0
		WHEN (
				SELECT b.AuthorId
				FROM dbo.Budget b
				INNER JOIN dbo.Category c ON b.Id = c.BudgetId
				WHERE c.Id = @CategoryId
				) = @UserId
			OR (
				SELECT s.Access
				FROM dbo.Share s
				INNER JOIN dbo.Category c ON s.BudgetId = c.BudgetId
				WHERE c.Id = @CategoryId
					AND s.UserId = @UserId
				) & @Access = @Access
			OR (
				(
					SELECT TOP 1 OwnerId
					FROM dbo.Category
					WHERE Id = @CategoryId
					) IS NULL
				AND @Access = 0
				)
			THEN 1
		ELSE 2
		END
";

			internal const string Clone = @"
INSERT INTO dbo.[Category] (
	[Name]
	,OwnerId
	,BudgetId
	,Color
	,ParentId
	)
SELECT [Name]
	,@OwnerId
	,@BudgetId
	,Color
	,@ParentId
FROM dbo.Category
WHERE Id = @Id;

SELECT scope_identity();
";

			internal const string Delete = @"
DELETE
FROM dbo.[Category]
WHERE Id = @Id
";

			internal const string GetLatest = @"
SELECT TOP 1 CategoryId
FROM dbo.[Purchase]
WHERE BudgetId = @BudgetId
	AND Name = @Name
ORDER BY [Date] DESC
";

			internal const string GetWithTotals = @"
DECLARE @DateFrom datetime = DATEADD(day, -@Days, GETDATE());

SELECT x.Id
	,c.Name
	,RIGHT('00000' + FORMAT(c.Color, 'X'), 6) as Color
	,x.TotalCost
FROM (
	SELECT CategoryId as Id
		,SUM(Cost) as TotalCost
	FROM dbo.[Purchase]
	WHERE BudgetId = @BudgetId
		AND Cost > 0
		AND (@Days = 0 OR [Date] >= @DateFrom)
	GROUP BY CategoryId
	) x
INNER JOIN dbo.[Category] c
	ON x.Id = c.Id";

			internal const string Insert = @"
INSERT INTO dbo.[Category] (
	Name
	,OwnerId
	,BudgetId
	,Color
	,ParentId
	)
VALUES (
	@Name
	,@OwnerId
	,@BudgetId
	,@Color
	,@ParentId
	);

SELECT scope_identity()
";

			internal const string Select = @"
SELECT TOP 1 *
FROM dbo.[Category]
WHERE Id = @Id
";

			internal const string SelectAll = @"
SELECT *
FROM dbo.[Category]
WHERE BudgetId = @BudgetId
";

			internal const string SelectAllRoots = @"
SELECT *
FROM dbo.[Category]
WHERE BudgetId = @BudgetId
	AND ParentId IS NULL
";

			internal const string SelectCommon = @"
SELECT *
FROM dbo.[Category]
WHERE OwnerId IS NULL
";

			internal const string SelectMostPopularId = @"
SELECT TOP 1 CategoryId
FROM (
	SELECT CategoryId
		,COUNT(*) AS [PurchaseCount]
	FROM dbo.[Purchase]
	WHERE BudgetId = @BudgetId
	GROUP BY CategoryId
	) x
ORDER BY PurchaseCount DESC
";

			internal const string Update = @"
UPDATE dbo.[Category]
SET Name = @Name
	,ParentId = @ParentId
	,Color = CONVERT(INT, CONVERT(VARBINARY, @Color, 2))
WHERE Id = @Id
";
		}

		internal static class Currencies
		{
			internal const string SelectAll = @"
SELECT *
FROM dbo.[Currency]
";
		}

		internal static class Options
		{
			internal const string Insert = @"
INSERT INTO dbo.[Options] (
	UserId
	,PurchaseGrouping
	,ShowPlanList
	)
VALUES (
	@UserId
	,@PurchaseGrouping
	,@ShowPlanList
	)
";

			internal const string Select = @"
SELECT TOP 1 *
FROM dbo.[Options]
WHERE UserId = @UserId
";

			internal const string Update = @"
UPDATE dbo.[Options]
SET PurchaseGrouping = @PurchaseGrouping
	,ShowPlanList = @ShowPlanList
WHERE UserId = @UserId
";
		}

		internal static class Plans
		{
			internal const string Buy = @"
UPDATE dbo.[Plan]
SET BuyDate = @BuyDate
	,BuyerId = @BuyerId
WHERE Id = @Id
";

			internal const string Delete = @"
DELETE
FROM dbo.[Plan]
WHERE Id = @Id
";

			internal const string Insert = @"
INSERT INTO dbo.[Plan] (
	AuthorId
	,BudgetId
	,Name
	)
VALUES (
	@AuthorId
	,@BudgetId
	,@Name
	);

SELECT scope_identity();
";

			internal const string Select = @"
SELECT TOP 1 *
FROM dbo.[Plan]
WHERE Id = @Id
";

			internal const string SelectAll = @"
SELECT *
FROM dbo.[Plan]
WHERE BudgetId = @BudgetId
";
		}

		internal static class Purchases
		{
			internal const string CheckRights = @"
SELECT CASE 
		WHEN NOT EXISTS (
				SELECT 1
				FROM dbo.[Purchase]
				WHERE Id = @PurchaseId
				)
			THEN 0
		WHEN (
				SELECT b.AuthorId
				FROM dbo.[Budget] b
				INNER JOIN dbo.[Purchase] p ON b.Id = p.BudgetId
				WHERE p.Id = @PurchaseId
				) = @UserId
			OR (
				SELECT s.Access
				FROM dbo.[Share] s
				INNER JOIN dbo.[Purchase] p ON s.BudgetId = p.BudgetId
				WHERE p.Id = @PurchaseId
					AND s.UserId = @UserId
				) & @Access = @Access
			THEN 1
		ELSE 2
		END
";

			internal const string Delete = @"
DELETE
FROM dbo.[Purchase]
WHERE Id = @Id
";

			internal const string GetNames = @"
SELECT DISTINCT Name
FROM dbo.[Purchase]
WHERE BudgetId = @BudgetId
	AND Name LIKE @NameFilter
ORDER BY Name
";

			internal const string GetShops = @"
SELECT DISTINCT Shop
FROM dbo.[Purchase]
WHERE BudgetId = @BudgetId
	AND Shop IS NOT NULL
ORDER BY Shop
";

			internal const string Insert = @"
INSERT INTO dbo.[Purchase] (
	Id
	,CategoryId
	,Date
	,Name
	,Cost
	,Shop
	,AuthorId
	,Comments
	,CreateDate
	,BudgetId
	)
VALUES (
	@Id
	,@CategoryId
	,@Date
	,@Name
	,@Cost
	,@Shop
	,@AuthorId
	,@Comments
	,@CreateDate
	,@BudgetId
	)
";

			internal const string Select = @"
SELECT TOP 1 *
FROM dbo.[Purchase]
WHERE Id = @Id
";

			internal const string SelectAll = @"
SELECT *
FROM dbo.[Purchase]
WHERE BudgetId = @BudgetId
	AND Name LIKE @NameFilter
ORDER BY [Date] DESC
	,CreateDate ASC
";

			internal const string SelectForStat = @"
SELECT *
FROM dbo.[Purchase]
WHERE BudgetId = @BudgetId
	AND Cost > 0
	AND [Date] >= @DateFrom
	AND [Date] < @DateTo
ORDER BY [Date] DESC
	,CreateDate ASC
";

			internal const string Update = @"
UPDATE dbo.[Purchase]
SET CategoryId = @CategoryId
	,Date = @Date
	,Name = @Name
	,Cost = @Cost
	,Shop = @Shop
	,Comments = @Comments
	,LastEditorId = @LastEditorId
WHERE Id = @Id
";

			internal const string UpdateCategory = @"
UPDATE dbo.[Purchase]
SET CategoryId = @NewCategoryId
WHERE CategoryId = @OldCategoryId
	AND BudgetId = @BudgetId
";
		}

		internal static class Users
		{
			internal const string FindIdByName = @"
SELECT TOP 1 [Id]
FROM dbo.[Users]
WHERE [Name] = @Name
";

			internal const string GetNameById = @"
SELECT TOP 1 [Name]
FROM dbo.[Users]
WHERE Id = @Id
";

			internal const string Insert = @"
INSERT INTO dbo.[Users] (
	Id
	,Name
	)
VALUES (
	@Id
	,@Name
	);
";
		}
	}
}