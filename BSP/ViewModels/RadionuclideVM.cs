﻿using BSP.BL.DTO;
using BSP.Common;
using BSP.ViewModels.Tabs;
using System.ComponentModel;
using System.Windows;

namespace BSP.ViewModels
{
    public class RadionuclideVM : BaseViewModel, IDataErrorInfo
    {
        public RadionuclideVM(SourceTabVM? vm = null)
        {
            this.vm = vm;
        }

        private readonly SourceTabVM? vm;
        private int id;
        private double activity;

        public int Id { get => id; set { id = value; vm?.OnRadionuclidesListUpdated(); } }
        public string Name { get; set; }
        public double Activity { get => activity; set { activity = value; OnChanged(); vm?.OnRadionuclidesListUpdated(); } }

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
