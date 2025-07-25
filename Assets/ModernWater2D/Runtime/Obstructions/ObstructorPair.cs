using UnityEngine;

namespace Water2D
{
    public struct ObstructorPair
    {
        public Sprite sprite;
        public float h;

        public ObstructorPair( Sprite sprite, float h)
        {
            this.sprite = sprite;
            this.h = h;
        }
    }

}