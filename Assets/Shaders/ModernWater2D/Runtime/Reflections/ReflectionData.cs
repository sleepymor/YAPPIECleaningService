using System;
using UnityEngine;

namespace Water2D
{
    [Serializable]
    public class ReflectionData 
    {
        //unity objects
        public Transform source;
        public Transform reflectionPivot;
        public ReflectionPivotSourceMode reflectionPivotSourceMode;
        public Transform reflection;
        public Transform customPivot;

        //sprite renderers
        public SpriteRenderer sourceSr;
        public SpriteRenderer reflectionSr;

        //properties
        public bool MSP_ReflectionGenerator;
        public bool flipX;
        public Vector2 displacement;
        public float additionalTilt;

        public ReflectionData(Transform source, Transform reflectionPivot, ReflectionPivotSourceMode reflectionPivotSourceMode, Transform reflection, SpriteRenderer sourceSr, SpriteRenderer reflectionSr, bool flipX, Vector2 displacement, bool MSP_ReflectionGenerator, float addTilt)
        {
            this.source = source;
            this.reflectionPivot = reflectionPivot;
            this.reflectionPivotSourceMode = reflectionPivotSourceMode;
            this.reflection = reflection;
            this.sourceSr = sourceSr;
            this.reflectionSr = reflectionSr;
            this.flipX = flipX;
            this.displacement = displacement;
            this.MSP_ReflectionGenerator = MSP_ReflectionGenerator;
            this.additionalTilt = addTilt;
        }
    }
}
