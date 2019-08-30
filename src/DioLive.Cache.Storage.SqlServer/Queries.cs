namespace DioLive.Cache.Storage.SqlServer
{
	internal static class Queries
	{
		internal static class Options
		{
			internal const string Select = "SELECT TOP 1 * FROM dbo.[Options] WHERE UserId=@UserId";
			internal const string Insert = "INSERT INTO dbo.[Options] (UserId, PurchaseGrouping, ShowPlanList) VALUES (@UserId, @PurchaseGrouping, @ShowPlanList)";
			internal const string Update = "UPDATE dbo.[Options] SET PurchaseGrouping=@PurchaseGrouping, ShowPlanList=@ShowPlanList WHERE UserId=@UserId";
		}

		internal static class Plans
		{
			internal const string Select = "SELECT TOP 1 * FROM dbo.[Plan] WHERE Id=@Id AND BudgetId=@BudgetId";
			internal const string SelectAll = "SELECT * FROM dbo.[Plan] WHERE BudgetId=@BudgetId";
			internal const string Insert = "INSERT INTO dbo.[Plan] (AuthorId, BudgetId, Name) VALUES (@AuthorId, @BudgetId, @Name); SELECT scope_identity();";
			internal const string Buy = "UPDATE dbo.[Plan] SET BuyDate=@BuyDate, BuyerId=@BuyerId WHERE Id=@Id AND BudgetId=@BudgetId";
			internal const string Delete = "DELETE FROM dbo.[Plan] WHERE Id=@Id AND BudgetId=@BudgetId";
		}

		internal static class Budgets
		{
			internal const string Select = "SELECT TOP 1 * FROM dbo.[Budget] WHERE Id=@Id";
			internal const string SelectAvailable = "SELECT DISTINCT b.* FROM dbo.[Budget] b LEFT JOIN dbo.[Share] s ON (b.AuthorId=@UserId OR b.[Id]=s.[BudgetId])";
			internal const string CheckRights = "SELECT CASE WHEN NOT EXISTS(SELECT 1 FROM dbo.Budget WHERE Id=@BudgetId) THEN 0 WHEN (SELECT AuthorId FROM dbo.Budget WHERE Id=@BudgetId)=@UserId OR (SELECT Access FROM dbo.Share WHERE BudgetId=@BudgetId AND UserId=@UserId) & @Access = @Access THEN 1 ELSE 2 END";
			internal const string Insert = "INSERT INTO dbo.[Budget] (Id, AuthorId, Name, Version) VALUES (@Id, @AuthorId, @Name, @Version)";
			internal const string Rename = "UPDATE dbo.[Budget] SET [Name]=@Name WHERE Id=@Id";
			internal const string Delete = "DELETE FROM dbo.[Budget] WHERE Id=@Id";
			internal const string Share = "IF EXISTS (SELECT 1 FROM dbo.[Share] WHERE BudgetId=@BudgetId AND UserId=@UserId) UPDATE dbo.[Share] SET Access=@Access WHERE BudgetId=@BudgetId AND UserId=@UserId ELSE INSERT INTO dbo.[Share] (BudgetId, UserId, Access) VALUES (@BudgetId, @UserId, @Access)";
			internal const string GetVersion = "SELECT TOP 1 Version FROM dbo.[Budget] WHERE BudgetId=@BudgetId";
			internal const string SetVersion = "UPDATE dbo.[Budget] SET Version=@Version WHERE BudgetId=@BudgetId";
			internal const string GetShares = "SELECT * FROM dbo.[Share] WHERE BudgetId=@BudgetId";
		}

		internal static class Categories
		{
			internal const string SelectCommon = "SELECT * FROM dbo.[Category] WHERE OwnerId IS NULL";
			internal const string Insert = "INSERT INTO dbo.[Category] (Name, OwnerId, BudgetId, Color, ParentId) VALUES (@Name, @OwnerId, @BudgetId, @Color, @ParentId); SELECT scope_identity()";
		}

		internal static class Purchases
		{
			internal const string UpdateCategory = "UPDATE dbo.[Purchase] SET CategoryId=@NewCategoryId WHERE CategoryId=@OldCategoryId AND BudgetId=@BudgetId";
		}
	}
}