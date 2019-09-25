using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using NUnit.Framework;
using Packing;
using Packing.Ex04;

namespace Tests
{
    using FloatGenerator = System.Func<(float[] data, float stepSize, string reproContext)>;

    public class Ex04Tests
    {
        const int NUM_ITERATIONS = 100;

        public static IEnumerable<TestCaseData> DecimalizeTests =>
            from stepSize in new[] { 1.0f, 0.25f, 0.5f, 0.01f, 0.015625f }
            from i in Enumerable.Range(0, NUM_ITERATIONS)
            select new ArrayTestCaseData<float>(() => DataGenerator.GenerateFloatTestArray(stepSize, 66666)).SetArgDisplayNames($"R{i}/{stepSize}");

        [TestCaseSource(nameof(DecimalizeTests))]
        public unsafe void TestScalar(FloatGenerator generator)
        {
            var (data, stepSize, reproContext) = generator();
            var copy = data.ToArray();

            fixed (float* dataPtr = &copy[0]) {
                var (exp, ints) = Decimalize.DecimalizeScalar(dataPtr, data.Length);
                var divisor = 1.0 / Math.Pow(10, exp);
                var floatsTest = ints.Select(i => (float)(i * divisor)).ToArray();
                Assert.That(floatsTest, Is.EqualTo(data).Within(10).Ulps, reproContext);
            }
        }

        [TestCaseSource(nameof(DecimalizeTests))]
        public unsafe void TestVectorized(FloatGenerator generator)
        {
            var (data, stepSize, reproContext) = generator();
            var copy = data.ToArray();

            fixed (float* dataPtr = &copy[0]) {
                var (exp, ints) = Decimalize.DecimalizeVectorized(dataPtr, data.Length);
                var divisor = 1.0 / Math.Pow(10, exp);
                var floatsTest = ints.Select(i => (float)(i * divisor)).ToArray();
                Assert.That(floatsTest, Is.EqualTo(data).Within(10).Ulps, reproContext);
            }
        }
    }
}