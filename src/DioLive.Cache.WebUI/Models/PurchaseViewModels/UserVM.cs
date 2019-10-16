namespace DioLive.Cache.WebUI.Models.PurchaseViewModels
{
	public class UserVM
	{
		public UserVM()
		{
		}

		public UserVM(string id, string name)
		{
			Id = id;
			Name = name;
		}

		public string Id { get; set; } = default!;

		public string Name { get; set; } = default!;
	}
}