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
            int[] discreteness = [200, 500];
            var processor = new CylinderRadial(dimensions, discreteness);
            var fluence = processor.StandardIntegrator(input);
            var expectedMathcad = 1.52624e-7;
            Assert.That(fluence, Is.EqualTo(expectedMathcad).Within(1e-6)); //Mathcad
            Debug.WriteLine($"Parallelepiped:\nActual: {fluence}\nExpected: {expectedMathcad}\nDiff: {fluence / expectedMathcad}");
        }

        [Test]
        public void AlternativeIntegrator()
        {
            float[] dimensions = [20, 50];
            int[] discreteness = [200, 500];
            var processor = new CylinderRadial(dimensions, discreteness);
            var fluence = processor.AlternativeIntegrator(input);
            var expectedMathcad = 1.52624e-7;
            Assert.That(fluence, Is.EqualTo(expectedMathcad).Within(1e-6)); //Mathcad
            Debug.WriteLine($"Parallelepiped (Simpson):\nActual: {fluence}\nExpected: {expectedMathcad}\nDiff: {fluence/ expectedMathcad}");
        }

        [Test]
        public void SelfabsorptionLength()
        {
            float[] dimensions = [20, 50];
            int[] discreteness = [100, 100];
            var processor = new CylinderRadial(dimensions, discreteness);
            var fluence = processor.SelfabsorptionLength(10.0, 12.299999999999972, 1.0053096491487337,100, 20);
            Assert.That(fluence, Is.EqualTo(13.442).Within(1e-3)); //Mathcad
        }
    }
}