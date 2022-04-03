using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class AstralWater : ModWaterStyle
    {
        public override bool ChooseWaterStyle()
        {
            CalamityPlayer modPlayer = Main.LocalPlayer.Calamity();
            return modPlayer.ZoneAstral;
        }

        public override int ChooseWaterfallStyle()
        {
            return Mod.GetWaterfallStyleSlot("AstralWaterflow");
        }

        public override int GetSplashDust()
        {
            return 52; //corruption water?
        }

        public override int GetDropletGore()
        {
            return Mod.GetGoreSlot("Gores/AstralWaterDroplet");
        }

        public override Color BiomeHairColor()
        {
            return Color.MediumPurple;
        }
    }
}
