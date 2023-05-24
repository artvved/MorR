using System;
using DG.Tweening;
using UnityEngine;

namespace Game.Mono
{
    public class BoxView : BaseView
    {
        private MeshRenderer meshRenderer;

        private void Start()
        {
            meshRenderer=GetComponentInChildren<MeshRenderer>();
        }

        private float animTime = 0.3f;

        public void Rise()
        {
            transform.DOMoveY(2, animTime);
        }

        public void Fall()
        {
            transform.DOMoveY(0, animTime);
        }

        public void SetMaterial(Material material)
        {
            meshRenderer.material = material;
        }
    }
}