using DioRed.Cache.Domain;

using Microsoft.Data.SqlClient;

namespace DioRed.Cache.Infrastructure.Data;

public abstract class StorageBase(
    IConnectionInfo connectionInfo,
    ICurrentContext currentContext
) : IDisposable
{
    protected string? CurrentUserId { get; } = currentContext.GetUserId();

    protected SqlConnection Connection { get; } = new(connectionInfo.ConnectionString);

    #region IDisposable implementation
    private bool _isDisposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            Connection?.Dispose();
        }

        _isDisposed = true;
    }

    ~StorageBase()
    {
        Dispose(false);
    }
    #endregion
}