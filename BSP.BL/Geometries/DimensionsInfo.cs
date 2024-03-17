namespace BSP.BL.Geometries
{
    public class DimensionsInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Units { get; set; } = "cm";
        public float DefaultValue { get; set; } = 10;
        public float MinValue { get; set; } = 0;
        public float MaxValue { get; set; } = float.MaxValue;
        public int Discreteness { get; set; } = 10;
        public bool IsValueEnabled { get; set; } = true;
        public bool IsDiscretenessEnabled { get; set; } = true;
    }
}
