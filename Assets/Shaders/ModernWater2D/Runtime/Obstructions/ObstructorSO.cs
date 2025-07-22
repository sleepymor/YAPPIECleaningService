using System;
using UnityEngine;

namespace Water2D
{
    [Serializable]
    public class ObstructorSO
    {
        public Transform source;
        public Transform child;

        public SpriteRenderer sourceSr;
        public SpriteRenderer childSr;


        public ObstructorSO(Transform source, Transform child, SpriteRenderer sourceSr, SpriteRenderer childSr)
        {
            this.source = source;
            this.child = child;
            this.sourceSr = sourceSr;
            this.childSr = childSr;
        }
    }
}