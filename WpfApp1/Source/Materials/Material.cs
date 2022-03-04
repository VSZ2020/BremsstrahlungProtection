/*
 * Создано в SharpDevelop.
 * Пользователь: Slava Izgagin
 * Дата: 04-Mar-20
 * Время: 20:55
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */


namespace BSP
{
	/// <summary>
	/// Класс, описывающий характеристики материала/вещества
	/// </summary>
	public class Material
	{
		private double _Density;				
		private double _Z;
		private string _Name;

		/// <summary>
		/// Задает порог пот энергии, ниже которого интерполяции будует проводиться по-другому.
		/// </summary>
		public float EnergyLimit;

		/// <summary>
		/// Содержит характеристики вещества и коэффициенты для ФН
		/// </summary>
		public  MaterialFactors Factors;

		/// <summary>
		/// Плотность вещества
		/// </summary>
		public double Density
		{
			get { return _Density; }
			set { _Density = (value <= 0.0) ? 1 : value; }
		}

		/// <summary>
		/// Атомный номер вещества материала
		/// </summary>
		public double Z
		{
			get { return _Z; }
			//set { _Z = (value > 0F) ? value : 1F; }
		}

		/// <summary>
		/// Название вещества
		/// </summary>
		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		public Material(string MaterialName, double Z)
		{
			this.Name = MaterialName;
			Density = 1.0;
			_Z = Z;
			Factors = new MaterialFactors();
		}
	}
}
