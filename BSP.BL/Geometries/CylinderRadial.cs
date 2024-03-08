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
            return StandardIntegrator(input);
        }

        public double StandardIntegrator(SingleEnergyInputData input)
        {
            //Задаем размеры источника
            var R = form.Radius;
            var h = 0.5 * form.Height;                     //берем для расчета половину высоты

            var layersThickness = input.Layers.Select(l => l.D).ToArray();
            var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();

            //Расстояние до точки измерения начиная от поверхности контейнера источника
            var b = input.CalculationDistance + R + layersThickness.Sum();

            //Шаги интегрирования
            var dro = R / form.NRadius;  //0..R
            var dz = h / form.NHeight;   //0..H/2
            var dPhi = Math.PI / form.NRadius;    //0..180

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

                        //double cFull = z * z + rho * rho + b * b - 2.0 * rho * b * Math.Cos(phi);
                        //double c = rho * rho + b * b - 2.0 * rho * b * Math.Cos(phi);

                        //Длина самопоглощения в источнике
                        double selfabsorptionLength = (rho * rho - rho * b * Math.Cos(phi) +
                                        Math.Sqrt(R * R * (rho * rho + b * b - 2.0 * rho * b * Math.Cos(phi)) - rho * rho * b * b * Math.Sin(phi) * Math.Sin(phi))) * Math.Sqrt(z * z + rho * rho + b * b - 2.0 * rho * b * Math.Cos(phi)) / (rho * rho + b * b - 2.0 * rho * b * Math.Cos(phi));
                        var effShieldThicknessFactor = Math.Sqrt(z * z + rho * rho + b * b - 2.0 * rho * b * Math.Cos(phi)) / (b - rho * Math.Cos(phi));

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

                        //Текущее значение интеграла
                        currIntegral += rho * totalLooseExp / (z * z + rho * rho + b * b - 2.0 * rho * b * Math.Cos(phi)) * buildupFactor;
                    }
                }
            }

            var sourceVolume = form.GetNormalizationFactor();
            return !input.CancellationToken.IsCancellationRequested ? 2.0 * currIntegral * dro * dz * dPhi / sourceVolume : 0;
        }

        public double AlternativeIntegrator(SingleEnergyInputData input)
        {
            //Задаем размеры источника
            double R = form.Radius;
            double h = 0.5 * form.Height;                     //берем для расчета половину высоты

            var layersThickness = input.Layers.Select(l => l.D).ToArray();
            var layersMassThickness = input.Layers.Select(l => l.Dm).ToArray();

            //Расстояние до точки измерения начиная от поверхности контейнера источника
            double b = input.CalculationDistance + R + layersThickness.Sum();

            var sourceVolume = form.GetNormalizationFactor();

            return 2.0 * Integrate((rho, phi, z) => 
            {
                double cFull = z * z + rho * rho + b * b - 2.0 * rho * b * Math.Cos(phi);
                double c = rho * rho + b * b - 2.0 * rho * b * Math.Cos(phi);

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

                if (!input.IsSelfAbsorptionAllowed)
                    ud = ud.Skip(1).ToArray();

                //Полная экспонента ослабления
                double totalLooseExp = Math.Exp(-ud.Sum());

                //Расчет вклада поля рассеянного излучения
                double buildupFactor = input.BuildupProcessor != null && ud.Length > 0 ? input.BuildupProcessor.EvaluateComplexBuildup(ud, input.BuildupFactors) : 1.0;

                //Текущее значение интеграла
                return rho * totalLooseExp / cFull * buildupFactor;
            }, 
            0, R, form.NRadius, 
            0, Math.PI, form.NRadius, 
            0, h, form.NHeight, 
            input.CancellationToken) / sourceVolume;
        }
    }
}
