using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class RustedPlating : ModTile
    {
        public static readonly SoundStyle MinePlatingSound = new("CalamityMod/Sounds/Custom/PlatingMine", 3);

        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);

            HitSound = MinePlatingSound;
            DustType = 32;
            MinPick = 30;
            ItemDrop = ModContent.ItemType<Items.Placeables.DraedonStructures.RustedPlating>();
            AddMapEntry(new Color(128, 90, 77));

            TileFraming.SetUpUniversalMerge(Type, TileID.Dirt, out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Stone, out secondTileAdjacency);
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, "CalamityMod/Tiles/Merges/StoneMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/Merges/DirtMerge");
        }

        public override bool CanExplode(int i, int j) => false;

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, TileID.Dirt, out tileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.Stone, out secondTileAdjacency[i, j]);
            return TileFraming.BetterGemsparkFraming(i, j, resetFrame);
        }
    }
}
