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
    public class CanonDirectionRotationSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        
        private readonly EcsPoolInject<Direction> directionPool = default;
        private readonly EcsPoolInject<Cannon> poolCannon = default;

        private EcsFilter unitTransformFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            unitTransformFilter = world.Filter<Direction>()
                .Inc<BaseViewComponent>()
                .Inc<Cannon>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in unitTransformFilter)
            {
                ref var isClockwise = ref poolCannon.Value.Get(entity).IsClockwise;
                ref var direction = ref directionPool.Value.Get(entity).Value;
                float angle = 60*Mathf.Deg2Rad;
                
                if ((direction.x>0 && direction.z<=Mathf.Sin(angle)) || (direction.x<0 && direction.z<=Mathf.Sin(angle)))
                {
                    isClockwise = !isClockwise;
                }
            }
        }
    }
}