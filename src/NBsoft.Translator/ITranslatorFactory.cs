using System.IO;

namespace NBsoft.Translator
{
    public interface ITranslatorManager
    {
        void InitializeDictionaries(DirectoryInfo languageDir);
        ILanguage[] AvailableLanguages { get; }
        void CreateDictionary(string language, string base64PngImage = null);
        ITranslatorService GetTranslator(string language);
        void SaveDictionaryAsXaml(string language, string languageDir = null);
        void SaveDictionaryAsCsv(string language, string languageDir = null);
    }
}
