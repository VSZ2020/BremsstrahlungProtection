using System;

namespace BSP.ViewModels
{
    public class ResultVM
    {
        public DateTime CalculationTime { get; set; }

        public string GeometryName { get; set; }

        public string SourceMaterialName { get; set; }

        public float Distance { get; set; }

        public double DoseRate { get; set; }
    }
}
