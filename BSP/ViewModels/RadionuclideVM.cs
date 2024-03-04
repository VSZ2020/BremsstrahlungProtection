using BSP.BL.DTO;
using BSP.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace BSP.ViewModels
{
    public class RadionuclideVM : BaseViewModel, IDataErrorInfo
    {
        private double activity;

        public int Id { get; set; }
        public string Name { get; set; }
        public double Activity { get => activity; set { activity = value; OnChanged(); } }

        public static List<RadionuclideVM> Load(List<RadionuclideDto> dto)
        {
            return dto.Select(d => new RadionuclideVM() { Id = d.Id, Name = d.Name, Activity = 0 }).ToList();
        }

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(Activity):
                        if (Activity < 0)
                            error = string.Format((Application.Current.TryFindResource("msg_ValidationActivity") as string) ?? "Incorrect value", Name);
                        break;
                }

                return error;
            }
        }
        public string Error => throw new NotImplementedException();
    }
}
