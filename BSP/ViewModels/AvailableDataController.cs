using BSP.BL.DTO;
using BSP.BL.Services;
using BSP.Common;

namespace BSP.ViewModels
{
    public class AvailableDataController : BaseViewModel
    {
        public AvailableDataController(RadionuclidesService radionuclidesService, MaterialsService materialsService)
        {
            this.radionuclidesService = radionuclidesService;
            this.materialsService = materialsService;

            AvailableNuclides = new(RadionuclideVM.Load(radionuclidesService.GetAllRadionuclides()));
            AvailableMaterials = new(materialsService.GetAllMaterials());
        }

        private readonly RadionuclidesService radionuclidesService;
        private readonly MaterialsService materialsService;

        /// <summary>
        /// Перечень доступных радионуклидов
        /// </summary>
        public static List<RadionuclideVM> AvailableNuclides { get; private set; }
        public static List<MaterialDto> AvailableMaterials { get; private set; }

        public void ReloadMaterialsList()
        {

        }

        public void ReloadRadionuclidesList()
        {

        }
    }
}
