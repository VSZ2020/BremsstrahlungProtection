using System.ComponentModel.DataAnnotations.Schema;

namespace BSP.Data.Entities.DoseConversionFactors
{
    [Table("EffectiveDCF")]
    public class EffectiveDoseEntity : BaseDoseFactorEntity
    {
        public int ExposureGeometryId { get; set; }

        [ForeignKey(nameof(ExposureGeometryId))]
        public ExposureGeometryEntity ExposureGeometry { get; set; }
    }
}
