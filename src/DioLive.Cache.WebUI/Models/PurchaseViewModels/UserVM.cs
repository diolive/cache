using DioLive.Cache.Storage.Legacy.Models;

namespace DioLive.Cache.WebUI.Models.PurchaseViewModels
{
	public class UserVM
	{
		public UserVM()
		{
		}

		public UserVM(ApplicationUser applicationUser)
		{
			Id = applicationUser.Id;
			Name = applicationUser.UserName;
		}

		public string Id { get; set; }

		public string Name { get; set; }
	}
}