# Exercise 2

## SIMD for text processing:

We'll start (emphasis on START!) exploring the use of SIMD/AVX2 for faster text processing:
Text isn't going anywhere as a way of storing/transmitting data. This is true for almost every industry out there.
It's almost 2020 and csv/Xml/Json are here to stay. That itself is an incentive to optimize text processing: On Intel CPUs that leaves us with
AVX instructions that are ideal for the task. We're going to start working on a textual parser for the FIX protocol, or the Financial Information eXchange protocol.

While I personally have much interest in this protocol, the general population is less aware of it. Anyhow, my reasons
for choosing this protocol:
* It's dead simple to explain
* Truly ueful: It is no exaggeration to claim that the world economy relies on this protocol.

## Background on the FIX Protocol
The FIX protocol is easy to implement, it's a generic, and flexible means of transmitting orders and market data via human readable text.
It is however, very old. At the time it was conjured the ability to transmit extensible, loosely structured data outweighed performance
considerations.

That said, FIX is only standardized, widely adopted protocol around the globe. 

FIX is a nightmare from a performance standpoint:
* Integers and decimals are transmitted as ASCII
  * Requiring extra bandwidth and
  * byte-by-byte conversion
* Messages aren't fixed length, and the to get useful messages we need intensive parsing. 

The basic FIX message looks like this:

```
message ::= (field soh)+
```

In other words, a message is at least one `field`, followed by an `soh`: a `0x1` ASCII char.

A `field` is composed or 3 parts:
```
field ::= tag "=" value
```

Again, pretty simple: `tag`, followed by and equals sign (`=`) and a `value`.

A `tag` is an ASCII encoded positive number, which we can assume to be always no larger than a simple int (32-bit).
tag must always be present.

A value is just any ASCII string.

Here's the full EBNF for this:

```
soh ::= '0x1'
char ::= "ASCII characters"
ascii_digit ::= [0-9]
value ::= char*
tag ::= ascii_digit+
field ::= tag "=" value
message ::= (field soh)+
```

Here's a sample message I used `▪` for the 0x1 charchter which is not normally printable:

```
0         1         2         3         4         5         6         7         8       
012345678901234567890123456789012345678901234567890123456789012345678901234567890
---------------------------------------------------------------------------------
8=FIX.4.1▪9=112▪35=0▪49=BRKR▪56=INVMGR▪34=237▪52=19980604-07:59:48▪10=225

```


This first excercise is about processing the message to determine field boundaries.

So the idea is that for the message above we would end up building a table like this:

| Index | Tag Offset | Value Offset |
|-------|------------|--------------|
| 0     | 0          | 2            |
| 1     | 10         | 12           |
| 2     | 16         | 19           |
| 3     | 21         | 24           |
| 4     | 29         | 32           |
| 5     | 39         | 42           |
| 6     | 46         | 49           |
| 7     | 67         | 70           |

This exercise requires that you think in vectors, Like you've never done before.

The good nes on the other hand is that if e make it, we can parse no less than 32-bytes of this
horrid-horrid protocol with each pass!

The provided function `GetFieldBounariesScalar` implements the scalar version of this.
Try and read it, and see that it makes sense to you!
It's super simple, unlike the hoops you'll have to jump for vectorization.

In order to keep things super simple/readable we make the following assumptions:

* The message is never empty
* After the last SOH, you will see straight null (\0) charchters
* The message will never exceed 1024 bytes, and you can assume that the buffer has that much bytes, in any case!
* We assume that the message is completely valid at this stage!
* We also pre-allocate the return value with arrays for the offsets that are suffcient in size!


Your mission, should you choose to accept it, is to write the vectorized counterpart for that function.

You'll need to use:

- AVX2 Loading / Storing
- Comparison
- Lzcnt/Tzcnt/Popcnt

You should probably do some RTFMing and plan how you want to tackle this.
There are many ways to get this right. And they all make use of slightly different intrinsics!
Which ones will you be using?

Have fun!
