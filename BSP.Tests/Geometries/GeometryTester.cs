using BSP.BL.Calculation;
using BSP.BL.Geometries;

namespace BSP.Tests.Geometries
{
    public class GeometryTests
    {
        SingleEnergyInputData input;

        [SetUp]
        public void Setup()
        {
            input = new()
            {
                massAttenuationFactors = [0.5f],
                SourceDensity = 2.4f,
                CalculationDistance = 80.0,
                Layers = new List<BL.Materials.ShieldLayer>()
            };
        }

        [Test]
        public void Parallelepiped()
        {
            float[] dimensions = [20.0f, 10.0f, 50.0f];
            int[] discreteness = [10, 10, 10];
            var processor = new Parallelepiped(dimensions, discreteness);
            var fluence = processor.GetFluence(input);
            Assert.That(fluence, Is.EqualTo(3.79123E-07).Within(1e-7));
        }

        [Test]
        public void CylinderRadial()
        {
            float[] dimensions = [20.0f, 3.141593f, 50.0f];
            int[] discreteness = [10, 10, 10];
            var processor = new CylinderRadial(dimensions, discreteness);
            var fluence = processor.GetFluence(input);
            Assert.That(fluence, Is.EqualTo(9.75742E-07).Within(1e-7));
        }

        [Test]
        public void CylinderAxial()
        {
            float[] dimensions = [20.0f, 3.141593f, 50.0f];
            int[] discreteness = [10, 10, 10];
            var processor = new CylinderAxial(dimensions, discreteness);
            var fluence = processor.GetFluence(input);
            Assert.That(fluence, Is.EqualTo(3.78e-5).Within(1e-7));
        }
    }
}