using System;
using System.Runtime.Intrinsics.X86;

namespace Packing.Ex03
{
    public static class Delta
    {
        public static unsafe int DeltaArray(int[] ints)
        {
            fixed (int* pInt = &ints[0])
                return Avx2.IsSupported ? DeltaVectorized(pInt, ints.Length) : DeltaScalar(pInt, ints.Length);
        } 

        internal static unsafe int DeltaVectorized(int *ints, int length)
        {
            throw new NotImplementedException();
        }

        internal static unsafe int DeltaScalar(int* a, int n)
        {
            var first = *a;
            int last = first;
            int next;
            for (var i = 1; i < n; i++)
            {
                next = a[i];
                a[i] -= last;
                last = next;
            }

            a[0] = 0;
            return first;
        }
    }
}