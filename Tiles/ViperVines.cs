using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class ViperVines : ModTile
	{
		public override void SetDefaults()
		{
            Main.tileCut[Type] = true;
			Main.tileBlockLight[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoFail[Type] = true;
            ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Viper Vines");
 			AddMapEntry(new Color(0, 50, 0), name);
            soundType = 6;
            dustType = 2;
		}

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void RandomUpdate(int i, int j)
        {
            int num8 = WorldGen.genRand.Next((int)Main.rockLayer, (int)(Main.rockLayer + (double)Main.maxTilesY * 0.143));
            if (Main.tile[i, j + 1] != null)
            {
                if (Main.tile[i, j + 1].active())
                {
                    if (Main.tile[i, j + 1].liquid == 255 &&
                        (Main.tile[i, j + 1].wall == (byte)mod.WallType("MossyGravelWall") || 
                        Main.tile[i, j + 1].wall == (byte)mod.WallType("AbyssGravelWall")) &&
                        !Main.tile[i, j + 1].lava())
                    {
                        bool flag13 = false;
                        for (int num52 = num8; num52 > num8 - 10; num52--)
                        {
                            if (Main.tile[i, num52].bottomSlope())
                            {
                                flag13 = false;
                                break;
                            }
                            if (Main.tile[i, num52].active() && !Main.tile[i, num52].bottomSlope())
                            {
                                flag13 = true;
                                break;
                            }
                        }
                        if (flag13)
                        {
                            int num53 = i;
                            int num54 = j + 1;
                            Main.tile[num53, num54].type = (ushort)mod.TileType("ViperVines");
                            Main.tile[num53, num54].active(true);
                            WorldGen.SquareTileFrame(num53, num54, true);
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendTileSquare(-1, num53, num54, 3, TileChangeType.None);
                            }
                        }
                    }
                }
            }
        }
    }
}