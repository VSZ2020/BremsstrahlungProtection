using BSP.BL.Calculation;
using BSP.BL.Services;
using BSP.Data;
using BSP.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows;

namespace BSP
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static List<CultureInfo> m_Languages = new List<CultureInfo>();
        private static ServiceProvider _provider;
        private const string languagesDirectory = "pack://application:,,,/Resources/Languages/";

        public static List<CultureInfo> Languages => m_Languages;

        public App()
        {
            InitializeComponent();

            m_Languages.Clear();
            m_Languages.Add(new CultureInfo("en-US")); //Нейтральная культура для этого проекта
            m_Languages.Add(new CultureInfo("ru-RU"));

            var savedCulture = BSP.Properties.Settings.Default.AppCulture;
            if (ValidateLanguageDictionary(savedCulture))
                Language = savedCulture;
            else
            {
                m_Languages.Remove(m_Languages.Single(l => l.Name == savedCulture.Name));
                Language = new CultureInfo("en-US");
                //MessageBox.Show("Resource file for choosed application language is corrupted. Default 'en-US' language resource to be used.");
            }
        }

        private static void SaveLanguageSettings()
        {
            BSP.Properties.Settings.Default.AppCulture = Language;
            BSP.Properties.Settings.Default.Save();
        }

        public static CultureInfo Language
        {
            get => Thread.CurrentThread.CurrentUICulture;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (value.Name == Thread.CurrentThread.CurrentUICulture.Name) return;

                Thread.CurrentThread.CurrentUICulture = value;

                ResourceDictionary newLangDict = new ResourceDictionary();
                switch (value.Name)
                {
                    case "ru-RU":
                        newLangDict.Source = new Uri(string.Format(languagesDirectory + "lang.{0}.xaml", value.Name), UriKind.Absolute);
                        break;
                    default:
                        newLangDict.Source = new Uri(languagesDirectory + "lang.xaml", UriKind.Absolute);
                        break;
                }

                var oldLangDict = Current.Resources.MergedDictionaries.SingleOrDefault(d => d.Source.OriginalString.IndexOf("lang.") >= 0);
                if (oldLangDict != null) Current.Resources.MergedDictionaries.Remove(oldLangDict);
                Current.Resources.MergedDictionaries.Insert(0, newLangDict);

                SaveLanguageSettings();
            }
        }

        private void LoadServices()
        {

            var connectionString = "Data Source=Database.mdb";
            _provider = new ServiceCollection()
                .AddDbContext<DataContext>(options => options.UseSqlite(connectionString))
                .AddTransient<BuildupService>()
                .AddTransient<DoseFactorsService>()
                .AddTransient<GeometryService>()
                .AddTransient<MaterialsService>()
                .AddTransient<RadionuclidesService>()
                .AddTransient<InputDataBuilder>()
                .AddTransient<MainWindowViewModel>()
                .BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            LoadServices();

            this.MainWindow = new MainWindow();
            this.MainWindow.Show();
        }

        public static T GetService<T>()
        {
            return _provider.GetService<T>();
        }

        private bool ValidateLanguageDictionary(CultureInfo newCulture)
        {
            var dict = Current.Resources.MergedDictionaries.SingleOrDefault(d => d.Source.OriginalString.Contains($"lang.{newCulture.Name}"));
            return (dict != null && ValidateLanguageDictionary(dict)) || newCulture.Name == m_Languages.First().Name;
        }

        private bool ValidateLanguageDictionary(ResourceDictionary langRD)
        {
            ResourceDictionary referenceRD = Current.Resources.MergedDictionaries.Single(d => d.Source.OriginalString.EndsWith("lang.xaml"));
            foreach (string key in referenceRD.Keys)
            {
                if (!langRD.Contains(key))
                    return false;
            }
            return true;
        }
    }
}
