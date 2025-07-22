using System;
using UnityEngine;
using UnityEngine.Events;

namespace Water2D
{
    [Serializable]
    public enum ColoringType
    {
        single_color = 0,
        two_colors = 1,
        depthY = 2,
        distance_from_obstructors = 3
    }

    [Serializable]
    public class WaterSettings
    {
        public WaterCryo<Color> color;
        public WaterCryo<Color> depthColor;
        public ColoringType coloringType = ColoringType.two_colors;
        public WaterCryo<Vector2> tiling;
        public WaterCryo<float> baseAlpha;
        public Texture2D alphaTexture;
        public WaterCryo<int> numOfPixels;
        public WaterCryo<bool> pixelPerfect;
        public WaterCryo<float> obstructionWidth;
        public WaterCryo<Color> obstructionColor;
        public WaterCryo<float> obstructionAlpha;
        public WaterCryo<Color> foamColor;
        public WaterCryo<float> foamSize;
        public WaterCryo<Vector2> foamSpeed;
        public WaterCryo<float> foamAlpha;
        public WaterCryo<Vector2> distortionSpeed;
        public WaterCryo<Vector2> distortionStrength;
        public WaterCryo<Vector2> distortionTiling;
        public WaterCryo<Vector2> distortionMinMax;
        public WaterCryo<Color> distortionColor = new WaterCryo<Color>(Color.black);
        public Texture2D distortionTexture;
        public Texture2D sunStripsTexture;
        public WaterCryo<float> stripsSpeed;
        public WaterCryo<float> stripsScrollingSpeed;
        public WaterCryo<float> stripsSize;

        public SpriteRenderer surfaceSprite;
        public Texture2D surfaceTexture;
        public WaterCryo<Vector2> surfaceTiling;
        public WaterCryo<Vector2> surfaceSpeed;
        public WaterCryo<bool> useFoamSpeed;
        public WaterCryo<float> surfaceAlpha;

        public WaterCryo<float> stripsAlpha;
        public WaterCryo<float> stripsDensity;
        public WaterCryo<float> foamDensity;
        public WaterCryo<Vector2> perspective;
        public WaterCryo<bool> _useLighting;
        public WaterCryo<bool> depthFromObstructors = new WaterCryo<bool>(false);

        public WaterCryo<bool> enableBelowWater;
        public WaterCryo<Vector4> belowWaterUV;
        public WaterCryo<float> belowWaterDistortionStrength;
        public WaterCryo<float> belowWaterAlpha;

        public WaterCryo<Gradient> colorGradient = new WaterCryo<Gradient>(new Gradient());
        public WaterCryo<float> depthMlp;

        internal void onValueChanged(UnityAction onWaterChanged)
        {
            depthMlp.onValueChanged = onWaterChanged;
            colorGradient.onValueChanged = onWaterChanged;
            enableBelowWater.onValueChanged = onWaterChanged;
            depthFromObstructors.onValueChanged = onWaterChanged;
            belowWaterAlpha.onValueChanged = onWaterChanged;
            belowWaterDistortionStrength.onValueChanged = onWaterChanged;
            belowWaterUV.onValueChanged = onWaterChanged;
            surfaceSpeed.onValueChanged = onWaterChanged;
            surfaceTiling.onValueChanged = onWaterChanged;
            useFoamSpeed.onValueChanged = onWaterChanged;
            surfaceAlpha.onValueChanged = onWaterChanged;
            color.onValueChanged = onWaterChanged;
            tiling.onValueChanged = onWaterChanged;
            pixelPerfect.onValueChanged = onWaterChanged;
            numOfPixels.onValueChanged = onWaterChanged;
            obstructionWidth.onValueChanged = onWaterChanged;
            obstructionColor.onValueChanged = onWaterChanged;
            obstructionAlpha.onValueChanged = onWaterChanged;
            depthColor.onValueChanged = onWaterChanged;
            foamColor.onValueChanged = onWaterChanged;
            foamSize.onValueChanged = onWaterChanged;
            foamSpeed.onValueChanged = onWaterChanged;
            foamAlpha.onValueChanged = onWaterChanged;
            distortionSpeed.onValueChanged = onWaterChanged;
            distortionStrength.onValueChanged = onWaterChanged;
            distortionTiling.onValueChanged = onWaterChanged;
            stripsSpeed.onValueChanged = onWaterChanged;
            stripsScrollingSpeed.onValueChanged = onWaterChanged;
            stripsSize.onValueChanged = onWaterChanged;
            stripsAlpha.onValueChanged = onWaterChanged;
            stripsDensity.onValueChanged = onWaterChanged;
            foamDensity.onValueChanged = onWaterChanged;
            baseAlpha.onValueChanged = onWaterChanged;
            perspective.onValueChanged = onWaterChanged;
            distortionMinMax.onValueChanged = onWaterChanged;
            distortionColor.onValueChanged = onWaterChanged;
            _useLighting.onValueChanged = onWaterChanged;
        }
    }

}