namespace BSP.BL.Analysis.Sensitivity
{
    public class VariableValueInfo
    {
        public string Name { get; set; }

        public double MinBoundary { get; set; } = double.MinValue;
        public double MaxBoundary { get; set; } = double.MaxValue;

        public double Step { get; set; } = 0.01;

        public double From { get; set; } = double.MinValue;

        public double To { get; set; } = double.MaxValue;

    }
}
