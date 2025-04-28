using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

partial struct AgentsSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This RequireForUpdate means the system only updates if at least one entity with the WanderingConfig component exists.
        // Effectively, this system will not update until the subscene with the WanderingConfig has been loaded.
        state.RequireForUpdate<WanderingConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //We just need one frame to spawn the wander targets and agents
        state.Enabled = false;

        var wanderConfig = SystemAPI.GetSingleton<WanderingConfig>();
        float density = wanderConfig.SpawnDensity;
        float spawnRangeX = wanderConfig.SpawnRange.x * density;
        float spawnRangeY = wanderConfig.SpawnRange.y * density;

        for (int i = 0; i < spawnRangeX * spawnRangeY; i++)
        {
            var spawnedWanderTarget = state.EntityManager.Instantiate(wanderConfig.WanderTargetPrefab);
            //Generate wander targets based on grid calculation
            float3 position = new float3(
                ((i % spawnRangeX) - (spawnRangeX / 2)) / density,
                0f,
                ((i / (int)spawnRangeX) - (spawnRangeY / 2)) / density);
            state.EntityManager.SetComponentData(spawnedWanderTarget, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 0.2f
            });
            state.EntityManager.AddComponentData(spawnedWanderTarget, new WanderTargetData
            {
                OriginalPosition = position
            });

            //Spawn an agent for every wander target
            var spawnedAgent = state.EntityManager.Instantiate(wanderConfig.AgentPrefab);

            //Set a random point around the wander target to wander
            var random = new Random((uint)UnityEngine.Random.Range(1, int.MaxValue));
            position += math.normalize(random.NextFloat3Direction());
            position.y = 0;
            state.EntityManager.SetComponentData(spawnedAgent, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 1
            });
            //Cache wander target entity for the spawned agent
            state.EntityManager.AddComponentData(spawnedAgent, new AgentMoveData
            {
                WanderTarget = spawnedWanderTarget,
            });
        }
    }
}
