using System.ComponentModel.DataAnnotations.Schema;

namespace BSP.Data.Entities.Radionuclides
{
    [Table("EnergyIntensityOfRadionuclides")]
    public class RadionuclideEnergyIntensityEntity : BaseEntity
    {
        [ForeignKey(nameof(RadionuclideId))]
        public RadionuclideEntity Radionuclide { get; set; }

        public int RadionuclideId { get; set; }

        /// <summary>
        /// Граничная энергия бета-спектра [МэВ]
        /// </summary>
        public float EndpointEnergy { get; set; }

        /// <summary>
        /// Средняя энергия бета-спектра для одного перехода [МэВ/част]
        /// </summary>
        public float AverageEnergy { get; set; }

        /// <summary>
        /// Выход бета-излучения на распад [част/распад]
        /// </summary>
        public float Yield { get; set; }
    }
}
