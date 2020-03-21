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
            bool inXZone = Main.LocalPlayer.Center.X < 10400f;
            if (!CalamityWorld.abyssSide)
                inXZone = Main.LocalPlayer.Center.X > Main.maxTilesX * 16f - 10400f;

            bool inYZone = Main.LocalPlayer.Center.Y < Main.rockLayer * 16f - 320 && Main.LocalPlayer.Center.Y >= 5800f;

            return (inXZone && inYZone) || Main.LocalPlayer.Calamity().ZoneSulphur;
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
