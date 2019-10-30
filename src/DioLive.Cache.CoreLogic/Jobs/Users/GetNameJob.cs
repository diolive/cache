using System.Threading.Tasks;

using DioLive.Cache.CoreLogic.Attributes;

namespace DioLive.Cache.CoreLogic.Jobs.Users
{
	[Authenticated]
	public class GetNameJob : Job<string?>
	{
		private readonly string _id;

		public GetNameJob(string id)
		{
			_id = id;
		}

		protected override async Task<string?> ExecuteAsync()
		{
			return await Settings.StorageCollection.Users.GetNameByIdAsync(_id);
		}
	}
}