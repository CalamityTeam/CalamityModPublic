
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.Ores
{
    public class UelibloomOre : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileValue[Type] = 1005;

            TileMerge.MergeGeneralTiles(Type);

            drop = ModContent.ItemType<Items.Placeables.Ores.UelibloomOre>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Uelibloom Ore");
            AddMapEntry(new Color(0, 255, 0), name);
            mineResist = 5f;
            minPick = 250;
            soundType = 21;
            Main.tileSpelunker[Type] = true;
        }

        public override bool CanExplode(int i, int j)
        {
            return NPC.downedMoonlord;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            CustomTileFraming.FrameTileForCustomMerge(i, j, Type, TileID.Mud, false, false, false, false, resetFrame);
            return false;
        }
    }
}
