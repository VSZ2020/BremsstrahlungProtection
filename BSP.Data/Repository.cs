using BSP.Data.Entities;

namespace BSP.Data
{
    public class Repository
    {
        public DataContext context;

        public virtual (float[] tableEnergies, float[] tableValues) GetAttenuationFactors(int[] materialsIds)
        {
            throw new NotImplementedException();
        }


        public virtual (float[] tableEnergies, float[] tableValues) GetAbsorptionFactors(int materialId)
        {
            throw new NotImplementedException();
        }


        public virtual (float[] tableEnergies, float[][] tableValues) GetTaylorBuildupFactors(int[] materialsIds)
        {
            throw new NotImplementedException();
        }


        public virtual (float[] tableEnergies, float[][] tableValues) GetGpBuildupFactors(int[] materialsIds)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<MaterialEntity> GetMaterials()
        {
            throw new NotImplementedException();
        }


        public virtual IEnumerable<OrganTissueEntity> GetOrgansAndTissues()
        {
            throw new NotImplementedException();
        }


        public virtual IEnumerable<ExposureGeometryEntity> GetExposureGeometries()
        {
            throw new NotImplementedException();
        }


        public virtual (float[] tableEnergies, float[] tableValues) GeEffectiveDoseConversionFactors(int geometryId)
        {
            throw new NotImplementedException();
        }

        public virtual (float[] tableEnergies, float[] tableValues) GeEquivalentDoseConversionFactors(int geometryId, int organId)
        {
            throw new NotImplementedException();
        }

        public virtual (float[] tableEnergies, float[] tableValues) GeAmbientDoseEquivalentConversionFactors()
        {
            throw new NotImplementedException();
        }

        public virtual (float[] tableEnergies, float[] tableValues) GeExposureDoseConversionFactors()
        {
            throw new NotImplementedException();
        }

        public virtual (float[] tableEnergies, float[] tableValues) GeHp10DoseConversionFactors()
        {
            throw new NotImplementedException();
        }

        public virtual (float[] tableEnergies, float[] tableValues) GeHp007DoseConversionFactors()
        {
            throw new NotImplementedException();
        }
    }
}
