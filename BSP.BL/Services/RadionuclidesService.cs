using BSP.BL.DTO;
using BSP.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BSP.BL.Services
{
    public class RadionuclidesService
    {
        public RadionuclidesService(DataContext ctx)
        {
            this.context = ctx;
        }

        private readonly DataContext context;


        public List<RadionuclideDto> GetAllRadionuclides()
        {
            return context.Radionuclides
                .AsNoTracking()
                .Select(e => new RadionuclideDto()
                {
                    Id = e.Id,
                    Name = e.Name,
                    HalfLive = e.HalfLife,
                    HalfLiveUnits = e.HalfLiveUnits,
                    Z = e.Z,
                    Weight = e.Weight
                })
                .OrderBy(r => r.Name)
                .ToList();
        }


        public RadionuclideDto GetRadionuclideById(int id)
        {
            var entity = context.Radionuclides.AsNoTracking().SingleOrDefault(e => e.Id == id);
            if (entity == null)
                return new RadionuclideDto() { Id = id };

            return new RadionuclideDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                HalfLive = entity.HalfLife,
                HalfLiveUnits = entity.HalfLiveUnits,
                Z = entity.Z,
                Weight = entity.Weight,
            };
        }

        public List<RadionuclideEnergyIntensityDto> GetEnergyIntensityData(int radionuclideId)
        {
            return context.RadionuclideEnergyIntensityData
                .AsNoTracking()
                .Where(e => e.RadionuclideId == radionuclideId)
                .Select(e => new RadionuclideEnergyIntensityDto()
                {
                    Id = e.Id,
                    EndpointEnergy = e.EndpointEnergy,
                    AverageEnergy = e.AverageEnergy,
                    Yield = e.Yield,
                })
                .ToList();
        }

        public (float[] endpointEnergies, float[] averageEnergies, float[] yields) GetEnergyIntensityDataArrays(int radionuclideId)
        {
            var eiData = GetEnergyIntensityData(radionuclideId);
            return (eiData.Select(e => e.EndpointEnergy).ToArray(), eiData.Select(e => e.AverageEnergy).ToArray(), eiData.Select(e => e.Yield).ToArray());
        }


        public (float[] endpointEnergies, float[] averageEnergies, float[] yields) GetEnergyIntensityDataArrays(int[] selectedRadionuclidesIds)
        {
            return GetEnergyIntensityDataArrays(GetRadionuclideWithMaxEnergyIntensity(selectedRadionuclidesIds).Id);
        }


        /// <summary>
        /// Ищет в базе и возвращает радионуклид с максимальным значением произведения граничной энергии на выход излучения (E*I)
        /// </summary>
        /// <param name="selectedRadionuclidesIds">Список идентификаторов радионуклидов, по которым ведется поиск</param>
        /// <returns></returns>
        public RadionuclideDto GetRadionuclideWithMaxEnergyIntensity(int[] selectedRadionuclidesIds)
        {
            var energyIntensityEntities = context.RadionuclideEnergyIntensityData
                .AsNoTracking()
                .Where(e => selectedRadionuclidesIds.Contains(e.RadionuclideId))
                .ToList();
            var primaryEnergyIntensity = energyIntensityEntities.MaxBy(e => e.EndpointEnergy * e.Yield);
            return GetRadionuclideById(primaryEnergyIntensity.RadionuclideId);
        }


    }
}
