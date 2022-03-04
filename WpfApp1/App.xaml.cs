using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using BSP.Properties;
using System.Runtime.CompilerServices;

namespace BSP
{
	/// <summary>
	/// Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static List<CultureInfo> m_Languages = new List<CultureInfo>();

		public static List<CultureInfo> Languages
		{
			get
			{
				return m_Languages;
			}
		}

		public App()
		{
			InitializeComponent();
			LanguageChanged += App_LanguageChanged;

			m_Languages.Clear();
			m_Languages.Add(new CultureInfo("en-US")); //Нейтральная культура для этого проекта
			m_Languages.Add(new CultureInfo("ru-RU"));

			Language = BSP.Properties.Settings.Default.AppCulture;
		}

		private void App_LanguageChanged(object sender, EventArgs e)
		{
			BSP.Properties.Settings.Default.AppCulture = Language;
			BSP.Properties.Settings.Default.Save();
		}

		public static event EventHandler LanguageChanged;

		public static CultureInfo Language
		{
			get
			{
				return System.Threading.Thread.CurrentThread.CurrentUICulture;
			}
			set
			{
				if (value == null) throw new ArgumentNullException("value");
				if (value == System.Threading.Thread.CurrentThread.CurrentUICulture) return;

				System.Threading.Thread.CurrentThread.CurrentUICulture = value;

				ResourceDictionary dict = new ResourceDictionary();
				switch (value.Name)
				{
					case "ru-RU":
						dict.Source = new Uri(String.Format("pack://application:,,,/Resources/lang.{0}.xaml", value.Name), UriKind.Absolute);
						break;
					default:
						dict.Source = new Uri("pack://application:,,,/Resources/lang.xaml", UriKind.Absolute);
						break;
				}
				
				ResourceDictionary oldDict = null;
				foreach (var d in Application.Current.Resources.MergedDictionaries)
				{
					if (d != null && d.Source.OriginalString.StartsWith("Resources/lang."))
					{
						oldDict = d;
						break;
					}
				}
				
				
				if (oldDict != null)
				{
					int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
					Application.Current.Resources.MergedDictionaries.Remove(oldDict);
					Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
				}
				else
				{
					Application.Current.Resources.MergedDictionaries.Add(dict);
				}

				LanguageChanged(Application.Current, new EventArgs());
			}
		}
	}
}
