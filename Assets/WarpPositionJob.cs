using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct WarpPositionJob : IJobParallelFor
{
  public float3 LeftBottomFront;
  public float3 RightTopBack;
  public NativeArray<float3> Positions;

  public void Execute(int index)
  {
    var pos = Positions[index];
    if (pos.x > RightTopBack.x) pos.x = LeftBottomFront.x;
    if (pos.x < LeftBottomFront.x) pos.x = RightTopBack.x;
    if (pos.y > RightTopBack.y) pos.y = LeftBottomFront.y;
    if (pos.y < LeftBottomFront.y) pos.y = RightTopBack.y;
    Positions[index] = pos;
  }
}
