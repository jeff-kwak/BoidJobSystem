using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct ZeroForcesJob : IJobParallelFor
{
  [WriteOnly]
  public NativeArray<float3> Forces;

  public void Execute(int index)
  {
    Forces[index] = float3.zero;
  }
}
