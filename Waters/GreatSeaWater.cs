using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class GreatSeaWater : ModWaterStyle
    {
        public override bool ChooseWaterStyle()
        {
            return Main.LocalPlayer.GetModPlayer<CalamityPlayer>(mod).ZoneGreatSea;
        }

        public override int ChooseWaterfallStyle()
        {
            return mod.GetWaterfallStyleSlot("GreatSeaWaterflow");
        }

        public override int GetSplashDust()
        {
            return 185;
        }

        public override int GetDropletGore()
        {
            return 713;
        }

        public override Color BiomeHairColor()
        {
            return Color.Cyan;
        }
    }
}