using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class SulphuricWater : ModWaterStyle
    {
        public override bool ChooseWaterStyle()
        {
            return Main.LocalPlayer.GetModPlayer<CalamityPlayer>(mod).ZoneSulphur;
        }

        public override int ChooseWaterfallStyle()
        {
            return mod.GetWaterfallStyleSlot("SulphuricWaterflow");
        }

        public override int GetSplashDust()
        {
            return 102;
        }

        public override int GetDropletGore()
        {
            return 711;
        }

        public override Color BiomeHairColor()
        {
            return Color.Yellow;
        }
    }
}
