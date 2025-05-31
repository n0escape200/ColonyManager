using Unity.Entities;
using UnityEngine;


//the value for the speed at witch the unit moves
public class MoveSpeedAuthoring : MonoBehaviour
{

    public float value;

    public class Baker : Baker<MoveSpeedAuthoring>
    {
        public override void Bake(MoveSpeedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveSpeed
            {
                value = authoring.value,
            });
        }
    }


}
public struct MoveSpeed : IComponentData
{
    public float value;

}
