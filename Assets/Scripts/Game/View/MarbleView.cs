using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Mono
{
    public class MarbleView : BaseView
    {
        private Rigidbody rb;
        
        
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }
        

        public void Bounce(Vector3 dir)
        {
            rb.AddForce(dir);
        }

      
    }
}