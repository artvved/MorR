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
    public class CanonRotationSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private readonly EcsCustomInject<StaticData> staticData = default;
        private readonly EcsPoolInject<BaseViewComponent> transformPool = default;
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
                var isClockwise = poolCannon.Value.Get(entity).IsClockwise;
                ref var direction = ref directionPool.Value.Get(entity).Value;
                var view = (CannonView)transformPool.Value.Get(entity).Value;
                var rotationSpeed = staticData.Value.CannonRotationSpeed;
                if (!isClockwise)
                {
                    rotationSpeed *= -1;
                }

                view.Body.rotation=Quaternion.Euler(view.Body.rotation.eulerAngles+new Vector3(0, rotationSpeed*Time.deltaTime,0));
                direction = view.transform.worldToLocalMatrix.MultiplyVector(view.Body.forward);

            }
        }
    }
}