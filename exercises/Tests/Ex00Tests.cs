using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Packing;
using Packing.Ex00;
using Packing.Ex03;

namespace Tests
{
    using FloatGenerator = Func<(float[] data, float stepSize, string reproContext)>;
    
    public class Ex00Tests
    {
        const int NUM_ITERATIONS = 100;

        public static IEnumerable<TestCaseData> MultiplyTests =>
            from stepSize in new[] {1.0f, 0.25f, 0.5f, 0.01f, 0.015625f}
            from i in Enumerable.Range(0, NUM_ITERATIONS)
            select new ArrayTestCaseData<float>(() => DataGenerator.GenerateFloatTestArray(stepSize, 66666)).SetArgDisplayNames($"R{i}/{stepSize}");
        


        [TestCaseSource(nameof(MultiplyTests))]
        public unsafe void TestScalar(FloatGenerator generator)
        {
            var (data, stepSize, reproContext) = generator();
            var copy = data.ToArray();
            fixed (float* dataPtr = &copy[0])
            {
                Multiply.MultiplyScalar(dataPtr, copy.Length, 1 / stepSize);
            }

            for (var i = 0; i < copy.Length; i++)
                copy[i] *= stepSize;
            
            Assert.That(copy, Is.EqualTo(data).Within(10).Ulps, reproContext);
        }


        [TestCaseSource(nameof(MultiplyTests))]
        public unsafe void TestVectorized(FloatGenerator generator)
        {
            var (data, stepSize, reproContext) = generator();
            var copy = data.ToArray();
            fixed (float* dataPtr = &copy[0])
            {
                Multiply.MultiplyVectorized(dataPtr, copy.Length, 1 / stepSize);
            }

            for (var i = 0; i < copy.Length; i++)
                copy[i] *= stepSize;
            
            Assert.That(copy, Is.EqualTo(data).Within(10).Ulps, reproContext);
        }
    }
}