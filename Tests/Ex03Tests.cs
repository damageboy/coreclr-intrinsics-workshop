using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Workshop;
using Workshop.Ex03;

namespace Tests
{
    using IntGenerator = Func<(int[] data, int stepSize, string reproContext)>;

    public class Ex03Tests
    {
        const int NUM_ITERATIONS = 100;

        public static IEnumerable<TestCaseData> DeltaTests =>
            from stepSize in new[] { 1, 10, 17 }
            from i in Enumerable.Range(0, NUM_ITERATIONS)
            select new ArrayTestCaseData<int>(() => DataGenerator.GenerateEx03TestArray(stepSize, 66666)).SetArgDisplayNames($"R{i}/{stepSize}");

        [TestCaseSource(nameof(DeltaTests))]
        public unsafe void TestScalar(IntGenerator generator)
        {
            var (data, stepSize, reproContext) = generator();
            var copy = data.ToArray();
            int start;
            fixed (int* dataPtr = &copy[0]) {
                start = Delta.DeltaScalar(dataPtr, copy.Length);
            }

            Assert.That(copy[0], Is.EqualTo(0));
            copy[0] = start;
            for (var i = 1; i < copy.Length; i++)
                copy[i] += copy[i - 1];

            Assert.That(copy, Is.EqualTo(data), reproContext);
        }

        [TestCaseSource(nameof(DeltaTests))]
        public unsafe void TestVectorized(IntGenerator generator)
        {
            var (data, stepSize, reproContext) = generator();
            var copy = data.ToArray();
            int start;
            fixed (int* dataPtr = &copy[0]) {
                start = Delta.DeltaVectorized(dataPtr, copy.Length);
            }

            Assert.That(copy[0], Is.EqualTo(0));
            copy[0] = start;
            for (var i = 1; i < copy.Length; i++)
                copy[i] += copy[i - 1];

            Assert.That(copy, Is.EqualTo(data), reproContext);
        }
    }
}