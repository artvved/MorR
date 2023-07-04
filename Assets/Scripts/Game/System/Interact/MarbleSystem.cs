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
        private readonly EcsPoolInject<OnCollisionStayEvent> poolColStayEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<ReleaseEvent> poolShotEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<X2Event> poolX2Event = Idents.EVENT_WORLD;

        private EcsPoolInject<Ally> poolAlly = default;
        private EcsPoolInject<Enemy> poolEnemy = default;


        private EcsFilter filterCollisionEnter;
        private EcsFilter filterCollisionStay;
        
        private readonly EcsCustomInject<StaticData> staticData = default;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);

            filterCollisionEnter = eventWorld.Filter<OnCollisionEnterEvent>().End();
            filterCollisionStay = eventWorld.Filter<OnCollisionStayEvent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var e in filterCollisionEnter)
            {
                var enterEvent = poolColEvent.Value.Get(e);
                var colliderGameObject = enterEvent.collider.gameObject;
                var senderGameObject = enterEvent.senderGameObject;
                var velocity = enterEvent.relativeVelocity;
                var contactPoint = enterEvent.firstContactPoint;

                if (senderGameObject.CompareTag("Marble"))
                {
                    var marble = senderGameObject.GetComponent<BaseView>().Entity;
                    var view = (MarbleView) poolView.Value.Get(marble).Value;
                    

                    if (colliderGameObject.CompareTag("Obstacle") || colliderGameObject.CompareTag("Marble"))
                    {
                        view.Bounce(GetBounce(velocity, contactPoint));
                    }
                    else if (colliderGameObject.CompareTag("Finish"))
                    {
                        var finish = colliderGameObject.GetComponent<MarbleFinishView>();
                        if (!finish.Active)
                            continue;
                        Finish(marble,finish,velocity,contactPoint);
                    }
                }
            }

            foreach (var e in filterCollisionStay)
            {
                var enterEvent = poolColStayEvent.Value.Get(e);
                var senderGameObject = enterEvent.senderGameObject;
                var colliderGameObject = enterEvent.collider.gameObject;
                var velocity = enterEvent.relativeVelocity;
                var contactPoint = enterEvent.firstContactPoint;
                
                if (senderGameObject.CompareTag("Marble"))
                {
                    if (colliderGameObject.CompareTag("Finish"))
                    { 
                        var marble = senderGameObject.GetComponent<BaseView>().Entity;
                        var finish = colliderGameObject.GetComponent<MarbleFinishView>();
                        if (!finish.Active)
                            continue;
                        Finish(marble,finish,velocity,contactPoint);
                    }
                }


            }
        }

        private void Finish(int marble,MarbleFinishView finish,Vector3 velocity, ContactPoint contactPoint)
        {
            
            var view = (MarbleView) poolView.Value.Get(marble).Value;
            var spawn = poolMarble.Value.Get(marble).Spawn;
            var finishType = finish.Type;
                        
            bool isAlly = poolAlly.Value.Has(marble);
                        
            view.transform.position = spawn;
            view.Bounce(GetBounce(velocity, contactPoint)); //mb needless 
                        

            switch (finishType)
            {
                case MarbleFinishType.R:
                    poolShotEvent.NewEntity(out int entity1).IsAlly=isAlly;
                    break;

                case MarbleFinishType.X2:
                    poolX2Event.NewEntity(out int entity2).IsAlly=isAlly;
                    break;
            }
        }


        private Vector3 GetBounce(Vector3 velocity, ContactPoint contactPoint)
        {
            return -staticData.Value.MarbleBounce * (Vector3.Reflect(velocity.normalized, contactPoint.normal)
                                                     + new Vector3(Random.Range(-0.1f, 0.1f), 0, 0));
        }
    }
}