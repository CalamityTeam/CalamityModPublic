using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class VernalSoil : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);

            TileID.Sets.CanBeDugByShovel[Type] = true;

            DustType = 38;
            ItemDrop = ModContent.ItemType<Items.Placeables.VernalSoil>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Vernal Soil");
            AddMapEntry(new Color(80, 120, 0), name);
            HitSound = SoundID.Dig;
        }

        public override void RandomUpdate(int i, int j)
        {
            int j2 = j - 1;
            if (j2 < 10)
                j2 = 10;

            if (Main.tile[i, j2].LiquidAmount == 0)
            {
                if (WorldGen.genRand.NextBool(15))
                {
                    ushort tileTypeToPlace = (ushort)ModContent.TileType<GiantPlanteraBulb>();
                    int tileTypeToPlaceThickness = 5;
                    bool placeBulb = true;
                    int minDistanceFromOtherBulbs = 10;
                    for (int k = i - minDistanceFromOtherBulbs; k < i + minDistanceFromOtherBulbs; k += 2)
                    {
                        for (int l = j - minDistanceFromOtherBulbs; l < j + minDistanceFromOtherBulbs; l += 2)
                        {
                            if (k > tileTypeToPlaceThickness && k < Main.maxTilesX - tileTypeToPlaceThickness && l > tileTypeToPlaceThickness && l < Main.maxTilesY - tileTypeToPlaceThickness && Main.tile[k, l].HasTile && Main.tile[k, l].TileType == tileTypeToPlace)
                            {
                                placeBulb = false;
                                break;
                            }
                        }
                    }

                    if (placeBulb)
                    {
                        if (i < tileTypeToPlaceThickness || i > Main.maxTilesX - tileTypeToPlaceThickness || j2 < tileTypeToPlaceThickness || j2 > Main.maxTilesY - tileTypeToPlaceThickness)
                            return;

                        bool placeTile = true;
                        for (int i2 = i - 2; i2 < i + 3; i2++)
                        {
                            for (int j3 = j2 - 4; j3 < j2 + 1; j3++)
                            {
                                if (Main.tile[i2, j3] == null)
                                    return;

                                if (Main.tile[i2, j3].HasTile)
                                    placeTile = false;
                            }

                            if (Main.tile[i2, j2 + 1] == null)
                                return;

                            if (!WorldGen.SolidTile2(i2, j2 + 1))
                                placeTile = false;
                        }

                        if (placeTile)
                        {
                            int num = 36;
                            for (int k = -4; k <= 0; k++)
                            {
                                short frameY = (short)((4 + k) * 18);
                                Main.tile[i - 2, j2 + k].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[i - 2, j2 + k].TileFrameY = frameY;
                                Main.tile[i - 2, j2 + k].TileFrameX = (short)(num - 36);
                                Main.tile[i - 2, j2 + k].TileType = tileTypeToPlace;
                                WorldGen.SquareTileFrame(i - 2, j2 + k);
                                Main.tile[i - 1, j2 + k].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[i - 1, j2 + k].TileFrameY = frameY;
                                Main.tile[i - 1, j2 + k].TileFrameX = (short)(num - 18);
                                Main.tile[i - 1, j2 + k].TileType = tileTypeToPlace;
                                WorldGen.SquareTileFrame(i - 1, j2 + k);
                                Main.tile[i, j2 + k].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[i, j2 + k].TileFrameY = frameY;
                                Main.tile[i, j2 + k].TileFrameX = (short)num;
                                Main.tile[i, j2 + k].TileType = tileTypeToPlace;
                                WorldGen.SquareTileFrame(i, j2 + k);
                                Main.tile[i + 1, j2 + k].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[i + 1, j2 + k].TileFrameY = frameY;
                                Main.tile[i + 1, j2 + k].TileFrameX = (short)(num + 18);
                                Main.tile[i + 1, j2 + k].TileType = tileTypeToPlace;
                                WorldGen.SquareTileFrame(i + 1, j2 + k);
                                Main.tile[i + 2, j2 + k].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[i + 2, j2 + k].TileFrameY = frameY;
                                Main.tile[i + 2, j2 + k].TileFrameX = (short)(num + 36);
                                Main.tile[i + 2, j2 + k].TileType = tileTypeToPlace;
                                WorldGen.SquareTileFrame(i + 2, j2 + k);
                            }

                            if (Main.tile[i, j2].TileType == tileTypeToPlace && Main.netMode == NetmodeID.Server)
                                NetMessage.SendTileSquare(-1, i, j2, tileTypeToPlaceThickness, tileTypeToPlaceThickness);
                        }
                    }
                }
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
