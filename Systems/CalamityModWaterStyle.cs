using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public abstract class CalamityModWaterStyle : ModWaterStyle
    {
        /// <summary>
        /// Allows water styles to manipulate what color the liquid is drawn to, this can allow waters to be see-throughable to see backgrounds (surface and underground backgrounds not walls)
        /// </summary>
        /// <param name="x">X position of the water</param>
        /// <param name="y">Y position of the water</param>
        /// <param name="liquidColor">The vertexColor of the water color, this is both used to get the current color and to set the color of the water</param>
        public virtual void DrawColor(int x, int y, ref VertexColors liquidColor, bool isSlope)
        {
        }

        /// <summary>
        /// Allows you to determine how much light this water emits.<br />
        /// It can also let you light up the block in front of this water.<br />
        /// See <see cref="M:Terraria.Graphics.Light.TileLightScanner.ApplyLiquidLight(Terraria.Tile,Microsoft.Xna.Framework.Vector3@)" /> for vanilla tile light values to use as a reference.<br />
        /// </summary>
        /// <param name="i">The x position in tile coordinates.</param>
        /// <param name="j">The y position in tile coordinates.</param>
        /// <param name="r">The red component of light, usually a value between 0 and 1</param>
        /// <param name="g">The green component of light, usually a value between 0 and 1</param>
        /// <param name="b">The blue component of light, usually a value between 0 and 1</param>
        public virtual void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
        }
    }

    internal static class CalamityWaterLoader
    {
        internal static void ModifyLightSetup(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            CalamityModWaterStyle styles = (CalamityModWaterStyle)LoaderManager.Get<WaterStylesLoader>().Get(type);
            if (styles != null)
            {
                styles?.ModifyLight(i, j, ref r, ref g, ref b);
            }
        }

        internal static void DrawColorSetup(int x, int y, int type, ref VertexColors liquidColor, bool isSlope = false)
        {
            CalamityModWaterStyle styles = (CalamityModWaterStyle)LoaderManager.Get<WaterStylesLoader>().Get(type);
            if (styles != null)
            {
                styles?.DrawColor(x, y, ref liquidColor, isSlope);
            }
        }
    }
}
