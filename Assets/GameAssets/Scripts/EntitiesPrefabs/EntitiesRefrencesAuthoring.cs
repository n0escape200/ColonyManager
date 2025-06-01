using Unity.Entities;
using UnityEngine;

public class EntitiesRefrencesAuthoring : MonoBehaviour
{

    public GameObject TreePrefabGameObject;
    public GameObject StonePrefabGameObject;
    public GameObject IronPrefabGameObject;
    public GameObject UnitPrefabGameObject;


    public class Baker : Baker<EntitiesRefrencesAuthoring>
    {
        public override void Bake(EntitiesRefrencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesRefrences
            {
                TreePrefabEntity = GetEntity(authoring.TreePrefabGameObject, TransformUsageFlags.Dynamic),
                StonePrefabEntity = GetEntity(authoring.StonePrefabGameObject, TransformUsageFlags.Dynamic),
                IronPrefabEntity = GetEntity(authoring.IronPrefabGameObject, TransformUsageFlags.Dynamic),
                UnitPrefabEntity = GetEntity(authoring.UnitPrefabGameObject, TransformUsageFlags.Dynamic)
            });
        }
    }
}





public struct EntitiesRefrences : IComponentData
{
    public Entity TreePrefabEntity;
    public Entity StonePrefabEntity;
    public Entity IronPrefabEntity;
    public Entity UnitPrefabEntity;
}


