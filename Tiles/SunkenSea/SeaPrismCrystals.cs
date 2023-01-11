using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
    public class SeaPrismCrystals : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Sea Prism Crystal");
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AddMapEntry(new Color(0, 150, 200), name);
            HitSound = SoundID.Item27;
            DustType = 67;
            ItemDrop = ModContent.ItemType<PrismShard>();
            Main.tileSpelunker[Type] = true;
            MinPick = 55;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            var tile = Main.tile[i, j];
            float brightness = 0.7f;
            if (tile.LiquidAmount > 50 && tile.LiquidType == 0)
                brightness = 1.0f;

            r = 0.6f;
            g = 0.9f;
            b = 0.9f;

            int xframe = tile.TileFrameX / 18;
            if (xframe > 7 && xframe < 14)
            {

                r = 0.8f;
                g = 0.7f;
                b = 0.9f;
            }

            if (xframe >= 14)
            {
                r = 0.5f;
                g = 0.9f;
                b = 0.7f;
            }

            r *= brightness;
            g *= brightness;
            b *= brightness;

        }

        public override bool CanPlace(int i, int j)
        {
            Tile belowTile = Main.tile[i, j + 1];
            Tile aboveTile = Main.tile[i, j - 1];
            Tile rightTile = Main.tile[i + 1, j];
            Tile leftTile = Main.tile[i - 1, j];

            if ((belowTile.Slope == SlopeType.Solid && !belowTile.IsHalfBlock && belowTile.HasTile && belowTile.IsTileSolid()) ||
                (aboveTile.Slope == SlopeType.Solid && !aboveTile.IsHalfBlock && aboveTile.HasTile && aboveTile.IsTileSolid()) ||
                (rightTile.Slope == SlopeType.Solid && !rightTile.IsHalfBlock && rightTile.HasTile && rightTile.IsTileSolid()) ||
                (leftTile.Slope == SlopeType.Solid && !leftTile.IsHalfBlock && leftTile.HasTile && leftTile.IsTileSolid()))
                return true;

            return false;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            Tile belowTile = Main.tile[i, j + 1];
            Tile aboveTile = Main.tile[i, j - 1];
            Tile rightTile = Main.tile[i + 1, j];
            Tile leftTile = Main.tile[i - 1, j];

            if (belowTile.Slope == SlopeType.Solid && !belowTile.IsHalfBlock && belowTile.HasTile && belowTile.IsTileSolid())
            {
                Main.tile[i, j].TileFrameY = 0;
            }
            else if (aboveTile.Slope == SlopeType.Solid && !aboveTile.IsHalfBlock && aboveTile.HasTile && aboveTile.IsTileSolid())
            {
                Main.tile[i, j].TileFrameY = 18;
            }
            else if (rightTile.Slope == SlopeType.Solid && !rightTile.IsHalfBlock && rightTile.HasTile && rightTile.IsTileSolid())
            {
                Main.tile[i, j].TileFrameY = 36;
            }
            else if (leftTile.Slope == SlopeType.Solid && !leftTile.IsHalfBlock && leftTile.HasTile && leftTile.IsTileSolid())
            {
                Main.tile[i, j].TileFrameY = 54;
            }
            Main.tile[i, j].TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
        }
    }
}
