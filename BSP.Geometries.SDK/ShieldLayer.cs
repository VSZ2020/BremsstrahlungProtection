using BSP.BL.DTO;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BSP.BL.Materials
{
    /// <summary>
    /// Слой защиты
    /// </summary>
    public class ShieldLayer
    {
        public string Name { get; set; }
        public float Z { get; set; }

        public float Weight { get; set; }
        public float Density { get; set; }
        
        /// <summary>
        /// Толщина [см]
        /// </summary>
        public float D { get; set; }

        /// <summary>
        /// Массовая толщина [г/см2]
        /// </summary>
        public float Dm => D * Density;
        
    }
}
