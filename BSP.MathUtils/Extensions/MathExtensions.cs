namespace BSP.MathUtils.Extensions
{
    public static class MathExtensions
    {
        public static double[] ToLog10(this double[] items)
        {
            return items.Select(item => Math.Log10(item)).ToArray();
        }

        public static double[] ToLog10(this float[] items)
        {
            return items.Select(item => Math.Log10(item)).ToArray();
        }

        public static double[] ToLinear(this double[] items)
        {
            return items.Select(item => Math.Pow(10, item)).ToArray();
        }
    }
}
