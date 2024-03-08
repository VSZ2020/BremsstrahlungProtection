using BSP.BL.Calculation;
using System;
using System.Linq;

namespace BSP.BL.Geometries
{
    public sealed class Parallelepiped : BaseGeometry
    {
        public Parallelepiped(float[] dims, int[] discreteness)
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

        private ParallelepipedForm form;


        /// <summary>
        /// Вычисление радиальной составляющей излучения от прямоугольного параллелепипеда
        /// </summary>
        /// <param name="input"></param>
        /// <param name="EnergyIndex"></param>
        /// <returns></returns>
        public override double GetFluence(SingleEnergyInputData input)
        {
            return AlternativeIntegration(input);
        }

        [Obsolete]
        public double StandardIntegrator(SingleEnergyInputData input)
        {
            var layersThickness = input.Layers.Select(l => l.D).ToArray();
            var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();

            //Расстояние до точки детектирования
            var b = input.CalculationDistance + layersThickness.Sum();

            //Переменные для расчета
            var dx = form.Thickness / form.NThickness;
            var dy = form.Width / form.NWidth;
            var dz = form.Height / form.NHeight;

            //Начальные координаты точки регистрации
            var x0 = input.CalculationDistance + layersThickness.Sum() + form.Thickness;
            var y0 = form.Width / 2.0;
            var z0 = form.Height / 2.0;

            double sumIntegral = 0.0;

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

                        sumIntegral += totalLooseExp / R2 * dx * dy * dz * buildupFactor;
                    }
                }
            }

            var sourceVolume = form.GetNormalizationFactor();
            return !input.CancellationToken.IsCancellationRequested ? sumIntegral / sourceVolume / (4.0 * Math.PI) : 0;
        }


        public double AlternativeIntegration(SingleEnergyInputData input)
        {
            var layersThickness = input.Layers.Select(l => l.D).ToArray();
            var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();

            //Расстояние до точки детектирования
            var b = input.CalculationDistance + layersThickness.Sum();

            //Начальные координаты точки регистрации
            var x0 = input.CalculationDistance + layersThickness.Sum() + form.Thickness;
            var y0 = form.Width / 2.0;
            var z0 = form.Height / 2.0;

            var integral = Integrate((x, y, z) => 
            {
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

                return totalLooseExp / R2 * buildupFactor;
            },
            0, form.Thickness, form.NThickness,
            0, form.Width, form.NWidth,
            0, form.Height, form.NHeight,
            input.CancellationToken);
            return integral / form.GetNormalizationFactor() / (4.0 * Math.PI);
        }
    }
}
