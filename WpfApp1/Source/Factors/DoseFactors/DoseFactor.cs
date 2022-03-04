
using System.Collections.Generic;

namespace BSP
{
	public class DoseFactor
	{
		public enum DoseFactorType
		{
			EffectiveDose,
			EquivalentDose,
			Hp10,
			ExposedDose,
			AirKerma
		}

		public struct DoseFactorData
		{
			/// <summary>
			/// Содержит название геометрии фактора
			/// </summary>
			public string GeometryName { get; set; }
			public List<DoseFactorWithOrganName> Value { get; set; }
		}
		/// <summary>
		/// Название Дозового коэффициента
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Units of dosymetric value
		/// </summary>
		public string DoseRateUnits { get; set; }

		public string ExtendedName
		{
			get { return $"{Name} ({DoseRateUnits})"; }
		}

		protected DoseFactorType _FactorType;

		public List<DoseFactorData> FactorData { get; set; }

		/// <summary>
		/// Type of dose coefficient
		/// </summary>
		public DoseFactorType FactorType
		{
			get { return _FactorType; }
		}

		public DoseFactor(DoseFactorType factorType)
		{
			this.Name = Name;
			this._FactorType = factorType;
			FactorData = new List<DoseFactorData>();	
		}

		public void AddDoseFactor(DoseFactorData data)
		{
			FactorData.Add(data);
		}
	}
}
