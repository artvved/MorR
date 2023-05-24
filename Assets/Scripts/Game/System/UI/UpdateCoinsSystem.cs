using DefaultNamespace;
using Game.Component;

using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;



namespace Game.System
{
    public class UpdateCoinsSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        private readonly EcsCustomInject<SceneData> sceneData = default;
        private readonly EcsPoolInject<UnitStatsComponent> unitPool = default;


        private EcsFilter eventFilter;
        private EcsFilter playerFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            eventFilter = eventWorld.Filter<CoinsChangedEventComponent>().End();
            playerFilter = world.Filter<Cannon>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in eventFilter)
            {
                foreach (var player in playerFilter)
                {
                    ref var coins = ref unitPool.Value.Get(player).Coins;
                    coins++;
                    sceneData.Value.CoinsView.TextMeshProUGUI.text = coins.ToString();
                }
            }
        }
    }
}