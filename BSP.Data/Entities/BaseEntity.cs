using System.ComponentModel.DataAnnotations;

namespace BSP.Data.Entities
{
    public class BaseEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }
    }
}
