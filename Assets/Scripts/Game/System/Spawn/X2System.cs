using DefaultNamespace;
using Game.Component;
using Game.Component.Time;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class X2System : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsPoolInject<X2Event> poolX2 = Idents.EVENT_WORLD;
  
        private readonly EcsPoolInject<Ally> poolAlly = default;
        private readonly EcsPoolInject<Cannon> poolCannon = default;

        private EcsFilter filterEvent;
        private EcsFilter filterCannon;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);

            filterEvent = eventWorld.Filter<X2Event>().End();
            filterCannon = world.Filter<Cannon>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filterEvent)
            {
                var isAlly = poolX2.Value.Get(entity).IsAlly;
                foreach (var cannon in filterCannon)
                {
                    if ((IsAlly(cannon) && isAlly) || (!IsAlly(cannon) && !isAlly))
                    {
                        poolCannon.Value.Get(cannon).BulletCount *= 2;
                    }
                }
            }
        }

        private bool IsAlly(int entity)
        {
            return poolAlly.Value.Has(entity);
        }
    }
}