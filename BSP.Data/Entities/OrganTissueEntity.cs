using BSP.Data.Entities.DoseConversionFactors;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSP.Data.Entities
{
    [Table("OrgansAndTissues")]
    public class OrganTissueEntity : BaseEntity
    {
        /// <summary>
        /// Название органа или ткани
        /// </summary>
        public string Name { get; set; }

        public List<EquivalentDoseEntity> DoseFactors { get; set; }
    }
}
