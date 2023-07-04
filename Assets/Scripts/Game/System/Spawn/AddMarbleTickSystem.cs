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
    public class AddMarbleTickSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private EcsCustomInject<Fabric> fabric = default;
        private EcsCustomInject<StaticData> data = default;
        private readonly EcsPoolInject<BaseViewComponent> poolView = default;

        private readonly EcsPoolInject<ReleaseEvent> poolRelease = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<RandomizerComponent> poolRandomizer = default;
        private readonly EcsPoolInject<Direction> poolDirection = default;
        private readonly EcsPoolInject<Ally> poolAlly = default;
        private readonly EcsPoolInject<Tick> poolTick = default;
        private readonly EcsPoolInject<Cannon> poolCannon = default;

        private EcsFilter filterEvent;
        private EcsFilter filterCannon;


        private float tick;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);

            filterEvent = eventWorld.Filter<ReleaseEvent>().End();
            filterCannon = world.Filter<Cannon>().End();
        }

        public void Run(IEcsSystems systems)
        {
            if (tick >= data.Value.AddMarbleTime)
            {
                foreach (var cannon in filterCannon)
                {
                    var randomizerView = poolRandomizer.Value.Get(cannon).Value;
                    fabric.Value.InstantiateMarble(randomizerView.MarbleSpawn.position, IsAlly(cannon));
                }

                tick = 0;
            }

            tick += Time.deltaTime;
        }

        private bool IsAlly(int entity)
        {
            return poolAlly.Value.Has(entity);
        }
    }
}