using BSP.BL.Interpolation.Functions;

namespace BSP.BL.Interpolation
{
    public class Interpolator
    {
        /// <summary>
        /// Проводит интерполяцию в в зависимости от вида сплайна 
        /// </summary>
        /// <param name="X">Табличные значения энергий</param>
        /// <param name="Y">Табличные значения параметра</param>
        /// <param name="InterpolationType">Тип интерполяционного полинома</param>
        /// <param name="NewX">Значения энергий, для которых получают новые значения</param>
        /// <returns></returns>
        public static double[] Interpolate(double[] X, double[] Y, double[] NewX, InterpolationType InterpolationType = InterpolationType.Linear, AxisLogScale interpolationScaleType = AxisLogScale.None)
        {
            return InterpolationType == InterpolationType.Cubic ? new CSpline().Interpolate(X, Y, NewX, interpolationScaleType) : new LSpline().Interpolate(X, Y, NewX, interpolationScaleType);
        }


    }
}
