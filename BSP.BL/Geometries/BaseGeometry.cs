using BSP.BL.Calculation;
using System.Linq;

namespace BSP.BL.Geometries
{
    public abstract class BaseGeometry
    {
        public abstract double GetFluence(SingleEnergyInputData input);

        protected double[] GetUD(float[] u, float[] d)
        {
            return Enumerable.Range(0, u.Length).Select(i => u[i] * d[i]).Cast<double>().ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="massAttenuationFactors">Массовые коэффициенты ослабления для слоев защиты, включая первым коэффициент для материала источника</param>
        /// <param name="shieldsMassThicknesses">Массовые толщины слоев защиты</param>
        /// <param name="sourceDensity">Плотность материала источника</param>
        /// <param name="selfabsorptionLength"></param>
        /// <param name="shieldEffecThicknessFactor"></param>
        /// <returns></returns>
        protected double[] GetUDWithFactors(float[] massAttenuationFactors, double sourceDensity, double selfabsorptionLength, float[] shieldsMassThicknesses, double shieldEffecThicknessFactor)
        {
            var ud = new double[massAttenuationFactors.Length];
            ud[0] = massAttenuationFactors[0] * sourceDensity * selfabsorptionLength;

            for (var i = 0; i < shieldsMassThicknesses.Length; i++)
                ud[i + 1] = massAttenuationFactors[i + 1] * shieldsMassThicknesses[i] * shieldEffecThicknessFactor;
            return ud;
        }

        protected double TrapezoidMethod(Func<double, double> func, double start, double end, int N, CancellationToken token)
        {
            var step = (end - start) / N;
            var sum = 0.5 * (func(start) + func(end));
            for (var i = 1; i < N - 1 && !token.IsCancellationRequested; i++)
            {
                sum += func(start + i * step);
            }
            return !token.IsCancellationRequested ? sum * step : 0;
        }

        protected double SimpsonMethod(Func<double, double> func, double start, double end, int N, CancellationToken token)
        {
            if (N % 2 != 0)
                N += 1;

            var step = (end - start) / N;
            var m = 4;
            var sum = func(start) + func(end);
            for (var i = 0; i < N - 1 && !token.IsCancellationRequested; i++)
            {
                sum += m * func(start + (i + 1) * step);
                m = 6 - m;
            }
            return !token.IsCancellationRequested ? sum * step / 3.0 : 0;
        }
    }
}
