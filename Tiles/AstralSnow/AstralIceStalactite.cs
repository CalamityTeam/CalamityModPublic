
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.AstralSnow
{
    public class AstralIceStalactite : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileBlockLight[Type] = true;

            DustType = ModContent.DustType<AstralBasic>();
            AddMapEntry(new Color(79, 61, 97));

            base.SetStaticDefaults();
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY <= 18 || tile.TileFrameY == 72)
            {
                offsetY = -2;
            }
            else if ((tile.TileFrameY >= 36 && tile.TileFrameY <= 54) || tile.TileFrameY == 90)
            {
                offsetY = 2;
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            WorldGen.CheckTight(i, j);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 4;
        }

        public override bool IsTileBiomeSightable(int i, int j, ref Color sightColor)
        {
            sightColor = Color.Cyan;
            return true;
        }
    }
}
