using System.Threading.Tasks;

using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Plans
{
	[Authenticated]
	[HasAnyRights]
	public class GetNameJob : Job<string>
	{
		private readonly int _planId;

		public GetNameJob(int planId)
		{
			_planId = planId;
		}

		protected override async Task<string> ExecuteAsync()
		{
			IStorageCollection storageCollection = Settings.StorageCollection;

			return (await storageCollection.Plans.FindAsync(_planId)).Name;
		}
	}
}