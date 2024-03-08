using BSP.BL.Buildups.Common;
using System;

namespace BSP.BL.Buildups
{
    public class BuildupTaylor : BaseBuildup
    {
        public override double EvaluateBuildup(double mfp, double[] coefficients)
        {
            var A = coefficients[0];
            var alpha1 = coefficients[1];
            var alpha2 = coefficients[2];
            return A * Math.Exp(-mfp * alpha1) + (1.0 - A) * Math.Exp(-mfp * alpha2);
        }
    }
}
