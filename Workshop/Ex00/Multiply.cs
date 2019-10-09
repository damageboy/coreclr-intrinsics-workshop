using System;
using System.Diagnostics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using static System.Runtime.Intrinsics.X86.Avx;

namespace Workshop.Ex00
{
    public static class Multiply
    {
        public static unsafe void MultiplyArray(float[] data, float constant)
        {
            fixed (float* dataPtr = &data[0])
                if (Avx2.IsSupported)
                    MultiplyVectorized(dataPtr, data.Length, constant);
                else
                    MultiplyScalar(dataPtr, data.Length, constant);
        }

        internal static unsafe void MultiplyVectorized(float* dataPtr, int length, float constant)
        {
            throw new NotImplementedException();
        }

        internal static unsafe void MultiplyVectorizedUnrolled(float* dataPtr, int length, float constant)
        {
            throw new NotImplementedException();
        }


        internal static unsafe void MultiplyScalar(float* dataPtr, int n, float constant)
        {
            for (var i = 0; i < n; i++)
            {
                *dataPtr++ *= constant;
            }
        }
    }
}
