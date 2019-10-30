using System.Threading.Tasks;

using DioLive.Cache.CoreLogic.Attributes;

namespace DioLive.Cache.CoreLogic.Jobs.Users
{
	[Authenticated]
	public class FindIdJob : Job<string?>
	{
		private readonly string _name;

		public FindIdJob(string name)
		{
			_name = name;
		}

		protected override async Task<string?> ExecuteAsync()
		{
			return await Settings.StorageCollection.Users.FindIdByNameAsync(_name);
		}
	}
}