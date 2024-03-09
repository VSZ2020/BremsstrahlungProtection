using BSP.BL.Geometries;

namespace BSP.BL.Calculation
{
    public class Calculation
    {
        private const double CONVERSION_CONST = 1.6E-10 * 3600; //(Дж/кг)/(МэВ/г) * (сек/ч)

        public static async Task<OutputValue> StartAsync(InputData input, BaseGeometry form)
        {
            //Создаем выходной массив
            int energiesCount = input.BremsstrahlungEnergyFluxes.Length;
            OutputValue output = new OutputValue();
            output.PartialFluxDensity = new double[energiesCount];
            output.PartialEnergyFluxDensity = new double[energiesCount];
            output.PartialAirKerma = new double[energiesCount];
            
            var calcTask = Task.Run(() =>
            {
                int calculatedEnergiesCount = 0;

                Parallel.For(0, energiesCount, energyIndex =>
                {
                    //Вычисляем интеграл
                    double fluence = form.GetFluence(input.BuildSingleEnergyInputData(energyIndex));
                    
                    //Вычисляем парциальную плотность потока квантов тормозного излучения [1/см2/с]
                    output.PartialFluxDensity[energyIndex] = fluence * input.SourceActivity;
                    //Вычисляем парциальную плотность потока энергии [МэВ/см2/с]
                    output.PartialEnergyFluxDensity[energyIndex] = input.BremsstrahlungEnergyFluxes[energyIndex] * fluence;
                    //Вычисляем парциальную мощность воздушной кермы [Гр/ч]
                    output.PartialAirKerma[energyIndex] =
                        CONVERSION_CONST * input.massEnvironmentAbsorptionFactors[energyIndex] * output.PartialEnergyFluxDensity[energyIndex];
                    
                    //Если значение NaN, то ...
                    if (double.IsNaN(output.PartialAirKerma[energyIndex]))
                    {
                        output.PartialAirKerma[energyIndex] = 0.0;
                        //Записываем в лог
                    }
                    //Если значение Inf, то ...
                    if (double.IsInfinity(output.PartialAirKerma[energyIndex]))
                    {
                        output.PartialAirKerma[energyIndex] = 0.0;
                        //Записываем в лог
                    }

                    Interlocked.Increment(ref calculatedEnergiesCount);
                    //calculatedEnergiesCount++;
                    input.Progress?.Report(calculatedEnergiesCount * 100 / energiesCount);
                });
            });

            await calcTask;
            return output;
        }
    }
}
