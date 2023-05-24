using System.Collections.Generic;
using Game.Component;
using Leopotam.EcsLite;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Game.Service
{
    public class AnimationService {

        private EcsWorld world;
        private EcsPool<BaseViewComponent> baseTransformPool;
        private EcsPool<Speed> speedPool;
        private EcsPool<Direction> dirPool;
        
        private EcsPool<MoveToTargetComponent> moveToPool ;
       
        private EcsPool<AnimatorComponent> animatorPool;
        private EcsPool<AnimatingTag> animatingPool; 
      
      

        public AnimationService(EcsWorld world)
        {
            this.world = world;
            baseTransformPool = world.GetPool<BaseViewComponent>();
            speedPool = world.GetPool<Speed>();
            dirPool = world.GetPool<Direction>();
            moveToPool = world.GetPool<MoveToTargetComponent>();
          
            animatorPool = world.GetPool<AnimatorComponent>();
            animatingPool = world.GetPool<AnimatingTag>();
          
        }

        public void AnimateMove(int ent,float curVel,float maxVelocity)
        {
            var animator = animatorPool.Get(ent).Value;
            animator.SetBool("Move",true);
            animator.speed = curVel / (float) maxVelocity;
        }
        
        public void AnimateMove(int ent)
        {
            var animator = animatorPool.Get(ent).Value;
            animator.SetBool("Move",true);
        }
        public void AnimateStopMove(int ent)
        {
            var animator = animatorPool.Get(ent).Value;
            animator.SetBool("Move",false);
        }
        
        
    }
}