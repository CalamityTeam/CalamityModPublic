using CalamityMod.Dusts.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class UelibloomBar : ModTile
    {
        public override void SetStaticDefaults()
        {
            this.SetUpBar(ModContent.ItemType<Items.Materials.UelibloomBar>(), new Color(134, 209, 102));
            DustType = ModContent.DustType<BloomTileLeaves>();
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            type = Main.rand.NextBool() ? ModContent.DustType<BloomTileLeaves>() : ModContent.DustType<BloomTileGold>();
            return true;
        }
    }
}
