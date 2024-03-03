using BSP.BL.DTO;
using BSP.BL.Interpolation;
using BSP.BL.Interpolation.Functions;
using BSP.Data;
using BSP.Data.Entities;
using BSP.Data.Entities.DoseConversionFactors;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSP.BL.Services
{
    public class DoseFactorsService
    {
        public DoseFactorsService(DataContext context) => this.context = context;

        private readonly DataContext context;

        private static Dictionary<Type, string> _availableDoseFactors = new Dictionary<Type, string>()
        {
            {typeof(EffectiveDoseEntity), "Effective dose" },
            {typeof(AirKermaEntity), "Air Kerma" },
            {typeof(EquivalentDoseEntity), "Equivalent dose" },
            {typeof(AmbientDoseEquivalentEntity), "Ambient dose equivalent" },
            {typeof(ExposureDoseEntity), "Exposure dose" },
            {typeof(Hp10Entity), "Hp(10)" },
            {typeof(Hp007Entity), "Hp(0.07)" },
        };

        public static string GetUnits(Type doseFactorType)
        {
            switch (doseFactorType)
            {
                case Type e when e == typeof(EffectiveDoseEntity): return "Sv";
                case Type e when e == typeof(AirKermaEntity): return "Gy";
                case Type e when e == typeof(EquivalentDoseEntity): return "Sv";
                case Type e when e == typeof(AmbientDoseEquivalentEntity): return "Sv";
                case Type e when e == typeof(ExposureDoseEntity): return "R";
                case Type e when e == typeof(Hp10Entity): return "Sv";
                case Type e when e == typeof(Hp007Entity): return "Sv";
            };
            return "";
        }

        public static Dictionary<Type, string> DoseFactors => _availableDoseFactors;

        /// <summary>
        /// Возвращает ключ словаря, в котором содержится название для текущей локализации приложения
        /// </summary>
        /// <param name="heteroBuildupType">Тип фактора накопления для гетерогенной среды</param>
        /// <returns></returns>
        public static string GetTranslationKey(Type doseFactorType)
        {
            return doseFactorType switch
            {
                Type e when e == typeof(AirKermaEntity) => "DoseNameAirKerma",
                Type e when e == typeof(AmbientDoseEquivalentEntity) => "DoseNameAmbientEquivalent",
                Type e when e == typeof(ExposureDoseEntity) => "DoseNameExposure",
                Type e when e == typeof(EffectiveDoseEntity) => "DoseNameEffective",
                Type e when e == typeof(EquivalentDoseEntity) => "DoseNameEquivalent",
                Type e when e == typeof(Hp10Entity) => "DoseNameHp10",
                Type e when e == typeof(Hp007Entity) => "DoseNameHp007",
                _ => null
            };
        }

        public float[] GetDoseConversionFactors(Type doseConversionFactorType, float[] energies, int exposureGeometryId, int organTissueId, InterpolationType interpolatorType = InterpolationType.Cubic)
        {
            List<BaseDoseFactorEntity> table_entities = new List<BaseDoseFactorEntity>();

            if (doseConversionFactorType == typeof(AirKermaEntity))
                //Возвращаем единицы, т.к. в функции расчета уже вычисляется коэффициент перехода к воздушной керме как (Energy * um)
                return Enumerable.Range(0, energies.Length).Select(i => 1.0F).ToArray();
            //table_entities.AddRange(context.AirKermaDoseFactors.AsNoTracking().Cast<BaseDoseFactorEntity>().ToList());

            if (doseConversionFactorType == typeof(ExposureDoseEntity))
                table_entities.AddRange(context.ExposureDoseFactors.AsNoTracking().Cast<BaseDoseFactorEntity>().ToList());

            if (doseConversionFactorType == typeof(Hp10Entity))
                table_entities.AddRange(context.Hp10Factors.AsNoTracking().Cast<BaseDoseFactorEntity>().ToList());

            if (doseConversionFactorType == typeof(Hp007Entity))
                table_entities.AddRange(context.Hp007Factors.AsNoTracking().Cast<BaseDoseFactorEntity>().ToList());

            if (doseConversionFactorType == typeof(AmbientDoseEquivalentEntity))
                table_entities.AddRange(context.AmbientDoseEquivalentFactors.AsNoTracking().Cast<BaseDoseFactorEntity>().ToList());

            if (doseConversionFactorType == typeof(EffectiveDoseEntity))
                table_entities.AddRange(context.EffectiveDoseFactors.AsNoTracking().Where(e => e.ExposureGeometryId == exposureGeometryId).Cast<BaseDoseFactorEntity>().ToList());

            if (doseConversionFactorType == typeof(EquivalentDoseEntity))
                table_entities.AddRange(context.EquivalentDoseFactors.AsNoTracking().Where(e => e.ExposureGeometryId == exposureGeometryId && e.OrganTissueId == organTissueId).Cast<BaseDoseFactorEntity>().ToList());

            //Интерполируем табличные данные в промежуточных значениях энергий
            var doseFactors = Interpolator.Interpolate(table_entities.Select(e => e.Energy).ToArray(), table_entities.Select(e => e.Value).ToArray(), energies, interpolatorType);

            return doseFactors;
        }

        public List<OrganTissueDto> GetAllOrgansAndTissues()
        {
            var entities = context.OrgansAndTissues.AsNoTracking().ToList();
            return entities.Select(e => new OrganTissueDto() { Id = e.Id, Name = e.Name }).ToList();
        }


        public List<ExposureGeometryDto> GetAllExposureGeometries()
        {
            var entities = context.ExposureGeometries.AsNoTracking().ToList();
            return entities.Select(e => new ExposureGeometryDto() { Id = e.Id, Name = e.Name }).ToList();
        }

        public static (bool exposureGeometryState, bool organTissueState) GetOptionalBoxesState(Type doseFactorType)
        {
            if (doseFactorType == typeof(EquivalentDoseEntity))
                return (true, true);

            if (doseFactorType == typeof(EffectiveDoseEntity))
                return (true, false);

            return (false, false);
        }
    }
}
