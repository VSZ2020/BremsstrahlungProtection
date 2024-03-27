using BSP.BL.Geometries;
using BSP.Geometries.SDK;

namespace BSP.BL.Calculation
{
    public class Calculation
    {
        private const double CONVERSION_CONST = 1.6E-10 * 3600; //(Дж/кг)/(МэВ/г) * (сек/ч)

        public static async Task<OutputValue> StartAsync(InputData input, IGeometry form)
        {
            //Создаем выходной массив
            int energiesCount = input.PhotonsFluxes.Length;
            OutputValue output = new OutputValue(energiesCount);
            output.DosePoint = input.CalculationPoint;
            
            var calcTask = Task.Run(() =>
            {
                int calculatedEnergiesCount = 0;

                Parallel.For(0, energiesCount, energyIndex =>
                {
                    //Вычисляем интеграл
                    double fluence = form.GetFluence(input.BuildSingleEnergyInputData(energyIndex));

                    //Если значение NaN, то ...
                    if (double.IsNaN(fluence))
                    {
                        fluence = 0.0;
                        //Записываем в лог
                    }
                    //Если значение Inf, то ...
                    if (double.IsInfinity(fluence))
                    {
                        fluence = 0.0;
                        //Записываем в лог
                    }

                    output.Energies[energyIndex] = input.Energies[energyIndex];

                    output.PartialPhotonsFlux[energyIndex] = input.PhotonsFluxes[energyIndex];

                    //Вычисляем парциальную плотность потока квантов тормозного излучения [1/см2/с]
                    output.PartialFluxDensity[energyIndex] = input.PhotonsFluxes[energyIndex] * fluence;

                    //Вычисляем парциальную мощность воздушной кермы [Гр/ч]
                    output.PartialAirKerma[energyIndex] =
                        CONVERSION_CONST * input.massEnvironmentAbsorptionFactors[energyIndex] * output.PartialFluxDensity[energyIndex] * input.Energies[energyIndex];
                    

                    Interlocked.Increment(ref calculatedEnergiesCount);
                    input.Progress?.Report(calculatedEnergiesCount / energiesCount);
                });
            });

            await calcTask;
            return output;
        }
    }
}
