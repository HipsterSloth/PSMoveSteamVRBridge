using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;

namespace SystemTrayApp
{
    public class SteamVRUtility
    {
        public static OpenGL.ModelMatrix ConvertToGlModelMatrix(HmdMatrix34_t mat)
        {
            return new OpenGL.ModelMatrix(new float[] {
                mat.m0, mat.m4, mat.m8, 0.0f,
                mat.m1, mat.m5, mat.m9, 0.0f,
                mat.m2, mat.m6, mat.m10, 0.0f,
                mat.m3, mat.m7, mat.m11, 1.0f});
        }
    }
}
