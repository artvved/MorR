using DefaultNamespace;
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
    public class ShotSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private EcsCustomInject<Fabric> fabric = default;
        readonly EcsPoolInject<BaseViewComponent> poolView = default;
        
        readonly EcsPoolInject<Direction> poolDirection = default;

        private EcsFilter filterEvent;
        private EcsFilter filterCannon;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);

            filterEvent = eventWorld.Filter<ShotEvent>().End();
            filterCannon = world.Filter<Cannon>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filterEvent)
            {
                foreach (var cannon in filterCannon)
                {
                    var view=(CannonView)poolView.Value.Get(cannon).Value;
                    var direction = poolDirection.Value.Get(cannon).Value;
                    fabric.Value.InstantiatePlayerBullet(view.BulletSpawnPlace.position, direction);
                }
            }
        }
    }
}