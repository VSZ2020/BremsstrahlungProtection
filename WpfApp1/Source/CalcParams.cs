/*
 * Создано в SharpDevelop.
 * Пользователь: Slava Izgagin
 * Дата: 07-Mar-20
 * Время: 22:36
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */

using BSP.Source.Calculation.CalcDirections;
using System.Collections.Generic;

namespace BSP
{
	/// <summary>
	/// Класс содержит глобальные значения и массивы, участвующие в текущей операции вычисления
	/// </summary>
	public static class CalcParams
	{	private static double _b = 100;

		/// <summary>
		/// Коэффициент, увеличивающий результат в несколько раз (по-умолчанию x1)
		/// </summary>
		public static uint multiplier = 1;

		/// <summary>
		/// Максимальное-возможное количество задаваемых нуклидов
		/// </summary>
		public const int MAX_INPUT_NUCLIDES_COUNT = 20;						

		/// <summary>
		/// Максимально-возможное количество слоев защиты
		/// </summary>
		public const int MAX_INPUT_LAYERS_COUNT = 10;                          //Максимальное количество выбираемых слоев защиты

		/// <summary>
		/// Массив хранит случайные цвета слоев защиты
		/// </summary>
		//public static System.Drawing.Color[] colorIndex;

		public static int precision { get; set; } = 5;

		/// <summary>
		/// Расстояние от источника до точки регистрации излучения
		/// </summary>
		public static double CalculationDistance
		{
			get
			{
				return _b;
			}
			set
			{
				_b = value;
			}
		} 
		
		/*
		 * Значения для связанных полей-вариантов 
		*/
		/// <summary>
		/// Набор форм источника
		/// </summary>
		public static List<ASourceForm> SourceForms;

		/// <summary>
		/// Набор направлений для расчета излучения
		/// </summary>
		public static List<ADirection> CalculationDirectionsList;

		/*
		 * Табличные значения
		*/
		public static Nuclides TableNuclides;								//Загруженный массив табличных нуклидов
		public static Materials TableMaterials;								//Массив материалов базовых материалов
		public static List<DoseFactor> TableDoseFactors;					//Набор табличных дозовых коэффициентов


		/*
		 * Выбранные пользователем значения
		*/
		public static RadiationSource Source;
		//public static Nuclides SelectedNuclidesList;
		public static ShieldLayers SelectedLayersList;                      //Массив выбранных слоев защиты
		
		public static string environmentKey = "Air";                     //Хранит ключ текущего материала среды
		public static string generalNuclideKey = "";						//Хранит ключ доминирующего нуклида в наборе
	}
}
