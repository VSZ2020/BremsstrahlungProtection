namespace BSP.BL.DTO
{
    public class RadionuclideDto : BaseDto
    {
        public string Name { get; set; }

        public double HalfLive { get; set; }

        public string HalfLiveUnits { get; set; }

        public float Z { get; set; }

        public float Weight { get; set; }
    }
}
