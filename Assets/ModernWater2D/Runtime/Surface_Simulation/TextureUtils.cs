using System;

namespace Water2D
{
    public static class TextureUtils
    {
        public enum ResolutionEnum
        {
            _256 = 0,
            _512 = 1,
            _1024 = 2,
            _2048 = 3,
            _4096 = 4,
        }

        public static int ToInt(this ResolutionEnum resolution)
        {
            switch (resolution)
            {
                case ResolutionEnum._256: return 256;
                case ResolutionEnum._512: return 512;
                case ResolutionEnum._1024: return 1024;
                case ResolutionEnum._2048: return 2048;
                case ResolutionEnum._4096: return 4096;
                default: return 1024;
            }

        }
        static int highestPowerof2(int N)
        {
            if ((N & (N - 1)) == 0)
                return N;
            return (1 << ((Convert.ToString(N, 2).Length) - 1));
        }

        public static ResolutionEnum ToPowerOf2(int resolution)
        {
            int p2 = highestPowerof2(resolution);
            if (p2 > 4096) p2 = 4096;
            else if (p2 < 256) p2 = 256;
            
            switch(p2)
            {
                case (256): return ResolutionEnum._256;
                case (512): return ResolutionEnum._512;
                case (1024): return ResolutionEnum._1024;
                case (2048): return ResolutionEnum._2048;
                case (4096): return ResolutionEnum._4096;
                default: return ResolutionEnum._1024;
            }

        }
    }
}