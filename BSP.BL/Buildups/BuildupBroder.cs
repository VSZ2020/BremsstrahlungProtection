using BSP.Geometries.SDK;

namespace BSP.BL.Buildups
{
    public class BuildupBroder : BaseHeterogeneousBuildup
    {
        public BuildupBroder(Func<double, double[], double> buildup) : base(buildup)
        {

        }

        public override string Description => "Evaluates buildup for heterogeneous medium using Broder expression.";

        public override double EvaluateComplexBuildup(double[] mfp, double[][] coefficients, double[]? complexBuildupFactors = null)
        {
            int layersCount = mfp.Length;

            //The first term of Broder equation
            double firstBuildup = buildup(mfp.Sum(), coefficients[^1]);

            //The least terms of Broder equation
            double buildupSum = 0;
            for (int i = 0; i < layersCount - 1; i++)
            {
                double sumUD = 0;
                for (int j = 0; j <= i; j++)
                    sumUD += mfp[j];
                buildupSum += buildup(sumUD, coefficients[i]) - buildup(sumUD, coefficients[i + 1]);
            }
            return firstBuildup + buildupSum;
        }
    }
}
