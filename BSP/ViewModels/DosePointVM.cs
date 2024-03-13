namespace BSP.ViewModels
{
    public class DosePointVM
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float[] Values => [X, Y, Z];
    }
}
