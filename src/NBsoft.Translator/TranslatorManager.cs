using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace NBsoft.Translator
{
    internal class TranslatorManager : ITranslatorManager
    {
        private string _resourceDirectory;
        private Dictionary<string, Hashtable> _dictionaries;

        public ILanguage[] AvailableLanguages { get; private set; }

        public void CreateDictionary(string language, string base64PngImage = null)
        {
            if (AvailableLanguages.Count(x => x.Name == language) > 0)
                throw new InvalidOperationException($"Dictionary already exists: [{language}]");

            var cinfo = new CultureInfo(language);
            var xmlFile = new FileInfo(Path.Combine(_resourceDirectory, $"{cinfo.Name}.xaml"));
            if (xmlFile.Exists)
                throw new InvalidOperationException($"Cannot create dictionary, xaml file already exists: [{xmlFile.FullName}]");
            if (base64PngImage != null)
            {
                var flagImageFile = new FileInfo(xmlFile.FullName.Replace(xmlFile.Extension, ".png"));
                File.WriteAllBytes(flagImageFile.FullName, Convert.FromBase64String(base64PngImage));
            }
            var newLanguage = new Language(base64PngImage, cinfo, xmlFile.FullName);
            var languages = AvailableLanguages.ToList();
            languages.Add(newLanguage);
            _dictionaries.Add(newLanguage.CultureInfo.Name, LoadDictionary(newLanguage.DictionaryFile));
            AvailableLanguages = languages.ToArray();
        }

        public ITranslatorService GetTranslator(string language)
        {
            ILanguage selected = null;
            foreach (var item in AvailableLanguages)
            {
                if (item.Name.ToLowerInvariant() == language.ToLowerInvariant()
                    || item.IsoName.ToLowerInvariant() == language.ToLowerInvariant())
                {
                    selected = item;
                    break;
                }
            }

            if (selected == null)
                throw new ArgumentOutOfRangeException(nameof(language), "Invalid language");
            return new TranslatorService(selected, _dictionaries[selected.CultureInfo.Name]);
        }

        public void InitializeDictionaries(DirectoryInfo languageDir)
        {
            _resourceDirectory = languageDir.FullName;
            var languages = new List<ILanguage>();
            var Di = new DirectoryInfo(_resourceDirectory);
            _dictionaries = new Dictionary<string, Hashtable>();
            foreach (var xmlfile in Di.GetFiles("*.xaml"))
            {
                var flagImageFile = new FileInfo(xmlfile.FullName.Replace(xmlfile.Extension, ".png"));
                string flagimage = null;
                if (flagImageFile.Exists)
                    flagimage = Convert.ToBase64String(File.ReadAllBytes(flagImageFile.FullName));

                CultureInfo cinfo;
                try { cinfo = new CultureInfo(xmlfile.Name.Replace(xmlfile.Extension, "")); }
                catch { cinfo = CultureInfo.InvariantCulture; }

                var language = new Language(flagimage, cinfo, xmlfile.FullName);
                languages.Add(language);
                _dictionaries.Add(language.CultureInfo.Name, LoadDictionary(language.DictionaryFile));
            }
            AvailableLanguages = languages.ToArray();
        }

        public void SaveDictionaryAsXaml(string language, string languageDir = null)
        {
            string destination = languageDir ?? _resourceDirectory;
            var translator = GetTranslator(language);
            var pathFile = Path.Combine(destination, $"{translator.Language.CultureInfo.Name}.xaml");
            using (var tw = new StreamWriter(pathFile,false, Encoding.UTF8))
            {
                tw.WriteLine("<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:system=\"clr-namespace:System;assembly=mscorlib\">");
                foreach (DictionaryEntry item in translator.Dictionary)
                {
                    tw.WriteLine(string.Format("<system:String xml:space=\"preserve\" x:Key=\"{0}\">{1}</system:String>", item.Key.ToString(), item.Value?.ToString()));
                }
                tw.WriteLine("</ResourceDictionary>");
                tw.Close();
            }
        }
        public void SaveDictionaryAsCsv(string language, string languageDir = null)
        {
            string destination = languageDir ?? _resourceDirectory;
            var translator = GetTranslator(language);
            var pathFile = Path.Combine(destination, $"{translator.Language.CultureInfo.Name}.xaml");
            using (var tw = new StreamWriter(pathFile, false, Encoding.UTF8))
            {
                tw.WriteLine("key;value");
                foreach (DictionaryEntry item in translator.Dictionary)
                {
                    tw.WriteLine($"{item.Key};{item.Value?.ToString().Replace(";", ".")}");
                }
                tw.Close();
            }
        }

        private Hashtable LoadDictionary(string dictionaryFile)
        {
            var hashtable = new Hashtable();
            System.Xml.XmlDocument XDoc = new System.Xml.XmlDocument();
            XDoc.Load(dictionaryFile);

            foreach (System.Xml.XmlNode XNode in XDoc.DocumentElement.ChildNodes)
            {
                if (XNode == null || XNode.NodeType == System.Xml.XmlNodeType.Comment || XNode.Attributes == null)
                    continue;
                string id = XNode.Attributes["x:Key"].Value;
                string value = XNode.InnerText;
                hashtable.Add(id, value);
            }
            return hashtable;
        }
    }
}
