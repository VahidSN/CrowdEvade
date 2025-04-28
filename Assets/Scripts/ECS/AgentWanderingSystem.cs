using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(AgentsSpawnerSystem))]
partial struct AgentWanderingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var wanderConfig = SystemAPI.GetSingleton<WanderingConfig>();
        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true); // readonly

        //Job 1: Cache the wander target positions from Wander Target entity
        var cacheWanderTargetPositionJob = new CachWanderTargetPositionJob
        {
            TransformLookup = transformLookup
        };
        JobHandle cacheWanderTargetPositionHandle = cacheWanderTargetPositionJob.ScheduleParallel(state.Dependency);

        //Job 2: Move agents using the cached wander target positions
        var agentWanderJob = new AgentWanderJob
        {
            DeltaTime = deltaTime,
            Config = wanderConfig,
            RandomSeed = (uint)UnityEngine.Random.Range(1, int.MaxValue)
        };

        //Run Job 2 after finishing Job 1 to avoid race condition
        JobHandle agentWanderJobHadler = agentWanderJob.ScheduleParallel(cacheWanderTargetPositionHandle);
        state.Dependency = agentWanderJobHadler;
    }

    [BurstCompile]
    public partial struct CachWanderTargetPositionJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;

        public void Execute(ref AgentMoveData moveData)
        {
            if (TransformLookup.HasComponent(moveData.WanderTarget))
            {
                moveData.WanderTargetPosition = TransformLookup[moveData.WanderTarget].Position;
            }
        }
    }

    [BurstCompile]
    public partial struct AgentWanderJob : IJobEntity
    {
        public float DeltaTime;
        public WanderingConfig Config;
        public uint RandomSeed;

        public void Execute(ref LocalTransform transform, ref AgentMoveData moveData)
        {
            float3 centerPosition = moveData.WanderTargetPosition;

            //If the target is too far, go towards it directly
            if (math.lengthsq(transform.Position - centerPosition) > Config.WanderOffset * Config.WanderOffset)
            {
                float3 centerDirection = math.normalizesafe(centerPosition - transform.Position);
                transform.Position += centerDirection * DeltaTime * Config.AgentMoveSpeed * 2;
            }
            else
            {
                //Wandering around the Wander Target position
                moveData.ChangeDirectionTimer -= DeltaTime;
                if (moveData.ChangeDirectionTimer > 0)
                {
                    transform.Position += moveData.CurrentDirection * Config.AgentMoveSpeed * DeltaTime;
                    quaternion targetRotation = quaternion.LookRotationSafe(moveData.CurrentDirection, math.up());
                    transform.Rotation = math.slerp(transform.Rotation, targetRotation, DeltaTime * Config.AgentRotateSpeed);
                }

                if (moveData.ChangeDirectionTimer <= 0f)
                {
                    //Select a random position around the Wander Target
                    var random = new Unity.Mathematics.Random(RandomSeed + (uint)math.hash(transform.Position));
                    float3 centerDirection = math.normalizesafe(centerPosition - transform.Position);
                    float3 randomOffset = math.normalize(random.NextFloat3Direction()) * Config.WanderOffset;
                    randomOffset.y = 0;
                    float3 newDir = math.normalizesafe(centerDirection + randomOffset);

                    moveData.CurrentDirection = newDir;
                    float2 interval = Config.FollowTimeInterval;
                    moveData.ChangeDirectionTimer = random.NextFloat(interval.x, interval.y);
                }
            }
        }
    }
}
