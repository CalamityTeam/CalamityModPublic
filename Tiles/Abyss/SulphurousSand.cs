using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    // Old, now deleted sub-variant of sulphurous sand that was only placed by players and did not create water.
    // It would create block swap issues with regular sulphurous sand and was rendered obsolete with the removal of this tile's water emission mechanic.
    // For compatibility reasons (including with schematics), however, that tile is converted into this one.
    [LegacyName("SulphurousSandNoWater")]
    public class SulphurousSand : ModTile
    {
        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;
        public byte[,] thirdTileAdjacency;
        public byte[,] fourthTileAdjacency;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithAbyss(Type);

            TileID.Sets.CanBeDugByShovel[Type] = true;

            DustType = 32;
            AddMapEntry(new Color(150, 100, 50));
            HitSound = SoundID.Dig;

            TileFraming.SetUpUniversalMerge(Type, TileID.Dirt, out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Stone, out secondTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<SulphurousSandstone>(), out thirdTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Sand, out fourthTileAdjacency);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
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
                        Main.tile[i, tileLocationY - 2].LiquidAmount == 255 && !Main.tile[i, tileLocationY - 2].HasTile && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(new EntitySource_WorldEvent(), (float)(i * 16 + 16), (float)(tileLocationY * 16 + 16), 0f, -0.1f, ModContent.ProjectileType<SulphuricAcidBubble>(), 0, 2f, Main.myPlayer);
                    }

                    if (i < 250 || i > Main.maxTilesX - 250)
                    {
                        if (Main.rand.NextBool(400))
                        {
                            if (Main.tile[i, tileLocationY].LiquidAmount == 255)
                            {
                                int ambientObjectDetectRadius = 7;
                                int ambientObjectMax = 6;
                                int ambientObjectAmt = 0;
                                for (int l = i - ambientObjectDetectRadius; l <= i + ambientObjectDetectRadius; l++)
                                {
                                    for (int m = tileLocationY - ambientObjectDetectRadius; m <= tileLocationY + ambientObjectDetectRadius; m++)
                                    {
                                        if (Main.tile[l, m].HasTile && Main.tile[l, m].TileType == 81)
                                            ambientObjectAmt++;
                                    }
                                }
                                if (ambientObjectAmt < ambientObjectMax && Main.tile[i, tileLocationY - 1].LiquidAmount == 255 &&
                                    Main.tile[i, tileLocationY - 2].LiquidAmount == 255 && Main.tile[i, tileLocationY - 3].LiquidAmount == 255 &&
                                    Main.tile[i, tileLocationY - 4].LiquidAmount == 255)
                                {
                                    WorldGen.PlaceTile(i, tileLocationY, 81, true, false, -1, 0);
                                    if (Main.netMode == NetmodeID.Server && Main.tile[i, tileLocationY].HasTile)
                                        NetMessage.SendTileSquare(-1, i, tileLocationY, 1, TileChangeType.None);
                                }
                            }
                            else if (Main.tile[i, tileLocationY].LiquidAmount == 0)
                            {
                                int ambientObjectDetectRadius = 7;
                                int ambientObjectMax = 6;
                                int ambientObjectAmt = 0;
                                for (int l = i - ambientObjectDetectRadius; l <= i + ambientObjectDetectRadius; l++)
                                {
                                    for (int m = tileLocationY - ambientObjectDetectRadius; m <= tileLocationY + ambientObjectDetectRadius; m++)
                                    {
                                        if (Main.tile[l, m].HasTile && Main.tile[l, m].TileType == 324)
                                            ambientObjectAmt++;
                                    }
                                }
                                if (ambientObjectAmt < ambientObjectMax)
                                {
                                    WorldGen.PlaceTile(i, tileLocationY, 324, true, false, -1, Main.rand.Next(2));
                                    if (Main.netMode == NetmodeID.Server && Main.tile[i, tileLocationY].HasTile)
                                        NetMessage.SendTileSquare(-1, i, tileLocationY, 1, TileChangeType.None);
                                }
                            }
                        }
                    }
                }
            }

            int vineLength = WorldGen.genRand.Next((int)Main.rockLayer, (int)(Main.rockLayer + (double)Main.maxTilesY * 0.143));
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

            if (Main.tile[i, j + 1] != null && nearbyVineCount < 5 && j >= SulphurousSea.VineGrowTopLimit)
            {
                if (!Main.tile[i, j + 1].HasTile && Main.tile[i, j + 1].TileType != (ushort)ModContent.TileType<SulphurousVines>())
                {
                    if (Main.tile[i, j + 1].LiquidAmount == 255 &&
                        Main.tile[i, j + 1].LiquidType != LiquidID.Lava)
                    {
                        bool canGrowVine = false;
                        for (int k = vineLength; k > vineLength - 10; k--)
                        {
                            if (Main.tile[i, k].BottomSlope)
                            {
                                canGrowVine = false;
                                break;
                            }
                            if (Main.tile[i, k].HasTile && !Main.tile[i, k].BottomSlope)
                            {
                                canGrowVine = true;
                                break;
                            }
                        }
                        if (canGrowVine)
                        {
                            int vineX = i;
                            int vineY = j + 1;
                            Main.tile[vineX, vineY].TileType = (ushort)ModContent.TileType<SulphurousVines>();
                            Main.tile[vineX, vineY].Get<TileWallWireStateData>().HasTile = true;
                            WorldGen.SquareTileFrame(vineX, vineY, true);
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendTileSquare(-1, vineX, vineY, 3, TileChangeType.None);
                        }
                        Main.tile[i, j].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                        Main.tile[i, j].Get<TileWallWireStateData>().IsHalfBlock = false;
                    }
                }
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {

            TileFraming.DrawUniversalMergeFrames(i, j, fourthTileAdjacency, "CalamityMod/Tiles/Merges/SandMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, thirdTileAdjacency, "CalamityMod/Tiles/Merges/SulphurousSandstoneMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, "CalamityMod/Tiles/Merges/StoneMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/Merges/DirtMerge");
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, TileID.Dirt, out tileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.Stone, out secondTileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<SulphurousSandstone>(), out thirdTileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.Sand, out fourthTileAdjacency[i, j]);
            return true;
        }
    }
}
