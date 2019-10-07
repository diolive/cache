namespace DioLive.Cache.Common.Localization
{
	public interface IPluralizer
	{
		string Language { get; }

		string Pluralize(int number);
	}
}