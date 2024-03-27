using BSP.BL.Calculation;
using BSP.Geometries.SDK;

namespace BSP.BL.Geometries
{
    public class PointGeometry : IGeometry
    {
        public string Name => "Point";
        public string Description => "";

        public string Author => "IVS";
        
        #region GetDimensionsInfo
        public  IEnumerable<DimensionsInfo> GetDimensionsInfo()
        {
            return Enumerable.Empty<DimensionsInfo>();
        }
        #endregion
        
        public double GetNormalizationFactor(float[] dims)
        {
            return 1;
        }

        public double GetFluence(SingleEnergyInputData input)
        {
            var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();

            //Начальные координаты точки регистрации
            var R = Math.Sqrt(input.CalculationPoint.X * input.CalculationPoint.X + input.CalculationPoint.Y * input.CalculationPoint.Y + input.CalculationPoint.Z * input.CalculationPoint.Z);

            var mfp = Enumerable.Range(0, layersMassThickness.Length).Select(i => layersMassThickness[i] * input.MassAttenuationFactors[i + 1]).ToArray();
            double totalLooseExp = Math.Exp(-mfp.Sum());

            //Расчет вклада поля рассеянного излучения
            double buildupFactor = input.BuildupProcessor != null && mfp.Length > 0 ? input.BuildupProcessor.EvaluateComplexBuildup(mfp, input.BuildupFactors) : 1.0;

            return totalLooseExp / (4.0 * Math.PI * R * R) * buildupFactor;
        }
    }
}
