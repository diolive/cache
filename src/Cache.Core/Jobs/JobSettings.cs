using DioRed.Cache.Domain.Repositories;

namespace DioRed.Cache.Core.Jobs;

public class JobSettings(
    IPermissionsValidator permissionsValidator,
    IStorageCollection storageCollection,
    bool useAttributeValidation = true
)
{
    private static JobSettings? _default;

    public IPermissionsValidator PermissionsValidator { get; } = permissionsValidator;
    public IStorageCollection StorageCollection { get; } = storageCollection;
    public bool UseAttributeValidation { get; } = useAttributeValidation;

    public static JobSettings Default => _default
        ?? throw new InvalidOperationException("Default job settings was not initialized");

    public static void ConfigureDefault(
        IPermissionsValidator permissionsValidator,
        IStorageCollection storageCollection
    )
    {
        _default = new JobSettings(permissionsValidator, storageCollection);
    }
}