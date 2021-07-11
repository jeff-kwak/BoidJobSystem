using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct RandomizeVelocitiesJob : IJob
{
  public Random Rng;
  public float BoidSpeed;

  [WriteOnly]
  public NativeArray<float3> Velocities;

  public void Execute()
  {
    for (int i = 0; i < Velocities.Length; i++)
    {
      // The temporary vector is to support 2D
      var d = Rng.NextFloat2Direction();
      Velocities[i] = new float3(d.x, d.y, 0) * BoidSpeed;
    }
  }
}
