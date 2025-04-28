using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct WanderTargetControllerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This RequireForUpdate means the system only updates if at least one entity with the PlayerTag component exists.
        // Effectively, this system will not update until the subscene with the PlayerTag has been loaded.
        state.RequireForUpdate<PlayerTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var player = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerPosition = SystemAPI.GetComponent<LocalTransform>(player).Position;
        var wanderConfig = SystemAPI.GetSingleton<WanderingConfig>();

        var wanderTargetMoveJob = new WanderTargetMoveJob()
        {
            WanderConfig = wanderConfig,
            PlayerPosition = playerPosition,
            DeltaTime = SystemAPI.Time.DeltaTime,
        };
        wanderTargetMoveJob.ScheduleParallel();
    }

    [BurstCompile]
    public partial struct WanderTargetMoveJob : IJobEntity
    {
        public WanderingConfig WanderConfig;
        public float3 PlayerPosition;
        public float DeltaTime;

        public void Execute(ref LocalTransform wanderTargetTrasnform, ref WanderTargetData wanderTargetData)
        {
            //Flee if the player is getting close
            if (math.lengthsq(wanderTargetTrasnform.Position - PlayerPosition) < WanderConfig.PlayerAvoidDistance * WanderConfig.PlayerAvoidDistance)
            {
                float3 fleeDirection = math.normalizesafe(wanderTargetTrasnform.Position - PlayerPosition);
                wanderTargetTrasnform.Position += fleeDirection * DeltaTime * WanderConfig.WanderTargetMoveSpeed;
            }
            //Original position isn't close to the player anymore, so come back to the original position
            else if (math.lengthsq(wanderTargetData.OriginalPosition - wanderTargetTrasnform.Position) > 0.02f)
            {
                float3 followDirection = math.normalizesafe(wanderTargetData.OriginalPosition - wanderTargetTrasnform.Position);
                wanderTargetTrasnform.Position += followDirection * DeltaTime * WanderConfig.WanderTargetMoveSpeed * 2; //Come back with extra speed!
            }
        }
    }
}
