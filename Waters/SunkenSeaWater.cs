using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class SunkenSeaWater : ModWaterStyle
    {
        public override bool ChooseWaterStyle()
        {
            CalamityPlayer modPlayer = Main.LocalPlayer.Calamity();
            return modPlayer.ZoneSunkenSea;
        }

        public override int ChooseWaterfallStyle()
        {
            return Mod.GetWaterfallStyleSlot("SunkenSeaWaterflow");
        }

        public override int GetSplashDust()
        {
            return 33;
        }

        public override int GetDropletGore()
        {
            return 713;
        }

        public override Color BiomeHairColor()
        {
            return Color.Blue;
        }
    }
}
