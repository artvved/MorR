using System.Collections.Generic;
using Game.Component;
using Game.Component.Time;
using Game.Mono;

using Leopotam.EcsLite;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;

namespace Game.Service
{
    public class Fabric
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        private StaticData staticData;
        private SceneData sceneData;

        //unit tag
        private EcsPool<Cannon> poolCanon;
        private EcsPool<Box> poolBox;
        private EcsPool<Ally> poolAlly;
        private EcsPool<Enemy> poolEnemy;
        private EcsPool<Bullet> poolBullet;
        private EcsPool<Marble> poolMarble;
        private EcsPool<MarbleFinish> poolMarbleFinish;
        //unit stats
        private EcsPool<Speed> poolSpeed;
        private EcsPool<Direction> poolDir;
        //view
        private EcsPool<BaseViewComponent> baseViewPool;
        private EcsPool<RandomizerComponent> poolRandomizer;


        public Fabric(EcsWorld world, StaticData staticData, SceneData sceneData)
        {
            this.world = world;

            this.staticData = staticData;
            this.sceneData = sceneData;

            poolCanon = world.GetPool<Cannon>();
           
            baseViewPool = world.GetPool<BaseViewComponent>();
         
            poolEnemy = this.world.GetPool<Enemy>();
            poolAlly = this.world.GetPool<Ally>();
            poolBox = world.GetPool<Box>();
            poolBullet = world.GetPool<Bullet>();
            poolSpeed = world.GetPool<Speed>();
            poolDir = world.GetPool<Direction>();
            poolMarble = world.GetPool<Marble>();
            poolMarbleFinish = world.GetPool<MarbleFinish>();
            poolRandomizer = world.GetPool<RandomizerComponent>();
        }


        private int InstantiateObj(BaseView prefab, Vector3 position)
        {
            var view = GameObject.Instantiate(prefab);
            view.transform.position = position;
            int unitEntity = world.NewEntity();
            view.Entity = unitEntity;

            baseViewPool.Add(unitEntity).Value = view;
           
            return unitEntity;
        }

        public int InitObj(BaseView view)
        {
            int entity = world.NewEntity();
            view.Entity = entity;
            baseViewPool.Add(entity).Value = view;
            return entity;
        }

        

        public void InstantiateLevel(LevelView levelView)
        {
            var level = GameObject.Instantiate(levelView);

            var playerField = level.PlayerField;
            var boxViewsPl = playerField.GetComponentsInChildren<BoxView>();
            foreach (var box in boxViewsPl)
            {
                var boxE = InitObj(box);
                AddSide(boxE,true);
                poolBox.Add(boxE);
                box.SetSide(true);
            }
            
            var enemyField = level.EnemyField;
            var boxViewsE = enemyField.GetComponentsInChildren<BoxView>();
            foreach (var box in boxViewsE)
            {
                var boxE = InitObj(box);
                AddSide(boxE,false);
                poolBox.Add(boxE);
                box.SetSide(false);
            }

            var playerCannon = InitObj(level.PlayerCannon);
            AddSide(playerCannon,true);
            ref var playerCannonComp = ref poolCanon.Add(playerCannon);
            playerCannonComp.IsClockwise=true;
            playerCannonComp.BulletCount = 1;
            poolDir.Add(playerCannon).Value = new Vector3(0, 0, 1);
            poolRandomizer.Add(playerCannon).Value=level.PlayerRandomizer;
            
            var enemyCannon = InitObj(level.EnemyCannon);
            AddSide(enemyCannon,false);
            ref var enemyCannonComp = ref poolCanon.Add(enemyCannon);
            enemyCannonComp.IsClockwise=true;
            enemyCannonComp.BulletCount = 1;
            poolDir.Add(enemyCannon).Value = new Vector3(0, 0, -1);
            poolRandomizer.Add(enemyCannon).Value=level.EnemyRandomizer;
            

            var playerRandomizer = level.PlayerRandomizer;
            InstantiateMarble(playerRandomizer.MarbleSpawn.position,true);
            foreach (var finish in playerRandomizer.Finishes)
            {
                InitMarbleFinish(finish);
            }
            
            var eRandomizer = level.EnemyRandomizer;
            InstantiateMarble(eRandomizer.MarbleSpawn.position,false);
            foreach (var finish in eRandomizer.Finishes)
            {
                InitMarbleFinish(finish);
            }

            
        }

        public int InstantiateBullet(Vector3 pos,Vector3 dir,bool isAlly)
        {
            var bullet= InstantiateObj(staticData.BulletPrefab,pos);
            AddSide(bullet,isAlly);
            poolBullet.Add(bullet);
            poolDir.Add(bullet).Value = dir;
            poolSpeed.Add(bullet).Value = staticData.BulletSpeed;
            return bullet;
        }

        private void AddSide(int entity,bool ally)
        {
            if (ally)
                poolAlly.Add(entity);
            else
                poolEnemy.Add(entity);
            
        }

        public int InstantiateMarble(Vector3 pos,bool ally)
        {
            var entity = InstantiateObj(staticData.MarblePrefab, pos);
            poolMarble.Add(entity).Spawn=pos;
            AddSide(entity,ally);
            return entity;
        }
        
        public int InitMarbleFinish(BaseView view)
        {
            var entity = InitObj(view);
            poolMarbleFinish.Add(entity).Type=((MarbleFinishView)view).Type;
            return entity;
        }




    }
}