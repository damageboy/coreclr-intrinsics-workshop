using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Workshop.Ex02
{
    public static class FixValidator
    {
        const sbyte SOH = 1;
        const sbyte EQ = (sbyte) '=';

        public static unsafe (int numSoh, int numEq) ValidateSeparators(ReadOnlySpan<byte> msg, int msgLength)
        {
            return Avx2.IsSupported ?
                ValidateSeparatorsVectorized(msg, msgLength) :
                ValidateSeparatorsScalar(msg, msgLength);
        }


        internal static unsafe (int numSoh, int numEq) ValidateSeparatorsVectorized(ReadOnlySpan<byte> msg, int msgLength)
        {
            throw new NotImplementedException();
        }

        internal static unsafe (int numSoh, int numEq) ValidateSeparatorsScalar(ReadOnlySpan<byte> msg, int msgLength)
        {
            fixed (byte *msgPtr = msg) {
                var p = msgPtr;

                var numSoh = 0;
                var numEq = 0;

	            for (var i = 0; i < msgLength; ++i) {
		            var c = *p++;
        		    if (c == EQ) {
                        numSoh++;
        		    } else if(c == SOH) {
                        numEq++;
                    }
    		    }
                return (numSoh, numEq);
	        }
        }
    }
}
