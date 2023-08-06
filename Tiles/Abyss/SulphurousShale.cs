//using CalamityMod.Tiles.Abyss.AbyssAmbient;
using CalamityMod.Tiles.Abyss.AbyssAmbient;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class SulphurousShale : ModTile
    {
        int animationFrameWidth = 234;
        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;
        public byte[,] thirdTileAdjacency;

        public static readonly SoundStyle MineSound = new("CalamityMod/Sounds/Custom/AbyssGravelMine", 3);

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithAbyss(Type);

            AddMapEntry(new Color(57, 44, 93));
            MineResist = 3f;
            MinPick = 65;
            HitSound = MineSound;
            DustType = 33;
            TileFraming.SetUpUniversalMerge(Type, TileID.Dirt, out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Stone, out secondTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<AbyssGravel>(), out thirdTileAdjacency);
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void RandomUpdate(int i, int j)
        {
            
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

            if (Main.tile[i, j + 1] != null && nearbyVineCount < 5 && j >= SulphurousSea.VineGrowTopLimit)
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
                                NetMessage.SendTileSquare(-1, num53, num54, 3, TileChangeType.None);
                        }
                        Main.tile[i, j].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                        Main.tile[i, j].Get<TileWallWireStateData>().IsHalfBlock = false;
                    }
                }
            }

            Tile tile = Main.tile[i, j];
            Tile up = Main.tile[i, j - 1];
            Tile up2 = Main.tile[i, j - 2];

            // Place sulphur tentacle corals
            if (WorldGen.genRand.Next(10) == 0 && !up.HasTile && !up2.HasTile && up.LiquidAmount > 0 && up2.LiquidAmount > 0 && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
            {
                up.TileType = (ushort)ModContent.TileType<SulphurTentacleCorals>();
                up.HasTile = true;
                up.TileFrameY = 0;

                // 18 different frames, choose a random one
                up.TileFrameX = (short)(WorldGen.genRand.Next(22) * 18);
                WorldGen.SquareTileFrame(i, j - 1, true);

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
            }
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int uniqueAnimationFrameX = 0;
            int xPos = i % 4;
            int yPos = j % 4;
            switch (xPos)
            {
                case 0:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
                case 1:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
                case 2:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
                case 3:
                    switch (yPos)
                    {
                        case 0:
                            uniqueAnimationFrameX = 1;
                            break;
                        case 1:
                            uniqueAnimationFrameX = 2;
                            break;
                        case 2:
                            uniqueAnimationFrameX = 0;
                            break;
                        case 3:
                            uniqueAnimationFrameX = 2;
                            break;
                        default:
                            uniqueAnimationFrameX = 2;
                            break;
                    }
                    break;
            }
            frameXOffset = uniqueAnimationFrameX * animationFrameWidth;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, thirdTileAdjacency, "CalamityMod/Tiles/Merges/AbyssGravelMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, "CalamityMod/Tiles/Merges/StoneMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/Merges/DirtMerge");
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, TileID.Dirt, out tileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.Stone, out secondTileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<AbyssGravel>(), out thirdTileAdjacency[i, j]);
            return true;
        }
    }
}
