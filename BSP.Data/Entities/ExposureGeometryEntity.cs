using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSP.Data.Entities
{
    [Table("ExposureGeometries")]
    public class ExposureGeometryEntity : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        public List<BaseDoseFactorEntity>? Factors { get; set; }
    }
}
