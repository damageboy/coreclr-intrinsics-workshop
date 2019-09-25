using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using NUnit.Framework;

namespace Tests
{
    public class ArrayTestCaseData<T> : TestCaseData
    {
        public ArrayTestCaseData(System.Func<(T[] data, T stepSize, string reproContext)> generator) : base(generator) { }
    }

    public static class DataGenerator
    {
        public static (float[] data, float stepSize, string reproContext) GenerateFloatTestArray(float stepSize, int size)
        {
            var minValue = (int)(1 / stepSize);
            var maxValue = minValue + 10000;

            var seed = (int)DateTime.UtcNow.Ticks;
            var r = new Random(seed);
            var data = Enumerable.Repeat(0, size).Select(_ => r.Next(minValue, maxValue) * stepSize).ToArray();

            var reproContext = "";

            using (var sha1 = new SHA1CryptoServiceProvider()) {
                Span<byte> hash = stackalloc byte[20];
                sha1.TryComputeHash(MemoryMarshal.Cast<float, byte>(new ReadOnlySpan<float>(data)), hash, out _);
                var dataHash = Convert.ToBase64String(hash);

                reproContext = $"[{seed},{size},{stepSize}] -> [{dataHash}]";
            }

            return (data, stepSize, reproContext);
        }

        public static (int[] data, int stepSize, string reproContext) GenerateEx01TestArray(int param, int size)
        {
            var seed = (int)DateTime.UtcNow.Ticks;
            var r = new Random(seed);
            var data = Enumerable.Repeat(0, size).Select(_ => r.Next(-1000 + param, +1000 + param)).ToArray();

            var reproContext = "";

            using (var sha1 = new SHA1CryptoServiceProvider()) {
                Span<byte> hash = stackalloc byte[20];
                sha1.TryComputeHash(MemoryMarshal.Cast<int, byte>(new ReadOnlySpan<int>(data)), hash, out _);
                var dataHash = Convert.ToBase64String(hash);

                reproContext = $"[{seed},{size},{param}] -> [{dataHash}]";
            }

            return (data, param, reproContext);
        }


        public static (int[] data, int stepSize, string reproContext) GenerateEx03TestArray(int stepSize, int size)
        {
            var seed = (int)DateTime.UtcNow.Ticks;
            var r = new Random(seed);
            var data = Enumerable.Repeat(0, size).Select(_ => r.Next(0, stepSize)).ToArray();

            var reproContext = "";

            using (var sha1 = new SHA1CryptoServiceProvider()) {
                Span<byte> hash = stackalloc byte[20];
                sha1.TryComputeHash(MemoryMarshal.Cast<int, byte>(new ReadOnlySpan<int>(data)), hash, out _);
                var dataHash = Convert.ToBase64String(hash);

                reproContext = $"[{seed},{size},{stepSize}] -> [{dataHash}]";
            }

            return (data, stepSize, reproContext);
        }

        public static void FillArrays<T>(ref T[][] arrays, int collectionsCount, T[] source)
        {
            if (arrays == null)
                arrays = Enumerable.Range(0, collectionsCount).Select(_ => new T[source.Length]).ToArray();

            foreach (var array in arrays)
                System.Array.Copy(sourceArray: source, destinationArray: array, length: source.Length);

            if (arrays.Any(collection => collection.Length != source.Length)) // we dont use Debug.Assert here because this code will be executed mostly in Release
                throw new InvalidOperationException();
        }

    }
}