namespace BSP.BL.Interpolation.Functions
{
    public interface IInterpolator
    {
        public float[] Interpolate(float[] x, float[] y, float[] new_x);
    }
}
