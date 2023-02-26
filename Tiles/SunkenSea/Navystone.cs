using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
    public class Navystone : ModTile
    {
        public byte[,] tileAdjacency;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithDesert(Type);

            TileID.Sets.ChecksForMerge[Type] = true;
            DustType = 96;
            ItemDrop = ModContent.ItemType<Items.Placeables.Navystone>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Navystone");
            AddMapEntry(new Color(31, 92, 114), name);
            MineResist = 2f;
            HitSound = SoundID.Tink;

            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<EutrophicSand>(), out tileAdjacency);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/SunkenSea/Navystone_Blend");
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<EutrophicSand>(), out tileAdjacency[i, j]);
            return TileFraming.BrimstoneFraming(i, j, resetFrame);
        }
    }
}
