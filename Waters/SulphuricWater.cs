using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

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
