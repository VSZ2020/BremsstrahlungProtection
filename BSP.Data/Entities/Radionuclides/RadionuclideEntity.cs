using System.ComponentModel.DataAnnotations.Schema;

namespace BSP.Data.Entities.Radionuclides
{
    [Table("Radionuclides")]
    public class RadionuclideEntity : BaseEntity
    {
        public string Name { get; set; }

        public float HalfLife { get; set; }

        public string HalfLiveUnits { get; set; }

        public float Z { get; set; }

        public float Weight { get; set; }

        public List<RadionuclideEnergyIntensityEntity> EnergyIntensityData { get; set; }
    }
}
