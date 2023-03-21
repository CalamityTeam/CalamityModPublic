using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Tiles
{
    public class CryonicBrick : ModTile
    {
        int subsheetHeight = 90;
        int subsheetWidth = 234;

        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeDecorativeTiles(Type);

            HitSound = SoundID.Tink;
            ItemDrop = ModContent.ItemType<Items.Placeables.CryonicBrick>();
            AddMapEntry(new Color(99, 131, 199));

            TileFraming.SetUpUniversalMerge(Type, TileID.Dirt, out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.SnowBlock, out secondTileAdjacency);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 176, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, TileID.Dirt, out tileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.SnowBlock, out secondTileAdjacency[i, j]);
            return true;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int xPos = i % 2;
            int yPos = j % 4;
            frameXOffset = xPos * subsheetWidth;
            frameYOffset = yPos * subsheetHeight;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, "CalamityMod/Tiles/Merges/SnowMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/Merges/DirtMerge");
        }
    }
}
