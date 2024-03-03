using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSP.Data.Entities
{
    [Table("Materials")]
    public class MaterialEntity : BaseEntity
    {
        /// <summary>
        /// Название вещества
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Плотность вещества
        /// </summary>
        public float Density { get; set; }

        /// <summary>
        /// Атомный номер вещества материала
        /// </summary>
        public float Z { get; set; }

        public float Weight { get; set; }

        public float MassFraction { get; set; }

        public List<BaseMaterialFactorEntity> Factors { get; set; }
    }
}
