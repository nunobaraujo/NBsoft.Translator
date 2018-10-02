using System.Globalization;

namespace NBsoft.Translator
{
    public class Language : ILanguage
    {
        public Language()
        {
            Image = null;
            CultureInfo = null;
            DictionaryFile = null;
        }

        public Language(string flagImageFile, CultureInfo cultureInfo, string dictionaryFile)
            : this()
        {
            Image = flagImageFile;
            CultureInfo = cultureInfo;
            DictionaryFile = dictionaryFile;
        }

        public string Image { get; }
        public CultureInfo CultureInfo { get; }
        public string DictionaryFile { get; }
        public string Name => CultureInfo.Name;
        public string IsoName => CultureInfo.TwoLetterISOLanguageName;

        public override string ToString() => CultureInfo.NativeName;
        public override int GetHashCode() => CultureInfo.GetHashCode();
        public override bool Equals(object obj)
        {
            Language other = obj as Language;
            return CultureInfo.Equals(other.CultureInfo);
        }

    }
}
