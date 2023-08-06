using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class ViperVines : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoFail[Type] = true;
            AddMapEntry(new Color(0, 50, 0));
            HitSound = SoundID.Grass;
            DustType = 2;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
            if (WorldGen.loadSuccess)
            {
                Tile tileAbove = Framing.GetTileSafely(i, j - 1);
                if (!tileAbove.HasTile)
                {
                    WorldGen.KillTile(i, j);
                    return true;
                }
            }
			return true;
		}

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            // GIVE VINE ROPE IF SPECIAL VINE BOOK
            if (WorldGen.genRand.NextBool() && Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].cordage)
                Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i * 16 + 8f, j * 16 + 8f), ItemID.VineRope);

            if (Main.tile[i, j + 1] != null)
            {
                if (Main.tile[i, j + 1].HasTile)
                {
                    if (Main.tile[i, j + 1].TileType == ModContent.TileType<ViperVines>())
                    {
                        WorldGen.KillTile(i, j + 1, false, false, false);
                        if (!Main.tile[i, j + 1].HasTile && Main.netMode != NetmodeID.SinglePlayer)
                            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, (float)i, (float)j + 1, 0f, 0, 0, 0);
                    }
                }
            }
        }

        public override void RandomUpdate(int i, int j)
        {
            if (Main.tile[i, j + 1] != null)
            {
                if (!Main.tile[i, j + 1].HasTile && Main.tile[i, j + 1].TileType != (ushort)ModContent.TileType<ViperVines>())
                {
                    if (Main.tile[i, j + 1].LiquidAmount >= 128 && Main.tile[i, j + 1].LiquidType != LiquidID.Lava)
                    {
                        bool flag13 = false;
                        for (int num52 = j; num52 > j - 10; j--)
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
                            Main.tile[num53, num54].TileType = (ushort)ModContent.TileType<ViperVines>();
                            Main.tile[num53, num54].TileFrameX = (short)(WorldGen.genRand.Next(8) * 18);
                            Main.tile[num53, num54].TileFrameY = (short)(4 * 18);
                            Main.tile[num53, num54 - 1].TileFrameX = (short)(WorldGen.genRand.Next(12) * 18);
                            Main.tile[num53, num54 - 1].TileFrameY = (short)(WorldGen.genRand.Next(4) * 18);
                            Main.tile[num53, num54].Get<TileWallWireStateData>().HasTile = true;
                            WorldGen.SquareTileFrame(num53, num54, true);
                            WorldGen.SquareTileFrame(num53, num54 - 1, true);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, num53, num54, 3, TileChangeType.None);
                                NetMessage.SendTileSquare(-1, num53, num54 - 1, 3, TileChangeType.None);
                            }
                        }
                    }
                }
            }
        }
    }
}
