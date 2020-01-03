using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.Cooldowns;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Pets;
using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Summon;
using CalamityMod.Dusts;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.NPCs;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.UI;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

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
			if (Config.ExpertDebuffDurationReduction)
				Main.expertDebuffTime = 1f;

			// Bool for any existing bosses, true if any boss NPC is active
			CalamityPlayer.areThereAnyDamnBosses = CalamityGlobalNPC.AnyBossNPCS();

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
			StatMeter(player, modPlayer);

			// Rogue Mirrors
			RogueMirrors(player, modPlayer);
		}
		#endregion

		#region Revengeance Effects
		private static void RevengeanceModeMiscEffects(Player player, CalamityPlayer modPlayer, Mod mod)
		{
			if (CalamityWorld.revenge)
			{
				// Life Steal nerf
				if (player.lifeSteal > (CalamityWorld.death ? 50f : 60f))
					player.lifeSteal = CalamityWorld.death ? 50f : 60f;

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
					if (Config.AdrenalineAndRage)
					{
						// Amount of Rage gained per 'tick'
						int stressGain = 0;
						if (modPlayer.rageMode)
							stressGain = -2000;
						else
						{
							if (modPlayer.draedonsHeart)
							{
								if (modPlayer.draedonsStressGain)
									stressGain += 60;
							}
							else if (modPlayer.heartOfDarkness)
								stressGain += 30;
						}

						// Add or subtract the amount of Rage gained every 'tick'
						modPlayer.stressCD++;
						if (modPlayer.stressCD >= 60)
						{
							modPlayer.stressCD = 0;
							modPlayer.stress += stressGain;
							if (modPlayer.stress < 0)
								modPlayer.stress = 0;

							if (modPlayer.stress >= modPlayer.stressMax)
							{
								// Play a sound when the Rage Meter is full
								if (modPlayer.playFullRageSound)
								{
									modPlayer.playFullRageSound = false;
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/FullRage"), (int)player.position.X, (int)player.position.Y);
								}
								modPlayer.stress = modPlayer.stressMax;
							}
							else
								modPlayer.playFullRageSound = true;
						}

						// Randomly-granted Absolute Rage buff when Rage is nearly full
						modPlayer.stressLevel500 = modPlayer.stress >= 9800;
						if (modPlayer.stressLevel500 && !modPlayer.hAttack)
						{
							int heartAttackChance = (modPlayer.draedonsHeart || modPlayer.heartOfDarkness) ? 2000 : 10000;
							if (Main.rand.Next(heartAttackChance) == 0)
								player.AddBuff(ModContent.BuffType<HeartAttack>(), 18000);
						}

						// Play Adrenaline burnout sounds that happen the more time you wait while having max Adrenaline
						if (modPlayer.adrenaline >= modPlayer.adrenalineMax)
						{
							modPlayer.adrenalineMaxTimer--;
							if (modPlayer.adrenalineMaxTimer <= 0)
							{
								if (modPlayer.playAdrenalineBurnoutSound)
								{
									modPlayer.playAdrenalineBurnoutSound = false;
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AdrenalineBurnout1"), (int)player.position.X, (int)player.position.Y);
								}

								modPlayer.adrenalineDmgDown--;
								if (modPlayer.adrenalineDmgDown < 0)
								{
									if (modPlayer.playFullAdrenalineBurnoutSound)
									{
										modPlayer.playFullAdrenalineBurnoutSound = false;
										Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AdrenalineBurnout2"), (int)player.position.X, (int)player.position.Y);
									}
									modPlayer.adrenalineDmgDown = 0;
								}
								modPlayer.adrenalineMaxTimer = 0;
							}
						}

						// Reset Adrenaline burnout variables
						else if (!modPlayer.adrenalineMode && modPlayer.adrenaline <= 0)
						{
							modPlayer.playAdrenalineBurnoutSound = true;
							modPlayer.playFullAdrenalineBurnoutSound = true;
							modPlayer.adrenalineDmgDown = 600;
							modPlayer.adrenalineMaxTimer = 300;
							modPlayer.adrenalineDmgMult = 1f;
						}

						// Reduce Adrenaline Mode damage based on burnout
						modPlayer.adrenalineDmgMult = 0.1f * (float)(modPlayer.adrenalineDmgDown / 60);
						if (modPlayer.adrenalineDmgMult < 0.33f)
							modPlayer.adrenalineDmgMult = 0.33f;

						// Amount of Adrenaline gained per 'tick'
						int adrenalineGain = 0;
						bool SCalAlive = NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>());
						if (modPlayer.adrenalineMode)
							adrenalineGain = SCalAlive ? -10000 : -2000;
						else
						{
							if (Main.wof >= 0 && player.position.Y < (float)((Main.maxTilesY - 200) * 16)) // >
								modPlayer.adrenaline = 0;
							else if (CalamityPlayer.areThereAnyDamnBosses || CalamityWorld.DoGSecondStageCountdown > 0)
							{
								int adrenalineTickBoost = 0 +
									(modPlayer.adrenalineBoostOne ? 63 : 0) + // 286
									(modPlayer.adrenalineBoostTwo ? 115 : 0) + // 401
									(modPlayer.adrenalineBoostThree ? 100 : 0); // 501
								adrenalineGain = 223 + adrenalineTickBoost; // pre-slime god = 45, pre-astrum deus = 35, pre-polterghast = 25, post-polter = 20
							}
							else
								modPlayer.adrenaline = 0;
						}

						// Add or subtract the amount of Adrenaline gained every 'tick'
						modPlayer.adrenalineCD++;
						if (modPlayer.adrenalineCD >= (SCalAlive ? 135 : 60))
						{
							modPlayer.adrenalineCD = 0;
							modPlayer.adrenaline += adrenalineGain;
							if (modPlayer.adrenaline < 0)
								modPlayer.adrenaline = 0;

							if (modPlayer.adrenaline >= modPlayer.adrenalineMax)
							{
								if (modPlayer.playFullAdrenalineSound)
								{
									modPlayer.playFullAdrenalineSound = false;
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/FullAdrenaline"), (int)player.position.X, (int)player.position.Y);
								}
								modPlayer.adrenaline = modPlayer.adrenalineMax;
							}
							else
								modPlayer.playFullAdrenalineSound = true;
						}
					}
				}
			}

			// If not in Revengeance Mode
			else
			{
				// Reduce Rage and Adrenaline until they are back to 0
				if (player.whoAmI == Main.myPlayer)
				{
					modPlayer.stressCD++;
					if (modPlayer.stressCD >= 60)
					{
						modPlayer.stressCD = 0;
						modPlayer.stress += -30;
						if (modPlayer.stress < 0)
							modPlayer.stress = 0;
					}

					modPlayer.adrenalineCD++;
					if (modPlayer.adrenalineCD >= 60)
					{
						modPlayer.adrenalineCD = 0;
						modPlayer.adrenaline += -30;
						if (modPlayer.adrenaline < 0)
							modPlayer.adrenaline = 0;
					}
				}
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
				}
			}
		}
		#endregion

		#region Misc Effects
		private static void MiscEffects(Player player, CalamityPlayer modPlayer, Mod mod)
		{
			// Proficiency level ups
			if (Config.ProficiencyEnabled)
				modPlayer.GetExactLevelUp();

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

			// Immunity to most debuffs
			if (modPlayer.silvaSet || modPlayer.invincible || modPlayer.margarita)
			{
				foreach (int debuff in CalamityMod.debuffList)
					player.buffImmune[debuff] = true;
			}

			// Transformer immunity to Electrified
			if (modPlayer.aSparkRare)
				player.buffImmune[BuffID.Electrified] = true;

			// Reduce breath meter while in icy water instead of chilling
			if (Config.ExpertChilledWaterRemoval)
			{
				if (Main.expertMode && player.ZoneSnow && player.wet && !player.lavaWet && !player.honeyWet && !player.arcticDivingGear)
				{
					player.buffImmune[BuffID.Chilled] = true;
					if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
					{
						if (Main.myPlayer == player.whoAmI && !player.gills && !player.merman)
						{
							if (player.breath > 0)
								player.breath--;
						}
					}
				}
			}

			// Increase fall speed
			if (!player.mount.Active)
			{
				if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) && modPlayer.ironBoots)
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
				if (player.ownedProjectileCounts[ModContent.ProjectileType<OmegaBlueTentacle>()] < 6)
				{
					bool[] tentaclesPresent = new bool[6];
					for (int i = 0; i < 1000; i++)
					{
						Projectile projectile = Main.projectile[i];
						if (projectile.active && projectile.type == ModContent.ProjectileType<OmegaBlueTentacle>() && projectile.owner == Main.myPlayer && projectile.ai[1] >= 0f && projectile.ai[1] < 6f)
							tentaclesPresent[(int)projectile.ai[1]] = true;
					}

					for (int i = 0; i < 6; i++)
					{
						if (!tentaclesPresent[i])
						{
							float modifier = player.meleeDamage + player.magicDamage + player.rangedDamage +
								modPlayer.throwingDamage + player.minionDamage;

							modifier /= 5f;
							int damage = (int)(666 * modifier);
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

			// Guardian bonuses if not burnt out
			if (!modPlayer.bOut)
			{
				if (modPlayer.gHealer)
				{
					if (modPlayer.healCounter > 0)
						modPlayer.healCounter--;

					if (modPlayer.healCounter <= 0)
					{
						modPlayer.healCounter = 300;
						if (player.whoAmI == Main.myPlayer)
						{
							int healAmount = 5 +
								(modPlayer.gDefense ? 5 : 0) +
								(modPlayer.gOffense ? 5 : 0);

							player.statLife += healAmount;
							player.HealEffect(healAmount);
						}
					}
				}

				if (modPlayer.gDefense)
				{
					player.moveSpeed += 0.1f +
						(modPlayer.gOffense ? 0.1f : 0f);
					player.endurance += 0.025f +
						(modPlayer.gOffense ? 0.025f : 0f);
				}

				if (modPlayer.gOffense)
				{
					player.minionDamage += 0.1f +
						(modPlayer.gDefense ? 0.05f : 0f);
				}
			}

			// You always get the max minions, even during the effect of the burnout debuff
			if (modPlayer.gOffense)
				player.maxMinions++;

			// Cooldowns and timers
			if (modPlayer.gainRageCooldown > 0)
				modPlayer.gainRageCooldown--;
			if (modPlayer.galileoCooldown > 0)
				modPlayer.galileoCooldown--;
			if (modPlayer.raiderCooldown > 0)
				modPlayer.raiderCooldown--;
			if (modPlayer.gSabatonCooldown > 0)
				modPlayer.gSabatonCooldown--;
			if (modPlayer.gSabatonFall > 0)
				modPlayer.gSabatonFall--;
			if (modPlayer.draconicSurgeCooldown > 0)
				modPlayer.draconicSurgeCooldown--;
			if (modPlayer.fleshTotemCooldown > 0)
				modPlayer.fleshTotemCooldown--;
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
            if (modPlayer.sandCloakCooldown > 0)
				modPlayer.sandCloakCooldown--;
			if (modPlayer.spectralVeilImmunity > 0)
				modPlayer.spectralVeilImmunity--;
			if (modPlayer.plaguedFuelPackCooldown > 0)
				modPlayer.plaguedFuelPackCooldown--;
			if (modPlayer.plaguedFuelPackDash > 0)
				modPlayer.plaguedFuelPackDash--;
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

			// Silva invincibility effects
			if (modPlayer.silvaCountdown > 0 && modPlayer.hasSilvaEffect && modPlayer.silvaSet)
			{
				player.buffImmune[ModContent.BuffType<VulnerabilityHex>()] = true;
				modPlayer.silvaCountdown--;
				if (modPlayer.silvaCountdown <= 0)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaDispel"), (int)player.position.X, (int)player.position.Y);

				for (int j = 0; j < 2; j++)
				{
					int num = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 2f);
					Main.dust[num].position.X += (float)Main.rand.Next(-20, 21);
					Main.dust[num].position.Y += (float)Main.rand.Next(-20, 21);
					Main.dust[num].velocity *= 0.9f;
					Main.dust[num].noGravity = true;
					Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
					Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
					if (Main.rand.NextBool(2))
						Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
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
					int num = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 2f);
					Dust expr_A4_cp_0 = Main.dust[num];
					expr_A4_cp_0.position.X += (float)Main.rand.Next(-20, 21);
					Dust expr_CB_cp_0 = Main.dust[num];
					expr_CB_cp_0.position.Y += (float)Main.rand.Next(-20, 21);
					Main.dust[num].velocity *= 0.9f;
					Main.dust[num].noGravity = true;
					Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
					Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
					if (Main.rand.NextBool(2))
						Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
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

					bool shouldAffect = CalamityMod.debuffList.Contains(hasBuff);
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
						int num = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 5, 0f, 0f, 100, default, 2f);
						Dust expr_A4_cp_0 = Main.dust[num];
						expr_A4_cp_0.position.X += (float)Main.rand.Next(-20, 21);
						Dust expr_CB_cp_0 = Main.dust[num];
						expr_CB_cp_0.position.Y += (float)Main.rand.Next(-20, 21);
						Main.dust[num].velocity *= 0.9f;
						Main.dust[num].noGravity = true;
						Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
						Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
						if (Main.rand.NextBool(2))
							Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
					}
				}
			}

			// Acid Rain debuff
			if (Main.raining && modPlayer.ZoneSulphur)
			{
				if (player.ZoneOverworldHeight || player.ZoneSkyHeight)
					player.AddBuff(ModContent.BuffType<Irradiated>(), 2);
			}

			// Raider Talisman bonus
			if (modPlayer.raiderTalisman)
			{
				float damageMult = modPlayer.nanotech ? 0.1f : 0.25f;
				modPlayer.throwingDamage += (float)modPlayer.raiderStack / 250f * damageMult;
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

			// Absorber bonus
			if (modPlayer.absorber)
			{
				player.moveSpeed += 0.12f;
				player.jumpSpeedBoost += 1.2f;
				player.thorns = 0.5f;
				player.endurance += 0.05f;

				if ((double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
					player.manaRegenBonus += 2;

				//This stacks with Sea Shell
				if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
				{
					player.statDefense += 2;
					player.moveSpeed += 0.05f;
				}
			}

			// Sea Shell bonus
			if (modPlayer.seaShell)
			{
				if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
				{
					player.statDefense += 3;
					player.endurance += 0.05f;
					player.moveSpeed += 0.15f;
					player.ignoreWater = true;
				}
			}

			// Stress Pills bonus
			if (modPlayer.stressPills)
			{
				player.statDefense += 8;
				player.allDamage += 0.08f;
			}

			// Laudanum bonus
			if (modPlayer.laudanum)
			{
				player.statDefense += 6;
				player.allDamage += 0.06f;
			}

			// Draedon's Heart bonus
			if (modPlayer.draedonsHeart)
			{
				player.allDamage += 0.1f;
				if ((double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
					player.statDefense += 25;
			}

			// Remove Absolute Rage buff if Rage Meter is lower than 9800
			if (!modPlayer.stressLevel500 && player.FindBuffIndex(ModContent.BuffType<HeartAttack>()) > -1)
				player.ClearBuff(ModContent.BuffType<HeartAttack>());

			// Absolute Rage bonus
			if (modPlayer.hAttack)
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
				light[1] += (float)Main.DiscoG / 400f;
				light[2] += 0.5f;
			}
			if (modPlayer.sirenIce)
			{
				light[0] += 0.35f;
				light[1] += 1f;
				light[2] += 1.25f;
			}
			if (modPlayer.sirenBoobs || modPlayer.sirenBoobsAlt)
			{
				light[0] += 1.5f;
				light[1] += 1f;
				light[2] += 0.1f;
			}
			if (modPlayer.tarraSummon)
			{
				light[0] += 0f;
				light[1] += 3f;
				light[2] += 0f;
			}
			if (modPlayer.dAmulet)
			{
				if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
				{
					light[0] += 1.35f;
					light[1] += 0.3f;
					light[2] += 0.9f;
				}
			}
			Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), light[0], light[1], light[2]);

			// Blazing Core bonus
			if (modPlayer.blazingCore)
				player.endurance += 0.1f;

			// Cosmic Discharge Cosmic Freeze buff, gives surrounding enemies the Glacial State debuff
			if (modPlayer.cFreeze)
			{
				int num = ModContent.BuffType<GlacialState>();
				float num2 = 200f;
				int random = Main.rand.Next(5);
				if (player.whoAmI == Main.myPlayer)
				{
					if (random == 0)
					{
						for (int l = 0; l < 200; l++)
						{
							NPC nPC = Main.npc[l];
							if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && !nPC.buffImmune[num] && Vector2.Distance(player.Center, nPC.Center) <= num2)
							{
								if (nPC.FindBuffIndex(num) == -1)
									nPC.AddBuff(num, 120, false);
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
			if (!modPlayer.polarisBoost || player.inventory[player.selectedItem].type != ModContent.ItemType<PolarisParrotfish>())
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
			}
			if (modPlayer.desertScourgeLore)
				player.statDefense += 5;
			if (modPlayer.eaterOfWorldsLore)
			{
				int damage = 10;
				float knockBack = 1f;
				if (Main.rand.NextBool(15))
				{
					int num = 0;
					for (int i = 0; i < 1000; i++)
					{
						if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<TheDeadlyMicrobeProjectile>())
							num++;
					}

					if (Main.rand.Next(15) >= num && num < 6)
					{
						int num2 = 50;
						int num3 = 24;
						int num4 = 90;

						for (int j = 0; j < num2; j++)
						{
							int num5 = Main.rand.Next(200 - j * 2, 400 + j * 2);
							Vector2 center = player.Center;
							center.X += (float)Main.rand.Next(-num5, num5 + 1);
							center.Y += (float)Main.rand.Next(-num5, num5 + 1);

							if (!Collision.SolidCollision(center, num3, num3) && !Collision.WetCollision(center, num3, num3))
							{
								center.X += (float)(num3 / 2);
								center.Y += (float)(num3 / 2);

								if (Collision.CanHit(new Vector2(player.Center.X, player.position.Y), 1, 1, center, 1, 1) || Collision.CanHit(new Vector2(player.Center.X, player.position.Y - 50f), 1, 1, center, 1, 1))
								{
									int num6 = (int)center.X / 16;
									int num7 = (int)center.Y / 16;
									bool flag = false;

									if (Main.rand.NextBool(3) && Main.tile[num6, num7] != null && Main.tile[num6, num7].wall > 0)
										flag = true;
									else
									{
										center.X -= (float)(num4 / 2);
										center.Y -= (float)(num4 / 2);

										if (Collision.SolidCollision(center, num4, num4))
										{
											center.X += (float)(num4 / 2);
											center.Y += (float)(num4 / 2);
											flag = true;
										}
									}

									if (flag)
									{
										for (int k = 0; k < 1000; k++)
										{
											if (Main.projectile[k].active && Main.projectile[k].owner == player.whoAmI && Main.projectile[k].type == ModContent.ProjectileType<TheDeadlyMicrobeProjectile>() && (center - Main.projectile[k].Center).Length() < 48f)
											{
												flag = false;
												break;
											}
										}

										if (flag && Main.myPlayer == player.whoAmI)
											Projectile.NewProjectile(center.X, center.Y, 0f, 0f, ModContent.ProjectileType<TheDeadlyMicrobeProjectile>(), damage, knockBack, player.whoAmI, 0f, 0f);
									}
								}
							}
						}
					}
				}
			}
			if (modPlayer.skeletronLore)
			{
				player.allDamage += 0.05f;
				modPlayer.AllCritBoost(5);
				player.statDefense += 5;
			}
			if (modPlayer.destroyerLore)
				player.pickSpeed -= 0.05f;
			if (modPlayer.aquaticScourgeLore && player.wellFed)
			{
				player.statDefense += 1;
				player.allDamage += 0.025f;
				modPlayer.AllCritBoost(1);
				player.meleeSpeed += 0.025f;
				player.minionKB += 0.25f;
				player.moveSpeed += 0.1f;
			}
			if (modPlayer.skeletronPrimeLore)
				player.armorPenetration += 5;
			if (modPlayer.leviathanAndSirenLore)
			{
				if (modPlayer.sirenPet)
				{
					player.spelunkerTimer += 1;
					if (player.spelunkerTimer >= 10)
					{
						player.spelunkerTimer = 0;
						int num65 = 30;
						int num66 = (int)player.Center.X / 16;
						int num67 = (int)player.Center.Y / 16;

						for (int num68 = num66 - num65; num68 <= num66 + num65; num68++)
						{
							for (int num69 = num67 - num65; num69 <= num67 + num65; num69++)
							{
								if (Main.rand.NextBool(4))
								{
									Vector2 vector = new Vector2((float)(num66 - num68), (float)(num67 - num69));
									if (vector.Length() < (float)num65 && num68 > 0 && num68 < Main.maxTilesX - 1 && num69 > 0 && num69 < Main.maxTilesY - 1 && Main.tile[num68, num69] != null && Main.tile[num68, num69].active())
									{
										bool flag7 = false;
										if (Main.tile[num68, num69].type == 185 && Main.tile[num68, num69].frameY == 18)
										{
											if (Main.tile[num68, num69].frameX >= 576 && Main.tile[num68, num69].frameX <= 882)
												flag7 = true;
										}
										else if (Main.tile[num68, num69].type == 186 && Main.tile[num68, num69].frameX >= 864 && Main.tile[num68, num69].frameX <= 1170)
											flag7 = true;

										if (flag7 || Main.tileSpelunker[(int)Main.tile[num68, num69].type] || (Main.tileAlch[(int)Main.tile[num68, num69].type] && Main.tile[num68, num69].type != 82))
										{
											int num70 = Dust.NewDust(new Vector2((float)(num68 * 16), (float)(num69 * 16)), 16, 16, 204, 0f, 0f, 150, default, 0.3f);
											Main.dust[num70].fadeIn = 0.75f;
											Main.dust[num70].velocity *= 0.1f;
											Main.dust[num70].noLight = true;
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
				if ((double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
					player.statDefense += 10;
			}
			if (modPlayer.dukeFishronLore)
			{
				player.allDamage += 0.05f;
				modPlayer.AllCritBoost(5);
				player.moveSpeed += 0.1f;
			}
			if (modPlayer.lunaticCultistLore)
			{
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
			}

			// Calcium Potion buff
			if (modPlayer.calcium)
				player.noFallDmg = true;

			// Ceaseless Hunger Potion buff
			if (modPlayer.ceaselessHunger)
			{
				for (int j = 0; j < 400; j++)
				{
					if (Main.item[j].active && Main.item[j].noGrabDelay == 0 && Main.item[j].owner == player.whoAmI)
					{
						Main.item[j].beingGrabbed = true;
						if ((double)player.position.X + (double)player.width * 0.5 > (double)Main.item[j].position.X + (double)Main.item[j].width * 0.5)
						{
							if (Main.item[j].velocity.X < 90f + player.velocity.X)
							{
								Item item = Main.item[j];
								item.velocity.X += 9f;
							}
							if (Main.item[j].velocity.X < 0f)
							{
								Item item = Main.item[j];
								item.velocity.X += 9f * 0.75f;
							}
						}
						else
						{
							if (Main.item[j].velocity.X > -90f + player.velocity.X)
							{
								Item item = Main.item[j];
								item.velocity.X -= 9f;
							}
							if (Main.item[j].velocity.X > 0f)
							{
								Item item = Main.item[j];
								item.velocity.X -= 9f * 0.75f;
							}
						}

						if ((double)player.position.Y + (double)player.height * 0.5 > (double)Main.item[j].position.Y + (double)Main.item[j].height * 0.5)
						{
							if (Main.item[j].velocity.Y < 90f)
							{
								Item item = Main.item[j];
								item.velocity.Y += 9f;
							}
							if (Main.item[j].velocity.Y < 0f)
							{
								Item item = Main.item[j];
								item.velocity.Y += 9f * 0.75f;
							}
						}
						else
						{
							if (Main.item[j].velocity.Y > -90f)
							{
								Item item = Main.item[j];
								item.velocity.Y -= 9f;
							}
							if (Main.item[j].velocity.Y > 0f)
							{
								Item item = Main.item[j];
								item.velocity.Y -= 9f * 0.75f;
							}
						}
					}
				}
			}

			// Spectral Veil effects
			if (modPlayer.spectralVeil && modPlayer.spectralVeilImmunity > 0)
			{
				Rectangle sVeilRectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && !Main.npc[i].townNPC && Main.npc[i].immune[player.whoAmI] <= 0 && Main.npc[i].damage > 0)
					{
						NPC nPC = Main.npc[i];
						Rectangle rect = nPC.getRect();
						if (sVeilRectangle.Intersects(rect) && (nPC.noTileCollide || player.CanHit(nPC)))
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

								Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaDispel"), (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);
							}
							break;
						}
					}
				}

				for (int i = 0; i < 1000; i++)
				{
					if (Main.projectile[i].active && !Main.projectile[i].friendly && Main.projectile[i].hostile && Main.projectile[i].damage > 0)
					{
						Projectile proj = Main.projectile[i];
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

								Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaDispel"), (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);
							}
							break;
						}
					}
				}
			}

			// Plagued Fuel Pack effects
			if (modPlayer.plaguedFuelPack && modPlayer.plaguedFuelPackDash > 0)
			{
				int velocityMult = modPlayer.plaguedFuelPackDash > 1 ? 25 : 5;
				player.velocity = new Vector2(modPlayer.plaguedFuelPackDirection, -1) * velocityMult;

				int numClouds = Main.rand.Next(2, 10);
				for (int i = 0; i < numClouds; i++)
				{
					Vector2 cloudVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
					cloudVelocity.Normalize();
					cloudVelocity *= Main.rand.NextFloat(0f, 1f);
					int projectile = Projectile.NewProjectile(player.Center, cloudVelocity, ModContent.ProjectileType<PlaguedFuelPackCloud>(), 20, 0, player.whoAmI, 0, 0);
					Main.projectile[projectile].timeLeft = Main.rand.Next(75, 125);
				}

				for (int i = 0; i < 3; i++)
				{
					int dust = Dust.NewDust(player.Center, 1, 1, 89, player.velocity.X * -0.1f, player.velocity.Y * -0.1f, 100, default, 3.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.2f;
					Main.dust[dust].velocity.Y -= 0.15f;
				}
			}

			// Gravistar Sabaton effects
			if (modPlayer.gSabaton)
			{
				if (modPlayer.gSabatonCooldown <= 0)
				{
					if (player.controlDown && player.releaseDown && player.position.Y != player.oldPosition.Y)
					{
						modPlayer.gSabatonFall = 300;
						modPlayer.gSabatonCooldown = 480; //8 second cooldown
						player.gravity *= 2f;
						Projectile.NewProjectile(player.Center.X, player.Center.Y, player.velocity.X, player.velocity.Y, ModContent.ProjectileType<SabatonSlam>(), 0, 0, player.whoAmI);
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
		}
		#endregion

		#region Abyss Effects
		private static void AbyssEffects(Player player, CalamityPlayer modPlayer)
		{
			int lightStrength = 0 +
				((player.lightOrb || player.crimsonHeart || player.magicLantern || modPlayer.radiator) ? 1 : 0) + // 1
				(modPlayer.aquaticEmblem ? 1 : 0) + // 2
				(player.arcticDivingGear ? 1 : 0) + // 3
				(modPlayer.jellyfishNecklace ? 1 : 0) + // 4
				((player.blueFairy || player.greenFairy || player.redFairy || player.petFlagDD2Ghost || modPlayer.babyGhostBell) ? 2 : 0) + // 6
				((modPlayer.shine || modPlayer.lumenousAmulet) ? 2 : 0) + // 8
				((player.wisp || player.suspiciouslookingTentacle || modPlayer.sirenPet) ? 3 : 0); // 11

			double breathLossMult = 1.0 -
				(player.gills ? 0.2 : 0.0) - // 0.8
				(player.accDivingHelm ? 0.25 : 0.0) - // 0.75
				(player.arcticDivingGear ? 0.25 : 0.0) - // 0.75
				(modPlayer.aquaticEmblem ? 0.25 : 0.0) - // 0.75
				(player.accMerman ? 0.3 : 0.0) - // 0.7
				(modPlayer.victideSet ? 0.2 : 0.0) - // 0.85
				(((modPlayer.sirenBoobs || modPlayer.sirenBoobsAlt) && NPC.downedBoss3) ? 0.3 : 0.0) - // 0.7
				(modPlayer.abyssalDivingSuit ? 0.3 : 0.0); // 0.7

			if (breathLossMult < 0.05)
				breathLossMult = 0.05;

			double tickMult = 1.0 +
				(player.gills ? 4.0 : 0.0) + // 5
				(player.ignoreWater ? 5.0 : 0.0) + // 10
				(player.accDivingHelm ? 10.0 : 0.0) + // 20
				(player.arcticDivingGear ? 10.0 : 0.0) + // 30
				(modPlayer.aquaticEmblem ? 10.0 : 0.0) + // 40
				(player.accMerman ? 15.0 : 0.0) + // 55
				(modPlayer.victideSet ? 5.0 : 0.0) + // 60
				(((modPlayer.sirenBoobs || modPlayer.sirenBoobsAlt) && NPC.downedBoss3) ? 15.0 : 0.0) + // 75
				(modPlayer.abyssalDivingSuit ? 15.0 : 0.0); // 90

			if (tickMult > 50.0)
				tickMult = 50.0;

			int lifeLossAtZeroBreathResist = 0;
			if (modPlayer.depthCharm)
			{
				lifeLossAtZeroBreathResist += 3;
				if (modPlayer.abyssalDivingSuit)
					lifeLossAtZeroBreathResist += 6;
			}

			modPlayer.abyssLightLevelStat = lightStrength;
			modPlayer.abyssBreathLossStats[0] = (int)(2D * breathLossMult);
			modPlayer.abyssBreathLossStats[1] = (int)(6D * breathLossMult);
			modPlayer.abyssBreathLossStats[2] = (int)(18D * breathLossMult);
			modPlayer.abyssBreathLossStats[3] = (int)(54D * breathLossMult);
			modPlayer.abyssBreathLossRateStat = (int)(6D * tickMult);
			modPlayer.abyssLifeLostAtZeroBreathStats[0] = 3 - lifeLossAtZeroBreathResist;
			modPlayer.abyssLifeLostAtZeroBreathStats[1] = 6 - lifeLossAtZeroBreathResist;
			modPlayer.abyssLifeLostAtZeroBreathStats[2] = 12 - lifeLossAtZeroBreathResist;
			modPlayer.abyssLifeLostAtZeroBreathStats[3] = 24 - lifeLossAtZeroBreathResist;

			if (modPlayer.abyssLifeLostAtZeroBreathStats[0] < 0)
				modPlayer.abyssLifeLostAtZeroBreathStats[0] = 0;
			if (modPlayer.abyssLifeLostAtZeroBreathStats[1] < 0)
				modPlayer.abyssLifeLostAtZeroBreathStats[1] = 0;

			if (modPlayer.ZoneAbyss)
			{
				if (Main.myPlayer == player.whoAmI) // 4200 total tiles small world
				{
					int breathLoss = 2;
					int lifeLossAtZeroBreath = 3;
					int tick = 6;

					bool lightLevelOne = lightStrength > 0; // 1+
					bool lightLevelTwo = lightStrength > 2; // 3+
					bool lightLevelThree = lightStrength > 4; // 5+
					bool lightLevelFour = lightStrength > 6; // 7+

					if (modPlayer.ZoneAbyssLayer4) // 3200 and below
					{
						breathLoss = 54;
						if (!lightLevelFour)
							player.blind = true;
						if (!lightLevelThree)
							player.headcovered = true;
						player.bleed = true;
						lifeLossAtZeroBreath = 24;
						player.statDefense -= modPlayer.anechoicPlating ? 40 : 120;
					}
					else if (modPlayer.ZoneAbyssLayer3) // 2700 to 3200
					{
						breathLoss = 18;
						if (!lightLevelThree)
							player.blind = true;
						if (!lightLevelTwo)
							player.headcovered = true;
						if (!modPlayer.abyssalDivingSuit)
							player.bleed = true;
						lifeLossAtZeroBreath = 12;
						player.statDefense -= modPlayer.anechoicPlating ? 20 : 60;
					}
					else if (modPlayer.ZoneAbyssLayer2) // 2100 to 2700
					{
						breathLoss = 6;
						if (!lightLevelTwo)
							player.blind = true;
						if (!modPlayer.depthCharm)
							player.bleed = true;
						lifeLossAtZeroBreath = 6;
						player.statDefense -= modPlayer.anechoicPlating ? 10 : 30;
					}
					else if (modPlayer.ZoneAbyssLayer1) // 1500 to 2100
					{
						if (!lightLevelOne)
							player.blind = true;
						player.statDefense -= modPlayer.anechoicPlating ? 5 : 15;
					}

					breathLoss = (int)((double)breathLoss * breathLossMult);
					tick = (int)((double)tick * tickMult);

					if (player.gills || player.merman)
					{
						if (player.breath > 0)
							player.breath -= 3;
					}

					modPlayer.abyssBreathCD++;
					if (modPlayer.abyssBreathCD >= tick)
					{
						modPlayer.abyssBreathCD = 0;

						if (player.breath > 0)
							player.breath -= breathLoss;

						if (modPlayer.cDepth)
						{
							if (player.breath > 0)
								player.breath--;
						}

						if (player.breath <= 0)
						{
							lifeLossAtZeroBreath -= lifeLossAtZeroBreathResist;

							if (lifeLossAtZeroBreath < 0)
								lifeLossAtZeroBreath = 0;

							player.statLife -= lifeLossAtZeroBreath;

							if (player.statLife <= 0)
							{
								modPlayer.abyssDeath = true;
								modPlayer.KillPlayer();
							}
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

				if ((double)Math.Abs(player.velocity.X) < 0.1 && (double)Math.Abs(player.velocity.Y) < 0.1 && !player.mount.Active)
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
				if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) && player.wet && !player.lavaWet && !player.honeyWet &&
					!player.mount.Active)
				{
					if (modPlayer.aquaticBoost > 0f)
					{
						modPlayer.aquaticBoost -= 0.0002f; // 0.015
						if ((double)modPlayer.aquaticBoost <= 0.0)
						{
							modPlayer.aquaticBoost = 0f;
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendData(84, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
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

				if ((double)Math.Abs(player.velocity.X) < 0.1 && (double)Math.Abs(player.velocity.Y) < 0.1 && !player.mount.Active)
				{
					if (modPlayer.modStealthTimer == 0 && modPlayer.modStealth > 0f)
					{
						modPlayer.modStealth -= 0.015f;
						if ((double)modPlayer.modStealth <= 0.0)
						{
							modPlayer.modStealth = 0f;
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendData(84, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
						}
					}
				}
				else
				{
					float num27 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
					modPlayer.modStealth += num27 * 0.0075f;
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

				if ((double)Math.Abs(player.velocity.X) < 0.1 && (double)Math.Abs(player.velocity.Y) < 0.1 && !player.mount.Active)
				{
					if (modPlayer.modStealthTimer == 0 && modPlayer.modStealth > 0f)
					{
						modPlayer.modStealth -= 0.015f;
						if ((double)modPlayer.modStealth <= 0.0)
						{
							modPlayer.modStealth = 0f;
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendData(84, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
						}
					}
				}
				else
				{
					float num27 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
					modPlayer.modStealth += num27 * 0.0075f;
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
		}
		#endregion

		#region Elysian Aegis Effects
		private static void ElysianAegisEffects(Player player, CalamityPlayer modPlayer)
		{
			if (modPlayer.elysianAegis)
			{
				bool flag14 = false;

				// Activate buff
				if (modPlayer.elysianGuard)
				{
					if (player.whoAmI == Main.myPlayer)
						player.AddBuff(ModContent.BuffType<ElysianGuard>(), 2, false);

					float num29 = modPlayer.shieldInvinc;
					modPlayer.shieldInvinc -= 0.08f;
					if (modPlayer.shieldInvinc < 0f)
						modPlayer.shieldInvinc = 0f;
					else
						flag14 = true;

					if (modPlayer.shieldInvinc == 0f && num29 != modPlayer.shieldInvinc && Main.netMode == NetmodeID.MultiplayerClient)
						NetMessage.SendData(84, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);

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
					float num30 = modPlayer.shieldInvinc;
					modPlayer.shieldInvinc += 0.08f;
					if (modPlayer.shieldInvinc > 5f)
						modPlayer.shieldInvinc = 5f;
					else
						flag14 = true;

					if (modPlayer.shieldInvinc == 5f && num30 != modPlayer.shieldInvinc && Main.netMode == NetmodeID.MultiplayerClient)
						NetMessage.SendData(84, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
				}

				// Emit dust
				if (flag14)
				{
					if (Main.rand.NextBool(2))
					{
						Vector2 vector = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
						Dust dust = Main.dust[Dust.NewDust(player.Center - vector * 30f, 0, 0, 244, 0f, 0f, 0, default, 1f)];
						dust.noGravity = true;
						dust.position = player.Center - vector * (float)Main.rand.Next(5, 11);
						dust.velocity = vector.RotatedBy(1.5707963705062866, default) * 4f;
						dust.scale = 0.5f + Main.rand.NextFloat();
						dust.fadeIn = 0.5f;
					}

					if (Main.rand.NextBool(2))
					{
						Vector2 vector2 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
						Dust dust2 = Main.dust[Dust.NewDust(player.Center - vector2 * 30f, 0, 0, 246, 0f, 0f, 0, default, 1f)];
						dust2.noGravity = true;
						dust2.position = player.Center - vector2 * 12f;
						dust2.velocity = vector2.RotatedBy(-1.5707963705062866, default) * 2f;
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
				float x = (float)(Main.maxTilesX / 4200);
				x *= x;
				float spaceGravityMult = (float)((double)(player.position.Y / 16f - (60f + 10f * x)) / (Main.worldSurface / 6.0));
				if (spaceGravityMult < 1f)
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

			if (modPlayer.irradiated)
			{
				player.statDefense -= 10;
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
				player.allDamage += 0.1f;

			if (modPlayer.xWrath)
				modPlayer.AllCritBoost(5);

			if (modPlayer.godSlayerCooldown)
				player.allDamage += 0.1f;

			if (modPlayer.graxDefense)
			{
				player.statDefense += 30;
				player.endurance += 0.1f;
				player.meleeDamage += 0.2f;
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
			}

			if (modPlayer.darkSunRing)
			{
				player.maxMinions += 2;
				player.allDamage += 0.12f;
				player.minionKB += 1.2f;
				player.pickSpeed -= 0.15f;
				if (!Main.dayTime)
					player.statDefense += 30;
			}

			if (modPlayer.eGauntlet)
			{
				player.longInvince = true;
				player.kbGlove = true;
				player.meleeDamage += 0.15f;
				player.meleeCrit += 5;
				player.lavaMax += 240;
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
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					for (int m = 0; m < 200; m++)
					{
						if (Main.npc[m].active && !Main.npc[m].friendly)
						{
							float distance = (Main.npc[m].Center - player.Center).Length();
							if (distance < 120f)
								Main.npc[m].AddBuff(ModContent.BuffType<PearlAura>(), 20, false);
						}
					}
				}
			}

			if (CalamityMod.scopedWeaponList.Contains(player.inventory[player.selectedItem].type))
				player.scope = true;

			if (CalamityMod.highTestFishList.Contains(player.inventory[player.selectedItem].type))
				player.accFishingLine = true;

			if (CalamityMod.boomerangList.Contains(player.inventory[player.selectedItem].type) && player.invis)
				modPlayer.throwingDamage += 0.1f;

			if (CalamityMod.javelinList.Contains(player.inventory[player.selectedItem].type) && player.invis)
				player.armorPenetration += 5;

			if (CalamityMod.flaskBombList.Contains(player.inventory[player.selectedItem].type) && player.invis)
				modPlayer.throwingVelocity += 0.1f;

			if (CalamityMod.spikyBallList.Contains(player.inventory[player.selectedItem].type) && player.invis)
				modPlayer.throwingCrit += 10;

			if (modPlayer.etherealExtorter)
			{
				bool ZoneForest = !modPlayer.ZoneAbyss && !modPlayer.ZoneSulphur && !modPlayer.ZoneAstral && !modPlayer.ZoneCalamity &&
					!modPlayer.ZoneSunkenSea && !player.ZoneSnow && !player.ZoneCorrupt && !player.ZoneCrimson && !player.ZoneHoly &&
					!player.ZoneDesert && !player.ZoneUndergroundDesert && !player.ZoneGlowshroom && !player.ZoneDungeon && !player.ZoneBeach && !player.ZoneMeteor;

				if (player.ZoneUnderworldHeight && !modPlayer.ZoneCalamity && CalamityMod.fireWeaponList.Contains(player.inventory[player.selectedItem].type))
					player.endurance += 0.03f;

				if ((player.ZoneDesert || player.ZoneUndergroundDesert) && CalamityMod.daggerList.Contains(player.inventory[player.selectedItem].type))
					player.scope = true;

				if (modPlayer.ZoneSunkenSea)
				{
					player.gills = true;
					player.ignoreWater = true;
				}

				if (player.ZoneSnow && CalamityMod.iceWeaponList.Contains(player.inventory[player.selectedItem].type))
					player.statDefense += 5;

				if (modPlayer.ZoneAstral)
				{
					if (player.wingTimeMax > 0)
						player.wingTimeMax = (int)((double)player.wingTimeMax * 1.05);
				}

				if (player.ZoneJungle && CalamityMod.natureWeaponList.Contains(player.inventory[player.selectedItem].type))
					player.AddBuff(165, 5, true); // Dryad's Blessing

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

				if (player.ZoneRockLayerHeight && ZoneForest && CalamityMod.flaskBombList.Contains(player.inventory[player.selectedItem].type))
					player.blackBelt = true;

				if (player.ZoneHoly)
				{
					player.maxMinions += 1;
					player.manaCost *= 0.9f;
					player.ammoCost75 = true; // 25% chance to not use ranged ammo
					modPlayer.throwingAmmoCost75 = true; // 25% chance to not consume rogue consumables
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
					player.wingTimeMax = (int)((double)player.wingTimeMax * 1.25);
				player.moveSpeed += 0.2f;
			}

			if (modPlayer.blueCandle)
			{
				if (player.wingTimeMax > 0)
					player.wingTimeMax = (int)((double)player.wingTimeMax * 1.1);
				player.moveSpeed += 0.15f;
			}

			if (modPlayer.plaguebringerGoliathLore)
			{
				if (player.wingTimeMax > 0)
					player.wingTimeMax = (int)((double)player.wingTimeMax * 1.25);
			}

			if (modPlayer.soaring)
			{
				if (player.wingTimeMax > 0)
					player.wingTimeMax = (int)((double)player.wingTimeMax * 1.1);
			}

			if (modPlayer.plagueReaper)
			{
				if (player.wingTimeMax > 0)
					player.wingTimeMax = (int)((double)player.wingTimeMax * 1.05);
			}

			if (modPlayer.draconicSurge)
			{
				if (player.wingTimeMax > 0)
					player.wingTimeMax = (int)((double)player.wingTimeMax * 1.35);
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
				player.discount = true;
				player.lifeMagnet = true;
				player.calmed = true;
				player.loveStruck = true;
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
					player.wingTimeMax = (int)((double)player.wingTimeMax * 1.15);
			}

			if (modPlayer.ravagerLore)
			{
				if (player.wingTimeMax > 0)
					player.wingTimeMax = (int)((double)player.wingTimeMax * 0.5);
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
				player.statDefense -= GodSlayerInferno.DefenseReduction;

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

			if (modPlayer.horror)
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

			if (modPlayer.thirdSageH && !player.dead && player.HasBuff(ModContent.BuffType<ThirdSageBuff>()))
				player.statLife = player.statLifeMax2;

			if (modPlayer.pinkCandle)
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
				if (player.statMana < (int)((double)player.statManaMax2 * 0.1))
					player.ghostHeal = true;
			}

			if (modPlayer.rBrain)
			{
				if (player.statLife <= (int)((double)player.statLifeMax2 * 0.75))
					player.allDamage += 0.1f;
				if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
					player.moveSpeed -= 0.05f;
			}

			if (modPlayer.bloodyWormTooth)
			{
				if (player.statLife < (int)((double)player.statLifeMax2 * 0.5))
				{
					player.meleeDamage += 0.1f;
					player.meleeSpeed += 0.1f;
					player.endurance += 0.1f;
				}
				else
				{
					player.meleeDamage += 0.05f;
					player.meleeSpeed += 0.05f;
					player.endurance += 0.05f;
				}
			}

			if (modPlayer.dAmulet)
			{
				player.panic = true;
				player.pStone = true;
				player.armorPenetration += 10;
			}

			if (modPlayer.rampartOfDeities)
			{
				player.armorPenetration += 10;
				player.noKnockback = true;
				if (player.statLife > (int)((double)player.statLifeMax2 * 0.25))
				{
					player.hasPaladinShield = true;
					if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
					{
						int myPlayer = Main.myPlayer;
						if (Main.player[myPlayer].team == player.team && player.team != 0)
						{
							float arg = player.position.X - Main.player[myPlayer].position.X;
							float num3 = player.position.Y - Main.player[myPlayer].position.Y;

							if ((float)Math.Sqrt((double)(arg * arg + num3 * num3)) < 800f)
								Main.player[myPlayer].AddBuff(43, 20, true);
						}
					}
				}

				if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
					player.AddBuff(62, 5, true);
				if (player.statLife <= (int)((double)player.statLifeMax2 * 0.15))
					player.endurance += 0.05f;
			}
			else if (modPlayer.fBulwark)
			{
				player.noKnockback = true;
				if (player.statLife > (int)((double)player.statLifeMax2 * 0.25))
				{
					player.hasPaladinShield = true;
					if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
					{
						int myPlayer = Main.myPlayer;
						if (Main.player[myPlayer].team == player.team && player.team != 0)
						{
							float arg = player.position.X - Main.player[myPlayer].position.X;
							float num3 = player.position.Y - Main.player[myPlayer].position.Y;

							if ((float)Math.Sqrt((double)(arg * arg + num3 * num3)) < 800f)
								Main.player[myPlayer].AddBuff(43, 20, true);
						}
					}
				}

				if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
					player.AddBuff(62, 5, true);
				if (player.statLife <= (int)((double)player.statLifeMax2 * 0.15))
					player.endurance += 0.05f;
			}

			if (modPlayer.frostFlare)
			{
				player.resistCold = true;
				player.buffImmune[44] = true;
				player.buffImmune[46] = true;
				player.buffImmune[47] = true;

				if (player.statLife > (int)((double)player.statLifeMax2 * 0.75))
					player.allDamage += 0.1f;
				if (player.statLife < (int)((double)player.statLifeMax2 * 0.25))
					player.statDefense += 10;
			}

			if (modPlayer.vexation)
			{
				if (player.statLife < (int)((double)player.statLifeMax2 * 0.5))
					player.allDamage += 0.15f;
			}

			if (modPlayer.ataxiaBlaze)
			{
				if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
					player.AddBuff(BuffID.Inferno, 2);
			}

			if (modPlayer.bloodflareThrowing)
			{
				if (player.statLife > (int)((double)player.statLifeMax2 * 0.8))
				{
					modPlayer.throwingCrit += 5;
					player.statDefense += 30;
				}
				else
					modPlayer.throwingDamage += 0.1f;
			}

			if (modPlayer.bloodflareSummon)
			{
				if (player.statLife >= (int)((double)player.statLifeMax2 * 0.9))
					player.minionDamage += 0.1f;
				else if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
					player.statDefense += 20;

				if (modPlayer.bloodflareSummonTimer > 0)
					modPlayer.bloodflareSummonTimer--;

				if (player.whoAmI == Main.myPlayer && modPlayer.bloodflareSummonTimer <= 0)
				{
					modPlayer.bloodflareSummonTimer = 900;
					for (int I = 0; I < 3; I++)
					{
						float ai1 = (float)(I * 120);
						Projectile.NewProjectile(player.Center.X + (float)(Math.Sin(I * 120) * 550), player.Center.Y + (float)(Math.Cos(I * 120) * 550), 0f, 0f,
							ModContent.ProjectileType<GhostlyMine>(), (int)((modPlayer.auricSet ? 15000f : 5000f) * player.minionDamage), 1f, player.whoAmI, ai1, 0f);
					}
				}
			}

			if (modPlayer.yInsignia)
			{
				player.longInvince = true;
				player.kbGlove = true;
				player.meleeDamage += 0.05f;
				player.lavaMax += 240;
				if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
					player.allDamage += 0.1f;
			}

			if (modPlayer.reaperToothNecklace)
			{
				player.allDamage += 0.25f;
				player.statDefense /= 2;
			}

			if (modPlayer.deepDiver)
			{
				player.allDamage += 0.15f;
				player.statDefense += (int)((double)player.statDefense * 0.15);
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

				if (player.statLife <= (int)((double)player.statLifeMax2 * 0.15))
				{
					player.endurance += 0.1f;
					player.allDamage += 0.2f;
				}
				else if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
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

			if (modPlayer.tarraSummon)
			{
				int lifeCounter = 0;
				float num2 = 300f;
				bool flag = lifeCounter % 60 == 0;
				int num3 = 200;

				if (player.whoAmI == Main.myPlayer)
				{
					for (int l = 0; l < 200; l++)
					{
						NPC nPC = Main.npc[l];
						if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && Vector2.Distance(player.Center, nPC.Center) <= num2)
						{
							if (flag)
							{
								nPC.StrikeNPC(num3, 0f, 0, false, false, false);
								if (Main.netMode != NetmodeID.SinglePlayer)
									NetMessage.SendData(28, -1, -1, null, l, (float)num3, 0f, 0f, 0, 0, 0);
							}
						}
					}
				}

				if (lifeCounter >= 180)
					lifeCounter = 0;
				lifeCounter++;
			}

			if (player.inventory[player.selectedItem].type == ModContent.ItemType<NavyFishingRod>() && player.ownedProjectileCounts[ModContent.ProjectileType<NavyBobber>()] != 0)
			{
				int auraCounter = 0;
				float num2 = 200f;
				bool flag = auraCounter % 120 == 0;
				int num3 = 10;

				if (player.whoAmI == Main.myPlayer)
				{
					for (int l = 0; l < 200; l++)
					{
						NPC nPC = Main.npc[l];
						if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && Vector2.Distance(player.Center, nPC.Center) <= num2)
						{
							if (flag)
							{
								nPC.StrikeNPC(num3, 0f, 0, false, false, false);
								if (Main.netMode != NetmodeID.SinglePlayer)
									NetMessage.SendData(28, -1, -1, null, l, (float)num3, 0f, 0f, 0, 0, 0);

								Vector2 value15 = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
								while (value15.X == 0f && value15.Y == 0f)
									value15 = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));

								value15.Normalize();
								value15 *= (float)Main.rand.Next(30, 61) * 0.1f;
								int num17 = Projectile.NewProjectile(nPC.Center.X, nPC.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<EutrophicSpark>(), 5, 0f, player.whoAmI, 0f, 0f);
								Main.projectile[num17].melee = false;
								Main.projectile[num17].localNPCHitCooldown = -1;
							}
						}
					}
				}

				if (auraCounter >= 360)
					auraCounter = 0;
				auraCounter++;
			}

			if (modPlayer.brimstoneElementalLore && player.inferno)
			{
				int num = ModContent.BuffType<BrimstoneFlames>();
				float num2 = 300f;
				bool flag = player.infernoCounter % 30 == 0;
				int damage = 50;

				if (player.whoAmI == Main.myPlayer)
				{
					for (int l = 0; l < 200; l++)
					{
						NPC nPC = Main.npc[l];
						if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && Vector2.Distance(player.Center, nPC.Center) <= num2)
						{
							if (nPC.FindBuffIndex(num) == -1 && !nPC.buffImmune[num])
								nPC.AddBuff(num, 120, false);
							if (flag)
								player.ApplyDamageToNPC(nPC, damage, 0f, 0, false);
						}
					}
				}
			}

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
				// LATER -- When Wulfrum Slimes start being definitely robots, remove this immunity.
				player.npcTypeNoAggro[ModContent.NPCType<WulfrumSlime>()] = true;
			}

			/*if (modPlayer.dukeScales)
            {
				player.buffImmune[ModContent.BuffType<SulphuricPoisoning>()] = true;
				player.buffImmune[BuffID.Poisoned] = true;
				player.buffImmune[BuffID.Venom] = true;
                if (player.statLife <= (int)((double)player.statLifeMax2 * 0.75))
                {
                    player.allDamage += 0.03f;
					modPlayer.AllCritBoost(3);
                }
                if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
                {
                    player.allDamage += 0.05f;
					modPlayer.AllCritBoost(5);
                }
				if (player.lifeRegen < 0)
                {
                    player.allDamage += 0.1f;
					modPlayer.AllCritBoost(5);
                }
            }*/

			if (modPlayer.auricSet && modPlayer.silvaMelee)
			{
				double multiplier = (double)player.statLife / (double)player.statLifeMax2;
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
						Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<SonOfYharon>(), (int)(232f * player.minionDamage), 2f, Main.myPlayer, 0f, 0f);
				}
			}

			if (modPlayer.pArtifact)
			{
				if (player.whoAmI == Main.myPlayer)
				{
					if (player.FindBuffIndex(ModContent.BuffType<GuardianHealer>()) == -1)
						player.AddBuff(ModContent.BuffType<GuardianHealer>(), 3600, true);

					if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianHealer>()] < 1)
						Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -6f, ModContent.ProjectileType<MiniGuardianHealer>(), 0, 0f, Main.myPlayer, 0f, 0f);

					float baseDamage = 100f +
						(CalamityWorld.downedDoG ? 100f : 0f) +
						(CalamityWorld.downedYharon ? 100f : 0f);

					if (player.maxMinions >= 8)
					{
						if (player.FindBuffIndex(ModContent.BuffType<GuardianDefense>()) == -1)
							player.AddBuff(ModContent.BuffType<GuardianDefense>(), 3600, true);

						if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianDefense>()] < 1)
							Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -3f, ModContent.ProjectileType<MiniGuardianDefense>(), (int)(baseDamage * player.minionDamage), 1f, Main.myPlayer, 0f, 0f);
					}

					if (modPlayer.tarraSummon || modPlayer.bloodflareSummon || modPlayer.godSlayerSummon || modPlayer.silvaSummon || modPlayer.dsSetBonus || modPlayer.omegaBlueSet || modPlayer.fearmongerSet)
					{
						if (player.FindBuffIndex(ModContent.BuffType<GuardianOffense>()) == -1)
							player.AddBuff(ModContent.BuffType<GuardianOffense>(), 3600, true);

						if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianAttack>()] < 1)
							Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<MiniGuardianAttack>(), (int)(baseDamage * player.minionDamage), 1f, Main.myPlayer, 0f, 0f);
					}
				}
			}

			if (player.ownedProjectileCounts[ModContent.ProjectileType<BrimstoneElementalMinion>()] > 1 || player.ownedProjectileCounts[ModContent.ProjectileType<WaterElementalMinion>()] > 1 ||
				player.ownedProjectileCounts[ModContent.ProjectileType<SandElementalHealer>()] > 1 || player.ownedProjectileCounts[ModContent.ProjectileType<SandElementalMinion>()] > 1 ||
				player.ownedProjectileCounts[ModContent.ProjectileType<CloudElementalMinion>()] > 1 || player.ownedProjectileCounts[ModContent.ProjectileType<FungalClumpMinion>()] > 1)
			{
				for (int projIndex = 0; projIndex < 1000; projIndex++)
				{
					if (Main.projectile[projIndex].active && Main.projectile[projIndex].owner == player.whoAmI)
					{
						if (Main.projectile[projIndex].type == ModContent.ProjectileType<BrimstoneElementalMinion>() || Main.projectile[projIndex].type == ModContent.ProjectileType<WaterElementalMinion>() ||
							Main.projectile[projIndex].type == ModContent.ProjectileType<SandElementalHealer>() || Main.projectile[projIndex].type == ModContent.ProjectileType<SandElementalMinion>() ||
							Main.projectile[projIndex].type == ModContent.ProjectileType<CloudElementalMinion>() || Main.projectile[projIndex].type == ModContent.ProjectileType<FungalClumpMinion>())
						{
							Main.projectile[projIndex].Kill();
						}
					}
				}
			}

			if (modPlayer.tesla)
			{
				if (player.whoAmI == Main.myPlayer)
				{
					if (player.ownedProjectileCounts[ModContent.ProjectileType<TeslaAura>()] < 1)
						Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<TeslaAura>(), 25, 0f, Main.myPlayer, 0f, 0f);
				}
			}
			else if (player.ownedProjectileCounts[ModContent.ProjectileType<TeslaAura>()] != 0)
            {
				if (player.whoAmI == Main.myPlayer)
				{
					for (int i = 0; i < 1000; i++)
					{
						if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<TeslaAura>() && Main.projectile[i].owner == player.whoAmI)
						{
							Main.projectile[i].Kill();
							break;
						}
					}
				}
			}

			if (Config.ProficiencyEnabled)
				modPlayer.GetStatBonuses();
		}
		#endregion

		#region Limits
		private static void Limits(Player player, CalamityPlayer modPlayer)
		{
			if (player.meleeSpeed < 0.5f)
				player.meleeSpeed = 0.5f;

			// 10% is converted to 9%, 25% is converted to 20%, 50% is converted to 33%, 75% is converted to 43%, 100% is converted to 50%
			player.endurance = 1f - (1f / (1f + player.endurance));

			if (modPlayer.yharonLore && !CalamityWorld.defiled)
			{
				if (player.wingTimeMax < 50000)
					player.wingTimeMax = 50000;
			}
		}
		#endregion

		#region Endurance Reductions
		private static void EnduranceReductions(Player player, CalamityPlayer modPlayer)
		{
			if (modPlayer.vHex)
				player.endurance -= 0.3f;
			if (modPlayer.irradiated)
				player.endurance -= 0.1f;
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
		private static void StatMeter(Player player, CalamityPlayer modPlayer)
		{
			float allDamageStat = player.allDamage - 1f;
			modPlayer.damageStats[0] = (int)((player.meleeDamage + allDamageStat - 1f) * 100f);
			modPlayer.damageStats[1] = (int)((player.rangedDamage + allDamageStat - 1f) * 100f);
			modPlayer.damageStats[2] = (int)((player.magicDamage + allDamageStat - 1f) * 100f);
			modPlayer.damageStats[3] = (int)((player.minionDamage + allDamageStat - 1f) * 100f);
			modPlayer.damageStats[4] = (int)((modPlayer.throwingDamage + allDamageStat - 1f) * 100f);
			modPlayer.critStats[0] = player.meleeCrit;
			modPlayer.critStats[1] = player.rangedCrit;
			modPlayer.critStats[2] = player.magicCrit;
			modPlayer.critStats[3] = modPlayer.throwingCrit;
			modPlayer.defenseStat = player.statDefense;
			modPlayer.DRStat = (int)(player.endurance * 100f);
			modPlayer.meleeSpeedStat = (int)((1f - player.meleeSpeed) * (100f / player.meleeSpeed));
			modPlayer.manaCostStat = (int)(player.manaCost * 100f);
			modPlayer.rogueVelocityStat = (int)((modPlayer.throwingVelocity - 1f) * 100f);
			modPlayer.minionSlotStat = player.maxMinions;
			modPlayer.manaRegenStat = player.manaRegen;
			modPlayer.armorPenetrationStat = player.armorPenetration;
			modPlayer.moveSpeedStat = (int)((player.moveSpeed - 1f) * 100f);
			modPlayer.wingFlightTimeStat = player.wingTimeMax;
			modPlayer.adrenalineChargeStat = 45 -
				(modPlayer.adrenalineBoostOne ? 10 : 0) -
				(modPlayer.adrenalineBoostTwo ? 10 : 0) -
				(modPlayer.adrenalineBoostThree ? 5 : 0);
			bool DHorHoD = modPlayer.draedonsHeart || modPlayer.heartOfDarkness;
			int rageDamageBoost = 0 +
				(modPlayer.rageBoostOne ? (CalamityWorld.death ? 50 : 15) : 0) +
				(modPlayer.rageBoostTwo ? (CalamityWorld.death ? 50 : 15) : 0) +
				(modPlayer.rageBoostThree ? (CalamityWorld.death ? 50 : 15) : 0);
			modPlayer.rageDamageStat = (CalamityWorld.death ? (DHorHoD ? 200 : 170) : (DHorHoD ? 65 : 50)) + rageDamageBoost; // Death Mode values: 2.3 and 2.0, rev: 0.65 and 0.5
		}
		#endregion

		#region Rogue Mirrors
		private static void RogueMirrors(Player player, CalamityPlayer modPlayer)
		{
			Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && !Main.npc[i].townNPC && Main.npc[i].immune[player.whoAmI] <= 0 && Main.npc[i].damage > 0)
				{
					NPC nPC = Main.npc[i];
					Rectangle rect = nPC.getRect();
					if (rectangle.Intersects(rect) && (nPC.noTileCollide || player.CanHit(nPC)))
					{
						if (Main.rand.Next(10) == 0 && player.immuneTime <= 0)
						{
							modPlayer.AbyssMirrorEvade();
							modPlayer.EclipseMirrorEvade();
						}
						break;
					}
				}
			}

			for (int i = 0; i < 1000; i++)
			{
				if (Main.projectile[i].active && !Main.projectile[i].friendly && Main.projectile[i].hostile && Main.projectile[i].damage > 0)
				{
					Projectile proj = Main.projectile[i];
					Rectangle rect = proj.getRect();
					if (rectangle.Intersects(rect))
					{
						if (Main.rand.Next(10) == 0)
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
	}
}
