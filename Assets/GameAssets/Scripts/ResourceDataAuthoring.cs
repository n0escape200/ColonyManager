using Unity.Entities;
using UnityEngine;

public enum Resource_Type {
    wood,
    iron,
    stone,
    food
}

public class ResourceDataAuthoring : MonoBehaviour
{
    public Resource_Type resourceType;
    public int resourceAmmount;
    public class Baker : Baker<ResourceDataAuthoring>
    {
        public override void Bake(ResourceDataAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitData
            {
                resourceType = authoring.resourceType,
                resourceAmmount = authoring.resourceAmmount
            });
        }
    }
}

public struct UnitData : IComponentData
{
    public Resource_Type resourceType;
    public int resourceAmmount;
}


