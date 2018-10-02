using System.Collections;

namespace NBsoft.Translator
{
    public interface ITranslatorService
    {
        ILanguage Language { get; }
        Hashtable Dictionary { get; }
        string Translate(string key);
        void AddKey(string key, string value);
        void RemoveKey(string key);
        
    }
}
