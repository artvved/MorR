using DefaultNamespace;
using Game.Component;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;


namespace Game.System
{
    public class UpdateBulletUISystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        private readonly EcsCustomInject<SceneData> sceneData = default;
        private readonly EcsPoolInject<Cannon> poolCannon = default;


        private EcsFilter eventFilter;
        private EcsFilter playerFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            playerFilter = world.Filter<Cannon>().Inc<Ally>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var player in playerFilter)
            {
                var bulletCount = poolCannon.Value.Get(player).BulletCount;
                sceneData.Value.counterView.SetText(bulletCount);
            }
        }
    }
}