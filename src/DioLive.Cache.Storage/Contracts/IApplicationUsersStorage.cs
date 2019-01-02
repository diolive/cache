using System.Threading.Tasks;

using DioLive.Cache.Models;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IApplicationUsersStorage
	{
		Task<ApplicationUser> GetWithOptionsAsync();
		Task<ApplicationUser> GetByUserNameAsync(string userName);
		Task UpdateOptionsAsync(int? purchaseGrouping, bool? showPlanList);
	}
}