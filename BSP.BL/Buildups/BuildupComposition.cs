using BSP.BL.Buildups.Common;

namespace BSP.BL.Buildups
{
    public class BuildupComposition : BaseHeterogeneousBuildup
    {
        public BuildupComposition(Func<double, double[], double> buildup) : base(buildup) { }

        public override string Description => "Evaluates buildup factor as product of partial buildup factors of each shielding layer.";

        public override double EvaluateComplexBuildup(double[] mfp, double[][] factors)
        {
            double prod = 1;
            for (int i = 0; i < mfp.Length; i++)
            {
                prod *= buildup(mfp[i], factors[i]);
            }
            return prod;
        }
    }
}
