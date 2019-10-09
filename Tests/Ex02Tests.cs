using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Workshop.Ex02;
using Workshop.Ex03;

namespace Tests
{
    using IntGenerator = Func<(int[] data, int stepSize, string reproContext)>;

    public class Ex02Tests
    {
        const int NUM_ITERATIONS = 100;

        public static IEnumerable<TestCaseData> DeltaTests =>
            from stepSize in new[] { 1, 10, 50, 666 }
            from i in Enumerable.Range(0, NUM_ITERATIONS)
            select new ArrayTestCaseData<int>(() => DataGenerator.GenerateEx01TestArray(stepSize, 66666)).SetArgDisplayNames($"R{i}/{stepSize}");

        const string msg = "8=FIX.4.1\u00019=112\u000135=0\u000149=BRKR\u000156=INVMGR\u000134=237\u000152=19980604-07:59:48\u000110=225\u0001";

        [Test]
        public unsafe void TestScalar()
        {
            Span<byte> bytes = stackalloc byte[1024];
            Span<int> tagStarts = stackalloc int[256];
            Span<int> valueStarts = stackalloc int[256];
            bytes.Clear();
            var byteLen = Encoding.ASCII.GetBytes(msg.AsSpan(), bytes);

            Assert.That(byteLen, Is.EqualTo(msg.Length));
            var (numSoh, numEq) = FixValidator.ValidateSeparatorsScalar(bytes, byteLen);

            Assert.That(numSoh, Is.EqualTo(8));
            Assert.That(numEq, Is.EqualTo(8));
        }


        [Test]
        public unsafe void TestVectorized()
        {
            Span<byte> bytes = stackalloc byte[1024];
            Span<int> tagStarts = stackalloc int[256];
            Span<int> valueStarts = stackalloc int[256];
            bytes.Clear();
            var byteLen = Encoding.ASCII.GetBytes(msg.AsSpan(), bytes);

            Assert.That(byteLen, Is.EqualTo(msg.Length));
            var (numSoh, numEq) =  FixValidator.ValidateSeparatorsVectorized(bytes, byteLen);

            Assert.That(numSoh, Is.EqualTo(8));
            Assert.That(numEq, Is.EqualTo(8));        }
    }
}
