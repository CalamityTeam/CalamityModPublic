using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Waters
{
    public class SulphuricWater : ModWaterStyle
    {
        public override int ChooseWaterfallStyle()
        {
            return ModContent.Find<ModWaterfallStyle>("CalamityMod/SulphuricWaterflow").Slot;
        }

        public override int GetSplashDust()
        {
            return 101;
        }

        public override int GetDropletGore()
        {
            return 708;
        }

        public override Asset<Texture2D> GetRainTexture() 
		{
			return ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/VanillaReplacements/RainSulphSea");
		}
		
		public override byte GetRainVariant() 
		{
			return (byte)Main.rand.Next(3);
		}

        public override Color BiomeHairColor()
        {
            return Color.Turquoise;
        }
    }
}
