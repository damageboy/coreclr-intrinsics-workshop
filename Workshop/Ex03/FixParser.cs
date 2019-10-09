using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Workshop.Ex03
{
    public static class FixParser
    {
        const sbyte SOH = 1;
        const sbyte EQ = (sbyte) '=';

        public static unsafe int GetFieldBoundaries(ReadOnlySpan<byte> msg, int msgLength, Span<int> tagStarts, Span<int> valueStarts)
        {
            return Avx2.IsSupported ?
                GetFieldBoundariesVectorized(msg, msgLength, tagStarts, valueStarts) :
                GetFieldBoundariesScalar(msg, msgLength, tagStarts, valueStarts);
        }


        internal static unsafe int GetFieldBoundariesVectorized(ReadOnlySpan<byte> msg, int msgLength, Span<int> tagStarts, Span<int> valueStarts)
        {
            throw new NotImplementedException();
        }

        internal static unsafe int GetFieldBoundariesScalar(ReadOnlySpan<byte> msg, int msgLength, Span<int> tagStarts, Span<int> valueStarts)
        {
            fixed (byte *msgPtr = msg)
            fixed (int *tagStart = tagStarts)
            fixed (int *valStart = valueStarts) {
                int numFields = 0;
                var p = msgPtr;
                var ts = tagStart;
                var vs = valStart;

                *ts++ = 0;

	            for (var i = 0; i < msgLength; ++i) {
		            var c = *p++;
        		    if (c == EQ) {
		        	    *vs++ = i + 1;
                        numFields++;
        		    } else if(c == SOH) {
                        *ts++ = i + 1;
                    }
    		    }
                return numFields;
	        }
        }
    }
}
