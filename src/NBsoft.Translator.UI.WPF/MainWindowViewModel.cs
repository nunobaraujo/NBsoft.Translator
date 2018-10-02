using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace NBsoft.Translator.UI.WPF
{
    class MainWindowViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<string> allKeys;

        public ITranslatorManager TranslatorManager { get; private set; }
        public bool CanChangeFolder => TranslatorManager == null;
        public string ChosenDirectory { get; set; }
        public DataTable Table { get; set; }

        public MainWindowViewModel()
        {
            
        }        

        public void InitializeTranslatorManager(string languageResourceDirectory)
        {
            TranslatorManager = null;
            var di = new System.IO.DirectoryInfo(languageResourceDirectory);
            if (!di.Exists)
                throw new ApplicationException("Invalid Directory");

            TranslatorManager = new TranslatorFactory().CreateManager(di.FullName);
            OnPropertyChanged(nameof(CanChangeFolder));

            BuildUI();
        }

        private void BuildUI()
        {
            Table = new DataTable();
            var keyColumn = Table.Columns.Add("Key");
            keyColumn.ReadOnly = true;

            allKeys = new List<string>();
            foreach (var language in TranslatorManager.AvailableLanguages)
            {   
                var translator = TranslatorManager.GetTranslator(language.Name);
                Table.Columns.Add(translator.Language.Name);
                foreach (var key in translator.Dictionary.Keys)
                {
                    if (!allKeys.Contains(key.ToString()))
                    {
                        allKeys.Add(key.ToString());
                    }
                }
            }
            allKeys.Sort();

            foreach (var key in allKeys)
            {
                var cols = new List<string>
                {
                    key
                };
                foreach (var language in TranslatorManager.AvailableLanguages)
                {
                    string languageValue = null;
                    var translator = TranslatorManager.GetTranslator(language.Name);
                    if (translator.Dictionary.ContainsKey(key))
                        languageValue = translator.Dictionary[key].ToString();
                    cols.Add(languageValue);
                }
                var row = Table.Rows.Add(cols.ToArray());
            }
        }


        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
