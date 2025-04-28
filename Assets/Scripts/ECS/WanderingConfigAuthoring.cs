using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class WanderingConfigAuthoring : MonoBehaviour
{
    [Header("Spawn Setup")]
    public GameObject wanderTargetPrefab;
    public GameObject agentPrefab;
    public Vector2 spawnRange;
    public float spawnDensity;

    [Header("Wander Target Setup")]
    public float wanderTargetMoveSpeed;
    public float playerAvoidDistance;

    [Header("Agent Setup")]
    public float agentMoveSpeed;
    public float agentRotateSpeed;
    public float wanderOffset;
    public float2 followTimeInterval;

    private class Baker : Baker<WanderingConfigAuthoring>
    {
        public override void Bake(WanderingConfigAuthoring authoring)
        {
            // GetEntity returns the Entity baked from the GameObject
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new WanderingConfig
            {
                //Set TransformUsageFlags to Dynamic because the transform is updated during runtime
                WanderTargetPrefab = GetEntity(authoring.wanderTargetPrefab, TransformUsageFlags.Dynamic),
                AgentPrefab = GetEntity(authoring.agentPrefab, TransformUsageFlags.Dynamic),
                SpawnRange = authoring.spawnRange,
                SpawnDensity = authoring.spawnDensity,

                AgentMoveSpeed = authoring.agentMoveSpeed,
                AgentRotateSpeed = authoring.agentRotateSpeed,
                WanderOffset = authoring.wanderOffset,
                FollowTimeInterval = authoring.followTimeInterval,

                WanderTargetMoveSpeed = authoring.wanderTargetMoveSpeed,
                PlayerAvoidDistance = authoring.playerAvoidDistance,
            });
        }
    }
}

public struct WanderingConfig : IComponentData
{
    public Entity WanderTargetPrefab;
    public Entity AgentPrefab;
    public float2 SpawnRange;
    public float SpawnDensity;

    public float AgentMoveSpeed;
    public float AgentRotateSpeed;
    public float WanderOffset;
    public float2 FollowTimeInterval;

    public float WanderTargetMoveSpeed;
    public float PlayerAvoidDistance;
}
