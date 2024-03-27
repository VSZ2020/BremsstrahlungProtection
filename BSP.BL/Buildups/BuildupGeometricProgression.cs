using BSP.BL.Buildups.Common;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BSP.BL.Buildups
{
    public class BuildupGeometricProgression : BaseBuildup
    {
        public const double ONE_MINUS_TANH_OF_MINUS_2 = 1.9640275801;
        public const double TANH_OF_MINUS_2 = -0.9640275801;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Calculate(double mfp, double a, double b, double c, double d, double xi, double barrierFactor = 1.0F)
        {
            if (mfp > 40)
                mfp = 40;

            var K = (int)(c * Math.Pow(mfp, a) + d * (Math.Tanh(mfp / xi - 2.0) - TANH_OF_MINUS_2) / ONE_MINUS_TANH_OF_MINUS_2);

            if (K == 1)
                return (1.0 + (b - 1.0) * mfp) * barrierFactor;
            var buildup = (1.0 + (b - 1.0) * (Math.Pow(K, mfp) - 1.0) / (K - 1.0)) * barrierFactor;
            
            return buildup;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override double EvaluateBuildup(double mfp, double[] coefficients)
        {
            return Calculate(mfp, coefficients[0], coefficients[1], coefficients[2], coefficients[3], coefficients[4], coefficients.Length > 5 ? coefficients[5] : 1.0F);
        }
    }
}
