using System.ComponentModel.DataAnnotations.Schema;

namespace BSP.Data.Entities
{
    public abstract class BaseMaterialFactorEntity : BaseEntity
    {
        public int MaterialId { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public MaterialEntity Material { get; set; }

        public float Energy { get; set; }

        [NotMapped]
        public abstract float[] Values { get; }
    }
}
