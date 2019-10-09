using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Workshop.Ex03;

namespace Tests
{
    using IntGenerator = Func<(int[] data, int stepSize, string reproContext)>;

    public class Ex03Tests
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
            var numFields = FixParser.GetFieldBoundariesVectorized(bytes, byteLen, tagStarts, valueStarts);

            Assert.That(numFields, Is.EqualTo(8));
            Assert.That(tagStarts.Slice(0, numFields).ToArray(),   Is.EquivalentTo(new [] { 0, 10, 16, 21, 29, 39, 46, 67}));
            Assert.That(valueStarts.Slice(0, numFields).ToArray(), Is.EquivalentTo(new [] { 2, 12, 19, 24, 32, 42, 49, 70}));

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
            var numFields = FixParser.GetFieldBoundariesVectorized(bytes, byteLen, tagStarts, valueStarts);

            Assert.That(numFields, Is.EqualTo(8));
            Assert.That(tagStarts.Slice(0, numFields).ToArray(),   Is.EquivalentTo(new [] { 0, 10, 16, 21, 29, 39, 46, 67}));
            Assert.That(valueStarts.Slice(0, numFields).ToArray(), Is.EquivalentTo(new [] { 2, 12, 19, 24, 32, 42, 49, 70}));
        }
    }
}
