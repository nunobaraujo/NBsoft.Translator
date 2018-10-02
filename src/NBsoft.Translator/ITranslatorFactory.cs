namespace NBsoft.Translator
{
    public interface ITranslatorFactory
    {
        ITranslatorManager CreateManager(string languageDir);
    }
}
