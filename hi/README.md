# Hello Scalar/Vectorized World

## Starting Point

`Program.cs` contains a simple `Main` that allocates+initializes 3 arrays:

- `bytes`
- `scalarDiff`
- `avx2Diff`

The code already detects AVX2 support at the CPU level, but throws a `NotImplementeException` in that case.

In case AVX2 is not available, it has ready-to-run code that adds `scalarDiff` to `bytes` element by element.

Finally, it constructs a string and prints the result to the console.

## Task

Replace the fragment that throws `NotImplementedException` with vectorized code
that does the same operation (element-by-element addition), but:
- Use `avx2Diff` instead of `scalarDiff`.
- Use AVX2 instructions
  - No loops / Linq!

## Running

- You can always use `dotnet run`
- You can use the provided `./run-with-scalar.{sh,cmd}` to force running without AVX2 support.
- You can use the provided `./run-with-avx2.{sh.cmd}` to force running *with* AVX2 (Although it should just use AVX2 if your CPU supports it...)
