using BSP.BL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BSP.ViewModels
{
    public class DoseFactorVM
    {
        public Type DoseFactorType { get; set; }

        public string Name { get; set; }

        public static List<DoseFactorVM> Load()
        {
            return DoseFactorsService.DoseFactors
                .Select(d => new DoseFactorVM()
                {
                    Name = DoseFactorsService.GetTranslationKey(d.Key) != null ? (string)Application.Current.Resources[DoseFactorsService.GetTranslationKey(d.Key)] : d.Value,
                    DoseFactorType = d.Key
                })
                .ToList();
        }
    }
}
