using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Terraria;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod
{
    public static class Injections
    {
        public static void Load()
        {
            // DoSwaps();
        }
        
        public static void Unload()
        {
            // DoSwaps();
        }

        // This function has no effect except causing crashes
		public static void DoSwaps()
        {
            try
            {
                if (!Main.dedServ)
                    Swap(typeof(Main), "checkMap", "NewCheckMap");
            }
            catch
            {
                ErrorLogger.Log("CalamityMod: Error DoSwaps()");
            }
        }

        public static void Swap(Type type, string oldMethod, string newMethod)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            RuntimeMethodHandle previousMethodHandle;
            RuntimeMethodHandle newMethodHandle;

            Type t = typeof(Injections);
            
            previousMethodHandle = type.GetMethod(oldMethod, flags).MethodHandle;
            newMethodHandle = t.GetMethod(newMethod, flags).MethodHandle;

            RuntimeHelpers.PrepareMethod(previousMethodHandle);
            RuntimeHelpers.PrepareMethod(newMethodHandle);

            IntPtr ptr = previousMethodHandle.Value + IntPtr.Size * 2;
            IntPtr ptr2 = newMethodHandle.Value + IntPtr.Size * 2;

            int value = ptr.ToInt32();
            Marshal.WriteInt32(ptr, Marshal.ReadInt32(ptr2));
            Marshal.WriteInt32(ptr2, Marshal.ReadInt32(new IntPtr(value)));
        }

        public static bool NewCheckMap(int i, int j)
        {
            MessageBox.Show("checking map");
            if (Main.instance.mapTarget[i, j] == null || Main.instance.mapTarget[i, j].IsDisposed)
            {
                Main.initMap[i, j] = false;
            }
            if (!Main.initMap[i, j])
            {
                try
                {
                    int width = Main.textureMaxWidth;
                    int height = Main.textureMaxHeight;
                    if (i == Main.mapTargetX - 1)
                    {
                        width = Main.maxTilesX % Main.textureMaxWidth;
                    }
                    if (j == Main.mapTargetY - 1)
                    {
                        height = Main.maxTilesY % Main.textureMaxHeight;
                    }
                    Main.instance.mapTarget[i, j] = new RenderTarget2D(Main.instance.GraphicsDevice, width, height, false, Main.instance.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth16, 0, RenderTargetUsage.PreserveContents);
                }
                catch
                {
                    Main.mapEnabled = false;
                    for (int k = 0; k < Main.mapTargetX; k++)
                    {
                        for (int l = 0; l < Main.mapTargetY; l++)
                        {
                            try
                            {
                                Main.initMap[k, l] = false;
                                Main.instance.mapTarget[k, l].Dispose();
                            }
                            catch
                            {
                            }
                        }
                    }
                    return false;
                }
                Main.initMap[i, j] = true;
                return true;
            }
            return true;
        }
    }
}
