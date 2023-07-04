using System;
using DefaultNamespace;
using UnityEngine;

namespace Game.Mono
{
    public class MarbleFinishView : BaseView
    {
        public MarbleFinishType Type;
        public bool Active;

        [Header("Materials")]
        [SerializeField] private Material activeMaterial;
        [SerializeField] private Material notActiveMaterial;
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public void Toggle(bool active)
        {
            Active = active;
            meshRenderer.material = Active ? activeMaterial : notActiveMaterial;
        }
    }
}