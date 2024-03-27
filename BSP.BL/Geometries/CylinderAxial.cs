using BSP.BL.Calculation;
using System.Runtime.CompilerServices;
using BSP.BL.Integration;
using BSP.Geometries.SDK;

namespace BSP.BL.Geometries
{
    public class CylinderAxial : IGeometry
    {
        private CylinderForm form;

        public string Name => "Cylinder Axial";
        public string Description => "";

        public string Author => "IVS";
        
        #region GetDimensionsInfo
        public IEnumerable<DimensionsInfo> GetDimensionsInfo()
        {
            return new List<DimensionsInfo>()
            {
                new (){ Name = "Radius", DefaultValue = 10, Discreteness = 100},
                new (){ Name = "Height",DefaultValue = 30, Discreteness = 300},
                new (){ Name = "Angle", DefaultValue = 360, Discreteness = 10, IsValueEnabled = false},
            };
        }
        #endregion
        
        public double GetNormalizationFactor(float[] dims)
        {
            return dims[1] * Math.PI * dims[0] * dims[0];
        }
        
        #region GetFluence
        public double GetFluence(SingleEnergyInputData input)
        {
            form = new CylinderForm()
            {
                Radius = input.Dimensions[0],
                Height = input.Dimensions[1],
                Angle = input.Dimensions[2],
                NRadius = input.Discreteness[0],
                NHeight = input.Discreteness[1],
                NAngle = input.Discreteness[2]
            };
            
            return AlternativeIntegrator(input);
        } 
        #endregion

        #region StandardIntegrator
        public double StandardIntegrator(SingleEnergyInputData input)
        {
            var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();

            //Радиус цилиндра
            double R = form.Radius;
            double H = form.Height;
            //Координата точки регистрации флюенса
            double b = input.CalculationPoint.X - H;

            //Первый интеграл по усеченному конусу
            var P1 = ExternalIntegralByAngle(
                //From To limits for external integral
                0, Math.Atan(R / (b + H)),
                //From To limits for inner integral
                angle => b * sec(angle), angle => (b + H) * sec(angle),
                form.NAngle, form.NRadius,
                input.SourceDensity,
                input.MassAttenuationFactors,
                layersMassThickness,
                input.CancellationToken,
                input.BuildupFactors,
                input.BuildupProcessor != null ? input.BuildupProcessor.EvaluateComplexBuildup : null,
                input.IsSelfAbsorptionAllowed);

            //Второй интеграл по телу вращения
            var P2 = ExternalIntegralByAngle(
                //From To limits for external integral
                Math.Atan(R / (b + H)), Math.Atan(R / b),
                //From To limits for inner integral
                angle => b * sec(angle), angle => R * cosec(angle),
                form.NAngle, form.NRadius, 
                input.SourceDensity,
                input.MassAttenuationFactors,
                layersMassThickness,
                input.CancellationToken,
                input.BuildupFactors,
                input.BuildupProcessor != null ? input.BuildupProcessor.EvaluateComplexBuildup : null,
                input.IsSelfAbsorptionAllowed);

            var sourceVolume = form.GetNormalizationFactor();
            return 2.0 * Math.PI * (P1 + P2) / sourceVolume / (4.0 * Math.PI);
        } 
        #endregion

        #region ExternalIntegralByAngle
        private double ExternalIntegralByAngle(double from, double to, Func<double, double> innerFrom, Func<double, double> innerTo, int NAngle, int NRadius, double sourceDensity, double[] um, float[] layersDm, CancellationToken token, double[][] buildupFactors, Func<double[], double[][], double[], double>? BuildupProcessor = null, bool isSelfabsorptionAllowed = true)
        {
            //Шаг интегрирования по углам
            double dtheta = (to - from) / NAngle;

            double sum = 0.0;
            for (int i = 0; i < form.NRadius && !token.IsCancellationRequested; i++)
            {
                //Итерируемая тета
                double theta = from + dtheta / 2.0 + dtheta * i;

                sum += Math.Sin(theta) * InnerIntegralByRadius(innerFrom(theta), innerTo(theta), NRadius, theta, sourceDensity, um, layersDm, token, buildupFactors, BuildupProcessor, isSelfabsorptionAllowed) * dtheta;
            }

            return !token.IsCancellationRequested ? sum : 0;
        }
        #endregion

        #region InnerIntegralByRadius
        private double InnerIntegralByRadius(double from, double to, int N, double theta, double sourceDensity, double[] um, float[] layersDm, CancellationToken token, double[][] buildupFactors, Func<double[], double[][], double[]?, double>? BuildupProcessor = null, bool isSelfabsorptionAllowed = true)
        {
            //Шаг интегрирования для интеграла по dr
            double dr = (to - from) / N;

            var sum = 0.0;
            for (int j = 0; j < N && !token.IsCancellationRequested; j++)
            {
                double r = from + dr / 2.0 + dr * j;

                //Рассчитываем начальные произведения u*d для всех слоев защиты, включая материал источника
                var UD = IGeometry.GetUdWithFactors(
                    massAttenuationFactors: um,
                    sourceDensity: sourceDensity,
                    selfabsorptionLength: r - from,
                    shieldsMassThicknesses: layersDm,
                    shieldEffecThicknessFactor: sec(theta));

                if (!isSelfabsorptionAllowed)
                    UD = UD.Skip(1).ToArray();

                //Учет вклада поля рассеянного излучения                                                                       
                var buildupFactor = BuildupProcessor != null && UD.Length > 0 ? BuildupProcessor.Invoke(UD, buildupFactors, null) : 1.0;

                sum += Math.Exp(-UD.Sum()) * buildupFactor;
            }
            return !token.IsCancellationRequested ? sum * dr : 0;
        } 
        #endregion


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

        #region AlternativeIntegrator
        public double AlternativeIntegrator(SingleEnergyInputData input)
        {
            var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();

            //Радиус цилиндра
            double R = form.Radius;
            double H = form.Height;
            //Координата точки регистрации флюенса
            double b = input.CalculationPoint.X - H;

            double func(double theta, double r)
            {
                //Рассчитываем начальные произведения u*d для всех слоев защиты, включая материал источника
                var UD = IGeometry.GetUdWithFactors(
                            massAttenuationFactors: input.MassAttenuationFactors,
                            sourceDensity: input.SourceDensity,
                            selfabsorptionLength: r - b * sec(theta),
                            shieldsMassThicknesses: layersMassThickness,
                            shieldEffecThicknessFactor: sec(theta));

                if (!input.IsSelfAbsorptionAllowed)
                    UD = UD.Skip(1).ToArray();

                //Учет вклада поля рассеянного излучения                                                                       
                var buildupFactor = UD.Length > 0 && input.BuildupProcessor != null ? input.BuildupProcessor.EvaluateComplexBuildup(UD, input.BuildupFactors) : 1.0;

                if (double.IsNaN(buildupFactor) || double.IsInfinity(buildupFactor))
                    buildupFactor = 0.0;

                return Math.Exp(-UD.Sum()) * buildupFactor;
            }

            var P1 = Integrators.Integrate(
                theta => Integrators.Integrate(
                    r => Math.Sin(theta) * func(theta, r), b * sec(theta), (b + H) * sec(theta), form.NAngle, input.CancellationToken),
                0, Math.Atan(R / (b + H)), form.NHeight, input.CancellationToken);

            var P2 = Integrators.Integrate(
                theta => Integrators.Integrate(
                    r => Math.Sin(theta) * func(theta, r), b * sec(theta), R * cosec(theta), form.NAngle, input.CancellationToken),
                Math.Atan(R / (b + H)), Math.Atan(R / b), form.NHeight, input.CancellationToken);

            var sourceVolume = form.GetNormalizationFactor();
            return 2.0 * Math.PI * (P1 + P2) / sourceVolume / (4.0 * Math.PI);
        } 
        #endregion
    }
}
