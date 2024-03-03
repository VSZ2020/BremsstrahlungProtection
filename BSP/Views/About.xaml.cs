using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace BSP.Source.XAML_Forms
{
    /// <summary>
    /// Логика взаимодействия для About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            labelVersion.Text = $" v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
            this.MouseDoubleClick += About_MouseDoubleClick;
        }


        private void About_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
