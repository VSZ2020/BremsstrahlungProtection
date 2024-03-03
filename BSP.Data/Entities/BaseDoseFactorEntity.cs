namespace BSP.Data.Entities
{
    public abstract class BaseDoseFactorEntity : BaseEntity
    {
        public float Energy { get; set; }

        public float Value { get; set; }
    }
}
