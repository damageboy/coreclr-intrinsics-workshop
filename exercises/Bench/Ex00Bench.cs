using System.Linq;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Packing;
using Packing.Ex00;
using Packing.Ex04;
using Tests;

namespace Bench
{
    [InvocationCount(InvocationsPerIteration)]
    [Config(typeof(ShortConfig))]
    public class Ex00Bench
    { 
        const int InvocationsPerIteration = 100;
        int _iterationIndex = 0;

        float[] _values;
        float[][] _arrays;
        GCHandle[] _handles;
        unsafe float*[] _arrayPtrs;

        
        [Params(100, 1_000, 10_000, 100_000, 1_000_000)]
        public int N;

        [GlobalSetup]
        public unsafe void Setup()
        {
            (_values, _, _) = DataGenerator.GenerateFloatTestArray(0.015625f, N);
            _arrays = Enumerable.Range(0, InvocationsPerIteration).Select(i => new float[N]).ToArray();
            _handles = _arrays.Select(a => GCHandle.Alloc(a, GCHandleType.Pinned)).ToArray();
            _arrayPtrs = new float*[InvocationsPerIteration];
            var i = 0;
            foreach (var ip in _handles.Select(h => h.AddrOfPinnedObject()))
                _arrayPtrs[i++] = (float*) ip.ToPointer();
        }

        [IterationCleanup]
        public void CleanupIteration() => _iterationIndex = 0; // after every iteration end we set the index to 0

        [IterationSetup]
        public void SetupArrayIteration() => DataGenerator.FillArrays(ref _arrays, InvocationsPerIteration, _values);

        [Benchmark(Baseline = true)]
        public unsafe void Scalar() => Multiply.MultiplyScalar(_arrayPtrs[_iterationIndex++], N, 1234.15f);
        
        //[Benchmark]
        public unsafe void Vectorized() => Multiply.MultiplyVectorized(_arrayPtrs[_iterationIndex++], N, 1234.15f);
    }
}