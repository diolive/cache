using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.CoreLogic.Attributes;

public class HasAnyRightsAttribute() : HasRightsAttribute(ShareAccess.ReadOnly)
{
}