using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Tests;
using Workshop.Ex04;

namespace Bench
{
    [InvocationCount(InvocationsPerIteration)]
    [Config(typeof(ShortConfig))]
    public class Ex04Bench
    { 
        const int InvocationsPerIteration = 100;
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

        [IterationCleanup]
        public void CleanupIteration() => _iterationIndex = 0; // after every iteration end we set the index to 0

        [IterationSetup]
        public void SetupArrayIteration() => DataGenerator.FillArrays(ref _arrays, InvocationsPerIteration, _values);

        [Benchmark(Baseline = true)]
        public unsafe int Scalar() => Decimalize.DecimalizeScalar(_arrayPtrs[_iterationIndex++], N).exp;
        
        [Benchmark]
        public unsafe int Vectorized() => Decimalize.DecimalizeVectorized(_arrayPtrs[_iterationIndex++], N).exp;
    }
}
