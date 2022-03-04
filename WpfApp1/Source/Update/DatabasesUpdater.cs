using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Windows;

namespace BSP
{
	class DatabasesUpdater
	{
        /// <summary>
        /// Путь к файлу с версиями баз данных
        /// </summary>
        public static string dbVerFile = "data\\FileRevision.xml";

        public const string dbKeyMaterials      = "Materials";
        public const string dbKeyNuclides       = "Nuclides";
        public const string dbKeyDoseFactors    = "DoseFactors";

        /// <summary>
        /// Версии баз данных, записываемые самой программой
        /// </summary>
        public static Dictionary<string, Version> dbVersions = new Dictionary<string, Version>
		{
			{dbKeyNuclides, new Version(1,0,0,0) },		    //Db Nuclides
			{dbKeyMaterials, new Version(1,0,0,0) },        //Db Materials
			{dbKeyDoseFactors, new Version(1,0,0,0) }		//Db DoseFactors
		};

		/// <summary>
		/// Версии баз данных, вводимые разработчиком
		/// </summary>
		public static Dictionary<string, Version> dbNewVersions = new Dictionary<string, Version>
		{
			{dbKeyNuclides, new Version(1,0,0,6)},			//Db Nuclides
			{dbKeyMaterials, new Version(1,0,0,6)},		    //Db Materials
			{dbKeyDoseFactors, new Version(1,0,0,5)}		//Db DoseFactors
		};

		/// <summary>
		/// Проверяем сущестование файла версий БД
		/// </summary>
		public  static void CheckDatabasesVersionFileExists()
		{
			if (!File.Exists(dbVerFile))
			{
				//Создаем директорию, если она отсутствует
				if (!Directory.Exists(@"data\"))
					Directory.CreateDirectory("data");

				File.WriteAllBytes(dbVerFile, Properties.Resources.FileRevision);
			}
		}

        /// <summary>
        /// Читаем версии баз данных из файла xml
        /// </summary>
        /// <param name="fileName"></param>
        public void ReadDBVersionsFromFile(string fileName)
        {
            if (!File.Exists(fileName)) throw new FileLoadException("Отсутствует файл версий " + fileName);

            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            XmlElement databases = doc.DocumentElement;

           //Получаем массив ключей
           List<string> keys = new List<string>(dbVersions.Count);
            foreach (string key in dbVersions.Keys)
                keys.Add(key);

            //Читаем файл версий
            foreach (XmlNode node in databases)
            {
                if (node.Attributes.Count < 1) continue;
                for (int i = 0; i < keys.Count; ++i)
                {
                    if (node.Name == keys[i])
                    {
                        dbVersions[keys[i]] = Version.Parse(node.Attributes.GetNamedItem("ver").Value);
                    }
                }

            }
        }

        /// <summary>
        /// Процедура обновления баз с разрешения пользователя
        /// </summary>
        /// <param name="IsForceUpdate"></param>
        public void UpdateDatabases(bool IsForceUpdate)
        {
            try
            {
                ReadDBVersionsFromFile(dbVerFile);

                //Словарь флагов обновления для БД
                Dictionary<string, bool> flags = new Dictionary<string, bool> {
                    {"Nuclides",false}, {"Materials",false }, {"DoseFactors",false }
                };

                foreach (string s in dbVersions.Keys)
                {
                    //Если только быстрая перезапись, то перезаписываем все базы без предупреджений
                    if (IsForceUpdate)
                    {
                        File.WriteAllBytes(Updater.defaultFileNames[s], Updater.fileBytes[s]);
                        flags[s] = true;
                    }
                    else
                    {
                        if (Updater.IsNewer(dbVersions[s], dbNewVersions[s]))
                        {
                            MessageBoxResult res = MessageBox.Show(
                             "Разработчик предлагает обновить базу " +
                             s + ". Будьте осторожны! После обновления все Ваши изменения в базах будут утеряны. Обновить?",
                             "Обновление файлов", MessageBoxButton.YesNo, MessageBoxImage.Question);

                            //Если согласился, то перезаписываем файл
                            if (res == MessageBoxResult.Yes)
                            {
                                File.WriteAllBytes(Updater.defaultFileNames[s], Updater.fileBytes[s]);

                                //Если файл перезаписан, то обновляем его номер старой версии в массиве
                                flags[s] = true;
                            }
                        }
                    }

                }

                //Подготоваливаем документ для записи
                XmlDocument doc = new XmlDocument();
                doc.Load(dbVerFile);
                XmlElement databases = doc.DocumentElement;

                //Записываем версии в xml
                foreach (XmlNode node in databases)
                {
                    //Проверяем на существование записываемого ключа БД
                    if (dbNewVersions.ContainsKey(node.Name))
                    {
                        if (flags[node.Name])
                            node.Attributes.GetNamedItem("ver").Value = dbNewVersions[node.Name].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Ключа " + node.Name + " в базе словаре нет! Проверьте имена файлов БД", "Не найден ключ", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                doc.Save(dbVerFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка во время обновления баз", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
