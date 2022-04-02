using CalamityMod.CalPlayer;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class SulphuricWater : ModWaterStyle
    {
        public override bool ChooseWaterStyle()
        {
            int biomeWidth;
            // Small world
            if (Main.maxTilesX == 4200)
            {
                biomeWidth = 270;
            }
            // Medium world
            else if (Main.maxTilesX == 6400)
            {
                biomeWidth = 365;
            }
            // Large world
            else
            {
                biomeWidth = 430;
            }
            biomeWidth += 25;
            bool inXZone = Main.LocalPlayer.Center.X < biomeWidth * 16f;
            if (!CalamityWorld.abyssSide)
                inXZone = Main.LocalPlayer.Center.X > Main.maxTilesX * 16f - biomeWidth * 16f;

            bool inYZone = Main.LocalPlayer.Center.Y < Main.rockLayer * 16f - 320 && Main.LocalPlayer.Center.Y >= 5800f;

            CalamityPlayer modPlayer = Main.LocalPlayer.Calamity();
            bool greenWater = (inXZone && inYZone) || modPlayer.ZoneSulphur;
            return greenWater;
        }

        public override int ChooseWaterfallStyle()
        {
            return mod.GetWaterfallStyleSlot("SulphuricWaterflow");
        }

        public override int GetSplashDust()
        {
            return 101;
        }

        public override int GetDropletGore()
        {
            return 708;
        }

        public override Color BiomeHairColor()
        {
            return Color.Turquoise;
        }
    }
}
