using UnityEngine;
using Unity.Entities;

public class PlayerAuthoring : MonoBehaviour
{    
    private class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            // GetEntity returns the Entity baked from the GameObject
            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerTag());
        }
    }
}

public struct PlayerTag : IComponentData { }

