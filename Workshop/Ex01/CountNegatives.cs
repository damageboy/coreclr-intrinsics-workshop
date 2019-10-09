using System.Diagnostics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using static System.Runtime.Intrinsics.X86.Avx;
using static System.Runtime.Intrinsics.X86.Avx2;
using static System.Runtime.Intrinsics.X86.Popcnt;

namespace Workshop.Ex01
{
    public static class CountNegatives
    {
        public static unsafe int CountNegativesArray(int[] numbers)
        {
            fixed (int* dataPtr = &numbers[0])
                return Avx2.IsSupported ? CountNegativesVectorized(dataPtr, numbers.Length) : CountNegativesScalar(dataPtr, numbers.Length);
        }

        internal static unsafe int CountNegativesVectorized(int *dataPtr, int length)
        {
            var remainder = length % Vector256<int>.Count;
            var endVectorized = dataPtr + (length - remainder);
            var p = dataPtr;
            var zeroVec = Vector256<int>.Zero;

            var numNegatives = 0;
            while (p < endVectorized) {
                numNegatives += (int) PopCount((uint) MoveMask(CompareGreaterThan(zeroVec, LoadVector256(p)).AsSingle()));
                p += 8;
            }

            numNegatives += CountNegativesScalar(p, remainder);

            return numNegatives;

#if DEBUG
            Vector256<int> LoadVector256(int *p)
            {
                Debug.Assert(p >= dataPtr, "reading from before start of array");
                Debug.Assert(p <= (dataPtr + length) - 8, "reading from after end of array");

                return Avx.LoadVector256(p);
            }
#endif
        }

        internal static unsafe int CountNegativesVectorizedUnrolled(int *dataPtr, int length)
        {
            var N = Vector256<int>.Count;
            var remainder1 = length % N;
            var remainder4 = length % (N * 4);

            var endVectorized1 = dataPtr + (length - remainder1);
            var endVectorized4 = dataPtr + (length - remainder4);
            var p = dataPtr;
            var zeroVec = Vector256<int>.Zero;

            var numNegatives = 0;

            while (p < endVectorized4) {
                var mask = 0U;
                mask |= (uint) MoveMask(CompareGreaterThan(zeroVec, LoadVector256(p + N*0)).AsSingle());
                mask <<= N;
                mask |= (uint) MoveMask(CompareGreaterThan(zeroVec, LoadVector256(p + N*1)).AsSingle());
                mask <<= N;
                mask |= (uint) MoveMask(CompareGreaterThan(zeroVec, LoadVector256(p + N*2)).AsSingle());
                mask <<= N;
                mask |= (uint) MoveMask(CompareGreaterThan(zeroVec, LoadVector256(p + N*3)).AsSingle());
                numNegatives += (int) PopCount(mask);
                p += N * 4;
            }


            while (p < endVectorized1) {
                numNegatives += (int) PopCount((uint) MoveMask(CompareGreaterThan(zeroVec, LoadVector256(p)).AsSingle()));
                p += 8;
            }

            numNegatives += CountNegativesScalar(p, remainder1);

            return numNegatives;

#if DEBUG
            Vector256<int> LoadVector256(int *p)
            {
                Debug.Assert(p >= dataPtr, "reading from before start of array");
                Debug.Assert(p <= (dataPtr + length) - 8, "reading from after end of array");

                return Avx.LoadVector256(p);
            }
#endif
        }

        internal static unsafe int CountNegativesScalar(int* dataPtr, int n)
        {
            var negs = 0;
            for (var i = 0; i < n; i++) {
                negs += *dataPtr++ < 0 ? 1 :0;
            }

            return negs;
        }
    }
}
