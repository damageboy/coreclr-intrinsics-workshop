using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using NUnit.Framework;
using Packing.Ex01;

namespace Tests
{
    using IntGenerator = Func<(int[] data, int stepSize, string reproContext)>;

    public class Ex01Tests
    {
        const int NUM_ITERATIONS = 100;

        public static IEnumerable<TestCaseData> DeltaTests =>
            from stepSize in new[] { 1, 10, 50, 666 }
            from i in Enumerable.Range(0, NUM_ITERATIONS)
            select new ArrayTestCaseData<int>(() => DataGenerator.GenerateEx01TestArray(stepSize, 66666 + i)).SetArgDisplayNames($"R{i}/{stepSize}");

        [TestCaseSource(nameof(DeltaTests))]
        public unsafe void TestScalar(IntGenerator generator)
        {
            var (data, stepSize, reproContext) = generator();

            fixed (int* dataPtr = &data[0]) {
                var numNegs = CountNegatives.CountNegativesScalar(dataPtr, data.Length);
                Assert.That(numNegs, Is.EqualTo(data.Count(i => i < 0)), reproContext);
                Console.WriteLine(numNegs);
            }
        }

        [TestCaseSource(nameof(DeltaTests))]
        public unsafe void TestVectorized(IntGenerator generator)
        {
            var (data, stepSize, reproContext) = generator();

            fixed (int* dataPtr = &data[0]) {
                var numNegs = CountNegatives.CountNegativesVectorized(dataPtr, data.Length);
                Assert.That(numNegs, Is.EqualTo(data.Count(i => i < 0)), reproContext);
                Console.WriteLine(numNegs);
            }
        }

        [TestCaseSource(nameof(DeltaTests))]
        public unsafe void TestVectorizedUnrolled(IntGenerator generator)
        {
            var (data, stepSize, reproContext) = generator();

            fixed (int* dataPtr = &data[0]) {
                var numNegs = CountNegatives.CountNegativesVectorizedUnrolled(dataPtr, data.Length);
                Assert.That(numNegs, Is.EqualTo(data.Count(i => i < 0)), reproContext);
                Console.WriteLine(numNegs);
            }
        }
    }
}
