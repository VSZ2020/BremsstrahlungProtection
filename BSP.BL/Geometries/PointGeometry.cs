using BSP.BL.Calculation;

namespace BSP.BL.Geometries
{
    public class PointGeometry : BaseGeometry
    {
        public PointGeometry(float[] dims, int[] discreteness)
        {

        }

        public override double GetFluence(SingleEnergyInputData input)
        {
            var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();

            //Начальные координаты точки регистрации
            var x0 = input.CalculationDistance + input.Layers.Select(l => l.D).Sum();

            var ud = Enumerable.Range(0, layersMassThickness.Length).Select(i => layersMassThickness[i] * input.massAttenuationFactors[i + 1]).ToArray();
            double totalLooseExp = Math.Exp(-ud.Sum());

            //Расчет вклада поля рассеянного излучения
            double buildupFactor = input.BuildupProcessor != null && ud.Length > 0 ? input.BuildupProcessor.EvaluateComplexBuildup(ud, input.BuildupFactors) : 1.0;

            return totalLooseExp / (4.0 * Math.PI * x0 * x0) * buildupFactor;
        }
    }
}
