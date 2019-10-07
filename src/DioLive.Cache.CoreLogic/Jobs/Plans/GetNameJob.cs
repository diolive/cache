using System.Threading.Tasks;

using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Plans
{
	public class GetNameJob : Job<string>
	{
		private readonly int _planId;

		public GetNameJob(int planId)
		{
			_planId = planId;
		}

		protected override void Validation()
		{
			AssertUserIsAuthenticated();
		}

		protected override async Task<string> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return (await storageCollection.Plans.FindAsync(_planId)).Name;
		}
	}
}