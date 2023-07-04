using DefaultNamespace;
using Game.Component;
using Game.Mono;
using Game.Service;
using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using Unity.VisualScripting;
using UnityEngine;


namespace Game.System
{
    public class ToggleInputSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        
        private readonly EcsPoolInject<RandomizerComponent> poolRandomizer = default;
        private readonly EcsPoolInject<ToggleInputEvent> poolToggleEvent = Idents.EVENT_WORLD;

        
        private EcsFilter filterToggle;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);

            filterToggle = eventWorld.Filter<ToggleInputEvent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var e in filterToggle)
            {
                var inputEvent = poolToggleEvent.Value.Get(e);
                var randomizerView= poolRandomizer.Value.Get(inputEvent.Cannon).Value;
                randomizerView.ToggleInput(inputEvent.Block);
            }
        }


      
    }
}