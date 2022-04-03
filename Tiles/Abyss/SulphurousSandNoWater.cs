using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.Abyss
{
    public class SulphurousSandNoWater : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithAbyss(Type);

            DustType = 32;
            ItemDrop = ModContent.ItemType<Items.Placeables.SulphurousSand>();
            AddMapEntry(new Color(150, 100, 50));
            mineResist = 1f;
            minPick = 55;
            SoundType = SoundID.Dig;
            SetModPalmTree(new AcidWoodTree());
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<SulphurousSandstone>(), false, false, false, false, resetFrame);
            return false;
        }
        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<AcidWoodTreeSapling>();
        }
    }
}
