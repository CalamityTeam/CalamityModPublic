using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Waters
{
    public class AbyssWater : ModWaterStyle
    {
        public override bool ChooseWaterStyle()
        {
            return Main.LocalPlayer.GetModPlayer<CalamityPlayer>(mod).ZoneAbyss;
        }

        public override int ChooseWaterfallStyle()
        {
            return mod.GetWaterfallStyleSlot("AbyssWaterflow");
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
