using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace Bench
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher
                .FromAssembly(typeof(Program).Assembly)
                .Run(args);
        }
    }
    
    class LongConfig : ManualConfig
    {
        public LongConfig()
        {
            Add(Job.LongRun);
        }
    }

    class ShortConfig : ManualConfig
    {
        public ShortConfig()
        {
            Add(Job.ShortRun);
        }
    }
}
