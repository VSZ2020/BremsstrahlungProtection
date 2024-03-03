using BSP.BL.Calculation;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace BSP.BL.Geometries
{
    public class CylinderAxial : BaseGeometry
    {
        public CylinderAxial(float[] dims, int[] discreteness)
        {
            form = new CylinderForm()
            {
                Radius = dims[0],
                Height = dims[1],
                NRadius = discreteness[0],
                NHeight = discreteness[1]
            };
        }

        private CylinderForm form;

        public static string Name => "Cylinder Axial"; //(string)Application.Current.Resources["SourceFormCylinder"];

        public override double GetFluence(SingleEnergyInputData input)
        {
            var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();

            //Радиус цилиндра
            double R = form.Radius;
            double H = form.Height;
            //Координата точки регистрации флюенса
            double b = input.CalculationDistance + input.Layers.Select(l => l.D).Sum();

            //Первый интеграл по усеченному конусу
            var P1 = ExternalIntegralByAngle(
                //From To limits for external integral
                0, Math.Atan(R / (b + H)),
                //From To limits for inner integral
                angle => b * sec(angle), angle => (b + H) * sec(angle),
                form.NRadius, input.SourceDensity,
                input.massAttenuationFactors,
                layersMassThickness,
                input.CancellationToken,
                input.BuildupFactors,
                input.BuildupProcessor != null ? input.BuildupProcessor.EvaluateComplexBuildup : null);

            //Второй интеграл по телу вращения
            var P2 = ExternalIntegralByAngle(
                //From To limits for external integral
                Math.Atan(R / (b + H)), Math.Atan(R / b),
                //From To limits for inner integral
                angle => b * sec(angle), angle => R * cosec(angle),
                form.NRadius, input.SourceDensity,
                input.massAttenuationFactors,
                layersMassThickness,
                input.CancellationToken,
                input.BuildupFactors,
                input.BuildupProcessor != null ? input.BuildupProcessor.EvaluateComplexBuildup : null);

            var sourceVolume = form.GetNormalizationFactor();
            return 2.0 * Math.PI * (P1 + P2) / sourceVolume;
        }

        private double ExternalIntegralByAngle(double from, double to, Func<double, double> innerFrom, Func<double, double> innerTo, int N, double sourceDensity, float[] um, float[] layersDm, CancellationToken token, float[][] buildupFactors, Func<double[], float[][], double>? BuildupProcessor = null)
        {
            //Шаг интегрирования по углам
            double dtheta = (to - from) / N;

            double sum = 0.0;
            for (int i = 0; i < form.NRadius && !token.IsCancellationRequested; i++)
            {
                //Итерируемая тета
                double theta = from + dtheta / 2.0 + dtheta * i;

                sum += Math.Sin(theta) * InnerIntegralByRadius(innerFrom(theta), innerTo(theta), N, theta, sourceDensity, um, layersDm, token, buildupFactors, BuildupProcessor) * dtheta;
            }

            return !token.IsCancellationRequested ? sum : 0;
        }

        private double InnerIntegralByRadius(double from, double to, int N, double theta, double sourceDensity, float[] um, float[] layersDm, CancellationToken token, float[][] buildupFactors, Func<double[], float[][], double>? BuildupProcessor = null)
        {
            //Шаг интегрирования для интеграла по dr
            double dr = (to - from) / N;

            var sum = 0.0;
            for (int j = 0; j < N && !token.IsCancellationRequested; j++)
            {
                double r = from + dr / 2.0 + dr * j;

                //Рассчитываем начальные произведения u*d для всех слоев защиты, включая материал источника
                var UD = GetUDWithFactors(
                    massAttenuationFactors: um,
                    sourceDensity: sourceDensity,
                    selfabsorptionLength: r - from,
                    shieldsMassThicknesses: layersDm,
                    shieldEffecThicknessFactor: sec(theta));

                //Полные интегралы

                //Учет вклада поля рассеянного излучения                                                                       
                var buildupFactor = BuildupProcessor?.Invoke(UD, buildupFactors) ?? 1.0;

                sum += Math.Exp(-UD.Sum()) * buildupFactor;
            }
            return !token.IsCancellationRequested ? sum * dr : 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double sec(double x)
        {
            return 1.0 / Math.Cos(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double cosec(double x)
        {
            return 1.0 / Math.Sin(x);
        }

        //public override double GetFluence(SingleEnergyInputData input)
        //{
        //    var layersThickness = input.Layers.Select(l => l.D).ToArray();
        //    var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();

        //    //Радиус цилиндра
        //    double R = form.Radius;
        //    double H = form.Height;
        //    //Координата точки регистрации флюенса
        //    double b = input.CalculationDistance + layersThickness.Sum();

        //    //Параметры первого частичного интеграла
        //    double theta1 = Math.Atan(R / (b + H));
        //    double dtheta1 = theta1 / form.NRadius;

        //    //Параметры второго частичного интеграла
        //    double theta2 = Math.Atan(R / b);
        //    double dtheta2 = (theta2 - theta1) / form.NRadius;

        //    double externalIntergaral = 0.0;

        //    for (int i = 0; i < form.NRadius && !input.CancellationToken.IsCancellationRequested; i++)
        //    {
        //        //Итерируемая тета для первого интеграла
        //        double thetaI_1 = dtheta1 / 2.0 + dtheta1 * i;
        //        //Итерируемая тета для второго интеграла
        //        double thetaI_2 = theta1 + dtheta2 / 2.0 + dtheta2 * i;

        //        //Пределы для внутреннего интеграла первого частичного интеграла
        //        double bsecTheta_1 = b * sec(thetaI_1);
        //        double bHsecTheta_1 = (b + H) * sec(thetaI_1);

        //        //Пределы для внутреннего интеграла второго частичного интеграла
        //        double bsecTheta_2 = b * sec(thetaI_2);
        //        double rcosecTheta_2 = R * cosec(thetaI_2);

        //        //Шаг интегрирования для внутр. интеграла первого частичного интеграла по dr
        //        double dr1 = (bHsecTheta_1 - bsecTheta_1) / form.NRadius;

        //        //Шаг интегрирования для внутр. интеграла второго частичного интеграла по dr
        //        double dr2 = (rcosecTheta_2 - bsecTheta_2) / form.NRadius;

        //        for (int j = 0; j < form.NRadius && !input.CancellationToken.IsCancellationRequested; j++)
        //        {
        //            double r1 = bsecTheta_1 + dr1 / 2.0 + dr1 * j;
        //            double r2 = bsecTheta_2 + dr2 / 2.0 + dr2 * j;

        //            //Рассчитываем начальные проихведения u*d для всех слоев защиты, включая материал источника
        //            double[] UD_1 = GetUDWithFactors(
        //                massAttenuationFactors: input.massAttenuationFactors,
        //                sourceDensity: input.SourceDensity,
        //                selfabsorptionLength: r1 - bsecTheta_1,
        //                shieldsMassThicknesses: layersMassThickness,
        //                shieldEffecThicknessFactor: sec(thetaI_1));

        //            double[] UD_2 = GetUDWithFactors(
        //                massAttenuationFactors: input.massAttenuationFactors,
        //                sourceDensity: input.SourceDensity,
        //                selfabsorptionLength: r2 - bsecTheta_2,
        //                shieldsMassThicknesses: layersMassThickness,
        //                shieldEffecThicknessFactor: sec(thetaI_2)); ;

        //            //Полные интегралы
        //            double cI_1 = Math.Sin(thetaI_1) * Math.Exp(-UD_1.Sum()) * dr1 * dtheta1;
        //            double cI_2 = Math.Sin(thetaI_2) * Math.Exp(-UD_2.Sum()) * dr2 * dtheta2;

        //            //Учет вклада поля рассеянного излучения                                                                       
        //            var buildupFactor1 = input.BuildupProcessor?.EvaluateComplexBuildup(UD_1, input.BuildupFactors) ?? 1.0;
        //            var buildupFactor2 = input.BuildupProcessor?.EvaluateComplexBuildup(UD_2, input.BuildupFactors) ?? 1.0;
        //            if (double.IsNaN(buildupFactor1) || double.IsInfinity(buildupFactor1)) buildupFactor1 = 1;
        //            if (double.IsNaN(buildupFactor2) || double.IsInfinity(buildupFactor2)) buildupFactor2 = 1;

        //            externalIntergaral += cI_1 * buildupFactor1 + cI_2 * buildupFactor2;                                            //Суммируем расчетные значения в один общий интеграл

        //        }
        //    }

        //    var sourceVolume = form.GetNormalizationFactor();
        //    return !input.CancellationToken.IsCancellationRequested ? 2.0 * Math.PI * externalIntergaral / sourceVolume : 0;
        //}
    }
}
