using CalamityMod.Tiles;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.FurnitureAbyss;
using CalamityMod.Tiles.FurnitureAshen;
using CalamityMod.Tiles.FurnitureEutrophic;
using CalamityMod.Tiles.FurnitureOccult;
using CalamityMod.Tiles.FurnitureProfaned;
using CalamityMod.Tiles.FurnitureVoid;
using CalamityMod.Tiles.Ores;
using CalamityMod.Tiles.SunkenSea;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod
{
	public static partial class CalamityUtils
	{
		#region Generic Utilities
		public static string GetMapChestName(string baseName, int x, int y)
		{
			// Bounds check.
			if (!WorldGen.InWorld(x, y, 2))
				return baseName;

			// Tile null check.
			Tile tile = Main.tile[x, y];
			if (tile is null)
				return baseName;

			int left = x;
			int top = y;
			if (tile.frameX % 36 != 0)
				left--;
			if (tile.frameY != 0)
				top--;

			int chest = Chest.FindChest(left, top);

			// Valid chest index check.
			if (chest < 0)
				return baseName;

			string name = baseName;

			// Concatenate the chest's custom name if it has one.
			if (!string.IsNullOrEmpty(Main.chest[chest].name))
				name += $": {Main.chest[chest].name}";

			return name;
		}

		public static void SafeSquareTileFrame(int x, int y, bool resetFrame = true)
		{
			if (Main.tile[x, y] is null)
				return;

			for (int xIter = x - 1; xIter <= x + 1; ++xIter)
			{
				if (xIter < 0 || xIter >= Main.maxTilesX)
					continue;
				for (int yIter = y - 1; yIter <= y + 1; yIter++)
				{
					if (yIter < 0 || yIter >= Main.maxTilesY)
						continue;
					if (xIter == x && yIter == y)
					{
						WorldGen.TileFrame(x, y, resetFrame, false);
					}
					else
					{
						WorldGen.TileFrame(xIter, yIter, false, false);
					}
				}
			}
		}

		public static void LightHitWire(int type, int i, int j, int tileX, int tileY)
		{
			int x = i - Main.tile[i, j].frameX / 18 % tileX;
			int y = j - Main.tile[i, j].frameY / 18 % tileY;
			for (int l = x; l < x + tileX; l++)
			{
				for (int m = y; m < y + tileY; m++)
				{
					if (Main.tile[l, m] == null)
					{
						Main.tile[l, m] = new Tile();
					}
					if (Main.tile[l, m].active() && Main.tile[l, m].type == type)
					{
						if (Main.tile[l, m].frameX < (18 * tileX))
						{
							Main.tile[l, m].frameX += (short)(18 * tileX);
						}
						else
						{
							Main.tile[l, m].frameX -= (short)(18 * tileX);
						}
					}
				}
			}
			if (Wiring.running)
			{
				for (int k = 0; k < tileX; k++)
				{
					for (int l = 0; l < tileY; l++)
					{
						Wiring.SkipWire(x + k, y + l);
					}
				}
			}
		}

		public static void DrawFlameEffect(Texture2D flameTexture, int i, int j, int offsetX = 0, int offsetY = 0)
		{
			Tile tile = Main.tile[i, j];
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			int width = 16;
			int height = 16;
			int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;

			ulong num190 = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);

			for (int c = 0; c < 7; c++)
			{
				float shakeX = Utils.RandomInt(ref num190, -10, 11) * 0.15f;
				float shakeY = Utils.RandomInt(ref num190, -10, 1) * 0.35f;
				Main.spriteBatch.Draw(flameTexture, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + shakeX, j * 16 - (int)Main.screenPosition.Y + shakeY + yOffset) + zero, new Rectangle(tile.frameX + offsetX, tile.frameY + offsetY, width, height), new Color(100, 100, 100, 0), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
			}
		}

		public static void DrawStaticFlameEffect(Texture2D flameTexture, int i, int j, int offsetX = 0, int offsetY = 0)
		{
			int xPos = Main.tile[i, j].frameX;
			int yPos = Main.tile[i, j].frameY;
			Color drawColour = new Color(100, 100, 100, 0);
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
			Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;
			for (int x = -1; x < 2; x++)
			{
				for (int y = -1; y < 2; y++)
				{
					Vector2 flameOffset = new Vector2(x, y).SafeNormalize(Vector2.Zero);
					flameOffset *= 1.5f;
					Main.spriteBatch.Draw(flameTexture, drawOffset + flameOffset, new Rectangle?(new Rectangle(xPos + offsetX, yPos + offsetY, 18, 18)), drawColour, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
				}
			}
		}

		public static void DrawFlameSparks(int dustType, int rarity, int i, int j)
		{
			if (!Main.gamePaused && Main.instance.IsActive && (!Lighting.UpdateEveryFrame || Main.rand.NextBool(4)))
			{
				if (Main.rand.NextBool(rarity))
				{
					int dust = Dust.NewDust(new Vector2(i * 16 + 4, j * 16 + 2), 4, 4, dustType, 0f, 0f, 100, default, 1f);
					if (Main.rand.Next(3) != 0)
					{
						Main.dust[dust].noGravity = true;
					}
					Main.dust[dust].velocity *= 0.3f;
					Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1.5f;
				}
			}
		}

		public static void DrawItemFlame(Texture2D flameTexture, Item item)
		{
			int width = flameTexture.Width;
			int height = flameTexture.Height;
			for (int c = 0; c < 7; c++)
			{
				float shakeX = Main.rand.Next(-10, 11) * 0.15f;
				float shakeY = Main.rand.Next(-10, 1) * 0.35f;
				Main.spriteBatch.Draw(flameTexture, new Vector2(item.position.X - Main.screenPosition.X + item.width * 0.5f + shakeX, item.position.Y - Main.screenPosition.Y + item.height - flameTexture.Height * 0.5f + 2f + shakeY), new Rectangle(0, 0, width, height), new Color(100, 100, 100, 0), 0f, default, 1f, SpriteEffects.None, 0f);
			}
		}
		public static Tile ParanoidTileRetrieval(int x, int y)
		{
			if (!WorldGen.InWorld(x, y))
				return new Tile();
			Tile tile = Main.tile[x, y];
			if (tile is null)
			{
				tile = new Tile();
				Main.tile[x, y] = tile;
			}
			return tile;
		}
		public static bool TileSelectionSolid(int x, int y, int width, int height)
		{
			for (int i = x; i != x + width; i += Math.Sign(width))
			{
				for (int j = y; y != y + height; j += Math.Sign(height))
				{
					if (!WorldGen.InWorld(i, j))
						return false;
					if (!WorldGen.SolidTile(Framing.GetTileSafely(i, j)))
						return false;
				}
			}
			return true;
		}
		public static bool TileSelectionSolidSquare(int x, int y, int width, int height)
		{
			for (int i = x - width; i != x + width; i += Math.Sign(width))
			{
				for (int j = y - height; y != y + height; j += Math.Sign(height))
				{
					if (!WorldGen.InWorld(i, j))
						return false;
					if (!WorldGen.SolidTile(Framing.GetTileSafely(i, j)))
						return false;
				}
			}
			return true;
		}
		public static bool TileActiveAndOfType(int x, int y, int type)
		{
			return ParanoidTileRetrieval(x, y).active() && ParanoidTileRetrieval(x, y).type == type;
		}

		/// <summary>
		/// Sets the mergeability state of two tiles. By default, enables tile merging.
		/// </summary>
		/// <param name="type1">The first tile type which should merge (or not).</param>
		/// <param name="type2">The second tile type which should merge (or not).</param>
		/// <param name="merge">The mergeability state of the tiles. Defaults to true if omitted.</param>
		public static void SetMerge(int type1, int type2, bool merge = true)
		{
			if (type1 != type2)
			{
				Main.tileMerge[type1][type2] = merge;
				Main.tileMerge[type2][type1] = merge;
			}
		}

		/// <summary>
		/// Makes the first tile type argument merge with all the other tile type arguments. Also accepts arrays.
		/// </summary>
		/// <param name="myType">The tile whose merging properties will be set.</param>
		/// <param name="otherTypes">Every tile that should be merged with.</param>
		public static void MergeWithSet(int myType, params int[] otherTypes)
		{
			for (int i = 0; i < otherTypes.Length; ++i)
				SetMerge(myType, otherTypes[i]);
		}

		/// <summary>
		/// Makes the specified tile merge with the most common types of tiles found in world generation.<br></br>
		/// Notably excludes Ice.
		/// </summary>
		/// <param name="type">The tile whose merging properties will be set.</param>
		public static void MergeWithGeneral(int type) => MergeWithSet(type, new int[] {
			// Soils
			TileID.Dirt,
			TileID.Mud,
			TileID.ClayBlock,
			// Stones
			TileID.Stone,
			TileID.Ebonstone,
			TileID.Crimstone,
			TileID.Pearlstone,
			// Sands
			TileID.Sand,
			TileID.Ebonsand,
			TileID.Crimsand,
			TileID.Pearlsand,
			// Snows
			TileID.SnowBlock,
			// Calamity Tiles
			TileType<AstralDirt>(),
			TileType<AstralClay>(),
			TileType<AstralStone>(),
			TileType<AstralSand>(),
			TileType<AstralSnow>(),
			TileType<Navystone>(),
			TileType<EutrophicSand>(),
			TileType<AbyssGravel>(),
			TileType<Voidstone>(),
		});

		/// <summary>
		/// Makes the specified tile merge with all ores, vanilla and Calamity. Particularly useful for stone blocks.
		/// </summary>
		/// <param name="type">The tile whose merging properties will be set.</param>
		public static void MergeWithOres(int type) => MergeWithSet(type, new int[] {
			// Vanilla Ores
			TileID.Copper,
			TileID.Tin,
			TileID.Iron,
			TileID.Lead,
			TileID.Silver,
			TileID.Tungsten,
			TileID.Gold,
			TileID.Platinum,
			TileID.Demonite,
			TileID.Crimtane,
			TileID.Cobalt,
			TileID.Palladium,
			TileID.Mythril,
			TileID.Orichalcum,
			TileID.Adamantite,
			TileID.Titanium,
			TileID.LunarOre,
			// Calamity Ores
			TileType<AerialiteOre>(),
			TileType<CryonicOre>(),
			TileType<PerennialOre>(),
			TileType<CharredOre>(),
			TileType<ChaoticOre>(),
			TileType<AstralOre>(),
			TileType<UelibloomOre>(),
			TileType<AuricOre>(),
		});

		/// <summary>
		/// Makes the specified tile merge with all types of desert tiles, including the Calamity Sunken Sea.
		/// </summary>
		/// <param name="type">The tile whose merging properties will be set.</param>
		public static void MergeWithDesert(int type) => MergeWithSet(type, new int[] {
			// Sands
			TileID.Sand,
			TileID.Ebonsand,
			TileID.Crimsand,
			TileID.Pearlsand,
			// Hardened Sands
			TileID.HardenedSand,
			TileID.CorruptHardenedSand,
			TileID.CrimsonHardenedSand,
			TileID.HallowHardenedSand,
			// Sandstones
			TileID.Sandstone,
			TileID.CorruptSandstone,
			TileID.CrimsonSandstone,
			TileID.HallowSandstone,
			// Miscellaneous Desert Tiles
			TileID.FossilOre,
			TileID.DesertFossil,
			// Astral Desert
			TileType<AstralSand>(),
			TileType<HardenedAstralSand>(),
			TileType<AstralSandstone>(),
			TileType<AstralFossil>(),
			// Sunken Sea
			TileType<EutrophicSand>(),
			TileType<Navystone>(),
			TileType<SeaPrism>(),
		});

		/// <summary>
		/// Makes the specified tile merge with all types of snow and ice tiles.
		/// </summary>
		/// <param name="type">The tile whose merging properties will be set.</param>
		public static void MergeWithSnow(int type) => MergeWithSet(type, new int[] {
			// Snows
			TileID.SnowBlock,
			// Ices
			TileID.IceBlock,
			TileID.CorruptIce,
			TileID.FleshIce,
			TileID.HallowedIce,
			// Astral Snow
			TileType<AstralIce>(),
			TileType<AstralSnow>(),
			TileType<AstralSilt>(),
		});

		/// <summary>
		/// Makes the specified tile merge with all tiles which generate in hell. Does not include Charred Ore.
		/// </summary>
		/// <param name="type">The tile whose merging properties will be set.</param>
		public static void MergeWithHell(int type) => MergeWithSet(type, new int[] {
			TileID.Ash,
			TileID.Hellstone,
			TileID.ObsidianBrick,
			TileID.HellstoneBrick,
			TileType<BrimstoneSlag>(),
		});

		/// <summary>
		/// Makes the specified tile merge with all tiles which generate in the Abyss or the Sulphurous Sea. Includes Chaotic Ore.
		/// </summary>
		/// <param name="type">The tile whose merging properties will be set.</param>
		public static void MergeWithAbyss(int type) => MergeWithSet(type, new int[] {
			// Sulphurous Sea
			TileType<SulphurousSand>(),
			TileType<SulphurousSandstone>(),
			// Abyss
			TileType<AbyssGravel>(),
			TileType<Voidstone>(),
			TileType<PlantyMush>(),
			TileType<Tenebris>(),
			TileType<ChaoticOre>(),
		});

		/// <summary>
		/// Makes the tile merge with all the tile types that generate within various types of astral tiles
		/// </summary>
		/// <param name="type"></param>
		public static void MergeAstralTiles(int type)
		{
			//Astral
			SetMerge(type, TileType<AstralDirt>());
			SetMerge(type, TileType<AstralStone>());
			SetMerge(type, TileType<AstralMonolith>());
			SetMerge(type, TileType<AstralClay>());
			//Astral Desert
			SetMerge(type, TileType<AstralSand>());
			SetMerge(type, TileType<HardenedAstralSand>());
			SetMerge(type, TileType<AstralSandstone>());
			SetMerge(type, TileType<AstralFossil>());
			//Astral Snow
			SetMerge(type, TileType<AstralIce>());
			SetMerge(type, TileType<AstralSnow>());
		}

		/// <summary>
		/// Makes the tile merge with all the decorative 'smooth' tiles
		/// </summary>
		/// <param name="type"></param>
		public static void MergeSmoothTiles(int type)
		{
			//Vanilla
			SetMerge(type, TileID.MarbleBlock);
			SetMerge(type, TileID.GraniteBlock);
			//Calam
			SetMerge(type, TileType<SmoothNavystone>());
			SetMerge(type, TileType<SmoothBrimstoneSlag>());
			SetMerge(type, TileType<SmoothAbyssGravel>());
			SetMerge(type, TileType<SmoothVoidstone>());
		}

		/// <summary>
		/// Makes the tile merge with other mergable decorative tiles
		/// </summary>
		/// <param name="type"></param>
		public static void MergeDecorativeTiles(int type)
		{
			//Vanilla decor
			Main.tileBrick[type] = true;
			//Calam
			SetMerge(type, TileType<CryonicBrick>());
			SetMerge(type, TileType<PerennialBrick>());
			SetMerge(type, TileType<UelibloomBrick>());
			SetMerge(type, TileType<OccultStone>());
			SetMerge(type, TileType<ProfanedSlab>());
			SetMerge(type, TileType<RunicProfanedBrick>());
			SetMerge(type, TileType<AshenSlab>());
			SetMerge(type, TileType<VoidstoneSlab>());
		}
		#endregion

		#region Furniture Interaction
		public static void RightClickBreak(int i, int j)
		{
			if (Main.tile[i, j] != null && Main.tile[i, j].active())
			{
				WorldGen.KillTile(i, j, false, false, false);
				if (!Main.tile[i, j].active() && Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, (float)i, (float)j, 0f, 0, 0, 0);
				}
			}
		}

		public static bool BedRightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int spawnX = i - tile.frameX / 18;
			int spawnY = j + 2;
			spawnX += tile.frameX >= 72 ? 5 : 2;
			if (tile.frameY % 38 != 0)
			{
				spawnY--;
			}
			player.FindSpawn();
			if (player.SpawnX == spawnX && player.SpawnY == spawnY)
			{
				player.RemoveSpawn();
				Main.NewText("Spawn point removed!", 255, 240, 20, false);
			}
			else if (Player.CheckSpawn(spawnX, spawnY))
			{
				player.ChangeSpawn(spawnX, spawnY);
				Main.NewText("Spawn point set!", 255, 240, 20, false);
			}
			return true;
		}

		public static bool ChestRightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			Main.mouseRightRelease = false;
			int left = i;
			int top = j;
			if (tile.frameX % 36 != 0)
			{
				left--;
			}
			if (tile.frameY != 0)
			{
				top--;
			}
			if (player.sign >= 0)
			{
				Main.PlaySound(SoundID.MenuClose);
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = "";
			}
			if (Main.editChest)
			{
				Main.PlaySound(SoundID.MenuTick);
				Main.editChest = false;
				Main.npcChatText = "";
			}
			if (player.editedChestName)
			{
				NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
				player.editedChestName = false;
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				if (left == player.chestX && top == player.chestY && player.chest >= 0)
				{
					player.chest = -1;
					Recipe.FindRecipes();
					Main.PlaySound(SoundID.MenuClose);
				}
				else
				{
					NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, (float)top, 0f, 0f, 0, 0, 0);
					Main.stackSplit = 600;
				}
			}
			else
			{
				int chest = Chest.FindChest(left, top);
				if (chest >= 0)
				{
					Main.stackSplit = 600;
					if (chest == player.chest)
					{
						player.chest = -1;
						Main.PlaySound(SoundID.MenuClose);
					}
					else
					{
						player.chest = chest;
						Main.playerInventory = true;
						Main.recBigList = false;
						player.chestX = left;
						player.chestY = top;
						Main.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
					}
					Recipe.FindRecipes();
				}
			}
			return true;
		}

		public static void ChestMouseOver<T>(string chestName, int i, int j) where T : ModItem
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;
			if (tile.frameX % 36 != 0)
			{
				left--;
			}
			if (tile.frameY != 0)
			{
				top--;
			}
			int chest = Chest.FindChest(left, top);
			player.showItemIcon2 = -1;
			if (chest < 0)
			{
				player.showItemIconText = Language.GetTextValue("LegacyChestType.0");
			}
			else
			{
				player.showItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : chestName;
				if (player.showItemIconText == chestName)
				{
					player.showItemIcon2 = ItemType<T>();
					player.showItemIconText = "";
				}
			}
			player.noThrow = 2;
			player.showItemIcon = true;
		}

		public static void ChestMouseFar<T>(string name, int i, int j) where T : ModItem
		{
			ChestMouseOver<T>(name, i, j);
			Player player = Main.LocalPlayer;
			if (player.showItemIconText == "")
			{
				player.showItemIcon = false;
				player.showItemIcon2 = 0;
			}
		}

		public static bool ClockRightClick()
		{
			string text = "AM";

			// Get Terraria's current strange time variable
			double time = Main.time;

			// Correct for night time (which for some reason isn't just a different number) by adding 54000.
			if (!Main.dayTime)
				time += 54000D;

			// Divide by seconds in an hour
			time /= 3600D;

			// Terraria night starts at 7:30 PM, so offset accordingly
			time -= 19.5;

			// Offset time to ensure it is not negative, then change to PM if necessary
			if (time < 0D)
				time += 24D;
			if (time >= 12D)
				text = "PM";

			// Get the decimal (smaller than hours, so minutes) component of time.
			int intTime = (int)time;
			double deltaTime = time - intTime;

			// Convert decimal time into an exact number of minutes.
			deltaTime = (int)(deltaTime * 60D);

			string minuteText = deltaTime.ToString();

			// Ensure minutes has a leading zero
			if (deltaTime < 10D)
				minuteText = "0" + minuteText;

			// Convert from 24 to 12 hour time (PM already handled earlier)
			if (intTime > 12)
				intTime -= 12;
			// 12 AM is actually hour zero in 24 hour time
			if (intTime == 0)
				intTime = 12;

			// Create an overall time readout and send it to chat
			var newText = string.Concat("Time: ", intTime, ":", minuteText, " ", text);
			Main.NewText(newText, 255, 240, 20);
			return true;
		}

		public static bool DresserRightClick()
		{
			Player player = Main.LocalPlayer;
			if (Main.tile[Player.tileTargetX, Player.tileTargetY].frameY == 0)
			{
				Main.CancelClothesWindow(true);

				int left = (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].frameX / 18);
				left %= 3;
				left = Player.tileTargetX - left;
				int top = Player.tileTargetY - (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].frameY / 18);
				if (player.sign > -1)
				{
					Main.PlaySound(SoundID.MenuClose);
					player.sign = -1;
					Main.editSign = false;
					Main.npcChatText = string.Empty;
				}
				if (Main.editChest)
				{
					Main.PlaySound(SoundID.MenuTick);
					Main.editChest = false;
					Main.npcChatText = string.Empty;
				}
				if (player.editedChestName)
				{
					NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
					player.editedChestName = false;
				}
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					if (left == player.chestX && top == player.chestY && player.chest != -1)
					{
						player.chest = -1;
						Recipe.FindRecipes();
						Main.PlaySound(SoundID.MenuClose);
					}
					else
					{
						NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, (float)top, 0f, 0f, 0, 0, 0);
						Main.stackSplit = 600;
					}
					return true;
				}
				else
				{
					player.flyingPigChest = -1;
					int num213 = Chest.FindChest(left, top);
					if (num213 != -1)
					{
						Main.stackSplit = 600;
						if (num213 == player.chest)
						{
							player.chest = -1;
							Recipe.FindRecipes();
							Main.PlaySound(SoundID.MenuClose);
						}
						else if (num213 != player.chest && player.chest == -1)
						{
							player.chest = num213;
							Main.playerInventory = true;
							Main.recBigList = false;
							Main.PlaySound(SoundID.MenuOpen);
							player.chestX = left;
							player.chestY = top;
						}
						else
						{
							player.chest = num213;
							Main.playerInventory = true;
							Main.recBigList = false;
							Main.PlaySound(SoundID.MenuTick);
							player.chestX = left;
							player.chestY = top;
						}
						Recipe.FindRecipes();
						return true;
					}
				}
			}
			else
			{
				Main.playerInventory = false;
				player.chest = -1;
				Recipe.FindRecipes();
				Main.dresserX = Player.tileTargetX;
				Main.dresserY = Player.tileTargetY;
				Main.OpenClothesWindow();
				return true;
			}

			return false;
		}

		public static void DresserMouseFar<T>(string chestName) where T : ModItem
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
			int left = Player.tileTargetX;
			int top = Player.tileTargetY;
			left -= (int)(tile.frameX % 54 / 18);
			if (tile.frameY % 36 != 0)
			{
				top--;
			}
			int chestIndex = Chest.FindChest(left, top);
			player.showItemIcon2 = -1;
			if (chestIndex < 0)
			{
				player.showItemIconText = Language.GetTextValue("LegacyDresserType.0");
			}
			else
			{
				if (Main.chest[chestIndex].name != "")
				{
					player.showItemIconText = Main.chest[chestIndex].name;
				}
				else
				{
					player.showItemIconText = chestName;
				}
				if (player.showItemIconText == chestName)
				{
					player.showItemIcon2 = ItemType<T>();
					player.showItemIconText = "";
				}
			}
			player.noThrow = 2;
			player.showItemIcon = true;
			if (player.showItemIconText == "")
			{
				player.showItemIcon = false;
				player.showItemIcon2 = 0;
			}
		}

		public static void DresserMouseOver<T>(string chestName) where T : ModItem
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
			int left = Player.tileTargetX;
			int top = Player.tileTargetY;
			left -= (int)(tile.frameX % 54 / 18);
			if (tile.frameY % 36 != 0)
			{
				top--;
			}
			int chestIndex = Chest.FindChest(left, top);
			player.showItemIcon2 = -1;
			if (chestIndex < 0)
			{
				player.showItemIconText = Language.GetTextValue("LegacyDresserType.0");
			}
			else
			{
				if (Main.chest[chestIndex].name != "")
				{
					player.showItemIconText = Main.chest[chestIndex].name;
				}
				else
				{
					player.showItemIconText = chestName;
				}
				if (player.showItemIconText == chestName)
				{
					player.showItemIcon2 = ItemType<T>();
					player.showItemIconText = "";
				}
			}
			player.noThrow = 2;
			player.showItemIcon = true;
			if (Main.tile[Player.tileTargetX, Player.tileTargetY].frameY > 0)
			{
				player.showItemIcon2 = ItemID.FamiliarShirt;
			}
		}

		public static bool LockedChestRightClick(bool isLocked, int left, int top, int i, int j)
		{
			Player player = Main.LocalPlayer;

			// If the player right clicked the chest while editing a sign, finish that up
			if (player.sign >= 0)
			{
				Main.PlaySound(SoundID.MenuClose);
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = "";
			}

			// If the player right clicked the chest while editing a chest, finish that up
			if (Main.editChest)
			{
				Main.PlaySound(SoundID.MenuTick);
				Main.editChest = false;
				Main.npcChatText = "";
			}

			// If the player right clicked the chest after changing another chest's name, finish that up
			if (player.editedChestName)
			{
				NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
				player.editedChestName = false;
			}
			if (Main.netMode == NetmodeID.MultiplayerClient && !isLocked)
			{
				// Right clicking the chest you currently have open closes it. This counts as interaction.
				if (left == player.chestX && top == player.chestY && player.chest >= 0)
				{
					player.chest = -1;
					Recipe.FindRecipes();
					Main.PlaySound(SoundID.MenuClose);
				}

				// Right clicking this chest opens it if it's not already open. This counts as interaction.
				else
				{
					NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, (float)top, 0f, 0f, 0, 0, 0);
					Main.stackSplit = 600;
				}
				return true;
			}

			else
			{
				if (isLocked)
				{
					// If you right click the locked chest and you can unlock it, it unlocks itself but does not open. This counts as interaction.
					if (Chest.Unlock(left, top))
					{
						if (Main.netMode == NetmodeID.MultiplayerClient)
						{
							NetMessage.SendData(MessageID.Unlock, -1, -1, null, player.whoAmI, 1f, (float)left, (float)top);
						}
						return true;
					}
				}
				else
				{
					int chest = Chest.FindChest(left, top);
					if (chest >= 0)
					{
						Main.stackSplit = 600;

						// If you right click the same chest you already have open, it closes. This counts as interaction.
						if (chest == player.chest)
						{
							player.chest = -1;
							Main.PlaySound(SoundID.MenuClose);
						}

						// If you right click this chest when you have a different chest selected, that one closes and this one opens. This counts as interaction.
						else
						{
							player.chest = chest;
							Main.playerInventory = true;
							Main.recBigList = false;
							player.chestX = left;
							player.chestY = top;
							Main.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
						}

						Recipe.FindRecipes();
						return true;
					}
				}
			}

			// This only occurs when the chest is locked and cannot be unlocked. You did not interact with the chest.
			return false;
		}

		public static void LockedChestMouseOver<K, C>(string chestName, int i, int j)
			where K : ModItem where C : ModItem
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;
			if (tile.frameX % 36 != 0)
			{
				left--;
			}
			if (tile.frameY != 0)
			{
				top--;
			}
			int chest = Chest.FindChest(left, top);
			player.showItemIcon2 = -1;
			if (chest < 0)
			{
				player.showItemIconText = Language.GetTextValue("LegacyChestType.0");
			}
			else
			{
				player.showItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : chestName;
				if (player.showItemIconText == chestName)
				{
					player.showItemIcon2 = ItemType<C>();
					if (Main.tile[left, top].frameX / 36 == 1)
						player.showItemIcon2 = ItemType<K>();
					player.showItemIconText = "";
				}
			}
			player.noThrow = 2;
			player.showItemIcon = true;
		}

		public static void LockedChestMouseOverFar<K, C>(string chestName, int i, int j)
			where K : ModItem where C : ModItem
		{
			LockedChestMouseOver<K, C>(chestName, i, j);
			Player player = Main.LocalPlayer;
			if (player.showItemIconText == "")
			{
				player.showItemIcon = false;
				player.showItemIcon2 = 0;
			}
		}
		#endregion

		#region Furniture SetDefaults
		/// <summary>
		/// Extension which initializes a ModTile to be a bathtub.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpBathtub(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 4, 0);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(mt.Type);

			// All bathtubs count as tables.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a bed.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpBed(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileID.Sets.HasOutlines[mt.Type] = true;
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 4, 0);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(mt.Type);

			// All beds count as chairs.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a bookcase.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		/// <param name="solidTop">Whether this tile is supposed to have a solid top. Defaults to true.</param>
		internal static void SetUpBookcase(this ModTile mt, bool lavaImmune = false, bool solidTop = true)
		{
			Main.tileSolidTop[mt.Type] = solidTop;
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileTable[mt.Type] = solidTop;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// All bookcases count as tables.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a candelabra.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpCandelabra(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = true;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// All candelabras count as light sources.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a candle.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpCandle(this ModTile mt, bool lavaImmune = false, int offset = -4)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 20 };
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.newTile.DrawYOffset = offset;
			TileObjectData.addTile(mt.Type);

			// All candles count as light sources.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a chair.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpChair(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(mt.Type);

			// As you could probably guess, all chairs count as chairs.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a chandelier.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpChandelier(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.Origin = new Point16(1, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// All chandeliers count as light sources.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a chest.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		internal static void SetUpChest(this ModTile mt, bool offset = false)
		{
			Main.tileSpelunker[mt.Type] = true;
			Main.tileContainer[mt.Type] = true;
			Main.tileShine2[mt.Type] = true;
			Main.tileShine[mt.Type] = 1200;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileValue[mt.Type] = 500;
			TileID.Sets.HasOutlines[mt.Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			if (offset)
				TileObjectData.newTile.DrawYOffset = 4;
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.HookCheck = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
			TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(mt.Type);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a clock.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpClock(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			TileID.Sets.HasOutlines[mt.Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 5;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16,
				16
			};
			TileObjectData.newTile.Origin = new Point16(0, 4);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a closed door.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpDoorClosed(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileBlockLight[mt.Type] = true;
			Main.tileSolid[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileID.Sets.NotReallySolid[mt.Type] = true;
			TileID.Sets.DrawsWalls[mt.Type] = true;
			TileID.Sets.HasOutlines[mt.Type] = true;
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 1);
			TileObjectData.addAlternate(0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 2);
			TileObjectData.addAlternate(0);
			TileObjectData.addTile(mt.Type);

			// As you could probably guess, all closed doors count as doors.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be an open door.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpDoorOpen(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileSolid[mt.Type] = false;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			Main.tileNoSunLight[mt.Type] = true;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 1);
			TileObjectData.addAlternate(0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 2);
			TileObjectData.addAlternate(0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 0);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 1);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 2);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(mt.Type);
			TileID.Sets.HousingWalls[mt.Type] = true;
			TileID.Sets.HasOutlines[mt.Type] = true;

			// As you could probably guess, all open doors count as doors.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a dresser.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		internal static void SetUpDresser(this ModTile mt)
		{
			Main.tileSolidTop[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileTable[mt.Type] = true;
			Main.tileContainer[mt.Type] = true;
			Main.tileWaterDeath[mt.Type] = false;
			Main.tileLavaDeath[mt.Type] = false;
			TileID.Sets.HasOutlines[mt.Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
			TileObjectData.newTile.HookCheck = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
			TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(mt.Type);

			// All dressers count as tables.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a floor lamp.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpLamp(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// All floor lamps count as light sources.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a hanging lantern.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpLantern(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// All hanging lanterns count as light sources.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a piano.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpPiano(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.addTile(mt.Type);

			// All pianos count as tables.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a platform.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpPlatform(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileSolidTop[mt.Type] = true;
			Main.tileSolid[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileTable[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			TileID.Sets.Platforms[mt.Type] = true;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleMultiplier = 27;
			TileObjectData.newTile.StyleWrapLimit = 27;
			TileObjectData.newTile.UsesCustomCanPlace = false;
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// All platforms count as doors (so that you may have top-or-bottom entry/exit rooms)
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a sink.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpSink(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a sofa.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpSofa(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// All sofas count as chairs.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a table.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpTable(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileSolidTop[mt.Type] = true;
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileTable[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// As you could probably guess, all tables count as tables.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a torch.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpTorch(this ModTile mt, bool lavaImmune = false, bool waterImmune = false)
		{
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileSolid[mt.Type] = false;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileNoFail[mt.Type] = true;
			Main.tileWaterDeath[mt.Type] = !waterImmune;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newTile.WaterDeath = !waterImmune;
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.WaterDeath = false;
			TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.WaterDeath = false;
			TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
			TileObjectData.addAlternate(2);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.WaterDeath = false;
			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.addAlternate(0);
			TileObjectData.addTile(mt.Type);

			// All torches count as light sources.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a work bench.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		/// <param name="lavaImmune">Whether this tile is supposed to be immune to lava. Defaults to false.</param>
		internal static void SetUpWorkBench(this ModTile mt, bool lavaImmune = false)
		{
			Main.tileSolidTop[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileNoAttach[mt.Type] = true;
			Main.tileTable[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = !lavaImmune;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
			TileObjectData.newTile.LavaDeath = !lavaImmune;
			TileObjectData.addTile(mt.Type);

			// All work benches count as tables.
			mt.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
		}

		/// <summary>
		/// Extension which initializes a ModTile to be a fountain.
		/// </summary>
		/// <param name="mt">The ModTile which is being initialized.</param>
		internal static void SetUpFountain(this ModTile mt)
		{
			//All fountains are immune to lava
			Main.tileLighted[mt.Type] = true;
			Main.tileFrameImportant[mt.Type] = true;
			Main.tileLavaDeath[mt.Type] = false;
			Main.tileWaterDeath[mt.Type] = false;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(mt.Type);
			TileID.Sets.HasOutlines[mt.Type] = true;

			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.Origin = new Point16(0, 3);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 2, 0);
			TileObjectData.addTile(mt.Type);
		}
		#endregion
	}
}
