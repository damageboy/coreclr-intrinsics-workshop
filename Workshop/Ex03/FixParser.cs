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
            fixed (byte *msgPtr = msg)
            fixed (int *tagStart = tagStarts)
            fixed (int *valStart = valueStarts) {
                int numFields = 0;
                var p = msgPtr;
                var ts = tagStart;
                var vs = valStart;

                int N = Vector256<byte>.Count;

                *ts++ = 0;

                var remainder = msgLength % N;
                var endVectorized = msgPtr + (msgLength - remainder);

                var sohVec = Vector256.Create((byte) SOH);
                var eqVec = Vector256.Create((byte) EQ);

                while (p <= endVectorized) {
                    var text1 = Avx.LoadDquVector256(p);
                    var text2 = Avx.LoadDquVector256(p + N);

                    var sohMask = (ulong)(uint) Avx2.MoveMask(Avx2.CompareEqual(text1, sohVec));
                    var eqMask = (ulong) (uint) Avx2.MoveMask(Avx2.CompareEqual(text1, eqVec));
                    sohMask |= (ulong) Avx2.MoveMask(Avx2.CompareEqual(text2, sohVec)) << 32;
                    eqMask  |= (ulong) Avx2.MoveMask(Avx2.CompareEqual(text2, eqVec)) << 32;
                    var numSoh = (int) Popcnt.X64.PopCount(sohMask);
                    var numEq  = (int) Popcnt.X64.PopCount(eqMask);

                    for (var i = 0; i < numSoh; i++) {
                        var pos = (int) Bmi1.X64.TrailingZeroCount(Bmi2.X64.ParallelBitDeposit(1UL << i, sohMask)) + 1;
                        *ts++ = pos;
                    }

                    for (var i = 0; i < numEq; i++) {
                        var pos = (int) Bmi1.X64.TrailingZeroCount(Bmi2.X64.ParallelBitDeposit(1UL << i, eqMask));
                        *vs++ = pos;
                    }

                    p += N;
                }


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
