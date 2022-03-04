using System;
using System.Windows.Controls;
using System.Xml.Schema;

namespace BSP
{
	public class RadiationSource
	{		                           
		/// <summary>
		/// Геометрические параметры источника
		/// </summary>
		public ASourceForm Geometry;

		/// <summary>
		/// Вещества, из которых состоит источник
		/// </summary>
		public Material Substances;

		/// <summary>
		/// Набор нуклидов в источнике излучения
		/// </summary>
		public Nuclides Radionuclides;

		/// <summary>
		/// Потоки энергии
		/// </summary>
		public double[] Ib;

		/*
		 *-----------------------------------------------------Перечень свойств---------------------------------------------------
		 */
		/// <summary>
		/// СУммарная активность источника в Беккерелях
		/// </summary>
		public double SummaryActivity
		{
			get
			{
				double bufferActivity = 0.0;
				if (Radionuclides != null)
					for (int i = 0; i < Radionuclides.Length; i++)
					{
						bufferActivity += Radionuclides.Collection[i].Activity;
					}
				return bufferActivity;
			}
		}

		/// <summary>
		/// Эффективный атомный номер вещества
		/// </summary>
		public float Z { get; set; } = 1.0F;

		/// <summary>
		/// Задает/Возвращает плотность источника в г/см^3
		/// </summary>
		public float Density { get; set; } = 1.0F;

		/// <summary>
		/// Возвращает удельную активность источника в Бк/см3
		/// </summary>
		public double SpecificActivity
		{
			get
			{
				if (Geometry.GetVolume() > 0.0) 
					return SummaryActivity / Geometry.GetVolume();
				else
					return 0.0;
			}
		}

		/// <summary>
		/// Конструктор класса источника. Требует обязательной инициализации классов
		/// </summary>
		public RadiationSource()
		{
			//Geometry = new Parallelepiped();
			Ib = new double[Breamsstrahlung.Length];
		}

		/// <summary>
		/// Конструктор класса Источник
		/// </summary>
		/// <param name="SourceForm">Геометрическая форма источника</param>
		public RadiationSource(ASourceForm.FormType SourceForm)
		{
			if (SourceForm == ASourceForm.FormType.Cylinder)
				Geometry = new Cylinder();
			else
				Geometry = new Parallelepiped();
			Ib = new double[Breamsstrahlung.Length];
		}

		public void UpdateIb(ref TextBox inputBox)
		{
			try
			{
				Ib = DataReader.CalculatePartialIBeta(ref inputBox);
			}
			catch
			{
				throw;
			}
			
		}

	}
}
