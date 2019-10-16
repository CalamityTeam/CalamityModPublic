
using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Tiles
{
    public class HardenedAstralSand : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileMerge.MergeGeneralTiles(Type);
            TileMerge.MergeDesertTiles(Type);
            TileMerge.MergeAstralTiles(Type);


            dustType = 108;
            drop = ModContent.ItemType<HardenedAstralSand>();

            AddMapEntry(new Color(45, 36, 63));

            TileID.Sets.Conversion.HardenedSand[Type] = true;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            CustomTileFraming.FrameTileForCustomMerge(i, j, Type, ModContent.TileType<AstralSand>(), false, false, false, false, resetFrame);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
