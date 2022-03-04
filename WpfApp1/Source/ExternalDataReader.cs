using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows;

namespace BSP
{
    public static class ExternalDataReader
    {
		/// <summary>
		/// Проверяет существование таблицы в БД
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static bool TableExists(OleDbConnection connection, string tableName)
		{
			var scheme = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
			if (scheme.Select("TABLE_NAME=\'" + tableName + "\'").Length == 0)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Чтение долей от полного выхода ТИ из файла
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public static double[] LoadBSDataFromTextFile(string fileName, System.Globalization.NumberFormatInfo culture)
		{
			bool isError = false;
			double[] bs_coeffs = new double[1];

			if (!File.Exists(fileName))
			{
				MessageBox.Show(
					(string)Application.Current.Resources["msgError_No_BSData_File"], 
					(string)Application.Current.Resources["msgError_Title"], 
					MessageBoxButton.OK, MessageBoxImage.Error);
				Breamsstrahlung.SetDefaultBSFactors();
				return Breamsstrahlung.BsFactor;
			}


			if (!isError)
			{
				using (StreamReader reader = new StreamReader(fileName))
				{
					int groupsCount = 1;
					string buf = reader.ReadLine();                                     //Чтение строки с количеством энергий
					try { groupsCount = int.Parse(buf, culture); }                      //Попытка парсинга количества энергетических групп
					catch { isError = true; }

					if (!isError) Array.Resize(ref bs_coeffs, groupsCount);                 //Создаем новый массив значений при отсутствии ошибок

					if (!isError) for (int i = 0; i < groupsCount; i++)
						{
							buf = reader.ReadLine();
							if (!string.IsNullOrEmpty(buf))
							{
								try { bs_coeffs[i] = double.Parse(buf, culture); }
								catch
								{ isError = true; break; }
							}
						}
				}
			}
			if (isError)
			{
				MessageBox.Show(
					string.Format((string)Application.Current.Resources["msgError_ErrorDuringFileReading"], "data\\BS_data.txt"),
					(string)Application.Current.Resources["msgError_Title"],
					MessageBoxButton.OK, MessageBoxImage.Error);
				Breamsstrahlung.SetDefaultBSFactors();
				return Breamsstrahlung.BsFactor;
			}
			return bs_coeffs;
		}

		/// <summary>
		/// Читает набор данных из указанной таблицы
		/// </summary>
		/// <param name="dbPath">Путь к БД</param>
		/// <param name="mainTableName">Название главной таблицы, в которой хранятся названия других</param>
		/// <param name="tableNames">Возвращаемый массив подтаблиц</param>
		/// <param name="prefixes">Суффиксы подтаблиц</param>
		/// <returns></returns>
		public static DataSet ReadDatabaseTables(string dbPath, string mainTableName, out string[] tableNames, string[] prefixes)
		{
			DataSet dsOutput = new DataSet("output dataset");

			//Основные проверки
			if (!File.Exists(dbPath)) throw new Exception("Database file " + dbPath + " was not found!");
			if (string.IsNullOrEmpty(mainTableName)) throw new Exception("Table title is empty!");

			using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;data source=" + dbPath))
			{
				conn.Open();                                //Открываем соединение
				if (!TableExists(conn, mainTableName)) throw new Exception($"The table '{mainTableName}' isn't exists in '{dbPath}' base");
				OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM " + mainTableName, conn);
				DataTable bufDt = new DataTable();
				adapter.Fill(bufDt);      //Загружаем таблицу в буфер

				//Сортируем названия
				bufDt.TableName = mainTableName;
				//bufDt.DefaultView.Sort = bufDt.Columns[0].ColumnName + " asc";
				//bufDt = bufDt.DefaultView.ToTable();
				dsOutput.Tables.Add(bufDt);
				//adapter.Fill(dsOutput, mainTableName);      //Загружаем таблицу в набор таблиц

				//Получаем количество таблиц
				int tablesCount = dsOutput.Tables[0].Rows.Count;
				if (tablesCount < 1) throw new Exception("There is nothong to read! The table list in '" + dbPath + "' is empty");
				tableNames = new string[tablesCount];

				int i = 0;                  //Счетчик подтаблиц
				while (i < tablesCount)
				{
					tableNames[i] = dsOutput.Tables[0].Rows[i][0].ToString();

					if (prefixes == null)
					{
						//Проверяем существование таблицы
						if (!TableExists(conn, tableNames[i])) throw new Exception($"Table '{tableNames[i]}' was not found in '{dbPath}' database");

						//Если таблица существует, то читаем её и сохраняем в DataSet
						adapter.SelectCommand.CommandText = $"SELECT * FROM [{tableNames[i]}]";
						adapter.Fill(dsOutput, tableNames[i]);
					}
					else//Если есть подтаблицы, то проверяем их наличие
						for (int j = 0; j < prefixes.Length; j++)
						{
							//Проверяем существование таблицы
							if (!TableExists(conn, tableNames[i] + prefixes[j]))
								throw new Exception($"Table '{tableNames[i] + prefixes[j]}' was not found in '{dbPath}' database");
							adapter.SelectCommand.CommandText = $"SELECT * FROM {tableNames[i] + prefixes[j]}";
							adapter.Fill(dsOutput, tableNames[i] + prefixes[j]);
						}
					//Инкремент количества таблиц
					i++;
				}
			}
			
			return dsOutput;
		}

		/// <summary>
		/// Загрузка радионуклидов из БД
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="BSEnergiesCount"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public static Nuclides LoadNuclidesFromDB(string fileName, int BSEnergiesCount, System.Globalization.NumberFormatInfo culture)
		{
			//Читаем таблицы из файла
			string[] tableNames;
			DataSet ds = ReadDatabaseTables(fileName, "Nuclides", out tableNames, null);
			if (ds == null) { throw new NullReferenceException("The database link for Nuclides is null or empty. Check the nuclides database at 'data' directory"); }

			int nuclidesCount = (tableNames.Length > 0) ? tableNames.Length : 0;
			Nuclides nuclides = new Nuclides();													//Создаем лист нуклидов с количеством записей nuclCount

			//Начинаем заполнение списка нуклидов
			for (int i = 0; i < nuclidesCount; i++)                                             //Цикл по нуклидам
			{
				DataTable dt = ds.Tables[i + 1];                                                //Текущая таблица
				int linesCount = dt.Rows.Count;                                                 //Количество линий излучения
				tableNames[i] = tableNames[i].Replace('_', '-');                                //Заменяем подчеркивания в названии нуклида

				//Создаем списки для хранение энергий
				List<double> maxEnergies = new List<double>(linesCount);
				List<double> yields = new List<double>(linesCount);
				List<double> meanEnergies = new List<double>(linesCount);
				for (int j = 0; j < linesCount; j++)                                            //Цикл по линиям нуклида
				{
					maxEnergies.Add(double.Parse(dt.Rows[j][0].ToString(), culture));
					meanEnergies.Add(double.Parse(dt.Rows[j][1].ToString(), culture));
					yields.Add(double.Parse(dt.Rows[j][2].ToString(), culture));
				}
				
				nuclides.AddNuclide(new Nuclide(tableNames[i], maxEnergies, meanEnergies, yields));	//Добавляес нуклид в список
			}

			return nuclides;                                                                       //Возвращаем созданный набор нуклидов
		}
		
		/// <summary>
		/// Чтение базовых материалов из БД
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static Materials LoadMaterialsFromDB(string fileName)
		{
			string[] tableNames;
			string[] prefixes = new string[] { "_um", "_umAbsorb", "_Buildup", "_taylor"};

			DataSet dsMain = ReadDatabaseTables(fileName, "Materials", out tableNames, prefixes);
			if (dsMain == null) { throw new NullReferenceException("Dataset link is empty or null for tables!"); }                                      //В случае ошибки завершаем чтение

			int materialsCount = (dsMain.Tables[0].Rows.Count > 0) ? dsMain.Tables[0].Rows.Count : 0;
			Materials materials = new Materials();

			//Заполняем все массивы
			int TablesCount = tableNames.Length;

			for (int i = 0; i < TablesCount; i++)
			{
				Material mat = new Material(dsMain.Tables[0].Rows[i][0].ToString(), (double)dsMain.Tables[0].Rows[i][1]);
				mat.Density = (double)dsMain.Tables[0].Rows[i][2];
				mat.EnergyLimit = (float)dsMain.Tables[0].Rows[i][3];
 
				//Создаем таблицы для хранения отдельных таблиц по коэффициентам
				DataTable dt_um = dsMain.Tables[tableNames[i] + prefixes[0]];
				DataTable dt_umAbsorb = dsMain.Tables[tableNames[i] + prefixes[1]];
				DataTable dt_buildup = dsMain.Tables[tableNames[i] + prefixes[2]];
				DataTable dt_taylor = dsMain.Tables[tableNames[i] + prefixes[3]];

				//Заполняем значения для коэф. ослабления
				int RowsCount = dt_um.Rows.Count;
				mat.Factors.Um_attennuation = new FactorValue(RowsCount);
				for (int EnergyIndex = 0; EnergyIndex < RowsCount; EnergyIndex++)
				{
					//um
					mat.Factors.Um_attennuation.Energy[EnergyIndex] = (double)dt_um.Rows[EnergyIndex][0];						//E
					mat.Factors.Um_attennuation.Value[EnergyIndex] = (double)dt_um.Rows[EnergyIndex][1];						//um (ослаб)

				}

				//Заполняем значения для коэф. поглощения
				RowsCount = dt_umAbsorb.Rows.Count;
				mat.Factors.Um_absorbtion = new FactorValue(RowsCount);
				for (int EnergyIndex = 0; EnergyIndex < RowsCount; EnergyIndex++)
				{
					//um_Absorbtion
					mat.Factors.Um_absorbtion.Energy[EnergyIndex] = (double)dt_umAbsorb.Rows[EnergyIndex][0];					//E
					mat.Factors.Um_absorbtion.Value[EnergyIndex] = (double)dt_umAbsorb.Rows[EnergyIndex][1];					//umAbsorb

				}

				//Заполняем значения для коэф. формулы японцев
				RowsCount = dt_buildup.Rows.Count;
				mat.Factors.KFactors = new KFactors(RowsCount);
				for (int EnergyIndex = 0; EnergyIndex < RowsCount; EnergyIndex++)
				{
					//Buildup
					mat.Factors.KFactors.Energy[EnergyIndex] = (double)dt_buildup.Rows[EnergyIndex][0];							//E
					mat.Factors.KFactors.b[EnergyIndex] = (double)dt_buildup.Rows[EnergyIndex][1];								//b
					mat.Factors.KFactors.c[EnergyIndex] = (double)dt_buildup.Rows[EnergyIndex][2];								//c
					mat.Factors.KFactors.a[EnergyIndex] = (double)dt_buildup.Rows[EnergyIndex][3];								//a
					mat.Factors.KFactors.xk[EnergyIndex] = (double)dt_buildup.Rows[EnergyIndex][4];								//xi
					mat.Factors.KFactors.d[EnergyIndex] = (double)dt_buildup.Rows[EnergyIndex][5];								//d
				}

				//Заполняем значения для коэф. формулы Тейлора
				RowsCount = dt_taylor.Rows.Count;
				mat.Factors.Taylor = new TaylorFactors(RowsCount);
				for (int EnergyIndex = 0; EnergyIndex < RowsCount; EnergyIndex++)
				{
					//taylor
					mat.Factors.Taylor.Energy[EnergyIndex] = (double)dt_taylor.Rows[EnergyIndex][0];							//E
					mat.Factors.Taylor.A1[EnergyIndex] = (double)dt_taylor.Rows[EnergyIndex][1];								//A1
					mat.Factors.Taylor.a1[EnergyIndex] = (double)dt_taylor.Rows[EnergyIndex][2];								//alpha1
					mat.Factors.Taylor.a2[EnergyIndex] = (double)dt_taylor.Rows[EnergyIndex][3];								//alpha2
					mat.Factors.Taylor.Delta[EnergyIndex] = (double)dt_taylor.Rows[EnergyIndex][4];								//delta	
				}
				
				materials.AddMaterial(mat);																						//Добавляем материал в список
			}
			return materials;       
		}
		/// <summary>
		/// Чтение дозовых коэффициентов из файла
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public static List<DoseFactor> LoadDoseFactorsFromDB(string fileName, System.Globalization.NumberFormatInfo culture)
		{
			List<DoseFactor> factors = new List<DoseFactor>();

			string[] tableNames;
			DataSet dsMain = ReadDatabaseTables(fileName, "Geometries", out tableNames, null);

			if (dsMain == null) { throw new NullReferenceException("There is no any link to dose factors geometries database!"); }

			int tablesCount = (dsMain.Tables[0].Rows.Count > 0) ? dsMain.Tables[0].Rows.Count : 0;

			DoseFactor effDoseFactor = new DoseFactor(DoseFactor.DoseFactorType.EffectiveDose);
			DoseFactor eqvDoseFactor = new DoseFactor(DoseFactor.DoseFactorType.EquivalentDose);

			//Naming of dose factors
			effDoseFactor.Name = (string)Application.Current.Resources["DoseNameEffective"];
			effDoseFactor.DoseRateUnits = (string)Application.Current.Resources["DoseUnitSvPerH"];

			eqvDoseFactor.Name = (string)Application.Current.Resources["DoseNameEquivalent"];
			eqvDoseFactor.DoseRateUnits = (string)Application.Current.Resources["DoseUnitSvPerH"];

			//Начинаем заполнение списка геометрий
			for (int i = 0; i < tablesCount; i++)																								//Цикл по таблицам набора DataSet
			{																																	
				int EnergiesCount = dsMain.Tables[i + 1].Rows.Count;																			//Количество линий излучения
				int OrgansCount = dsMain.Tables[i + 1].Columns.Count - 2;                                                                       //Количество органов

				//Data arrays for DoseFactor at current geometry
				DoseFactor.DoseFactorData effDoseData = new DoseFactor.DoseFactorData() { Value = new List<DoseFactorWithOrganName>() };
				DoseFactor.DoseFactorData eqvDoseData = new DoseFactor.DoseFactorData() { Value = new List<DoseFactorWithOrganName>(OrgansCount) };

				//Struct value for DoseFactorData arrays
				DoseFactorWithOrganName effDoseFactorValue = new DoseFactorWithOrganName(EnergiesCount);

				//Из таблицы названий геометрий считываем полное имя текущей геометрии (ISO, ROt, ...)
				effDoseData.GeometryName = dsMain.Tables[0].Rows[i][1].ToString();
				eqvDoseData.GeometryName = effDoseData.GeometryName;

				//For EFFECTIVE DOSE RATE
				for (int j = 0; j < EnergiesCount; j++)
				{
					effDoseFactorValue.Factor.Energy[j] = double.Parse(dsMain.Tables[i + 1].Rows[j][0].ToString(), culture);
					effDoseFactorValue.Factor.Value[j] = double.Parse(dsMain.Tables[i + 1].Rows[j][1].ToString(), culture);
				}
				effDoseData.Value.Add(effDoseFactorValue);

				//FOR EQUIVALENT DOSE
				//Цикл по органам для заданной геометрии
				for (int col = 0; col < OrgansCount; col++)
				{
					var eqvDoseFactorValue = new DoseFactorWithOrganName(EnergiesCount);
					for (int j = 0; j < EnergiesCount; j++)
					{
						eqvDoseFactorValue.Factor.Energy[j] = double.Parse(dsMain.Tables[i + 1].Rows[j][0].ToString(), culture);
						eqvDoseFactorValue.Factor.Value[j] = double.Parse(dsMain.Tables[i + 1].Rows[j][col + 2].ToString(), culture);
					}
					//Set organ name
					eqvDoseFactorValue.Name = dsMain.Tables[i + 1].Columns[col + 2].Caption;
					//Add fulled data to list
					eqvDoseData.Value.Add(eqvDoseFactorValue);
				}

				//Add fulled assets for geometry
				effDoseFactor.AddDoseFactor(effDoseData);
				eqvDoseFactor.AddDoseFactor(eqvDoseData);
			}
			factors.Add(effDoseFactor);
			factors.Add(eqvDoseFactor);

			/*
			 * Считываем данные по другим дозовым коэффициентам (переход к Hp(10) и к экспозиционной дозе)
			 */

			dsMain = ReadDatabaseTables(fileName, "Additional", out tableNames, null);
			if (dsMain == null) { throw new NullReferenceException("There is no any link to additional dose factors database!"); }
			tablesCount = (dsMain.Tables[0].Rows.Count > 0) ? dsMain.Tables[0].Rows.Count : 0;

			for (int i = 0; i < tablesCount; i++)
			{
				int EnergiesCount = dsMain.Tables[i + 1].Rows.Count;                                             //Количество линий излучения

				if (dsMain.Tables[i + 1].TableName.Contains("H10"))
				{
					DoseFactor H10DoseFactor = new DoseFactor(DoseFactor.DoseFactorType.Hp10) { FactorData = new List<DoseFactor.DoseFactorData>()};
					DoseFactor.DoseFactorData H10FactorData = new DoseFactor.DoseFactorData();

					DoseFactorWithOrganName factor = new DoseFactorWithOrganName(EnergiesCount);

					H10DoseFactor.Name = (string)Application.Current.Resources["DoseNameAmbientEquivalent"];
					H10DoseFactor.DoseRateUnits = (string)Application.Current.Resources["DoseUnitSvPerH"];

					for (int j = 0; j < EnergiesCount; j++)
					{
						factor.Factor.Energy[j] = double.Parse(dsMain.Tables[i + 1].Rows[j][0].ToString(), culture);
						factor.Factor.Value[j] = double.Parse(dsMain.Tables[i + 1].Rows[j][1].ToString(), culture);
					}
					H10FactorData.Value = new List<DoseFactorWithOrganName>() { factor };
					H10DoseFactor.FactorData.Add(H10FactorData) ;

					factors.Add(H10DoseFactor);
				}
				if (dsMain.Tables[i + 1].TableName.Contains("ExpDose"))
				{
					DoseFactor expDoseFactor = new DoseFactor(DoseFactor.DoseFactorType.ExposedDose) { FactorData = new List<DoseFactor.DoseFactorData>()};
					DoseFactor.DoseFactorData expFactorData = new DoseFactor.DoseFactorData();
					DoseFactorWithOrganName factor = new DoseFactorWithOrganName(EnergiesCount);

					expDoseFactor.Name = (string)Application.Current.Resources["DoseNameExposed"];
					expDoseFactor.DoseRateUnits = (string)Application.Current.Resources["DoseUnitExposed"];

					for (int j = 0; j < EnergiesCount; j++)
					{
						factor.Factor.Energy[j] = double.Parse(dsMain.Tables[i + 1].Rows[j][0].ToString(), culture);
						factor.Factor.Value[j] = double.Parse(dsMain.Tables[i + 1].Rows[j][1].ToString(), culture);
					}
					expFactorData.Value = new List<DoseFactorWithOrganName> { factor };
					expDoseFactor.FactorData.Add(expFactorData);

					factors.Add(expDoseFactor);
				}
			}

			factors.Add(new DoseFactor(DoseFactor.DoseFactorType.AirKerma)
			{
				Name = (string)Application.Current.Resources["DoseNameAirKerma"],
				DoseRateUnits = (string)Application.Current.Resources["DoseUnitGyPerH"]
			});

			return factors;                   //Возвращаем набор дозовых коэффициентов
		}
	}
}
