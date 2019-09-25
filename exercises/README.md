# Floating Point Packing Exercises

## Intro

This is a series of exercises where in each exercise, we will re-write a non-vectorized function in C# and provide a vectorized variant.

Hopefully, more often than not, the vectorized version will run much more quickly than the non-vectorized version if we complete our task correctly.

This CoreCLR 3.0 Solution is separated into 4 projects:

* Packing
  * Where the actual code lives.
* Tests
  * [NUnit](https://github.com/nunit/docs/wiki) based unit tests.
* Bench
  * [BenchmarkDotNet](https://benchmarkdotnet.org/index.html) based benchmarks.
* Example
  * A plain console application that is similar to the benchmark project, but more suitable to run under performance monitoring tools such as `perf` and `VTune`.

## Exercises

Each exercise is placed in a separate folder: `Ex01`, `Ex02` etc.

For each such exercise we have:

* Top-level function that selects between the vectorized and non-vectorized variants according to actual hardware support at the CPU level
* A scalar function, that Works (Hopefully?)
  * This function will also have working unit tests in the `Tests` project.
* An empty vectorized skeleton that throws a [`NotImplementedException`](https://docs.microsoft.com/en-us/dotnet/api/system.notimplementedexception?redirectedfrom=MSDN&view=netcore-3.0)
  * This function will also have ready to run (but obviously failing!) unit tests in the `Test` project.
* A benchmark harness waiting for a working implementation

The basic work cycle should be:

1. Understand what the original does (by reading the exercise text + code)
2. Try and implement something (maybe even a limited test case)
3. Use the unit testing runner to run/debug
4. Once the vectorized tests pass (all of them!), run the benchmark

List of Exercises:

* [Exercise 0](Packing/Ex00/) - Multiply Array by Constant

* [Exercise 1](Packing/Ex01/) - Count # of Negative Numbers
* [Exercise 2](Packing/Ex02/) - Find Max Value in Array

* [Exercise 3](Packing/Ex03/) - Calculate a streaming "delta" series

- [Exercise 4](Packing/Ex04/) - Decimalize Floating Point Arrays
- [Exercise 5](Packing/Ex05/) - Find the GCD for an array of integers
- Exercise 6 - ZigZag Encoding