using BSP.BL.Calculation;
using System.Linq;

namespace BSP.BL.Geometries
{
    public abstract class BaseGeometry
    {
        public abstract double GetFluence(SingleEnergyInputData input);

        public double[] GetUD(float[] u, float[] d)
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
        public double[] GetUDWithFactors(float[] massAttenuationFactors, double sourceDensity, double selfabsorptionLength, float[] shieldsMassThicknesses, double shieldEffecThicknessFactor)
        {
            var ud = new double[massAttenuationFactors.Length];
            ud[0] = massAttenuationFactors[0] * sourceDensity * selfabsorptionLength;

            for (var i = 0; i < shieldsMassThicknesses.Length; i++)
                ud[i + 1] = massAttenuationFactors[i + 1] * shieldsMassThicknesses[i] * shieldEffecThicknessFactor;
            return ud;
        }
    }
}
