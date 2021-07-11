using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct UpdatePositionJob : IJobParallelFor
{
  public float DeltaTime;
  
  [ReadOnly]
  public NativeArray<float3> Velocities;
  
  public NativeArray<float3> Positions;
  
  public void Execute(int index)
  {
    Positions[index] += Velocities[index] * DeltaTime;
  }
}
