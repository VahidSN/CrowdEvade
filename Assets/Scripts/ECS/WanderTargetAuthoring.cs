using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class WanderTargetAuthoring : MonoBehaviour
{
    private class Baker : Baker<WanderTargetAuthoring>
    {
        public override void Bake(WanderTargetAuthoring authoring)
        {
            // GetEntity returns the Entity baked from the GameObject
            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
        }
    }
}

public struct WanderTargetData : IComponentData
{
    public float3 OriginalPosition;
}