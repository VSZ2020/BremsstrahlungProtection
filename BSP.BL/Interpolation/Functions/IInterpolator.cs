namespace BSP.BL.Interpolation.Functions
{
    public interface IInterpolator
    {
        public double[] Interpolate(double[] x, double[] y, double[] new_x);
    }
}
