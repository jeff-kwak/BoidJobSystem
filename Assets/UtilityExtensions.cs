using System.Collections.Generic;
using Unity.Mathematics;

public static class UtilityExtensions
{
  public static IEnumerable<int> Times(this int count)
  {
    for (int i = 0; i < count; i++)
    {
      yield return i;
    }
  }

  public static int4x4 WithValueAt(this int4x4 register, int index, int value)
  {
    var row = register[index / 4];
    switch(index % 4)
    {
      case 0:
        row.x = value;
        break;
      case 1:
        row.y = value;
        break;
      case 2:
        row.z = value;
        break;
      case 3:
        row.w = value;
        break;
    }
    register[index / 4] = row;
    return register;
  }

  public static int At(this int4x4 register, int index)
  {
    var row = register[index / 4];
    switch(index % 4)
    {
      case 0:
        return row.x;
      case 1:
        return row.y;
      case 2:
        return row.z;
      case 3:
        return row.w;
    }
    return -1;
  }

  public static int Count(this int4x4 register)
  {
    var empty = 0;
    for (int i = 0; i < 4; i++)
    {
      if (register[i].x == -1) empty++;
      if (register[i].y == -1) empty++;
      if (register[i].z == -1) empty++;
      if (register[i].w == -1) empty++;
    }
    return 16 - empty;
  }

  public static float3 magnitude(this float3 vector, float scalar) => math.normalizesafe(vector) * scalar;

  public static float3 limit(this float3 vector, float scalar) => math.lengthsq(vector) <= scalar * scalar ?
      vector :
      vector.magnitude(scalar);
}
