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
                CalculationDistance = 100.0,
                Layers = new List<BL.Materials.ShieldLayer>()
            };
        }

        [Test]
        public void Parallelepiped()
        {
            float[] dimensions = [20.0f, 10.0f, 50.0f];
            int[] discreteness = [100, 100, 100];
            var processor = new Parallelepiped(dimensions, discreteness);
            var fluence = processor.GetFluence(input);
            Assert.That(fluence, Is.EqualTo(2.48601E-07).Within(1e-7)); //Manual calculation
            Assert.That(fluence, Is.EqualTo(2.912E-007).Within(1e-7)); //Mathcad = True value
            Assert.That(fluence, Is.EqualTo(2.911747197848614e-07).Within(1e-7)); //Python integration by 'quad' func
        }

        [Test]
        public void ParallelepipedTestSimpson()
        {
            float[] dimensions = [20.0f, 10.0f, 50.0f];
            int[] discreteness = [100, 100, 100];
            var processor = new Parallelepiped(dimensions, discreteness);
            var fluenceSimpson = processor.AlternativeIntegration(input);
            Assert.That(fluenceSimpson, Is.EqualTo(2.48601E-07).Within(1e-7)); //Manual calculation
            Assert.That(fluenceSimpson, Is.EqualTo(2.912E-007).Within(1e-7)); //Mathcad = True value
            Assert.That(fluenceSimpson, Is.EqualTo(2.911747197848614e-07).Within(1e-7)); //Python integration by 'quad' func
        }

        [Test]
        public void CylinderRadial()
        {
            float[] dimensions = [20, 50];
            int[] discreteness = [1000, 100];
            var processor = new CylinderRadial(dimensions, discreteness);
            var fluence = processor.GetFluence(input);
            Assert.That(fluence, Is.EqualTo(1.02449E-06).Within(1e-7));//Manual calculation
            Assert.That(fluence, Is.EqualTo(1.518E-006).Within(1e-6));//Mathcad
        }

        [Test]
        public void CylinderRadialWithSimpson()
        {
            float[] dimensions = [20, 50];
            int[] discreteness = [1000, 100];
            var processor = new CylinderRadial(dimensions, discreteness);
            var fluence = processor.AlternativeIntegrator(input);
            Assert.That(fluence, Is.EqualTo(1.02449E-06).Within(1e-7));//Manual calculation
            Assert.That(fluence, Is.EqualTo(1.518E-006).Within(1e-6));//Mathcad
        }


        [Test]
        public void CylinderAxial()
        {
            float[] dimensions = [20.0f, 3.141593f, 50.0f];
            int[] discreteness = [100, 100, 100];
            var processor = new CylinderAxial(dimensions, discreteness);
            var fluence = processor.GetFluence(input);
            //Assert.That(fluence, Is.EqualTo(3.78e-5).Within(1e-7));
            Assert.That(fluence, Is.EqualTo(1.593E-006).Within(1e-6));//Mathcad
        }
    }
}