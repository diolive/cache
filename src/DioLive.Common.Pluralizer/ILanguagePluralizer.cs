namespace DioLive.Common.Pluralizer
{
    public interface ILanguagePluralizer
    {
        string Language { get; }

        string Pluralize(int number);
    }
}
