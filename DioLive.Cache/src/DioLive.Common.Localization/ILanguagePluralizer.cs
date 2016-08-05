namespace DioLive.Common.Localization
{
    public interface ILanguagePluralizer
    {
        string Language { get; }

        string Pluralize(int number);
    }
}