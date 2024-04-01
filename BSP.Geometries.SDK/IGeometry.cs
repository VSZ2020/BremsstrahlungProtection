namespace BSP.Geometries.SDK
{
    public interface IGeometry
    {
        /// <summary>
        /// Название геометрии
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Автор геометрии
        /// </summary>
        public string Author { get; }

        /// <summary>
        /// Описание геометрии
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Рассчитывает флюенс частиц
        /// </summary>
        /// <param name="dims"></param>
        /// <param name="discreteness"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public double GetFluence(SingleEnergyInputData input);

        /// <summary>
        /// Возвращает информацию о необходимых геометрических размерах источника
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DimensionsInfo> GetDimensionsInfo();

        public double GetNormalizationFactor(float[] dims);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="massAttenuationFactors">Массовые коэффициенты ослабления для слоев защиты, включая первым коэффициент для материала источника</param>
        /// <param name="shieldsMassThicknesses">Массовые толщины слоев защиты</param>
        /// <param name="sourceDensity">Плотность материала источника</param>
        /// <param name="selfabsorptionLength"></param>
        /// <param name="shieldEffecThicknessFactor"></param>
        /// <returns></returns>
        protected static double[] GetUdWithFactors(double[] massAttenuationFactors, double sourceDensity, double selfabsorptionLength, float[] shieldsMassThicknesses, double shieldEffecThicknessFactor)
        {
            var ud = new double[massAttenuationFactors.Length];
            ud[0] = massAttenuationFactors[0] * sourceDensity * selfabsorptionLength;

            for (var i = 0; i < shieldsMassThicknesses.Length; i++)
                ud[i + 1] = massAttenuationFactors[i + 1] * shieldsMassThicknesses[i] * shieldEffecThicknessFactor;
            return ud;
        }
    }
}
