namespace DioRed.Cache.Domain.Localization;

public interface IPluralizer
{
    string Language { get; }
    string Pluralize(int number);
}