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

        public static unsafe (int exp, int[] ints) DecimalizeArray(float[] floats) => 
            Avx2.IsSupported ? DecimalizeVectorized(floats) : DecimalizeScalar(floats);

        internal static unsafe (int exp, int[] ints) DecimalizeVectorized(float[] floats)
        {
            var copy = floats.ToArray();
            fixed (float* dataPtr = &copy[0])
                return DecimalizeVectorizedInPlace(dataPtr, floats.Length);
        }

        static unsafe (int exp, int[] ints) DecimalizeVectorizedInPlace(float *dataPtr, int length)
        {
            throw new NotImplementedException();
        }

        internal static (int exp, int[] ints) DecimalizeScalar(float[] dataPtr)
        {
            var floats = dataPtr.ToArray();
            var exp = 0;

            double MaximalRemainder(float[] a) => a.Select(x => Abs((Round(x)) - x)).Max();

            // While we have a significant remainder, multiply by 10. Any insignificant remainder
            // will be discarded. Also, always discard beyond the 7th digit
            // Note that the significance threshold must be smaller than the threshold/epsilon used when
            // doing data integrity checks on doubles.
            while (MaximalRemainder(floats) > EPSILON && exp < 7) {
                for (var i = 0; i < floats.Length; i++)
                    floats[i] *= 10;
                exp += 1;
            }
            
            if (exp > 7)
                throw new Exception("Data cannot be decimalized");

            var result = new int[floats.Length];
            for (var i = 0; i < floats.Length; i++)
                result[i] = (int) Round(floats[i]);
            return (exp, result);
        }
    }
}
