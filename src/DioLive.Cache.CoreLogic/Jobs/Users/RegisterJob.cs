using System.Threading.Tasks;

using DioLive.Cache.CoreLogic.Attributes;

namespace DioLive.Cache.CoreLogic.Jobs.Users
{
	[Authenticated]
	public class RegisterJob : Job
	{
		private readonly string _userId;
		private readonly string _userName;

		public RegisterJob(string userId, string userName)
		{
			_userId = userId;
			_userName = userName;
		}

		protected override async Task ExecuteAsync()
		{
			await Settings.StorageCollection.Users.AddAsync(_userId, _userName);
		}
	}
}