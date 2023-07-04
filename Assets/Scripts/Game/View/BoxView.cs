using System;
using DG.Tweening;
using UnityEngine;

namespace Game.Mono
{
    public class BoxView : BaseView
    {
        [SerializeField] private GameObject playerVisual;
        [SerializeField] private GameObject enemyVisual;
        

        [SerializeField] private float animTime = 0.3f;

        public void Rise()
        {
            transform.DOMoveY(2, animTime);
        }

        public void Fall()
        {
            transform.DOMoveY(0, animTime);
        }

        public void SetSide(bool isPlayer)
        { 
           playerVisual.SetActive(isPlayer);
           enemyVisual.SetActive(!isPlayer);
        }
    }
}