using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Packing.Ex02;
using Tests;

namespace Bench
{
    [InvocationCount(InvocationsPerIteration)]
    [Config(typeof(ShortConfig))]
    public class Ex02Bench
    { 
        const int InvocationsPerIteration = 100;
        int _iterationIndex;

        int[] _values;
        int[][] _arrays;
        GCHandle[] _handles;
        unsafe int*[] _arrayPtrs;

        
        [Params(100, 1_000, 10_000, 100_000, 1_000_000)]
        public int N;

        [GlobalSetup]
        public unsafe void Setup()
        {
            (_values, _, _) = DataGenerator.GenerateEx01TestArray(333, N);
            (_arrays, _handles, _arrayPtrs)  = BenchUtils.GenerateArrayWithPointers<int>(InvocationsPerIteration, N);
        }

        [GlobalCleanup]
        public unsafe void Cleanup()
        {
            for (var i = 0; i < InvocationsPerIteration; i++) {
                _handles[i].Free();
                _arrays[i] = null;
                _arrayPtrs[i] = null;
            }
        }

        [IterationCleanup]
        public void CleanupIteration() => _iterationIndex = 0; // after every iteration end we set the index to 0

        [IterationSetup]
        public void SetupArrayIteration() => DataGenerator.FillArrays(ref _arrays, InvocationsPerIteration, _values);

        [Benchmark(Baseline = true)]
        public unsafe void Scalar() => Max.MaxScalar(_arrayPtrs[_iterationIndex++], N);
        
        [Benchmark]
        public unsafe void Vectorized() => Max.MaxVectorized(_arrayPtrs[_iterationIndex++], N);
    }
}
