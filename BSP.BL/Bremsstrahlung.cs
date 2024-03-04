/*
 * Создано в SharpDevelop.
 * Пользователь: Slava Izgagin
 * Дата: 07-Mar-20
 * Время: 14:00
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using BSP.BL.Nuclides;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSP.BL
{
    /// <summary>
    /// Класс, описывающий характеристики тормозного излучения
    /// </summary>
    public static class Bremsstrahlung
    {
        /// <summary>
        /// Количество энергетических групп спектра тормозного излучения
        /// </summary>
        public static int Length => 10;


        public static double[] GetBremsstrahlungFractions(bool isMonoenergeticElectron = false)
        {
            return isMonoenergeticElectron ?
                new double[] { 0.269, 0.205, 0.158, 0.121, 0.09, 0.065, 0.045, 0.028, 0.015, 0.004 } :
                new double[] { 0.435, 0.258, 0.152, 0.083, 0.043, 0.02, 7.0E-3, 2.0E-3, 3.0E-4, 0 };
        }


        /// <summary>
        /// Рассчитывается выход тормозного излучения в заданном материале для набора энергий бета-радионуклида и выходов бета-излучения для каждой энергии
        /// </summary>
        /// <param name="Zeff">Эффективный атомный номер материала вещества</param>
        /// <param name="energies">Массив средний энергий бета-излучения</param>
        /// <param name="yields">Массив выходов бета-частиц заданных энергий</param>
        /// <returns>Выход тормозного излучения для бета-радионуклида</returns>
        private static double WyardBremsstrahlungYield(float Zeff, float[] energies, float[] yields)
        {
            var sumEI = Enumerable.Range(0, energies.Length).Select(i => energies[i] * energies[i] * yields[i]).Sum();
            return 8.5E-4 * (Zeff + 3) * sumEI;
        }


        /// <summary>
        /// Правые границы энергетических групп фотонов спектра тормозного излучения
        /// </summary>
        /// <param name="maxEnergy"></param>
        /// <returns></returns>
        public static float[] GetEnergyBinsRightEdges(float maxEnergy) => Enumerable.Range(1, 10).Select(i => 0.1f * i * maxEnergy).ToArray();


        /// <summary>
        /// Средние энергии энергетических групп фотонов спектра тормозного излучения
        /// </summary>
        /// <param name="binEdges">Массив правых границ энергетических групп</param>
        /// <returns></returns>
        public static float[] GetEnergyBinsAverageEnergies(float[] binEdges)
        {
            var meanEnergies = new float[binEdges.Length];
            meanEnergies[0] = binEdges[0] / 2;
            for (int i = 1; i < binEdges.Length; i++)
            {
                meanEnergies[i] = (binEdges[i - 1] + binEdges[i]) / 2;
            }
            return meanEnergies;
        }


        /// <summary>
        /// Средние энергии энергетических групп фотонов спектра тормозного излучения
        /// </summary>
        /// <param name="maxEnergy">Максимальная энергия бета-спектра</param>
        /// <returns></returns>
        public static float[] GetEnergyBinsAverageEnergies(float maxEnergy) => GetEnergyBinsAverageEnergies(GetEnergyBinsRightEdges(maxEnergy));


        /// <summary>
        /// Возвращает средние энергии для энергетических групп фотонов спектра тормозного излучения
        /// </summary>
        /// <param name="nuclides">Список выбранных пользователем радионуклидов</param>
        /// <returns></returns>
        [Obsolete]
        public static float[] GetMeanEnergyBins(List<Radionuclide> nuclides)
        {
            var primaryNuclide = SearchPrimaryNuclide(nuclides);
            return GetEnergyBinsAverageEnergies(primaryNuclide.GetEnergyWithMaxIntensity());
        }


        public static (float[] energies, double[] yields) GetEnergyYieldData(float[] nuclideEnergies, float[] nuclideYields, float Zeff, bool isMonoenergeticElectrons = false, float cutoffEnergy = 0.015f)
        {
            var energies = GetEnergyBinsAverageEnergies(nuclideEnergies.Max());
            var yields = GetBremsstrahlungEnergyYields(nuclideEnergies, nuclideYields, Zeff, isMonoenergeticElectrons);
            return (energies, yields);
        }


        [Obsolete]
        public static (float[] energies, double[] yields) GetEnergyYieldData(List<Radionuclide> selectedNuclides, float Zeff, bool isMonoenergeticElectrons = false, float cutoffEnergy = 0.015f)
        {
            var primaryNuclide = SearchPrimaryNuclide(selectedNuclides);
            var energies = GetEnergyBinsAverageEnergies(primaryNuclide.GetEnergyWithMaxIntensity());
            var yields = GetBremsstrahlungEnergyYields(selectedNuclides, Zeff, isMonoenergeticElectrons);
            return (energies, yields);
        }


        /// <summary>
        /// Массив выходов энергий тормозного излучения на распад [МэВ/распад]
        /// </summary>
        /// <param name="nuclideEnergies">Массив энергий бета-излучения [МэВ]</param>
        /// <param name="nuclideYields">Массив выходов бета-излучения</param>
        /// <param name="Zeff">Эффективный атомный номер материала источника</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static double[] GetBremsstrahlungEnergyYields(float[] nuclideEnergies, float[] nuclideYields, float Zeff, bool isMonoenergeticElectrons = false)
        {
            //Вычисление максимального выхода тормозного излучения для доминирующего нуклида
            var bremsstrahlungYield = WyardBremsstrahlungYield(Zeff, nuclideEnergies, nuclideYields);

            //Вычисление энергетических выходов тормозного излучения для базового нуклида [МэВ/распад]
            return GetBremsstrahlungFractions(isMonoenergeticElectrons).Select(f => f * bremsstrahlungYield).ToArray();
        }


        /// <summary>
        /// Рассчитывает поток энергии тормозного излучения для указанных энергетических групп [МэВ/с]
        /// </summary>
        /// <param name="bremsstrahlungYields">Выходы тормозного излучения на распад</param>
        /// <param name="Activity">Активность источника в Беккерелях</param>
        /// <returns></returns>
        public static double[] GetBremsstrahlungFluxOfEnergy(double[] bremsstrahlungYields, double Activity)
        {
            return Enumerable.Range(0, bremsstrahlungYields.Length).Select(i => bremsstrahlungYields[i] * Activity).ToArray();
        }

        /// <summary>
        /// Рассчитывает потоки тормозного излучения для указанных энергетических групп [фотон/с]
        /// </summary>
        /// <param name="averageEnergies">Средние энергии диапазонов </param>
        /// <param name="bremsstrahlungYields">Выходы тормозного излучения на распад</param>
        /// <param name="Activity">Активность источника в Беккерелях</param>
        /// <returns></returns>
        public static double[] GetBremsstrahlungFluxes(float[] averageEnergies, float[] bremsstrahlungYields, double Activity)
        {
            return Enumerable.Range(0, averageEnergies.Length).Select(i => bremsstrahlungYields[i] / averageEnergies[i] * Activity).ToArray();
        }


        /// <summary>
        /// Массив выходов энергий тормозного излучения на распад [МэВ/распад]
        /// </summary>
        /// <param name="SelectedNuclides">Массив выбранных пользователем радионуклидов</param>
        /// <param name="Z_eff">Эффективный атомный номер материала источника</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        [Obsolete("Используйте перегрузку (float[] nuclideEnergies, float[] nuclideYields, float Zeff, bool isMonoenergeticElectrons = false)")]
        public static double[] GetBremsstrahlungEnergyYields(List<Radionuclide> SelectedNuclides, float Zeff, bool isMonoenergeticElectrons = false)
        {
            if (SelectedNuclides == null | SelectedNuclides.Count == 0)
                throw new NullReferenceException("Selected radionuclides array is empty");//(string)Application.Current.Resources["msgError_NullOrZeroNuclides"]);

            //Поиск нуклида с максимальным I*E
            var primeNuclide = SearchPrimaryNuclide(SelectedNuclides);

            //Вычисление энергетических выходов тормозного излучения для базового нуклида [МэВ/распад]
            return GetBremsstrahlungEnergyYields(primeNuclide.MeanEnergies, primeNuclide.EnergyYields, Zeff, isMonoenergeticElectrons);
        }


        /// <summary>
        /// Находит из перечня нуклидов изотоп с максимальным произведением I*E
        /// </summary>
        [Obsolete]
        private static Radionuclide SearchPrimaryNuclide(List<Radionuclide> Nuclides)
        {
            Radionuclide majorNuclide = Nuclides.First();
            double maxEI = majorNuclide.EnergyByIntensityArray.Max();

            foreach (var nuclide in Nuclides)
            {
                var maxNuclideEnergyIntensity = nuclide.EnergyByIntensityArray.Max();
                if (maxNuclideEnergyIntensity > maxEI)
                {
                    majorNuclide = nuclide;
                    maxEI = maxNuclideEnergyIntensity;
                }
            }
            return majorNuclide;
        }
    }
}
