using BSP.BL.Geometries;

namespace BSP.BL.Services
{
    public class GeometryService
    {
        private static Dictionary<Type, string> availableGeometries = new Dictionary<Type, string>()
        {
            {typeof(CylinderRadial), "Cylinder Radial" },
            {typeof(CylinderAxial), "Cylinder Axial" },
            {typeof(Parallelepiped), "Parallelepiped" },
            {typeof(PointGeometry), "Point" },
        };

        public static Dictionary<Type, string> Geometries => availableGeometries;


        public static BaseGeometry GetGeometryInstance(Type geometryType, float[] dimensions, int[] discreteness)
        {
            return Activator.CreateInstance(geometryType, new object[] { dimensions, discreteness }) as BaseGeometry;
        }

        public static IEnumerable<DimensionsInfo> GetDimensionsInfo(Type geometryType)
        {
            if (geometryType == typeof(CylinderRadial) || geometryType == typeof(CylinderAxial))
            {
                return new List<DimensionsInfo>()
                {
                    new DimensionsInfo(){ Name = "Radius", DefaultValue = 10, Discreteness = 100},
                    new DimensionsInfo(){ Name = "Height",DefaultValue = 30, Discreteness = 300},
                };
            }
            if (geometryType == typeof(Parallelepiped))
            {
                return new List<DimensionsInfo>()
                {
                    new DimensionsInfo(){ Name = "Thickness", DefaultValue = 10, Discreteness = 100},
                    new DimensionsInfo(){ Name = "Width", DefaultValue = 10, Discreteness = 100},
                    new DimensionsInfo(){ Name = "Height", DefaultValue = 10, Discreteness = 100},
                };
            }
            return Enumerable.Empty<DimensionsInfo>();
        }

        /// <summary>
        /// Возвращает ключ словаря, в котором содержится название для текущей локализации приложения
        /// </summary>
        /// <param name="geometryType"></param>
        /// <returns></returns>
        public static string GetTranslationKey(Type geometryType)
        {
            return geometryType switch
            {
                Type e when e == typeof(CylinderRadial) => "SourceFormCylinderRadial",
                Type e when e == typeof(CylinderAxial) => "SourceFormCylinderAxial",
                Type e when e == typeof(Parallelepiped) => "SourceFormParallelepiped",
                Type e when e == typeof(PointGeometry) => "SourceFormPoint",
                _ => null
            };
        }
    }
}
