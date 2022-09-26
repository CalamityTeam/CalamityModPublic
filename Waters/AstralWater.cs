using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class AstralWater : ModWaterStyle
    {
        public override int ChooseWaterfallStyle()
        {
            return ModContent.Find<ModWaterfallStyle>("CalamityMod/AstralWaterflow").Slot;
        }

        public override int GetSplashDust()
        {
            return 52; //corruption water?
        }

        public override int GetDropletGore()
        {
            return ModContent.Find<ModGore>("CalamityMod/AstralWaterDroplet").Type;
        }

        public override Color BiomeHairColor()
        {
            return Color.MediumPurple;
        }
    }
}
