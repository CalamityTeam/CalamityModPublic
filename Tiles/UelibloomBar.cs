using CalamityMod.Dusts.Furniture;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class UelibloomBar : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpBar(new Color(134, 209, 102));
            dustType = ModContent.DustType<BloomTileLeaves>();
            drop = ModContent.ItemType<UeliaceBar>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = Main.rand.NextBool() ? ModContent.DustType<BloomTileLeaves>() : ModContent.DustType<BloomTileGold>();
            return true;
        }
    }
}
