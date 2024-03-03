using BSP.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BSP.ViewModels
{
    public class LanguageVM
    {
        public LanguageVM()
        {
            AvailableLanguages = new(Load(App.Languages, App.Language));
        }

        public ObservableCollection<MenuItem> AvailableLanguages { get; private set; }

        private RelayCommand clickLanguageCommand;
        public RelayCommand ClickLanguageCommand => clickLanguageCommand ?? (clickLanguageCommand = new RelayCommand(culture => ChangeLanguage(culture as CultureInfo)));


        public List<MenuItem> Load(List<CultureInfo> cultures, CultureInfo appCulture)
        {
            return cultures.Select(c => new MenuItem { Header = c.Name.ToString(), IsChecked = c.Equals(appCulture), Command = ClickLanguageCommand, CommandParameter = c }).ToList();
        }

        public void ChangeLanguage(CultureInfo culture)
        {
            MessageBox.Show((Application.Current.TryFindResource("msg_RestartApplication") as string) ?? "To apply all changes, restart this application!");

            App.Language = culture;
            foreach (var lang in AvailableLanguages)
            {
                lang.IsChecked = (lang.Header as string) == culture.Name;
            }

        }
    }
}
