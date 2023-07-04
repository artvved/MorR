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
    public class HitSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsPoolInject<HitEvent> poolHitEvent = Idents.EVENT_WORLD;

        private readonly EcsPoolInject<BaseViewComponent> poolView = default;
        private readonly EcsPoolInject<Ally> poolAlly = default;
        private readonly EcsPoolInject<Enemy> poolEnemy = default;

        private readonly EcsCustomInject<StaticData> staticData = default;


        private EcsFilter filterHitEvent;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            
           
            filterHitEvent = eventWorld.Filter<HitEvent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filterHitEvent)
            {
                var target = poolHitEvent.Value.Get(entity).Target;
                var boxView = (BoxView)poolView.Value.Get(target).Value;

                bool isPlayer;
                if (poolAlly.Value.Has(target))
                {
                    isPlayer = false;
                    poolAlly.Value.Del(target);
                    poolEnemy.Value.Add(target);
                }
                else
                {
                    isPlayer = true;
                    poolAlly.Value.Add(target);
                    poolEnemy.Value.Del(target);
                }
                
                boxView.SetSide(isPlayer);
               
            }
            
           
        }

        
    }
}