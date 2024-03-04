using BSP.BL.Geometries;
using System.Threading.Tasks;

namespace BSP.BL.Calculation
{
    public class Calculation
    {
        private const double CONVERSION_CONST = 1.6E-10 * 3600; //(Дж/кг)/(МэВ/г) * (ч/сек)

        public static async Task<OutputValue> StartAsync(InputData input, BaseGeometry form)
        {
            //Создаем выходной массив
            int EnergiesCount = input.BremsstrahlungFlux.Length;
            OutputValue doseRate = new OutputValue();
            doseRate.PartialDoseRates = new double[EnergiesCount];

            var calcTask = Task.Run(() =>
            {
                int calculatedEnergiesCount = 0;

                Parallel.For(0, EnergiesCount, EnergyIndex =>
                {
                    //Вычисляем интеграл
                    double Fluence = form.GetFluence(input.BuildSingleEnergyInputData(EnergyIndex));

                    //Вычисляем парциальную мощность воздушной кермы: K = [МэВ/(с * г) → Гр/ч] / (4 * pi) * k * Am * Ib * um(air) * Intergral
                    doseRate.PartialDoseRates[EnergyIndex] =
                            CONVERSION_CONST * input.massEnvironmentAbsorptionFactors[EnergyIndex] * input.BremsstrahlungFlux[EnergyIndex] * Fluence;

                    //Если значение NaN, то ...
                    if (double.IsNaN(doseRate.PartialDoseRates[EnergyIndex]))
                    {
                        doseRate.PartialDoseRates[EnergyIndex] = 0.0;
                        //Записываем в лог
                    }
                    //Если значение Inf, то ...
                    if (double.IsInfinity(doseRate.PartialDoseRates[EnergyIndex]))
                    {
                        doseRate.PartialDoseRates[EnergyIndex] = 0.0;
                        //Записываем в лог
                    }

                    calculatedEnergiesCount++;
                    input.Progress?.Report(calculatedEnergiesCount * 100 / EnergiesCount);
                });
            });

            await calcTask;
            return doseRate;
        }
    }
}
