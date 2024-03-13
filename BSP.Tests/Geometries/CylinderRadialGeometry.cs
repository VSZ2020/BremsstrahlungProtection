using BSP.BL.Calculation;
using BSP.BL.Geometries;
using System.Diagnostics;

namespace BSP.Tests.Geometries
{
    public class CylinderRadialGeometry
    {
        SingleEnergyInputData input;

        [SetUp]
        public void Setup()
        {
            input = new()
            {
                massAttenuationFactors = [0.5f, 0],
                SourceDensity = 2.4f,
                CalculationPoint = new System.Numerics.Vector3(120, 0, 0),
                Layers = new List<BL.Materials.ShieldLayer>() { new BL.Materials.ShieldLayer() { D = 100, Density = 0.0012928f } }
            };
        }

       

        [Test]
        public void StandardIntegrator()
        {
            float[] dimensions = [20, 50];
            int[] discreteness = [100, 100];
            var processor = new CylinderRadial(dimensions, discreteness);
            var fluence = processor.StandardIntegrator(input);
            var expectedMathcad = 1.03373E-006;
            Assert.That(fluence, Is.EqualTo(1.02449E-06).Within(1e-7)); //Manual calculation
            Assert.That(fluence, Is.EqualTo(expectedMathcad).Within(1e-6)); //Mathcad
            Debug.WriteLine($"Parallelepiped:\nActual: {fluence}\nExpected: {expectedMathcad}\nDiff: {fluence / expectedMathcad}");
        }

        [Test]
        public void AlternativeIntegrator()
        {
            float[] dimensions = [20, 50];
            int[] discreteness = [100, 100];
            var processor = new CylinderRadial(dimensions, discreteness);
            var fluence = processor.AlternativeIntegrator(input);
            var expectedMathcad = 1.03373E-006;
            Assert.That(fluence, Is.EqualTo(1.02449E-06).Within(1e-7)); //Manual calculation
            Assert.That(fluence, Is.EqualTo(expectedMathcad).Within(1e-6)); //Mathcad
            Debug.WriteLine($"Parallelepiped (Simpson):\nActual: {fluence}\nExpected: {expectedMathcad}\nDiff: {fluence/ expectedMathcad}");
        }
    }
}