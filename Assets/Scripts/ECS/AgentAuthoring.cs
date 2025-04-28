using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class AgentAuthoring : MonoBehaviour
{    
    private class Baker : Baker<AgentAuthoring>
    {
        public override void Bake(AgentAuthoring authoring)
        {
            // GetEntity returns the Entity baked from the GameObject
            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
        }
    }
}

public struct AgentMoveData : IComponentData
{
    public Entity WanderTarget;
    public float3 WanderTargetPosition;
    public float ChangeDirectionTimer;
    public float3 CurrentDirection;
}
