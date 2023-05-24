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
    }
}