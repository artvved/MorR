using System.Collections.Generic;
using Game.Component;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game.Service
{
    public class MovementService
    {
        private EcsWorld world;
        private EcsPool<BaseViewComponent> baseTransformPool;
        private EcsPool<Speed> speedPool;
        private EcsPool<Direction> dirPool;

        private EcsPool<MoveToTargetComponent> moveToPool;

        private EcsPool<AnimatingTag> animatingPool;
    
        private EcsPool<UIViewComponent> viewUIPool;


        public MovementService(EcsWorld world)
        {
            this.world = world;
            baseTransformPool = world.GetPool<BaseViewComponent>();
            speedPool = world.GetPool<Speed>();
            dirPool = world.GetPool<Direction>();
            moveToPool = world.GetPool<MoveToTargetComponent>();
           
            animatingPool = world.GetPool<AnimatingTag>();
            viewUIPool = this.world.GetPool<UIViewComponent>();
        }

        public void SetSpeed(int ent, float speed)
        {
            speedPool.Add(ent).Value = speed;
        }

        public void SetDirection(int ent, Vector3 dir)
        {
            dirPool.Add(ent).Value = dir;
        }
        

        public void MoveUI(int entity, Camera camera, Vector2 offset)
        {
            ref var view = ref viewUIPool.Get(entity).Value;
            var pos = baseTransformPool.Get(entity).Value.transform.position;

            var screenPoint = camera.WorldToScreenPoint(pos + (Vector3)offset);
            view.transform.position = screenPoint;
        }
    }
}