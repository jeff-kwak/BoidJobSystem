using UnityEngine.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public struct UpdateTransformJob : IJobParallelForTransform
{
  [ReadOnly]
  public NativeArray<float3> Positions;

  public void Execute(int index, TransformAccess transform)
  {
    transform.position = Positions[index];
  }
}
