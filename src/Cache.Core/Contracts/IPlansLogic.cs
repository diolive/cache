using DioRed.Cache.Domain.Entities;

using DioRed.Common;

namespace DioRed.Cache.Core.Contracts;

public interface IPlansLogic
{
    Result<IReadOnlyCollection<Plan>> GetAll();
    Result<string> GetName(int planId);
    Result<Plan> Create(string name);
    Result Delete(int id);
}