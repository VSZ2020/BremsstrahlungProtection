using System.ComponentModel.DataAnnotations.Schema;

namespace BSP.Data.Entities.MaterialFactors
{
    [Table("GeometricProgressionFactors")]
    public class GeometricProgressionEntity : BaseMaterialFactorEntity
    {
        public float a { get; set; }
        public float b { get; set; }
        public float c { get; set; }
        public float d { get; set; }
        public float xi { get; set; }

        public float BarrierFactor { get; set; }

        public override float[] Values => [a, b, c, d, xi, BarrierFactor];
    }
}
