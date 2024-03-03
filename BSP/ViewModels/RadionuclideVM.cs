using BSP.BL.DTO;
using BSP.Common;
using System.Collections.Generic;
using System.Linq;

namespace BSP.ViewModels
{
    public class RadionuclideVM : BaseViewModel
    {
        private double activity;

        public int Id { get; set; }
        public string Name { get; set; }
        public double Activity { get => activity; set { activity = value; OnChanged(); } }

        public static List<RadionuclideVM> Load(List<RadionuclideDto> dto)
        {
            return dto.Select(d => new RadionuclideVM() { Id = d.Id, Name = d.Name, Activity = 0 }).ToList();
        }
    }
}
