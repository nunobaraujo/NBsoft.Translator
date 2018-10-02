using System;

namespace NBsoft.Translator
{
    public class TranslatorFactory : ITranslatorFactory
    {
        public ITranslatorManager CreateManager(string languageResourceDirectory)
        {
            var result = new TranslatorManager();
            var di = new System.IO.DirectoryInfo(languageResourceDirectory);
            if (!di.Exists)
                throw new ArgumentException($"Resource Directory doesn't exist [{languageResourceDirectory}]");
            result.InitializeDictionaries(new System.IO.DirectoryInfo(languageResourceDirectory));
            return result;
        }
    }
}
