using BSP.BL.Buildups.Common;
using BSP.Geometries.SDK;

namespace BSP.BL.Buildups
{
    public class BuildupAverage : BaseHeterogeneousBuildup
    {
        public BuildupAverage(Func<double, double[], double> buildup) : base(buildup) { }

        public override string Description => "Calculates the accumulation factor as a weighted average. The weight is the optical thickness of the layer.";

        public override double EvaluateComplexBuildup(double[] mfp, double[][] factors, double[]? complexBuildupFactors = null)
        {
            return Enumerable.Range(0, mfp.Length).Select(i => buildup(mfp[i], factors[i]) * mfp[i]).Sum() / mfp.Sum();
        }
    }
}
