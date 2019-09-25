using System.Linq;
using System.Runtime.InteropServices;

namespace Bench
{
    public class BenchUtils
    {
        internal static unsafe (T[][] arrays, GCHandle[] handles, T*[] ptrs) GenerateArrayWithPointers<T>(int numArrays, int arraySize)
            where T : unmanaged
        {
            var arrays = Enumerable.Range(0, numArrays).Select(i => new T[arraySize]).ToArray();
            var handles = arrays.Select(a => GCHandle.Alloc(a, GCHandleType.Pinned)).ToArray();
            var ptrs = new T*[numArrays];
            var i = 0;
            foreach (var ip in handles.Select(h => h.AddrOfPinnedObject()))
                ptrs[i++] = (T*)ip.ToPointer();

            return (arrays, handles, ptrs);
        }        
    }
}
