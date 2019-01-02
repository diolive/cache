using System.Threading.Tasks;

using DioLive.Cache.Models;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IApplicationUsersStorage
	{
		Task<ApplicationUser> GetWithOptionsAsync(string id);
		Task<ApplicationUser> GetByUserNameAsync(string userName);
		Task UpdateOptionsAsync(string userId, int? purchaseGrouping, bool? showPlanList);
	}
}