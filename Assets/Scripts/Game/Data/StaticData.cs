using Game.Mono;
using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class StaticData : ScriptableObject
    {
      
        [Header("Materials")]
        public Material AllyMaterial;
        public Material EnemyMaterial;

        [Header("Stats")] 
        public string LevelPath;
        public int MaxLevel;
        
        public float CannonRotationSpeed;
        public float BulletSpeed;
        public float MarbleBounce;
        public float AddMarbleTime;

        public float RiseRange ;
        public float BulletSize ;
        public float BoxSize ;
        public float CannonSize ;
        public float RotationAngle;
        public float ShotTimeDelta;

        [Header("Prefabs")]
        public CannonView CannonPrefab;
        public BoxView BoxPrefab;
        public BulletView BulletPrefab;
        public MarbleView MarblePrefab;


        
    }
}