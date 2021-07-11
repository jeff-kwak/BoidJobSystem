using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct BoidAlignmentJob : IJobParallelFor
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
    var localNeighbors = Neighbors[index];
    if (localNeighbors.Count() > 0)
    {
      var averageVelocity = float3.zero;
      for (int i = 0; i < localNeighbors.Count(); i++)
      {
        averageVelocity += Velocities[localNeighbors.At(i)];
      }

      averageVelocity /= localNeighbors.Count();
      var targetVelocity = averageVelocity.magnitude(Speed);
      Forces[index] += (targetVelocity - Velocities[index]).limit(MaxForce);
    }
  }
}
