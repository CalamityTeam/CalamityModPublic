using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public override Asset<Texture2D> GetRainTexture() 
		{
			return ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/VanillaReplacements/RainAstral");
		}
		
		public override byte GetRainVariant() 
		{
			return (byte)Main.rand.Next(3);
		}

        public override Color BiomeHairColor()
        {
            return Color.MediumPurple;
        }
    }
}
