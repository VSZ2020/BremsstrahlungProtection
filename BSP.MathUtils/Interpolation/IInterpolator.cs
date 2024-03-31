namespace BSP.MathUtils.Interpolation
{
    public interface IInterpolator
    {
        public double[] Interpolate(double[] x, double[] y, double[] new_x, AxisLogScale interpolationScaleType = AxisLogScale.None);
    }
}
