using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct FindNeighborsJob : IJob
{
  public float Radius;

  [ReadOnly]
  public NativeArray<float3> Positions;

  [WriteOnly]
  public NativeHashMap<int, int4x4> Neighbors;

  public void Execute()
  {
    var localNeighbors = new int4x4(-1);
    var count = 0;
    var radiusSqrd = Radius * Radius;

    for (int index = 0; index < Positions.Length; index++)
    {
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
      count = 0;
    }
  }
}
