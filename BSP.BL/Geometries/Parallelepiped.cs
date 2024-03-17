using BSP.BL.Calculation;

namespace BSP.BL.Geometries
{
    public sealed class Parallelepiped : BaseGeometry
    {
        public Parallelepiped(float[] dims, int[] discreteness) : base(dims, discreteness) { }

        private ParallelepipedForm form;

        #region AssignDimensions
        public override void AssignDimensions(float[] dims, int[] discreteness)
        {
            form = new ParallelepipedForm()
            {
                Thickness = dims[0],
                Width = dims[1],
                Height = dims[2],
                NThickness = discreteness[0],
                NWidth = discreteness[1],
                NHeight = discreteness[2]
            };
        } 
        #endregion

        #region GetFluence
        /// <summary>
        /// Вычисление радиальной составляющей излучения от прямоугольного параллелепипеда. Метод средних прямоугольников
        /// </summary>
        /// <param name="input"></param>
        /// <param name="EnergyIndex"></param>
        /// <returns></returns>
        public override double GetFluence(SingleEnergyInputData input)
        {
            return AlternativeIntegration(input);
        } 
        #endregion

        #region StandardIntegrator
        [Obsolete]
        public double StandardIntegrator(SingleEnergyInputData input)
        {
            var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();

            //Расстояние до точки детектирования
            var b = input.Layers.Select(l => l.D).Sum();

            //Переменные для расчета
            var dx = form.Thickness / form.NThickness;
            var dy = form.Width / form.NWidth;
            var dz = form.Height / form.NHeight;

            //Начальные координаты точки регистрации
            var x0 = input.CalculationPoint.X;
            var y0 = input.CalculationPoint.Y;
            var z0 = input.CalculationPoint.Z;

            double sum = 0.0;

            for (int i = 0; i < form.NThickness && !input.CancellationToken.IsCancellationRequested; i++)
            {
                var x = 0.5 * dx / 2 + dx * i;
                for (int j = 0; j < form.NWidth && !input.CancellationToken.IsCancellationRequested; j++)
                {
                    var y = 0.5 * dy + dy * j;
                    for (int k = 0; k < form.NHeight && !input.CancellationToken.IsCancellationRequested; k++)
                    {
                        var z = 0.5 * dz + dz * k;
                        var c = form.Thickness - x;

                        //Квадрат расстояния от дискретного объема до точки регистрации R
                        var R2 = (x - x0) * (x - x0) + (y - y0) * (y - y0) + (z - z0) * (z - z0);
                        var R = Math.Sqrt(R2);

                        //Длина самопоглощения в источнике
                        var xe = R * c / (c + b);

                        //Коэффициент перехода от толщины защиты d к эффективной толщине ослабления в защите y
                        var m = R / (c + b);
                        var ud = GetUDWithFactors(input.massAttenuationFactors, input.SourceDensity, xe, layersMassThickness, m);

                        if (!input.IsSelfAbsorptionAllowed)
                            ud = ud.Skip(1).ToArray();

                        double totalLooseExp = Math.Exp(-ud.Sum());

                        //Расчет вклада поля рассеянного излучения
                        double buildupFactor = input.BuildupProcessor != null && ud.Length > 0 ? input.BuildupProcessor.EvaluateComplexBuildup(ud, input.BuildupFactors) : 1.0;

                        sum += totalLooseExp / R2 * dx * dy * dz * buildupFactor;
                    }
                }
            }

            var sourceVolume = form.GetNormalizationFactor();
            return !input.CancellationToken.IsCancellationRequested ? sum / sourceVolume / (4.0 * Math.PI) : 0;
        }
        #endregion

        #region AlternativeIntegration
        public double AlternativeIntegration(SingleEnergyInputData input)
        {
            var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();
            var b = input.Layers.Select(l => l.D).Sum();

            //Начальные координаты точки регистрации. Абсолютные координаты
            var x0 = input.CalculationPoint.X;
            var y0 = input.CalculationPoint.Y;
            var z0 = input.CalculationPoint.Z;

            var integral = Integrate((x, y, z) =>
            {
                var c = form.Thickness - x;

                //Квадрат расстояния от дискретного объема до точки регистрации R
                var R2 = (x - x0) * (x - x0) + (y - y0) * (y - y0) + (z - z0) * (z - z0);
                var R = Math.Sqrt(R2);

                //Длина самопоглощения в источнике
                var xe = R * c / (x0 - x);

                //Коэффициент перехода от толщины защиты d к эффективной толщине ослабления в защите y
                var m = R / (x0 - x);
                var ud = GetUDWithFactors(input.massAttenuationFactors, input.SourceDensity, xe, layersMassThickness, m);

                if (!input.IsSelfAbsorptionAllowed)
                    ud = ud.Skip(1).ToArray();

                double totalLooseExp = Math.Exp(-ud.Sum());

                //Расчет вклада поля рассеянного излучения
                double buildupFactor = input.BuildupProcessor != null && ud.Length > 0 ? input.BuildupProcessor.EvaluateComplexBuildup(ud, input.BuildupFactors) : 1.0;

                return totalLooseExp / R2 * buildupFactor;
            },
            0, form.Thickness, form.NThickness,
            0, form.Width, form.NWidth,
            0, form.Height, form.NHeight,
            input.CancellationToken);
            return integral / form.GetNormalizationFactor() / (4.0 * Math.PI);
        }
        #endregion
    }
}
