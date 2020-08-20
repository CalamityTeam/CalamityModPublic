using CalamityMod.Buffs.Cooldowns;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Summon;
using CalamityMod.Dusts;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Environment;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.UI;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Events;

namespace CalamityMod.CalPlayer
{
	public class CalamityPlayerMiscEffects
	{
		#region Post Update Misc Effects
		public static void CalamityPostUpdateMiscEffects(Player player, Mod mod)
		{
			CalamityPlayer modPlayer = player.Calamity();

			// No category

			// Decrease the counter on Fearmonger set turbo regeneration
			if (modPlayer.fearmongerRegenFrames > 0)
				modPlayer.fearmongerRegenFrames--;

			// Reduce the expert debuff time multiplier to the normal mode multiplier
			if (CalamityConfig.Instance.NerfExpertDebuffs)
				Main.expertDebuffTime = 1f;

			// Bool for any existing bosses, true if any boss NPC is active
			CalamityPlayer.areThereAnyDamnBosses = CalamityGlobalNPC.AnyBossNPCS();

			// Bool for any existing events, true if any event is active
			CalamityPlayer.areThereAnyDamnEvents = CalamityGlobalNPC.AnyEvents(player);

			// If any boss NPC is active, apply Zen to the player to reduce spawn rate
			if (CalamityPlayer.areThereAnyDamnBosses)
			{
				if (player.whoAmI == Main.myPlayer)
					player.AddBuff(ModContent.BuffType<BossZen>(), 2, false);
			}

			// Revengeance effects
			RevengeanceModeMiscEffects(player, modPlayer, mod);

			// Abyss effects
			AbyssEffects(player, modPlayer);

			// Misc effects, because I don't know what else to call it
			MiscEffects(player, modPlayer, mod);

			// Max life and mana effects
			MaxLifeAndManaEffects(player, modPlayer, mod);

			// Standing still effects
			StandingStillEffects(player, modPlayer);

			// Elysian Aegis effects
			ElysianAegisEffects(player, modPlayer);

			// Other buff effects
			OtherBuffEffects(player, modPlayer);

			// Limits
			Limits(player, modPlayer);

			// Endurance reductions
			EnduranceReductions(player, modPlayer);

			// Stat Meter
			UpdateStatMeter(player, modPlayer);

			// Rogue Mirrors
			RogueMirrors(player, modPlayer);

			// Double Jumps
			DoubleJumps(player, modPlayer);

			// Potions (Quick Buff && Potion Sickness)
			HandlePotions(player, modPlayer);
		}
		#endregion

		#region Revengeance Effects
		private static void RevengeanceModeMiscEffects(Player player, CalamityPlayer modPlayer, Mod mod)
		{
			if (CalamityWorld.revenge)
			{
				// This effect is way too annoying during the fight so I disabled it - Fab
				// Signus headcrab effect
				/*if (CalamityGlobalNPC.signus != -1)
				{
					if (Main.npc[CalamityGlobalNPC.signus].active)
					{
						if (Vector2.Distance(player.Center, Main.npc[CalamityGlobalNPC.signus].Center) <= 5200f)
						{
							float signusLifeRatio = 1f - (Main.npc[CalamityGlobalNPC.signus].life / Main.npc[CalamityGlobalNPC.signus].lifeMax);

							// Reduce the power of Signus darkness based on your light level.
							float multiplier = 1f;
							switch (modPlayer.GetTotalLightStrength())
							{
								case 0:
									break;
								case 1:
								case 2:
									multiplier = 0.75f;
									break;
								case 3:
								case 4:
									multiplier = 0.5f;
									break;
								case 5:
								case 6:
									multiplier = 0.25f;
									break;
								default:
									multiplier = 0f;
									break;
							}

							// Increased darkness in Death Mode
							if (CalamityWorld.death)
								multiplier += (1f - multiplier) * 0.1f;

							// Total darkness
							float signusDarkness = signusLifeRatio * multiplier;

							// Headcrab effect
							if (!modPlayer.ZoneAbyss && !player.headcovered)
							{
								float screenObstructionAmt = MathHelper.Clamp(signusDarkness, 0f, 0.63f);
								float targetValue = MathHelper.Clamp(screenObstructionAmt * 0.33f, 0.1f, 0.2f);
								ScreenObstruction.screenObstruction = MathHelper.Lerp(ScreenObstruction.screenObstruction, screenObstructionAmt, targetValue);
							}
						}
					}
				}*/

				// Adjusts the life steal cap in rev/death
				float lifeStealCap = CalamityWorld.death ? 50f : 60f;
				/*if (Main.masterMode)
					lifeStealCap *= 0.75f;*/
				if (player.lifeSteal > lifeStealCap)
					player.lifeSteal = lifeStealCap;

				if (player.whoAmI == Main.myPlayer)
				{
					// Titanium Armor nerf
					if (player.onHitDodge)
					{
						for (int l = 0; l < Player.MaxBuffs; l++)
						{
							int hasBuff = player.buffType[l];
							if (player.buffTime[l] > 360 && hasBuff == BuffID.ShadowDodge)
								player.buffTime[l] = 360;
						}
					}

					// Immunity Frames nerf
					if (player.immuneTime > 120)
						player.immuneTime = 120;

					// Adrenaline and Rage
					if (CalamityConfig.Instance.Rippers)
					{
						// This is how much Rage will be changed by this frame.
						float rageDiff = 0;

						// If Rage Mode is currently active, you smoothly lose all rage over the duration.
						if (modPlayer.rageModeActive)
							rageDiff = -modPlayer.rageMax / CalamityPlayer.RageDuration;

						// Draedon's Heart gives 1 rage per frame. Heart of Darkness gives 0.5 rage per frame.
						else if (modPlayer.draedonsHeart)
							rageDiff += 1f;
						else if (modPlayer.heartOfDarkness)
							rageDiff += 0.5f;

						// Apply the rage change and cap rage in both directions.
						modPlayer.rage += rageDiff;
						if (modPlayer.rage < 0)
							modPlayer.rage = 0;

						if (modPlayer.rage >= modPlayer.rageMax)
						{
							modPlayer.rage = modPlayer.rageMax;

							// Play a sound when the Rage Meter is full
							if (modPlayer.playFullRageSound)
							{
								modPlayer.playFullRageSound = false;
								Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/FullRage"), (int)player.position.X, (int)player.position.Y);
							}
						}
						else
							modPlayer.playFullRageSound = true;

						// Randomly-granted Absolute Rage buff when Rage is nearly full
						if (modPlayer.rage / modPlayer.rageMax >= CalamityPlayer.AbsoluteRageThreshold && !modPlayer.absoluteRage)
						{
							int absoluteRageChance = (modPlayer.draedonsHeart || modPlayer.heartOfDarkness) ? 2000 : 10000;
							if (Main.rand.NextBool(absoluteRageChance))
								player.AddBuff(ModContent.BuffType<AbsoluteRage>(), 18000);
						}

						// This is how much Adrenaline will be changed by this frame.
						float adrenalineDiff = 0;
						bool SCalAlive = NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>());
						bool wofAndNotHell = Main.wof >= 0 && player.position.Y < (float)((Main.maxTilesY - 200) * 16);

						// If Adrenaline Mode is currently active, you smoothly lose all adrenaline over the duration.
						if (modPlayer.adrenalineModeActive)
							adrenalineDiff = -modPlayer.adrenalineMax / CalamityPlayer.AdrenalineDuration;

						else
						{
							// If any boss is alive (or you are between DoG phases), you gain adrenaline smoothly.
							// EXCEPTION: Wall of Flesh is alive and you are not in hell. Then you don't get anything.
							if ((CalamityPlayer.areThereAnyDamnBosses || CalamityWorld.DoGSecondStageCountdown > 0) && !wofAndNotHell)
							{
								int numAdrenBoosts =
									(modPlayer.adrenalineBoostOne ? 1 : 0) +
									(modPlayer.adrenalineBoostTwo ? 1 : 0) +
									(modPlayer.adrenalineBoostThree ? 1 : 0);

								int adrenFillSeconds = 3600;
								switch (numAdrenBoosts)
								{
									default: adrenFillSeconds = 45; break; // Early game: 45 seconds
									case 1: adrenFillSeconds = 35; break; // Slime God: 35 seconds
									case 2: adrenFillSeconds = 25; break; // Astrum Deus: 25 sceonds
									case 3: adrenFillSeconds = 20; break; // Polterghast: 20 seconds
								}
								adrenalineDiff += modPlayer.adrenalineMax / (60f * adrenFillSeconds);
							}

							// If you aren't actively in a boss fight, adrenaline rapidly fades away over 2 seconds.
							else
								adrenalineDiff = -modPlayer.adrenalineMax / 120f;
						}

						// All positive adrenaline gains are multiplied by 44.44444% during the SCal fight.
						// This is likely to be changed so that SCal cancels the items directly.
						if (SCalAlive && adrenalineDiff > 0f)
							adrenalineDiff *= 4f / 9f;

						// Apply the adrenaline change and cap adrenaline in both directions.
						modPlayer.adrenaline += adrenalineDiff;
						if (modPlayer.adrenaline < 0)
							modPlayer.adrenaline = 0;

						if (modPlayer.adrenaline >= modPlayer.adrenalineMax)
						{
							modPlayer.adrenaline = modPlayer.adrenalineMax;

							// Play a sound when the Adrenaline Meter is full
							if (modPlayer.playFullAdrenalineSound)
							{
								modPlayer.playFullAdrenalineSound = false;
								Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/FullAdrenaline"), (int)player.position.X, (int)player.position.Y);
							}
						}
						else
							modPlayer.playFullAdrenalineSound = true;
					}
				}
			}

			// If Revengeance Mode is not active, then set rippers to zero
			else if (player.whoAmI == Main.myPlayer)
			{
				modPlayer.rage = 0;
				modPlayer.adrenaline = 0;
			}

			// Send Rage and Adrenaline info packets during multiplayer
			if (player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
			{
				modPlayer.packetTimer++;
				if (modPlayer.packetTimer == 60)
				{
					modPlayer.packetTimer = 0;
					modPlayer.StressPacket(false);
					modPlayer.AdrenalinePacket(false);
					modPlayer.DeathModeUnderworldTimePacket(false);
					modPlayer.DeathModeBlizzardTimePacket(false);
				}
			}
		}
		#endregion

		#region Misc Effects
		private static void MiscEffects(Player player, CalamityPlayer modPlayer, Mod mod)
		{
			if (modPlayer.stealthUIAlpha > 0f && (modPlayer.rogueStealth <= 0f || modPlayer.rogueStealthMax <= 0f))
			{
				modPlayer.stealthUIAlpha -= 0.035f;
				modPlayer.stealthUIAlpha = MathHelper.Clamp(modPlayer.stealthUIAlpha, 0f, 1f);
			}
			else if (modPlayer.stealthUIAlpha < 1f)
			{
				modPlayer.stealthUIAlpha += 0.035f;
				modPlayer.stealthUIAlpha = MathHelper.Clamp(modPlayer.stealthUIAlpha, 0f, 1f);
			}

			if (player.Calamity().andromedaState == AndromedaPlayerState.LargeRobot ||
				player.ownedProjectileCounts[ModContent.ProjectileType<RelicOfDeliveranceSpear>()] > 0)
			{
				player.controlHook = player.releaseHook = false;
			}

			if (modPlayer.andromedaCripple > 0)
			{
				player.velocity = Vector2.Clamp(player.velocity, new Vector2(-11f, -8f), new Vector2(11f, 8f));
				modPlayer.andromedaCripple--;
			}

			if (player.ownedProjectileCounts[ModContent.ProjectileType<GiantIbanRobotOfDoom>()] <= 0 &&
				player.Calamity().andromedaState != AndromedaPlayerState.Inactive)
			{
				player.Calamity().andromedaState = AndromedaPlayerState.Inactive;
			}
			
			if(player.Calamity().andromedaState == AndromedaPlayerState.LargeRobot)
			{
				player.width = 80;
				player.height = 212;
				player.position.Y -= 170;
			}
			else if (player.Calamity().andromedaState == AndromedaPlayerState.SpecialAttack)
			{
				player.width = 24;
				player.height = 98;
				player.position.Y -= 56;
			}
			else if (!player.mount.Active)
			{
				player.width = 20;
				player.height = 42;
			}

			// Proficiency level ups
			if (CalamityConfig.Instance.Proficiency)
				modPlayer.GetExactLevelUp();

			// Max mana bonuses
			player.statManaMax2 +=
				(modPlayer.permafrostsConcoction ? 50 : 0) +
				(modPlayer.pHeart ? 50 : 0) +
				(modPlayer.eCore ? 50 : 0) +
				(modPlayer.cShard ? 50 : 0) +
				(modPlayer.starBeamRye ? 50 : 0);

			// Life Steal nerf
			// Reduces normal mode life steal recovery rate from 0.6/s to 0.5/s
			// Reduces expert mode life steal recovery rate from 0.5/s to 0.35/s
			// Revengeance mode recovery rate is 0.3/s
			// Death mode recovery rate is 0.25/s
			float lifeStealCooldown = CalamityWorld.death ? 0.25f : CalamityWorld.revenge ? 0.2f : Main.expertMode ? 0.15f : 0.1f;
			/*if (Main.masterMode)
				lifeStealCooldown *= 1.25f;*/
			player.lifeSteal -= lifeStealCooldown;

			// Nebula Armor nerf
			if (player.nebulaLevelMana > 0 && player.statMana < player.statManaMax2)
			{
				int num = 12;
				modPlayer.nebulaManaNerfCounter += player.nebulaLevelMana;
				if (modPlayer.nebulaManaNerfCounter >= num)
				{
					modPlayer.nebulaManaNerfCounter -= num;
					player.statMana--;
					if (player.statMana < 0)
						player.statMana = 0;
				}
			}
			else
				modPlayer.nebulaManaNerfCounter = 0;

			// Bool for drawing boss health bar small text or not
			if (Main.myPlayer == player.whoAmI)
				BossHealthBarManager.SHOULD_DRAW_SMALLTEXT_HEALTH = modPlayer.shouldDrawSmallText;

			// Margarita halved debuff duration
			if (modPlayer.margarita)
			{
				if (Main.myPlayer == player.whoAmI)
				{
					for (int l = 0; l < Player.MaxBuffs; l++)
					{
						int hasBuff = player.buffType[l];
						if (player.buffTime[l] > 2 && CalamityLists.debuffList.Contains(hasBuff))
						{
							player.buffTime[l]--;
						}
					}
				}
			}

			// Immunity to most debuffs
			if (modPlayer.invincible)
			{
				foreach (int debuff in CalamityLists.debuffList)
					player.buffImmune[debuff] = true;
			}

			// Transformer immunity to Electrified
			if (modPlayer.aSparkRare)
				player.buffImmune[BuffID.Electrified] = true;

			// Reduce breath meter while in icy water instead of chilling
			bool canBreath = (modPlayer.sirenBoobs && NPC.downedBoss3) || player.gills || player.merman;
			if (player.arcticDivingGear || canBreath)
			{
				player.buffImmune[ModContent.BuffType<FrozenLungs>()] = true;
			}
			if (CalamityConfig.Instance.ReworkChilledWater)
			{
				if (Main.expertMode && player.ZoneSnow && player.wet && !player.lavaWet && !player.honeyWet)
				{
					player.buffImmune[BuffID.Chilled] = true;
					if (player.IsUnderwater())
					{
						if (Main.myPlayer == player.whoAmI)
						{
							player.AddBuff(ModContent.BuffType<FrozenLungs>(), 2, false);
						}
					}
				}
				if (modPlayer.iCantBreathe)
				{
					if (player.breath > 0)
						player.breath--;
				}
			}

			//extra DoT in the lava of the crags			
			if (modPlayer.ZoneCalamity && player.lavaWet)
			{
				player.AddBuff(ModContent.BuffType<CragsLava>(), 2, false);
            }

            // Acid rain droplets
            if (player.whoAmI == Main.myPlayer)
            {
                if (CalamityWorld.rainingAcid && modPlayer.ZoneSulphur && !CalamityPlayer.areThereAnyDamnBosses && player.Center.Y < Main.worldSurface * 16f + 800f)
                {
                    int slimeRainRate = (int)(MathHelper.Clamp(Main.invasionSize * 0.4f, 13.5f, 50) * 2.25);
                    Vector2 spawnPoint = new Vector2(player.Center.X + Main.rand.Next(-1000, 1001), player.Center.Y - Main.rand.Next(700, 801));

                    if (player.miscCounter % slimeRainRate == 0f)
                    {
						if (CalamityWorld.downedAquaticScourge && !CalamityWorld.downedPolterghast && Main.rand.NextBool(12))
						{
							NPC.NewNPC((int)spawnPoint.X, (int)spawnPoint.Y, ModContent.NPCType<IrradiatedSlime>());
						}
                    }
                }
            }

			//Hydrothermal blue smoke effects but it doesn't work epicccccc
			if (player.whoAmI == Main.myPlayer)
			{
				if (modPlayer.hydrothermalSmoke)
				{
					if (Math.Abs(player.velocity.X) > 0.1f || Math.Abs(player.velocity.Y) > 0.1f)
					{
						Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<HydrothermalSmoke>(), 0, 0f, player.whoAmI);
					}
				}
				//Trying to find a workaround because apparently putting the bool in ResetEffects prevents it from working
				if (!player.armorEffectDrawOutlines)
				{
					modPlayer.hydrothermalSmoke = false;
				}
			}

            // Death Mode effects
            modPlayer.caveDarkness = 0f;
			if (CalamityWorld.death)
			{
				if (player.whoAmI == Main.myPlayer)
				{
					// Calculate underground darkness here. The effect is applied in CalamityMod.ModifyLightingBrightness.
					Point point = player.Center.ToTileCoordinates();
					if (point.Y > Main.worldSurface && !modPlayer.ZoneAbyss && !player.ZoneUnderworldHeight)
					{
						// Darkness strength scales smoothly with how deep you are.
						double totalUndergroundDepth = Main.maxTilesY - 200D - Main.worldSurface;
						double playerUndergroundDepth = point.Y - Main.worldSurface;
						double depthRatio = playerUndergroundDepth / totalUndergroundDepth;
						int lightStrength = modPlayer.GetTotalLightStrength();

						// In the last 50 blocks before hell, the darkness smoothly fades away.
						float FadeAwayStart = (float)(1D - 50D / totalUndergroundDepth);
						float darknessStrength = (float)(depthRatio / FadeAwayStart);
						if (depthRatio > FadeAwayStart)
						{
							// Varies from 1.0 to 0.0 as depthRatio varies from FadeAwayStart to 1.0.
							darknessStrength = MathHelper.Lerp(0f, 0.75f, (1f - (float)depthRatio) / (1f - FadeAwayStart));
						}

						// Reduce the power of cave darkness based on your light level. 5+ is enough to totally eliminate it.
						switch (lightStrength)
						{
							case 0:
								break;
							case 1:
								darknessStrength *= 0.75f;
								break;
							case 2:
								darknessStrength *= 0.55f;
								break;
							case 3:
								darknessStrength *= 0.35f;
								break;
							case 4:
								darknessStrength *= 0.15f;
								break;
							default:
								darknessStrength = 0f;
								break;
						}
						modPlayer.caveDarkness = darknessStrength;
					}

					// Immunity bools
					bool hasMoltenSet = player.head == ArmorIDs.Head.MoltenHelmet && player.body == ArmorIDs.Body.MoltenBreastplate && player.legs == ArmorIDs.Legs.MoltenGreaves;

					bool immunityToHotAndCold = hasMoltenSet || player.magmaStone || player.frostArmor || modPlayer.fBulwark || modPlayer.fBarrier ||
						modPlayer.frostFlare || modPlayer.rampartOfDeities || modPlayer.cryogenSoul || modPlayer.snowman || modPlayer.blazingCore || modPlayer.permafrostsConcoction || modPlayer.profanedCrystalBuffs || modPlayer.coldDivinity || modPlayer.eGauntlet;

					bool immunityToCold = player.HasBuff(BuffID.Campfire) || Main.campfire || player.resistCold || modPlayer.eskimoSet || player.buffImmune[BuffID.Frozen] || modPlayer.aAmpoule || player.HasBuff(BuffID.Inferno) || immunityToHotAndCold || modPlayer.externalColdImmunity;

					bool immunityToHot = player.lavaImmune || player.lavaRose || player.lavaMax > 0 || immunityToHotAndCold || modPlayer.externalHeatImmunity;

					// Thorn and spike effects
					// 10 = crimson/corruption thorns, 17 = jungle thorns, 40 = dungeon spikes, 60 = temple spikes
					Vector2 tileType;
					if (!player.mount.Active || !player.mount.Cart)
						tileType = Collision.HurtTiles(player.position, player.velocity, player.width, player.height, player.fireWalk);
					else
						tileType = Collision.HurtTiles(player.position, player.velocity, player.width, player.height - 16, player.fireWalk);
					switch ((int)tileType.Y)
					{
						case 10:
							player.AddBuff(BuffID.Weak, 300, false);
							player.AddBuff(BuffID.Bleeding, 300, false);
							break;
						case 17:
							player.AddBuff(BuffID.Poisoned, 300, false);
							break;
						case 40:
							player.AddBuff(BuffID.Bleeding, 300, false);
							break;
						case 60:
							player.AddBuff(BuffID.Venom, 300, false);
							break;
						default:
							break;
					}

					// Astral effects
					if (modPlayer.ZoneAstral)
					{
						player.gravity *= 0.75f;
					}

					// Space effects
					if (player.InSpace())
					{
						if (Main.dayTime)
						{
							if (!immunityToHot)
								player.AddBuff(BuffID.Burning, 2, false);
						}
						else
						{
							if (!immunityToCold)
								player.AddBuff(BuffID.Frostburn, 2, false);
						}
					}

					// Leech bleed
					if (player.ZoneJungle && player.wet && !player.lavaWet && !player.honeyWet)
					{
						if (player.IsUnderwater())
							player.AddBuff(BuffID.Bleeding, 300, false);
					}

					// Ice shards, lightning and sharknadoes
					bool nearPillar =  player.PillarZone();
					if (player.ZoneOverworldHeight && !BossRushEvent.BossRushActive && !CalamityPlayer.areThereAnyDamnBosses && !CalamityPlayer.areThereAnyDamnEvents && NPC.MoonLordCountdown == 0 && !player.InSpace())
					{
						Vector2 sharknadoSpawnPoint = new Vector2(player.Center.X - Main.rand.Next(300, 701), player.Center.Y - Main.rand.Next(700, 801));
						if (point.X > Main.maxTilesX / 2)
							sharknadoSpawnPoint.X = player.Center.X + Main.rand.Next(300, 701);

						if (Main.raining)
						{
							float frequencyMult = (1f - Main.cloudAlpha) * CalamityConfig.Instance.DeathWeatherMultiplier; // 3 to 0.055

							Vector2 spawnPoint = new Vector2(player.Center.X + Main.rand.Next(-1000, 1001), player.Center.Y - Main.rand.Next(700, 801));
							Tile tileSafely = Framing.GetTileSafely((int)(spawnPoint.X / 16f), (int)(spawnPoint.Y / 16f));

							if (player.ZoneSnow)
							{
								if (!tileSafely.active())
								{
									int divisor = (int)((Main.hardMode ? 50f : 60f) * frequencyMult);
									float windVelocity = (float)Math.Sqrt(Math.Abs(Main.windSpeed)) * Math.Sign(Main.windSpeed) * (Main.cloudAlpha + 0.5f) * 25f + Main.rand.NextFloat() * 0.2f - 0.1f;
									Vector2 velocity = new Vector2(windVelocity * 0.2f, 3f * Main.rand.NextFloat());

									if (player.miscCounter % divisor == 0 && Main.rand.NextBool(3))
										Projectile.NewProjectile(spawnPoint, velocity, ModContent.ProjectileType<IceRain>(), 20, 0f, player.whoAmI, 2f, 0f);
								}
							}
							else
							{
								if (player.ZoneBeach && !modPlayer.ZoneSulphur)
								{
									int randomFrequency = (int)(50f * frequencyMult);
									if (player.miscCounter == 280 && Main.rand.NextBool(randomFrequency) && player.ownedProjectileCounts[ProjectileID.Cthulunado] < 1)
									{
										Main.PlaySound(SoundID.NPCDeath19, (int)sharknadoSpawnPoint.X, (int)sharknadoSpawnPoint.Y);
										int y = (int)(sharknadoSpawnPoint.Y / 16f);
										int x = (int)(sharknadoSpawnPoint.X / 16f);
										int yAdjust = 100;
										if (x < 10)
											x = 10;
										if (x > Main.maxTilesX - 10)
											x = Main.maxTilesX - 10;
										if (y < 10)
											y = 10;
										if (y > Main.maxTilesY - yAdjust - 10)
											y = Main.maxTilesY - yAdjust - 10;

										int spawnAreaY = Main.maxTilesY - y;
										for (int j = y; j < y + spawnAreaY; j++)
										{
											Tile tile = Main.tile[x, j];
											if ((tile.active() && Main.tileSolid[tile.type]) || tile.liquid >= 200)
											{
												y = j;
												break;
											}
										}

										int tornado = Projectile.NewProjectile(x * 16 + 8, y * 16 - 24, 0f, 0f, ProjectileID.Cthulunado, 50, 4f, player.whoAmI, 16f, 24f);
										Main.projectile[tornado].netUpdate = true;
									}
								}

								int randomFrequency2 = (int)(20f * frequencyMult);
                                if (CalamityWorld.rainingAcid && player.Calamity().ZoneSulphur)
                                    randomFrequency2 = (int)(randomFrequency2 * 3.75);
                                if (player.miscCounter % (Main.hardMode ? 90 : 120) == 0 && Main.rand.NextBool(randomFrequency2))
								{
									if (!tileSafely.active())
									{
										float randomVelocity = Main.rand.NextFloat() - 0.5f;
										Vector2 fireTo = new Vector2(spawnPoint.X + 100f * randomVelocity, spawnPoint.Y + 900f);
										Vector2 direction = fireTo - spawnPoint;
										Vector2 velocity = Vector2.Normalize(direction) * 12f;
										Projectile.NewProjectile(spawnPoint.X, spawnPoint.Y, 0f, velocity.Y, ModContent.ProjectileType<LightningMark>(), 0, 0f, player.whoAmI, 0f, 0f);
									}
								}
							}
						}
						else
						{
							if (player.ZoneBeach && !modPlayer.ZoneSulphur)
							{
								if (player.miscCounter == 280 && Main.rand.NextBool(10) && player.ownedProjectileCounts[ProjectileID.Sharknado] < 1)
								{
									Main.PlaySound(SoundID.NPCDeath19, sharknadoSpawnPoint);
									int y = (int)(sharknadoSpawnPoint.Y / 16f);
									int x = (int)(sharknadoSpawnPoint.X / 16f);
									int num333 = 100;
									if (x < 10)
										x = 10;
									if (x > Main.maxTilesX - 10)
										x = Main.maxTilesX - 10;
									if (y < 10)
										y = 10;
									if (y > Main.maxTilesY - num333 - 10)
										y = Main.maxTilesY - num333 - 10;

									int spawnAreaY = Main.maxTilesY - y;
									for (int j = y; j < y + spawnAreaY; j++)
									{
										Tile tile = Main.tile[x, j];
										if ((tile.active() && Main.tileSolid[tile.type]) || tile.liquid >= 200)
										{
											y = j;
											break;
										}
									}

									int tornado = Projectile.NewProjectile(x * 16 + 8, y * 16 - 24, 0.01f, 0f, ProjectileID.Sharknado, 25, 4f, player.whoAmI, 16f, 15f);
									Main.projectile[tornado].netUpdate = true;
								}
							}
						}
					}

					// Cold timer
					if (!player.behindBackWall && Main.raining && player.ZoneSnow && !immunityToCold && player.ZoneOverworldHeight)
					{
						bool affectedByColdWater = player.wet && !player.lavaWet && !player.honeyWet && !player.arcticDivingGear;

						player.AddBuff(ModContent.BuffType<DeathModeCold>(), 2, false);

						modPlayer.deathModeBlizzardTime++;
						if (affectedByColdWater)
							modPlayer.deathModeBlizzardTime++;

						if (modPlayer.deathModeUnderworldTime > 0)
						{
							modPlayer.deathModeUnderworldTime--;
							if (affectedByColdWater)
								modPlayer.deathModeUnderworldTime--;
							if (modPlayer.deathModeUnderworldTime < 0)
								modPlayer.deathModeUnderworldTime = 0;
						}
					}
					else if (modPlayer.deathModeBlizzardTime > 0)
					{
						modPlayer.deathModeBlizzardTime--;
						if (immunityToCold)
							modPlayer.deathModeBlizzardTime--;
						if (modPlayer.deathModeBlizzardTime < 0)
							modPlayer.deathModeBlizzardTime = 0;
					}

					// Hot timer
					if (player.ZoneUnderworldHeight && !immunityToHot)
					{
						bool affectedByHotLava = player.lavaWet;

						player.AddBuff(ModContent.BuffType<DeathModeHot>(), 2, false);

						modPlayer.deathModeUnderworldTime++;
						if (affectedByHotLava)
							modPlayer.deathModeUnderworldTime++;

						if (modPlayer.deathModeBlizzardTime > 0)
						{
							modPlayer.deathModeBlizzardTime--;
							if (affectedByHotLava)
								modPlayer.deathModeBlizzardTime--;
							if (modPlayer.deathModeBlizzardTime < 0)
								modPlayer.deathModeBlizzardTime = 0;
						}
					}
					else if (modPlayer.deathModeUnderworldTime > 0)
					{
						modPlayer.deathModeUnderworldTime--;
						if (immunityToHot)
							modPlayer.deathModeUnderworldTime--;
						if (modPlayer.deathModeUnderworldTime < 0)
							modPlayer.deathModeUnderworldTime = 0;
					}

					// Cold effects
					if (modPlayer.deathModeBlizzardTime > 1800)
						player.AddBuff(BuffID.Frozen, 2, false);
					if (modPlayer.deathModeBlizzardTime > 1980)
						modPlayer.KillPlayer();

					// Hot effects
					if (modPlayer.deathModeUnderworldTime > 360)
						player.AddBuff(BuffID.Weak, 2, false);
					if (modPlayer.deathModeUnderworldTime > 720)
						player.AddBuff(BuffID.Slow, 2, false);
					if (modPlayer.deathModeUnderworldTime > 1080)
						player.AddBuff(BuffID.OnFire, 2, false);
					if (modPlayer.deathModeUnderworldTime > 1440)
						player.AddBuff(BuffID.Confused, 2, false);
					if (modPlayer.deathModeUnderworldTime > 1800)
						player.AddBuff(BuffID.Burning, 2, false);
				}
			}

			// Increase fall speed
			if (!player.mount.Active)
			{
				if (player.IsUnderwater() && modPlayer.ironBoots)
					player.maxFallSpeed = 9f;
				if (modPlayer.aeroSet && !player.wet)
					player.maxFallSpeed = 15f;
				if (modPlayer.gSabatonFall > 0 && !player.wet)
					player.maxFallSpeed = 20f;
				if (modPlayer.normalityRelocator)
					player.maxFallSpeed *= 1.1f;
				if (modPlayer.etherealExtorter && player.ZoneSkyHeight)
					player.maxFallSpeed *= 1.25f;
			}

			// Normality Relocator bonus
			if (modPlayer.normalityRelocator)
				player.moveSpeed += 0.1f;

			// Omega Blue Armor bonus
			if (modPlayer.omegaBlueSet)
			{
				// Add tentacles
				if (player.ownedProjectileCounts[ModContent.ProjectileType<OmegaBlueTentacle>()] < 6 && Main.myPlayer == player.whoAmI)
				{
					bool[] tentaclesPresent = new bool[6];
					for (int i = 0; i < Main.maxProjectiles; i++)
					{
						Projectile projectile = Main.projectile[i];
						if (projectile.active && projectile.type == ModContent.ProjectileType<OmegaBlueTentacle>() && projectile.owner == Main.myPlayer && projectile.ai[1] >= 0f && projectile.ai[1] < 6f)
							tentaclesPresent[(int)projectile.ai[1]] = true;
					}

					for (int i = 0; i < 6; i++)
					{
						if (!tentaclesPresent[i])
						{
							int damage = (int)(666 * player.AverageDamage());
							Vector2 vel = new Vector2(Main.rand.Next(-13, 14), Main.rand.Next(-13, 14)) * 0.25f;
							Projectile.NewProjectile(player.Center, vel, ModContent.ProjectileType<OmegaBlueTentacle>(), damage, 8f, Main.myPlayer, Main.rand.Next(120), i);
						}
					}
				}

				float damageUp = 0.1f;
				int critUp = 10;
				if (modPlayer.omegaBlueHentai)
				{
					damageUp *= 2f;
					critUp *= 2;
				}
				player.allDamage += damageUp;
				modPlayer.AllCritBoost(critUp);
			}
			bool canProvideBuffs = modPlayer.profanedCrystalBuffs || (!modPlayer.profanedCrystal && modPlayer.pArtifact) || (modPlayer.profanedCrystal && CalamityWorld.downedSCal);
			bool attack = player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianAttack>()] > 0;
			// Guardian bonuses if not burnt out
			if (!modPlayer.bOut && canProvideBuffs)
			{
				bool healer = player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianHealer>()] > 0;
				bool defend = player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianDefense>()] > 0;
				if (healer)
				{
					if (modPlayer.healCounter > 0)
						modPlayer.healCounter--;

					if (modPlayer.healCounter <= 0)
					{
						bool enrage = player.statLife <= (int)(player.statLifeMax2 * 0.5);
						modPlayer.healCounter = (!enrage && modPlayer.profanedCrystalBuffs) ? 360 : 300;
						if (player.whoAmI == Main.myPlayer)
						{
							int healAmount = 5 +
								(defend ? 5 : 0) +
								(attack ? 5 : 0);

							player.statLife += healAmount;
							player.HealEffect(healAmount);
						}
					}
				}

				if (defend)
				{
					player.moveSpeed += 0.1f +
						(attack ? 0.1f : 0f);
					player.endurance += 0.025f +
						(attack ? 0.025f : 0f);
				}

				if (attack)
				{
					player.minionDamage += 0.1f +
						(defend ? 0.05f : 0f);
				}
			}

			// You always get the max minions, even during the effect of the burnout debuff
			if (attack && canProvideBuffs)
				player.maxMinions++;

			// Cooldowns and timers
			if (modPlayer.gainRageCooldown > 0)
				modPlayer.gainRageCooldown--;
			if (modPlayer.KameiBladeUseDelay > 0)
				modPlayer.KameiBladeUseDelay--;
			if (modPlayer.galileoCooldown > 0)
				modPlayer.galileoCooldown--;
			if (modPlayer.soundCooldown > 0)
				modPlayer.soundCooldown--;
			if (modPlayer.shadowPotCooldown > 0)
				modPlayer.shadowPotCooldown--;
			if (modPlayer.raiderCooldown > 0)
				modPlayer.raiderCooldown--;
			if (modPlayer.gSabatonCooldown > 0)
				modPlayer.gSabatonCooldown--;
			if (modPlayer.gSabatonFall > 0)
				modPlayer.gSabatonFall--;
			if (modPlayer.astralStarRainCooldown > 0)
				modPlayer.astralStarRainCooldown--;
			if (modPlayer.bloodflareMageCooldown > 0)
				modPlayer.bloodflareMageCooldown--;
			if (modPlayer.tarraMageHealCooldown > 0)
				modPlayer.tarraMageHealCooldown--;
			if (modPlayer.featherCrownCooldown > 0)
				modPlayer.featherCrownCooldown--;
			if (modPlayer.moonCrownCooldown > 0)
				modPlayer.moonCrownCooldown--;
			if (modPlayer.nanoFlareCooldown > 0)
				modPlayer.nanoFlareCooldown--;
			if (modPlayer.spectralVeilImmunity > 0)
				modPlayer.spectralVeilImmunity--;
			if (modPlayer.jetPackCooldown > 0)
				modPlayer.jetPackCooldown--;
			if (modPlayer.plaguedFuelPackDash > 0)
				modPlayer.plaguedFuelPackDash--;
			if (modPlayer.blunderBoosterDash > 0)
				modPlayer.blunderBoosterDash--;
			if (modPlayer.theBeeCooldown > 0)
				modPlayer.theBeeCooldown--;
			if (modPlayer.jellyDmg > 0f)
				modPlayer.jellyDmg -= 1f;
			if (modPlayer.ataxiaDmg > 0f)
				modPlayer.ataxiaDmg -= 1.5f;
			if (modPlayer.ataxiaDmg < 0f)
				modPlayer.ataxiaDmg = 0f;
			if (modPlayer.xerocDmg > 0f)
				modPlayer.xerocDmg -= 2f;
			if (modPlayer.xerocDmg < 0f)
				modPlayer.xerocDmg = 0f;
			if (modPlayer.godSlayerDmg > 0f)
				modPlayer.godSlayerDmg -= 2.5f;
			if (modPlayer.godSlayerDmg < 0f)
				modPlayer.godSlayerDmg = 0f;
			if (modPlayer.aBulwarkRareMeleeBoostTimer > 0)
				modPlayer.aBulwarkRareMeleeBoostTimer--;
			if (modPlayer.bossRushImmunityFrameCurseTimer > 0)
				modPlayer.bossRushImmunityFrameCurseTimer--;
			if (modPlayer.gaelRageCooldown > 0)
				modPlayer.gaelRageCooldown--;
			if (modPlayer.projRefRareLifeRegenCounter > 0)
				modPlayer.projRefRareLifeRegenCounter--;
			if (modPlayer.hurtSoundTimer > 0)
				modPlayer.hurtSoundTimer--;
			if (modPlayer.icicleCooldown > 0)
				modPlayer.icicleCooldown--;
			if (modPlayer.statisTimer > 0 && player.dashDelay >= 0)
				modPlayer.statisTimer = 0;
			if (modPlayer.hallowedRuneCooldown > 0)
				modPlayer.hallowedRuneCooldown--;
			if (modPlayer.sulphurBubbleCooldown > 0)
				modPlayer.sulphurBubbleCooldown--;
			if (modPlayer.forbiddenCooldown > 0)
				modPlayer.forbiddenCooldown--;
			if (modPlayer.tornadoCooldown > 0)
				modPlayer.tornadoCooldown--;
			if (modPlayer.ladHearts > 0)
				modPlayer.ladHearts--;
			if (modPlayer.titanBoost > 0)
				modPlayer.titanBoost--;
			if (modPlayer.prismaticLasers > 0)
				modPlayer.prismaticLasers--;
			if (modPlayer.dogTextCooldown > 0)
				modPlayer.dogTextCooldown--;
			if (modPlayer.titanCooldown > 0)
				modPlayer.titanCooldown--;
			if (modPlayer.omegaBlueCooldown > 0)
				modPlayer.omegaBlueCooldown--;
			if (modPlayer.plagueReaperCooldown > 0)
				modPlayer.plagueReaperCooldown--;
			if (modPlayer.roverDrive)
			{
				if (modPlayer.roverDriveTimer < CalamityUtils.SecondsToFrames(30f))
					modPlayer.roverDriveTimer++;
				if (modPlayer.roverDriveTimer >= CalamityUtils.SecondsToFrames(30f))
					modPlayer.roverDriveTimer = 0;
			}
			else
				modPlayer.roverDriveTimer = 616; //Doesn't reset to zero to prevent exploits
			if (modPlayer.auralisAurora > 0)
				modPlayer.auralisAurora--;
			if (modPlayer.auralisAuroraCooldown > 0)
				modPlayer.auralisAuroraCooldown--;

			// Silva invincibility effects
			if (modPlayer.silvaCountdown > 0 && modPlayer.hasSilvaEffect && modPlayer.silvaSet)
			{
				foreach (int debuff in CalamityLists.debuffList)
					player.buffImmune[debuff] = true;

				player.buffImmune[ModContent.BuffType<VulnerabilityHex>()] = true;
				modPlayer.silvaCountdown -= modPlayer.auricSet ? 2 : 1;
				if (modPlayer.silvaCountdown <= 0)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaDispel"), player.Center);

				for (int j = 0; j < 2; j++)
				{
					int green = Dust.NewDust(player.position, player.width, player.height, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 2f);
					Main.dust[green].position.X += (float)Main.rand.Next(-20, 21);
					Main.dust[green].position.Y += (float)Main.rand.Next(-20, 21);
					Main.dust[green].velocity *= 0.9f;
					Main.dust[green].noGravity = true;
					Main.dust[green].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
					Main.dust[green].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
					if (Main.rand.NextBool(2))
						Main.dust[green].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
				}
			}

			// Tarragon cloak effects
			if (modPlayer.tarragonCloak)
			{
				modPlayer.tarraDefenseTime--;
				if (modPlayer.tarraDefenseTime <= 0)
				{
					modPlayer.tarraDefenseTime = 600;
					if (player.whoAmI == Main.myPlayer)
						player.AddBuff(ModContent.BuffType<TarragonCloakCooldown>(), 1800, false);
				}

				for (int j = 0; j < 2; j++)
				{
					int green = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 2f);
					Dust dust = Main.dust[green];
					dust.position.X += (float)Main.rand.Next(-20, 21);
					dust.position.Y += (float)Main.rand.Next(-20, 21);
					dust.velocity *= 0.9f;
					dust.noGravity = true;
					dust.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
					dust.shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
					if (Main.rand.NextBool(2))
						dust.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
				}
			}

			// Tarragon immunity effects
			if (modPlayer.tarraThrowing)
			{
				if (modPlayer.tarragonImmunity)
				{
					player.immune = true;
					player.immuneTime = 2;
				}

				if (modPlayer.tarraThrowingCrits >= 25)
				{
					modPlayer.tarraThrowingCrits = 0;
					if (player.whoAmI == Main.myPlayer)
						player.AddBuff(ModContent.BuffType<TarragonImmunity>(), 300, false);
				}

				for (int l = 0; l < Player.MaxBuffs; l++)
				{
					int hasBuff = player.buffType[l];
					if (player.buffTime[l] <= 2 && hasBuff == ModContent.BuffType<TarragonImmunity>())
					{
						if (player.whoAmI == Main.myPlayer)
							player.AddBuff(ModContent.BuffType<TarragonImmunityCooldown>(), 1500, false);
					}

					bool shouldAffect = CalamityLists.debuffList.Contains(hasBuff);
					if (shouldAffect)
						modPlayer.throwingDamage += 0.1f;
				}
			}

			// Bloodflare pickup spawn cooldowns
			if (modPlayer.bloodflareSet)
			{
				if (modPlayer.bloodflareHeartTimer > 0)
					modPlayer.bloodflareHeartTimer--;
				if (modPlayer.bloodflareManaTimer > 0)
					modPlayer.bloodflareManaTimer--;
			}

			// Bloodflare frenzy effects
			if (modPlayer.bloodflareMelee)
			{
				if (modPlayer.bloodflareMeleeHits >= 15)
				{
					modPlayer.bloodflareMeleeHits = 0;
					if (player.whoAmI == Main.myPlayer)
						player.AddBuff(ModContent.BuffType<BloodflareBloodFrenzy>(), 302, false);
				}

				if (modPlayer.bloodflareFrenzy)
				{
					for (int l = 0; l < Player.MaxBuffs; l++)
					{
						int hasBuff = player.buffType[l];
						if (player.buffTime[l] <= 2 && hasBuff == ModContent.BuffType<BloodflareBloodFrenzy>())
						{
							if (player.whoAmI == Main.myPlayer)
								player.AddBuff(ModContent.BuffType<BloodflareBloodFrenzyCooldown>(), 1800, false);
						}
					}

					player.meleeCrit += 25;
					player.meleeDamage += 0.25f;

					for (int j = 0; j < 2; j++)
					{
						int blood = Dust.NewDust(player.position, player.width, player.height, DustID.Blood, 0f, 0f, 100, default, 2f);
						Dust dust = Main.dust[blood];
						dust.position.X += (float)Main.rand.Next(-20, 21);
						dust.position.Y += (float)Main.rand.Next(-20, 21);
						dust.velocity *= 0.9f;
						dust.noGravity = true;
						dust.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
						dust.shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
						if (Main.rand.NextBool(2))
							dust.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
					}
				}
			}

			// Raider Talisman bonus
			if (modPlayer.raiderTalisman)
			{
				float damageMult = modPlayer.nanotech ? 0.1f : 0.15f;
				modPlayer.throwingDamage += (float)modPlayer.raiderStack / 150f * damageMult;
			}

			// Silva minion bonus
			if (modPlayer.silvaCountdown <= 0 && modPlayer.hasSilvaEffect && modPlayer.silvaSummon)
				player.maxMinions += 2;

			// Spirit Glyph defense buff
			if (modPlayer.sDefense)
			{
				player.statDefense += 5;
				player.endurance += 0.05f;
			}

            // Hallowed Rune defense buff
            if (modPlayer.hallowedDefense)
            {
                player.statDefense += 7;
                player.endurance += 0.07f;
            }

			if (modPlayer.kamiBoost)
			{
				player.allDamage += 0.3f;
				player.statDefense += 20;
			}

			if (modPlayer.roverDriveTimer < 616)
			{
				player.statDefense += 10;
				if (modPlayer.roverDriveTimer > 606)
					player.statDefense -= modPlayer.roverDriveTimer - 606; //so it scales down when the shield dies
			}

			// Absorber bonus
			if (modPlayer.absorber)
			{
				player.moveSpeed += 0.12f;
				player.jumpSpeedBoost += 1.2f;
				player.thorns += 0.5f;
				player.endurance += 0.05f;

				if (player.StandingStill() && player.itemAnimation == 0)
					player.manaRegenBonus += 2;
			}

			// Sea Shell bonus
			if (modPlayer.seaShell)
			{
				if (player.IsUnderwater())
				{
					player.statDefense += modPlayer.absorber ? 5 : 3;
					player.endurance += 0.05f;
					player.moveSpeed += modPlayer.absorber ? 0.2f : 0.15f;
					player.ignoreWater = true;
				}
			}

			// Laudanum bonus
			if (modPlayer.laudanum)
			{
				if (Main.myPlayer == player.whoAmI)
				{
					for (int l = 0; l < Player.MaxBuffs; l++)
					{
						int hasBuff = player.buffType[l];
						if (!modPlayer.doubledHorror && hasBuff == ModContent.BuffType<Horror>())
						{
							player.buffTime[l] *= 2;
							modPlayer.doubledHorror = true;
							break;
						}
					}
				}
				if (modPlayer.horror)
				{
					player.statDefense += 15;
					player.allDamage += 0.1f;
					player.moveSpeed += 0.15f;
					player.nightVision = true;
				}
			}
			if (!modPlayer.horror)
				modPlayer.doubledHorror = false;

			// Draedon's Heart bonus
			if (modPlayer.draedonsHeart)
			{
				player.allDamage += 0.05f;
				if (player.StandingStill() && player.itemAnimation == 0)
					player.statDefense += 25;
			}

			// Remove Absolute Rage buff if Rage isn't high enough
			bool rageHighEnough = modPlayer.rage / modPlayer.rageMax >= CalamityPlayer.AbsoluteRageThreshold;
			if (!rageHighEnough && player.FindBuffIndex(ModContent.BuffType<AbsoluteRage>()) > -1)
				player.ClearBuff(ModContent.BuffType<AbsoluteRage>());

			// Absolute Rage bonus
			if (modPlayer.absoluteRage)
			{
				if (modPlayer.heartOfDarkness || modPlayer.draedonsHeart)
					player.allDamage += 0.1f;
			}

			// Affliction bonus
			if (modPlayer.affliction || modPlayer.afflicted)
			{
				player.endurance += 0.07f;
				player.statDefense += 20;
				player.allDamage += 0.12f;
			}

			// Ambrosial Ampoule bonus and other light-granting bonuses
			float[] light = new float[3];
			if (modPlayer.aAmpoule)
			{
				light[0] += 1f;
				light[1] += 1f;
				light[2] += 0.6f;
				player.endurance += 0.05f;
				player.pickSpeed -= 0.25f;
				player.buffImmune[BuffID.Venom] = true;
				player.buffImmune[BuffID.Frozen] = true;
				player.buffImmune[BuffID.Chilled] = true;
				player.buffImmune[BuffID.Frostburn] = true;
				player.buffImmune[BuffID.Poisoned] = true;
			}
			if (modPlayer.cFreeze)
			{
				light[0] += 0.3f;
				light[1] += Main.DiscoG / 400f;
				light[2] += 0.5f;
			}
			if (modPlayer.sirenIce)
			{
				light[0] += 0.35f;
				light[1] += 1f;
				light[2] += 1.25f;
			}
			if (modPlayer.sirenBoobs)
			{
				light[0] += 0.1f;
				light[1] += 1f;
				light[2] += 1.5f;
			}
			if (modPlayer.tarraSummon)
			{
				light[0] += 0f;
				light[1] += 3f;
				light[2] += 0f;
			}
			if (modPlayer.dAmulet)
			{
				if (player.IsUnderwater())
				{
					light[0] += 1.35f;
					light[1] += 0.3f;
					light[2] += 0.9f;
				}
			}
			if (modPlayer.forbiddenCirclet)
			{
				light[0] += 0.8f;
				light[1] += 0.7f;
				light[2] += 0.2f;
			}
			Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), light[0], light[1], light[2]);

			// Blazing Core bonus
			if (modPlayer.blazingCore)
				player.endurance += 0.1f;

			//Permafrost's Concoction bonuses/debuffs
			if (modPlayer.permafrostsConcoction)
				player.manaCost *= 0.85f;

			if (modPlayer.encased)
			{
				player.statDefense += 30;
				player.frozen = true;
				player.velocity.X = 0f;
				player.velocity.Y = -0.4f; //should negate gravity

				int ice = Dust.NewDust(player.position, player.width, player.height, 88);
				Main.dust[ice].noGravity = true;
				Main.dust[ice].velocity *= 2f;

				player.buffImmune[BuffID.Frozen] = true;
				player.buffImmune[BuffID.Chilled] = true;
				player.buffImmune[ModContent.BuffType<GlacialState>()] = true;
			}

			// Cosmic Discharge Cosmic Freeze buff, gives surrounding enemies the Glacial State debuff
			if (modPlayer.cFreeze)
			{
				int buffType = ModContent.BuffType<GlacialState>();
				float freezeDist = 200f;
				if (player.whoAmI == Main.myPlayer)
				{
					if (Main.rand.NextBool(5))
					{
						for (int l = 0; l < Main.maxNPCs; l++)
						{
							NPC npc = Main.npc[l];
							if (!npc.active || npc.friendly || npc.damage <= 0 || npc.dontTakeDamage)
								continue;
							if (!npc.buffImmune[buffType] && Vector2.Distance(player.Center, npc.Center) <= freezeDist)
							{
								if (npc.FindBuffIndex(buffType) == -1)
									npc.AddBuff(buffType, 120, false);
							}
						}
					}
				}
			}

			// Remove Purified Jam and Lul accessory thorn damage exploits
			if (modPlayer.invincible || modPlayer.lol)
			{
				player.thorns = 0f;
				player.turtleThorns = false;
			}

			// Vortex Armor nerf
			if (player.vortexStealthActive)
			{
				player.rangedDamage -= (1f - player.stealth) * 0.4f; // Change 80 to 40
				player.rangedCrit -= (int)((1f - player.stealth) * 5f); // Change 20 to 15
			}

			// Polaris fish stuff
			if (modPlayer.polarisBoost)
			{
				player.endurance += 0.01f;
				player.statDefense += 2;
			}
			if (!modPlayer.polarisBoost || player.ActiveItem().type != ModContent.ItemType<PolarisParrotfish>())
			{
				modPlayer.polarisBoost = false;
				if (player.FindBuffIndex(ModContent.BuffType<PolarisBuff>()) > -1)
					player.ClearBuff(ModContent.BuffType<PolarisBuff>());

				modPlayer.polarisBoostCounter = 0;
				modPlayer.polarisBoostTwo = false;
				modPlayer.polarisBoostThree = false;
			}
			if (modPlayer.polarisBoostCounter >= 20)
			{
				modPlayer.polarisBoostTwo = false;
				modPlayer.polarisBoostThree = true;
			}
			else if (modPlayer.polarisBoostCounter >= 10)
				modPlayer.polarisBoostTwo = true;

			// Lore bonuses
			if (modPlayer.kingSlimeLore)
			{
				player.moveSpeed += 0.05f;
				player.jumpSpeedBoost += player.autoJump ? 0f : 0.1f;
				player.statDefense -= 3;
			}
			if (modPlayer.desertScourgeLore)
			{
				if (player.ZoneDesert || player.Calamity().ZoneSunkenSea)
				{
					player.statDefense += 5;
					player.allDamage -= 0.025f;
				}
			}
			if (modPlayer.crabulonLore)
			{
				if (player.ZoneGlowshroom || player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight)
				{
					if (Main.myPlayer == player.whoAmI)
						player.AddBuff(ModContent.BuffType<Mushy>(), 2);

					player.moveSpeed -= 0.1f;
				}
			}
			if (modPlayer.eaterOfWorldsLore)
			{
				int damage = (int)(15 * player.AverageDamage());
				float knockBack = 1f;
				if (Main.rand.NextBool(15))
				{
					int projCount = 0;
					for (int i = 0; i < Main.maxProjectiles; i++)
					{
						if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<TheDeadlyMicrobeProjectile>())
							projCount++;
					}

					if (Main.rand.Next(15) >= projCount && projCount < 6)
					{
						int loopAmt = 50;
						int num3 = 24;
						int num4 = 90;

						for (int j = 0; j < loopAmt; j++)
						{
							int sourceVariance = Main.rand.Next(200 - j * 2, 400 + j * 2);
							Vector2 center = player.Center;
							center.X += (float)Main.rand.Next(-sourceVariance, sourceVariance + 1);
							center.Y += (float)Main.rand.Next(-sourceVariance, sourceVariance + 1);

							if (!Collision.SolidCollision(center, num3, num3) && !Collision.WetCollision(center, num3, num3))
							{
								center.X += (float)(num3 / 2);
								center.Y += (float)(num3 / 2);

								if (Collision.CanHit(new Vector2(player.Center.X, player.position.Y), 1, 1, center, 1, 1) || Collision.CanHit(new Vector2(player.Center.X, player.position.Y - 50f), 1, 1, center, 1, 1))
								{
									int xSpawn = (int)center.X / 16;
									int ySpawn = (int)center.Y / 16;
									bool shouldSpawn = false;

									if (Main.rand.NextBool(3) && Main.tile[xSpawn, ySpawn] != null && Main.tile[xSpawn, ySpawn].wall > 0)
										shouldSpawn = true;
									else
									{
										center.X -= (float)(num4 / 2);
										center.Y -= (float)(num4 / 2);

										if (Collision.SolidCollision(center, num4, num4))
										{
											center.X += (float)(num4 / 2);
											center.Y += (float)(num4 / 2);
											shouldSpawn = true;
										}
									}

									if (shouldSpawn)
									{
										for (int k = 0; k < Main.maxProjectiles; k++)
										{
											if (Main.projectile[k].active && Main.projectile[k].owner == player.whoAmI && Main.projectile[k].type == ModContent.ProjectileType<TheDeadlyMicrobeProjectile>() && (center - Main.projectile[k].Center).Length() < 48f)
											{
												shouldSpawn = false;
												break;
											}
										}

										if (shouldSpawn && Main.myPlayer == player.whoAmI)
											Projectile.NewProjectile(center, Vector2.Zero, ModContent.ProjectileType<TheDeadlyMicrobeProjectile>(), damage, knockBack, player.whoAmI, 0f, 0f);
									}
								}
							}
						}
					}
				}
			}
			if (modPlayer.skeletronLore)
			{
				player.allDamage += 0.1f;
				modPlayer.AllCritBoost(5);
			}
			if (modPlayer.destroyerLore)
			{
				player.pickSpeed -= 0.05f;
			}
			if (modPlayer.dashMod == 6) // Cryogen lore
			{
				player.statDefense -= 10;
			}
			if (modPlayer.aquaticScourgeLore)
			{
				if (player.wellFed)
				{
					player.statDefense += 1;
					player.allDamage += 0.025f;
					modPlayer.AllCritBoost(1);
					player.minionKB += 0.25f;
					player.moveSpeed += 0.1f;
				}
				else
				{
					player.statDefense -= 1;
					player.allDamage -= 0.025f;
					modPlayer.AllCritBoost(-1);
					player.minionKB -= 0.25f;
					player.moveSpeed -= 0.1f;
				}
			}
			if (modPlayer.skeletronPrimeLore)
			{
				player.armorPenetration += 5;
			}
			if (modPlayer.leviathanAndSirenLore)
			{
				if (!player.IsUnderwater())
				{
					player.statDefense -= 8;
					player.endurance -= 0.05f;
				}
				if (modPlayer.sirenPet)
				{
					player.spelunkerTimer += 1;
					if (player.spelunkerTimer >= 10)
					{
						player.spelunkerTimer = 0;
						int distance = 30;
						int i = (int)player.Center.X / 16;
						int j = (int)player.Center.Y / 16;

						for (int x = i - distance; x <= i + distance; x++)
						{
							for (int y = j - distance; y <= j + distance; y++)
							{
								Tile tile = Main.tile[x, y];
								if (Main.rand.NextBool(4))
								{
									Vector2 vector = new Vector2((float)(i - x), (float)(j - y));
									if (vector.Length() < (float)distance && x > 0 && x < Main.maxTilesX - 1 && y > 0 && y < Main.maxTilesY - 1 && tile != null && tile.active())
									{
										bool shouldSpawnDust = false;
										//These checks are for money piles. They share a sheet with other background objects so it checks for frames.
										if (tile.type == TileID.SmallPiles && tile.frameY == 18)
										{
											if (tile.frameX >= 576 && tile.frameX <= 882)
												shouldSpawnDust = true;
										}
										else if (tile.type == TileID.LargePiles && tile.frameX >= 864 && tile.frameX <= 1170)
											shouldSpawnDust = true;

										if (shouldSpawnDust || Main.tileSpelunker[tile.type] || (Main.tileAlch[tile.type] && tile.type != TileID.ImmatureHerbs))
										{
											int sparkle = Dust.NewDust(new Vector2((x * 16f), (y * 16f)), 16, 16, 204, 0f, 0f, 150, default, 0.3f);
											Main.dust[sparkle].fadeIn = 0.75f;
											Main.dust[sparkle].velocity *= 0.1f;
											Main.dust[sparkle].noLight = true;
										}
									}
								}
							}
						}
					}
				}
			}
			if (player.ZoneSkyHeight)
			{
				if (modPlayer.astrumDeusLore)
					player.moveSpeed += 0.2f;
				if (modPlayer.astrumAureusLore)
					player.jumpSpeedBoost += 0.5f;
			}
			if (modPlayer.golemLore)
			{
				if (player.StandingStill() && player.itemAnimation == 0)
					player.statDefense += 10;
			}
			if (modPlayer.dukeFishronLore)
			{
				if (player.IsUnderwater())
				{
					player.allDamage += 0.05f;
					modPlayer.AllCritBoost(5);
					player.moveSpeed += 0.1f;
				}
				else
				{
					player.allDamage -= 0.02f;
					modPlayer.AllCritBoost(-2);
					player.moveSpeed -= 0.04f;
				}
			}
			if (modPlayer.lunaticCultistLore)
			{
				player.blind = true;
				player.endurance += 0.04f;
				player.statDefense += 4;
				player.allDamage += 0.04f;
				modPlayer.AllCritBoost(4);
				player.minionKB += 0.5f;
				player.moveSpeed += 0.1f;
			}
			if (modPlayer.moonLordLore)
			{
				if (player.gravDir == -1f && player.gravControl2)
				{
					player.endurance += 0.05f;
					player.statDefense += 10;
					player.allDamage += 0.1f;
					modPlayer.AllCritBoost(10);
					player.minionKB += 1.5f;
					player.moveSpeed += 0.15f;
				}
				else
					player.slowFall = true;
			}

			// Calcium Potion buff
			if (modPlayer.calcium)
				player.noFallDmg = true;

			// Ceaseless Hunger Potion buff
			if (modPlayer.ceaselessHunger)
			{
				for (int j = 0; j < Main.maxItems; j++)
				{
					Item item = Main.item[j];
					if (item.active && item.noGrabDelay == 0 && item.owner == player.whoAmI)
					{
						item.beingGrabbed = true;
						if (player.Center.X > item.Center.X)
						{
							if (item.velocity.X < 90f + player.velocity.X)
							{
								item.velocity.X += 9f;
							}
							if (item.velocity.X < 0f)
							{
								item.velocity.X += 9f * 0.75f;
							}
						}
						else
						{
							if (item.velocity.X > -90f + player.velocity.X)
							{
								item.velocity.X -= 9f;
							}
							if (item.velocity.X > 0f)
							{
								item.velocity.X -= 9f * 0.75f;
							}
						}

						if (player.Center.Y > item.Center.Y)
						{
							if (item.velocity.Y < 90f)
							{
								item.velocity.Y += 9f;
							}
							if (item.velocity.Y < 0f)
							{
								item.velocity.Y += 9f * 0.75f;
							}
						}
						else
						{
							if (item.velocity.Y > -90f)
							{
								item.velocity.Y -= 9f;
							}
							if (item.velocity.Y > 0f)
							{
								item.velocity.Y -= 9f * 0.75f;
							}
						}
					}
				}
			}

			// Spectral Veil effects
			if (modPlayer.spectralVeil && modPlayer.spectralVeilImmunity > 0)
			{
				Rectangle sVeilRectangle = new Rectangle((int)(player.position.X + player.velocity.X * 0.5f - 4f), (int)(player.position.Y + player.velocity.Y * 0.5f - 4f), player.width + 8, player.height + 8);
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					if (!npc.active || npc.friendly || npc.damage <= 0 || npc.dontTakeDamage)
						continue;
					if (!npc.townNPC && npc.immune[player.whoAmI] <= 0 && npc.damage > 0)
					{
						Rectangle rect = npc.getRect();
						if (sVeilRectangle.Intersects(rect) && (npc.noTileCollide || player.CanHit(npc)))
						{
							if (player.whoAmI == Main.myPlayer)
							{
								player.noKnockback = true;
								modPlayer.rogueStealth = modPlayer.rogueStealthMax;
								modPlayer.spectralVeilImmunity = 0;

								for (int k = 0; k < player.hurtCooldowns.Length; k++)
									player.hurtCooldowns[k] = player.immuneTime;

								Vector2 sVeilDustDir = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
								sVeilDustDir.Normalize();
								sVeilDustDir *= 0.5f;

								for (int j = 0; j < 20; j++)
								{
									int sVeilDustIndex1 = Dust.NewDust(player.Center, 1, 1, 21, sVeilDustDir.X * j, sVeilDustDir.Y * j);
									int sVeilDustIndex2 = Dust.NewDust(player.Center, 1, 1, 21, -sVeilDustDir.X * j, -sVeilDustDir.Y * j);
									Main.dust[sVeilDustIndex1].noGravity = false;
									Main.dust[sVeilDustIndex1].noLight = false;
									Main.dust[sVeilDustIndex2].noGravity = false;
									Main.dust[sVeilDustIndex2].noLight = false;
								}

								Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaDispel"), player.Center);
							}
							break;
						}
					}
				}

				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile proj = Main.projectile[i];
					if (proj.active && !proj.friendly && proj.hostile && proj.damage > 0)
					{
						Rectangle rect = proj.getRect();
						if (sVeilRectangle.Intersects(rect))
						{
							if (player.whoAmI == Main.myPlayer)
							{
								player.noKnockback = true;
								modPlayer.rogueStealth = modPlayer.rogueStealthMax;
								modPlayer.spectralVeilImmunity = 0;

								for (int k = 0; k < player.hurtCooldowns.Length; k++)
									player.hurtCooldowns[k] = player.immuneTime;

								Vector2 sVeilDustDir = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
								sVeilDustDir.Normalize();
								sVeilDustDir *= 0.5f;

								for (int j = 0; j < 20; j++)
								{
									int sVeilDustIndex1 = Dust.NewDust(player.Center, 1, 1, 21, sVeilDustDir.X * j, sVeilDustDir.Y * j);
									int sVeilDustIndex2 = Dust.NewDust(player.Center, 1, 1, 21, -sVeilDustDir.X * j, -sVeilDustDir.Y * j);
									Main.dust[sVeilDustIndex1].noGravity = false;
									Main.dust[sVeilDustIndex1].noLight = false;
									Main.dust[sVeilDustIndex2].noGravity = false;
									Main.dust[sVeilDustIndex2].noLight = false;
								}

								Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaDispel"), player.Center);
							}
							break;
						}
					}
				}
			}

			// Plagued Fuel Pack effects
			if (modPlayer.plaguedFuelPackDash > 0 && player.whoAmI == Main.myPlayer)
			{
				int velocityMult = modPlayer.plaguedFuelPackDash > 1 ? 25 : 5;
				player.velocity = new Vector2(modPlayer.plaguedFuelPackDirection, -1) * velocityMult;

				int numClouds = Main.rand.Next(2, 10);
				for (int i = 0; i < numClouds; i++)
				{
					Vector2 cloudVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
					cloudVelocity.Normalize();
					cloudVelocity *= Main.rand.NextFloat(0f, 1f);
					int projectile = Projectile.NewProjectile(player.Center, cloudVelocity, ModContent.ProjectileType<PlaguedFuelPackCloud>(), (int)(20 * player.RogueDamage()), 0, player.whoAmI, 0, 0);
					Main.projectile[projectile].timeLeft = Main.rand.Next(180, 240);
				}

				for (int i = 0; i < 3; i++)
				{
					int dust = Dust.NewDust(player.Center, 1, 1, 89, player.velocity.X * -0.1f, player.velocity.Y * -0.1f, 100, default, 3.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.2f;
					Main.dust[dust].velocity.Y -= 0.15f;
				}
			}

			// Blunder Booster effects
			if (modPlayer.blunderBoosterDash > 0 && player.whoAmI == Main.myPlayer)
			{
				int velocityMult = modPlayer.blunderBoosterDash > 1 ? 35 : 5;
				player.velocity = new Vector2(modPlayer.blunderBoosterDirection, -1) * velocityMult;

				int lightningCount = Main.rand.Next(2, 7);
				for (int i = 0; i < lightningCount; i++)
				{
					Vector2 lightningVel = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
					lightningVel.Normalize();
					lightningVel *= Main.rand.NextFloat(1f, 2f);
					int projectile = Projectile.NewProjectile(player.Center, lightningVel, ModContent.ProjectileType<BlunderBoosterLightning>(), (int)(30 * player.RogueDamage()), 0, player.whoAmI, Main.rand.Next(2), 0f);
					Main.projectile[projectile].timeLeft = Main.rand.Next(180, 240);
				}

				for (int i = 0; i < 3; i++)
				{
					int dust = Dust.NewDust(player.Center, 1, 1, 60, player.velocity.X * -0.1f, player.velocity.Y * -0.1f, 100, default, 3.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.2f;
					Main.dust[dust].velocity.Y -= 0.15f;
				}
			}

			// Gravistar Sabaton effects
			if (modPlayer.gSabaton && player.whoAmI == Main.myPlayer)
			{
				if (modPlayer.gSabatonCooldown <= 0 && !player.mount.Active)
				{
					if (player.controlDown && player.releaseDown && player.position.Y != player.oldPosition.Y)
					{
						modPlayer.gSabatonFall = 300;
						modPlayer.gSabatonCooldown = 480; //8 second cooldown
						player.gravity *= 2f;
						Projectile.NewProjectile(player.Center.X, player.Center.Y + (player.height / 5f), player.velocity.X, player.velocity.Y, ModContent.ProjectileType<SabatonSlam>(), 0, 0, player.whoAmI);
					}
				}
				if (modPlayer.gSabatonCooldown == 1) //dust when ready to use again
				{
					for (int i = 0; i < 66; i++)
					{
						int d = Dust.NewDust(player.position, player.width, player.height, Main.rand.NextBool(2) ? ModContent.DustType<AstralBlue>() : ModContent.DustType<AstralOrange>(), 0, 0, 100, default, 2.6f);
						Main.dust[d].noGravity = true;
						Main.dust[d].noLight = true;
						Main.dust[d].fadeIn = 1f;
						Main.dust[d].velocity *= 6.6f;
					}
				}
			}

			if (!modPlayer.brimflameSet && modPlayer.brimflameFrenzy)
			{
				modPlayer.brimflameFrenzy = false;
				player.ClearBuff(ModContent.BuffType<BrimflameFrenzyBuff>());
				player.AddBuff(ModContent.BuffType<BrimflameFrenzyCooldown>(), 30 * 60, true);
			}
			if (!modPlayer.bloodflareMelee && modPlayer.bloodflareFrenzy)
			{
				modPlayer.bloodflareFrenzy = false;
				player.ClearBuff(ModContent.BuffType<BloodflareBloodFrenzy>());
				player.AddBuff(ModContent.BuffType<BloodflareBloodFrenzyCooldown>(), 1800, false);
			}
			if (!modPlayer.tarraMelee && modPlayer.tarragonCloak)
			{
				modPlayer.tarragonCloak = false;
				player.ClearBuff(ModContent.BuffType<TarragonCloak>());
				player.AddBuff(ModContent.BuffType<TarragonCloakCooldown>(), 600, false);
			}
			if (!modPlayer.tarraThrowing && modPlayer.tarragonImmunity)
			{
				modPlayer.tarragonImmunity = false;
				player.ClearBuff(ModContent.BuffType<TarragonImmunity>());
				player.AddBuff(ModContent.BuffType<TarragonImmunityCooldown>(), 600, false);
			}
			if (!modPlayer.omegaBlueSet && modPlayer.omegaBlueCooldown > 1500)
			{
				modPlayer.omegaBlueCooldown = 1500;
				player.ClearBuff(ModContent.BuffType<AbyssalMadness>());
				player.AddBuff(ModContent.BuffType<AbyssalMadnessCooldown>(), 1500, false);
			}
			if (!modPlayer.plagueReaper && modPlayer.plagueReaperCooldown > 1500)
			{
				modPlayer.plagueReaperCooldown = 1500;
				player.AddBuff(ModContent.BuffType<PlagueBlackoutCooldown>(), 1500, false);
			}
			if (!modPlayer.prismaticSet && modPlayer.prismaticLasers > 1800)
			{
				modPlayer.prismaticLasers = 1800;
				player.AddBuff(ModContent.BuffType<PrismaticCooldown>(), CalamityUtils.SecondsToFrames(30f), true);
			}
		}
		#endregion

		#region Abyss Effects
		private static void AbyssEffects(Player player, CalamityPlayer modPlayer)
		{
			int lightStrength = modPlayer.GetTotalLightStrength();
			modPlayer.abyssLightLevelStat = lightStrength;

			if (modPlayer.ZoneAbyss)
			{
				if (Main.myPlayer == player.whoAmI)
				{
					// Abyss depth variables
					Point point = player.Center.ToTileCoordinates();
					double abyssSurface = Main.rockLayer - (double)Main.maxTilesY * 0.05;
					double totalAbyssDepth = Main.maxTilesY - 250D - abyssSurface;
					double playerAbyssDepth = point.Y - abyssSurface;
					double depthRatio = playerAbyssDepth / totalAbyssDepth;

					// Darkness strength scales smoothly with how deep you are.
					float darknessStrength = (float)depthRatio;

					// Reduce the power of abyss darkness based on your light level.
					float multiplier = 1f;
					switch (lightStrength)
					{
						case 0:
							break;
						case 1:
							multiplier = 0.85f;
							break;
						case 2:
							multiplier = 0.7f;
							break;
						case 3:
							multiplier = 0.55f;
							break;
						case 4:
							multiplier = 0.4f;
							break;
						case 5:
							multiplier = 0.25f;
							break;
						case 6:
							multiplier = 0.15f;
							break;
						case 7:
							multiplier = 0.1f;
							break;
						default:
							multiplier = 0.05f;
							break;
					}

					// Increased darkness in Death Mode
					if (CalamityWorld.death)
						multiplier += (1f - multiplier) * 0.1f;

					// Modify darkness variable
					modPlayer.caveDarkness = darknessStrength * multiplier;

					// Nebula Headcrab darkness effect
					if (!player.headcovered)
					{
						float screenObstructionAmt = MathHelper.Clamp(modPlayer.caveDarkness, 0f, 0.95f);
						float targetValue = MathHelper.Clamp(screenObstructionAmt * 0.7f, 0.1f, 0.3f);
						ScreenObstruction.screenObstruction = MathHelper.Lerp(ScreenObstruction.screenObstruction, screenObstructionAmt, targetValue);
					}

					// Breath lost while at zero breath
					double breathLoss = 50D * depthRatio;

					// Breath Loss Multiplier, depending on gear
					double breathLossMult = 1D -
						(player.gills ? 0.2 : 0D) - // 0.8
						(player.accDivingHelm ? 0.25 : 0D) - // 0.75
						(player.arcticDivingGear ? 0.25 : 0D) - // 0.75
						(modPlayer.aquaticEmblem ? 0.25 : 0D) - // 0.75
						(player.accMerman ? 0.3 : 0D) - // 0.7
						(modPlayer.victideSet ? 0.2 : 0D) - // 0.85
						((modPlayer.sirenBoobs && NPC.downedBoss3) ? 0.3 : 0D) - // 0.7
						(modPlayer.abyssalDivingSuit ? 0.3 : 0D); // 0.7

					// Limit the multiplier to 5%
					if (breathLossMult < 0.05)
						breathLossMult = 0.05;

					// Reduce breath lost while at zero breath, depending on gear
					breathLoss *= breathLossMult;

					// Stat Meter stat
					modPlayer.abyssBreathLossStat = (int)breathLoss;

					// Defense loss
					int defenseLoss = (int)(120D * depthRatio);

					// Anechoic Plating reduces defense loss by 66%
					// Fathom Swarmer Breastplate reduces defense loss by 40%
					// In tandem, reduces defense loss by 80%
					if (modPlayer.anechoicPlating && modPlayer.fathomSwarmerBreastplate)
						defenseLoss = (int)(defenseLoss * 0.2f);
					else if (modPlayer.anechoicPlating)
						defenseLoss /= 3;
					else if (modPlayer.fathomSwarmerBreastplate)
						defenseLoss = (int)(defenseLoss * 0.6f);

					// Reduce defense
					player.statDefense -= defenseLoss;

					// Stat Meter stat
					modPlayer.abyssDefenseLossStat = defenseLoss;

					// Bleed effect based on abyss layer
					if (modPlayer.ZoneAbyssLayer4)
					{
						player.bleed = true;
					}
					else if (modPlayer.ZoneAbyssLayer3)
					{
						if (!modPlayer.abyssalDivingSuit)
							player.bleed = true;
					}
					else if (modPlayer.ZoneAbyssLayer2)
					{
						if (!modPlayer.depthCharm)
							player.bleed = true;
					}

					// Ticks (frames) until breath is deducted from the breath meter
					double tick = 12D * (1D - depthRatio);

					// Prevent 0
					if (tick < 1D)
						tick = 1D;

					// Tick (frame) multiplier, depending on gear
					double tickMult = 1D +
						(player.gills ? 4D : 0D) + // 5
						(player.ignoreWater ? 5D : 0D) + // 10
						(player.accDivingHelm ? 10D : 0D) + // 20
						(player.arcticDivingGear ? 10D : 0D) + // 30
						(modPlayer.aquaticEmblem ? 10D : 0D) + // 40
						(player.accMerman ? 15D : 0D) + // 55
						(modPlayer.victideSet ? 5D : 0D) + // 60
						((modPlayer.sirenBoobs && NPC.downedBoss3) ? 15D : 0D) + // 75
						(modPlayer.abyssalDivingSuit ? 15D : 0D); // 90

					// Limit the multiplier to 50
					if (tickMult > 50D)
						tickMult = 50D;

					// Increase ticks (frames) until breath is deducted, depending on gear
					tick *= tickMult;

					// Stat Meter stat
					modPlayer.abyssBreathLossRateStat = (int)tick;

					// Reduce breath over ticks (frames)
					modPlayer.abyssBreathCD++;
					if (modPlayer.abyssBreathCD >= (int)tick)
					{
						// Reset modded breath variable
						modPlayer.abyssBreathCD = 0;

						// Reduce breath
						if (player.breath > 0)
							player.breath -= (int)(modPlayer.cDepth ? breathLoss + 1D : breathLoss);
					}

					// If breath is greater than 0 and player has gills or is merfolk, balance out the effects by reducing breath
					if (player.breath > 0)
					{
						if (player.gills || player.merman)
							player.breath -= 3;
					}

					// Life loss at zero breath
					int lifeLossAtZeroBreath = (int)(12D * depthRatio);

					// Resistance to life loss at zero breath
					int lifeLossAtZeroBreathResist = 0 +
						(modPlayer.depthCharm ? 3 : 0) +
						(modPlayer.abyssalDivingSuit ? 6 : 0);

					// Reduce life loss, depending on gear
					lifeLossAtZeroBreath -= lifeLossAtZeroBreathResist;

					// Prevent negatives
					if (lifeLossAtZeroBreath < 0)
						lifeLossAtZeroBreath = 0;

					// Stat Meter stat
					modPlayer.abyssLifeLostAtZeroBreathStat = lifeLossAtZeroBreath;

					// Check breath value
					if (player.breath <= 0)
					{
						// Reduce life
						player.statLife -= lifeLossAtZeroBreath;

						// Special kill code if the life loss kills the player
						if (player.statLife <= 0)
						{
							modPlayer.abyssDeath = true;
							modPlayer.KillPlayer();
						}
					}
				}
			}
			else
			{
				modPlayer.abyssBreathCD = 0;
				modPlayer.abyssDeath = false;
			}
		}
		#endregion

		#region Max Life And Mana Effects
		private static void MaxLifeAndManaEffects(Player player, CalamityPlayer modPlayer, Mod mod)
		{
			// Silva invincibility effects
			if (modPlayer.silvaHitCounter > 0)
			{
				player.statLifeMax2 -= modPlayer.silvaHitCounter * 100;
				if (player.statLifeMax2 <= 400)
				{
					player.statLifeMax2 = 400;
					if (modPlayer.silvaCountdown > 0)
					{
						if (player.FindBuffIndex(ModContent.BuffType<SilvaRevival>()) > -1)
							player.ClearBuff(ModContent.BuffType<SilvaRevival>());

						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaDispel"), (int)player.position.X, (int)player.position.Y);
					}
					modPlayer.silvaCountdown = 0;
				}
			}

			// New textures
			if (Main.netMode != NetmodeID.Server && player.whoAmI == Main.myPlayer)
			{
				Texture2D rain3 = ModContent.GetTexture("CalamityMod/ExtraTextures/Rain3");
				Texture2D rainOriginal = ModContent.GetTexture("CalamityMod/ExtraTextures/RainOriginal");
				Texture2D mana2 = ModContent.GetTexture("CalamityMod/ExtraTextures/Mana2");
				Texture2D mana3 = ModContent.GetTexture("CalamityMod/ExtraTextures/Mana3");
				Texture2D mana4 = ModContent.GetTexture("CalamityMod/ExtraTextures/Mana4");
				Texture2D manaOriginal = ModContent.GetTexture("CalamityMod/ExtraTextures/ManaOriginal");
				Texture2D carpetAuric = ModContent.GetTexture("CalamityMod/ExtraTextures/AuricCarpet");
				Texture2D carpetOriginal = ModContent.GetTexture("CalamityMod/ExtraTextures/Carpet");

				int totalManaBoost =
					(modPlayer.pHeart ? 1 : 0) +
					(modPlayer.eCore ? 1 : 0) +
					(modPlayer.cShard ? 1 : 0);
				switch (totalManaBoost)
				{
					default:
						Main.manaTexture = manaOriginal;
						break;
					case 3:
						Main.manaTexture = mana4;
						break;
					case 2:
						Main.manaTexture = mana3;
						break;
					case 1:
						Main.manaTexture = mana2;
						break;
				}

				if (Main.bloodMoon)
					Main.rainTexture = rainOriginal;
				else if (Main.raining && modPlayer.ZoneSulphur)
					Main.rainTexture = rain3;
				else
					Main.rainTexture = rainOriginal;

				if (modPlayer.auricSet)
					Main.flyingCarpetTexture = carpetAuric;
				else
					Main.flyingCarpetTexture = carpetOriginal;
			}
		}
		#endregion

		#region Standing Still Effects
		private static void StandingStillEffects(Player player, CalamityPlayer modPlayer)
		{
			// Rogue Stealth
			modPlayer.UpdateRogueStealth();

			// Trinket of Chi bonus
			if (modPlayer.trinketOfChi)
			{
				if (modPlayer.trinketOfChiBuff)
				{
					player.allDamage += 0.5f;
					if (player.itemAnimation > 0)
						modPlayer.chiBuffTimer = 0;
				}

				if (player.StandingStill(0.1f) && !player.mount.Active)
				{
					if (modPlayer.chiBuffTimer < 120)
						modPlayer.chiBuffTimer++;
					else
						player.AddBuff(ModContent.BuffType<ChiBuff>(), 6);
				}
				else
					modPlayer.chiBuffTimer--;
			}
			else
				modPlayer.chiBuffTimer = 0;

			// Aquatic Emblem bonus
			if (modPlayer.aquaticEmblem)
			{
				if (player.IsUnderwater() && player.wet && !player.lavaWet && !player.honeyWet &&
					!player.mount.Active)
				{
					if (modPlayer.aquaticBoost > 0f)
					{
						modPlayer.aquaticBoost -= 0.0002f; // 0.015
						if (modPlayer.aquaticBoost <= 0f)
						{
							modPlayer.aquaticBoost = 0f;
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
						}
					}
				}
				else
				{
					modPlayer.aquaticBoost += 0.0002f;
					if (modPlayer.aquaticBoost > 1f)
						modPlayer.aquaticBoost = 1f;
					if (player.mount.Active)
						modPlayer.aquaticBoost = 1f;
				}

				player.statDefense += (int)((1f - modPlayer.aquaticBoost) * 30f);
				player.moveSpeed -= (1f - modPlayer.aquaticBoost) * 0.1f;
			}
			else
				modPlayer.aquaticBoost = 1f;

			// Auric bonus
			if (modPlayer.auricBoost)
			{
				if (player.itemAnimation > 0)
					modPlayer.modStealthTimer = 5;

				if (player.StandingStill(0.1f) && !player.mount.Active)
				{
					if (modPlayer.modStealthTimer == 0 && modPlayer.modStealth > 0f)
					{
						modPlayer.modStealth -= 0.015f;
						if (modPlayer.modStealth <= 0f)
						{
							modPlayer.modStealth = 0f;
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
						}
					}
				}
				else
				{
					float playerVel = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
					modPlayer.modStealth += playerVel * 0.0075f;
					if (modPlayer.modStealth > 1f)
						modPlayer.modStealth = 1f;
					if (player.mount.Active)
						modPlayer.modStealth = 1f;
				}

				float damageBoost = (1f - modPlayer.modStealth) * 0.2f;
				player.allDamage += damageBoost;

				int critBoost = (int)((1f - modPlayer.modStealth) * 10f);
				modPlayer.AllCritBoost(critBoost);

				if (modPlayer.modStealthTimer > 0)
					modPlayer.modStealthTimer--;
			}

			// Psychotic Amulet bonus
			else if (modPlayer.pAmulet)
			{
				if (player.itemAnimation > 0)
					modPlayer.modStealthTimer = 5;

				if (player.StandingStill(0.1f) && !player.mount.Active)
				{
					if (modPlayer.modStealthTimer == 0 && modPlayer.modStealth > 0f)
					{
						modPlayer.modStealth -= 0.015f;
						if (modPlayer.modStealth <= 0f)
						{
							modPlayer.modStealth = 0f;
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
						}
					}
				}
				else
				{
					float playerVel = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
					modPlayer.modStealth += playerVel * 0.0075f;
					if (modPlayer.modStealth > 1f)
						modPlayer.modStealth = 1f;
					if (player.mount.Active)
						modPlayer.modStealth = 1f;
				}

				modPlayer.throwingDamage += (1f - modPlayer.modStealth) * 0.2f;
				modPlayer.throwingCrit += (int)((1f - modPlayer.modStealth) * 10f);
				player.aggro -= (int)((1f - modPlayer.modStealth) * 750f);
				if (modPlayer.modStealthTimer > 0)
					modPlayer.modStealthTimer--;
			}
			else
				modPlayer.modStealth = 1f;

			if (player.ActiveItem().type == ModContent.ItemType<Auralis>() && player.StandingStill(0.1f))
			{
				if (modPlayer.auralisStealthCounter < 300f)
					modPlayer.auralisStealthCounter++;

				bool usingScope = false;
				if (!Main.gameMenu && Main.netMode != NetmodeID.Server)
				{
					if (player.noThrow <= 0 && !player.lastMouseInterface || (Main.zoomX != 0f || Main.zoomY != 0f))
					{
						if (PlayerInput.UsingGamepad)
						{
							if (PlayerInput.GamepadThumbstickRight.Length() != 0f || !Main.SmartCursorEnabled)
							{
								usingScope = true;
							}
						}
						else if (Main.mouseRight)
							usingScope = true;
					}
				}

				int chargeDuration = CalamityUtils.SecondsToFrames(5f);
				int auroraDuration = CalamityUtils.SecondsToFrames(20f);

				if (usingScope && modPlayer.auralisAuroraCounter < chargeDuration + auroraDuration)
					modPlayer.auralisAuroraCounter++;

				if (modPlayer.auralisAuroraCounter > chargeDuration + auroraDuration)
				{
					modPlayer.auralisAuroraCounter = 0;
					modPlayer.auralisAuroraCooldown = CalamityUtils.SecondsToFrames(30f);
				}

				if (modPlayer.auralisAuroraCounter > 0 && modPlayer.auralisAuroraCounter < chargeDuration && !usingScope)
					modPlayer.auralisAuroraCounter--;

				if (modPlayer.auralisAuroraCounter > chargeDuration && modPlayer.auralisAuroraCounter < chargeDuration + auroraDuration && !usingScope)
					modPlayer.auralisAuroraCounter = 0;
			}
			else
			{
				modPlayer.auralisStealthCounter = 0f;
				modPlayer.auralisAuroraCounter = 0;
			}
			if (modPlayer.auralisAuroraCooldown > 0)
			{
				if (modPlayer.auralisAuroraCooldown == 1)
				{
					int dustAmt = 36;
					for (int d = 0; d < dustAmt; d++)
					{
						Vector2 source = Vector2.Normalize(player.velocity) * new Vector2((float)player.width / 2f, (float)player.height) * 1f; //0.75
						source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + player.Center;
						Vector2 dustVel = source - player.Center;
						int blue = Dust.NewDust(source + dustVel, 0, 0, 229, dustVel.X, dustVel.Y, 100, default, 1.2f);
						Main.dust[blue].noGravity = true;
						Main.dust[blue].noLight = false;
						Main.dust[blue].velocity = dustVel;
					}
					for (int d = 0; d < dustAmt; d++)
					{
						Vector2 source = Vector2.Normalize(player.velocity) * new Vector2((float)player.width / 2f, (float)player.height) * 0.75f;
						source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + player.Center;
						Vector2 dustVel = source - player.Center;
						int green = Dust.NewDust(source + dustVel, 0, 0, 107, dustVel.X, dustVel.Y, 100, default, 1.2f);
						Main.dust[green].noGravity = true;
						Main.dust[green].noLight = false;
						Main.dust[green].velocity = dustVel;
					}
				}
				modPlayer.auralisAuroraCounter = 0;
			}
		}
		#endregion

		#region Elysian Aegis Effects
		private static void ElysianAegisEffects(Player player, CalamityPlayer modPlayer)
		{
			if (modPlayer.elysianAegis)
			{
				bool spawnDust = false;

				// Activate buff
				if (modPlayer.elysianGuard)
				{
					if (player.whoAmI == Main.myPlayer)
						player.AddBuff(ModContent.BuffType<ElysianGuard>(), 2, false);

					float shieldBoostInitial = modPlayer.shieldInvinc;
					modPlayer.shieldInvinc -= 0.08f;
					if (modPlayer.shieldInvinc < 0f)
						modPlayer.shieldInvinc = 0f;
					else
						spawnDust = true;

					if (modPlayer.shieldInvinc == 0f && shieldBoostInitial != modPlayer.shieldInvinc && Main.netMode == NetmodeID.MultiplayerClient)
						NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);

					float damageBoost = (5f - modPlayer.shieldInvinc) * 0.03f;
					player.allDamage += damageBoost;

					int critBoost = (int)((5f - modPlayer.shieldInvinc) * 2f);
					modPlayer.AllCritBoost(critBoost);

					player.aggro += (int)((5f - modPlayer.shieldInvinc) * 220f);
					player.statDefense += (int)((5f - modPlayer.shieldInvinc) * 4f);
					player.moveSpeed *= 0.85f;

					if (player.mount.Active)
						modPlayer.elysianGuard = false;
				}

				// Remove buff
				else
				{
					float shieldBoostInitial = modPlayer.shieldInvinc;
					modPlayer.shieldInvinc += 0.08f;
					if (modPlayer.shieldInvinc > 5f)
						modPlayer.shieldInvinc = 5f;
					else
						spawnDust = true;

					if (modPlayer.shieldInvinc == 5f && shieldBoostInitial != modPlayer.shieldInvinc && Main.netMode == NetmodeID.MultiplayerClient)
						NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
				}

				// Emit dust
				if (spawnDust)
				{
					if (Main.rand.NextBool(2))
					{
						Vector2 vector = Vector2.UnitY.RotatedByRandom(Math.PI * 2D);
						Dust dust = Main.dust[Dust.NewDust(player.Center - vector * 30f, 0, 0, (int)CalamityDusts.ProfanedFire, 0f, 0f, 0, default, 1f)];
						dust.noGravity = true;
						dust.position = player.Center - vector * (float)Main.rand.Next(5, 11);
						dust.velocity = vector.RotatedBy(Math.PI / 2D, default) * 4f;
						dust.scale = 0.5f + Main.rand.NextFloat();
						dust.fadeIn = 0.5f;
					}

					if (Main.rand.NextBool(2))
					{
						Vector2 vector2 = Vector2.UnitY.RotatedByRandom(Math.PI * 2D);
						Dust dust2 = Main.dust[Dust.NewDust(player.Center - vector2 * 30f, 0, 0, 246, 0f, 0f, 0, default, 1f)];
						dust2.noGravity = true;
						dust2.position = player.Center - vector2 * 12f;
						dust2.velocity = vector2.RotatedBy(-Math.PI / 2D, default) * 2f;
						dust2.scale = 0.5f + Main.rand.NextFloat();
						dust2.fadeIn = 0.5f;
					}
				}
			}
			else
				modPlayer.elysianGuard = false;
		}
		#endregion

		#region Other Buff Effects
		private static void OtherBuffEffects(Player player, CalamityPlayer modPlayer)
		{
			if (modPlayer.gravityNormalizer)
			{
				player.buffImmune[BuffID.VortexDebuff] = true;
				if (player.InSpace())
				{
					player.gravity = Player.defaultGravity;
					if (player.wet)
					{
						if (player.honeyWet)
							player.gravity = 0.1f;
						else if (player.merman)
							player.gravity = 0.3f;
						else
							player.gravity = 0.2f;
					}
				}
			}

			// Effigy of Decay effects
			if (modPlayer.decayEffigy)
			{
				player.buffImmune[ModContent.BuffType<SulphuricPoisoning>()] = true;
				if (!modPlayer.ZoneAbyss)
				{
					player.gills = true;
				}
			}

			if (modPlayer.astralInjection)
			{
				if (player.statMana < player.statManaMax2)
					player.statMana += 3;
				if (player.statMana > player.statManaMax2)
					player.statMana = player.statManaMax2;
			}

			if (modPlayer.armorCrumbling)
			{
				modPlayer.throwingCrit += 5;
				player.meleeCrit += 5;
			}

			if (modPlayer.armorShattering)
			{
				if (player.FindBuffIndex(ModContent.BuffType<ArmorCrumbling>()) > -1)
					player.ClearBuff(ModContent.BuffType<ArmorCrumbling>());
				modPlayer.throwingDamage += 0.08f;
				player.meleeDamage += 0.08f;
				modPlayer.throwingCrit += 8;
				player.meleeCrit += 8;
			}

			if (modPlayer.holyWrath)
			{
				if (player.FindBuffIndex(BuffID.Wrath) > -1)
					player.ClearBuff(BuffID.Wrath);
				player.allDamage += 0.12f;
				player.moveSpeed += 0.05f;
			}

			if (modPlayer.profanedRage)
			{
				if (player.FindBuffIndex(BuffID.Rage) > -1)
					player.ClearBuff(BuffID.Rage);
				modPlayer.AllCritBoost(12);
				player.moveSpeed += 0.05f;
			}

			if (modPlayer.shadow)
			{
				if (player.FindBuffIndex(BuffID.Invisibility) > -1)
					player.ClearBuff(BuffID.Invisibility);
			}

			if (modPlayer.irradiated)
			{
				if (modPlayer.boomerDukeLore)
				{
					player.statDefense += 10;
				}
				else
				{
					player.statDefense -= 10;
				}
				player.allDamage += 0.05f;
				player.minionKB += 0.5f;
				player.moveSpeed += 0.05f;
			}

			if (modPlayer.rRage)
			{
				player.allDamage += 0.05f;
				player.moveSpeed += 0.05f;
			}

			if (modPlayer.xRage)
				modPlayer.throwingDamage += 0.1f;

			if (modPlayer.xWrath)
				modPlayer.throwingCrit += 5;

			if (modPlayer.graxDefense)
			{
				player.statDefense += 30;
				player.endurance += 0.1f;
				player.meleeDamage += 0.2f;
			}

			if (modPlayer.eScarfBoost)
			{
				player.allDamage += 0.1f;
				modPlayer.AllCritBoost(10);
			}
			if (modPlayer.sMeleeBoost)
			{
				player.allDamage += 0.1f;
				modPlayer.AllCritBoost(5);
			}

			if (modPlayer.tFury)
			{
				player.meleeDamage += 0.3f;
				player.meleeCrit += 10;
			}

			if (modPlayer.yPower)
			{
				player.endurance += 0.05f;
				player.statDefense += 5;
				player.allDamage += 0.06f;
				modPlayer.AllCritBoost(2);
				player.minionKB += 1f;
				player.moveSpeed += 0.15f;
			}

			if (modPlayer.tScale)
			{
				player.endurance += 0.05f;
				player.statDefense += 5;
				player.kbBuff = true;
				if (modPlayer.titanBoost > 0)
				{
                    player.statDefense += 25;
                    player.endurance += 0.1f;
				}
			}
			else
				modPlayer.titanBoost = 0;

			if (modPlayer.darkSunRing)
			{
				player.maxMinions += 2;
				player.allDamage += 0.12f;
				player.minionKB += 1.2f;
				player.pickSpeed -= 0.15f;
				if (Main.eclipse || !Main.dayTime)
					player.statDefense += 30;
			}

			if (modPlayer.eGauntlet)
			{
				player.longInvince = true;
				player.kbGlove = true;
				player.magmaStone = true;
				player.meleeDamage += 0.15f;
				player.meleeCrit += 5;
				player.lavaMax += 240;
			}

			if (modPlayer.eQuiver)
			{
				player.magicQuiver = true;
			}

			if (modPlayer.bloodPactBoost)
			{
				player.allDamage += 0.05f;
				player.statDefense += 20;
				player.endurance += 0.1f;
				player.longInvince = true;
				player.crimsonRegen = true;
			}

			if (modPlayer.fabsolVodka)
			{
				player.allDamage += 0.08f;
				player.statDefense -= 20;
			}

			if (modPlayer.vodka)
			{
				player.statDefense -= 4;
				player.allDamage += 0.06f;
				modPlayer.AllCritBoost(2);
			}

			if (modPlayer.grapeBeer)
			{
				player.statDefense -= 2;
				player.moveSpeed -= 0.05f;
			}

			if (modPlayer.moonshine)
			{
				player.statDefense += 10;
				player.endurance += 0.05f;
			}

			if (modPlayer.rum)
			{
				player.moveSpeed += 0.1f;
				player.statDefense -= 8;
			}

			if (modPlayer.whiskey)
			{
				player.statDefense -= 8;
				player.allDamage += 0.04f;
				modPlayer.AllCritBoost(2);
			}

			if (modPlayer.everclear)
			{
				player.statDefense -= 40;
				player.allDamage += 0.25f;
			}

			if (modPlayer.bloodyMary)
			{
				if (Main.bloodMoon)
				{
					player.statDefense -= 6;
					player.allDamage += 0.15f;
					modPlayer.AllCritBoost(7);
					player.moveSpeed += 0.15f;
				}
			}

			if (modPlayer.tequila)
			{
				if (Main.dayTime)
				{
					player.statDefense += 5;
					player.allDamage += 0.03f;
					modPlayer.AllCritBoost(2);
					player.endurance += 0.03f;
				}
			}

			if (modPlayer.tequilaSunrise)
			{
				if (Main.dayTime)
				{
					player.statDefense += 15;
					player.allDamage += 0.07f;
					modPlayer.AllCritBoost(3);
					player.endurance += 0.07f;
				}
			}

			if (modPlayer.caribbeanRum)
			{
				player.moveSpeed += 0.2f;
				player.statDefense -= 12;
			}

			if (modPlayer.cinnamonRoll)
			{
				player.statDefense -= 12;
				player.manaRegenDelay--;
				player.manaRegenBonus += 10;
			}

			if (modPlayer.margarita)
				player.statDefense -= 6;

			if (modPlayer.starBeamRye)
			{
				player.statDefense -= 6;
				player.magicDamage += 0.08f;
				player.manaCost *= 0.9f;
			}

			if (modPlayer.moscowMule)
			{
				player.allDamage += 0.09f;
				modPlayer.AllCritBoost(3);
			}

			if (modPlayer.whiteWine)
			{
				player.statDefense -= 6;
				player.magicDamage += 0.1f;
			}

			if (modPlayer.evergreenGin)
				player.endurance += 0.05f;

			if (modPlayer.giantPearl)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient && !CalamityPlayer.areThereAnyDamnBosses)
				{
					for (int m = 0; m < Main.maxNPCs; m++)
					{
						NPC npc = Main.npc[m];
						if (!npc.active || npc.friendly || npc.dontTakeDamage)
							continue;
						float distance = (npc.Center - player.Center).Length();
						if (distance < 120f)
							npc.AddBuff(ModContent.BuffType<PearlAura>(), 20, false);
					}
				}
			}

			if (CalamityLists.scopedWeaponList.Contains(player.ActiveItem().type))
				player.scope = true;

			if (CalamityLists.highTestFishList.Contains(player.ActiveItem().type))
				player.accFishingLine = true;

			if (CalamityLists.boomerangList.Contains(player.ActiveItem().type) && player.invis)
				modPlayer.throwingDamage += 0.1f;

			if (CalamityLists.javelinList.Contains(player.ActiveItem().type) && player.invis)
				player.armorPenetration += 5;

			if (CalamityLists.flaskBombList.Contains(player.ActiveItem().type) && player.invis)
				modPlayer.throwingVelocity += 0.1f;

			if (CalamityLists.spikyBallList.Contains(player.ActiveItem().type) && player.invis)
				modPlayer.throwingCrit += 10;

			if (modPlayer.planarSpeedBoost != 0)
			{
				if (player.ActiveItem().type != ModContent.ItemType<PrideHuntersPlanarRipper>())
					modPlayer.planarSpeedBoost = 0;
			}

			if (modPlayer.brimlashBusterBoost)
			{
				if (player.ActiveItem().type != ModContent.ItemType<BrimlashBuster>() && player.ActiveItem().type != ModContent.ItemType<EvilSmasher>())
					modPlayer.brimlashBusterBoost = false;
			}
			if (modPlayer.animusBoost > 1f)
			{
				if (player.ActiveItem().type != ModContent.ItemType<Animus>())
					modPlayer.animusBoost = 1f;
			}

			if (modPlayer.etherealExtorter)
			{
				bool ZoneForest = !modPlayer.ZoneAbyss && !modPlayer.ZoneSulphur && !modPlayer.ZoneAstral && !modPlayer.ZoneCalamity &&
					!modPlayer.ZoneSunkenSea && !player.ZoneSnow && !player.ZoneCorrupt && !player.ZoneCrimson && !player.ZoneHoly &&
					!player.ZoneDesert && !player.ZoneUndergroundDesert && !player.ZoneGlowshroom && !player.ZoneDungeon && !player.ZoneBeach && !player.ZoneMeteor;

				if (player.ZoneUnderworldHeight && !modPlayer.ZoneCalamity && CalamityLists.fireWeaponList.Contains(player.ActiveItem().type))
					player.endurance += 0.03f;

				if ((player.ZoneDesert || player.ZoneUndergroundDesert) && CalamityLists.daggerList.Contains(player.ActiveItem().type))
					player.scope = true;

				if (modPlayer.ZoneSunkenSea)
				{
					player.gills = true;
					player.ignoreWater = true;
				}

				if (player.ZoneSnow && CalamityLists.iceWeaponList.Contains(player.ActiveItem().type))
					player.statDefense += 5;

				if (modPlayer.ZoneAstral)
				{
					if (player.wingTimeMax > 0)
						player.wingTimeMax = (int)(player.wingTimeMax * 1.05);
				}

				if (player.ZoneJungle && CalamityLists.natureWeaponList.Contains(player.ActiveItem().type))
					player.AddBuff(BuffID.DryadsWard, 5, true); // Dryad's Blessing

				if (modPlayer.ZoneAbyss)
				{
					player.blind = true;
					player.headcovered = true;
					player.blackout = true;
					if (player.FindBuffIndex(BuffID.Shine) > -1)
						player.ClearBuff(BuffID.Shine);
					if (player.FindBuffIndex(BuffID.NightOwl) > -1)
						player.ClearBuff(BuffID.NightOwl);
					player.nightVision = false;
					modPlayer.shine = false;
					player.allDamage += 0.2f;
					modPlayer.AllCritBoost(5);
					player.statDefense += 8;
					player.endurance += 0.05f;
				}

				if (player.ZoneRockLayerHeight && ZoneForest && CalamityLists.flaskBombList.Contains(player.ActiveItem().type))
					player.blackBelt = true;

				if (player.ZoneHoly)
				{
					player.maxMinions += 1;
					player.manaCost *= 0.9f;
					player.ammoCost75 = true; // 25% chance to not use ranged ammo
					modPlayer.throwingAmmoCost *= 0.75f; // 25% chance to not consume rogue consumables
				}

				if (player.ZoneBeach)
					player.moveSpeed += 0.05f;

				if (player.ZoneGlowshroom)
					player.statDefense += 3;

				if (player.ZoneMeteor)
				{
					modPlayer.gravityNormalizer = true;
					player.slowFall = true;
				}

				if (Main.moonPhase == 0) // Full moon
					player.fishingSkill += 30;

				if (Main.moonPhase == 6) // First quarter
					player.discount = true;
			}

			if (modPlayer.harpyRing)
			{
				if (player.wingTimeMax > 0)
					player.wingTimeMax = (int)(player.wingTimeMax * 1.25);
				player.moveSpeed += 0.2f;
			}

			if (modPlayer.blueCandle)
			{
				if (player.wingTimeMax > 0)
					player.wingTimeMax = (int)(player.wingTimeMax * 1.1);
				player.moveSpeed += 0.15f;
			}

			if (modPlayer.plaguebringerGoliathLore)
			{
				if (player.wingTimeMax > 0)
					player.wingTimeMax = (int)(player.wingTimeMax * 1.25);
			}

			if (modPlayer.soaring)
			{
				if (player.wingTimeMax > 0)
					player.wingTimeMax = (int)(player.wingTimeMax * 1.1);
			}

			if (modPlayer.prismaticGreaves)
			{
				if (player.wingTimeMax > 0)
					player.wingTimeMax = (int)(player.wingTimeMax * 1.1);
			}

			if (modPlayer.plagueReaper)
			{
				if (player.wingTimeMax > 0)
					player.wingTimeMax = (int)(player.wingTimeMax * 1.05);
			}

			if (modPlayer.draconicSurge)
			{
				if (player.wingTimeMax > 0)
					player.wingTimeMax = (int)(player.wingTimeMax * 1.35);
			}
			if (modPlayer.draconicSurgeCooldown) //weird mod conflicts with like Luiafk
			{
				modPlayer.draconicSurge = false;
				if (player.FindBuffIndex(ModContent.BuffType<DraconicSurgeBuff>()) > -1)
					player.ClearBuff(ModContent.BuffType<DraconicSurgeBuff>());
			}

			if (modPlayer.bounding)
			{
				player.jumpSpeedBoost += 0.5f;
				Player.jumpHeight += 10;
				player.extraFall += 25;
			}

			if (modPlayer.mushy)
				player.statDefense += 5;

			if (modPlayer.omniscience)
			{
				player.detectCreature = true;
				player.dangerSense = true;
				player.findTreasure = true;
			}

			if (modPlayer.aWeapon)
				player.moveSpeed += 0.15f;

			if (modPlayer.molten)
				player.resistCold = true;

			if (modPlayer.shellBoost)
				player.moveSpeed += 0.9f;

			if (modPlayer.tarraSet)
			{
				player.calmed = modPlayer.tarraMelee ? false : true;
				player.lifeMagnet = true;
			}

			if (modPlayer.aChicken)
			{
				player.statDefense += 5;
				player.moveSpeed += 0.1f;
			}

			if (modPlayer.cadence)
			{
				if (player.FindBuffIndex(BuffID.Regeneration) > -1)
					player.ClearBuff(BuffID.Regeneration);
				if (player.FindBuffIndex(BuffID.Lifeforce) > -1)
					player.ClearBuff(BuffID.Lifeforce);
				player.lifeMagnet = true;
				player.calmed = true;
			}

			if (modPlayer.community)
			{
				float floatTypeBoost = 0.01f +
					(NPC.downedSlimeKing ? 0.01f : 0f) +
					(NPC.downedBoss1 ? 0.01f : 0f) +
					(NPC.downedBoss2 ? 0.01f : 0f) +
					(NPC.downedQueenBee ? 0.01f : 0f) + //0.05
					(NPC.downedBoss3 ? 0.01f : 0f) +
					(Main.hardMode ? 0.01f : 0f) +
					(NPC.downedMechBossAny ? 0.01f : 0f) +
					(NPC.downedPlantBoss ? 0.01f : 0f) +
					(NPC.downedGolemBoss ? 0.01f : 0f) + //0.1
					(NPC.downedFishron ? 0.01f : 0f) +
					(NPC.downedAncientCultist ? 0.01f : 0f) +
					(NPC.downedMoonlord ? 0.01f : 0f) +
					(CalamityWorld.downedProvidence ? 0.02f : 0f) + //0.15
					(CalamityWorld.downedDoG ? 0.02f : 0f) + //0.17
					(CalamityWorld.downedYharon ? 0.03f : 0f); //0.2
				int integerTypeBoost = (int)(floatTypeBoost * 50f);
				int critBoost = integerTypeBoost / 2;
				float damageBoost = floatTypeBoost * 0.5f;
				player.endurance += floatTypeBoost * 0.25f;
				player.statDefense += integerTypeBoost;
				player.allDamage += damageBoost;
				modPlayer.AllCritBoost(critBoost);
				player.minionKB += floatTypeBoost;
				player.moveSpeed += floatTypeBoost;
				if (player.wingTimeMax > 0)
					player.wingTimeMax = (int)(player.wingTimeMax * 1.15);
			}

			if (modPlayer.ravagerLore)
			{
				if (player.wingTimeMax > 0)
					player.wingTimeMax = (int)(player.wingTimeMax * 0.5);
				player.allDamage += 0.1f;
			}

			if (modPlayer.wDeath)
			{
				player.statDefense -= WhisperingDeath.DefenseReduction;
				player.allDamage -= 0.1f;
			}

			if (modPlayer.aFlames)
				player.statDefense -= AbyssalFlames.DefenseReduction;

			if (modPlayer.gsInferno)
			{
				player.blackout = true;
				player.statDefense -= GodSlayerInferno.DefenseReduction;
			}

			if (modPlayer.astralInfection)
				player.statDefense -= AstralInfectionDebuff.DefenseReduction;

			if (modPlayer.pFlames)
			{
				player.blind = true;
				player.statDefense -= Plague.DefenseReduction;
				player.moveSpeed -= 0.15f;
			}

			if (modPlayer.bBlood)
			{
				player.blind = true;
				player.statDefense -= 3;
				player.moveSpeed += 0.2f;
				player.meleeDamage += 0.05f;
				player.rangedDamage -= 0.1f;
				player.magicDamage -= 0.1f;
			}

			if (modPlayer.horror && !modPlayer.laudanum)
			{
				player.blind = true;
				player.statDefense -= 15;
				player.moveSpeed -= 0.15f;
			}

			if (modPlayer.aCrunch)
			{
				player.statDefense -= ArmorCrunch.DefenseReduction;
				player.endurance *= 0.33f;
			}

			if (modPlayer.wCleave)
			{
				player.statDefense -= WarCleave.DefenseReduction;
				player.endurance *= 0.75f;
			}

			if (modPlayer.vHex)
			{
				player.blind = true;
				player.statDefense -= 30;
				player.moveSpeed -= 0.1f;

				if (player.wingTimeMax <= 0)
					player.wingTimeMax = 0;

				player.wingTimeMax /= 2;
			}

			if (modPlayer.gState)
			{
				player.statDefense -= GlacialState.DefenseReduction;
				player.velocity.Y = 0f;
				player.velocity.X = 0f;
			}

			if (modPlayer.eFreeze || modPlayer.silvaStun || modPlayer.eutrophication)
			{
				player.velocity.Y = 0f;
				player.velocity.X = 0f;
			}

			if (modPlayer.vaporfied || modPlayer.teslaFreeze)
			{
				player.velocity.Y *= 0.98f;
				player.velocity.X *= 0.98f;
			}

			if (modPlayer.eGravity)
			{
				if (player.wingTimeMax < 0)
					player.wingTimeMax = 0;

				if (player.wingTimeMax > 400)
					player.wingTimeMax = 400;

				player.wingTimeMax /= 4;
			}

			if (modPlayer.eGrav)
			{
				if (player.wingTimeMax < 0)
					player.wingTimeMax = 0;

				if (player.wingTimeMax > 400)
					player.wingTimeMax = 400;

				player.wingTimeMax /= 2;
			}

			if (modPlayer.molluskSet)
				player.velocity.X *= 0.985f;

			if ((modPlayer.warped || modPlayer.caribbeanRum) && !player.slowFall && !player.mount.Active)
				player.velocity.Y *= 1.01f;

			if (modPlayer.corrEffigy)
			{
				player.moveSpeed += 0.15f;
				modPlayer.AllCritBoost(10);
			}

			if (modPlayer.crimEffigy)
			{
				player.allDamage += 0.15f;
				player.statDefense += 10;
			}

			if (modPlayer.badgeOfBraveryRare)
				player.meleeDamage += 0.2f;

			if (modPlayer.calamitasLore)
				player.maxMinions += 2;

			// The player's true max life value with Calamity adjustments
			modPlayer.actualMaxLife = player.statLifeMax2;

			if (modPlayer.thirdSageH && !player.dead && modPlayer.healToFull)
				player.statLife = player.statLifeMax2;

			if (modPlayer.pinkCandle && !CalamityWorld.ironHeart)
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

			if (modPlayer.manaOverloader)
			{
				player.magicDamage += 0.06f;
			}

			if (modPlayer.twinsLore)
			{
				if (!Main.dayTime)
				{
					player.invis = true;
					modPlayer.throwingCrit += 5;
					modPlayer.throwingDamage += 0.05f;
				}

				if (player.statLife >= (int)(player.statLifeMax2 * 0.5))
					player.statDefense -= 10;
			}

			if (modPlayer.rBrain)
			{
				if (player.statLife <= (int)(player.statLifeMax2 * 0.75))
					player.allDamage += 0.1f;
				if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
					player.moveSpeed -= 0.05f;
			}

			if (modPlayer.bloodyWormTooth)
			{
				if (player.statLife < (int)(player.statLifeMax2 * 0.5))
				{
					player.meleeDamage += 0.1f;
					player.endurance += 0.1f;
				}
				else
				{
					player.meleeDamage += 0.05f;
					player.endurance += 0.05f;
				}
			}

			if (modPlayer.dAmulet)
			{
				player.panic = true;
				player.pStone = true;
				player.armorPenetration += modPlayer.rampartOfDeities ? 20 : 10;
			}

			if (modPlayer.fBulwark)
			{
				player.noKnockback = true;
				if (player.statLife > (int)(player.statLifeMax2 * 0.25))
				{
					player.hasPaladinShield = true;
					if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
					{
						if (Main.LocalPlayer.team == player.team && player.team != 0)
						{
							Vector2 otherPlayerPos = player.position - Main.LocalPlayer.position;

							if (otherPlayerPos.Length() < 800f)
								Main.LocalPlayer.AddBuff(BuffID.PaladinsShield, 20, true);
						}
					}
				}

				if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
					player.AddBuff(BuffID.IceBarrier, 5, true);
				if (player.statLife <= (int)(player.statLifeMax2 * 0.15))
					player.endurance += 0.05f;
			}

			if (modPlayer.frostFlare)
			{
				player.resistCold = true;
				player.buffImmune[BuffID.Frostburn] = true;
				player.buffImmune[BuffID.Chilled] = true;
				player.buffImmune[BuffID.Frozen] = true;

				if (player.statLife > (int)(player.statLifeMax2 * 0.75))
					player.allDamage += 0.1f;
				if (player.statLife < (int)(player.statLifeMax2 * 0.25))
					player.statDefense += 10;
			}

			if (modPlayer.vexation)
			{
				if (player.statLife < (int)(player.statLifeMax2 * 0.5))
					player.allDamage += 0.2f;
			}

			if (modPlayer.ataxiaBlaze)
			{
				if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
					player.AddBuff(BuffID.Inferno, 2);
			}

			if (modPlayer.bloodflareThrowing)
			{
				if (player.statLife > (int)(player.statLifeMax2 * 0.8))
				{
					modPlayer.throwingCrit += 5;
					player.statDefense += 30;
				}
				else
					modPlayer.throwingDamage += 0.1f;
			}

			if (modPlayer.bloodflareSummon)
			{
				if (player.statLife >= (int)(player.statLifeMax2 * 0.9))
					player.minionDamage += 0.1f;
				else if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
					player.statDefense += 20;

				if (modPlayer.bloodflareSummonTimer > 0)
					modPlayer.bloodflareSummonTimer--;

				if (player.whoAmI == Main.myPlayer && modPlayer.bloodflareSummonTimer <= 0)
				{
					modPlayer.bloodflareSummonTimer = 900;
					for (int I = 0; I < 3; I++)
					{
						float ai1 = I * 120;
						Projectile.NewProjectile(player.Center.X + (float)(Math.Sin(I * 120) * 550), player.Center.Y + (float)(Math.Cos(I * 120) * 550), 0f, 0f,
							ModContent.ProjectileType<GhostlyMine>(), (int)((modPlayer.auricSet ? 15000 : 5000) * player.MinionDamage()), 1f, player.whoAmI, ai1, 0f);
					}
				}
			}

			if (modPlayer.yInsignia)
			{
				player.longInvince = true;
				player.meleeDamage += 0.05f;
				player.lavaMax += 240;
				if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
					player.allDamage += 0.1f;
			}

			if (modPlayer.reaperToothNecklace)
			{
				player.allDamage += 0.25f;
				if (player.statDefense > 0)
					player.statDefense /= 2;
			}

			if (modPlayer.deepDiver)
			{
				player.allDamage += 0.15f;
				player.statDefense += (int)(player.statDefense * 0.15);
				player.moveSpeed += 0.15f;
			}

			if (modPlayer.coreOfTheBloodGod)
			{
				player.endurance += 0.05f;
				player.allDamage += 0.07f;
				if (player.statDefense < 100)
					player.allDamage += 0.15f;
			}
			else if (modPlayer.bloodflareCore)
			{
				if (player.statDefense < 100)
					player.allDamage += 0.15f;

				if (player.statLife <= (int)(player.statLifeMax2 * 0.15))
				{
					player.endurance += 0.1f;
					player.allDamage += 0.2f;
				}
				else if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
				{
					player.endurance += 0.05f;
					player.allDamage += 0.1f;
				}
			}

			if (modPlayer.godSlayerThrowing)
			{
				if (player.statLife >= player.statLifeMax2)
				{
					modPlayer.throwingCrit += 10;
					modPlayer.throwingDamage += 0.1f;
					modPlayer.throwingVelocity += 0.1f;
				}
			}

			#region Damage Auras
			// Tarragon Summon set bonus life aura
			if (modPlayer.tarraSummon)
			{
				const int FramesPerHit = 80;

				// Constantly increment the timer every frame.
				modPlayer.tarraLifeAuraTimer = (modPlayer.tarraLifeAuraTimer + 1) % FramesPerHit;

				// If the timer rolls over, it's time to deal damage. Only run this code for the client which is wearing the armor.
				if (modPlayer.tarraLifeAuraTimer == 0 && player.whoAmI == Main.myPlayer)
				{
					const int BaseDamage = 150;
					int damage = (int)(BaseDamage * player.MinionDamage());
					float range = 300f;

					for (int i = 0; i < Main.maxNPCs; ++i)
					{
						NPC npc = Main.npc[i];
						if (!npc.active || npc.friendly || npc.damage <= 0 || npc.dontTakeDamage)
							continue;

						if (Vector2.Distance(player.Center, npc.Center) <= range)
							Projectile.NewProjectileDirect(npc.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), damage, 0f, player.whoAmI, i);
					}
				}
			}

			// Navy Fishing Rod's electric aura when in-use
			if (player.ActiveItem().type == ModContent.ItemType<NavyFishingRod>() && player.ownedProjectileCounts[ModContent.ProjectileType<NavyBobber>()] != 0)
			{
				const int FramesPerHit = 120;

				// Constantly increment the timer every frame.
				modPlayer.navyRodAuraTimer = (modPlayer.navyRodAuraTimer + 1) % FramesPerHit;

				// If the timer rolls over, it's time to deal damage. Only run this code for the client which is holding the fishing rod,
				if (modPlayer.navyRodAuraTimer == 0 && player.whoAmI == Main.myPlayer)
				{
					const int BaseDamage = 10;
					int damage = (int)(BaseDamage * player.AverageDamage());
					float range = 200f;

					for (int i = 0; i < Main.maxNPCs; ++i)
					{
						NPC npc = Main.npc[i];
						if (!npc.active || npc.friendly || npc.damage <= 0 || npc.dontTakeDamage)
							continue;

						if (Vector2.Distance(player.Center, npc.Center) <= range)
							Projectile.NewProjectileDirect(npc.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), damage, 0f, player.whoAmI, i);

						// Occasionally spawn cute sparks so it looks like an electrical aura
						if (Main.rand.NextBool(10))
						{
							Vector2 velocity = CalamityUtils.RandomVelocity(50f, 30f, 60f);
							int spark = Projectile.NewProjectile(npc.Center, velocity, ModContent.ProjectileType<EutrophicSpark>(), damage / 2, 0f, player.whoAmI, 0f, 0f);
							Main.projectile[spark].Calamity().forceTypeless = true;
							Main.projectile[spark].localNPCHitCooldown = -2;
							Main.projectile[spark].penetrate = 5;
						}
					}
				}
			}

			// Brimstone Elemental lore inferno potion boost
			if ((modPlayer.brimstoneElementalLore || modPlayer.ataxiaBlaze) && player.inferno)
			{
				const int FramesPerHit = 30;

				// Constantly increment the timer every frame.
				modPlayer.brimLoreInfernoTimer = (modPlayer.brimLoreInfernoTimer + 1) % FramesPerHit;

				// Only run this code for the client which is wearing the armor.
				// Brimstone flames is applied every single frame, but direct damage is only dealt twice per second.
				if (player.whoAmI == Main.myPlayer)
				{
					const int BaseDamage = 50;
					int damage = (int)(BaseDamage * player.AverageDamage());
					float range = 300f;

					for (int i = 0; i < Main.maxNPCs; ++i)
					{
						NPC npc = Main.npc[i];
						if (!npc.active || npc.friendly || npc.damage <= 0 || npc.dontTakeDamage)
							continue;

						if (Vector2.Distance(player.Center, npc.Center) <= range)
						{
							npc.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
							if (modPlayer.brimLoreInfernoTimer == 0)
								Projectile.NewProjectileDirect(npc.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), damage, 0f, player.whoAmI, i);
						}
					}
				}
			}
			#endregion

			if (modPlayer.royalGel)
			{
				player.npcTypeNoAggro[ModContent.NPCType<AeroSlime>()] = true;
				player.npcTypeNoAggro[ModContent.NPCType<BloomSlime>()] = true;
				player.npcTypeNoAggro[ModContent.NPCType<CharredSlime>()] = true;
				player.npcTypeNoAggro[ModContent.NPCType<CrimulanBlightSlime>()] = true;
				player.npcTypeNoAggro[ModContent.NPCType<CryoSlime>()] = true;
				player.npcTypeNoAggro[ModContent.NPCType<EbonianBlightSlime>()] = true;
				player.npcTypeNoAggro[ModContent.NPCType<IrradiatedSlime>()] = true;
				player.npcTypeNoAggro[ModContent.NPCType<PerennialSlime>()] = true;
				player.npcTypeNoAggro[ModContent.NPCType<PlaguedJungleSlime>()] = true;
				player.npcTypeNoAggro[ModContent.NPCType<AstralSlime>()] = true;
				player.npcTypeNoAggro[ModContent.NPCType<GammaSlime>()] = true;
				// NOTE: These don't even spawn anymore.
				player.npcTypeNoAggro[ModContent.NPCType<WulfrumSlime>()] = true;
			}

			if (modPlayer.dukeScales)
            {
				player.buffImmune[ModContent.BuffType<SulphuricPoisoning>()] = true;
				player.buffImmune[BuffID.Poisoned] = true;
				player.buffImmune[BuffID.Venom] = true;
                if (player.statLife <= (int)(player.statLifeMax2 * 0.75))
                {
                    player.allDamage += 0.06f;
					modPlayer.AllCritBoost(3);
                }
                if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
                {
                    player.allDamage += 0.06f;
					modPlayer.AllCritBoost(3);
                }
                if (player.statLife <= (int)(player.statLifeMax2 * 0.25))
                {
                    player.allDamage += 0.06f;
					modPlayer.AllCritBoost(3);
                }
				if (player.lifeRegen < 0)
                {
                    player.allDamage += 0.1f;
					modPlayer.AllCritBoost(5);
                }
            }

			if (modPlayer.auricSet && modPlayer.silvaMelee)
			{
				double multiplier = player.statLife / (double)player.statLifeMax2;
				player.meleeDamage += (float)(multiplier * 0.2); //ranges from 1.2 times to 1 times
			}

			if (modPlayer.dArtifact)
				player.allDamage += 0.25f;

			if (modPlayer.trippy)
				player.allDamage += 0.5f;

			if (modPlayer.eArtifact)
			{
				player.manaCost *= 0.85f;
				modPlayer.throwingDamage += 0.15f;
				player.maxMinions += 2;
			}

			if (modPlayer.gArtifact)
			{
				player.maxMinions += 8;
				if (player.whoAmI == Main.myPlayer)
				{
					if (player.FindBuffIndex(ModContent.BuffType<YharonKindleBuff>()) == -1)
						player.AddBuff(ModContent.BuffType<YharonKindleBuff>(), 3600, true);

					if (player.ownedProjectileCounts[ModContent.ProjectileType<SonOfYharon>()] < 2)
						Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<SonOfYharon>(), (int)(232f * player.MinionDamage()), 2f, Main.myPlayer, 0f, 0f);
				}
			}

			if (modPlayer.pArtifact)
			{
				if (player.whoAmI == Main.myPlayer)
				{
					if (player.FindBuffIndex(ModContent.BuffType<ProfanedBabs>()) == -1 && !player.Calamity().profanedCrystalBuffs)
						player.AddBuff(ModContent.BuffType<ProfanedBabs>(), 3600, true);

					bool crystal = modPlayer.profanedCrystal && !modPlayer.profanedCrystalForce;
					bool summonSet = modPlayer.tarraSummon || modPlayer.bloodflareSummon || modPlayer.godSlayerSummon || modPlayer.silvaSummon || modPlayer.dsSetBonus || modPlayer.omegaBlueSet || modPlayer.fearmongerSet;

					if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianHealer>()] < 1)
						Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -6f, ModContent.ProjectileType<MiniGuardianHealer>(), 0, 0f, Main.myPlayer, 0f, 0f);

					if (crystal || player.Calamity().minionSlotStat >= 10)
					{
						player.Calamity().gDefense = true;

						if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianDefense>()] < 1)
							Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -3f, ModContent.ProjectileType<MiniGuardianDefense>(), 1, 1f, Main.myPlayer, 0f, 0f);
					}

					if (crystal || summonSet)
					{
						player.Calamity().gOffense = true;

						if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianAttack>()] < 1)
							Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<MiniGuardianAttack>(), 1, 1f, Main.myPlayer, 0f, 0f);
					}
				}
			}

			if (modPlayer.profanedCrystalBuffs && modPlayer.gOffense && modPlayer.gDefense)
			{
				if (player.whoAmI == Main.myPlayer)
				{
					player.scope = false; //this is so it doesn't mess with the balance of ranged transform attacks over the others
					player.lavaImmune = true;
					player.lavaMax += 420;
					player.lavaRose = true;
					player.fireWalk = true;
					player.buffImmune[ModContent.BuffType<HolyFlames>()] = Main.dayTime;
					player.buffImmune[ModContent.BuffType<Nightwither>()] = !Main.dayTime;
					player.buffImmune[BuffID.OnFire] = true;
					player.buffImmune[BuffID.Burning] = true;
					player.buffImmune[BuffID.Daybreak] = true;
					bool offenseBuffs = (Main.dayTime && !player.wet) || player.lavaWet;
					if (offenseBuffs)
					{
						player.minionDamage += 0.15f;
						player.minionKB += 0.15f;
						player.moveSpeed += 0.25f;
						player.statDefense -= 15;
						if (!player.Calamity().yharonLore)
							player.wingTimeMax = (int)(player.wingTimeMax * 1.1f);
						player.ignoreWater = true;
					}
					else
					{
						player.moveSpeed -= 0.15f;
						player.endurance += 0.05f;
						player.statDefense += 15;
						player.lifeRegen += 5;
					}
					bool enrage = player.statLife <= (int)(player.statLifeMax2 * 0.5);
					bool notRetro = Lighting.NotRetro;
					if (!modPlayer.ZoneAbyss) //No abyss memes.
						Lighting.AddLight(player.Center, enrage ? 60 : offenseBuffs ? 50 : 10, enrage ? 12 : offenseBuffs ? 10 : 2, 0);
					if (enrage)
					{
						bool special = player.name == "Amber" || player.name == "Nincity" || player.name == "IbanPlay" || player.name == "Chen"; //People who either helped create the item or test it.
						for (int i = 0; i < 3; i++)
						{
							int fire = Dust.NewDust(player.position, player.width, player.height, special ? 231 : (int)CalamityDusts.ProfanedFire, 0f, 0f, 100, special ? Color.DarkRed : default, 1f);
							Main.dust[fire].scale = special ? 1.169f : 2f;
							Main.dust[fire].noGravity = true;
							Main.dust[fire].velocity *= special ? 10f : 6.9f;
						}
					}
				}
			}

			if (modPlayer.plaguebringerPistons)
			{
				//Spawn bees while sprinting or dashing
				modPlayer.pistonsCounter++;
				if (modPlayer.pistonsCounter % 12 == 0)
				{
					if (player.velocity.Length() >= 5f && player.whoAmI == Main.myPlayer)
					{
						int beeCount = 1;
						if (Main.rand.NextBool(3))
							++beeCount;
						if (Main.rand.NextBool(3))
							++beeCount;
						if (player.strongBees && Main.rand.NextBool(3))
							++beeCount;
						int damage = (int)(30 * player.MinionDamage());
						for (int index = 0; index < beeCount; ++index)
						{
							int bee = Projectile.NewProjectile(player.Center.X, player.Center.Y, Main.rand.NextFloat(-35f, 35f) * 0.02f, Main.rand.NextFloat(-35f, 35f) * 0.02f, (Main.rand.NextBool(4) ? ModContent.ProjectileType<PlagueBeeSmall>() : player.beeType()), damage, player.beeKB(0f), player.whoAmI, 0f, 0f);
							Main.projectile[bee].usesLocalNPCImmunity = true;
							Main.projectile[bee].localNPCHitCooldown = 10;
							Main.projectile[bee].penetrate = 2;
						}
					}
				}
			}

			List<int> summonDeleteList = new List<int>()
			{
				ModContent.ProjectileType<BrimstoneElementalMinion>(),
				ModContent.ProjectileType<WaterElementalMinion>(),
				ModContent.ProjectileType<SandElementalHealer>(),
				ModContent.ProjectileType<SandElementalMinion>(),
				ModContent.ProjectileType<CloudElementalMinion>(),
				ModContent.ProjectileType<FungalClumpMinion>(),
				ModContent.ProjectileType<HowlsHeartHowl>(),
				ModContent.ProjectileType<HowlsHeartCalcifer>(),
				ModContent.ProjectileType<HowlsHeartTurnipHead>()
			};
			for (int i = 0; i < summonDeleteList.Count; i++)
			{
				if (player.ownedProjectileCounts[summonDeleteList[i]] > 1)
				{
					for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
					{
						Projectile proj = Main.projectile[projIndex];
						if (proj.active && proj.owner == player.whoAmI)
						{
							if (summonDeleteList.Contains(proj.type))
							{
								proj.Kill();
							}
						}
					}
				}
			}

			if (modPlayer.blunderBooster)
			{
				if (player.whoAmI == Main.myPlayer)
				{
					if (player.ownedProjectileCounts[ModContent.ProjectileType<BlunderBoosterAura>()] < 1)
						Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<BlunderBoosterAura>(), (int)(30 * player.RogueDamage()), 0f, player.whoAmI, 0f, 0f);
				}
			}
			else if (player.ownedProjectileCounts[ModContent.ProjectileType<BlunderBoosterAura>()] != 0)
			{
				if (player.whoAmI == Main.myPlayer)
				{
					for (int i = 0; i < Main.maxProjectiles; i++)
					{
						if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<BlunderBoosterAura>() && Main.projectile[i].owner == player.whoAmI)
						{
							Main.projectile[i].Kill();
							break;
						}
					}
				}
			}

			if (modPlayer.tesla)
			{
				if (player.whoAmI == Main.myPlayer)
				{
					//Reduce the buffTime of Electrified
					for (int l = 0; l < Player.MaxBuffs; l++)
					{
						bool electrified = player.buffType[l] == BuffID.Electrified;
						if (player.buffTime[l] > 2 && electrified)
						{
							player.buffTime[l]--;
						}
					}
					//Summon the aura
					if (player.ownedProjectileCounts[ModContent.ProjectileType<TeslaAura>()] < 1)
						Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<TeslaAura>(), (int)(10 * player.AverageDamage()), 0f, player.whoAmI, 0f, 0f);
				}
			}
			else if (player.ownedProjectileCounts[ModContent.ProjectileType<TeslaAura>()] != 0)
			{
				if (player.whoAmI == Main.myPlayer)
				{
					for (int i = 0; i < Main.projectile.Length; i++)
					{
						if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<TeslaAura>() && Main.projectile[i].owner == player.whoAmI)
						{
							Main.projectile[i].Kill();
							break;
						}
					}
				}
			}

			if (modPlayer.prismaticLasers > 1800 && player.whoAmI == Main.myPlayer)
			{
				float shootSpeed = 18f;
				int dmg = (int)(50 * player.MagicDamage());
				Vector2 startPos = player.RotatedRelativePoint(player.MountedCenter, true);
				Vector2 velocity = Main.MouseWorld - startPos;
				if (player.gravDir == -1f)
				{
					velocity.Y = Main.screenPosition.Y + Main.screenHeight - Main.mouseY - startPos.Y;
				}
				float travelDist = velocity.Length();
				if ((float.IsNaN(velocity.X) && float.IsNaN(velocity.Y)) || (velocity.X == 0f && velocity.Y == 0f))
				{
					velocity.X = player.direction;
					velocity.Y = 0f;
					travelDist = shootSpeed;
				}
				else
				{
					travelDist = shootSpeed / travelDist;
				}

				int laserAmt = Main.rand.Next(2);
				for (int index = 0; index < laserAmt; index++)
				{
					startPos = new Vector2(player.Center.X + (Main.rand.Next(201) * -(float)player.direction) + (Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
					startPos.X = (startPos.X + player.Center.X) / 2f + Main.rand.Next(-200, 201);
					startPos.Y -= 100 * index;
					velocity.X = Main.mouseX + Main.screenPosition.X - startPos.X;
					velocity.Y = Main.mouseY + Main.screenPosition.Y - startPos.Y;
					if (velocity.Y < 0f)
					{
						velocity.Y *= -1f;
					}
					if (velocity.Y < 20f)
					{
						velocity.Y = 20f;
					}
					travelDist = velocity.Length();
					travelDist = shootSpeed / travelDist;
					velocity.X *= travelDist;
					velocity.Y *= travelDist;
					velocity.X += Main.rand.Next(-50, 51) * 0.02f;
					velocity.Y += Main.rand.Next(-50, 51) * 0.02f;
					int laser = Projectile.NewProjectile(startPos, velocity, ModContent.ProjectileType<MagicNebulaShot>(), dmg, 4f, player.whoAmI, 0f, 0f);
					Main.projectile[laser].localNPCHitCooldown = 5;
				}
				Main.PlaySound(SoundID.Item12, player.Center);
			}
			if (modPlayer.prismaticLasers == 1800)
			{
				//Set the cooldown
				player.AddBuff(ModContent.BuffType<PrismaticCooldown>(), CalamityUtils.SecondsToFrames(30f), true);
			}
			if (modPlayer.prismaticLasers == 1)
			{
				//Spawn some dust since you can use it again
                int dustAmt = 36;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
					Color color = Utils.SelectRandom(Main.rand, new Color[]
					{
						new Color(255, 0, 0, 50), //Red
						new Color(255, 128, 0, 50), //Orange
						new Color(255, 255, 0, 50), //Yellow
						new Color(128, 255, 0, 50), //Lime
						new Color(0, 255, 0, 50), //Green
						new Color(0, 255, 128, 50), //Turquoise
						new Color(0, 255, 255, 50), //Cyan
						new Color(0, 128, 255, 50), //Light Blue
						new Color(0, 0, 255, 50), //Blue
						new Color(128, 0, 255, 50), //Purple
						new Color(255, 0, 255, 50), //Fuschia
						new Color(255, 0, 128, 50) //Hot Pink
					});
                    Vector2 source = Vector2.Normalize(player.velocity) * new Vector2(player.width / 2f, player.height) * 0.75f;
                    source = source.RotatedBy((dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / dustAmt, default) + player.Center;
                    Vector2 dustVel = source - player.Center;
                    int dusty = Dust.NewDust(source + dustVel, 0, 0, 267, dustVel.X * 1f, dustVel.Y * 1f, 100, color, 1f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].noLight = true;
                    Main.dust[dusty].velocity = dustVel;
                }
			}

			if (modPlayer.theBee)
			{
				if (player.statLife >= player.statLifeMax2)
				{
					float beeBoost = player.endurance / 2f;
					player.allDamage += beeBoost;
				}
			}

			if (modPlayer.badgeOfBravery)
			{
				if ((player.armor[0].type == ModContent.ItemType<TarragonHelmet>() || player.armor[0].type == ModContent.ItemType<TarragonHelm>() ||
					player.armor[0].type == ModContent.ItemType<TarragonHornedHelm>() || player.armor[0].type == ModContent.ItemType<TarragonMask>() ||
					player.armor[0].type == ModContent.ItemType<TarragonVisage>()) &&
					player.armor[1].type == ModContent.ItemType<TarragonBreastplate>() && player.armor[2].type == ModContent.ItemType<TarragonLeggings>())
				{
					player.meleeDamage += 0.1f;
					player.meleeCrit += 5;
				}
			}

			if (CalamityConfig.Instance.Proficiency)
				modPlayer.GetStatBonuses();

			// True melee damage bonuses
			double damageAdd = (modPlayer.dodgeScarf ? 0.15 : 0) +
					(modPlayer.evasionScarf ? 0.1 : 0) +
					((modPlayer.aBulwarkRare && modPlayer.aBulwarkRareMeleeBoostTimer > 0) ? 0.5 : 0) +
					(modPlayer.DoGLore ? 0.25 : 0) +
					(modPlayer.fungalSymbiote ? 0.15 : 0) +
					((player.head == ArmorIDs.Head.MoltenHelmet && player.body == ArmorIDs.Body.MoltenBreastplate && player.legs == ArmorIDs.Legs.MoltenGreaves) ? 0.2 : 0) +
					(player.kbGlove ? 0.1 : 0) +
					(modPlayer.eGauntlet ? 0.1 : 0) +
					(modPlayer.yInsignia ? 0.1 : 0) +
					(modPlayer.badgeOfBraveryRare ? 0.2 : 0);
			modPlayer.trueMeleeDamage += damageAdd;
		}
		#endregion

		#region Limits
		private static void Limits(Player player, CalamityPlayer modPlayer)
		{
			//not sure where else this should go
			if (modPlayer.forbiddenCirclet)
			{
				float rogueDmg = player.thrownDamage + modPlayer.throwingDamage - 1f;
				float minionDmg = player.minionDamage;
				if (minionDmg < rogueDmg)
				{
					player.minionDamage = rogueDmg;
				}
				if (rogueDmg < minionDmg)
				{
					modPlayer.throwingDamage = minionDmg - player.thrownDamage + 1f;
				}
			}

			// 10% is converted to 9%, 25% is converted to 20%, 50% is converted to 33%, 75% is converted to 43%, 100% is converted to 50%
			if (player.endurance > 0f)
				player.endurance = 1f - (1f / (1f + player.endurance));

			// Very similar scaling to damage reduction. Ex. Frog Leg goes from 48% jump speed to 38%
			//if (player.jumpSpeedBoost > 0f)
			//	player.jumpSpeedBoost = (1f - 1f / (1f + (player.jumpSpeedBoost / 10f))) * 10f

			if (modPlayer.yharonLore && !CalamityWorld.defiled)
			{
				if (player.wingTimeMax < 50000)
					player.wingTimeMax = 50000;
			}

			// Do not apply reduced aggro if there are any bosses alive and it's singleplayer
			if (CalamityPlayer.areThereAnyDamnBosses && Main.netMode == NetmodeID.SinglePlayer)
			{
				if (player.aggro < 0)
					player.aggro = 0;
			}
		}
		#endregion

		#region Endurance Reductions
		private static void EnduranceReductions(Player player, CalamityPlayer modPlayer)
		{
			if (modPlayer.vHex)
				player.endurance -= 0.3f;
			if (modPlayer.irradiated)
			{
				if (modPlayer.boomerDukeLore)
				{
					player.endurance += 0.05f;
				}
				else
				{
					player.endurance -= 0.1f;
				}
			}
			if (modPlayer.corrEffigy)
				player.endurance -= 0.2f;
			if (modPlayer.marked || modPlayer.reaperToothNecklace)
			{
				if (player.endurance > 0f)
					player.endurance *= 0.5f;
			}
		}
		#endregion

		#region Stat Meter
		private static void UpdateStatMeter(Player player, CalamityPlayer modPlayer)
		{
			float allDamageStat = player.allDamage - 1f;
			modPlayer.actualMeleeDamageStat = player.meleeDamage + allDamageStat;
			modPlayer.damageStats[0] = (int)((player.meleeDamage + allDamageStat - 1f) * 100f);
			modPlayer.damageStats[1] = (int)((player.rangedDamage + allDamageStat - 1f) * 100f);
			modPlayer.damageStats[2] = (int)((player.magicDamage + allDamageStat - 1f) * 100f);
			modPlayer.damageStats[3] = (int)((player.minionDamage + allDamageStat - 1f) * 100f);
			modPlayer.damageStats[4] = (int)((modPlayer.throwingDamage + allDamageStat - 1f) * 100f);
			modPlayer.damageStats[5] = (int)(modPlayer.trueMeleeDamage * 100D);
			modPlayer.critStats[0] = player.meleeCrit;
			modPlayer.critStats[1] = player.rangedCrit;
			modPlayer.critStats[2] = player.magicCrit;
			modPlayer.critStats[3] = player.thrownCrit + modPlayer.throwingCrit;
			modPlayer.ammoReductionRanged = (int)(100f *
				(player.ammoBox ? 0.8f : 1f) *
				(player.ammoPotion ? 0.8f : 1f) *
				(player.ammoCost80 ? 0.8f : 1f) *
				(player.ammoCost75 ? 0.75f : 1f));
			modPlayer.ammoReductionRogue = (int)(modPlayer.throwingAmmoCost * 100);
			modPlayer.defenseStat = player.statDefense;
			modPlayer.DRStat = (int)(player.endurance * 100f);
			modPlayer.meleeSpeedStat = (int)((1f - player.meleeSpeed) * (100f / player.meleeSpeed));
			modPlayer.manaCostStat = (int)(player.manaCost * 100f);
			modPlayer.rogueVelocityStat = (int)((modPlayer.throwingVelocity - 1f) * 100f);

			// Max stealth 1f is actually "100 stealth", so multiply by 100 to get visual stealth number.
			modPlayer.stealthStat = (int)(modPlayer.rogueStealthMax * 100f);
			// Then divide by 3, because it takes 3 seconds to regen full stealth.
			// Divide by 3 again for moving, because it recharges at 1/3 speed (so divide by 9 overall).
			// Then multiply by stealthGen variables, which start at 1f and increase proportionally to your boosts.
			modPlayer.standingRegenStat = (modPlayer.rogueStealthMax * 100f / 3f) * modPlayer.stealthGenStandstill;
			modPlayer.movingRegenStat = (modPlayer.rogueStealthMax * 100f / 9f) * modPlayer.stealthGenMoving * modPlayer.stealthAcceleration;

			modPlayer.minionSlotStat = player.maxMinions;
			modPlayer.manaRegenStat = player.manaRegen;
			modPlayer.armorPenetrationStat = player.armorPenetration;
			modPlayer.moveSpeedStat = (int)((player.moveSpeed - 1f) * 100f);
			modPlayer.wingFlightTimeStat = player.wingTimeMax / 60f;
			float trueJumpSpeedBoost = player.jumpSpeedBoost + 
				(player.wereWolf ? 0.2f : 0f) +
				(player.jumpBoost ? 1.5f : 0f);
			modPlayer.jumpSpeedStat = trueJumpSpeedBoost * 20f;
			modPlayer.adrenalineChargeStat = 45 -
				(modPlayer.adrenalineBoostOne ? 10 : 0) -
				(modPlayer.adrenalineBoostTwo ? 10 : 0) -
				(modPlayer.adrenalineBoostThree ? 5 : 0);
			bool DHorHoD = modPlayer.draedonsHeart || modPlayer.heartOfDarkness;
			int rageDamageBoost = 0 +
				(modPlayer.rageBoostOne ? 15 : 0) +
				(modPlayer.rageBoostTwo ? 15 : 0) +
				(modPlayer.rageBoostThree ? 15 : 0);
			modPlayer.rageDamageStat = (DHorHoD ? 65 : 50) + rageDamageBoost;
		}
		#endregion

		#region Rogue Mirrors
		private static void RogueMirrors(Player player, CalamityPlayer modPlayer)
		{
			Rectangle rectangle = new Rectangle((int)(player.position.X + player.velocity.X * 0.5f - 4f), (int)(player.position.Y + player.velocity.Y * 0.5f - 4f), player.width + 8, player.height + 8);
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.active && !npc.dontTakeDamage && !npc.friendly && !npc.townNPC && npc.immune[player.whoAmI] <= 0 && npc.damage > 0)
				{
					Rectangle rect = npc.getRect();
					if (rectangle.Intersects(rect) && (npc.noTileCollide || player.CanHit(npc)))
					{
						if (Main.rand.NextBool(10) && player.immuneTime <= 0)
						{
							modPlayer.AbyssMirrorEvade();
							modPlayer.EclipseMirrorEvade();
						}
						break;
					}
				}
			}

			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile proj = Main.projectile[i];
				if (proj.active && proj.hostile && proj.damage > 0)
				{
					Rectangle rect = proj.getRect();
					if (rectangle.Intersects(rect))
					{
						if (Main.rand.NextBool(10))
						{
							modPlayer.AbyssMirrorEvade();
							modPlayer.EclipseMirrorEvade();
						}
						break;
					}
				}
			}
		}
		#endregion

		#region Double Jumps
		private static void DoubleJumps(Player player, CalamityPlayer modPlayer)
		{
			if (CalamityUtils.CountHookProj() > 0 || player.sliding || player.autoJump && player.justJumped)
			{
				modPlayer.jumpAgainSulfur = true;
				modPlayer.jumpAgainStatigel = true;
				return;
			}

			bool mountCheck = true;
			if (player.mount != null && player.mount.Active)
				mountCheck = player.mount.BlockExtraJumps;
			bool carpetCheck = true;
			if (player.carpet)
				carpetCheck = player.carpetTime <= 0 && player.canCarpet;

			if (player.position.Y == player.oldPosition.Y && player.wingTime == player.wingTimeMax && mountCheck && carpetCheck)
			{
				modPlayer.jumpAgainSulfur = true;
				modPlayer.jumpAgainStatigel = true;
			}
		}
		#endregion

		#region Potion Handling
		private static void HandlePotions(Player player, CalamityPlayer modPlayer)
		{
			if (modPlayer.potionTimer > 0)
				modPlayer.potionTimer--;
			if (modPlayer.potionTimer > 0 && player.potionDelay == 0)
				player.potionDelay = modPlayer.potionTimer;
			if (modPlayer.potionTimer == 1)
			{
				//Reduced duration than normal
				int duration = 3000;
				if (player.pStone)
					duration = (int)(duration * 0.75);
				player.ClearBuff(BuffID.PotionSickness);
				player.AddBuff(BuffID.PotionSickness, duration);
			}

			if (PlayerInput.Triggers.JustPressed.QuickBuff)
			{
				for (int i = 0; i < Main.maxInventory; ++i)
				{
					Item item = player.inventory[i];

					if (player.potionDelay > 0 && modPlayer.potionTimer > 0)
						continue;
					if (item is null || item.stack <= 0)
						continue;

					if (item.type == ModContent.ItemType<SunkenStew>())
						CalamityUtils.ConsumeItemViaQuickBuff(player, item, SunkenStew.BuffType, SunkenStew.BuffDuration, true);
					if (item.type == ModContent.ItemType<Margarita>())
						CalamityUtils.ConsumeItemViaQuickBuff(player, item, Margarita.BuffType, Margarita.BuffDuration, false);
					if (item.type == ModContent.ItemType<Bloodfin>())
						CalamityUtils.ConsumeItemViaQuickBuff(player, item, Bloodfin.BuffType, Bloodfin.BuffDuration, false);
				}
			}
		}
		#endregion
	}
}
