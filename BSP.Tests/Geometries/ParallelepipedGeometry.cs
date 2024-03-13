using BSP.BL.Calculation;
using BSP.BL.Geometries;

namespace BSP.Tests.Geometries
{
    public class ParallelepipedGeometry
    {
        SingleEnergyInputData input;

        [SetUp]
        public void Setup()
        {
            input = new()
            {
                massAttenuationFactors = [0.5f, 0],
                SourceDensity = 2.4f,
                CalculationPoint = new System.Numerics.Vector3(120,5,25),
                Layers = new List<BL.Materials.ShieldLayer>() { new BL.Materials.ShieldLayer() { D = 100, Density = 0.0012928f } }
            };
        }

        [Test]
        public void StandardIntegration()
        {
            float[] dimensions = [20.0f, 10.0f, 50.0f];
            int[] discreteness = [100, 100, 100];
            var processor = new Parallelepiped(dimensions, discreteness);
            var fluence = processor.StandardIntegrator(input);

            Assert.That(fluence, Is.EqualTo(2.48601E-07).Within(1e-7)); //Manual calculation
            Assert.That(fluence, Is.EqualTo(2.912E-007).Within(1e-7)); //Mathcad = True value
            Assert.That(fluence, Is.EqualTo(2.911747197848614e-07).Within(1e-7)); //Python integration by 'quad' func
        }

        [Test]
        public void AlternativeIntegration()
        {
            float[] dimensions = [20.0f, 10.0f, 50.0f];
            int[] discreteness = [100, 100, 100];
            var processor = new Parallelepiped(dimensions, discreteness);
            var fluenceSimpson = processor.AlternativeIntegration(input);

            Assert.That(fluenceSimpson, Is.EqualTo(2.48601E-07).Within(1e-7)); //Manual calculation
            Assert.That(fluenceSimpson, Is.EqualTo(2.912E-007).Within(1e-7)); //Mathcad = True value
            Assert.That(fluenceSimpson, Is.EqualTo(2.911747197848614e-07).Within(1e-7)); //Python integration by 'quad' func
        }
    }
}
