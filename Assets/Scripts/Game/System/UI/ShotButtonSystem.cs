using DefaultNamespace;
using Game.Component;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using ScriptableData;
using UnityEngine;
using UnityEngine.Scripting;


namespace Game.System
{
    public class ShotButtonSystem : EcsUguiCallbackSystem,IEcsInitSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsPoolInject<ShotEvent> poolShotEvent = Idents.EVENT_WORLD;
        
        private EcsFilter playerFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
           
            playerFilter = world.Filter<Cannon>().Exc<CantMoveComponent>().End();
        }

          
        [Preserve]
        [EcsUguiClickEvent(Idents.Ui.Shot, Idents.EVENT_WORLD)]
        void OnDragStart (in EcsUguiClickEvent e)
        {
            poolShotEvent.NewEntity(out int ent);
        }
        
      

      

    }
}