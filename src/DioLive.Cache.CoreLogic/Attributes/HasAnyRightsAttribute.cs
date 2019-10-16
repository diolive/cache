using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.CoreLogic.Attributes
{
	public class HasAnyRightsAttribute : HasRightsAttribute
	{
		public HasAnyRightsAttribute()
			: base(ShareAccess.ReadOnly)
		{
		}
	}
}