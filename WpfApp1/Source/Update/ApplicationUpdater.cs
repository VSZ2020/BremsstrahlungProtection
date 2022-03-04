﻿using BSP.Source.XAML_Forms;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace BSP
{
	public class ApplicationUpdater
	{
        public string appLink = "";
        public static string verFileUrl = "https://drive.google.com/u/0/uc?id=15jsp4LS8vxvQj82S-EChdXzofnQWNj07&export=download";
        public static string verFilePath = "BSP.ver";         //место хранения файла xml и его имя
        public string newerVersionCode = "0.0.0.0";

        public async void CheckUpdatesASync()
		{
            

		}

        /// <summary>
        /// Проверяет наличие обновлений для приложения
        /// </summary>
        public void CheckApplicationUpdate()
        {
            MessageBoxResult res  = MessageBoxResult.No;
            if (CheckNewAppVersionOnServer())
                res = MessageBox.Show(
                    string.Format((string)Application.Current.Resources["msgQuestion_UpdateApp"], newerVersionCode), 
                    (string)Application.Current.Resources["title_UpdateDialogTitle"], 
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

            //Если пользователь соглашается обновить программу, то запускаем форму для скачивания
            if (res == MessageBoxResult.Yes && !string.IsNullOrEmpty(appLink))
            {
                UpdaterDownloadWindow updaterWindow = new UpdaterDownloadWindow(appLink);
                updaterWindow.ShowDialog();
            }
        }

        /// <summary>
        /// Проверяет наличие новой версии приложения на сервере
        /// </summary>
        /// <returns></returns>
        private bool CheckNewAppVersionOnServer()
        {
            try
            {
                System.Net.WebClient client = new System.Net.WebClient();
                client.DownloadFile(new Uri(verFileUrl), verFilePath);

                //Открываем документ и считываем данные
                using (StreamReader reader = new StreamReader(verFilePath))
                {
                    this.newerVersionCode = reader.ReadLine();
                    appLink = reader.ReadLine();                            //Получаем ссылку на новую версию файла
                }

                if (string.IsNullOrEmpty(this.newerVersionCode)) throw new Exception("Version code field is null or empty!");
                if (!Updater.IsNewer(Assembly.GetExecutingAssembly().GetName().Version, Version.Parse(this.newerVersionCode))) return false;
            }
            catch (Exception ex)
            {
                // TODO: Add error to Log
                //MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                //После использования файла, удаляем его
                if (File.Exists(verFilePath))
                {
                    File.Delete(verFilePath);
                }
            }
            return true;
        }
    }
}
