using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class MoveApplySystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        
        readonly EcsPoolInject<BaseViewComponent> transformPool=default;
       
        readonly EcsPoolInject<CantMoveComponent> cantMovePool = default;
        readonly EcsPoolInject<Speed> speedPool = default;
        readonly EcsPoolInject<Direction> directionPool = default;

        private EcsFilter unitTransformFilter;
       

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            unitTransformFilter = world.Filter<Speed>()
                .Inc<Direction>()
                .Inc<BaseViewComponent>()
                .Exc<ReachedTargetComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in unitTransformFilter)
            {
                if (cantMovePool.Value.Has(entity))
                    continue;
                var speed = speedPool.Value.Get(entity).Value;
                var direction = directionPool.Value.Get(entity).Value;
                var transform = transformPool.Value.Get(entity).Value.transform;
                
                var delta =  speed * direction;
                transform.position=Vector3.Lerp(transform.position, transform.position + delta, Time.deltaTime);
                //transform.position += delta * Time.deltaTime;
                
                
            }
        }
    }
}