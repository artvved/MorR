using System;
using Game.Mono;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class CounterView : BaseView
    {
        [SerializeField] private TextMeshProUGUI text;
        
        public void SetText(int value)
        {
            text.text =  $"{value}";
        }
    }
}