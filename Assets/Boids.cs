using System.Linq;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using Unity.Collections;
using Unity.Jobs;

public class Boids : MonoBehaviour
{

  [Header("Boids")]
  public GameObject BoidPrefab;
  public int FlockSize;
  public float BoidSpeed = 3f;
  public float PerceptionRadius = 2f;
  public float MaxCohesionForce = 1f;
  public float MaxAvoidanceForce = 1f;
  public float MaxAlignmentForce = 1f;

  [Header("Boundaries")]
  public float3 LeftBottomFront;
  public float3 RightTopBack;

  private const int JobBatchSize = 64;
  private Random Rng;
  private NativeArray<float3> Positions;
  private NativeArray<float3> Velocities;
  private NativeArray<float3> Forces;
  private NativeHashMap<int, int4x4> Neighbors;
  private TransformAccessArray Transforms;

  private BoidAlignmentJob Alignment() => new BoidAlignmentJob { Forces = Forces, MaxForce = MaxAlignmentForce, Neighbors = Neighbors, Positions = Positions, Speed = BoidSpeed, Velocities = Velocities };
  private ApplySteeringForcesJob ApplyForces() => new ApplySteeringForcesJob{ DeltaTime = Time.deltaTime, Forces = Forces, Velocities = Velocities};
  private BoidCohesionJob Cohesion() => new BoidCohesionJob { Forces = Forces, MaxForce = MaxCohesionForce, Neighbors = Neighbors, Positions = Positions, Speed = BoidSpeed, Velocities = Velocities };
  private FindNeighborsJob FindNeighbors() => new FindNeighborsJob { Neighbors = Neighbors, Positions = Positions, Radius = PerceptionRadius };
  private BoidAvoidanceJob Separation() => new BoidAvoidanceJob { Forces = Forces, MaxForce = MaxAvoidanceForce, Neighbors = Neighbors, Positions = Positions, Speed = BoidSpeed, Velocities = Velocities };
  private UpdateTransformJob TransformUpdate() => new UpdateTransformJob { Positions = Positions };
  private UpdatePositionJob UpdatePosition(float dt) => new UpdatePositionJob { DeltaTime = dt, Positions = Positions, Velocities = Velocities };
  private WarpPositionJob WarpPosition() => new WarpPositionJob { LeftBottomFront = LeftBottomFront, Positions = Positions, RightTopBack = RightTopBack };
  private ZeroForcesJob ZeroForces() => new ZeroForcesJob { Forces = Forces };

  private void Awake()
  {
    Rng = new Random((uint)System.DateTime.Now.Ticks);
    Positions = new NativeArray<float3>(FlockSize, Allocator.Persistent);
    Velocities = new NativeArray<float3>(FlockSize, Allocator.Persistent);
    Neighbors = new NativeHashMap<int, int4x4>(FlockSize, Allocator.Persistent);
    Forces = new NativeArray<float3>(FlockSize, Allocator.Persistent);
  }

  private void OnDestroy()
  {
    Positions.Dispose();
    Velocities.Dispose();
    Forces.Dispose();
    Neighbors.Dispose();
    Transforms.Dispose();
  }

  private void Start()
  {
    var individuals = FlockSize.Times().Select(CreateIndividualBoid);
    Transforms = new TransformAccessArray(individuals.Select(i => i.GetComponent<Transform>()).ToArray());

    new RandomizePositionsJob
    {
      Rng = Rng,
      LeftBottomFront = LeftBottomFront,
      RightTopBack = RightTopBack,
      Positions = Positions
    }
    .Schedule()
    .Complete();

    new RandomizeVelocitiesJob
    {
      Rng = Rng,
      BoidSpeed = BoidSpeed,
      Velocities = Velocities
    }
    .Schedule()
    .Complete();
  }


  private void Update()
  {
    var zHandle = ZeroForces()
      .Schedule(FlockSize, JobBatchSize);

    var wHandle = WarpPosition()
      .Schedule(FlockSize, JobBatchSize, zHandle);

    var nHandle = FindNeighbors()
      .Schedule(wHandle);

    var cHandle = Cohesion()
      .Schedule(FlockSize, JobBatchSize, nHandle);

    var alHandle = Alignment()
    .Schedule(FlockSize, JobBatchSize, cHandle);

    var avHandle = Separation()
    .Schedule(FlockSize, JobBatchSize, alHandle);

    var afHandle = ApplyForces()
    .Schedule(FlockSize, JobBatchSize, avHandle);

    var uPosHandle = UpdatePosition(Time.deltaTime)
      .Schedule(FlockSize, JobBatchSize, afHandle);

    TransformUpdate()
      .Schedule(Transforms, uPosHandle)
      .Complete();
  }

  private void OnValidate()
  {
    FlockSize = Mathf.Max(1, FlockSize);
    MaxCohesionForce = Mathf.Max(0, MaxCohesionForce);
    MaxAvoidanceForce = Mathf.Max(0, MaxAvoidanceForce);
    MaxAlignmentForce = Mathf.Max(0, MaxAlignmentForce);
  }

  private GameObject CreateIndividualBoid(int index)
  {
    var gObj = Instantiate(BoidPrefab, transform);
    gObj.name = $"{BoidPrefab.name}:{index}";
    return gObj;
  }
}
