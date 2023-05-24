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
    public class RiseFallViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsPoolInject<RiseEvent> poolRisingEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<FallEvent> poolFallEvent = Idents.EVENT_WORLD;


        private readonly EcsPoolInject<BaseViewComponent> poolView = default;
        
        private EcsFilter filterRisingBoxEvent;
        private EcsFilter filterFallBoxEvent;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            
            filterRisingBoxEvent = eventWorld.Filter<RiseEvent>().End();
            filterFallBoxEvent = eventWorld.Filter<FallEvent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filterRisingBoxEvent)
            {
                var target = poolRisingEvent.Value.Get(entity).Target;
                var boxView = (BoxView)poolView.Value.Get(target).Value;
                boxView.Rise();
               
            }
            
            foreach (var entity in filterFallBoxEvent)
            {
                var target = poolFallEvent.Value.Get(entity).Target;
                var boxView = (BoxView)poolView.Value.Get(target).Value;
                boxView.Fall();
               
            }
        }

        
    }
}