using System;

namespace DioLive.Cache.Storage.Contracts
{
	public interface ICurrentContext
	{
		string UserId { get; }
	}
}