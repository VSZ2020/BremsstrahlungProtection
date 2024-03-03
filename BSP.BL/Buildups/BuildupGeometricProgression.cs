using BSP.BL.Buildups.Common;
using System;
using System.Runtime.CompilerServices;

namespace BSP.BL.Buildups
{
    public class BuildupGeometricProgression : BaseBuildup
    {
        public const double tanh2_1 = 1.96402758007581688395;
        public const double tanh2 = -0.96402758007581688395;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Calculate(double ud, float a, float b, float c, float d, float xi, float barrierFactor = 1.0F)
        {
            //TODO: Проверить правильность преобразования к типу int
            int K = Convert.ToInt32(c * Math.Pow(ud, a) + d * (Math.Tanh(ud / xi - 2.0) - tanh2) / tanh2_1);

            if (K == 1)
                return (1.0 + (b - 1.0) * ud) * barrierFactor;
            return (1.0 + (b - 1.0) * (Math.Pow(K, ud) - 1.0) / (K - 1.0)) * barrierFactor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override double EvaluateBuildup(double mfp, float[] coefficients)
        {
            return Calculate(mfp, coefficients[0], coefficients[1], coefficients[2], coefficients[3], coefficients[4], coefficients.Length > 5 ? coefficients[5] : 1.0F);
        }
    }
}
