using BSP.BL.Calculation;

namespace BSP.BL.Geometries
{
    public class CylinderRadial : BaseGeometry
    {
        public CylinderRadial(float[] dims, int[] discreteness)
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

        public override double GetFluence(SingleEnergyInputData input)
        {
            return AlternativeIntegrator(input);
        }

        public double StandardIntegrator(SingleEnergyInputData input)
        {
            //Задаем размеры источника
            double R = form.Radius;
            double h = form.Height / 2.0;                     //берем для расчета половину высоты

            var layersThickness = input.Layers.Select(l => l.D).ToArray();
            var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();

            //Расстояние до точки измерения начиная от поверхности контейнера источника
            double b = input.CalculationDistance + R + layersThickness.Sum();

            //Объявляем перечень используемых переменных-итераций
            double rho = 0.0; double z = 0.0; double phi = 0.0;

            //Шаги интегрирования
            double dro = R / form.NRadius;  //0..R
            double dz = h / form.NHeight;   //0..H/2
            double dPhi = Math.PI / form.NRadius;    //0..180

            double currIntegral = 0.0;
            for (int i = 0; i < form.NRadius && !input.CancellationToken.IsCancellationRequested; i++)
            {
                rho = dro / 2.0 + dro * i;
                for (int j = 0; j < form.NHeight && !input.CancellationToken.IsCancellationRequested; j++)
                {
                    z = dz / 2.0 + dz * j;
                    for (int k = 0; k < form.NRadius && !input.CancellationToken.IsCancellationRequested; k++)
                    {
                        phi = dPhi / 2.0 + dPhi * k;

                        double cFull = z * z + rho * rho + b * b - 2.0 * rho * b * Math.Cos(phi);
                        double c = rho * rho + b * b - 2 * rho * b * Math.Cos(phi);

                        //Длина самопоглощения в источнике
                        double selfabsorptionLength = (rho * rho - rho * b * Math.Cos(phi) +
                                        Math.Sqrt(R * R * c - rho * rho * b * b * Math.Sin(phi) * Math.Sin(phi))) * Math.Sqrt(cFull) / c;
                        var effShieldThicknessFactor = Math.Sqrt(cFull) / (b - rho * Math.Cos(phi));

                        double[] ud = GetUDWithFactors(
                            massAttenuationFactors: input.massAttenuationFactors,
                            sourceDensity: input.SourceDensity,
                            selfabsorptionLength: selfabsorptionLength,
                            shieldsMassThicknesses: layersMassThickness,
                            shieldEffecThicknessFactor: effShieldThicknessFactor);

                        //Полная экспонента ослабления
                        double totalLooseExp = Math.Exp(-ud.Sum());

                        //Расчет вклада поля рассеянного излучения
                        double buildupFactor = input.BuildupProcessor != null ? input.BuildupProcessor.EvaluateComplexBuildup(ud, input.BuildupFactors) : 1.0;

                        //Текущее значение интеграла
                        currIntegral += rho * totalLooseExp / cFull * dro * dz * dPhi * buildupFactor;
                    }
                }
            }

            var sourceVolume = form.GetNormalizationFactor();
            return !input.CancellationToken.IsCancellationRequested ? 2.0 * currIntegral / sourceVolume : 0;
        }

        public double AlternativeIntegrator(SingleEnergyInputData input)
        {
            //Задаем размеры источника
            double R = form.Radius;
            double h = form.Height / 2.0;                     //берем для расчета половину высоты

            var layersThickness = input.Layers.Select(l => l.D).ToArray();
            var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();

            //Расстояние до точки измерения начиная от поверхности контейнера источника
            double b = input.CalculationDistance + R + layersThickness.Sum();

            double ByRho(double rho)
            {

                double ByPhi(double phi)
                {

                    double ByZ(double z)
                    {
                        double cFull = z * z + rho * rho + b * b - 2.0 * rho * b * Math.Cos(phi);
                        double c = rho * rho + b * b - 2 * rho * b * Math.Cos(phi);

                        //Длина самопоглощения в источнике
                        double selfabsorptionLength = (rho * rho - rho * b * Math.Cos(phi) +
                                        Math.Sqrt(R * R * c - rho * rho * b * b * Math.Sin(phi) * Math.Sin(phi))) * Math.Sqrt(cFull) / c;
                        var effShieldThicknessFactor = Math.Sqrt(cFull) / (b - rho * Math.Cos(phi));

                        double[] ud = GetUDWithFactors(
                            massAttenuationFactors: input.massAttenuationFactors,
                            sourceDensity: input.SourceDensity,
                            selfabsorptionLength: selfabsorptionLength,
                            shieldsMassThicknesses: layersMassThickness,
                            shieldEffecThicknessFactor: effShieldThicknessFactor);

                        //Полная экспонента ослабления
                        double totalLooseExp = Math.Exp(-ud.Sum());

                        //Расчет вклада поля рассеянного излучения
                        double buildupFactor = input.BuildupProcessor != null ? input.BuildupProcessor.EvaluateComplexBuildup(ud, input.BuildupFactors) : 1.0;

                        //Текущее значение интеграла
                        return rho * totalLooseExp / cFull * buildupFactor;
                    }

                    return SimpsonMethod(ByZ, 0, h, form.NHeight, input.CancellationToken);
                }

                return SimpsonMethod(ByPhi, 0, Math.PI, form.NRadius, input.CancellationToken);
            }

            return 2.0 * SimpsonMethod(ByRho, 0, R, form.NRadius, input.CancellationToken) / form.GetNormalizationFactor();
        }
    }
}
