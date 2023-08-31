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
            this.SetUpBar(ModContent.ItemType<Items.Placeables.AstralBar>(), new Color(47, 66, 90));
            DustType = ModContent.DustType<AstralBlue>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = Main.rand.NextBool() ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>();
            return true;
        }
    }
}
