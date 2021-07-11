using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct ApplySteeringForcesJob : IJobParallelFor
{
  public float DeltaTime;

  [ReadOnly]
  public NativeArray<float3> Forces;

  public NativeArray<float3> Velocities;

  public void Execute(int index)
  {
    Velocities[index] += Forces[index] * DeltaTime;
  }
}
