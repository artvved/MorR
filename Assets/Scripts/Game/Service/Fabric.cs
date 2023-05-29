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


        public int InstantiatePlayer()
        {
            var playerEntity = InstantiateObj(staticData.CannonPrefab, Vector3.zero+new Vector3(12,0,12f));
            poolCanon.Add(playerEntity).IsClockwise=true;
            poolDir.Add(playerEntity).Value = new Vector3(0, 0, 1);
            return playerEntity;
        }

        public void InstantiateField()
        {
            var x = 24;
            var z = 24;
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < z; j++)
                {
                    var box=InstantiateBox(new Vector3(i,0,j));
                    poolAlly.Add(box);
                    baseViewPool.Get(box).Value.GetComponentInChildren<MeshRenderer>().material = staticData.AllyMaterial;
                }
            }
            
            for (int i = 0; i < x; i++)
            {
                for (int j = z; j < z*2; j++)
                {
                    var box=InstantiateBox(new Vector3(i,0,j));
                    poolEnemy.Add(box);
                    baseViewPool.Get(box).Value.GetComponentInChildren<MeshRenderer>().material = staticData.EnemyMaterial;
                }
            }
        }

        private int InstantiateBox(Vector3 pos)
        {
           var box= InstantiateObj(staticData.BoxPrefab,pos);
           poolBox.Add(box);
           return box;
        }

        public int InstantiatePlayerBullet(Vector3 pos,Vector3 dir)
        {
            var bullet= InstantiateObj(staticData.BulletPrefab,pos);
            poolAlly.Add(bullet);
            poolBullet.Add(bullet);
            poolDir.Add(bullet).Value = dir;
            poolSpeed.Add(bullet).Value = staticData.BulletSpeed;
            return bullet;
        }

        private void AddSide(int entity,bool ally)
        {
            if (ally)
            {
                poolAlly.Add(entity);
            }
            else
            {
                poolEnemy.Add(entity);
            }
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