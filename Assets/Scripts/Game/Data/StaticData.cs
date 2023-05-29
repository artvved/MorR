using Game.Mono;
using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class StaticData : ScriptableObject
    {
        public UnitStats PlayerStats;
        public CannonView CannonPrefab;
        public float CannonRotationSpeed;
        public BoxView BoxPrefab;
        public Material AllyMaterial;
        public Material EnemyMaterial;
        public float BulletSpeed;
        public BulletView BulletPrefab;
        public MarbleView MarblePrefab;

        public float MarbleBounce;
        
        public float RiseRange ;
        public float BulletSize ;
        public float BoxSize ;
        public float CannonSize ;
        public float RotationAngle;
    }
}