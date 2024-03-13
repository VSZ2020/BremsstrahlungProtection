using BSP.BL.Buildups.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP.BL.Buildups
{
    public class BuildupLastLayer : BaseHeterogeneousBuildup
    {
        public BuildupLastLayer(Func<double, double[], double> buildup) : base(buildup)
        {

        }

        public override string Description => "Evaluates buildup for heterogeneous medium as buildup of the last layer with full optical length. Warning! This expression works well with lats layer MFP > 3.";

        public override double EvaluateComplexBuildup(double[] mfp, double[][] factors)
        {
            return buildup(mfp.Sum(), factors.Last());
        }
    }
}
