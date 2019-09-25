using System;
using System.Runtime.Intrinsics.X86;

namespace Packing.Ex05
{
    public class GCD
    { 
        public static unsafe int GCDArray(int[] dataPtr)
        {
            fixed (int* pInt = &dataPtr[0])
                return Avx2.IsSupported ? GCDVectorized(pInt, dataPtr.Length) : GCDScalar(pInt, dataPtr.Length);
        } 
            
        static unsafe int GCDVectorized(int *dataPtr, int length)
        {
            throw new NotImplementedException();
        }

        internal static unsafe int GCDScalar(int* dataPtr, int n)
        {
            if (n == 1)
                return *dataPtr;

            var c = Gcd(dataPtr[0], dataPtr[1]);
            int i;
            int g;
            for (i = 1; i < n - 1; i++) {
                g = Gcd(c, dataPtr[i + 1]);
                c = g;
            }
            return c;
        }
        
        static int Gcd(int a, int b) => a < b ? GcdPrimitive(b, a) : GcdPrimitive(a, b);

        static int GcdPrimitive(int a, int b)
        {
            var rem = a % b;
            while (rem > 1)
            {
                a = b;
                b = rem;
                rem = a % b;
            }
            return rem == 0 ? b : 1;
        }

    }
}