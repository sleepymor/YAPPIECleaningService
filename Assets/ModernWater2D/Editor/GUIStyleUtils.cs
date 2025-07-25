using UnityEngine;
using UnityEditor;

namespace Water2D
{
    public static class GUIStyleUtils 
    {


        public static GUIStyle DropDown(int size = 18, string color = "ffffff") 
        {
   

            GUIStyle res = EditorStyles.foldout;
            res.onHover.textColor = WaterColorUtils.HexToRGB(color);
            res.fontSize = size;
            res.normal.textColor = WaterColorUtils.HexToRGB(color);
            res.fontStyle = FontStyle.Bold;
            return res;
        }

        public static GUIStyle Label(int size = 18, string color = "ffffff")
        {


            GUIStyle res = EditorStyles.largeLabel;
            res.onHover.textColor = WaterColorUtils.HexToRGB(color);
            res.fontSize = size;
            res.normal.textColor = WaterColorUtils.HexToRGB(color);
            res.fontStyle = FontStyle.Bold;
            return res;
        }
    }

}