using BSP.BL.Buildups.Common;
using System.Runtime.CompilerServices;

namespace BSP.BL.Buildups
{
    public class BuildupImprovedGeometricProgression : BaseBuildup
    {
        private const double ONE_MINUS_TANH_OF_MINUS_2 = 1.9640275801;
        private const double TANH_OF_MINUS_2 = -0.9640275801;
        private const double fm = 0.8;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Calculate(double mfp, double a, double b, double c, double d, double xi, double barrierFactor = 1.0F)
        {
            if (mfp > 100)
                mfp = 100;
            var result = mfp <= 40 ? CalculateBuildupLess40MFP(mfp, a, b, c, d, xi, barrierFactor) : CalculateBuildupGreater40MFP(mfp, a, b, c, d, xi, barrierFactor);
            if (double.IsNaN(result) || double.IsInfinity(result))
                result = 1.0;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override double EvaluateBuildup(double mfp, double[] coefficients)
        {
            return Calculate(mfp, coefficients[0], coefficients[1], coefficients[2], coefficients[3], coefficients[4], coefficients.Length > 5 ? coefficients[5] : 1.0F);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double GetK(double mfp, double a, double c, double d, double xi)
        {
            return (c * Math.Pow(mfp, a) + d * (Math.Tanh(mfp / xi - 2.0) - TANH_OF_MINUS_2) / ONE_MINUS_TANH_OF_MINUS_2);
        } 

        private static double CalculateBuildupLess40MFP(double mfp, double a, double b, double c, double d, double xi, double barrierFactor = 1.0F)
        {
            var K = (int)GetK(mfp, a, c, d, xi);
            if (K == 1)
                return (1.0 + (b - 1.0) * mfp) * barrierFactor;
            return (1.0 + (b - 1.0) * (Math.Pow(K, mfp) - 1.0) / (K - 1.0)) * barrierFactor;
        }

        private static double CalculateBuildupGreater40MFP(double mfp, double a, double b, double c, double d, double xi, double barrierFactor = 1.0F)
        {
            int K = 1;
            var K35 = GetK(35, a, c, d, xi);
            var K40 = GetK(40, a, c, d, xi);
            var ratio = (K40 - 1) / (K35 - 1);
            var ksi = (Math.Pow(mfp/35, 0.1) - 1) / (Math.Pow(40 / 35, 0.1) - 1);

            if (0 <= ratio && ratio <= 1)
            {
                K = (int)(1.0 + (K35 - 1) * Math.Pow(ratio, ksi));
            }
            else
                K = (int)(K35 * Math.Pow(K40/K35, Math.Pow(ksi, fm)));

            if (K == 1)
                return (1.0 + (b - 1.0) * mfp) * barrierFactor;
            return (1.0 + (b - 1.0) * (Math.Pow(K, mfp) - 1.0) / (K - 1.0)) * barrierFactor;
        }
    }
}
