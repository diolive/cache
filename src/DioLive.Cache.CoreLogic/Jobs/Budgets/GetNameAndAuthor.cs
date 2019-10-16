using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Budgets
{
	[Authenticated]
	[HasAnyRights]
	public class GetNameAndAuthorJob : Job<(string name, string authorId)>
	{
		protected override async Task<(string name, string authorId)> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;
			Budget budget = await storageCollection.Budgets.GetAsync(CurrentBudget);

			return (name: budget.Name, authorId: budget.AuthorId);
		}
	}
}