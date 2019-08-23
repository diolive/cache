namespace DioLive.Common.Localization
{
	public interface IPluralizer
	{
		string Language { get; }

		string Pluralize(int number);
	}
}