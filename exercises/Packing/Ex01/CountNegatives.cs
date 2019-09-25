using System;
using System.Runtime.Intrinsics.X86;

namespace Packing.Ex01
{
    public static class CountNegatives
    {
        public static unsafe int CountNegativesArray(int[] ints)
        {
            fixed (int* dataPtr = &ints[0])
                return Avx2.IsSupported ? CountNegativesVectorized(dataPtr, ints.Length) : CountNegativesScalar(dataPtr, ints.Length);
        } 

        internal static unsafe int CountNegativesVectorized(int *dataPtr, int length)
        {
            throw new NotImplementedException();
        }

        internal static unsafe int CountNegativesScalar(int* dataPtr, int n)
        {
            var negs = 0;
            for (var i = 0; i < n; i++)
            {
                negs += *dataPtr++ < 0 ? 1 :0;
            }

            return negs;
        }
    }
}