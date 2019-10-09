using System;
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
            throw new NotImplementedException();
        }

        internal static unsafe int CountNegativesVectorizedUnrolled(int *dataPtr, int length)
        {
            throw new NotImplementedException();
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
