using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class AstralBar : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpBar(new Color(47, 66, 90));
            dustType = ModContent.DustType<AstralBlue>();
            drop = ModContent.ItemType<Items.Placeables.AstralBar>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = Main.rand.NextBool() ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>();
            return true;
        }
    }
}
