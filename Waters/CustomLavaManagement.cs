using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public static class CustomLavaManagement
    {
        internal static List<CustomLavaStyle> CustomLavaStyles;
        internal static Texture2D LavaBlockTexture;
        internal static Texture2D LavaTexture;
        internal static Texture2D LavaSlopeTexture;

        // The IL Edits are loaded separately.
        internal static void Load()
        {
            CustomLavaStyles = new List<CustomLavaStyle>();

            foreach (Type type in typeof(CalamityMod).Assembly.GetTypes())
            {
                // Ignore abstract types; they cannot have instances.
                // Also ignore types which do not derive from CustomLavaStyle.
                if (!type.IsSubclassOf(typeof(CustomLavaStyle)) || type.IsAbstract)
                    continue;

                CustomLavaStyles.Add(Activator.CreateInstance(type) as CustomLavaStyle);
                CustomLavaStyles.Last().Load();
            }

            if (Main.netMode != NetmodeID.Server)
            {
                LavaBlockTexture = ModContent.Request<Texture2D>("Terraria/Images/Liquid_1", AssetRequestMode.ImmediateLoad).Value;
                LavaTexture = ModContent.Request<Texture2D>("Terraria/Images/Misc/water_1", AssetRequestMode.ImmediateLoad).Value;
                LavaSlopeTexture = ModContent.Request<Texture2D>("Terraria/Images/LiquidSlope_1", AssetRequestMode.ImmediateLoad).Value;
            }
        }

        internal static void Unload()
        {
            if (CustomLavaStyles != null)
                foreach (CustomLavaStyle lavaStyle in CustomLavaStyles)
                    lavaStyle?.Unload();

            CustomLavaStyles = null;
            LavaBlockTexture = null;
            LavaTexture = null;
        }

        internal static int SelectLavafallStyle(int initialLavafallStyle)
        {
            // Lava waterfall.
            if (initialLavafallStyle != 1)
                return initialLavafallStyle;

            foreach (CustomLavaStyle lavaStyle in CustomLavaStyles)
            {
                int waterfallStyle = lavaStyle.ChooseWaterfallStyle();
                if (lavaStyle.ChooseLavaStyle() && waterfallStyle >= 0)
                    return waterfallStyle;
            }

            return initialLavafallStyle;
        }

        internal static Color SelectLavafallColor(int initialLavafallStyle, Color initialLavafallColor)
        {
            // Lava waterfall.
            if (initialLavafallStyle != 1)
                return initialLavafallColor;

            foreach (CustomLavaStyle lavaStyle in CustomLavaStyles)
            {
                if (lavaStyle.ChooseLavaStyle())
                {
                    lavaStyle.SelectLightColor(ref initialLavafallColor);
                    return initialLavafallColor;
                }
            }

            return initialLavafallColor;
        }
    }
}
