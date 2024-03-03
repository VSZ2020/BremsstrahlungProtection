using BSP.BL.Calculation;
using System;
using System.Linq;

namespace BSP.BL.Geometries
{
    public class Parallelepiped : BaseGeometry
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

        public static string Name => "Parallelepiped";//(string)Application.Current.Resources["SourceFormParallelepiped"];


        /// <summary>
        /// Вычисление радиальной составляющей излучения от прямоугольного параллелепипеда
        /// </summary>
        /// <param name="input"></param>
        /// <param name="EnergyIndex"></param>
        /// <returns></returns>
        public override double GetFluence(SingleEnergyInputData input)
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
                var x = dx / 2 + dx * i;
                for (int j = 0; j < form.NWidth && !input.CancellationToken.IsCancellationRequested; j++)
                {
                    var y = dy / 2 + dy * j;
                    for (int k = 0; k < form.NHeight && !input.CancellationToken.IsCancellationRequested; k++)
                    {
                        var z = dz / 2 + dz * k;
                        var c = form.Thickness - x;

                        //Квадрат расстояния от дискретного объема до точки регистрации R
                        var R2 = (x - x0) * (x - x0) + (y - y0) * (y - y0) + (z - z0) * (z - z0);
                        var R = Math.Sqrt(R2);

                        //Длина самопоглощения в источнике
                        var xe = R * c / (c + b);

                        //Коэффициент перехода от толщины защиты d к эффективной толщине ослабления в защите y
                        var m = R / (c + b);
                        var ud = GetUDWithFactors(input.massAttenuationFactors, input.SourceDensity, xe, layersMassThickness, m);

                        double totalLooseExp = Math.Exp(-ud.Sum());

                        //Расчет вклада поля рассеянного излучения
                        double buildupFactor = input.BuildupProcessor != null ? input.BuildupProcessor.EvaluateComplexBuildup(ud, input.BuildupFactors) : 1.0;

                        sumIntegral += totalLooseExp / R2 * dx * dy * dz * buildupFactor;
                    }
                }
            }
            if (input.CancellationToken.IsCancellationRequested)
            {
                return -1.0;
            }
            var sourceVolume = form.GetNormalizationFactor();
            return sumIntegral / sourceVolume / (4.0 * Math.PI);
        }
    }
}
