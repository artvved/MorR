using UnityEngine;

namespace Game.Mono
{
    public class LevelView : BaseView
    {
        public GameObject PlayerField;
        public GameObject EnemyField;
        
        public CannonView PlayerCannon;
        public CannonView EnemyCannon;

        public RandomizerView PlayerRandomizer;
        public RandomizerView EnemyRandomizer;
    }
}