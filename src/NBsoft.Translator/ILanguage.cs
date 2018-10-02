using System.Globalization;

namespace NBsoft.Translator
{
    public interface ILanguage
    {
        string Image { get; }
        string Name { get; }
        string IsoName { get; }
        CultureInfo CultureInfo { get; }
        string DictionaryFile { get; }
    }
}
