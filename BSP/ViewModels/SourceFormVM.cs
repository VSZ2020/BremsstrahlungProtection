using BSP.BL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BSP.ViewModels
{
    public class SourceFormVM
    {
        public Type FormType { get; set; }

        public string Name { get; set; }

        public static List<SourceFormVM> Load()
        {
            return GeometryService.Geometries
                .Select(g => new SourceFormVM()
                {
                    FormType = g.Key,
                    Name = (Application.Current.TryFindResource(GeometryService.GetTranslationKey(g.Key)) as string) ?? g.Value
                })
                .ToList();
        }
    }
}
