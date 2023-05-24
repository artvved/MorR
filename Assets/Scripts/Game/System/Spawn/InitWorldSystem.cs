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
        private readonly EcsCustomInject<SceneData> sceneData = default;
        private EcsPool<BaseViewComponent> playerTransformPool;
        

        public void Init(IEcsSystems systems)
        {
            playerTransformPool = systems.GetWorld().GetPool<BaseViewComponent>();
            
            var plEntity=fabric.Value.InstantiatePlayer();
            var playerView = (CannonView)playerTransformPool.Get(plEntity).Value;
           
            fabric.Value.InstantiateField();
            
        }


       
    }
}