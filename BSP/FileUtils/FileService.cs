using BSP.ViewModels;
using BSP.ViewModels.RadionuclidesViewer;
using BSP.ViewModels.Tabs;
using System.IO;
using System.Windows;

namespace BSP.FileUtils
{
    public class FileService
    {
        private const char CSV_DELIMETER = ',';

        #region ExportEnergyIntensityToTextFile
        public static void ExportEnergyIntensityToTextFile(string filename, List<BremsstrahlungEnergyYieldVM> EnergyYieldList, double activity)
        {
            char delimeter = '\t';
            if (filename.EndsWith(".csv"))
                delimeter = CSV_DELIMETER;

            using (StreamWriter wr = new StreamWriter(filename))
            {
                //Заголовок таблицы
                wr.WriteLine(string.Join(delimeter, "#Energy (MeV)", "Energy Yield (MeV/decay)", "Energy Flux (MeV/s)", "Photons Flux (photon/s)", "Activity (Bq)"));
                for (var i = 0; i < EnergyYieldList.Count; i++)
                {
                    wr.WriteLine(string.Join(delimeter, EnergyYieldList[i].Energy, EnergyYieldList[i].EnergyYield, EnergyYieldList[i].EnergyFlux, EnergyYieldList[i].PhotonsFlux, activity));
                }
            }
            MessageBox.Show("File was successfully saved at " + filename, "Export", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region ReadEnergyIntensityFile
        public static (double totalActivity, IEnumerable<BremsstrahlungEnergyYieldVM> energyIntensity) ReadEnergyIntensityFile(string filename, SourceTabVM vm, char delimeter = '\t')
        {
            double totalActivity = 0;
            if (filename.EndsWith(".csv"))
                delimeter = CSV_DELIMETER;

            try
            {
                var items = new List<BremsstrahlungEnergyYieldVM>();
                using (StreamReader rd = new StreamReader(filename))
                {
                    //Пропускаем строку с заголовками
                    string line = rd.ReadLine();
                    while ((line = rd.ReadLine()) != null)
                    {
                        float energy = 0;
                        double energyYield = 0;
                        double energyFlux = 0;
                        double photonsFlux = 0;
                        double activity = 0;

                        var strValues = line.Split(delimeter);
                        if (strValues.Length < 5)
                            continue;

                        float.TryParse(strValues[0], out energy);
                        double.TryParse(strValues[1], out energyYield);
                        double.TryParse(strValues[2], out energyFlux);
                        double.TryParse(strValues[3], out photonsFlux);
                        double.TryParse(strValues[4], out activity);

                        if (totalActivity != activity)
                            totalActivity = activity;

                        var bey = new BremsstrahlungEnergyYieldVM(vm) { Energy = energy, EnergyYield = energyYield, EnergyFlux = energyFlux, PhotonsFlux = photonsFlux };
                        items.Add(bey);
                    }
                }
                return (totalActivity, items);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading of file {filename}. Incorrect file content", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return (0, Enumerable.Empty<BremsstrahlungEnergyYieldVM>());
        }
        #endregion

        #region ExportRadionuclideEnergyIntensity
        public static bool ExportRadionuclideEnergyIntensity(string filename, List<RadionuclideEnergyIntensityVM> data)
        {
            char delimeter = filename.EndsWith(".csv") ? ',' : '\t';
            using (StreamWriter wr = new StreamWriter(filename))
            {
                wr.WriteLine(string.Join(delimeter, "#Endpoint energy (MeV)", "Average energy (MeV)", "Yield"));
                foreach (var item in data)
                {
                    wr.WriteLine(string.Join(delimeter, item.EndpointEnergy, item.AverageEnergy, item.Intensity));
                }
            }
            return true;
        }
        #endregion
    }



}
