using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer
{
    public class CalamityPlayerLifeRegen
    {
        #region Update Bad Life Regen
        public static void CalamityUpdateBadLifeRegen(Player player, Mod mod)
        {
            Point point = player.Center.ToTileCoordinates();
            CalamityPlayer modPlayer = player.Calamity();

			if (CalamityWorld.ironHeart || player.ownedProjectileCounts[ModContent.ProjectileType<BloodBoilerFire>()] > 0)
				modPlayer.noLifeRegen = true;

			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			double lifeRegenMult = death ? 1.5 : 1D;
			int lifeRegenLost = 0;

            // Initial Debuffs

			// Vanilla
			if (death)
			{
				if (player.poisoned)
					lifeRegenLost += 4;

				if (player.onFire)
					lifeRegenLost += 8;

				if (player.tongued)
					lifeRegenLost += 100;

				if (player.venom)
					lifeRegenLost += 12;

				if (player.onFrostBurn)
					lifeRegenLost += 12;

				if (player.onFire2)
					lifeRegenLost += 12;

				if (player.burned)
					lifeRegenLost += 60;

				if (player.suffocating)
					lifeRegenLost += 40;

				if (player.electrified)
				{
					lifeRegenLost += 8;
					if (player.controlLeft || player.controlRight)
						lifeRegenLost += 32;
				}
			}

			// Calamity
            if (modPlayer.shadowflame)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
				lifeRegenLost += 30;
            }

            if (modPlayer.wDeath)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
            }

            if (modPlayer.aFlames)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
				lifeRegenLost += 16;
            }

            if (modPlayer.bFlames)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
				lifeRegenLost += modPlayer.abaddon ? 8 : 16;
            }

            if (modPlayer.nightwither)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
				lifeRegenLost += 16;
			}

            if (modPlayer.vaporfied)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
				lifeRegenLost += 8;
			}

			if (modPlayer.cragsLava)
			{
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
				lifeRegenLost += 30;
            }

            if (modPlayer.gsInferno)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
				lifeRegenLost += modPlayer.profanedCrystalBuffs ? 35 : 30;
            }

            if (modPlayer.astralInfection)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
				lifeRegenLost += 20;
            }

            if (modPlayer.ZoneSulphur && player.IsUnderwater() && !modPlayer.aquaticScourgeLore && !modPlayer.decayEffigy)
            {
                player.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 2, true);
                modPlayer.pissWaterBoost++;

                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
				int waterDivisor = 250;
				int minimumRegenLost = 8;
				if (modPlayer.sulfurSet && modPlayer.sulphurskin)
				{
					waterDivisor = 500;
					minimumRegenLost = 1;
				}
				else if (modPlayer.sulfurSet)
				{
					waterDivisor = 400;
					minimumRegenLost = 3;
				}
				else if (modPlayer.sulphurskin)
				{
					waterDivisor = 350;
					minimumRegenLost = 2;
				}
				int sulphurWater = modPlayer.pissWaterBoost / waterDivisor;
                if (sulphurWater < minimumRegenLost)
                    sulphurWater = minimumRegenLost;
				lifeRegenLost += sulphurWater;
            }
            else
                modPlayer.pissWaterBoost = 0;

            if (modPlayer.sulphurPoison)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
				lifeRegenLost += 6;
				if (modPlayer.sulfurSet)
				{
					lifeRegenLost -= 2;
				}
				if (modPlayer.sulphurskin)
				{
					lifeRegenLost -= 2;
				}
            }

            if (modPlayer.hFlames)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
				lifeRegenLost += 16;
            }

            if (modPlayer.pFlames)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
				lifeRegenLost += modPlayer.reducedPlagueDmg ? 10 : 20;
            }

            if (modPlayer.bBlood)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
				lifeRegenLost += 8;
            }

            if (modPlayer.vHex)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
				lifeRegenLost += 16;
            }

            if (modPlayer.cDepth)
            {
                if (player.statDefense > 0)
                {
                    int depthDamage = modPlayer.depthCharm ? 9 : 18;
                    int subtractDefense = (int)(player.statDefense * 0.05); // 240 defense = 0 damage taken with depth charm
                    int calcDepthDamage = depthDamage - subtractDefense;

                    if (calcDepthDamage < 0)
                        calcDepthDamage = 0;

                    if (player.lifeRegen > 0)
                        player.lifeRegen = 0;

                    player.lifeRegenTime = 0;
					lifeRegenLost += calcDepthDamage;
                }
            }

			if (modPlayer.vodka)
			{
				modPlayer.alcoholPoisonLevel++;
				lifeRegenLost += 1;
			}
			if (modPlayer.redWine)
			{
				modPlayer.alcoholPoisonLevel++;
				lifeRegenLost += 1;
				if (modPlayer.baguette)
					lifeRegenLost += 3;
			}
			if (modPlayer.grapeBeer)
			{
				modPlayer.alcoholPoisonLevel++;
			}
			if (modPlayer.moonshine)
			{
				modPlayer.alcoholPoisonLevel++;
				lifeRegenLost += 1;
			}
			if (modPlayer.rum)
			{
				modPlayer.alcoholPoisonLevel++;
			}
			if (modPlayer.fabsolVodka)
			{
				modPlayer.alcoholPoisonLevel++;
			}
			if (modPlayer.fireball)
			{
				modPlayer.alcoholPoisonLevel++;
				lifeRegenLost += 1;
			}
			if (modPlayer.whiskey)
			{
				modPlayer.alcoholPoisonLevel++;
			}
			if (modPlayer.everclear)
			{
				modPlayer.alcoholPoisonLevel += 2;
				lifeRegenLost += 10;
			}
			if (modPlayer.bloodyMary)
			{
				modPlayer.alcoholPoisonLevel++;
				lifeRegenLost += 2;
			}
			if (modPlayer.tequila)
			{
				modPlayer.alcoholPoisonLevel++;
				lifeRegenLost += 1;
			}
			if (modPlayer.tequilaSunrise)
			{
				modPlayer.alcoholPoisonLevel++;
				lifeRegenLost += 1;
			}
			if (modPlayer.screwdriver)
			{
				modPlayer.alcoholPoisonLevel++;
				lifeRegenLost += 1;
			}
			if (modPlayer.caribbeanRum)
			{
				modPlayer.alcoholPoisonLevel++;
			}
			if (modPlayer.cinnamonRoll)
			{
				modPlayer.alcoholPoisonLevel++;
			}
			if (modPlayer.margarita)
			{
				modPlayer.alcoholPoisonLevel++;
				lifeRegenLost += 1;
			}
			if (modPlayer.starBeamRye)
			{
				modPlayer.alcoholPoisonLevel++;
				lifeRegenLost += 1;
			}
			if (modPlayer.moscowMule)
			{
				modPlayer.alcoholPoisonLevel++;
				lifeRegenLost += 2;
			}
			if (modPlayer.whiteWine)
			{
				modPlayer.alcoholPoisonLevel++;
				lifeRegenLost += 1;
			}
			if (modPlayer.evergreenGin)
			{
				modPlayer.alcoholPoisonLevel++;
				lifeRegenLost += 1;
			}
			if (modPlayer.alcoholPoisonLevel > 3)
			{
				player.nebulaLevelLife = 0;

				if (player.whoAmI == Main.myPlayer)
					player.AddBuff(ModContent.BuffType<AlcoholPoisoning>(), 2, false);

				if (player.lifeRegen > 0)
					player.lifeRegen = 0;

				player.lifeRegenTime = 0;
				lifeRegenLost += 3 * modPlayer.alcoholPoisonLevel;
			}

			if (modPlayer.manaOverloader)
			{
				if (player.statMana > (int)(player.statManaMax2 * 0.5))
					lifeRegenLost += 3;
			}
            if (modPlayer.brimflameFrenzy)
            {
                player.manaRegen = 0;
                player.manaRegenBonus = 0;
                player.manaRegenDelay = (int) player.maxRegenDelay;
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;
                lifeRegenLost += 42; //the meaning of death
            }

			player.lifeRegen -= (int)(lifeRegenLost * lifeRegenMult);

			// Buffs

			if (modPlayer.bloodfinBoost)
			{
				if (player.lifeRegen < 0)
				{
					if (player.lifeRegenTime < 1800)
						player.lifeRegenTime = 1800;

					player.lifeRegen += 10;
				}
				else
				{
					player.lifeRegen += 5;
					player.lifeRegenTime += 10;
				}
                if (modPlayer.bloodfinTimer > 0)
                { modPlayer.bloodfinTimer--; }
                if (player.whoAmI == Main.myPlayer && modPlayer.bloodfinTimer <= 0)
                {
                    modPlayer.bloodfinTimer = 30;
					if (player.statLife <= (int)(player.statLifeMax2 * 0.75) && !modPlayer.noLifeRegen)
						player.statLife += 1;
                }
            }

			if (modPlayer.celestialJewel || modPlayer.astralArcanum)
            {
                bool lesserEffect = false;
                for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    int hasBuff = player.buffType[l];
                    lesserEffect = CalamityLists.alcoholList.Contains(hasBuff);
                }

                if (lesserEffect)
                {
                    player.lifeRegen += 1;
                    player.statDefense += 20;
                }
                else
                {
                    if (player.lifeRegen < 0)
                    {
                        if (player.lifeRegenTime < 1800)
                            player.lifeRegenTime = 1800;

                        player.lifeRegen += 4;
                        player.statDefense += 20;
                    }
                    else
                        player.lifeRegen += 2;
                }
            }
            else if (modPlayer.crownJewel)
            {
                bool lesserEffect = false;
                for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    int hasBuff = player.buffType[l];
                    lesserEffect = CalamityLists.alcoholList.Contains(hasBuff);
                }

                if (lesserEffect)
                    player.statDefense += 10;
                else
                {
                    if (player.lifeRegen < 0)
                    {
                        if (player.lifeRegenTime < 1800)
                            player.lifeRegenTime = 1800;

                        player.lifeRegen += 2;
                        player.statDefense += 10;
                    }
                    else
                        player.lifeRegen += 1;
                }
            }

			// Last Debuffs

			if (modPlayer.noLifeRegen)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;

                if (player.lifeRegenCount > 0)
                    player.lifeRegenCount = 0;
            }

            if (CalamityConfig.Instance.LethalLava || CalamityWorld.death) // Always occurs in Death regardless of config
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    if (Collision.LavaCollision(player.position, player.width, player.waterWalk ? (player.height - 6) : player.height))
                    {
                        if (player.lavaImmune && !player.immune)
                        {
                            if (player.lavaTime > 0)
                                player.lavaTime--;
                        }

                        if (player.lavaTime <= 0)
                            player.AddBuff(ModContent.BuffType<LethalLavaBurn>(), 2, true);
                    }
                }
            }

			if (modPlayer.lethalLavaBurn)
			{
				if (player.lifeRegen > 0)
					player.lifeRegen = 0;

				player.lifeRegenTime = 0;
				int lifeRegenDown = player.lavaImmune ? 9 : 18;

				if (player.lavaRose)
					lifeRegenDown = 3;

				player.lifeRegen -= (int)(lifeRegenDown * lifeRegenMult);
			}

			if (modPlayer.hInferno)
            {
				player.nebulaLevelLife = 0;

                modPlayer.hInfernoBoost++;

                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                player.lifeRegen -= (int)(modPlayer.hInfernoBoost * lifeRegenMult);

				if (player.lifeRegen < -200)
                    player.lifeRegen = -200;
            }
            else
                modPlayer.hInfernoBoost = 0;

			if (modPlayer.ZoneAbyss)
			{
				if (!player.IsUnderwater())
				{
					if (player.statLife > 100)
					{
						player.nebulaLevelLife = 0;

						if (player.lifeRegen > 0)
							player.lifeRegen = 0;

						player.lifeRegenTime = 0;
						player.lifeRegen -= (int)(160D * lifeRegenMult);
					}
				}
			}

            if (modPlayer.weakPetrification)
            {
                if (player.mount.Active)
                    player.mount.Dismount(player);
            }

            if (modPlayer.lol || (modPlayer.silvaCountdown > 0 && modPlayer.hasSilvaEffect && modPlayer.silvaSet))
            {
                if (player.lifeRegen < 0)
                    player.lifeRegen = 0;
            }
        }
        #endregion

        #region Update Life Regen
        public static void CalamityUpdateLifeRegen(Player player, Mod mod)
        {
            CalamityPlayer modPlayer = player.Calamity();

			if (modPlayer.rum)
				player.lifeRegen += 2;

			if (modPlayer.caribbeanRum)
				player.lifeRegen += 2;

			if (modPlayer.aChicken)
				player.lifeRegen += 1;

			if (modPlayer.cadence)
				player.lifeRegen += 5;

			if (modPlayer.mushy)
				player.lifeRegen += 1;

			if (modPlayer.permafrostsConcoction)
			{
				if (player.statLife < modPlayer.actualMaxLife / 2)
					player.lifeRegen++;
				if (player.statLife < modPlayer.actualMaxLife / 4)
					player.lifeRegen++;
				if (player.statLife < modPlayer.actualMaxLife / 10)
					player.lifeRegen += 2;

				if (player.poisoned || player.onFire || modPlayer.bFlames)
					player.lifeRegen += 4;
			}

			if (modPlayer.tRegen)
				player.lifeRegen += 3;

			if (modPlayer.sRegen)
				player.lifeRegen += 2;

            if (modPlayer.hallowedRegen)
                player.lifeRegen += 3;

            if (modPlayer.affliction || modPlayer.afflicted)
				player.lifeRegen += 1;

			if (modPlayer.absorber)
			{
				if (player.StandingStill() && player.itemAnimation == 0)
					player.lifeRegen += 2;
			}

			if (modPlayer.aAmpoule)
			{
				if (!player.honey && player.lifeRegen < 0)
				{
					player.lifeRegen += 2;
					if (player.lifeRegen > 0)
						player.lifeRegen = 0;
				}
				player.lifeRegenTime += 1;
				player.lifeRegen += 2;
			}

			if (modPlayer.ursaSergeant)
			{
				if (player.statLife <= (int)(modPlayer.actualMaxLife * 0.15))
				{
					player.lifeRegen += 3;
					player.lifeRegenTime += 3;
				}
				else if (player.statLife <= (int)(modPlayer.actualMaxLife * 0.25))
				{
					player.lifeRegen += 2;
					player.lifeRegenTime += 2;
				}
				else if (player.statLife <= (int)(modPlayer.actualMaxLife * 0.5))
				{
					player.lifeRegen += 1;
					player.lifeRegenTime += 1;
				}
			}

			if (modPlayer.polarisBoost)
			{
				player.lifeRegen += 1;
				player.lifeRegenTime += 1;
			}

			if (modPlayer.projRefRareLifeRegenCounter > 0)
			{
				player.lifeRegenTime += 2;
				player.lifeRegen += 2;
			}

			if (modPlayer.darkSunRing)
			{
				if (Main.eclipse || Main.dayTime)
					player.lifeRegen += 3;
			}

			if (modPlayer.community)
			{
				float floatTypeBoost = 0.05f +
					(NPC.downedSlimeKing ? 0.01f : 0f) +
					(NPC.downedBoss1 ? 0.01f : 0f) +
					(NPC.downedBoss2 ? 0.01f : 0f) +
					(NPC.downedQueenBee ? 0.01f : 0f) +
					(NPC.downedBoss3 ? 0.01f : 0f) + // 0.1
					(Main.hardMode ? 0.01f : 0f) +
					(NPC.downedMechBossAny ? 0.01f : 0f) +
					(NPC.downedPlantBoss ? 0.01f : 0f) +
					(NPC.downedGolemBoss ? 0.01f : 0f) +
					(NPC.downedFishron ? 0.01f : 0f) + // 0.15
					(NPC.downedAncientCultist ? 0.01f : 0f) +
					(NPC.downedMoonlord ? 0.01f : 0f) +
					(CalamityWorld.downedProvidence ? 0.01f : 0f) +
					(CalamityWorld.downedDoG ? 0.01f : 0f) +
					(CalamityWorld.downedYharon ? 0.01f : 0f); // 0.2
				int integerTypeBoost = (int)(floatTypeBoost * 50f);
				int regenBoost = 1 + (integerTypeBoost / 5);
				bool lesserEffect = false;
				for (int l = 0; l < Player.MaxBuffs; l++)
				{
					int hasBuff = player.buffType[l];
					bool shouldAffect = CalamityLists.alcoholList.Contains(hasBuff);
					if (shouldAffect)
						lesserEffect = true;
				}
				if (player.lifeRegen < 0)
					player.lifeRegen += lesserEffect ? 1 : regenBoost;
			}

			if (modPlayer.regenator)
			{
				player.lifeRegenTime += 8;
				player.lifeRegen += 16;
			}
            if (modPlayer.camper)
            {
                player.lifeRegen += 2;
            }
            if (modPlayer.handWarmer && modPlayer.eskimoSet)
            {
                player.lifeRegen += 2;
            }
			if (modPlayer.bloodPactBoost)
            {
                player.lifeRegen += 2;
            }

			if (modPlayer.bloodflareSummon)
			{
				if (player.statLife <= (int)(modPlayer.actualMaxLife * 0.5))
					player.lifeRegen += 2;
			}

            if (modPlayer.fearmongerSet && modPlayer.fearmongerRegenFrames > 0)
            {
                player.lifeRegen += 14;
                player.lifeRegenTime += 7;
                if (player.lifeRegenTime < 1800)
                    player.lifeRegenTime = 1800;
            }

			if (modPlayer.etherealExtorter && player.ZoneGlowshroom)
				player.lifeRegen += 1;

			if (modPlayer.pinkCandle && !modPlayer.noLifeRegen)
			{
				// Every frame, add up 1/60th of the healing value (0.4% max HP per second)
				modPlayer.pinkCandleHealFraction += player.statLifeMax2 * 0.004 / 60;
				if (modPlayer.pinkCandleHealFraction >= 1D)
				{
					modPlayer.pinkCandleHealFraction = 0D;
					if (player.statLife < player.statLifeMax2)
						player.statLife++;
				}
			}
			else
				modPlayer.pinkCandleHealFraction = 0D;

			// Standing still healing bonuses (all exclusive with vanilla Shiny Stone)
			if (!player.shinyStone)
			{
				int lifeRegenTimeMaxBoost = CalamityPlayer.areThereAnyDamnBosses ? 450 : 1800;
				int lifeRegenMaxBoost = CalamityPlayer.areThereAnyDamnBosses ? 1 : 4;
				float lifeRegenLifeRegenTimeMaxBoost = CalamityPlayer.areThereAnyDamnBosses ? 8f : 30f;

				if (player.StandingStill() && player.itemAnimation == 0)
				{
					bool boostedRegen = false;
					bool noSunlight = false;
					if (modPlayer.shadeRegen)
					{
						boostedRegen = true;
						if (player.lifeRegen > 0 && player.statLife < modPlayer.actualMaxLife)
						{
							if (Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.NextBool(30))
							{
								int regen = Dust.NewDust(player.position, player.width, player.height, 173, 0f, 0f, 200, default, 1f);
								Main.dust[regen].noGravity = true;
								Main.dust[regen].fadeIn = 1.3f;
								Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
								Main.dust[regen].velocity = velocity;
								velocity.Normalize();
								velocity *= 34f;
								Main.dust[regen].position = player.Center - velocity;
							}
						}
					}
					else if (modPlayer.cFreeze)
					{
						boostedRegen = true;
						if (player.lifeRegen > 0 && player.statLife < modPlayer.actualMaxLife)
						{
							if (Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.NextBool(30))
							{
								int regen = Dust.NewDust(player.position, player.width, player.height, 67, 0f, 0f, 200, new Color(150, Main.DiscoG, 255), 0.75f);
								Main.dust[regen].noGravity = true;
								Main.dust[regen].fadeIn = 1.3f;
								Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
								Main.dust[regen].velocity = velocity;
								velocity.Normalize();
								velocity *= 34f;
								Main.dust[regen].position = player.Center - velocity;
							}
						}
					}
					else if (modPlayer.draedonsHeart)
					{
						boostedRegen = true;
						if (player.lifeRegen > 0 && player.statLife < modPlayer.actualMaxLife)
						{
							if (Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.NextBool(2))
							{
								int regen = Dust.NewDust(player.position, player.width, player.height, 107, 0f, 0f, 200, default, 1f);
								Main.dust[regen].noGravity = true;
								Main.dust[regen].fadeIn = 1.3f;
								Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
								Main.dust[regen].velocity = velocity;
								velocity.Normalize();
								velocity *= 34f;
								Main.dust[regen].position = player.Center - velocity;
							}
						}
					}
					else if (modPlayer.photosynthesis)
					{
						boostedRegen = true;
						if (!Main.dayTime)
							noSunlight = true;
						if (player.lifeRegen > 0 && player.statLife < modPlayer.actualMaxLife)
						{
							if (Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.NextBool(2))
							{
								int regen = Dust.NewDust(player.position, player.width, player.height, 244, 0f, 0f, 200, default, 1f);
								Main.dust[regen].noGravity = true;
								Main.dust[regen].fadeIn = 1.3f;
								Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
								Main.dust[regen].velocity = velocity;
								velocity.Normalize();
								velocity *= 34f;
								Main.dust[regen].position = player.Center - velocity;
							}
						}
					}
					if (boostedRegen)
					{
						int lifeRegenTimeMaxBoost2 = !noSunlight ? lifeRegenTimeMaxBoost : (lifeRegenTimeMaxBoost / 5);
						int lifeRegenMaxBoost2 = !noSunlight ? lifeRegenMaxBoost : (lifeRegenMaxBoost / 5);
						float lifeRegenLifeRegenTimeMaxBoost2 = !noSunlight ? lifeRegenLifeRegenTimeMaxBoost : (lifeRegenLifeRegenTimeMaxBoost / 5);

						if (player.lifeRegenTime > 90 && player.lifeRegenTime < lifeRegenTimeMaxBoost2)
							player.lifeRegenTime = lifeRegenTimeMaxBoost2;

						player.lifeRegenTime += lifeRegenMaxBoost2;
						player.lifeRegen += lifeRegenMaxBoost2;

						float num3 = player.lifeRegenTime * 2.5f; // lifeRegenTime max is 3600
						num3 /= 300f;
						if (num3 > 0f)
						{
							if (num3 > lifeRegenLifeRegenTimeMaxBoost2)
								num3 = lifeRegenLifeRegenTimeMaxBoost2;

							player.lifeRegen += (int)num3;
						}
						if (player.lifeRegen > 0 && player.statLife < modPlayer.actualMaxLife)
						{
							player.lifeRegenCount++;
						}
					}
				}
			}

			// The Camper regen boost activates while moving so it can stack with Shiny Stone like effects
			if (modPlayer.camper && player.statLife < modPlayer.actualMaxLife && !player.StandingStill())
			{
				float camperRegenMult = CalamityPlayer.areThereAnyDamnBosses ? 1.3f : 1.75f;
				int camperCap = CalamityPlayer.areThereAnyDamnBosses ? 20 : 30;
				player.lifeRegen = (int)((player.lifeRegen * 2) * camperRegenMult);
				player.lifeRegenCount = player.lifeRegenCount > camperCap ? player.lifeRegenCount : camperCap;
				player.lifeRegenCount++;
				if (Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.NextBool(2))
				{
					int regen = Dust.NewDust(player.position, player.width, player.height, 12, 0f, 0f, 200, Color.OrangeRed, 1f);
					Main.dust[regen].noGravity = true;
					Main.dust[regen].fadeIn = 1.3f;
					Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
					Main.dust[regen].velocity = velocity;
					velocity.Normalize();
					velocity *= 34f;
					Main.dust[regen].position = player.Center - velocity;
				}
			}

			if (CalamityWorld.revenge)
			{
				if (player.statLife < modPlayer.actualMaxLife)
				{
					bool noLifeRegenCap = (player.shinyStone || modPlayer.draedonsHeart || modPlayer.cFreeze || modPlayer.shadeRegen || modPlayer.photosynthesis || modPlayer.camper) &&
						player.StandingStill() && player.itemAnimation == 0;

					if (!noLifeRegenCap)
					{
						// Max HP = 400
						// 350 HP = 1 - 0.875 * 10 = 1.25 = 1
						// 100 HP = 1 - 0.25 * 10 = 7.5 = 7
						// 200 HP = 1 - 0.5 * 10 = 5
						int lifeRegenScale = (int)((1f - (player.statLife / modPlayer.actualMaxLife)) * 10f); // 9 to 0 (1% HP to 100%)
						if (player.lifeRegen > lifeRegenScale)
						{
							float lifeRegenScalar = 1f + (player.statLife / modPlayer.actualMaxLife); // 1 to 2 (1% HP to 100%)
							int defLifeRegen = (int)(player.lifeRegen / lifeRegenScalar);
							player.lifeRegen = defLifeRegen;
						}
					}
				}
			}

			if (player.lifeRegen > 0)
			{
				if (modPlayer.plaguebringerGoliathLore)
					player.lifeRegen /= 2;

				if (modPlayer.eaterOfWorldsLore)
					player.lifeRegen /= 2;
			}

			if (BossRushEvent.BossRushActive)
			{
				if (CalamityConfig.Instance.BossRushHealthCurse)
				{
					if (player.lifeRegen > 0)
						player.lifeRegen = 0;

					player.lifeRegenTime = 0;

					if (player.lifeRegenCount > 0)
						player.lifeRegenCount = 0;
				}
			}

			// For the stat meter
			modPlayer.lifeRegenStat = player.lifeRegen;
		}
        #endregion
    }
}
