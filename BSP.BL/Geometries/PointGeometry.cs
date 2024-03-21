using BSP.BL.Calculation;

namespace BSP.BL.Geometries
{
    public class PointGeometry : BaseGeometry
    {
        public PointGeometry(float[] dims, int[] discreteness):base(dims,discreteness)
        {

        }

        public override void AssignDimensions(float[] dims, int[] dicreteness)
        {
            
        }

        public override double GetFluence(SingleEnergyInputData input)
        {
            var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();

            //Начальные координаты точки регистрации
            var R = Math.Sqrt(input.CalculationPoint.X * input.CalculationPoint.X + input.CalculationPoint.Y * input.CalculationPoint.Y + input.CalculationPoint.Z * input.CalculationPoint.Z);

            var mfp = Enumerable.Range(0, layersMassThickness.Length).Select(i => layersMassThickness[i] * input.massAttenuationFactors[i + 1]).ToArray();
            double totalLooseExp = Math.Exp(-mfp.Sum());

            //Расчет вклада поля рассеянного излучения
            double buildupFactor = input.BuildupProcessor != null && mfp.Length > 0 ? input.BuildupProcessor.EvaluateComplexBuildup(mfp, input.BuildupFactors) : 1.0;

            return totalLooseExp / (4.0 * Math.PI * R * R) * buildupFactor;
        }
    }
}
