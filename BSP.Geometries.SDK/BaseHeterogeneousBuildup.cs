namespace BSP.Geometries.SDK
{
    public abstract class BaseHeterogeneousBuildup
    {

        public BaseHeterogeneousBuildup(Func<double, double[], double> func)
        {
            buildup = func;
        }

        public abstract string Description { get; }

        /// <summary>
        /// Делегат, ссылающийся на функцию расчета простого фактора накопления
        /// </summary>
        protected Func<double, double[], double> buildup;

        public abstract double EvaluateComplexBuildup(double[] mfp, double[][] factors, double[]? complexBuildupFactors = null);
    }
}
