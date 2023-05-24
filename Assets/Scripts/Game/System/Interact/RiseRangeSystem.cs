using DefaultNamespace;
using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class RiseRangeSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        
        private readonly EcsPoolInject<BaseViewComponent> poolView = default;
        
        private readonly EcsPoolInject<Rising> poolRising = default;
        private readonly EcsPoolInject<RiseEvent> poolRisingEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<FallEvent> poolFallEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<HitEvent> poolHitEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<DeadTag> poolDead = default;

        private EcsFilter filterNotRisingEnemyBox;
        private EcsFilter filterRisingEnemyBox;
        private EcsFilter filterAllyBullet;
        
        private EcsFilter filterNotRisingAllyBox;
        private EcsFilter filterRisingAllyBox;
        private EcsFilter filterEnemyBullet;


        private float riseRange = 2;
        private float collisionRange = 0.5f;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);

            filterNotRisingEnemyBox = world.Filter<Box>().Inc<Enemy>().Exc<Rising>().End();
            filterRisingEnemyBox = world.Filter<Box>().Inc<Enemy>().Inc<Rising>().End();
            filterAllyBullet = world.Filter<Bullet>().Inc<Ally>().End();
            
            filterNotRisingAllyBox = world.Filter<Box>().Inc<Ally>().Exc<Rising>().End();
            filterRisingAllyBox = world.Filter<Box>().Inc<Ally>().Inc<Rising>().End();
            filterEnemyBullet = world.Filter<Bullet>().Inc<Enemy>().End();
        }

        public void Run(IEcsSystems systems)
        {
            Handle();
        }

        private void RiseBoxesInRange(EcsFilter filterNotRisingBox,EcsFilter filterBullet)
        {
            foreach (var box in filterNotRisingBox)
            {
                var boxPos = poolView.Value.Get(box).Value.transform.position;
                foreach (var bullet in filterBullet)
                {
                    var bulletPos = poolView.Value.Get(bullet).Value.transform.position;
                    if (IsInRange(bulletPos, boxPos, riseRange))
                    {
                        poolRising.Value.Add(box);
                        poolRisingEvent.NewEntity(out int e).Target = box;
                        break;
                    }
                }
            }
        }

        private void BoxBulletCollision(EcsFilter filterRisingBox,EcsFilter filterBullet)
        {
            //del rise on collision
            foreach (var box in filterRisingBox)
            {
                var boxPos = poolView.Value.Get(box).Value.transform.position;

                foreach (var bullet in filterBullet)
                {
                    var bulletPos = poolView.Value.Get(bullet).Value.transform.position;
                    //collision check
                    if (IsInRange(bulletPos, boxPos, collisionRange))
                    {
                        poolRising.Value.Del(box);
                        poolFallEvent.NewEntity(out int e).Target = box;
                        poolHitEvent.NewEntity(out int hit).Target = box;
                        poolDead.Value.Add(bullet);
                        break;
                    }
                }
            }
        }

        private void CheckForBulletsInRange(EcsFilter filterRisingBox,EcsFilter filterBullet)
        {
            //del rise on no bullets in range
            foreach (var box in filterRisingBox)
            {
                var boxPos = poolView.Value.Get(box).Value.transform.position;
                bool smbInRange = false;
                foreach (var bullet in filterBullet)
                {
                    var bulletPos = poolView.Value.Get(bullet).Value.transform.position;

                    if (IsInRange(bulletPos, boxPos, riseRange))
                    {
                        smbInRange = true;
                        break;
                    }
                }

                if (!smbInRange)
                {
                    poolRising.Value.Del(box);
                    poolFallEvent.NewEntity(out int e).Target = box;
                }
            }
        }

        private void Handle()
        {
            //enemy boxes - player bullets
            RiseBoxesInRange(filterNotRisingEnemyBox,filterAllyBullet);
            BoxBulletCollision(filterRisingEnemyBox,filterAllyBullet);
            CheckForBulletsInRange(filterRisingEnemyBox,filterAllyBullet);
            
            //enemy bullets - player boxes
            RiseBoxesInRange(filterNotRisingAllyBox,filterEnemyBullet);
            BoxBulletCollision(filterRisingAllyBox,filterEnemyBullet);
            CheckForBulletsInRange(filterRisingAllyBox,filterEnemyBullet);
        }


        private bool IsInRange(in Vector3 pos1, in Vector3 pos2, float range)
        {
            var v1 = new Vector2(pos1.x, pos1.z);
            var v2 = new Vector2(pos2.x, pos2.z);
            return (v1 - v2).magnitude <= range;
        }
    }
}