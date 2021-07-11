using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct BoidAvoidanceJob : IJobParallelFor
{
  public float Speed;
  public float MaxForce;

  [ReadOnly]
  public NativeHashMap<int, int4x4> Neighbors;

  [ReadOnly]
  public NativeArray<float3> Positions;

  [ReadOnly]
  public NativeArray<float3> Velocities;

  public NativeArray<float3> Forces;

  public void Execute(int index)
  {
    var myPosition = Positions[index];
    var localNeighbors = Neighbors[index];
    if (localNeighbors.Count() > 0)
    {
      var avoidance = float3.zero;
      for (int i = 0; i < localNeighbors.Count(); i++)
      {
        var myNeighbor = localNeighbors.At(i);
        var collisionVector = myPosition - Positions[myNeighbor];
        var distSqrd = math.lengthsq(collisionVector);

        if (distSqrd > 0)
        {
          avoidance += math.normalizesafe(collisionVector) / distSqrd;
        }

        avoidance /= localNeighbors.Count();
        avoidance = avoidance.magnitude(Speed);

        Forces[index] += (avoidance - Velocities[index]).limit(MaxForce);
      }
    }
  }
}
