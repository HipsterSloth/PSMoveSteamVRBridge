using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SystemTrayApp
{
    public class FreePIEApi
    {
        /*
          An attempt to access the shared store of data failed.
        */
        public static int FREEPIE_IO_ERROR_SHARED_DATA = -1;

        /*
          An attempt to access out of bounds data was made.
        */
        public static int FREEPIE_IO_ERROR_OUT_OF_BOUNDS = -2;

        [StructLayout(LayoutKind.Sequential)]
        public struct FreepieData
        {
            public float yaw;
            public float pitch;
            public float roll;

            public float x;
            public float y;
            public float z;
        }

        [DllImport("freepie_io.dll")]
        public static extern int freepie_io_6dof_slots();

        [DllImport("freepie_io.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int freepie_io_6dof_write(int index, int length, FreepieData[] data);
    }
}
