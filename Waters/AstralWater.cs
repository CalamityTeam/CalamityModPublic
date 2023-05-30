using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Waters
{
    public class AstralWater : ModWaterStyle
    {
        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("CalamityMod/AstralWaterflow").Slot;
        public override int GetSplashDust() => 52; //corruption water?
        public override int GetDropletGore() => ModContent.Find<ModGore>("CalamityMod/AstralWaterDroplet").Type;
        public override Asset<Texture2D> GetRainTexture() => ModContent.Request<Texture2D>("CalamityMod/Waters/AstralRain");
		public override byte GetRainVariant() => (byte)Main.rand.Next(3);
        public override Color BiomeHairColor() => Color.MediumPurple;
    }
}
