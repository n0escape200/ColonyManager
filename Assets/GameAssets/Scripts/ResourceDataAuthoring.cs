using Unity.Entities;
using UnityEngine;

public class ResourceDataAuthoring : MonoBehaviour
{
    public ResourceType resourceType;
    public int resourceAmmount;
    public class Baker : Baker<ResourceDataAuthoring>
    {
        public override void Bake(ResourceDataAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ResourceData
            {
                resourceType = authoring.resourceType,
                resourceAmmount = authoring.resourceAmmount
            });
        }
    }
}

public struct ResourceData : IComponentData
{
    public ResourceType resourceType;
    public int resourceAmmount;
}


