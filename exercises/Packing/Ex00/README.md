# Exercise 0

The first exercise is about getting used to using AVX2 intrinsics over arrays.

We'll be multiplying an array of floats with a constant.
Pretty boring stuff.

In other words, turning an input array such as this:

```
100.25, 99.75, 100.00, 101.25, 101.50, ...
```

With a constant multiplier of 4, into:

```
401, 399, 400, 405, 406, ...
```

The provided function `MultiplyScalar` achieves this.

Your mission, should you choose to accept it, is to write the vectorized counterpart for that function.

You'll need to use:

- AVX2 Loading / Storing
- Multiplication

There is no "tricky" part to this exercise, this is here to get you warmed up.


Remember that while the AVX/AVX2 Vectors are 8 32-bit floating point elements wide, you **cannot** assume that the input array has a size that is divisible by 8 without a remainder.
So make it a habit to always handle that edge case!

Have fun!