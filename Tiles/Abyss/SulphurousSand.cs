
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Enemy;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class SulphurousSand : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithAbyss(Type);

            DustType = 32;
            ItemDrop = ModContent.ItemType<Items.Placeables.SulphurousSand>();
            AddMapEntry(new Color(150, 100, 50));
            MineResist = 1f;
            SoundType = SoundID.Dig;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (i < 250 && i > 150)
            {
                if (!closer)
                {
                    if (Main.tile[i - 1, j] != null)
                    {
                        if (!Main.tile[i - 1, j].HasTile)
                        {
                            if (Main.tile[i - 1, j].LiquidAmount <= 128)
                            {
                                Main.tile[i - 1, j].LiquidAmount = 255;
                                Main.tile[i - 1, j].Get<LiquidData>().LiquidType = LiquidID.Water;
                            }
                        }
                    }
                    if (Main.tile[i - 2, j] != null)
                    {
                        if (!Main.tile[i - 2, j].HasTile)
                        {
                            if (Main.tile[i - 2, j].LiquidAmount <= 128)
                            {
                                Main.tile[i - 2, j].LiquidAmount = 255;
                                Main.tile[i - 2, j].Get<LiquidData>().LiquidType = LiquidID.Water;
                            }
                        }
                    }
                    if (Main.tile[i - 3, j] != null)
                    {
                        if (!Main.tile[i - 3, j].HasTile)
                        {
                            if (Main.tile[i - 3, j].LiquidAmount <= 128)
                            {
                                Main.tile[i - 3, j].LiquidAmount = 255;
                                Main.tile[i - 3, j].Get<LiquidData>().LiquidType = LiquidID.Water;
                            }
                        }
                    }
                }
            }
            else if (i > Main.maxTilesX - 250 && i < Main.maxTilesX - 150)
            {
                if (!closer)
                {
                    if (Main.tile[i + 1, j] != null)
                    {
                        if (!Main.tile[i + 1, j].HasTile)
                        {
                            if (Main.tile[i + 1, j].LiquidAmount <= 128)
                            {
                                Main.tile[i + 1, j].LiquidAmount = 255;
                                Main.tile[i + 1, j].Get<LiquidData>().LiquidType = LiquidID.Water;
                            }
                        }
                    }
                    if (Main.tile[i + 2, j] != null)
                    {
                        if (!Main.tile[i + 2, j].HasTile)
                        {
                            if (Main.tile[i + 2, j].LiquidAmount <= 128)
                            {
                                Main.tile[i + 2, j].LiquidAmount = 255;
                                Main.tile[i + 2, j].Get<LiquidData>().LiquidType = LiquidID.Water;
                            }
                        }
                    }
                    if (Main.tile[i + 3, j] != null)
                    {
                        if (!Main.tile[i + 3, j].HasTile)
                        {
                            if (Main.tile[i + 3, j].LiquidAmount <= 128)
                            {
                                Main.tile[i + 3, j].LiquidAmount = 255;
                                Main.tile[i + 3, j].Get<LiquidData>().LiquidType = LiquidID.Water;
                            }
                        }
                    }
                }
            }
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (CalamityUtils.ParanoidTileRetrieval(i, j + 1).HasTile &&
                CalamityUtils.ParanoidTileRetrieval(i, j + 1).TileType == (ushort)ModContent.TileType<SulphurousVines>())
            {
                WorldGen.KillTile(i, j + 1);
            }
        }
        public override void RandomUpdate(int i, int j)
        {
            int tileLocationY = j - 1;
            if (Main.tile[i, tileLocationY] != null)
            {
                if (!Main.tile[i, tileLocationY].HasTile)
                {
                    if (!CalamityPlayer.areThereAnyDamnBosses && Main.tile[i, tileLocationY].LiquidAmount == 255 && Main.tile[i, tileLocationY - 1].LiquidAmount == 255 &&
                        Main.tile[i, tileLocationY - 2].LiquidAmount == 255 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(new EntitySource_WorldEvent(), (float)(i * 16 + 16), (float)(tileLocationY * 16 + 16), 0f, -0.1f, ModContent.ProjectileType<SulphuricAcidBubble>(), 0, 2f, Main.myPlayer, 0f, 0f);
                    }
                    if (i < 250 || i > Main.maxTilesX - 250)
                    {
                        if (Main.rand.NextBool(400))
                        {
                            if (Main.tile[i, tileLocationY].LiquidAmount == 255)
                            {
                                int num13 = 7;
                                int num14 = 6;
                                int num15 = 0;
                                for (int l = i - num13; l <= i + num13; l++)
                                {
                                    for (int m = tileLocationY - num13; m <= tileLocationY + num13; m++)
                                    {
                                        if (Main.tile[l, m].HasTile && Main.tile[l, m].TileType == 81)
                                        {
                                            num15++;
                                        }
                                    }
                                }
                                if (num15 < num14 && Main.tile[i, tileLocationY - 1].LiquidAmount == 255 &&
                                    Main.tile[i, tileLocationY - 2].LiquidAmount == 255 && Main.tile[i, tileLocationY - 3].LiquidAmount == 255 &&
                                    Main.tile[i, tileLocationY - 4].LiquidAmount == 255)
                                {
                                    WorldGen.PlaceTile(i, tileLocationY, 81, true, false, -1, 0);
                                    if (Main.netMode == NetmodeID.Server && Main.tile[i, tileLocationY].HasTile)
                                    {
                                        NetMessage.SendTileSquare(-1, i, tileLocationY, 1, TileChangeType.None);
                                    }
                                }
                            }
                            else if (Main.tile[i, tileLocationY].LiquidAmount == 0)
                            {
                                int num13 = 7;
                                int num14 = 6;
                                int num15 = 0;
                                for (int l = i - num13; l <= i + num13; l++)
                                {
                                    for (int m = tileLocationY - num13; m <= tileLocationY + num13; m++)
                                    {
                                        if (Main.tile[l, m].HasTile && Main.tile[l, m].TileType == 324)
                                        {
                                            num15++;
                                        }
                                    }
                                }
                                if (num15 < num14)
                                {
                                    WorldGen.PlaceTile(i, tileLocationY, 324, true, false, -1, Main.rand.Next(2));
                                    if (Main.netMode == NetmodeID.Server && Main.tile[i, tileLocationY].HasTile)
                                    {
                                        NetMessage.SendTileSquare(-1, i, tileLocationY, 1, TileChangeType.None);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            int num8 = WorldGen.genRand.Next((int)Main.rockLayer, (int)(Main.rockLayer + (double)Main.maxTilesY * 0.143));
            int nearbyVineCount = 0;
            for (int x = i - 15; x <= i + 15; x++)
            {
                for (int y = j - 15; y <= j + 15; y++)
                {
                    if (WorldGen.InWorld(x, y))
                    {
                        if (CalamityUtils.ParanoidTileRetrieval(x, y).HasTile &&
                            CalamityUtils.ParanoidTileRetrieval(x, y).TileType == (ushort)ModContent.TileType<SulphurousVines>())
                        {
                            nearbyVineCount++;
                        }
                    }
                }
            }
            if (Main.tile[i, j + 1] != null && nearbyVineCount < 5)
            {
                if (!Main.tile[i, j + 1].HasTile && Main.tile[i, j + 1].TileType != (ushort)ModContent.TileType<SulphurousVines>())
                {
                    if (Main.tile[i, j + 1].LiquidAmount == 255 &&
                        Main.tile[i, j + 1].LiquidType != LiquidID.Lava)
                    {
                        bool flag13 = false;
                        for (int num52 = num8; num52 > num8 - 10; num52--)
                        {
                            if (Main.tile[i, num52].BottomSlope)
                            {
                                flag13 = false;
                                break;
                            }
                            if (Main.tile[i, num52].HasTile && !Main.tile[i, num52].BottomSlope)
                            {
                                flag13 = true;
                                break;
                            }
                        }
                        if (flag13)
                        {
                            int num53 = i;
                            int num54 = j + 1;
                            Main.tile[num53, num54].TileType = (ushort)ModContent.TileType<SulphurousVines>();
                            Main.tile[num53, num54].Get<TileWallWireStateData>().HasTile = true;
                            WorldGen.SquareTileFrame(num53, num54, true);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, num53, num54, 3, TileChangeType.None);
                            }
                        }
                        Main.tile[i, j].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                        Main.tile[i, j].Get<TileWallWireStateData>().IsHalfBlock = false;
                    }
                }
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<SulphurousSandstone>(), false, false, false, false, resetFrame);
            return false;
        }
    }
}
