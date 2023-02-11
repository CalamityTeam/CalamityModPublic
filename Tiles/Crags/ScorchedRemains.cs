using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.Crags
{
    public class ScorchedRemains : ModTile
    {
        private int sheetWidth = 288;
        private int sheetHeight = 270;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithHell(Type);
            CalamityUtils.SetMerge(Type, ModContent.TileType<BrimstoneSlag>());

            DustType = 155;
            HitSound = SoundID.Dig;
            MineResist = 1f;
            MinPick = 100;
            ItemDrop = ModContent.ItemType<Items.Placeables.ScorchedRemains>();
            AddMapEntry(new Color(57, 52, 72));
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile up = Main.tile[i, j - 1];
            Tile left = Main.tile[i - 1, j];
            Tile right = Main.tile[i + 1, j];

            if (WorldGen.genRand.Next(3) == 0 && !up.HasTile && (left.TileType == ModContent.TileType<ScorchedRemainsGrass>() || 
            right.TileType == ModContent.TileType<ScorchedRemainsGrass>()))
            {
                Main.tile[i, j].TileType = (ushort)ModContent.TileType<ScorchedRemainsGrass>();
            }
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 1, 0f, 0f, 1, new Color(100, 100, 100), 1f);
            return false;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 3 * sheetWidth;
            frameYOffset = j % 3 * sheetHeight;
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<BrimstoneSlag>(), false, false, false, false, resetFrame);
            return false;
        }
    }
}
