using System;

namespace PES5_WE9_LE_GDB_Manager
{
    public class PESTexture
    {
        public ushort Width;
        public ushort Height;
        public byte[] Palette;
        public byte[] Pixels;
        public void ConvertPalette()
        {
            byte[] d = Palette;

            // Swap certain blocks in palette
            for (int i = 32; i < d.Length; i += 128)
            {
                byte[] a = new byte[32];
                byte[] b = new byte[32];
                Array.Copy(d, i, a, 0, 32);
                Array.Copy(d, i + 32, b, 0, 32);

                Array.Copy(a, 0, d, i + 32, 32);
                Array.Copy(b, 0, d, i, 32);
            }

            Palette = d;
        }
        public void DisableAlpha() 
        {
            byte[] d = Palette;

            // Disable alpha (commented out in the original VB.NET code)
            for (int i = 3; i < d.Length; i += 4)
            {
                if (d[i] > 0)
                {
                    d[i] = (byte)(d[i] * 2 - 1);
                }
            }
            Palette = d;
        }
    }
}
