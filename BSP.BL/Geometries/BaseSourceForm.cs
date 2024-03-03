using System;

namespace BSP.BL.Geometries
{
    public abstract class BaseSourceForm
    {
        /// <summary>
        /// Коэффициент, зависящий от формы источника. Если двумерный источник, то значение - длина или площадь. Если источник объемный, то значение - объем источника.
        /// </summary>
        /// <returns></returns>
        public abstract double GetNormalizationFactor();
    }

    public class CylinderForm : BaseSourceForm
    {
        public float Radius;
        public float Height;

        public int NRadius;
        public int NHeight;

        public override double GetNormalizationFactor()
        {
            return Height * Math.PI * Radius * Radius;
        }
    }

    public class ParallelepipedForm : BaseSourceForm
    {
        public float Thickness;
        public float Width;
        public float Height;

        public int NThickness;
        public int NWidth;
        public int NHeight;

        public override double GetNormalizationFactor()
        {
            return Height * Thickness * Width;
        }
    }
}
