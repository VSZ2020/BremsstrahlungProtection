using BSP.BL.Geometries;
using BSP.Geometries.SDK;
using System.Diagnostics;

namespace BSP.Tests.Geometries
{
    internal class CylinderAxialGeometry
    {
        SingleEnergyInputData input;

        [SetUp]
        public void Setup()
        {
            float[] dimensions = [20.0f, 50.0f];
            int[] discreteness = [100, 100];

            input = new()
            {
                Dimensions = dimensions,
                Discreteness = discreteness,
                MassAttenuationFactors = [0.5f, 0],
                SourceDensity = 2.4f,
                CalculationPoint = new System.Numerics.Vector3(100, 0, 0),
                Layers = new List<ShieldLayer>() { new ShieldLayer() { D = 100, Density = 0.0012928f } }
            };
        }

        [Test]
        public void StandardIntegrator()
        {

            var processor = new CylinderAxial();
            var fluence = processor.GetFluence(input);
            var expectedMathcad = 1.2678E-7;
            Assert.That(fluence, Is.EqualTo(expectedMathcad).Within(1e-6));//Mathcad
            Debug.WriteLine($"Parallelepiped:\nActual: {fluence}\nExpected: {expectedMathcad}\nDiff: {fluence / expectedMathcad}");
        }

        [Test]
        public void AlternativeIntegrator()
        {
            var processor = new CylinderAxial();
            var fluence = processor.AlternativeIntegrator(input);
            var expectedMathcad = 1.2678E-7;
            Assert.That(fluence, Is.EqualTo(expectedMathcad).Within(1e-6));//Mathcad
            Debug.WriteLine($"Parallelepiped:\nActual: {fluence}\nExpected: {expectedMathcad}\nDiff: {fluence / expectedMathcad}");
        }
    }
}
