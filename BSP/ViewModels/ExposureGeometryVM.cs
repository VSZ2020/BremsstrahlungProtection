using BSP.BL.Services;
using BSP.Common;
using System.Windows;

namespace BSP.ViewModels
{
    public class ExposureGeometryVM : BaseViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public static List<ExposureGeometryVM> Load(DoseFactorsService dcfService)
        {
            return dcfService.GetAllExposureGeometries()
                .Select(e => new ExposureGeometryVM()
                {
                    Id = e.Id,
                    Name = TryTranslate(e.Name)
                })
                .ToList();
        }

        private static string TryTranslate(string name)
        {
            string key = name;
            switch (name.ToLower())
            {
                case "antero-posterier":
                    key = "ExposureGeometry_Name_AP";
                    break;
                case "postero-anterier":
                    key = "ExposureGeometry_Name_PA";
                    break;
                case "left lateral":
                    key = "ExposureGeometry_Name_LLAT";
                    break;
                case "right lateral":
                    key = "ExposureGeometry_Name_RLAT";
                    break;
                case "rotational":
                    key = "ExposureGeometry_Name_ROT";
                    break;
                case "isotropic":
                    key = "ExposureGeometry_Name_ISO";
                    break;
            }
            var translation = Application.Current.TryFindResource((string)key);
            return translation != null ? (string)translation : name;
        }
    }
}
