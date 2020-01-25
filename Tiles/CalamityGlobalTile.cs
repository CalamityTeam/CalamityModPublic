using CalamityMod.CalPlayer;
using CalamityMod.Items.Potions;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.Ores;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class CalamityGlobalTile : GlobalTile
    {
        public static ushort[] PlantTypes = new ushort[]
        {
            TileID.Plants,
            TileID.CorruptPlants,
            TileID.JunglePlants,
            TileID.MushroomPlants,
            TileID.Plants2,
            TileID.JunglePlants2,
            TileID.HallowedPlants,
            TileID.HallowedPlants2,
            TileID.FleshWeeds,
            (ushort)ModContent.TileType<AstralShortPlants>(),
            (ushort)ModContent.TileType<AstralTallPlants>(),
        };

        public override bool PreHitWire(int i, int j, int type)
        {
            return !CalamityWorld.bossRushActive;
        }

        public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
        {
            // Custom plant framing
            for (int k = 0; k < PlantTypes.Length; k++)
                if (type == PlantTypes[k])
                {
                    TileFraming.PlantFrame(i, j);
                    return false;
                }

            // Custom vine framing
            if (type == TileID.Vines || type == TileID.CrimsonVines || type == TileID.HallowedVines || type == ModContent.TileType<AstralVines>())
            {
                TileFraming.VineFrame(i, j);
                return false;
            }
            return base.TileFrame(i, j, type, ref resetFrame, ref noBreak);
        }

        public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            // Guaranteed not null at this point
            Tile tile = Main.tile[i, j];

            // This function is only for Astral Cactus. If the tile isn't even cactus, forget about it.
            if (type != TileID.Cactus)
                return;

            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;
            int frameX = tile.frameX;
            int frameY = tile.frameY;

            // Search down the cactus to find out whether the block it is planted in is Astral Sand.
            bool astralCactus = false;
            if (!Main.canDrawColorTile(i, j))
            {
                int xTile = i;
                if (frameX == 36) // Cactus segment which splits left
                    xTile--;
                if (frameX == 54) // Cactus segment which splits right
                    xTile++;
                if (frameX == 108) // Cactus segment which splits both directions
                    xTile += (frameY == 18) ? -1 : 1;

                int yTile = j;
                bool slidingDownCactus = Main.tile[xTile, yTile] != null && Main.tile[xTile, yTile].type == TileID.Cactus && Main.tile[xTile, yTile].active();
                while (!Main.tile[xTile, yTile].active() || !Main.tileSolid[Main.tile[xTile, yTile].type] || !slidingDownCactus)
                {
                    if (Main.tile[xTile, yTile].type == TileID.Cactus && Main.tile[xTile, yTile].active())
                    {
                        slidingDownCactus = true;
                    }
                    yTile++;
                    // Cacti are assumed to be no more than 20 blocks tall.
                    if (yTile > i + 20)
                        break;
                }
                astralCactus = Main.tile[xTile, yTile].type == (ushort)ModContent.TileType<AstralSand>();
            }

            // If it is actually astral cactus, then draw its glowmask.
            if (astralCactus)
            {
                spriteBatch.Draw(CalamityMod.AstralCactusGlowTexture, new Vector2((float)(i * 16 - (int)Main.screenPosition.X), (float)(j * 16 - (int)Main.screenPosition.Y)) + zero, new Rectangle((int)frameX, (int)frameY, 16, 18), Color.White * 0.75f, 0f, default, 1f, SpriteEffects.None, 0f);
            }
        }

        // This function exists only to shatter adjacent Lumenyl or Sea Prism crystals when a neighboring solid tile is destroyed.
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Tile tile = Main.tile[i, j];
            if(tile is null)
				return;

            // Helper function to shatter crystals attached to neighboring solid tiles.
            void CheckShatterCrystal(int xPos, int yPos)
            {
                Tile t = Main.tile[xPos, yPos];
                if (t != null && t.active() && (t.type == ModContent.TileType<LumenylCrystals>() || (t.type == ModContent.TileType<SeaPrismCrystals>() && CalamityWorld.downedDesertScourge)))
                {
                    WorldGen.KillTile(xPos, yPos, false, false, false);
                    if (!Main.tile[xPos, yPos].active() && Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(17, -1, -1, null, 0, (float)xPos, (float)yPos, 0f, 0, 0, 0);
                }
            }
			
			// CONSIDER -- Lumenyl Crystals and Sea Prism Crystals aren't solid. They shouldn't need to be checked here.
            if (Main.tileSolid[tile.type] && tile.type != ModContent.TileType<LumenylCrystals>() && tile.type != ModContent.TileType<SeaPrismCrystals>())
            {
                CheckShatterCrystal(i + 1, j);
                CheckShatterCrystal(i - 1, j);
                CheckShatterCrystal(i, j + 1);
                CheckShatterCrystal(i, j - 1);
            }
        }

        // LATER -- clean up copied decompiled pot code here
        public override bool Drop(int i, int j, int type)
        {
            if (type == TileID.Pots)
            {
                int x = Main.maxTilesX;
                int y = Main.maxTilesY;
                int genLimit = x / 2;
                int abyssChasmSteps = y / 4;
                int abyssChasmY = y - abyssChasmSteps + (int)((double)y * 0.055); //132 = 1932 large
                if (y < 1500)
                {
                    abyssChasmY = y - abyssChasmSteps + (int)((double)y * 0.095); //114 = 1014 small
                }
                else if (y < 2100)
                {
                    abyssChasmY = y - abyssChasmSteps + (int)((double)y * 0.0735); //132 = 1482 medium
                }
                int abyssChasmX = CalamityWorld.abyssSide ? genLimit - (genLimit - 135) : genLimit + (genLimit - 135);

                bool abyssPosX = false;
                bool sulphurPosX = false;
                bool abyssPosY = j <= abyssChasmY;
                if (CalamityWorld.abyssSide)
                {
                    if (i < 380)
                    {
                        sulphurPosX = true;
                    }
                    if (i < abyssChasmX + 80)
                    {
                        abyssPosX = true;
                    }
                }
                else
                {
                    if (i > Main.maxTilesX - 380)
                    {
                        sulphurPosX = true;
                    }
                    if (i > abyssChasmX - 80)
                    {
                        abyssPosX = true;
                    }
                }
                if (abyssPosX && abyssPosY)
                {
                    if (Main.rand.NextBool(10))
                    {
						int potionType = ItemID.SpelunkerPotion;
						switch (WorldGen.genRand.Next(15))
						{
							case 0:
								potionType = ItemID.SpelunkerPotion;
								break;
							case 1:
								potionType = ItemID.MagicPowerPotion;
								break;
							case 2:
								potionType = ItemID.ShinePotion;
								break;
							case 3:
								potionType = ItemID.WaterWalkingPotion;
								break;
							case 4:
								potionType = ItemID.ObsidianSkinPotion;
								break;
							case 5:
								potionType = ItemID.GravitationPotion;
								break;
							case 6:
								potionType = ItemID.RegenerationPotion;
								break;
							case 7:
								potionType = ModContent.ItemType<TriumphPotion>();
								break;
							case 8:
								potionType = ModContent.ItemType<AnechoicCoating>();
								break;
							case 9:
								potionType = ItemID.GillsPotion;
								break;
							case 10:
								potionType = ItemID.EndurancePotion;
								break;
							case 11:
								potionType = ItemID.HeartreachPotion;
								break;
							case 12:
								potionType = ItemID.FlipperPotion;
								break;
							case 13:
								potionType = ItemID.LifeforcePotion;
								break;
							case 14:
								potionType = ItemID.InfernoPotion;
								break;
							default:
								break;
						}
                        Item.NewItem(i * 16, j * 16, 16, 16, potionType, 1, false, 0, false, false);
                    }
                    else
                    {
                        int lootType = Main.rand.Next(10); //0 to 9
                        if (lootType == 0) //spelunker glowsticks
                        {
                            int sglowstickAmt = Main.rand.Next(2, 6);
                            if (Main.expertMode)
                            {
                                sglowstickAmt += Main.rand.Next(1, 7);
                            }
                            Item.NewItem(i * 16, j * 16, 16, 16, ItemID.SpelunkerGlowstick, sglowstickAmt, false, 0, false, false);
                        }
                        else if (lootType == 1) //hellfire arrows
                        {
                            int arrowAmt = Main.rand.Next(10, 21);
                            Item.NewItem(i * 16, j * 16, 16, 16, ItemID.HellfireArrow, arrowAmt, false, 0, false, false);
                        }
                        else if (lootType == 2) //stew
                        {
                            Item.NewItem(i * 16, j * 16, 16, 16, ModContent.ItemType<SunkenStew>(), 1, false, 0, false, false);
                        }
                        else if (lootType == 3) //sticky dynamite
                        {
                            Item.NewItem(i * 16, j * 16, 16, 16, ItemID.StickyDynamite, 1, false, 0, false, false);
                        }
                        else //money
                        {
                            float num13 = (float)(5000 + WorldGen.genRand.Next(-100, 101));
                            while ((int)num13 > 0)
                            {
                                if (num13 > 1000000f)
                                {
                                    int ptCoinAmt = (int)(num13 / 1000000f);
                                    if (ptCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        ptCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        ptCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    num13 -= (float)(1000000 * ptCoinAmt);
                                    Item.NewItem(i * 16, j * 16, 16, 16, ItemID.PlatinumCoin, ptCoinAmt, false, 0, false, false);
                                }
                                else if (num13 > 10000f)
                                {
                                    int auCoinAmt = (int)(num13 / 10000f);
                                    if (auCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        auCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        auCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    num13 -= (float)(10000 * auCoinAmt);
                                    Item.NewItem(i * 16, j * 16, 16, 16, ItemID.GoldCoin, auCoinAmt, false, 0, false, false);
                                }
                                else if (num13 > 100f)
                                {
                                    int agCoinAmt = (int)(num13 / 100f);
                                    if (agCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        agCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        agCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    num13 -= (float)(100 * agCoinAmt);
                                    Item.NewItem(i * 16, j * 16, 16, 16, ItemID.SilverCoin, agCoinAmt, false, 0, false, false);
                                }
                                else
                                {
                                    int cuCoinAmt = (int)num13;
                                    if (cuCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        cuCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        cuCoinAmt /= Main.rand.Next(4) + 1;
                                    }
                                    if (cuCoinAmt < 1)
                                    {
                                        cuCoinAmt = 1;
                                    }
                                    num13 -= (float)cuCoinAmt;
                                    Item.NewItem(i * 16, j * 16, 16, 16, ItemID.CopperCoin, cuCoinAmt, false, 0, false, false);
                                }
                            }
                        }
                    }
                }
                else if (sulphurPosX)
                {
                    if (Main.rand.NextBool(15))
                    {
						int potionType = ItemID.SpelunkerPotion;
						switch (WorldGen.genRand.Next(15))
						{
							case 0:
								potionType = ItemID.SpelunkerPotion;
								break;
							case 1:
								potionType = ItemID.MagicPowerPotion;
								break;
							case 2:
								potionType = ItemID.ShinePotion;
								break;
							case 3:
								potionType = ItemID.WaterWalkingPotion;
								break;
							case 4:
								potionType = ItemID.ObsidianSkinPotion;
								break;
							case 5:
								potionType = ItemID.GravitationPotion;
								break;
							case 6:
								potionType = ItemID.RegenerationPotion;
								break;
							case 7:
								potionType = ModContent.ItemType<TriumphPotion>();
								break;
							case 8:
								potionType = ModContent.ItemType<AnechoicCoating>();
								break;
							case 9:
								potionType = ItemID.GillsPotion;
								break;
							case 10:
								potionType = ItemID.EndurancePotion;
								break;
							case 11:
								potionType = ItemID.HeartreachPotion;
								break;
							case 12:
								potionType = ItemID.FlipperPotion;
								break;
							case 13:
								potionType = ItemID.LifeforcePotion;
								break;
							case 14:
								potionType = ItemID.InfernoPotion;
								break;
							default:
								break;
						}
                        Item.NewItem(i * 16, j * 16, 16, 16, potionType, 1, false, 0, false, false);
                    }
                    else
                    {
                        int lootType = Main.rand.Next(10); //0 to 9
                        if (lootType == 0) //glowsticks
                        {
                            int glowstickAmt = Main.rand.Next(2, 6);
                            if (Main.expertMode)
                            {
                                glowstickAmt += Main.rand.Next(1, 7);
                            }
                            Item.NewItem(i * 16, j * 16, 16, 16, ItemID.Glowstick, glowstickAmt, false, 0, false, false);
                        }
                        else if (lootType == 1) //jesters arrows
                        {
                            int jArrowAmt = Main.rand.Next(10, 21);
                            Item.NewItem(i * 16, j * 16, 16, 16, ItemID.JestersArrow, jArrowAmt, false, 0, false, false);
                        }
                        else if (lootType == 2) //potion
                        {
                            Item.NewItem(i * 16, j * 16, 16, 16, ItemID.HealingPotion, 1, false, 0, false, false);
                        }
                        else if (lootType == 3) //bomb
                        {
                            int bombAmt = Main.rand.Next(5, 9);
                            Item.NewItem(i * 16, j * 16, 16, 16, ItemID.Bomb, bombAmt, false, 0, false, false);
                        }
                        else //money
                        {
                            float num13 = (float)(2500 + WorldGen.genRand.Next(-100, 101));
                            while ((int)num13 > 0)
                            {
                                if (num13 > 1000000f)
                                {
                                    int ptCoinAmt = (int)(num13 / 1000000f);
                                    if (ptCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        ptCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        ptCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    num13 -= (float)(1000000 * ptCoinAmt);
                                    Item.NewItem(i * 16, j * 16, 16, 16, ItemID.PlatinumCoin, ptCoinAmt, false, 0, false, false);
                                }
                                else if (num13 > 10000f)
                                {
                                    int auCoinAmt = (int)(num13 / 10000f);
                                    if (auCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        auCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        auCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    num13 -= (float)(10000 * auCoinAmt);
                                    Item.NewItem(i * 16, j * 16, 16, 16, ItemID.GoldCoin, auCoinAmt, false, 0, false, false);
                                }
                                else if (num13 > 100f)
                                {
                                    int agCoinAmt = (int)(num13 / 100f);
                                    if (agCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        agCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        agCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    num13 -= (float)(100 * agCoinAmt);
                                    Item.NewItem(i * 16, j * 16, 16, 16, ItemID.SilverCoin, agCoinAmt, false, 0, false, false);
                                }
                                else
                                {
                                    int cuCoinAmt = (int)num13;
                                    if (cuCoinAmt > 50 && Main.rand.NextBool(2))
                                    {
                                        cuCoinAmt /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.NextBool(2))
                                    {
                                        cuCoinAmt /= Main.rand.Next(4) + 1;
                                    }
                                    if (cuCoinAmt < 1)
                                    {
                                        cuCoinAmt = 1;
                                    }
                                    num13 -= (float)cuCoinAmt;
                                    Item.NewItem(i * 16, j * 16, 16, 16, ItemID.CopperCoin, cuCoinAmt, false, 0, false, false);
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

		public override void NearbyEffects(int i, int j, int type, bool closer)
		{
			if (Main.gamePaused)
			{
				return;
			}
			if (CalamityWorld.death && !CalamityPlayer.areThereAnyDamnBosses)
			{
				bool underworldTile = type == TileID.Ash || type == TileID.Hellstone;
				bool cragTile = type == ModContent.TileType<BrimstoneSlag>() || type == ModContent.TileType<CharredOre>();
				if ((underworldTile || cragTile) && closer)
				{
					if (j > Main.maxTilesY - 180 && j < Main.maxTilesY - 50)
					{
						if (Main.tile[i, j - 1] == null)
							Main.tile[i, j - 1] = new Tile();

						Tile tileAbove = Main.tile[i, j - 1];
						if (tileAbove.liquidType() == 1 && !tileAbove.active())
						{
							// Only shoot flames if tiles underneath are lava and if tiles above and below aren't active
							bool shootFlames = Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(300);
							if (shootFlames)
							{
								int lavaTilesAbove = 0;
								int lavaTopY = 0;
								Tile lavaTop = new Tile();
								for (int k = j - 1; k > Main.maxTilesY - 180; k--)
								{
									if (Main.tile[i, k] == null)
										Main.tile[i, k] = new Tile();

									if (!Main.tile[i, k].active() && Main.tile[i, k].liquidType() == 1)
									{
										if (lavaTilesAbove < 5)
											lavaTilesAbove++;
									}
									else
									{
										if (lavaTilesAbove == 5)
										{
											lavaTopY = k;
											lavaTop = Main.tile[i, k];
										}
										else
											shootFlames = false;

										break;
									}
								}
								if (shootFlames)
								{
									for (int l = lavaTopY - 1; l > lavaTopY - 10; l--)
									{
										if (Main.tile[i, l].active())
										{
											shootFlames = false;
											break;
										}
									}
								}
							}
							if (shootFlames)
							{
								int projectileType = underworldTile ? ProjectileID.GeyserTrap : ModContent.ProjectileType<BrimstoneFire>();
								float randomVelocity = Main.rand.NextFloat() + 0.5f;
								Projectile.NewProjectile((float)(i * 16), (float)(j * 16), 0f, -8f * randomVelocity, projectileType, 20, 2f, Main.myPlayer, 0f, 0f);
							}
						}
					}
				}
			}
		}
	}
}
