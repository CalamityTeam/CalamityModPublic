using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
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
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AddMapEntry(new Color(0, 150, 200), CalamityUtils.GetItemName<PrismShard>());
            HitSound = SoundID.Item27;
            DustType = 67;
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
            bool lessLightDueToLowWater = tile.LiquidAmount > 50 && tile.LiquidType == LiquidID.Water;
            int xframe = tile.TileFrameX / 18;
            if (xframe > 7 && xframe < 14)
            {
                r = lessLightDueToLowWater ? 0.36f : 0.48f;
                g = lessLightDueToLowWater ? 0.315f : 0.42f;
                b = lessLightDueToLowWater ? 0.405f : 0.54f;
            }
            else if (xframe >= 14)
            {
                r = lessLightDueToLowWater ? 0.225f : 0.3f;
                g = lessLightDueToLowWater ? 0.405f : 0.54f;
                b = lessLightDueToLowWater ? 0.315f : 0.42f;
            }
            else
            {
                r = lessLightDueToLowWater ? 0.27f : 0.36f;
                g = lessLightDueToLowWater ? 0.405f : 0.54f;
                b = lessLightDueToLowWater ? 0.405f : 0.54f;
            }
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
                Main.tile[i, j].TileFrameY = 0;
            else if (aboveTile.Slope == SlopeType.Solid && !aboveTile.IsHalfBlock && aboveTile.HasTile && aboveTile.IsTileSolid())
                Main.tile[i, j].TileFrameY = 18;
            else if (rightTile.Slope == SlopeType.Solid && !rightTile.IsHalfBlock && rightTile.HasTile && rightTile.IsTileSolid())
                Main.tile[i, j].TileFrameY = 36;
            else if (leftTile.Slope == SlopeType.Solid && !leftTile.IsHalfBlock && leftTile.HasTile && leftTile.IsTileSolid())
                Main.tile[i, j].TileFrameY = 54;

            Main.tile[i, j].TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
        }
    }
}
