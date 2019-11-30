using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	[Authenticated]
	[HasAnyRights]
	public class GetNameAndAuthorJob : Job<(string name, string authorName)>
	{
		protected override async Task<(string name, string authorName)> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;
			Budget budget = await storageCollection.Budgets.GetAsync(CurrentBudget);
			string author = await storageCollection.Users.GetNameByIdAsync(budget.AuthorId) ?? string.Empty;

			return (name: budget.Name, authorName: author);
		}
	}
}