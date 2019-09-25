using System;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using static System.Math;

namespace Packing.Ex04
{
    public static class Decimalize
    {
        const float EPSILON = 1E-6f;

        public static unsafe (int exp, int[] ints) DecimalizeArray(float[] floats)
        {
            fixed (float* dataPtr = &floats[0])
                return Avx2.IsSupported ? DecimalizeVectorized(dataPtr, floats.Length) : DecimalizeScalar(dataPtr, floats.Length);
        }

        internal static unsafe (int exp, int[] ints) DecimalizeVectorized(float *dataPtr, int length)
        {
            throw new NotImplementedException();
        }

        internal static unsafe (int exp, int[] ints) DecimalizeScalar(float* dataPtr, int length)
        {
            bool MaximalRemainderAbove(float *p, int length, float epsilon) {
                for (var i = 0; i < length; i++, p++) {
                    if (Abs((Round(*p)) - *p) > epsilon)
                        return true;
                }

                return false;
            }

            // While we have a significant remainder, multiply by 10. Any insignificant remainder
            // will be discarded. Also, always discard beyond the 7th digit
            var exp = 0;
            while (MaximalRemainderAbove(dataPtr, length, EPSILON) && exp < 7) {
                for (var i = 0; i < length; i++)
                    dataPtr[i] *= 10;
                exp += 1;
            }
            
            if (exp > 7)
                throw new Exception("Data cannot be decimalized");

            var result = new int[length];
            for (var i = 0; i < length; i++,dataPtr++)
                result[i] = (int) Round(*dataPtr);
            return (exp, result);
        }
    }
}
