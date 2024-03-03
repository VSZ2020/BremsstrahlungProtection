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
        public static float[] Interpolate(float[] X, float[] Y, float[] NewX, InterpolationType InterpolationType = InterpolationType.Linear)
        {
            return InterpolationType == InterpolationType.Cubic ? new CSpline().Interpolate(X, Y, NewX) : new LSpline().Interpolate(X, Y, NewX);
        }


    }
}
