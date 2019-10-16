using System.Collections.Generic;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.CoreLogic.Contacts
{
	public interface IPlansLogic
	{
		Result<IReadOnlyCollection<Plan>> GetAll();
		Result<string> GetName(int planId);
		Result<Plan> Create(string name);
		Result Delete(int id);
	}
}