using System.Collections;

namespace NBsoft.Translator
{
    internal class TranslatorService : ITranslatorService
    {
        public ILanguage Language { get; }
        public Hashtable Dictionary { get; }

        public TranslatorService(ILanguage language, Hashtable hashTable)
        {
            Language = language;
            Dictionary = hashTable;
        }        

        public void AddKey(string key, string value)
        {
            if (!Dictionary.ContainsKey(key))
                Dictionary.Add(key, value);
            else
                throw new System.ArgumentException($"Key [{key}] already exists in dictionary");
        }
        public void RemoveKey(string key)
        {
            if (Dictionary.ContainsKey(key))
                Dictionary.Remove(key);
            else
                throw new System.ArgumentException($"Key [{key}] is not present in dictionary");
        }

        public string Translate(string key)
        {
            if (key == null)
                return "";
            if (!Dictionary.ContainsKey(key))
                return key;
            return Dictionary[key].ToString();
        }

    }
}
