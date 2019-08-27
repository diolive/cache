namespace DioLive.Cache.Storage.SqlServer
{
	internal static class Queries
	{
		internal static class Options
		{
			internal static string Select = "SELECT * FROM dbo.[Options] WHERE UserId=@UserId";
			internal static string Insert = "INSERT INTO dbo.[Options] (UserId, PurchaseGrouping, ShowPlanList) VALUES (@UserId, @PurchaseGrouping, @ShowPlanList)";
			internal static string Update = "UPDATE dbo.[Options] SET PurchaseGrouping=@PurchaseGrouping, ShowPlanList=@ShowPlanList WHERE UserId=@UserId";
		}

		internal static class Plans
		{
			internal static string Select = "SELECT * FROM dbo.[Plan] WHERE Id=@Id AND BudgetId=@BudgetId";
			internal static string SelectAll = "SELECT * FROM dbo.[Plan] WHERE BudgetId=@BudgetId";
			internal static string Insert = "INSERT INTO dbo.[Plan] (AuthorId, BudgetId, Name) VALUES (@AuthorId, @BudgetId, @Name); SELECT scope_identity();";
			internal static string Buy = "UPDATE dbo.[Plan] SET BuyDate=@BuyDate, BuyerId=@BuyerId WHERE Id=@Id AND BudgetId=@BudgetId";
			internal static string Delete = "DELETE FROM dbo.[Plan] WHERE Id=@Id AND BudgetId=@BudgetId";
		}
	}
}