using DefaultNamespace;
using Game.Component.Time;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.System.Timing
{
    public class TickSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        
        private readonly EcsPoolInject<Tick> tickPool = default;
       
        private EcsFilter tickFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();

            tickFilter = world.Filter<Tick>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in tickFilter)
            {
                ref var component = ref tickPool.Value.Get(entity);
                
                component.CurrentTime += Time.deltaTime;
            }
        }

       
    }
    
}