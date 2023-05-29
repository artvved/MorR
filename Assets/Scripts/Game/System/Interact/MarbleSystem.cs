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
    public class MarbleSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsPoolInject<BaseViewComponent> poolView = default;
        private readonly EcsPoolInject<Marble> poolMarble = default;
        private readonly EcsPoolInject<MarbleFinish> poolFinish = default;
        
        private readonly EcsPoolInject<OnCollisionEnterEvent> poolColEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<ShotEvent> poolShotEvent = Idents.EVENT_WORLD;

        private readonly EcsCustomInject<SceneData> data = default;
        private readonly EcsCustomInject<StaticData> staticData = default;
        private readonly EcsCustomInject<Fabric> fabric = default;

        private EcsFilter filterCollision;
  


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            var marble = fabric.Value.InstantiateMarble(data.Value.PlayerRandomizer.MarbleSpawn.position,true);

            foreach (var finish in data.Value.PlayerRandomizer.Finishes)
            {
                fabric.Value.InitMarbleFinish(finish);
            }

            filterCollision = eventWorld.Filter<OnCollisionEnterEvent>().End();

        }

        public void Run(IEcsSystems systems)
        {
            foreach (var e in filterCollision)
            {
                var enterEvent = poolColEvent.Value.Get(e);
                var colliderGameObject = enterEvent.collider.gameObject;
                var senderGameObject = enterEvent.senderGameObject;
                var velocity = enterEvent.relativeVelocity;
                var contactPoint = enterEvent.firstContactPoint;

                if (senderGameObject.CompareTag("Marble"))
                {
                    var marble = senderGameObject.GetComponent<BaseView>().Entity;
                    var view = (MarbleView)poolView.Value.Get(marble).Value;
                    var spawn = poolMarble.Value.Get(marble).Spawn;

                    if (colliderGameObject.CompareTag("Obstacle") || colliderGameObject.CompareTag("Marble"))
                    {
                      
                        view.Bounce(GetBounce(velocity,contactPoint));
                    }else
                    if (colliderGameObject.CompareTag("Finish"))
                    {
                        view.transform.position = spawn;
                        view.Bounce(GetBounce(velocity,contactPoint)); //mb needless 
                        var finish = colliderGameObject.GetComponent<BaseView>().Entity;
                        var finishType = poolFinish.Value.Get(finish).Type;

                        switch (finishType)
                        {
                            case MarbleFinishType.R:
                                poolShotEvent.NewEntity(out int entity1);
                                break;
                            
                            case MarbleFinishType.X2:
                                poolShotEvent.NewEntity(out int entity2);
                                break;
                        }


                       
                    }
                }
                
               
                
            }
        }

      
        private Vector3 GetBounce(Vector3 velocity, ContactPoint contactPoint)
        {
            return -staticData.Value.MarbleBounce * (Vector3.Reflect(velocity.normalized,contactPoint.normal)
                                                     +new Vector3(Random.Range(-0.1f,0.1f),0,0));
        }

       


       
    }
}