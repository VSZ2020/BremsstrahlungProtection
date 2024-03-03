using System.ComponentModel.DataAnnotations.Schema;

namespace BSP.Data.Entities.MaterialFactors
{
    [Table("Taylor2ExpFactors")]
    public class Taylor2ExpEntity : BaseMaterialFactorEntity
    {
        public float A { get; set; }
        public float Alpha1 { get; set; }
        public float Alpha2 { get; set; }

        public float BarrierFactor { get; set; }

        public override float[] Values => [A, Alpha1, Alpha2, BarrierFactor];
    }
}
