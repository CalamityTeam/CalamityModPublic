
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class SulphurousSandstone : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithAbyss(Type);

            dustType = 32;
            drop = ModContent.ItemType<Items.Placeables.SulphurousSandstone>();
            AddMapEntry(new Color(113, 90, 71));
            mineResist = 1f;
            minPick = 55;
            soundType = 0;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<AbyssGravel>(), false, false, false, false, resetFrame);
            return false;
        }
    }
}
