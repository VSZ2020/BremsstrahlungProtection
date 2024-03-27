using System;
using BSP.Geometries.SDK;

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


    public class EmptyForm : BaseSourceForm
    {
        public override double GetNormalizationFactor()
        {
            return 1;
        }

        public static IEnumerable<DimensionsInfo> GetDimensionsInfo()
        {
            return Enumerable.Empty<DimensionsInfo>();
        }
    }


    public class CylinderForm : BaseSourceForm
    {
        public float Radius;
        public float Height;
        public float Angle;

        public int NRadius;
        public int NHeight;
        public int NAngle;

        public override double GetNormalizationFactor()
        {
            return Height * Math.PI * Radius * Radius;
        }

        public static IEnumerable<DimensionsInfo> GetDimensionsInfo()
        {
            return new List<DimensionsInfo>()
            {
                new DimensionsInfo(){ Name = "Radius", DefaultValue = 10, Discreteness = 100},
                new DimensionsInfo(){ Name = "Height",DefaultValue = 30, Discreteness = 300},
                new DimensionsInfo(){ Name = "Angle", DefaultValue = 360, Discreteness = 10, IsValueEnabled = false},
            };
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

        public static IEnumerable<DimensionsInfo> GetDimensionsInfo()
        {
            return new List<DimensionsInfo>()
            {
                new DimensionsInfo(){ Name = "Thickness", DefaultValue = 10, Discreteness = 100},
                new DimensionsInfo(){ Name = "Width", DefaultValue = 10, Discreteness = 100},
                new DimensionsInfo(){ Name = "Height", DefaultValue = 10, Discreteness = 100},
            };
        }
    }
}
