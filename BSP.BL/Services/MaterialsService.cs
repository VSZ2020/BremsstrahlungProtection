using BSP.BL.DTO;
using BSP.BL.Interpolation;
using BSP.BL.Interpolation.Functions;
using BSP.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BSP.BL.Services
{
    /// <summary>
    /// Работает на уровне бизнес-логики. Обращается к фреймворку, работающему с БД. Запрашивает данные, проводит обработку перед передачей дальше по цепочке
    /// </summary>
    public class MaterialsService
    {
        public MaterialsService(DataContext context)
        {
            this.context = context;
        }

        private readonly DataContext context;


        public List<MaterialDto> GetAllMaterials()
        {
            var entities = context.Materials.AsNoTracking().ToList();

            return entities
                .Select(e => new MaterialDto() { Id = e.Id, Name = e.Name, Density = e.Density, Z = e.Z, Weight = e.Weight })
                .OrderBy(m => m.Name)
                .ToList();
        }

        public List<MaterialDto> GetMaterialsById(int[] ids)
        {
            var entities = context.Materials.AsNoTracking().Where(m => ids.Contains(m.Id)).ToList();
            return entities
                .Select(e => new MaterialDto() { Id = e.Id, Name = e.Name, Density = e.Density, Z = e.Z, Weight = e.Weight })
                .ToList();
        }

        public float[][] GetMassAttenuationFactors(int[] materialsIds, float[] energies, InterpolationType interpolationType = InterpolationType.Linear)
        {
            //Массив для выходных значений
            var interpolatedData = Enumerable.Range(0, energies.Length).Select(i => new float[materialsIds.Length]).ToArray();

            for (var i = 0; i < materialsIds.Length; i++)
            {
                var interpolated_values = new float[energies.Length];
                var entities = context.MassAttenuationFactors.AsNoTracking().Where(e => e.MaterialId == materialsIds[i]).ToList();
                if (entities.Count > 3)
                {
                    var table_energies = entities.Select(e => e.Energy).ToArray();
                    var table_values = entities.Select(e => e.Value).ToArray();
                    interpolated_values = Interpolator.Interpolate(table_energies, table_values, energies, interpolationType);
                }
                else
                {
                    //TODO: Log absent data
                }

                for (var j = 0; j < energies.Length; j++)
                    interpolatedData[j][i] = interpolated_values[j];
            }
            return interpolatedData;
        }

        public float[] GetAbsorptionFactors(int materialId, float[] energies, InterpolationType interpolationType = InterpolationType.Linear)
        {
            var entities = context.MassAbsorptionFactors.AsNoTracking().Where(e => e.MaterialId == materialId).ToList();
            if (entities.Count < 3)
            {
                //TODO: Log absent data
                return new float[energies.Length];
            }

            var table_energies = entities.Select(e => e.Energy).ToArray();
            var table_values = entities.Select(e => e.Value).ToArray();

            var interpolated_values = Interpolator.Interpolate(table_energies, table_values, energies, interpolationType);
            return interpolated_values;
        }
    }
}
