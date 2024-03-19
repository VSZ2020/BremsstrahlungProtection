using BSP.BL.Calculation;
using System.Diagnostics;

namespace BSP.BL.Geometries
{
    public class CylinderRadial : BaseGeometry
    {
        public CylinderRadial(float[] dims, int[] discreteness): base(dims, discreteness) { }

        private CylinderForm form;

        #region AssignDimensions
        public override void AssignDimensions(float[] dims, int[] discreteness)
        {
            form = new CylinderForm()
            {
                Radius = dims[0],
                Height = dims[1],
                NRadius = discreteness[0],
                NHeight = discreteness[1]
            };
        }
        #endregion

        #region GetFluence
        public override double GetFluence(SingleEnergyInputData input)
        {
            return AlternativeIntegrator(input);
        } 
        #endregion

        #region StandardIntegrator
        public double StandardIntegrator(SingleEnergyInputData input)
        {
            //Задаем размеры источника
            var R = form.Radius;

            var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();

            //Расстояние до точки измерения
            var b = input.CalculationPoint.X;
            var phi0 = Math.Atan2(input.CalculationPoint.Y, input.CalculationPoint.X);
            var z0 = input.CalculationPoint.Z;

            //Шаги интегрирования
            var dro = R / form.NRadius;
            var dz = form.Height / form.NHeight;
            var dPhi = 2 * Math.PI / form.NRadius;

            double currIntegral = 0.0;
            for (int i = 0; i < form.NRadius && !input.CancellationToken.IsCancellationRequested; i++)
            {
                var rho = 0.5 * dro + dro * i;
                for (int j = 0; j < form.NHeight && !input.CancellationToken.IsCancellationRequested; j++)
                {
                    var z = 0.5 * dz + dz * j;
                    for (int k = 0; k < form.NRadius && !input.CancellationToken.IsCancellationRequested; k++)
                    {
                        var phi = 0.5 * dPhi + dPhi * k;

                        double cFull = (z - z0) * (z - z0) + rho * rho + b * b - 2.0 * rho * b * Math.Cos(phi - phi0);
                        //double c = rho * rho + b * b - 2.0 * rho * b * Math.Cos(phi);

                        //Длина самопоглощения в источнике
                        double selfabsorptionLength = SelfabsorptionLength(rho, z - z0, phi - phi0, b, R);
                        var effShieldThicknessFactor = Math.Sqrt(cFull) / (b - rho * Math.Cos(phi - phi0));

                        double[] mfp = GetUDWithFactors(
                            massAttenuationFactors: input.massAttenuationFactors,
                            sourceDensity: input.SourceDensity,
                            selfabsorptionLength: selfabsorptionLength,
                            shieldsMassThicknesses: layersMassThickness,
                            shieldEffecThicknessFactor: effShieldThicknessFactor);

                        if (!input.IsSelfAbsorptionAllowed)
                            mfp = mfp.Skip(1).ToArray();

                        //Полная экспонента ослабления
                        double totalLooseExp = Math.Exp(-mfp.Sum());


                        //Расчет вклада поля рассеянного излучения
                        double buildupFactor = input.BuildupProcessor != null && mfp.Length > 0 ? input.BuildupProcessor.EvaluateComplexBuildup(mfp, input.BuildupFactors) : 1.0;

                        //Текущее значение интеграла
                        currIntegral += rho * totalLooseExp / cFull * buildupFactor;
                    }
                }
            }

            var sourceVolume = form.GetNormalizationFactor();
            return !input.CancellationToken.IsCancellationRequested ? currIntegral * dro * dz * dPhi / sourceVolume / (4.0 * Math.PI) : 0;
        }
        #endregion


        #region AlternativeIntegrator
        public double AlternativeIntegrator(SingleEnergyInputData input)
        {
            //Задаем размеры источника
            double R = form.Radius;
            double H = form.Height;

            var rho0 = Math.Sqrt(input.CalculationPoint.X * input.CalculationPoint.X + input.CalculationPoint.Y + input.CalculationPoint.Y);
            var phi0 = input.CalculationPoint.X > 0 ? Math.Atan2(input.CalculationPoint.Y, input.CalculationPoint.X) : 0;
            var z0 = input.CalculationPoint.Z;

            var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();

            //Расстояние до точки измерения начиная от поверхности контейнера источника
            //var b = input.CalculationPoint.X;

            var sourceVolume = form.GetNormalizationFactor();
            double func(double rho, double phi, double z)
            {
                var cFull = rho * rho + rho0 * rho0 + (z - z0) * (z - z0) - 2.0 * rho * rho0 * Math.Cos(phi - phi0);

                //Длина самопоглощения в источнике
                double selfabsorptionLength = SelfabsorptionLength(rho, z - z0, phi - phi0, rho0, R);
                var effShieldThicknessFactor = Math.Sqrt(cFull) / (rho0 - rho * Math.Cos(phi - phi0));

                double[] ud = GetUDWithFactors(
                    massAttenuationFactors: input.massAttenuationFactors,
                    sourceDensity: input.SourceDensity,
                    selfabsorptionLength: selfabsorptionLength,
                    shieldsMassThicknesses: layersMassThickness,
                    shieldEffecThicknessFactor: effShieldThicknessFactor);

                if (!input.IsSelfAbsorptionAllowed)
                    ud = ud.Skip(1).ToArray();

                //Полная экспонента ослабления
                double totalLooseExp = Math.Exp(-ud.Sum());

                //Расчет вклада поля рассеянного излучения
                double buildupFactor = input.BuildupProcessor != null && ud.Length > 0 ? input.BuildupProcessor.EvaluateComplexBuildup(ud, input.BuildupFactors) : 1.0;

                var result = rho * totalLooseExp / cFull * buildupFactor;

                return result;
            }

            var integral = Integrate(func,
                0, R, form.NRadius,
                0, 2.0 * Math.PI, form.NRadius,
                0, H, form.NHeight,
                input.CancellationToken);

            return integral / sourceVolume / (4 * Math.PI);
        }
        #endregion

        #region SelfabsorptionLength
        /// <summary>
        /// Длина самопоглощения
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="z"></param>
        /// <param name="phi"></param>
        /// <param name="b"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public double SelfabsorptionLength(double rho, double z, double phi, double b, double R)
        {
            var rho_b_z = z * z + rho * rho + b * b - 2.0 * rho * b * Math.Cos(phi);
            var rho_b = rho * rho + b * b - 2.0 * rho * b * Math.Cos(phi);
            var x = (rho * rho - rho * b * Math.Cos(phi) + Math.Sqrt(R * R * rho_b - rho * rho * b * b * Math.Sin(phi) * Math.Sin(phi))) * Math.Sqrt(rho_b_z) / rho_b;
            return x > 0 ? x : 0;
        } 
        #endregion
    }
}
