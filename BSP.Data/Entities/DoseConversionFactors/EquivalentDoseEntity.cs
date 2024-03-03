using System.ComponentModel.DataAnnotations.Schema;

namespace BSP.Data.Entities.DoseConversionFactors
{
    [Table("EquivalentDCF")]
    public class EquivalentDoseEntity : BaseDoseFactorEntity
    {
        public int ExposureGeometryId { get; set; }

        [ForeignKey(nameof(ExposureGeometryId))]
        public ExposureGeometryEntity ExposureGeometry { get; set; }


        public int OrganTissueId { get; set; }

        [ForeignKey(nameof(OrganTissueId))]
        public OrganTissueEntity OrganTissue { get; set; }
    }
}
