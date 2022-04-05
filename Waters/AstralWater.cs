using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class AstralWater : ModWaterStyle
    {
        public override int ChooseWaterfallStyle()
        {
            return ModContent.Find<AstralWaterflow>("CalamityMod/Waters/AstralWaterflow").Slot;
        }

        public override int GetSplashDust()
        {
            return 52; //corruption water?
        }

        public override int GetDropletGore()
        {
            return ModContent.Find<ModGore>("CalamityMod/Gores/AstralWaterDroplet").Type;
        }

        public override Color BiomeHairColor()
        {
            return Color.MediumPurple;
        }
    }
}
