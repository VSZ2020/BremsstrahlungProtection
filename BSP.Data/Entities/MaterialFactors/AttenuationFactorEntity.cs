using System.ComponentModel.DataAnnotations.Schema;

namespace BSP.Data.Entities.MaterialFactors
{
    [Table("AttenuationFactors")]
    public class AttenuationFactorEntity : BaseMaterialFactorEntity
    {
        public float Value { get; set; }
        public override float[] Values => [Value];
    }

    [Table("AbsorptionFactors")]
    public class AbsorptionFactorEntity : BaseMaterialFactorEntity
    {
        public float Value { get; set; }

        public override float[] Values => [Value];
    }
}
