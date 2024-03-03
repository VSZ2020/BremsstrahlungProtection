using System;

namespace BSP.BL.Buildups.Common
{
    public abstract class BaseHeterogeneousBuildup
    {


        public BaseHeterogeneousBuildup(Func<double, float[], double> func)
        {
            buildup = func;
        }

        /// <summary>
        /// Делегат, ссылающийся на функцию расчета простого фактора накопления
        /// </summary>
        protected Func<double, float[], double> buildup;

        public abstract double EvaluateComplexBuildup(double[] mfp, float[][] factors);
    }
}
