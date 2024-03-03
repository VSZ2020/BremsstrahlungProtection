using BSP.BL.DTO;
using BSP.BL.Services;
using BSP.Common;
using System.Collections.Generic;

namespace BSP.ViewModels
{
    public class AvailableDataController : BaseViewModel
    {
        public AvailableDataController(RadionuclidesService radionuclidesService, MaterialsService materialsService)
        {
            AvailableNuclides = new(RadionuclideVM.Load(radionuclidesService.GetAllRadionuclides()));
            AvailableMaterials = new(materialsService.GetAllMaterials());
        }

        /// <summary>
        /// Перечень доступных радионуклидов
        /// </summary>
        public static List<RadionuclideVM> AvailableNuclides { get; private set; }
        public static List<MaterialDto> AvailableMaterials { get; private set; }
    }
}
