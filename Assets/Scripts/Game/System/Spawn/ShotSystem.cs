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
    public class ShotSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private EcsCustomInject<Fabric> fabric = default;
        private EcsCustomInject<StaticData> data = default;
        private readonly EcsPoolInject<BaseViewComponent> poolView = default;

        private readonly EcsPoolInject<ReleaseEvent> poolRelease = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<ToggleInputEvent> poolToggleInput = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<Direction> poolDirection = default;
        private readonly EcsPoolInject<Ally> poolAlly = default;
        private readonly EcsPoolInject<Tick> poolTick = default;
        private readonly EcsPoolInject<Cannon> poolCannon = default;

        private EcsFilter filterEvent;
        private EcsFilter filterCannon;
        private EcsFilter filterCannonTick;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);

            filterEvent = eventWorld.Filter<ReleaseEvent>().End();
            filterCannon = world.Filter<Cannon>().Exc<Tick>().End();
            filterCannonTick = world.Filter<Cannon>().Inc<Tick>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filterEvent)
            {
                var isAlly = poolRelease.Value.Get(entity).IsAlly;
                foreach (var cannon in filterCannon)
                {
                    if ((IsAlly(cannon) && isAlly) || (!IsAlly(cannon) && !isAlly))
                    {
                        ref var toggleInputEvent = ref poolToggleInput.NewEntity(out int e);
                        toggleInputEvent.Block = true;
                        toggleInputEvent.Cannon = cannon;
                        poolTick.Value.Add(cannon).FinalTime = data.Value.ShotTimeDelta;
                    }
                }
            }

            foreach (var cannon in filterCannonTick)
            {
                ref var tick = ref poolTick.Value.Get(cannon);
                if (tick.CurrentTime >= tick.FinalTime)
                {
                    var view = (CannonView) poolView.Value.Get(cannon).Value;
                    var direction = poolDirection.Value.Get(cannon).Value;
                    fabric.Value.InstantiateBullet(view.BulletSpawnPlace.position,
                        view.transform.localToWorldMatrix.MultiplyVector(direction),
                        IsAlly(cannon));
                    tick.CurrentTime = 0;

                    ref var cannonComp = ref poolCannon.Value.Get(cannon);
                    cannonComp.BulletCount--;
                    if (cannonComp.BulletCount <= 0)
                    {
                        ref var toggleInputEvent = ref poolToggleInput.NewEntity(out int e);
                        toggleInputEvent.Block = false;
                        toggleInputEvent.Cannon = cannon;
                        cannonComp.BulletCount = 1;
                        poolTick.Value.Del(cannon); // раннее удаление - тик внизу
                    }
                }

                tick.CurrentTime += Time.deltaTime;
            }
        }

        private bool IsAlly(int entity)
        {
            return poolAlly.Value.Has(entity);
        }
    }
}