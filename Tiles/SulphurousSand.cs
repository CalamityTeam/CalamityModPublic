using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
	public class SulphurousSand : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			dustType = 32;
			drop = mod.ItemType("SulphurousSand");
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Sulphurous Sand");
			AddMapEntry(new Color(150, 100, 50), name);
			mineResist = 1f;
			minPick = 55;
			soundType = 0;
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
						if (!Main.tile[i - 1, j].active())
						{
							if (Main.tile[i - 1, j].liquid <= 128)
							{
								Main.tile[i - 1, j].liquid = 255;
								Main.tile[i - 1, j].lava(false);
							}
						}
					}
					if (Main.tile[i - 2, j] != null)
					{
						if (!Main.tile[i - 2, j].active())
						{
							if (Main.tile[i - 2, j].liquid <= 128)
							{
								Main.tile[i - 2, j].liquid = 255;
								Main.tile[i - 2, j].lava(false);
							}
						}
					}
					if (Main.tile[i - 3, j] != null)
					{
						if (!Main.tile[i - 3, j].active())
						{
							if (Main.tile[i - 3, j].liquid <= 128)
							{
								Main.tile[i - 3, j].liquid = 255;
								Main.tile[i - 3, j].lava(false);
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
						if (!Main.tile[i + 1, j].active())
						{
							if (Main.tile[i + 1, j].liquid <= 128)
							{
								Main.tile[i + 1, j].liquid = 255;
								Main.tile[i + 1, j].lava(false);
							}
						}
					}
					if (Main.tile[i + 2, j] != null)
					{
						if (!Main.tile[i + 2, j].active())
						{
							if (Main.tile[i + 2, j].liquid <= 128)
							{
								Main.tile[i + 2, j].liquid = 255;
								Main.tile[i + 2, j].lava(false);
							}
						}
					}
					if (Main.tile[i + 3, j] != null)
					{
						if (!Main.tile[i + 3, j].active())
						{
							if (Main.tile[i + 3, j].liquid <= 128)
							{
								Main.tile[i + 3, j].liquid = 255;
								Main.tile[i + 3, j].lava(false);
							}
						}
					}
				}
			}
		}

		public override void RandomUpdate(int i, int j)
		{
			int tileLocationY = j - 1;
			if (Main.tile[i, tileLocationY] != null)
			{
				if (!Main.tile[i, tileLocationY].active())
				{
					if (Main.tile[i, tileLocationY].liquid == 255 && Main.tile[i, tileLocationY - 1].liquid == 255 &&
						Main.tile[i, tileLocationY - 2].liquid == 255 && Main.netMode != 1)
					{
						Projectile.NewProjectile((float)(i * 16 + 16), (float)(tileLocationY * 16 + 16), 0f, -0.1f, mod.ProjectileType("SulphuricAcidCannon"), 0, 2f, Main.myPlayer, 0f, 0f);
					}
					if (i < 250 || i > Main.maxTilesX - 250)
					{
						if (Main.rand.Next(400) == 0)
						{
							if (Main.tile[i, tileLocationY].liquid == 255)
							{
								int num13 = 7;
								int num14 = 6;
								int num15 = 0;
								for (int l = i - num13; l <= i + num13; l++)
								{
									for (int m = tileLocationY - num13; m <= tileLocationY + num13; m++)
									{
										if (Main.tile[l, m].active() && Main.tile[l, m].type == 81)
										{
											num15++;
										}
									}
								}
								if (num15 < num14 && Main.tile[i, tileLocationY - 1].liquid == 255 &&
									Main.tile[i, tileLocationY - 2].liquid == 255 && Main.tile[i, tileLocationY - 3].liquid == 255 &&
									Main.tile[i, tileLocationY - 4].liquid == 255)
								{
									WorldGen.PlaceTile(i, tileLocationY, 81, true, false, -1, 0);
									if (Main.netMode == 2 && Main.tile[i, tileLocationY].active())
									{
										NetMessage.SendTileSquare(-1, i, tileLocationY, 1, TileChangeType.None);
									}
								}
							}
							else if (Main.tile[i, tileLocationY].liquid == 0)
							{
								int num13 = 7;
								int num14 = 6;
								int num15 = 0;
								for (int l = i - num13; l <= i + num13; l++)
								{
									for (int m = tileLocationY - num13; m <= tileLocationY + num13; m++)
									{
										if (Main.tile[l, m].active() && Main.tile[l, m].type == 324)
										{
											num15++;
										}
									}
								}
								if (num15 < num14)
								{
									WorldGen.PlaceTile(i, tileLocationY, 324, true, false, -1, Main.rand.Next(2));
									if (Main.netMode == 2 && Main.tile[i, tileLocationY].active())
									{
										NetMessage.SendTileSquare(-1, i, tileLocationY, 1, TileChangeType.None);
									}
								}
							}
						}
					}
				}
			}
		}
	}
}