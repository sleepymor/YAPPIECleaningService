using UnityEngine;

namespace Water2D
{
    public class RenderTextureBlitPack 
    {
        public RenderTexture CurrentState, NextState, Temporary;

        public RenderTextureBlitPack()
        {
            CurrentState = null;
            NextState = null; 
            Temporary = null;
        }

        public void Release()
        {
            CurrentState.Release();
            NextState.Release();
            Temporary.Release();
        }
    }
}