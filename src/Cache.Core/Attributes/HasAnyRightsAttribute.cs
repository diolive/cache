using DioRed.Cache.Domain.Entities;

namespace DioRed.Cache.Core.Attributes;

public class HasAnyRightsAttribute() : HasRightsAttribute(ShareAccess.ReadOnly)
{
}