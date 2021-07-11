using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct RandomizePositionsJob : IJob
{
  public Random Rng;
  public float3 LeftBottomFront;
  public float3 RightTopBack;

  [WriteOnly]
  public NativeArray<float3> Positions;

  public void Execute()
  {
    for (int i = 0; i < Positions.Length; i++)
    {
      Positions[i] = Rng.NextFloat3(LeftBottomFront, RightTopBack);
    }
  }
}
