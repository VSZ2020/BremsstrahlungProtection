using BSP.BL.DTO;
using BSP.BL.Interpolation;
using BSP.BL.Interpolation.Functions;
using BSP.Data;
using Microsoft.EntityFrameworkCore;

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
            return ids
                .Select(id => GetMaterialById(id))
                .ToList();
        }

        /// <summary>
        /// Возвращает объект материала по его идентификатору
        /// </summary>
        /// <param name="id">Уникальный идентификатор материала</param>
        /// <returns></returns>
        public MaterialDto GetMaterialById(int id)
        {
            var e = context.Materials.AsNoTracking().Single(m => m.Id == id);
            return new MaterialDto() { Id = e.Id, Name = e.Name, Density = e.Density, Z = e.Z, Weight = e.Weight };
        }

        /// <summary>
        /// Табличные значения энергий и массовых коэффициентов ослабления для указанного материала
        /// </summary>
        /// <param name="materialsId">Уникальный идентификатор материала</param>
        /// <returns></returns>
        public (double[] energies, double[] values) GetTableMassAttenuationFactors(int materialsId)
        {
            var tableEntities = context.MassAttenuationFactors.AsNoTracking().Where(e => e.MaterialId == materialsId).ToList();
            return (tableEntities.Select(e => (double)e.Energy).ToArray(), tableEntities.Select(e => (double)e.Values[0]).ToArray());
        }

        /// <summary>
        /// Табличные значения энергий и массовых коэффициентов поглощения для указанного материала
        /// </summary>
        /// <param name="materialsId">Уникальный идентификатор материала</param>
        /// <returns></returns>
        public (double[] energies, double[] values) GetTableMassAbsoprtionFactors(int materialsId)
        {
            var tableEntities = context.MassAbsorptionFactors.AsNoTracking().Where(e => e.MaterialId == materialsId).ToList();
            return (tableEntities.Select(e => (double)e.Energy).ToArray(), tableEntities.Select(e => (double)e.Values[0]).ToArray());
        }



        /// <summary>
        /// Интерполированные для указанных энергий значения массовых коэффициентов ослабления
        /// </summary>
        /// <param name="materialsId">Уникальный идентификатор материала</param>
        /// <param name="energies">Массив энергий, для которых выполняется интерполяция</param>
        /// <param name="interpolationType"></param>
        /// <returns></returns>
        public double[] GetInterpolatedMassAttenuationFactors(int materialsId, double[] energies, InterpolationType interpolationType = InterpolationType.Linear)
        {
            (var table_energies, var table_values) = GetTableMassAttenuationFactors(materialsId);

            return Interpolator.Interpolate(table_energies, table_values, energies, interpolationType);
        }

        /// <summary>
        /// Интерполированные для указанных энергий значения массовых коэффициентов поглощения
        /// </summary>
        /// <param name="materialsId"></param>
        /// <param name="energies"></param>
        /// <param name="interpolationType"></param>
        /// <returns></returns>
        public double[] GetInterpolatedMassAbsorptionFactors(int materialsId, double[] energies, InterpolationType interpolationType = InterpolationType.Linear)
        {
            (var table_energies, var table_values) = GetTableMassAbsoprtionFactors(materialsId);

            return Interpolator.Interpolate(table_energies, table_values, energies, interpolationType);
        }



        /// <summary>
        /// Специально сформированный массив интерполированных массовых коэффициентов ослабления с учетом заданного порядка слоев материалов. Первый индекс массива - энергия, второй - материал
        /// </summary>
        /// <param name="materialsIds"></param>
        /// <param name="energies"></param>
        /// <param name="interpolationType"></param>
        /// <returns></returns>
        public double[][] GetMassAttenuationFactors(int[] materialsIds, double[] energies, InterpolationType interpolationType = InterpolationType.Linear)
        {
            //Массив для выходных значений
            var interpolatedData = Enumerable.Range(0, energies.Length).Select(i => new double[materialsIds.Length]).ToArray();

            for (var i = 0; i < materialsIds.Length; i++)
            {
                var interpolated_values = new double[energies.Length];
                var entities = context.MassAttenuationFactors.AsNoTracking().Where(e => e.MaterialId == materialsIds[i]).ToList();
                if (entities.Count > 3)
                {
                    interpolated_values = GetInterpolatedMassAttenuationFactors(materialsIds[i], energies, interpolationType);
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


        /// <summary>
        /// Массив интерполированных массовых коэффициентов поглощения
        /// </summary>
        /// <param name="materialsIds"></param>
        /// <param name="energies"></param>
        /// <param name="interpolationType"></param>
        /// <returns></returns>
        public double[] GetMassAbsorptionFactors(int materialId, double[] energies, InterpolationType interpolationType = InterpolationType.Linear)
        {
            return GetInterpolatedMassAbsorptionFactors(materialId, energies, interpolationType);
        }
    }
}
