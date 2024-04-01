using BSP.BL.Services;
using BSP.Common;
using System.Windows;

namespace BSP.ViewModels
{
    public class OrganTissueVM : BaseViewModel
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public static List<OrganTissueVM> Load(DoseFactorsService dcfService)
        {
            return dcfService.GetAllOrgansAndTissues()
                .Select(e => new OrganTissueVM()
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
                case "urinary bladder":
                    key = "organTissue_Name_UB";
                    break;
                case "red marrow":
                    key = "organTissue_Name_RM";
                    break;
                case "bone surface":
                    key = "organTissue_Name_BS";
                    break;
                case "breast":
                    key = "organTissue_Name_Breast";
                    break;
                case "colon":
                    key = "organTissue_Name_Colon";
                    break;
                case "gonads":
                    key = "organTissue_Name_Gonads";
                    break;
                case "liver":
                    key = "organTissue_Name_Liver";
                    break;
                case "lung":
                    key = "organTissue_Name_Lung";
                    break;
                case "esophagus":
                    key = "organTissue_Name_Esophagus";
                    break;
                case "skin":
                    key = "organTissue_Name_Skin";
                    break;
                case "stomach":
                    key = "organTissue_Name_Stomach";
                    break;
                case "thyroid":
                    key = "organTissue_Name_Thyroid";
                    break;
                case "lens of the eye":
                    key = "organTissue_Name_Lens";
                    break;
                case "thymus":
                    key = "organTissue_Name_Thymus";
                    break;
                case "uterus":
                    key = "organTissue_Name_Uterus";
                    break;
                case "remainder":
                    key = "organTissue_Name_Remainder";
                    break;
            }
            var translation = Application.Current.TryFindResource((string)key);
            return translation != null ? (string)translation : name;
        }
    }
}
