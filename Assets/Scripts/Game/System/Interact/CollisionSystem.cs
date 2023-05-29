using DefaultNamespace;
using Game.Component;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class CollisionSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsPoolInject<BaseViewComponent> poolView = default;

        private readonly EcsPoolInject<Rising> poolRising = default;
        private readonly EcsPoolInject<RiseEvent> poolRisingEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<FallEvent> poolFallEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<HitEvent> poolHitEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<EndGameEvent> poolEndEvent = Idents.EVENT_WORLD;

        private readonly EcsPoolInject<DeadTag> poolDead = default;

        private readonly EcsCustomInject<StaticData> data = default;

        private EcsFilter filterNotRisingEnemyBox;
        private EcsFilter filterRisingEnemyBox;
        private EcsFilter filterAllyBullet;

        private EcsFilter filterNotRisingAllyBox;
        private EcsFilter filterRisingAllyBox;
        private EcsFilter filterEnemyBullet;

        private EcsFilter filterAllyCannon;
        private EcsFilter filterEnemyCannon;


        private float riseRange;
        private float collisionBoxRange;
        private float collisionCannonRange;

        public void Init(IEcsSystems systems)
        {
            riseRange = data.Value.RiseRange;
            collisionBoxRange = (data.Value.BoxSize + data.Value.BulletSize) / 2;
            collisionCannonRange = (data.Value.CannonSize + data.Value.BulletSize) / 2;

            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);

            filterNotRisingEnemyBox = world.Filter<Box>().Inc<Enemy>().Exc<Rising>().End();
            filterRisingEnemyBox = world.Filter<Box>().Inc<Enemy>().Inc<Rising>().End();
            filterAllyBullet = world.Filter<Bullet>().Inc<Ally>().End();

            filterNotRisingAllyBox = world.Filter<Box>().Inc<Ally>().Exc<Rising>().End();
            filterRisingAllyBox = world.Filter<Box>().Inc<Ally>().Inc<Rising>().End();
            filterEnemyBullet = world.Filter<Bullet>().Inc<Enemy>().End();

            filterAllyCannon = world.Filter<Cannon>().Inc<Ally>().End();
            filterEnemyCannon = world.Filter<Cannon>().Inc<Enemy>().End();
        }

        public void Run(IEcsSystems systems)
        {
            //enemy boxes - player bullets
            RiseBoxesInRange(filterNotRisingEnemyBox, filterAllyBullet);
            BoxBulletCollision(filterRisingEnemyBox, filterAllyBullet);
            CheckForBulletsInRange(filterRisingEnemyBox, filterAllyBullet);

            //enemy bullets - player boxes
            RiseBoxesInRange(filterNotRisingAllyBox, filterEnemyBullet);
            BoxBulletCollision(filterRisingAllyBox, filterEnemyBullet);
            CheckForBulletsInRange(filterRisingAllyBox, filterEnemyBullet);

            CheckCannonHit(true);
            CheckCannonHit(false);
        }

        private void CheckCannonHit(bool ally)
        {
            EcsFilter filterCannon, filterBullet;
            if (ally)
            {
                filterCannon = filterAllyCannon;
                filterBullet = filterEnemyBullet;
            }
            else
            {
                filterCannon = filterEnemyCannon;
                filterBullet = filterAllyBullet;
            }

            foreach (var cannon in filterCannon)
            {
                var cannonPos = poolView.Value.Get(cannon).Value.transform.position;
                foreach (var bullet in filterBullet)
                {
                    var bulletPos = poolView.Value.Get(bullet).Value.transform.position;
                    if (IsInRange(bulletPos, cannonPos, collisionCannonRange))
                    {
                        poolEndEvent.NewEntity(out int e).IsWin = !ally;
                        Debug.Log("Fin");
                    }
                }
            }
        }

        private void RiseBoxesInRange(EcsFilter filterNotRisingBox, EcsFilter filterBullet)
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

        private void BoxBulletCollision(EcsFilter filterRisingBox, EcsFilter filterBullet)
        {
            //del rise on collision
            foreach (var box in filterRisingBox)
            {
                var boxPos = poolView.Value.Get(box).Value.transform.position;

                foreach (var bullet in filterBullet)
                {
                    var bulletPos = poolView.Value.Get(bullet).Value.transform.position;
                    //collision check
                    if (IsInRange(bulletPos, boxPos, collisionBoxRange) && !poolDead.Value.Has(bullet))
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

        private void CheckForBulletsInRange(EcsFilter filterRisingBox, EcsFilter filterBullet)
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


        private bool IsInRange(in Vector3 pos1, in Vector3 pos2, float range)
        {
            var v1 = new Vector2(pos1.x, pos1.z);
            var v2 = new Vector2(pos2.x, pos2.z);
            return (v1 - v2).magnitude <= range;
        }
    }
}