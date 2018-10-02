using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NBsoft.Translator.UI.WPF
{
    class MainWindowViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ITranslatorManager TranslatorManager { get; private set; }
        public bool CanChangeFolder => TranslatorManager == null;
        public string ChosenDirectory { get; set; }

        

        public void InitializeTranslatorManager(string languageResourceDirectory)
        {
            TranslatorManager = null;
            var di = new System.IO.DirectoryInfo(languageResourceDirectory);
            if (!di.Exists)
                throw new ApplicationException("Invalid Directory");

            TranslatorManager = new TranslatorFactory().CreateManager(di.FullName);
            OnPropertyChanged(nameof(CanChangeFolder));
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
