using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class Voidstone : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			drop = mod.ItemType("Voidstone");
            AddMapEntry(new Color(10, 10, 10));
            mineResist = 10f;
            minPick = 189;
            soundType = 21;
            dustType = 187;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer && j < Main.maxTilesY - 205)
            {
                if (Main.tile[i, j].liquid <= 0)
                {
                    Main.tile[i, j].liquid = 255;
                    Main.tile[i, j].lava(false);
                }
            }
        }

        public override void RandomUpdate(int i, int j)
        {
            if (NPC.downedPlantBoss || CalamityWorld.downedCalamitas)
            {
                int random = WorldGen.genRand.Next(4);
                if (random == 0)
                {
                    i++;
                }
                if (random == 1)
                {
                    i--;
                }
                if (random == 2)
                {
                    j++;
                }
				if (random == 3)
				{
					j--;
				}
                if (Main.tile[i, j] != null)
                {
                    if (!Main.tile[i, j].active())
                    {
                        if (Main.rand.Next(20) == 0 && !Main.tile[i, j].lava() && Main.tile[i, j].slope() == 0 && !Main.tile[i, j].halfBrick())
                        {
                            Main.tile[i, j].type = (ushort)mod.TileType("LumenylCrystals");
                            Main.tile[i, j].active(true);
							if (Main.tile[i, j + 1].active() && Main.tileSolid[Main.tile[i, j + 1].type] && Main.tile[i, j + 1].slope() == 0 && !Main.tile[i, j + 1].halfBrick())
							{
								Main.tile[i, j].frameY = (short)(0 * 18);
							}
							else if (Main.tile[i, j - 1].active() && Main.tileSolid[Main.tile[i, j - 1].type] && Main.tile[i, j - 1].slope() == 0 && !Main.tile[i, j - 1].halfBrick())
							{
								Main.tile[i, j].frameY = (short)(1 * 18);
							}
							else if (Main.tile[i + 1, j].active() && Main.tileSolid[Main.tile[i + 1, j].type] && Main.tile[i + 1, j].slope() == 0 && !Main.tile[i + 1, j].halfBrick())
							{
								Main.tile[i, j].frameY = (short)(2 * 18);
							}
							else if (Main.tile[i - 1, j].active() && Main.tileSolid[Main.tile[i - 1, j].type] && Main.tile[i - 1, j].slope() == 0 && !Main.tile[i - 1, j].halfBrick())
							{
								Main.tile[i, j].frameY = (short)(3 * 18);
							}
							Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(18) * 18);
							WorldGen.SquareTileFrame(i, j, true);
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
                            }
                        }
                    }
                }
            }
        }
    }
}