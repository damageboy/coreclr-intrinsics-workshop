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
            bool IsRoundedProperly(float* pf, int length, float epsilon)
            {
                var vectorizedLen = length / 8;
                var end = pf + length;

                var epsPlusVec = Vector256.Create(epsilon);
                var epsMinusVec = Vector256.Create(-epsilon);
                for (var i = 0; i < vectorizedLen; i++, pf += 8)
                {
                    var vec = Avx.LoadVector256(pf);
                    var roundedVec = Avx.RoundToNearestInteger(vec);
                    var diffVec = Avx.Subtract(vec, roundedVec);
                    var c1 = Avx.Compare(diffVec, epsPlusVec, FloatComparisonMode.OrderedGreaterThanOrEqualNonSignaling);
                    var c2 = Avx.Compare(diffVec, epsMinusVec, FloatComparisonMode.OrderedLessThanOrEqualNonSignaling);
                    var res = Avx.Or(c1, c2);
                    var resScalar = Avx.MoveMask(res);
                    if (resScalar > 0)
                        return false;
                }
                
                for (; pf < end; pf++)
                    if (Abs((Round(*pf)) - *pf) > epsilon)
                        return false;

                return true;
            }

            void MultiplyBy(float* pf, int length, float mul)
            {
                var vectorizedLen = length / 8;
                var end = pf + length;

                var mulVec = Vector256.Create(mul);
                for (var i = 0; i < vectorizedLen; i++, pf += 8)
                {
                    var vec = Avx.LoadVector256(pf);
                    vec = Avx.Multiply(vec, mulVec);
                    Avx.Store(pf, vec);
                }

                for (; pf < end; pf++)
                    *pf *= mul;
            }

            var exp = 0;
            while (!IsRoundedProperly(dataPtr, length, EPSILON))
            {
                MultiplyBy(dataPtr, length, 10);
                exp++;
            }

            var vectorizedLen = length / 8;
            var result = new int[length];
            var pf = dataPtr;
            var end = pf + length;
            fixed (int* pResult = &result[0])
            {
                var pi = pResult;
                for (var i = 0; i < vectorizedLen; i++, pf += 8, pi += 8) {
                    var vec = Avx.LoadVector256(pf);
                    var roundedVec = Avx.RoundToNearestInteger(vec);
                    var intVec = Avx.ConvertToVector256Int32(roundedVec);
                    Avx.Store(pi, intVec);
                }
                
                while (pf < end)
                    *pi++ = (int) Round(*pf++);
            }

            return (exp, result);
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
