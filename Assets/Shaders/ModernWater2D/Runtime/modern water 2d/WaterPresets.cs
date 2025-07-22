namespace Water2D
{
    public static class WaterPresets
    {
        public static string GetPath(WaterPresetShaders preset)
        {
            switch(preset) 
            {
                case WaterPresetShaders.gradientFoam: return gradientFoamShaderPath;
                case WaterPresetShaders.vonroiFoam: return vonroiFoamShaderPath;
                default: return gradientFoamShaderPath;
            }
        }

        public const string gradientFoamShaderPath = "Materials/gradientFoamWater";
        public const string vonroiFoamShaderPath = "Materials/vonroiFoamWater";

        public enum WaterPresetShaders
        {
            gradientFoam,
            vonroiFoam
        }
    }

}