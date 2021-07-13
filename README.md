# Boids using Unity's C# Job System

## Purpose
The purpose of this experiment was to get some experience with Unity's C# Job system. I was insprired to solve the Boid algorithm after watching [The Coding Train's, Coding Challenge #124: Flocking Simulation](https://www.youtube.com/watch?v=mhjuuHl6qHM). I'll share what I learned in this README.

The GIF below shows the results:

![Animated Gif of the Implemented Boid System](BoidsJobSystem.gif)

There's a lot of room for optimizing my solution. I just took whatever performance increases the job system gave me and slapped `[BurstCompile]` on the jobs. The GIF shows a little more than 12K boids flocking in the mid 30fps range.

## Using the Job System
At the time of this writing the C# Job System is in the "Experimental" phase. I also discovered that to find it in the package manager, I had to use the 2020 LTS version of Unity. Also, you also have to enable "Experimental Packages" in the package manager.

## First Rule of the Job System, Don't Use the Job System (First).

Work out the algorithm outside of the job system. As with most multithreading apps it's more difficult to debug and test. For example, it took me a long time to figure out I wasn't resetting the count in my `FindNeighborsJob`.

```csharp
      for (int inner = 0; inner < Positions.Length && count < 16; inner++)
      {
        if (inner == index) continue;

        var distSqrd = math.lengthsq(Positions[index] - Positions[inner]);
        if(distSqrd <= radiusSqrd)
        {
          localNeighbors = localNeighbors.WithValueAt(count, inner);
          count++;
        }
      }
      Neighbors[index] = localNeighbors;
      localNeighbors = new int4x4(-1);
      count = 0;  // ARGH!!! RESET COUNT
    }
```

## The `Unity.Mathmetics` Library is it's Own Beastie
I learned about the `math` library for working with the new blittable structure types. This was new to me. My understanding of the world "blittable" means it's a primitive enough type that it can be easily translated into other runtime languages, is a value type, and basically can be easily crunched in the CPU registers.

Using value-types is something you have to adjust to coming from a reference type habbit. Take for example the `Random` class. In the Mathmatics package, `Random` is a struct, and therefore, a value type. When you pass an instance of `Random`, a copy is created as part of the argument. The job system is multi-threaded, so each thread starts with the same random state. When a `Next` whatever is called, because each instance of `Random` is at the same state, identical values are returned. If that's not the desired behavior, don't call `Next` across multiple threads.

## The Native Containers
The Native Containers are the way the job system manages access to arrays, lists, and even hash maps with multi threading in mind. They're used extensively in the Job system and are the way to pass values for the parallel jobs. The types they manage need to be blittable, but they themselves aren't. 

For example, I wanted to calculate the neighbors once, not once per phase of the algorithm, so I created the `FindNeighborsJob`. The idea was each boid (identified by the index in the important arrays), would have a list of neighboring jobs. The system doesn't like `NativeHashMap<int, NativeList<int>>`. You'll get a runtime error with the burst compiler. An experiment, I wouldn't entirely recommend (it limits your list size and it's really hacky), was I kludged together a rudimentary list on top of the `int4x4` data structure. The `int4x4` data structure holds 4, 4-part integer vectors for a total of 16 indexes. 


I wish I would have taken the time to put tests around my kooky list implementation over the `int4x4`. That would have saved me a ton of time debugging.

## Links and Inspriation from [The Coding Train's, Coding Challenge #124: Flocking Simulation](https://www.youtube.com/watch?v=mhjuuHl6qHM)

* [Craig Reynold's Site Describing the Original Boid Research](https://www.red3d.com/cwr/boids/)
* [Daniel Shiftman's, Nature of Code Book](https://natureofcode.com/book/)
* [The Computational Beauty of Nature](https://mitpress.mit.edu/books/computational-beauty-nature)
