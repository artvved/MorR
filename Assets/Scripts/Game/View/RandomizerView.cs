using DefaultNamespace;
using UnityEngine;

namespace Game.Mono
{
    public class RandomizerView : BaseView
    {
        public Transform MarbleSpawn;
        public MarbleFinishView[] Finishes;

        public void ToggleInput(bool block)
        {
            foreach (var finish in Finishes)
            {
                //change color
                finish.Toggle(!block);
            }
        }
    }
}