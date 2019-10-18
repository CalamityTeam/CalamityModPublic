
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.SunkenSea
{
    public class EutrophicSand : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileMerge.MergeGeneralTiles(Type);
            TileMerge.MergeDesertTiles(Type);

            TileID.Sets.ChecksForMerge[Type] = true;
            dustType = 108;
            drop = ModContent.ItemType<Items.EutrophicSand>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Eutrophic Sand");
            AddMapEntry(new Color(100, 100, 150), name);
            mineResist = 2f;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return CalamityWorld.downedDesertScourge;
        }

        public override bool CanExplode(int i, int j)
        {
            return CalamityWorld.downedDesertScourge;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            CustomTileFraming.FrameTileForCustomMerge(i, j, Type, TileID.Sandstone, false, false, false, false, resetFrame);
            return false;
        }
    }
}
