using System;

using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs
{
	public class JobSettings
	{
		private static JobSettings? _default;

		public JobSettings(IPermissionsValidator permissionsValidator,
		                   IStorageCollection storageCollection)
		{
			PermissionsValidator = permissionsValidator;
			StorageCollection = storageCollection;
		}

		public JobSettings(IPermissionsValidator permissionsValidator,
		                   IStorageCollection storageCollection,
		                   bool useAttributeValidation)
		{
			PermissionsValidator = permissionsValidator;
			StorageCollection = storageCollection;
			UseAttributeValidation = useAttributeValidation;
		}

		public IPermissionsValidator PermissionsValidator { get; }

		public IStorageCollection StorageCollection { get; }
		public bool UseAttributeValidation { get; } = true;

		public static JobSettings Default => _default ?? throw new InvalidOperationException("Default job settings was not initialized");

		public static void ConfigureDefault(IPermissionsValidator permissionsValidator, IStorageCollection storageCollection)
		{
			_default = new JobSettings(permissionsValidator, storageCollection);
		}
	}
}