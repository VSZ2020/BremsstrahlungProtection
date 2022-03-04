/*
 * Created by SharpDevelop.
 * User: Slava Izgagin
 * Date: 29-Feb-20
 * Time: 01:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Windows;

namespace BSP
{
	/// <summary>
	/// Описываем слой защиты
	/// </summary>
	public class MaterialLayer
	{
		/// <summary>
		/// Толщина защиты [см]
		/// </summary>
		public double _d;
		private double _Density;                //Нестандартная плотность слоя защиты
		private Material _Material;

		/// <summary>
		/// Содержит список веществ, из которых  состоит данный слой защиты
		/// </summary>
		public Material Material {
			get
			{
				return _Material;
			}
		}
		/// <summary>
		/// Возвращает толщину слоя [см]
		/// </summary>
		public double d
		{
			get { return _d; }
			set
			{
				_d = (value > 0) ? value : throw new Exception("Толщина d должна быть > 0");
			}
		}

		/// <summary>
		/// Возвращает массовую толщину слоя [г/см2]
		/// </summary>
		public double dm
		{
			get { return d * _Density; }
		}

		/// <summary>
		/// Возвращает (устанавливает) плотность данного слоя материала
		/// </summary>
		public double Density
		{
			get { return _Density; }
			set {
				if (value > 0) //throw new ArgumentOutOfRangeException("Значение плотности должно быть больше 0!");
					_Density = value;
			}
		}

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="d">Толщина слоя, см</param>
		/// <param name="LayerMaterial">Объект материала для слоя</param>
		public MaterialLayer(float d, Material LayerMaterial)
		{
			//Инициализация значений
			this._Material = LayerMaterial;
			this.Density = LayerMaterial.Density;
			this.d = d;
			
		}

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="LayerMaterial">Объект материала для слоя</param>
		public MaterialLayer(Material LayerMaterial)
		{
			//Инициализация значений
			this._Material = LayerMaterial;
			this.Density = LayerMaterial.Density;
			//this.d = 1;
		}
	}
}
