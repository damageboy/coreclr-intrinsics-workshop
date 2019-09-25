using System;
using System.Runtime.Intrinsics.X86;

namespace Packing.Ex02
{
    public class Max
    {
        public static unsafe int MaxArray(int[] ints)
        {
            fixed (int* dataPtr = &ints[0])
                return Avx2.IsSupported ? MaxVectorized(dataPtr, ints.Length) : MaxScalar(dataPtr, ints.Length);
        } 

        internal static unsafe int MaxVectorized(int *dataPtr, int length)
        {
            throw new NotImplementedException();
        }

        internal static unsafe int MaxScalar(int* dataPtr, int n)
        {
            var max = int.MinValue;
            for (var i = 0; i < n; i++, dataPtr++)
                if (*dataPtr > max)
                    max = *dataPtr;

            return max;
        }
    }
}