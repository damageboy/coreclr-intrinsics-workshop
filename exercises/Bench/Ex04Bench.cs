using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Packing;
using Packing.Ex04;
using Tests;

namespace Bench
{
    [InvocationCount(InvocationsPerIteration)]
    [Config(typeof(ShortConfig))]
    public class Ex04Bench
    { 
        const int InvocationsPerIteration = 100;
        int _iterationIndex = 0;

        float[] _values;
        float[][] _arrays;

        
        [Params(100, 1_000, 10_000, 100_000, 1_000_000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            (_values, _, _) = DataGenerator.GenerateFloatTestArray(0.015625f, N);
        }

        [IterationCleanup]
        public void CleanupIteration() => _iterationIndex = 0; // after every iteration end we set the index to 0

        [IterationSetup]
        public void SetupArrayIteration() => DataGenerator.FillArrays(ref _arrays, InvocationsPerIteration, _values);

        [Benchmark(Baseline = true)]
        public int Scalar() => Decimalize.DecimalizeScalar(_arrays[_iterationIndex++]).exp;
        
        [Benchmark]
        public int Vectorized() => Decimalize.DecimalizeVectorized(_arrays[_iterationIndex++]).exp;
    }
}