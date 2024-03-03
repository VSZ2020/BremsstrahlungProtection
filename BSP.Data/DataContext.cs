using BSP.Data.Entities;
using BSP.Data.Entities.DoseConversionFactors;
using BSP.Data.Entities.MaterialFactors;
using BSP.Data.Entities.Radionuclides;
using Microsoft.EntityFrameworkCore;

namespace BSP.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<MaterialEntity> Materials { get; set; } = null!;
        public DbSet<AttenuationFactorEntity> MassAttenuationFactors { get; set; } = null!;
        public DbSet<AbsorptionFactorEntity> MassAbsorptionFactors { get; set; } = null!;
        public DbSet<Taylor2ExpEntity> Taylor2ExpFactors { get; set; } = null!;
        public DbSet<GeometricProgressionEntity> GeometricProgressionFactors { get; set; } = null!;

        public DbSet<ExposureGeometryEntity> ExposureGeometries { get; set; } = null!;
        public DbSet<OrganTissueEntity> OrgansAndTissues { get; set; } = null!;
        public DbSet<AirKermaEntity> AirKermaDoseFactors { get; set; } = null!;
        public DbSet<EffectiveDoseEntity> EffectiveDoseFactors { get; set; } = null!;
        public DbSet<EquivalentDoseEntity> EquivalentDoseFactors { get; set; } = null!;
        public DbSet<AmbientDoseEquivalentEntity> AmbientDoseEquivalentFactors { get; set; } = null!;
        public DbSet<ExposureDoseEntity> ExposureDoseFactors { get; set; } = null!;
        public DbSet<Hp10Entity> Hp10Factors { get; set; } = null!;
        public DbSet<Hp007Entity> Hp007Factors { get; set; } = null!;

        public DbSet<RadionuclideEntity> Radionuclides { get; set; } = null!;
        public DbSet<RadionuclideEnergyIntensityEntity> RadionuclideEnergyIntensityData { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaseMaterialFactorEntity>().UseTpcMappingStrategy();
            modelBuilder.Entity<BaseDoseFactorEntity>().UseTpcMappingStrategy();
        }
    }
}
