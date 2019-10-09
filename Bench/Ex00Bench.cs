using System.Linq;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Tests;
using Workshop.Ex00;

namespace Bench
{
    [InvocationCount(InvocationsPerIteration)]
    [Config(typeof(ShortConfig))]
    public class Ex00Bench
    { 
        const int InvocationsPerIteration = 1000;
        int _iterationIndex;

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
            (_arrays, _handles, _arrayPtrs)  = BenchUtils.GenerateArrayWithPointers<float>(InvocationsPerIteration, N);
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
        public unsafe void Scalar() => Multiply.MultiplyScalar(_arrayPtrs[_iterationIndex++], N, 1234.15f);
        
        [Benchmark]
        public unsafe void Vectorized() => Multiply.MultiplyVectorized(_arrayPtrs[_iterationIndex++], N, 1234.15f);

        [Benchmark]
        public unsafe void VectorizedUnrolled() => Multiply.MultiplyVectorizedUnrolled(_arrayPtrs[_iterationIndex++], N, 1234.15f);

    }
}
