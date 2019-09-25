using System;
using System.Diagnostics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using static System.Runtime.Intrinsics.X86.Avx;

namespace Packing.Ex00
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
            var remainder = length % Vector256<float>.Count;
            var endVectorized = dataPtr + (length - remainder);
            var p = dataPtr;
            var constantVec = Vector256.Create(constant);

            while (p < endVectorized) {
                Store(p, Multiply(LoadVector256(p), constantVec));
                p += 8;
            }

            MultiplyScalar(p, remainder, constant);

#if DEBUG
            Vector256<float> LoadVector256(float *p)
            {
                Debug.Assert(p >= dataPtr, "reading from before start of array");
                Debug.Assert(p <= (dataPtr + length) - 8, "reading from after end of array");

                return Avx.LoadVector256(p);
            }
#endif
        }

        internal static unsafe void MultiplyVectorizedUnrolled(float* dataPtr, int length, float constant)
        {
            var N = Vector256<float>.Count;
            var remainder1 = length % N;
            var remainder4 = length % (N * 4);

            var endVectorized1 = dataPtr + (length - remainder1);
            var endVectorized4 = dataPtr + (length - remainder4);
            var p = dataPtr;
            var constantVec = Vector256.Create(constant);

            while (p < endVectorized4) {
                Store(p + N*0, Multiply(LoadVector256(p + N*0), constantVec));
                Store(p + N*1, Multiply(LoadVector256(p + N*1), constantVec));
                Store(p + N*2, Multiply(LoadVector256(p + N*2), constantVec));
                Store(p + N*3, Multiply(LoadVector256(p + N*3), constantVec));
                p += N * 4;
            }

            while (p < endVectorized1) {
                Store(p, Multiply(LoadVector256(p), constantVec));
                p += 8;
            }

            MultiplyScalar(p, remainder1, constant);

#if DEBUG
            Vector256<float> LoadVector256(float *p)
            {
                Debug.Assert(p >= dataPtr, "reading from before start of array");
                Debug.Assert(p <= (dataPtr + length) - 8, "reading from after end of array");

                return Avx.LoadVector256(p);
            }
#endif
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
