# Exercise 1

The first exercise is about implementing a decimalization function with AVX2 intrinsics.

We'll define decimalization as:

The process of finding a single power of 10 that can be used to multiplier for an array of floating point numbers, such that the resulting floating point array can be rounded / casted to an array of integers with an error smaller than epsilon (For example: `1E-6`).

In other words, turning an input such as this:

```
100.25, 99.75, 100.00, 101.25, 101.50, ...
```

Into:

```
10025, 9975, 10000, 10125, 10150, ...
```

And returning the power of 10 used for the multiplication, in the above case, this would be 2, since 102 is how much we need to multiply all the elements of the input.

The provided function `DecimalizeScalar` achieves this.

It's not the most efficient implementation we could write in the scalar "world", but it is certainly very readable and easy to understand.

Your mission, should you choose to accept it, is to write the vectorized counterpart for that function.

You'll need to use:

- AVX2 Loading / Storing
- Multiplication
- Rounding
- Subtraction
- Comparison
- More...

Try to plan ahead and think about what intrinsics you might need that you don't know about.

Read the Intel Intrinsics guide, and have fun RTFMing.

Remember that while the AVX/AVX2 Vectors are 8 32-bit floating point elements wide, you **cannot** assume that the input array has a size that is divisible by 8 without a remainder. And that your vector