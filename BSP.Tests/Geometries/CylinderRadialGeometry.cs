using BSP.BL.Calculation;
using BSP.BL.Geometries;
using System.Diagnostics;
using BSP.Geometries.SDK;

namespace BSP.Tests.Geometries
{
    public class CylinderRadialGeometry
    {
        SingleEnergyInputData input;

        [SetUp]
        public void Setup()
        {
            float[] dimensions = [20, 50];
            int[] discreteness = [200, 500];
            
            input = new()
            {
                Dimensions = dimensions,
                Discreteness = discreteness,
                MassAttenuationFactors = [0.5f, 0],
                SourceDensity = 2.4f,
                CalculationPoint = new System.Numerics.Vector3(120, 0, 0),
                Layers = new List<ShieldLayer>() { new ShieldLayer() { D = 100, Density = 0.0012928f } }
            };
        }

       

        [Test]
        public void StandardIntegrator()
        {
            var processor = new CylinderRadial();
            var fluence = processor.StandardIntegrator(input);
            var expectedMathcad = 1.52624e-7;
            Assert.That(fluence, Is.EqualTo(expectedMathcad).Within(1e-6)); //Mathcad
            Debug.WriteLine($"Parallelepiped:\nActual: {fluence}\nExpected: {expectedMathcad}\nDiff: {fluence / expectedMathcad}");
        }

        [Test]
        public void AlternativeIntegrator()
        {
            var processor = new CylinderRadial();
            var fluence = processor.AlternativeIntegrator(input);
            var expectedMathcad = 1.52624e-7;
            Assert.That(fluence, Is.EqualTo(expectedMathcad).Within(1e-6)); //Mathcad
            Debug.WriteLine($"Parallelepiped (Simpson):\nActual: {fluence}\nExpected: {expectedMathcad}\nDiff: {fluence/ expectedMathcad}");
        }

        [Test]
        public void SelfabsorptionLength()
        {
            var processor = new CylinderRadial();
            var fluence = processor.SelfabsorptionLength(10.0, 12.299999999999972, 1.0053096491487337,100, 20);
            Assert.That(fluence, Is.EqualTo(13.442).Within(1e-3)); //Mathcad
        }
    }
}