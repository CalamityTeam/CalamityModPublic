using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
    public class HardenedEutrophicSand : ModTile
    {
        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithDesert(Type);

            Main.tileShine[Type] = 2200;
            Main.tileShine2[Type] = false;

            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;

            DustType = 108;
            ItemDrop = ModContent.ItemType<Items.Placeables.HardenedEutrophicSand>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Hardened Eutrophic Sand");
            AddMapEntry(new Color(67, 107, 143), name);
            MineResist = 2f;

            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<EutrophicSand>(), out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<Navystone>(), out secondTileAdjacency);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, "CalamityMod/Tiles/SunkenSea/NavystoneMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/SunkenSea/EutrophicSandMerge");
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<EutrophicSand>(), out tileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<Navystone>(), out secondTileAdjacency[i, j]);
            return TileFraming.BrimstoneFraming(i, j, resetFrame);
        }
    }
}
