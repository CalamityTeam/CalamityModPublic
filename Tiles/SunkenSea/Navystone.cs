using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Tiles.SunkenSea
{
	public class Navystone : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			dustType = 96;
			drop = mod.ItemType("Navystone");
			ModTranslation name = CreateMapEntryName();
 			name.SetDefault("Navystone");
 			AddMapEntry(new Color(0, 50, 50), name);
			mineResist = 2f;
			minPick = 55;
			soundType = 21;
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
		{
			return CalamityWorld.downedDesertScourge;
		}

		public override bool CanExplode(int i, int j)
		{
			return CalamityWorld.downedDesertScourge;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

		public override void RandomUpdate(int i, int j)
		{
			if (Main.rand.Next(100) == 0)
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
						if (!Main.tile[i, j].lava() && Main.tile[i, j].slope() == 0 && !Main.tile[i, j].halfBrick())
						{
							Main.tile[i, j].type = (ushort)mod.TileType("SeaPrismCrystals");
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
