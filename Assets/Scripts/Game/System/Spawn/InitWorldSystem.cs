using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;

namespace Game.System
{
    public class InitWorldSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<Fabric> fabric=default;
        private readonly EcsCustomInject<StaticData> staticData = default;
        private EcsPool<BaseViewComponent> playerTransformPool;
        

        public void Init(IEcsSystems systems)
        {
            playerTransformPool = systems.GetWorld().GetPool<BaseViewComponent>();

            var level = Mathf.Clamp(Startup.PlayerData.Level, 0, staticData.Value.MaxLevel - 1);
            var levelGo = Resources.Load<LevelView>(string.Format(staticData.Value.LevelPath, level + 1));

            fabric.Value.InstantiateLevel(levelGo);
            
            
        }


       
    }
}