using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.Cooldowns;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Summon;
using CalamityMod.CustomRecipes;
using CalamityMod.DataStructures;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Mounts.Minecarts;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Other;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Environment;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.UI;
using CalamityMod.UI.CooldownIndicators;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using ProvidenceBoss = CalamityMod.NPCs.Providence.Providence;

namespace CalamityMod.CalPlayer
{
	public partial class CalamityPlayer : ModPlayer
	{
		#region Post Update Misc Effects
        public override void PostUpdateMiscEffects()
		{
			// No category

			// Give the player a 24% jump speed boost while wings are equipped
			if (player.wingsLogic > 0)
				player.jumpSpeedBoost += 1.2f;

			// Decrease the counter on Fearmonger set turbo regeneration
			if (fearmongerRegenFrames > 0)
				fearmongerRegenFrames--;

			// Reduce the expert debuff time multiplier to the normal mode multiplier
			if (CalamityConfig.Instance.NerfExpertDebuffs)
				Main.expertDebuffTime = 1f;

			// Bool for any existing bosses, true if any boss NPC is active
			areThereAnyDamnBosses = CalamityUtils.AnyBossNPCS();

			// Bool for any existing events, true if any event is active
			areThereAnyDamnEvents = CalamityGlobalNPC.AnyEvents(player);

			// Hurt the nearest NPC to the mouse if using the burning mouse.
			if (blazingCursorDamage)
				HandleBlazingMouseEffects();

			// Revengeance effects
			RevengeanceModeMiscEffects();

			// Abyss effects
			AbyssEffects();

			// Misc effects, because I don't know what else to call it
			MiscEffects();

			// Max life and mana effects
			MaxLifeAndManaEffects();

			// Standing still effects
			StandingStillEffects();

			// Elysian Aegis effects
			ElysianAegisEffects();

			// Other buff effects
			OtherBuffEffects();

			// Defense manipulation (Mostly defense damage, but also Bloodflare Core and others)
			DefenseEffects();

			// Limits
			Limits();

			// Stat Meter
			UpdateStatMeter();

			// Double Jumps
			DoubleJumps();

			// Potions (Quick Buff && Potion Sickness)
			HandlePotions();

			// Check if schematics are present on the mouse, for the sake of registering their recipes.
			CheckIfMouseItemIsSchematic();

			// Update all particle sets for items.
			// This must be done here instead of in the item logic because these sets are not properly instanced
			// in the global classes. Attempting to update them there will cause multiple updates to one set for multiple items.
			CalamityGlobalItem.UpdateAllParticleSets();
			BiomeBlade.UpdateAllParticleSets();
			TrueBiomeBlade.UpdateAllParticleSets();
			OmegaBiomeBlade.UpdateAllParticleSets();

			// Update the gem tech armor set.
			GemTechState.Update();

			// Regularly sync player stats & mouse control info during multiplayer
			if (player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
			{
				packetTimer++;
				if (packetTimer == GlobalSyncPacketTimer)
				{
					packetTimer = 0;
					StandardSync();
				}

				if (syncMouseControls)
				{
					syncMouseControls = false;
					MouseControlsSync();
				}
			}


			// After everything else, if Daawnlight Spirit Origin is equipped, set ranged crit to the base 4%.
			// Store all the crit so it can be used in damage calculations.
			if (spiritOrigin)
			{
				// player.rangedCrit already contains the crit stat of the held item, no need to grab it separately.
				// Don't store the base 4% because you're not removing it.
				spiritOriginConvertedCrit = player.rangedCrit - 4;
				player.rangedCrit = 4;
			}

            if (player.ActiveItem().type == ModContent.ItemType<GaelsGreatsword>())
                heldGaelsLastFrame = true;

            // De-equipping Gael's Greatsword deletes all rage.
            else if (heldGaelsLastFrame)
            {
                heldGaelsLastFrame = false;
                rage = 0f;
            }
		}
		#endregion

		#region Revengeance Effects
		private void RevengeanceModeMiscEffects()
		{
			if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
			{
				// Adjusts the life steal cap in rev/death
				float lifeStealCap = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 30f : CalamityWorld.death ? 45f : 60f;
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
					int immuneTimeLimit = 150;
					if (player.immuneTime > immuneTimeLimit)
						player.immuneTime = immuneTimeLimit;

					for (int k = 0; k < player.hurtCooldowns.Length; k++)
					{
						if (player.hurtCooldowns[k] > immuneTimeLimit)
							player.hurtCooldowns[k] = immuneTimeLimit;
					}

					// Adrenaline and Rage
					if (CalamityWorld.revenge)
						UpdateRippers();
				}
			}

			// If Revengeance Mode is not active, then set rippers to zero
			else if (player.whoAmI == Main.myPlayer)
			{
				rage = 0;
				adrenaline = 0;
			}
		}

		private void UpdateRippers()
		{
			// Figure out Rage's current duration based on boosts.
			if (rageBoostOne)
				RageDuration += RageDurationPerBooster;
			if (rageBoostTwo)
				RageDuration += RageDurationPerBooster;
			if (rageBoostThree)
				RageDuration += RageDurationPerBooster;

			// Tick down "Rage Combat Frames". When they reach zero, Rage begins fading away.
			if (rageCombatFrames > 0)
				--rageCombatFrames;

			// Tick down the Rage gain cooldown.
			if (rageGainCooldown > 0)
				--rageGainCooldown;

			// This is how much Rage will be changed by this frame.
			float rageDiff = 0;

			// If the player equips multiple rage generation accessories they get the max possible effect without stacking any of them.
			{
				float rageGen = 0f;

				// Shattered Community provides constant rage generation (stronger than Heart of Darkness).
				if (shatteredCommunity)
				{
					float scRageGen = rageMax * ShatteredCommunity.RagePerSecond / 60f;
					if (rageGen < scRageGen)
						rageGen = scRageGen;
				}
				// Heart of Darkness grants constant rage generation.
				else if (heartOfDarkness)
				{
					float hodRageGen = rageMax * HeartofDarkness.RagePerSecond / 60f;
					if (rageGen < hodRageGen)
						rageGen = hodRageGen;
				}

				rageDiff += rageGen;
			}

			// Holding Gael's Greatsword grants constant rage generation.
			if (heldGaelsLastFrame)
				rageDiff += rageMax * GaelsGreatsword.RagePerSecond / 60f;

			// Calculate and grant proximity rage.
			// Regular enemies can give up to 1x proximity rage. Bosses can give up to 3x. Multiple regular enemies don't stack.
			// Proximity rage is maxed out when within 10 blocks (160 pixels) of the enemy's hitbox.
			// Its max range is 50 blocks (800 pixels), at which you get zero proximity rage.
			// Proximity rage does not generate while Rage Mode is active.
			if (!rageModeActive)
			{
				float bossProxRageMultiplier = 3f;
				float minProxRageDistance = 160f;
				float maxProxRageDistance = 800f;
				float enemyDistance = maxProxRageDistance + 1f;
				float bossDistance = maxProxRageDistance + 1f;

				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					NPC npc = Main.npc[i];
					if (npc is null || !npc.IsAnEnemy() || npc.Calamity().DoesNotGenerateRage)
						continue;

					// Take the longer of the two directions for the NPC's hitbox to be generous.
					float generousHitboxWidth = Math.Max(npc.Hitbox.Width / 2f, npc.Hitbox.Height / 2f);
					float hitboxEdgeDist = npc.Distance(player.Center) - generousHitboxWidth;

					// If this enemy is closer than the previous, reduce the current minimum proximity distance.
					if (enemyDistance > hitboxEdgeDist)
					{
						enemyDistance = hitboxEdgeDist;

						// If they're a boss, reduce the boss distance.
						// Boss distance will always be >= enemy distance, so there's no need to do another check.
						// Worm boss body and tail segments are not counted as bosses for this calculation.
						if (npc.IsABoss() && !CalamityLists.noRageWormSegmentList.Contains(npc.type))
							bossDistance = hitboxEdgeDist;
					}
				}

				// Helper function to implement proximity rage formula
				float ProxRageFromDistance(float dist)
				{
					// Adjusted distance with the 160 grace pixels added in. If you're closer than that it counts as zero.
					float d = Math.Max(dist - minProxRageDistance, 0f);

					// The first term is exponential decay which reduces rage gain significantly over distance.
					// The second term is a linear component which allows a baseline but weak rage generation even at far distances.
					// This function takes inputs from 0.0 to 640.0 and returns a value from 1.0 to 0.0.
					float r = 1f / (0.034f * d + 2f) + (590.5f - d) / 1181f;
					return MathHelper.Clamp(r, 0f, 1f);
				}

				// If anything is close enough then provide proximity rage.
				// You can only get proximity rage from one target at a time. You gain rage from whatever target would give you the most rage.
				if (enemyDistance <= maxProxRageDistance)
				{
					// If the player is close enough to get proximity rage they are also considered to have rage combat frames.
					// This prevents proximity rage from fading away unless you run away without attacking for some reason.
					rageCombatFrames = Math.Max(rageCombatFrames, 3);

					float proxRageFromEnemy = ProxRageFromDistance(enemyDistance);
					float proxRageFromBoss = 0f;
					if (bossDistance <= maxProxRageDistance)
						proxRageFromBoss = bossProxRageMultiplier * ProxRageFromDistance(bossDistance);

					float finalProxRage = Math.Max(proxRageFromEnemy, proxRageFromBoss);

					// 300% proximity rage (max possible from a boss) will fill the Rage meter in 15 seconds.
					// 100% proximity rage (max possible from an enemy) will fill the Rage meter in 45 seconds.
					rageDiff += finalProxRage * rageMax / CalamityUtils.SecondsToFrames(45f);
				}
			}

			bool rageFading = rageCombatFrames <= 0 && !heartOfDarkness && !shatteredCommunity;

			// If Rage Mode is currently active, you smoothly lose all rage over the duration.
			if (rageModeActive)
				rageDiff -= rageMax / RageDuration;

			// If out of combat and NOT using Heart of Darkness or Shattered Community, Rage fades away.
			else if (!rageModeActive && rageFading)
				rageDiff -= rageMax / RageFadeTime;

			// Apply the rage change and cap rage in both directions.
			rage += rageDiff;
			if (rage < 0)
				rage = 0;

			if (rage >= rageMax)
			{
				// If Rage is not active, it is capped at 100%.
				if (!rageModeActive)
					rage = rageMax;

				// If using the Shattered Community, Rage is capped at 200% while it's active.
				// This prevents infinitely stacking rage before a fight by standing on spikes/lava with a regen build or the Nurse handy.
				else if (shatteredCommunity && rage >= 2f * rageMax)
					rage = 2f * rageMax;

				// Play a sound when the Rage Meter is full
				if (playFullRageSound)
				{
					playFullRageSound = false;
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/FullRage"), (int)player.position.X, (int)player.position.Y);
				}
			}
			else
				playFullRageSound = true;

			// This is how much Adrenaline will be changed by this frame.
			float adrenalineDiff = 0;
			bool SCalAlive = NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>());
			bool wofAndNotHell = Main.wof >= 0 && player.position.Y < (float)((Main.maxTilesY - 200) * 16);

			// If Adrenaline Mode is currently active, you smoothly lose all adrenaline over the duration.
			if (adrenalineModeActive)
				adrenalineDiff = -adrenalineMax / AdrenalineDuration;
			else
			{
				// If any boss is alive (or you are between DoG phases or Boss Rush is active), you gain adrenaline smoothly.
				// EXCEPTION: Wall of Flesh is alive and you are not in hell. Then you don't get anything.
				if ((areThereAnyDamnBosses || CalamityWorld.DoGSecondStageCountdown > 0 || BossRushEvent.BossRushActive) &&
					!wofAndNotHell)
				{
					adrenalineDiff += adrenalineMax / AdrenalineChargeTime;
				}

				// If you aren't actively in a boss fight, adrenaline rapidly fades away.
				else
					adrenalineDiff = -adrenalineMax / AdrenalineFadeTime;
			}

			// In the SCal fight, adrenaline charges 33% slower (meaning it takes 50% longer to fully charge it).
			if (SCalAlive && adrenalineDiff > 0f)
				adrenalineDiff *= 0.67f;

			// Apply the adrenaline change and cap adrenaline in both directions.
			adrenaline += adrenalineDiff;
			if (adrenaline < 0)
				adrenaline = 0;

			if (adrenaline >= adrenalineMax)
			{
				adrenaline = adrenalineMax;

				// Play a sound when the Adrenaline Meter is full
				if (playFullAdrenalineSound)
				{
					playFullAdrenalineSound = false;
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/FullAdrenaline"), (int)player.position.X, (int)player.position.Y);
				}
			}
			else
				playFullAdrenalineSound = true;
		}
		#endregion

		#region Misc Effects

		private void HandleBlazingMouseEffects()
		{
			// The sigil's brightness slowly fades away every frame if not incinerating anything.
			blazingMouseAuraFade = MathHelper.Clamp(blazingMouseAuraFade - 0.025f, 0.25f, 1f);

			// miscCounter is used to limit Calamity's hit rate.
			int framesPerHit = 60 / Calamity.HitsPerSecond;
			if (player.miscCounter % framesPerHit != 1)
				return;

			Rectangle sigilHitbox = Utils.CenteredRectangle(Main.MouseWorld, new Vector2(35f, 62f));
			int sigilDamage = (int)(player.AverageDamage() * Calamity.BaseDamage);
			bool brightenedSigil = false;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC target = Main.npc[i];
				if (!target.active || !target.Hitbox.Intersects(sigilHitbox) || target.immortal || target.dontTakeDamage || target.townNPC)
					continue;

				// Brighten the sigil because it is dealing damage. This can only happen once per hit event.
				if (!brightenedSigil)
				{
					blazingMouseAuraFade = MathHelper.Clamp(blazingMouseAuraFade + 0.2f, 0.25f, 1f);
					brightenedSigil = true;
				}

				// Create a direct strike to hit this specific NPC.
				Projectile.NewProjectileDirect(target.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), sigilDamage, 0f, player.whoAmI, i);

				// Incinerate the target with Vulnerability Hex.
				target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), VulnerabilityHex.CalamityDuration);

				// Make some fancy dust to indicate damage is being done.
				for (int j = 0; j < 12; j++)
				{
					Dust fire = Dust.NewDustDirect(target.position, target.width, target.height, 267);
					fire.velocity = Vector2.UnitY * -Main.rand.NextFloat(2f, 3.45f);
					fire.scale = 1f + fire.velocity.Length() / 6f;
					fire.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0.85f));
					fire.noGravity = true;
				}
			}
		}

		private void MiscEffects()
		{
			// Do a vanity/social slot check for SCal's expert drop since alternatives to get this working are a pain in the ass to create.
			int blazingCursorItem = ModContent.ItemType<Calamity>();
			for (int i = 13; i < 18 + player.extraAccessorySlots; i++)
			{
				if (player.armor[i].type == blazingCursorItem)
				{
					blazingCursorVisuals = true;
					break;
				}
			}

			// Calculate/reset DoG cart rotations based on whether the DoG cart is in use.
			if (player.mount.Active && player.mount.Type == ModContent.MountType<DoGCartMount>())
			{
				SmoothenedMinecartRotation = MathHelper.Lerp(SmoothenedMinecartRotation, DelegateMethods.Minecart.rotation, 0.05f);

				// Initialize segments from null if necessary.
				int direction = (player.velocity.SafeNormalize(Vector2.UnitX * player.direction).X > 0f).ToDirectionInt();
				if (player.velocity.X == 0f)
					direction = player.direction;

				float idealRotation = DoGCartMount.CalculateIdealWormRotation(player);
				float minecartRotation = DelegateMethods.Minecart.rotation;
				if (Math.Abs(minecartRotation) < 0.5f)
					minecartRotation = 0f;
				Vector2 stickOffset = minecartRotation.ToRotationVector2() * player.velocity.Length() * direction * 1.25f;
				for (int i = 0; i < DoGCartSegments.Length; i++)
				{
					if (DoGCartSegments[i] is null)
					{
                        DoGCartSegments[i] = new DoGCartSegment
                        {
                            Center = player.Center - idealRotation.ToRotationVector2() * i * 20f
                        };
                    }
				}

				Vector2 startingStickPosition = player.Center + stickOffset + new Vector2(direction * (float)Math.Cos(SmoothenedMinecartRotation * 2f) * -34f, 12f);
				DoGCartSegments[0].Update(player, startingStickPosition, idealRotation);
				DoGCartSegments[0].Center = startingStickPosition;

				for (int i = 1; i < DoGCartSegments.Length; i++)
				{
					Vector2 waveOffset = DoGCartMount.CalculateSegmentWaveOffset(i, player);
					DoGCartSegments[i].Update(player, DoGCartSegments[i - 1].Center + waveOffset, DoGCartSegments[i - 1].Rotation);
				}
			}
			else
				DoGCartSegments = new DoGCartSegment[DoGCartSegments.Length];

			// Dust on hand when holding the phosphorescent gauntlet.
			if (player.ActiveItem().type == ModContent.ItemType<PhosphorescentGauntlet>())
				PhosphorescentGauntletPunches.GenerateDustOnOwnerHand(player);

			if (stealthUIAlpha > 0f && (rogueStealth <= 0f || rogueStealthMax <= 0f))
			{
				stealthUIAlpha -= 0.035f;
				stealthUIAlpha = MathHelper.Clamp(stealthUIAlpha, 0f, 1f);
			}
			else if (stealthUIAlpha < 1f)
			{
				stealthUIAlpha += 0.035f;
				stealthUIAlpha = MathHelper.Clamp(stealthUIAlpha, 0f, 1f);
			}

			if (andromedaState == AndromedaPlayerState.LargeRobot ||
				player.ownedProjectileCounts[ModContent.ProjectileType<RelicOfDeliveranceSpear>()] > 0)
			{
				player.controlHook = player.releaseHook = false;
			}

			if (andromedaCripple > 0)
			{
				player.velocity = Vector2.Clamp(player.velocity, new Vector2(-11f, -8f), new Vector2(11f, 8f));
				andromedaCripple--;
			}

			if (player.ownedProjectileCounts[ModContent.ProjectileType<GiantIbanRobotOfDoom>()] <= 0 &&
				andromedaState != AndromedaPlayerState.Inactive)
			{
				andromedaState = AndromedaPlayerState.Inactive;
			}

			if (andromedaState == AndromedaPlayerState.LargeRobot)
			{
				player.width = 80;
				player.height = 212;
				player.position.Y -= 170;
				resetHeightandWidth = true;
			}
			else if (andromedaState == AndromedaPlayerState.SpecialAttack)
			{
				player.width = 24;
				player.height = 98;
				player.position.Y -= 56;
				resetHeightandWidth = true;
			}
			else if (!player.mount.Active && resetHeightandWidth)
			{
				player.width = 20;
				player.height = 42;
				resetHeightandWidth = false;
			}

			// Summon bullseyes on nearby targets.
			if (spiritOrigin)
            {
				int bullseyeType = ModContent.ProjectileType<SpiritOriginBullseye>();
				List<int> alreadyTargetedNPCs = new List<int>();
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					if (Main.projectile[i].type != bullseyeType || !Main.projectile[i].active || Main.projectile[i].owner != player.whoAmI)
						continue;

					alreadyTargetedNPCs.Add((int)Main.projectile[i].ai[0]);
				}

				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (!Main.npc[i].active || Main.npc[i].friendly || Main.npc[i].lifeMax < 5 || alreadyTargetedNPCs.Contains(i) || Main.npc[i].realLife >= 0 || Main.npc[i].dontTakeDamage || Main.npc[i].immortal)
						continue;

					if (Main.myPlayer == player.whoAmI && Main.npc[i].WithinRange(player.Center, 2000f))
						Projectile.NewProjectile(Main.npc[i].Center, Vector2.Zero, bullseyeType, 0, 0f, player.whoAmI, i);
					if (spiritOriginBullseyeShootCountdown <= 0)
						spiritOriginBullseyeShootCountdown = 45;
				}
			}

			// Proficiency level ups
			if (CalamityConfig.Instance.Proficiency)
				GetExactLevelUp();

			// Max mana bonuses
			player.statManaMax2 +=
				(permafrostsConcoction ? 50 : 0) +
				(pHeart ? 50 : 0) +
				(eCore ? 50 : 0) +
				(cShard ? 50 : 0) +
				(starBeamRye ? 50 : 0);

			// Life Steal nerf
			// Reduces Normal Mode life steal recovery rate from 0.6/s to 0.5/s
			// Reduces Expert Mode life steal recovery rate from 0.5/s to 0.35/s
			// Revengeance Mode recovery rate is 0.3/s
			// Death Mode recovery rate is 0.25/s
			// Malice Mode recovery rate is 0.2/s
			float lifeStealCooldown = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 0.3f : CalamityWorld.death ? 0.25f : CalamityWorld.revenge ? 0.2f : Main.expertMode ? 0.15f : 0.1f;
			player.lifeSteal -= lifeStealCooldown;

			// Nebula Armor nerf
			if (player.nebulaLevelMana > 0 && player.statMana < player.statManaMax2)
			{
				int num = 12;
				nebulaManaNerfCounter += player.nebulaLevelMana;
				if (nebulaManaNerfCounter >= num)
				{
					nebulaManaNerfCounter -= num;
					player.statMana--;
					if (player.statMana < 0)
						player.statMana = 0;
				}
			}
			else
				nebulaManaNerfCounter = 0;

			// Bool for drawing boss health bar small text or not
			if (Main.myPlayer == player.whoAmI)
				BossHealthBarManager.CanDrawExtraSmallText = shouldDrawSmallText;

			// Margarita halved debuff duration
			if (margarita)
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

			// Update the Providence Burn effect drawer if applicable.
			float providenceBurnIntensity = 0f;
			if (Main.npc.IndexInRange(CalamityGlobalNPC.holyBoss) && Main.npc[CalamityGlobalNPC.holyBoss].active)
				providenceBurnIntensity = (Main.npc[CalamityGlobalNPC.holyBoss].modNPC as ProvidenceBoss).CalculateBurnIntensity();
			ProvidenceBurnEffectDrawer.ParticleSpawnRate = int.MaxValue;

			// If the burn intensity is great enough, cause the player to ignite into flames.
			if (providenceBurnIntensity > 0.45f)
				ProvidenceBurnEffectDrawer.ParticleSpawnRate = 1;

			// Otherwise, if the intensity is too weak, but still presernt, cause the player to release holy cinders.
			else if (providenceBurnIntensity > 0f)
			{
				int cinderCount = (int)MathHelper.Lerp(1f, 4f, Utils.InverseLerp(0f, 0.45f, providenceBurnIntensity, true));
				for (int i = 0; i < cinderCount; i++)
				{
					if (!Main.rand.NextBool(3))
						continue;

					Dust holyCinder = Dust.NewDustDirect(player.position, player.width, player.head, (int)CalamityDusts.ProfanedFire);
					holyCinder.velocity = Main.rand.NextVector2Circular(3.5f, 3.5f);
					holyCinder.velocity.Y -= Main.rand.NextFloat(1f, 3f);
					holyCinder.scale = Main.rand.NextFloat(1.15f, 1.45f);
					holyCinder.noGravity = true;
				}
			}

			ProvidenceBurnEffectDrawer.Update();

			// Immunity to most debuffs
			if (invincible)
			{
				foreach (int debuff in CalamityLists.debuffList)
					player.buffImmune[debuff] = true;
			}

			// Transformer immunity to Electrified
			if (aSparkRare)
				player.buffImmune[BuffID.Electrified] = true;

			// Reduce breath meter while in icy water instead of chilling
			bool canBreath = (sirenBoobs && NPC.downedBoss3) || player.gills || player.merman;
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
				if (iCantBreathe)
				{
					if (player.breath > 0)
						player.breath--;
				}
			}

			// Extra DoT in the lava of the crags. Negated by Abaddon.
			if (player.lavaWet)
			{
				if (ZoneCalamity && !abaddon)
					player.AddBuff(ModContent.BuffType<CragsLava>(), 2, false);
			}
			else
			{
				if (player.lavaImmune)
				{
					if (player.lavaTime < player.lavaMax)
						player.lavaTime++;
				}
			}

			// Acid rain droplets
			if (player.whoAmI == Main.myPlayer)
			{
				if (CalamityWorld.rainingAcid && ZoneSulphur && !areThereAnyDamnBosses && player.Center.Y < Main.worldSurface * 16f + 800f)
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

			// Hydrothermal blue smoke effects but it doesn't work epicccccc
			if (player.whoAmI == Main.myPlayer)
			{
				if (hydrothermalSmoke)
				{
					if (Math.Abs(player.velocity.X) > 0.1f || Math.Abs(player.velocity.Y) > 0.1f)
					{
						Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<HydrothermalSmoke>(), 0, 0f, player.whoAmI);
					}
				}
				// Trying to find a workaround because apparently putting the bool in ResetEffects prevents it from working
				if (!player.armorEffectDrawOutlines)
				{
					hydrothermalSmoke = false;
				}
			}

			// Death Mode effects
			caveDarkness = 0f;
			if (CalamityWorld.death)
			{
				if (player.whoAmI == Main.myPlayer)
				{
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
				}
			}

			// Increase fall speed
			if (!player.mount.Active)
			{
				if (player.IsUnderwater() && ironBoots)
					player.maxFallSpeed = 9f;

				if (!player.wet)
				{
					if (cirrusDress)
						player.maxFallSpeed = 12f;
					if (aeroSet)
						player.maxFallSpeed = 15f;
					if (gSabatonFall > 0 || player.PortalPhysicsEnabled)
						player.maxFallSpeed = 20f;
				}

				if (LungingDown)
				{
					player.maxFallSpeed = 80f;
					player.noFallDmg = true;
				}
			}

			// Omega Blue Armor bonus
			if (omegaBlueSet)
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
							int damage = (int)(390 * player.AverageDamage());
							Vector2 vel = new Vector2(Main.rand.Next(-13, 14), Main.rand.Next(-13, 14)) * 0.25f;
							Projectile.NewProjectile(player.Center, vel, ModContent.ProjectileType<OmegaBlueTentacle>(), damage, 8f, Main.myPlayer, Main.rand.Next(120), i);
						}
					}
				}

				float damageUp = 0.1f;
				int critUp = 10;
				if (omegaBlueHentai)
				{
					damageUp *= 2f;
					critUp *= 2;
				}
				player.allDamage += damageUp;
				AllCritBoost(critUp);
			}

			bool canProvideBuffs = profanedCrystalBuffs || (!profanedCrystal && pArtifact) || (profanedCrystal && CalamityWorld.downedSCal && CalamityWorld.downedExoMechs);
			bool attack = player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianAttack>()] > 0;
			// Guardian bonuses if not burnt out
			if (!bOut && canProvideBuffs)
			{
				bool healer = player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianHealer>()] > 0;
				bool defend = player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianDefense>()] > 0;
				if (healer)
				{
					if (healCounter > 0)
						healCounter--;

					if (healCounter <= 0)
					{
						bool enrage = player.statLife < (int)(player.statLifeMax2 * 0.5);

						healCounter = (!enrage && profanedCrystalBuffs) ? 360 : 300;

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
					player.moveSpeed += 0.05f +
						(attack ? 0.05f : 0f);
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

			if (nucleogenesis)
			{
				player.maxMinions += 4;
			}
			else
			{
				// First Shadowflame is +1, Statis' Blessing is +2, Statis' Curse inherits both for +3
				if (shadowMinions)
					player.maxMinions ++;
				if (holyMinions)
					player.maxMinions += 2;

				if (starTaintedGenerator)
					player.maxMinions += 2;
				else
				{
					if (starbusterCore)
						player.maxMinions++;

					if (voltaicJelly)
						player.maxMinions++;

					if (nuclearRod)
						player.maxMinions++;
				}
			}

			// Cooldowns and timers

			foreach (CooldownIndicator cd in Cooldowns)
            {
				if (cd.CanTickDown)
					cd.TimeLeft--;

				if (cd.TimeLeft < 0)
				{
					Main.PlaySound(cd.EndSound);
					cd.OnCooldownEnd();

					if (cd.SyncID != "" && Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
                    {
						player.Calamity().SyncCooldown(false, cd.SyncID);
					}
				}
            }

			Cooldowns.RemoveAll(cooldown => cooldown.TimeLeft < 0);

			if (spiritOriginBullseyeShootCountdown > 0)
				spiritOriginBullseyeShootCountdown--;
			if (phantomicHeartRegen > 0 && phantomicHeartRegen < 1000)
				phantomicHeartRegen--;
			if (phantomicBulwarkCooldown > 0)
				phantomicBulwarkCooldown--;
			if (KameiBladeUseDelay > 0)
				KameiBladeUseDelay--;
			if (galileoCooldown > 0)
				galileoCooldown--;
			if (soundCooldown > 0)
				soundCooldown--;
			if (shadowPotCooldown > 0)
				shadowPotCooldown--;
			if (raiderCooldown > 0)
				raiderCooldown--;
			if (gSabatonCooldown > 0)
				gSabatonCooldown--;
			if (gSabatonFall > 0)
				gSabatonFall--;
			if (astralStarRainCooldown > 0)
				astralStarRainCooldown--;
			if (tarraRangedCooldown > 0)
				tarraRangedCooldown--;
			if (bloodflareMageCooldown > 0)
				bloodflareMageCooldown--;
			if (silvaMageCooldown > 0)
				silvaMageCooldown--;
			if (tarraMageHealCooldown > 0)
				tarraMageHealCooldown--;
			if (rogueCrownCooldown > 0)
				rogueCrownCooldown--;
			if (spectralVeilImmunity > 0)
				spectralVeilImmunity--;
			if (jetPackCooldown > 0)
				jetPackCooldown--;
			if (jetPackDash > 0)
				jetPackDash--;
			if (theBeeCooldown > 0)
				theBeeCooldown--;
			if (jellyDmg > 0f)
				jellyDmg -= 1f;
			if (ataxiaDmg > 0f)
				ataxiaDmg -= 1.5f;
			if (ataxiaDmg < 0f)
				ataxiaDmg = 0f;
			if (xerocDmg > 0f)
				xerocDmg -= 2f;
			if (xerocDmg < 0f)
				xerocDmg = 0f;
			if (aBulwarkRareMeleeBoostTimer > 0)
				aBulwarkRareMeleeBoostTimer--;
			if (bossRushImmunityFrameCurseTimer > 0)
				bossRushImmunityFrameCurseTimer--;
			if (gaelRageAttackCooldown > 0)
				gaelRageAttackCooldown--;
			if (projRefRareLifeRegenCounter > 0)
				projRefRareLifeRegenCounter--;
			if (hurtSoundTimer > 0)
				hurtSoundTimer--;
			if (icicleCooldown > 0)
				icicleCooldown--;
			if (statisTimer > 0 && player.dashDelay >= 0)
				statisTimer = 0;
			if (hallowedRuneCooldown > 0)
				hallowedRuneCooldown--;
			if (sulphurBubbleCooldown > 0)
				sulphurBubbleCooldown--;
			if (forbiddenCooldown > 0)
				forbiddenCooldown--;
			if (tornadoCooldown > 0)
				tornadoCooldown--;
			if (ladHearts > 0)
				ladHearts--;
			if (titanBoost > 0)
				titanBoost--;
			if (prismaticLasers > 0)
				prismaticLasers--;
			if (dogTextCooldown > 0)
				dogTextCooldown--;
			if (titanCooldown > 0)
				titanCooldown--;
			if (plagueReaperCooldown > 0)
				plagueReaperCooldown--;
			if (brimflameFrenzyTimer > 0)
				brimflameFrenzyTimer--;
			if (bloodflareSoulTimer > 0)
				bloodflareSoulTimer--;
			if (fungalSymbioteTimer > 0)
				fungalSymbioteTimer--;
			if (aBulwarkRareTimer > 0)
				aBulwarkRareTimer--;
			if (hellbornBoost > 0)
				hellbornBoost--;
			if (persecutedEnchantSummonTimer < 1800)
				persecutedEnchantSummonTimer++;
            else
            {
				persecutedEnchantSummonTimer = 0;
				if (Main.myPlayer == player.whoAmI && persecutedEnchant && NPC.CountNPCS(ModContent.NPCType<DemonPortal>()) < 2)
				{
					int tries = 0;
					Vector2 spawnPosition;
					do
					{
						spawnPosition = player.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(270f, 420f);
						tries++;
					}
					while (Collision.SolidCollision(spawnPosition - Vector2.One * 24f, 48, 24) && tries < 100);
					CalamityNetcode.NewNPC_ClientSide(spawnPosition, ModContent.NPCType<DemonPortal>(), player);
				}
			}
			if (player.miscCounter % 20 == 0)
				canFireAtaxiaRangedProjectile = true;
			if (player.miscCounter % 100 == 0)
				canFireBloodflareMageProjectile = true;
			if (player.miscCounter % 150 == 0)
			{
				canFireGodSlayerRangedProjectile = true;
				canFireBloodflareRangedProjectile = true;
				canFireAtaxiaRogueProjectile = true;
			}
			if (reaverRegenCooldown < 60 && reaverRegen)
				reaverRegenCooldown++;
			else
				reaverRegenCooldown = 0;
			if (roverDrive)
			{
				if (roverDriveTimer < CalamityUtils.SecondsToFrames(30f))
					roverDriveTimer++;
				if (roverDriveTimer >= CalamityUtils.SecondsToFrames(30f))
					roverDriveTimer = 0;
			}
			else
				roverDriveTimer = 616; // Doesn't reset to zero to prevent exploits
			if (auralisAurora > 0)
				auralisAurora--;
			if (auralisAuroraCooldown > 0)
				auralisAuroraCooldown--;
			if (MythrilFlareSpawnCountdown > 0)
				MythrilFlareSpawnCountdown--;
			if (AdamantiteSetDecayDelay > 0)
				AdamantiteSetDecayDelay--;
			else if (AdamantiteSet)
			{
				adamantiteSetDefenseBoostInterpolant -= 1f / AdamantiteArmorSetChange.TimeUntilBoostCompletelyDecays;
				adamantiteSetDefenseBoostInterpolant = MathHelper.Clamp(adamantiteSetDefenseBoostInterpolant, 0f, 1f);
			}
			else
				adamantiteSetDefenseBoostInterpolant = 0f;

			// God Slayer Armor dash debuff immunity
			if (dashMod == 9 && player.dashDelay < 0)
			{
				foreach (int debuff in CalamityLists.debuffList)
					player.buffImmune[debuff] = true;
			}

			// Auric dye cinders.
			int auricDyeCount = player.dye.Count(dyeItem => dyeItem.type == ModContent.ItemType<AuricDye>());
			if (auricDyeCount > 0)
			{
				int sparkCreationChance = (int)MathHelper.Lerp(15f, 50f, Utils.InverseLerp(4f, 1f, auricDyeCount, true));
				if (Main.rand.NextBool(sparkCreationChance))
				{
					Dust spark = Dust.NewDustDirect(player.position, player.width, player.height, 267);
					spark.color = Color.Lerp(Color.Cyan, Color.SeaGreen, Main.rand.NextFloat(0.5f));
					spark.velocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2 * 1.33f) * Main.rand.NextFloat(2f, 5.4f);
					spark.noGravity = true;
				}
			}

			// Silva invincibility effects
			if (silvaCountdown > 0 && hasSilvaEffect && silvaSet)
			{
				foreach (int debuff in CalamityLists.debuffList)
					player.buffImmune[debuff] = true;

				silvaCountdown -= 1;
				if (silvaCountdown <= 0)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SilvaDispel"), player.Center);
					// 5 minutes
					Cooldowns.Add(new SilvaReviveCooldown(18000, player));
				}

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
			if (!Cooldowns.Exists(cooldown => cooldown.GetType() == typeof(SilvaReviveCooldown)) && hasSilvaEffect && silvaCountdown <= 0 && !areThereAnyDamnBosses && !areThereAnyDamnEvents)
			{
				silvaCountdown = 480;
				hasSilvaEffect = false;
			}

			// Tarragon cloak effects
			if (tarragonCloak)
			{
				tarraDefenseTime--;
				if (tarraDefenseTime <= 0)
				{
					tarraDefenseTime = 600;
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
			if (tarraThrowing)
			{
				// The iframes from the evasion are disabled by Armageddon.
				if (tarragonImmunity && !disableAllDodges)
					player.GiveIFrames(2, true);


				if (tarraThrowingCrits >= 25)
				{
					tarraThrowingCrits = 0;
					if (player.whoAmI == Main.myPlayer && !disableAllDodges)
						player.AddBuff(ModContent.BuffType<TarragonImmunity>(), 180, false);
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
						throwingDamage += 0.1f;
				}
			}

			// Bloodflare pickup spawn cooldowns
			if (bloodflareSet)
			{
				if (bloodflareHeartTimer > 0)
					bloodflareHeartTimer--;
			}

			// Bloodflare frenzy effects
			if (bloodflareMelee)
			{
				if (bloodflareMeleeHits >= 15)
				{
					bloodflareMeleeHits = 0;
					if (player.whoAmI == Main.myPlayer)
						player.AddBuff(ModContent.BuffType<BloodflareBloodFrenzy>(), 302, false);
				}

				if (bloodflareFrenzy)
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
			if (raiderTalisman)
			{
				// Nanotech use to have an exclusive nerf here, but since they are currently equal, there
				// is no check to indicate such.
				float damageMult = 0.15f;
				throwingDamage += raiderStack / 150f * damageMult;
			}

			if (kamiBoost)
				player.allDamage += 0.15f;

			if (avertorBonus)
				player.allDamage += 0.1f;

			if (roverDriveTimer < 616)
			{
				player.statDefense += 15;
				if (roverDriveTimer > 606)
					player.statDefense -= roverDriveTimer - 606; //so it scales down when the shield dies
			}

			// Absorber bonus
			if (absorber)
			{
				player.moveSpeed += 0.05f;
				player.jumpSpeedBoost += 0.25f;
				player.thorns += 0.5f;
				player.endurance += sponge ? 0.15f : 0.1f;

				if (player.StandingStill() && player.itemAnimation == 0)
					player.manaRegenBonus += 4;
			}

			// Sea Shell bonus
			if (seaShell)
			{
				if (player.IsUnderwater())
				{
					player.statDefense += 3;
					player.endurance += 0.05f;
					player.moveSpeed += 0.1f;
					player.ignoreWater = true;
				}
			}

			// Affliction bonus
			if (affliction || afflicted)
			{
				player.endurance += 0.07f;
				player.statDefense += 13;
				player.allDamage += 0.1f;
			}

			// Ambrosial Ampoule bonus and other light-granting bonuses
			float[] light = new float[3];
			if ((rOoze && !Main.dayTime) || aAmpoule)
			{
				light[0] += 1f;
				light[1] += 1f;
				light[2] += 0.6f;
			}
			if (aAmpoule)
			{
				player.endurance += 0.07f;
				player.buffImmune[BuffID.Frozen] = true;
				player.buffImmune[BuffID.Chilled] = true;
				player.buffImmune[BuffID.Frostburn] = true;
			}
			if (cFreeze)
			{
				light[0] += 0.3f;
				light[1] += Main.DiscoG / 400f;
				light[2] += 0.5f;
			}
			if (sirenIce)
			{
				light[0] += 0.35f;
				light[1] += 1f;
				light[2] += 1.25f;
			}
			if (sirenBoobs)
			{
				light[0] += 0.1f;
				light[1] += 1f;
				light[2] += 1.5f;
			}
			if (tarraSummon)
			{
				light[0] += 0f;
				light[1] += 3f;
				light[2] += 0f;
			}
			if (forbiddenCirclet)
			{
				light[0] += 0.8f;
				light[1] += 0.7f;
				light[2] += 0.2f;
			}
			Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), light[0], light[1], light[2]);

			// Blazing Core bonus
			if (blazingCore)
				player.endurance += 0.1f;

			//Permafrost's Concoction bonuses/debuffs
			if (permafrostsConcoction)
				player.manaCost *= 0.85f;

			if (encased)
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
			if (cFreeze)
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
									npc.AddBuff(buffType, 60, false);
							}
						}
					}
				}
			}

			// Remove Purified Jam and Lul accessory thorn damage exploits
			if (invincible || lol)
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
			if (polarisBoost)
			{
				player.endurance += 0.01f;
				player.statDefense += 2;
			}
			if (!polarisBoost || player.ActiveItem().type != ModContent.ItemType<PolarisParrotfish>())
			{
				polarisBoost = false;
				if (player.FindBuffIndex(ModContent.BuffType<PolarisBuff>()) > -1)
					player.ClearBuff(ModContent.BuffType<PolarisBuff>());

				polarisBoostCounter = 0;
				polarisBoostTwo = false;
				polarisBoostThree = false;
			}
			if (polarisBoostCounter >= 20)
			{
				polarisBoostTwo = false;
				polarisBoostThree = true;
			}
			else if (polarisBoostCounter >= 10)
				polarisBoostTwo = true;

			// Calcium Potion buff
			if (calcium)
				player.noFallDmg = true;

			// Ceaseless Hunger Potion buff
			if (ceaselessHunger)
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

			// Plagued Fuel Pack and Blunder Booster effects
			if (jetPackDash > 0 && player.whoAmI == Main.myPlayer)
			{
				int velocityAmt = blunderBooster ? 35 : 25;
				int velocityMult = jetPackDash > 1 ? velocityAmt : 5;
				player.velocity = new Vector2(jetPackDirection, -1) * velocityMult;

				if (blunderBooster)
				{
					int lightningCount = Main.rand.Next(2, 7);
					for (int i = 0; i < lightningCount; i++)
					{
						Vector2 lightningVel = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
						lightningVel.Normalize();
						lightningVel *= Main.rand.NextFloat(1f, 2f);
						int projectile = Projectile.NewProjectile(player.Center, lightningVel, ModContent.ProjectileType<BlunderBoosterLightning>(), (int)(30 * player.RogueDamage()), 0, player.whoAmI, Main.rand.Next(2), 0f);
						Main.projectile[projectile].timeLeft = Main.rand.Next(180, 240);
						if (projectile.WithinBounds(Main.maxProjectiles))
							Main.projectile[projectile].Calamity().forceTypeless = true;
					}

					for (int i = 0; i < 3; i++)
					{
						int dust = Dust.NewDust(player.Center, 1, 1, 60, player.velocity.X * -0.1f, player.velocity.Y * -0.1f, 100, default, 3.5f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].velocity *= 1.2f;
						Main.dust[dust].velocity.Y -= 0.15f;
					}
				}
				else if (plaguedFuelPack)
				{
					int numClouds = Main.rand.Next(2, 10);
					for (int i = 0; i < numClouds; i++)
					{
						Vector2 cloudVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
						cloudVelocity.Normalize();
						cloudVelocity *= Main.rand.NextFloat(0f, 1f);
						int projectile = Projectile.NewProjectile(player.Center, cloudVelocity, ModContent.ProjectileType<PlaguedFuelPackCloud>(), (int)(20 * player.RogueDamage()), 0, player.whoAmI, 0, 0);
						Main.projectile[projectile].timeLeft = Main.rand.Next(180, 240);
						if (projectile.WithinBounds(Main.maxProjectiles))
							Main.projectile[projectile].Calamity().forceTypeless = true;
					}

					for (int i = 0; i < 3; i++)
					{
						int dust = Dust.NewDust(player.Center, 1, 1, 89, player.velocity.X * -0.1f, player.velocity.Y * -0.1f, 100, default, 3.5f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].velocity *= 1.2f;
						Main.dust[dust].velocity.Y -= 0.15f;
					}
				}
			}

			// Gravistar Sabaton effects
			if (gSabaton && player.whoAmI == Main.myPlayer)
			{
				if (gSabatonCooldown <= 0 && !player.mount.Active)
				{
					if (player.controlDown && player.releaseDown && player.position.Y != player.oldPosition.Y)
					{
						gSabatonFall = 300;
						gSabatonCooldown = 480; //8 second cooldown
						player.gravity *= 2f;
						Projectile.NewProjectile(player.Center.X, player.Center.Y + (player.height / 5f), player.velocity.X, player.velocity.Y, ModContent.ProjectileType<SabatonSlam>(), 0, 0, player.whoAmI);
					}
				}
				if (gSabatonCooldown == 1) //dust when ready to use again
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

			if (!brimflameSet && brimflameFrenzy)
			{
				brimflameFrenzy = false;
				player.ClearBuff(ModContent.BuffType<BrimflameFrenzyBuff>());
				player.AddBuff(ModContent.BuffType<BrimflameFrenzyCooldown>(), BrimflameScowl.CooldownLength, true);
				brimflameFrenzyTimer = BrimflameScowl.CooldownLength;
			}
			if (!bloodflareMelee && bloodflareFrenzy)
			{
				bloodflareFrenzy = false;
				player.ClearBuff(ModContent.BuffType<BloodflareBloodFrenzy>());
				player.AddBuff(ModContent.BuffType<BloodflareBloodFrenzyCooldown>(), 1800, false);
			}
			if (!tarraMelee && tarragonCloak)
			{
				tarragonCloak = false;
				player.ClearBuff(ModContent.BuffType<TarragonCloak>());
				player.AddBuff(ModContent.BuffType<TarragonCloakCooldown>(), 600, false);
			}
			if (!tarraThrowing && tarragonImmunity)
			{
				tarragonImmunity = false;
				player.ClearBuff(ModContent.BuffType<TarragonImmunity>());
				player.AddBuff(ModContent.BuffType<TarragonImmunityCooldown>(), 600, false);
			}
			if (!omegaBlueSet && Cooldowns.Exists(cooldown => cooldown.GetType() == typeof(OmegaBlueCooldown) && cooldown.TimeLeft > 1500))
			{
				Cooldowns.Find(cooldown => cooldown.GetType() == typeof(OmegaBlueCooldown)).TimeLeft = 1500;
				player.ClearBuff(ModContent.BuffType<AbyssalMadness>());
			}
			if (!plagueReaper && plagueReaperCooldown > 1500)
			{
				plagueReaperCooldown = 1500;
				player.AddBuff(ModContent.BuffType<PlagueBlackoutCooldown>(), 1500, false);
			}
			if (!prismaticSet && prismaticLasers > 1800)
			{
				prismaticLasers = 1800;
				Cooldowns.Add(new PrismaticLaserCooldown(CalamityUtils.SecondsToFrames(30f), player));
			}
			if (!angelicAlliance && divineBless)
			{
				divineBless = false;
				player.ClearBuff(ModContent.BuffType<DivineBless>());
				int seconds = CalamityUtils.SecondsToFrames(60f);
				Cooldowns.Add(new DivineBlessCooldown(seconds, player));
			}

			// Armageddon's Dodge Disable feature puts Shadow Dodge/Holy Protection on permanent cooldown
			if (disableAllDodges)
			{
				if (player.shadowDodgeTimer < 2)
					player.shadowDodgeTimer = 2;
			}
		}
		#endregion

		#region Abyss Effects
		private void AbyssEffects()
		{
			int lightStrength = GetTotalLightStrength();
			abyssLightLevelStat = lightStrength;

			if (ZoneAbyss)
			{
				if (Main.myPlayer == player.whoAmI)
				{
					// Abyss depth variables
					Point point = player.Center.ToTileCoordinates();
					double abyssSurface = Main.rockLayer - Main.maxTilesY * 0.05;
					double abyssLevel1 = Main.rockLayer + Main.maxTilesY * 0.03;
					double totalAbyssDepth = Main.maxTilesY - 250D - abyssSurface;
					double totalAbyssDepthFromLayer1 = Main.maxTilesY - 250D - abyssLevel1;
					double playerAbyssDepth = point.Y - abyssSurface;
					double playerAbyssDepthFromLayer1 = point.Y - abyssLevel1;
					double depthRatio = playerAbyssDepth / totalAbyssDepth;
					double depthRatioFromAbyssLayer1 = playerAbyssDepthFromLayer1 / totalAbyssDepthFromLayer1;

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
					caveDarkness = darknessStrength * multiplier;

					// Nebula Headcrab darkness effect
					if (!player.headcovered)
					{
						float screenObstructionAmt = MathHelper.Clamp(caveDarkness, 0f, 0.95f);
						float targetValue = MathHelper.Clamp(screenObstructionAmt * 0.7f, 0.1f, 0.3f);
						ScreenObstruction.screenObstruction = MathHelper.Lerp(ScreenObstruction.screenObstruction, screenObstructionAmt, targetValue);
					}

					// Breath lost while at zero breath
					double breathLoss = point.Y > abyssLevel1 ? 50D * depthRatioFromAbyssLayer1 : 0D;

					// Breath Loss Multiplier, depending on gear
					double breathLossMult = 1D -
						(player.gills ? 0.2 : 0D) - // 0.8
						(player.accDivingHelm ? 0.25 : 0D) - // 0.75
						(player.arcticDivingGear ? 0.25 : 0D) - // 0.75
						(aquaticEmblem ? 0.25 : 0D) - // 0.75
						(player.accMerman ? 0.3 : 0D) - // 0.7
						(victideSet ? 0.2 : 0D) - // 0.85
						((sirenBoobs && NPC.downedBoss3) ? 0.3 : 0D) - // 0.7
						(abyssalDivingSuit ? 0.3 : 0D); // 0.7

					// Limit the multiplier to 5%
					if (breathLossMult < 0.05)
						breathLossMult = 0.05;

					// Reduce breath lost while at zero breath, depending on gear
					breathLoss *= breathLossMult;

					// Stat Meter stat
					abyssBreathLossStat = (int)breathLoss;

					// Defense loss
					int defenseLoss = (int)(120D * depthRatio);

					// Anechoic Plating reduces defense loss by 66%
					// Fathom Swarmer Breastplate reduces defense loss by 40%
					// In tandem, reduces defense loss by 80%
					if (anechoicPlating && fathomSwarmerBreastplate)
						defenseLoss = (int)(defenseLoss * 0.2f);
					else if (anechoicPlating)
						defenseLoss /= 3;
					else if (fathomSwarmerBreastplate)
						defenseLoss = (int)(defenseLoss * 0.6f);

					// Reduce defense
					player.statDefense -= defenseLoss;

					// Stat Meter stat
					abyssDefenseLossStat = defenseLoss;

					// Bleed effect based on abyss layer
					if (ZoneAbyssLayer4)
					{
						player.bleed = true;
					}
					else if (ZoneAbyssLayer3)
					{
						if (!abyssalDivingSuit)
							player.bleed = true;
					}
					else if (ZoneAbyssLayer2)
					{
						if (!depthCharm)
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
						(aquaticEmblem ? 10D : 0D) + // 40
						(player.accMerman ? 15D : 0D) + // 55
						(victideSet ? 5D : 0D) + // 60
						((sirenBoobs && NPC.downedBoss3) ? 15D : 0D) + // 75
						(abyssalDivingSuit ? 15D : 0D); // 90

					// Limit the multiplier to 50
					if (tickMult > 50D)
						tickMult = 50D;

					// Increase ticks (frames) until breath is deducted, depending on gear
					tick *= tickMult;

					// Stat Meter stat
					abyssBreathLossRateStat = (int)tick;

					// Reduce breath over ticks (frames)
					abyssBreathCD++;
					if (abyssBreathCD >= (int)tick)
					{
						// Reset modded breath variable
						abyssBreathCD = 0;

						// Reduce breath
						if (player.breath > 0)
							player.breath -= (int)(cDepth ? breathLoss + 1D : breathLoss);
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
						(depthCharm ? 3 : 0) +
						(abyssalDivingSuit ? 6 : 0);

					// Reduce life loss, depending on gear
					lifeLossAtZeroBreath -= lifeLossAtZeroBreathResist;

					// Prevent negatives
					if (lifeLossAtZeroBreath < 0)
						lifeLossAtZeroBreath = 0;

					// Stat Meter stat
					abyssLifeLostAtZeroBreathStat = lifeLossAtZeroBreath;

					// Check breath value
					if (player.breath <= 0)
					{
						// Reduce life
						player.statLife -= lifeLossAtZeroBreath;

						// Special kill code if the life loss kills the player
						if (player.statLife <= 0)
						{
							abyssDeath = true;
							KillPlayer();
						}
					}
				}
			}
			else
			{
				abyssBreathCD = 0;
				abyssDeath = false;
			}
		}
		#endregion

		#region Calamitas Enchantment Held Item Effects
		public static void EnchantHeldItemEffects(Player player, CalamityPlayer modPlayer, Item heldItem)
		{
			if (heldItem.IsAir)
				return;

			// Exhaustion recharge effects.
			foreach (Item item in player.inventory)
			{
				if (item.IsAir)
					continue;

				if (item.Calamity().AppliedEnchantment.HasValue && item.Calamity().AppliedEnchantment.Value.ID == 600)
				{
					// Initialize the exhaustion if it is currently not defined.
					if (item.Calamity().DischargeEnchantExhaustion <= 0f)
						item.Calamity().DischargeEnchantExhaustion = CalamityGlobalItem.DischargeEnchantExhaustionCap;

					// Slowly recharge the weapon over time. This is depleted when the item is actaully used.
					else if (item.Calamity().DischargeEnchantExhaustion < CalamityGlobalItem.DischargeEnchantExhaustionCap)
						item.Calamity().DischargeEnchantExhaustion++;
				}
				else
					item.Calamity().DischargeEnchantExhaustion = 0f;
			}

			if (!heldItem.Calamity().AppliedEnchantment.HasValue || heldItem.Calamity().AppliedEnchantment.Value.HoldEffect is null)
				return;

			heldItem.Calamity().AppliedEnchantment.Value.HoldEffect(player);

			// Weak brimstone flame hold curse effect.
			if (modPlayer.flamingItemEnchant)
				player.AddBuff(ModContent.BuffType<WeakBrimstoneFlames>(), 10);
		}
		#endregion

		#region Max Life And Mana Effects
		private void MaxLifeAndManaEffects()
		{
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
					(pHeart ? 1 : 0) +
					(eCore ? 1 : 0) +
					(cShard ? 1 : 0);
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
				else if (Main.raining && ZoneSulphur)
					Main.rainTexture = rain3;
				else
					Main.rainTexture = rainOriginal;

				if (auricSet)
					Main.flyingCarpetTexture = carpetAuric;
				else
					Main.flyingCarpetTexture = carpetOriginal;
			}
		}
		#endregion

		#region Standing Still Effects
		private void StandingStillEffects()
		{
			// Rogue Stealth
			UpdateRogueStealth();

			// Trinket of Chi bonus
			if (trinketOfChi)
			{
				if (trinketOfChiBuff)
				{
					player.allDamage += 0.5f;
					if (player.itemAnimation > 0)
						chiBuffTimer = 0;
				}

				if (player.StandingStill(0.1f) && !player.mount.Active)
				{
					if (chiBuffTimer < 60)
						chiBuffTimer++;
					else
						player.AddBuff(ModContent.BuffType<ChiBuff>(), 6);
				}
				else
					chiBuffTimer--;
			}
			else
				chiBuffTimer = 0;

			// Aquatic Emblem bonus
			if (aquaticEmblem)
			{
				if (player.IsUnderwater() && player.wet && !player.lavaWet && !player.honeyWet &&
					!player.mount.Active)
				{
					if (aquaticBoost > 0f)
					{
						aquaticBoost -= 2f;
						if (aquaticBoost <= 0f)
						{
							aquaticBoost = 0f;
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
						}
					}
				}
				else
				{
					aquaticBoost += 2f;
					if (aquaticBoost > aquaticBoostMax)
						aquaticBoost = aquaticBoostMax;
					if (player.mount.Active)
						aquaticBoost = aquaticBoostMax;
				}

				player.statDefense += (int)((1f - aquaticBoost * 0.0001f) * 50f);
				player.moveSpeed -= (1f - aquaticBoost * 0.0001f) * 0.1f;
			}
			else
				aquaticBoost = aquaticBoostMax;

			// Auric bonus
			if (auricBoost)
			{
				if (player.itemAnimation > 0)
					modStealthTimer = 5;

				if (player.StandingStill(0.1f) && !player.mount.Active)
				{
					if (modStealthTimer == 0 && modStealth > 0f)
					{
						modStealth -= 0.015f;
						if (modStealth <= 0f)
						{
							modStealth = 0f;
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
						}
					}
				}
				else
				{
					float playerVel = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
					modStealth += playerVel * 0.0075f;
					if (modStealth > 1f)
						modStealth = 1f;
					if (player.mount.Active)
						modStealth = 1f;
				}

				float damageBoost = (1f - modStealth) * 0.2f;
				player.allDamage += damageBoost;

				int critBoost = (int)((1f - modStealth) * 10f);
				AllCritBoost(critBoost);

				if (modStealthTimer > 0)
					modStealthTimer--;
			}

			// Psychotic Amulet bonus
			else if (pAmulet)
			{
				if (player.itemAnimation > 0)
					modStealthTimer = 5;

				if (player.StandingStill(0.1f) && !player.mount.Active)
				{
					if (modStealthTimer == 0 && modStealth > 0f)
					{
						modStealth -= 0.015f;
						if (modStealth <= 0f)
						{
							modStealth = 0f;
							if (Main.netMode == NetmodeID.MultiplayerClient)
								NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
						}
					}
				}
				else
				{
					float playerVel = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
					modStealth += playerVel * 0.0075f;
					if (modStealth > 1f)
						modStealth = 1f;
					if (player.mount.Active)
						modStealth = 1f;
				}

				throwingDamage += (1f - modStealth) * 0.2f;
				throwingCrit += (int)((1f - modStealth) * 10f);
				player.aggro -= (int)((1f - modStealth) * 750f);
				if (modStealthTimer > 0)
					modStealthTimer--;
			}
			else
				modStealth = 1f;

			if (player.ActiveItem().type == ModContent.ItemType<Auralis>() && player.StandingStill(0.1f))
			{
				if (auralisStealthCounter < 300f)
					auralisStealthCounter++;

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

				if (usingScope && auralisAuroraCounter < chargeDuration + auroraDuration)
					auralisAuroraCounter++;

				if (auralisAuroraCounter > chargeDuration + auroraDuration)
				{
					auralisAuroraCounter = 0;
					auralisAuroraCooldown = CalamityUtils.SecondsToFrames(30f);
				}

				if (auralisAuroraCounter > 0 && auralisAuroraCounter < chargeDuration && !usingScope)
					auralisAuroraCounter--;

				if (auralisAuroraCounter > chargeDuration && auralisAuroraCounter < chargeDuration + auroraDuration && !usingScope)
					auralisAuroraCounter = 0;
			}
			else
			{
				auralisStealthCounter = 0f;
				auralisAuroraCounter = 0;
			}
			if (auralisAuroraCooldown > 0)
			{
				if (auralisAuroraCooldown == 1)
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
				auralisAuroraCounter = 0;
			}
		}
		#endregion

		#region Elysian Aegis Effects
		private void ElysianAegisEffects()
		{
			if (elysianAegis)
			{
				bool spawnDust = false;

				// Activate buff
				if (elysianGuard)
				{
					if (player.whoAmI == Main.myPlayer)
						player.AddBuff(ModContent.BuffType<ElysianGuard>(), 2, false);

					float shieldBoostInitial = shieldInvinc;
					shieldInvinc -= 0.08f;
					if (shieldInvinc < 0f)
						shieldInvinc = 0f;
					else
						spawnDust = true;

					if (shieldInvinc == 0f && shieldBoostInitial != shieldInvinc && Main.netMode == NetmodeID.MultiplayerClient)
						NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);

					float damageBoost = (5f - shieldInvinc) * 0.03f;
					player.allDamage += damageBoost;

					int critBoost = (int)((5f - shieldInvinc) * 2f);
					AllCritBoost(critBoost);

					player.aggro += (int)((5f - shieldInvinc) * 220f);
					player.statDefense += (int)((5f - shieldInvinc) * 8f);
					player.moveSpeed *= 0.85f;

					if (player.mount.Active)
						elysianGuard = false;
				}

				// Remove buff
				else
				{
					float shieldBoostInitial = shieldInvinc;
					shieldInvinc += 0.08f;
					if (shieldInvinc > 5f)
						shieldInvinc = 5f;
					else
						spawnDust = true;

					if (shieldInvinc == 5f && shieldBoostInitial != shieldInvinc && Main.netMode == NetmodeID.MultiplayerClient)
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
				elysianGuard = false;
		}
		#endregion

		#region Other Buff Effects
		private void OtherBuffEffects()
		{
			if (gravityNormalizer)
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
			if (decayEffigy)
			{
				player.buffImmune[ModContent.BuffType<SulphuricPoisoning>()] = true;
				if (!ZoneAbyss)
				{
					player.gills = true;
				}
			}

			// Cobalt armor set effects.
			if (CobaltSet)
				CobaltArmorSetChange.ApplyMovementSpeedBonuses(player);

			// Adamantite armor set effects.
			if (AdamantiteSet)
				player.statDefense += AdamantiteSetDefenseBoost;

			if (astralInjection)
			{
				if (player.statMana < player.statManaMax2)
					player.statMana += 3;
				if (player.statMana > player.statManaMax2)
					player.statMana = player.statManaMax2;
			}

			if (armorCrumbling)
			{
				throwingCrit += 5;
				player.meleeCrit += 5;
			}

			if (armorShattering)
			{
				if (player.FindBuffIndex(ModContent.BuffType<ArmorCrumbling>()) > -1)
					player.ClearBuff(ModContent.BuffType<ArmorCrumbling>());
				throwingDamage += 0.08f;
				player.meleeDamage += 0.08f;
				throwingCrit += 8;
				player.meleeCrit += 8;
			}

			if (holyWrath)
			{
				if (player.FindBuffIndex(BuffID.Wrath) > -1)
					player.ClearBuff(BuffID.Wrath);
				player.allDamage += 0.12f;
			}

			if (profanedRage)
			{
				if (player.FindBuffIndex(BuffID.Rage) > -1)
					player.ClearBuff(BuffID.Rage);
				AllCritBoost(12);
			}

			if (shadow)
			{
				if (player.FindBuffIndex(BuffID.Invisibility) > -1)
					player.ClearBuff(BuffID.Invisibility);
			}

			if (irradiated)
			{
				player.statDefense -= 10;
				player.moveSpeed -= 0.1f;
				player.allDamage += 0.05f;
				player.minionKB += 0.5f;
			}

			if (rRage)
			{
				player.allDamage += 0.3f;
				player.statDefense += 5;
			}

			if (xRage)
				throwingDamage += 0.1f;

			if (xWrath)
				throwingCrit += 5;

			if (graxDefense)
			{
				player.statDefense += 30;
				player.endurance += 0.1f;
				player.meleeDamage += 0.2f;
			}

			if (tFury)
			{
				player.meleeDamage += 0.3f;
				player.meleeCrit += 10;
			}

			if (yPower)
			{
				player.endurance += 0.06f;
				player.statDefense += 8;
				player.pickSpeed -= 0.05f;
				player.allDamage += 0.06f;
				AllCritBoost(2);
				player.minionKB += 1f;
				player.moveSpeed += 0.06f;
			}

			if (tScale)
			{
				player.endurance += 0.05f;
				player.statDefense += 5;
				player.kbBuff = true;
				if (titanBoost > 0)
				{
					player.statDefense += 25;
					player.endurance += 0.1f;
				}
			}
			else
				titanBoost = 0;

			if (darkSunRing)
			{
				player.maxMinions += 2;
				player.allDamage += 0.12f;
				player.minionKB += 1.2f;
				player.pickSpeed -= 0.15f;
				if (Main.eclipse || !Main.dayTime)
					player.statDefense += 15;
			}

			if (eGauntlet)
			{
				player.kbGlove = true;
				player.magmaStone = true;
				player.meleeDamage += 0.15f;
				player.meleeCrit += 5;
				player.lavaMax += 240;
			}

			if (bloodPactBoost)
			{
				player.allDamage += 0.05f;
				player.statDefense += 20;
				player.endurance += 0.1f;
				player.longInvince = true;
				player.crimsonRegen = true;
			}

			if (cirrusDress)
				player.moveSpeed -= 0.2f;

			if (fabsolVodka)
				player.allDamage += 0.08f;

			if (vodka)
			{
				player.allDamage += 0.06f;
				AllCritBoost(2);
			}

			if (grapeBeer)
				player.moveSpeed -= 0.05f;

			if (moonshine)
			{
				player.statDefense += 10;
				player.endurance += 0.05f;
			}

			if (rum)
				player.moveSpeed += 0.1f;

			if (whiskey)
			{
				player.allDamage += 0.04f;
				AllCritBoost(2);
			}

			if (everclear)
				player.allDamage += 0.25f;

			if (bloodyMary)
			{
				if (Main.bloodMoon)
				{
					player.allDamage += 0.15f;
					AllCritBoost(7);
					player.moveSpeed += 0.1f;
				}
			}

			if (tequila)
			{
				if (Main.dayTime)
				{
					player.statDefense += 5;
					player.allDamage += 0.03f;
					AllCritBoost(2);
					player.endurance += 0.03f;
				}
			}

			if (tequilaSunrise)
			{
				if (Main.dayTime)
				{
					player.statDefense += 10;
					player.allDamage += 0.07f;
					AllCritBoost(3);
					player.endurance += 0.07f;
				}
			}

			if (caribbeanRum)
				player.moveSpeed += 0.1f;

			if (cinnamonRoll)
			{
				player.manaRegenDelay--;
				player.manaRegenBonus += 10;
			}

			if (starBeamRye)
			{
				player.magicDamage += 0.08f;
				player.manaCost *= 0.9f;
			}

			if (moscowMule)
			{
				player.allDamage += 0.09f;
				AllCritBoost(3);
			}

			if (whiteWine)
				player.magicDamage += 0.1f;

			if (evergreenGin)
				player.endurance += 0.05f;

			if (giantPearl)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient && !areThereAnyDamnBosses)
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
				throwingDamage += 0.1f;

			if (CalamityLists.javelinList.Contains(player.ActiveItem().type) && player.invis)
				player.armorPenetration += 5;

			if (CalamityLists.flaskBombList.Contains(player.ActiveItem().type) && player.invis)
				throwingVelocity += 0.1f;

			if (CalamityLists.spikyBallList.Contains(player.ActiveItem().type) && player.invis)
				throwingCrit += 10;

			if (planarSpeedBoost != 0)
			{
				if (player.ActiveItem().type != ModContent.ItemType<PrideHuntersPlanarRipper>())
					planarSpeedBoost = 0;
			}
			if (brimlashBusterBoost)
			{
				if (player.ActiveItem().type != ModContent.ItemType<BrimlashBuster>())
					brimlashBusterBoost = false;
			}
			if (evilSmasherBoost > 0)
			{
				if (player.ActiveItem().type != ModContent.ItemType<EvilSmasher>())
					evilSmasherBoost = 0;
			}
			if (searedPanCounter > 0)
			{
				if (player.ActiveItem().type != ModContent.ItemType<SearedPan>())
				{
					searedPanCounter = 0;
					searedPanTimer = 0;
				}
				else if (searedPanTimer < SearedPan.ConsecutiveHitOpening)
					searedPanTimer++;
				else
					searedPanCounter = 0;
			}
			if (animusBoost > 1f)
			{
				if (player.ActiveItem().type != ModContent.ItemType<Animus>())
					animusBoost = 1f;
			}

			// Flight time boosts
			double flightTimeMult = 1D +
				(ZoneAstral ? 0.05 : 0D) +
				(harpyRing ? 0.2 : 0D) +
				(reaverSpeed ? 0.1 : 0D) +
				(aeroStone ? 0.1 : 0D) +
				(angelTreads ? 0.1 : 0D) +
				(blueCandle ? 0.1 : 0D) +
				(soaring ? 0.1 : 0D) +
				(prismaticGreaves ? 0.1 : 0D) +
				(plagueReaper ? 0.05 : 0D) +
				(draconicSurge ? 0.2 : 0D);

			if (harpyRing)
				player.moveSpeed += 0.1f;

			if (blueCandle)
				player.moveSpeed += 0.1f;

			if (Cooldowns.Exists(cooldown => cooldown.GetType() == typeof(DraconicElixirCooldown))) // Weird mod conflicts with like Luiafk
			{
				draconicSurge = false;
				if (player.FindBuffIndex(ModContent.BuffType<DraconicSurgeBuff>()) > -1)
					player.ClearBuff(ModContent.BuffType<DraconicSurgeBuff>());
			}

			if (community)
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
				int critBoost = integerTypeBoost / 2;
				float damageBoost = floatTypeBoost * 0.5f;
				player.endurance += floatTypeBoost * 0.25f;
				player.statDefense += integerTypeBoost;
				player.allDamage += damageBoost;
				AllCritBoost(critBoost);
				player.minionKB += floatTypeBoost;
				player.moveSpeed += floatTypeBoost * 0.5f;
				flightTimeMult += floatTypeBoost;
			}
			// Shattered Community gives the same wing time boost as normal Community
			if (shatteredCommunity)
				flightTimeMult += 0.2f;

			if (profanedCrystalBuffs && gOffense && gDefense)
			{
				bool offenseBuffs = (Main.dayTime && !player.wet) || player.lavaWet;
				if (offenseBuffs)
						flightTimeMult += 0.1;
				}

			// Increase wing time
			if (player.wingTimeMax > 0)
				player.wingTimeMax = (int)(player.wingTimeMax * flightTimeMult);

			if (vHex)
			{
				player.blind = true;
				player.statDefense -= 10;
				player.moveSpeed -= 0.1f;

				if (player.wingTimeMax < 0)
					player.wingTimeMax = 0;

				player.wingTimeMax = (int)(player.wingTimeMax * 0.75);
			}

			if (eGravity)
			{
				if (player.wingTimeMax < 0)
					player.wingTimeMax = 0;

				if (player.wingTimeMax > 400)
					player.wingTimeMax = 400;

				player.wingTimeMax = (int)(player.wingTimeMax * 0.66);
			}

			if (eGrav)
			{
				if (player.wingTimeMax < 0)
					player.wingTimeMax = 0;

				if (player.wingTimeMax > 400)
					player.wingTimeMax = 400;

				player.wingTimeMax = (int)(player.wingTimeMax * 0.75);
			}

			if (bounding)
			{
				player.jumpSpeedBoost += 0.25f;
				Player.jumpHeight += 10;
				player.extraFall += 25;
			}

			if (mushy)
				player.statDefense += 5;

			if (omniscience)
			{
				player.detectCreature = true;
				player.dangerSense = true;
				player.findTreasure = true;
			}

			if (aWeapon)
				player.moveSpeed += 0.1f;

			if (molten)
				player.resistCold = true;

			if (shellBoost)
				player.moveSpeed += 0.4f;

			if (tarraSet)
			{
				if (!tarraMelee)
					player.calmed = true;
				player.lifeMagnet = true;
			}

			if (cadence)
			{
				if (player.FindBuffIndex(BuffID.Regeneration) > -1)
					player.ClearBuff(BuffID.Regeneration);
				if (player.FindBuffIndex(BuffID.Lifeforce) > -1)
					player.ClearBuff(BuffID.Lifeforce);
				player.lifeMagnet = true;
				player.calmed = true;
			}

			if (player.wellFed)
				player.moveSpeed -= 0.1f;

			if (player.poisoned)
				player.moveSpeed -= 0.1f;

			if (player.venom)
				player.moveSpeed -= 0.15f;

			if (wDeath)
			{
				player.allDamage -= 0.2f;
				player.moveSpeed -= 0.1f;
			}

			if (lethalLavaBurn)
				player.moveSpeed -= 0.15f;

			if (hInferno)
				player.moveSpeed -= 0.25f;

			if (gsInferno)
				player.moveSpeed -= 0.15f;

			if (astralInfection)
			{
				player.allDamage -= 0.1f;
				player.moveSpeed -= 0.15f;
			}

			if (pFlames)
			{
				player.blind = !reducedPlagueDmg;
				player.allDamage -= 0.1f;
				player.moveSpeed -= 0.15f;
			}

			if (bBlood)
			{
				player.blind = true;
				player.statDefense -= 3;
				player.moveSpeed += 0.1f;
				player.meleeDamage += 0.05f;
				player.rangedDamage -= 0.1f;
				player.magicDamage -= 0.1f;
			}

			if (aCrunch && !laudanum)
			{
				player.statDefense -= ArmorCrunch.DefenseReduction;
				player.endurance *= 0.33f;
			}

			if (wCleave && !laudanum)
			{
				player.statDefense -= WarCleave.DefenseReduction;
				player.endurance *= 0.75f;
			}

			if (wither)
			{
				player.statDefense -= WitherDebuff.DefenseReduction;
			}

			if (gState)
			{
				player.velocity.X *= 0.5f;
				player.velocity.Y += 0.05f;
				if (player.velocity.Y > 15f)
					player.velocity.Y = 15f;
			}

			if (eFreeze)
			{
				player.velocity.X *= 0.5f;
				player.velocity.Y += 0.1f;
				if (player.velocity.Y > 15f)
					player.velocity.Y = 15f;
			}

			if (eFreeze || eutrophication)
				player.velocity = Vector2.Zero;

			if (vaporfied || teslaFreeze)
				player.velocity *= 0.98f;

			if (molluskSet)
				player.velocity.X *= 0.985f;

			if ((warped || caribbeanRum) && !player.slowFall && !player.mount.Active)
			{
				player.velocity.Y *= 1.01f;
				player.moveSpeed -= 0.1f;
			}

			if (corrEffigy)
			{
				player.moveSpeed += 0.1f;
				AllCritBoost(10);
			}

			if (crimEffigy)
			{
				player.allDamage += 0.15f;
				player.statDefense += 10;
			}

			if (badgeOfBraveryRare)
				player.meleeDamage += warBannerBonus;

			// The player's true max life value with Calamity adjustments
			actualMaxLife = player.statLifeMax2;

			if (thirdSageH && !player.dead && healToFull)
			{
				thirdSageH = false;
				player.statLife = actualMaxLife;
			}

			if (manaOverloader)
				player.magicDamage += 0.06f;

			if (rBrain)
			{
				if (player.statLife <= (int)(player.statLifeMax2 * 0.75))
					player.allDamage += 0.1f;
				if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
					player.moveSpeed -= 0.05f;
			}

			if (bloodyWormTooth)
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

			if (dAmulet)
				player.pStone = true;

			if (fBulwark)
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

			if (frostFlare)
			{
				player.resistCold = true;
				player.buffImmune[BuffID.Frostburn] = true;
				player.buffImmune[BuffID.Chilled] = true;
				player.buffImmune[BuffID.Frozen] = true;

				if (player.statLife > (int)(player.statLifeMax2 * 0.75))
					player.allDamage += 0.1f;
				if (player.statLife < (int)(player.statLifeMax2 * 0.25))
					player.statDefense += 20;
			}

			if (vexation)
			{
				if (player.statLife < (int)(player.statLifeMax2 * 0.5))
					player.allDamage += 0.2f;
			}

			if (ataxiaBlaze)
			{
				if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
					player.AddBuff(BuffID.Inferno, 2);
			}

			if (bloodflareThrowing)
			{
				if (player.statLife > (int)(player.statLifeMax2 * 0.8))
				{
					throwingCrit += 5;
					player.statDefense += 30;
				}
				else
					throwingDamage += 0.1f;
			}

			if (bloodflareSummon)
			{
				if (player.statLife >= (int)(player.statLifeMax2 * 0.9))
					player.minionDamage += 0.1f;
				else if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
					player.statDefense += 20;

				if (bloodflareSummonTimer > 0)
					bloodflareSummonTimer--;

				if (player.whoAmI == Main.myPlayer && bloodflareSummonTimer <= 0)
				{
					bloodflareSummonTimer = 900;
					for (int I = 0; I < 3; I++)
					{
						float ai1 = I * 120;
						int projectile = Projectile.NewProjectile(player.Center.X + (float)(Math.Sin(I * 120) * 550), player.Center.Y + (float)(Math.Cos(I * 120) * 550), 0f, 0f,
							ModContent.ProjectileType<GhostlyMine>(), (int)(3750 * player.MinionDamage()), 1f, player.whoAmI, ai1, 0f);
						if (projectile.WithinBounds(Main.maxProjectiles))
							Main.projectile[projectile].Calamity().forceTypeless = true;
					}
				}
			}

			if (yInsignia)
			{
				player.meleeDamage += 0.1f;
				player.lavaMax += 240;
				if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
					player.allDamage += 0.1f;
			}

			if (deepDiver && player.IsUnderwater())
			{
				player.allDamage += 0.15f;
				player.statDefense += 15;
				player.moveSpeed += 0.15f;
			}

			if (abyssalDivingSuit && !player.IsUnderwater())
			{
				float moveSpeedLoss = (3 - abyssalDivingSuitPlateHits) * 0.2f;
				player.moveSpeed -= moveSpeedLoss;
			}

			if (ursaSergeant)
				player.moveSpeed -= 0.15f;

			if (elysianGuard)
				player.moveSpeed -= 0.5f;

			if (coreOfTheBloodGod)
			{
				player.endurance += 0.08f;
				player.allDamage += 0.08f;
			}

			if (godSlayerThrowing)
			{
				if (player.statLife >= player.statLifeMax2)
				{
					throwingCrit += 10;
					throwingDamage += 0.1f;
					throwingVelocity += 0.1f;
				}
			}

			#region Damage Auras
			// Tarragon Summon set bonus life aura
			if (tarraSummon)
			{
				const int FramesPerHit = 80;

				// Constantly increment the timer every frame.
				tarraLifeAuraTimer = (tarraLifeAuraTimer + 1) % FramesPerHit;

				// If the timer rolls over, it's time to deal damage. Only run this code for the client which is wearing the armor.
				if (tarraLifeAuraTimer == 0 && player.whoAmI == Main.myPlayer)
				{
					const int BaseDamage = 120;
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
				navyRodAuraTimer = (navyRodAuraTimer + 1) % FramesPerHit;

				// If the timer rolls over, it's time to deal damage. Only run this code for the client which is holding the fishing rod,
				if (navyRodAuraTimer == 0 && player.whoAmI == Main.myPlayer)
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
							int spark = Projectile.NewProjectile(npc.Center, velocity, ModContent.ProjectileType<EutrophicSpark>(), damage / 2, 0f, player.whoAmI);
							if (spark.WithinBounds(Main.maxProjectiles))
							{
								Main.projectile[spark].Calamity().forceTypeless = true;
								Main.projectile[spark].localNPCHitCooldown = -2;
								Main.projectile[spark].penetrate = 5;
							}
						}
					}
				}
			}

			// Inferno potion boost
			if (ataxiaBlaze && player.inferno)
			{
				const int FramesPerHit = 30;

				// Constantly increment the timer every frame.
				brimLoreInfernoTimer = (brimLoreInfernoTimer + 1) % FramesPerHit;

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
							if (brimLoreInfernoTimer == 0)
								Projectile.NewProjectileDirect(npc.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), damage, 0f, player.whoAmI, i);
						}
					}
				}
			}
			#endregion

			if (royalGel)
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
			}

			if (dukeScales)
			{
				player.buffImmune[ModContent.BuffType<SulphuricPoisoning>()] = true;
				player.buffImmune[BuffID.Poisoned] = true;
				player.buffImmune[BuffID.Venom] = true;
				if (player.statLife <= (int)(player.statLifeMax2 * 0.75))
				{
					player.allDamage += 0.06f;
					AllCritBoost(3);
				}
				if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
				{
					player.allDamage += 0.06f;
					AllCritBoost(3);
				}
				if (player.statLife <= (int)(player.statLifeMax2 * 0.25))
				{
					player.allDamage += 0.06f;
					AllCritBoost(3);
				}
				if (player.lifeRegen < 0)
				{
					player.allDamage += 0.1f;
					AllCritBoost(5);
				}
			}

			if (dArtifact)
				player.allDamage += 0.25f;

			if (trippy)
				player.allDamage += 0.5f;

			if (eArtifact)
			{
				player.manaCost *= 0.85f;
				throwingDamage += 0.15f;
				player.maxMinions += 2;
			}

			if (gArtifact && player.FindBuffIndex(ModContent.BuffType<YharonKindleBuff>()) != -1)
				player.maxMinions += player.ownedProjectileCounts[ModContent.ProjectileType<SonOfYharon>()];

			if (pArtifact)
			{
				if (player.whoAmI == Main.myPlayer)
				{
					if (player.FindBuffIndex(ModContent.BuffType<ProfanedBabs>()) == -1 && !profanedCrystalBuffs)
						player.AddBuff(ModContent.BuffType<ProfanedBabs>(), 3600, true);

					bool crystal = profanedCrystal && !profanedCrystalForce;
					bool summonSet = tarraSummon || bloodflareSummon || silvaSummon || dsSetBonus || omegaBlueSet || fearmongerSet;
					int guardianAmt = 1;

					if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianHealer>()] < guardianAmt)
						Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -6f, ModContent.ProjectileType<MiniGuardianHealer>(), 0, 0f, Main.myPlayer, 0f, 0f);

					if (crystal || minionSlotStat >= 10)
					{
						gDefense = true;

						if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianDefense>()] < guardianAmt)
							Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -3f, ModContent.ProjectileType<MiniGuardianDefense>(), 1, 1f, Main.myPlayer, 0f, 0f);
					}

					if (crystal || summonSet)
					{
						gOffense = true;

						if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianAttack>()] < guardianAmt)
							Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<MiniGuardianAttack>(), 1, 1f, Main.myPlayer, 0f, 0f);
					}
				}
			}

			if (profanedCrystalBuffs && gOffense && gDefense)
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
						player.moveSpeed += 0.1f;
						player.statDefense -= 15;
						player.ignoreWater = true;
					}
					else
					{
						player.moveSpeed -= 0.1f;
						player.endurance += 0.05f;
						player.statDefense += 15;
						player.lifeRegen += 5;
					}
					bool enrage = player.statLife <= (int)(player.statLifeMax2 * 0.5);
					if (!ZoneAbyss) //No abyss memes.
						Lighting.AddLight(player.Center, enrage ? 1.2f : offenseBuffs ? 1f : 0.2f, enrage ? 0.21f : offenseBuffs ? 0.2f : 0.01f, 0);
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

			if (plaguebringerPistons)
			{
				//Spawn bees while sprinting or dashing
				pistonsCounter++;
				if (pistonsCounter % 12 == 0)
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
							if (bee.WithinBounds(Main.maxProjectiles))
								Main.projectile[bee].Calamity().forceTypeless = true;
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
				ModContent.ProjectileType<HowlsHeartTurnipHead>(),
				ModContent.ProjectileType<MiniGuardianAttack>(),
				ModContent.ProjectileType<MiniGuardianDefense>(),
				ModContent.ProjectileType<MiniGuardianHealer>()
			};
			int projAmt = 1;
			for (int i = 0; i < summonDeleteList.Count; i++)
			{
				if (player.ownedProjectileCounts[summonDeleteList[i]] > projAmt)
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

			if (blunderBooster)
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

			if (tesla)
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
					int auraType = ModContent.ProjectileType<TeslaAura>();
					for (int i = 0; i < Main.maxProjectiles; i++)
					{
						if (Main.projectile[i].type != auraType || !Main.projectile[i].active || Main.projectile[i].owner != player.whoAmI)
							continue;

						Main.projectile[i].Kill();
						break;
					}
				}
			}

			if (CryoStone)
			{
				if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<CryonicShield>()] == 0)
					Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<CryonicShield>(), (int)(player.AverageDamage() * 70), 0f, player.whoAmI);
            }
            else if (player.whoAmI == Main.myPlayer)
			{
				int shieldType = ModContent.ProjectileType<CryonicShield>();
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					if (Main.projectile[i].type != shieldType || !Main.projectile[i].active || Main.projectile[i].owner != player.whoAmI)
						continue;

					Main.projectile[i].Kill();
					break;
				}
			}

			if (prismaticLasers > 1800 && player.whoAmI == Main.myPlayer)
			{
				float shootSpeed = 18f;
				int dmg = (int)(30 * player.MagicDamage());
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
					int laser = Projectile.NewProjectile(startPos, velocity, ModContent.ProjectileType<DeathhailBeam>(), dmg, 4f, player.whoAmI, 0f, 0f);
					Main.projectile[laser].localNPCHitCooldown = 5;
					if (laser.WithinBounds(Main.maxProjectiles))
						Main.projectile[laser].Calamity().forceTypeless = true;
				}
				Main.PlaySound(SoundID.Item12, player.Center);
			}
			if (prismaticLasers == 1800)
			{
				//Set the cooldown
				Cooldowns.Add(new PrismaticLaserCooldown(CalamityUtils.SecondsToFrames(30f), player));
			}
			if (prismaticLasers == 1)
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

			if (angelicAlliance && Main.myPlayer == player.whoAmI)
			{
				for (int l = 0; l < Player.MaxBuffs; l++)
				{
					int hasBuff = player.buffType[l];
					if (hasBuff == ModContent.BuffType<DivineBless>())
					{
						angelicActivate = player.buffTime[l];
					}
					if (Cooldowns.Exists(cooldown => cooldown.GetType() == typeof(DivineBlessCooldown)))
					{
						if (player.buffTime[l] == 1)
							Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<AllianceTriangle>(), 0, 0f, player.whoAmI);
					}
				}
				if (angelicActivate == 1)
				{
					int seconds = CalamityUtils.SecondsToFrames(60f);
					Cooldowns.Add(new DivineBlessCooldown(seconds, player));
				}
				if (player.FindBuffIndex(ModContent.BuffType<DivineBless>()) == -1)
					angelicActivate = -1;
			}

			if (theBee)
			{
				if (player.statLife >= player.statLifeMax2)
				{
					float beeBoost = player.endurance / 2f;
					player.allDamage += beeBoost;
				}
			}

			if (badgeOfBravery)
			{
				player.meleeDamage += 0.05f;
				player.meleeCrit += 5;
			}

			if (CalamityConfig.Instance.Proficiency)
				GetStatBonuses();

			// True melee damage bonuses
			double damageAdd = (dodgeScarf ? 0.1 : 0) +
					(evasionScarf ? 0.05 : 0) +
					((aBulwarkRare && aBulwarkRareMeleeBoostTimer > 0) ? 0.5 : 0) +
					(fungalSymbiote ? 0.15 : 0) +
					((player.head == ArmorIDs.Head.MoltenHelmet && player.body == ArmorIDs.Body.MoltenBreastplate && player.legs == ArmorIDs.Legs.MoltenGreaves) ? 0.2 : 0) +
					(player.kbGlove ? 0.1 : 0) +
					(eGauntlet ? 0.1 : 0) +
					(yInsignia ? 0.1 : 0) +
					(badgeOfBraveryRare ? warBannerBonus : 0);
			trueMeleeDamage += damageAdd;

			// Amalgam boosts
			if (Main.myPlayer == player.whoAmI)
			{
				for (int l = 0; l < Player.MaxBuffs; l++)
				{
					int hasBuff = player.buffType[l];
					if ((hasBuff >= BuffID.ObsidianSkin && hasBuff <= BuffID.Gravitation) || hasBuff == BuffID.Tipsy || hasBuff == BuffID.WellFed ||
						hasBuff == BuffID.Honey || hasBuff == BuffID.WeaponImbueVenom || (hasBuff >= BuffID.WeaponImbueCursedFlames && hasBuff <= BuffID.WeaponImbuePoison) ||
						(hasBuff >= BuffID.Mining && hasBuff <= BuffID.Wrath) || (hasBuff >= BuffID.Lovestruck && hasBuff <= BuffID.Warmth) || hasBuff == BuffID.SugarRush ||
						hasBuff == ModContent.BuffType<AbyssalWeapon>() || hasBuff == ModContent.BuffType<AnechoicCoatingBuff>() || hasBuff == ModContent.BuffType<ArmorCrumbling>() ||
						hasBuff == ModContent.BuffType<ArmorShattering>() || hasBuff == ModContent.BuffType<AstralInjectionBuff>() || hasBuff == ModContent.BuffType<BaguetteBuff>() ||
						hasBuff == ModContent.BuffType<BloodfinBoost>() || hasBuff == ModContent.BuffType<BoundingBuff>() || hasBuff == ModContent.BuffType<Cadence>() ||
						hasBuff == ModContent.BuffType<CalciumBuff>() || hasBuff == ModContent.BuffType<CeaselessHunger>() || hasBuff == ModContent.BuffType<DraconicSurgeBuff>() ||
						hasBuff == ModContent.BuffType<GravityNormalizerBuff>() || hasBuff == ModContent.BuffType<HolyWrathBuff>() || hasBuff == ModContent.BuffType<Omniscience>() ||
						hasBuff == ModContent.BuffType<PenumbraBuff>() || hasBuff == ModContent.BuffType<PhotosynthesisBuff>() || hasBuff == ModContent.BuffType<ProfanedRageBuff>() ||
						hasBuff == ModContent.BuffType<Revivify>() || hasBuff == ModContent.BuffType<ShadowBuff>() || hasBuff == ModContent.BuffType<Soaring>() ||
						hasBuff == ModContent.BuffType<SulphurskinBuff>() || hasBuff == ModContent.BuffType<TeslaBuff>() || hasBuff == ModContent.BuffType<TitanScale>() ||
						hasBuff == ModContent.BuffType<TriumphBuff>() || hasBuff == ModContent.BuffType<YharimPower>() || hasBuff == ModContent.BuffType<Zen>() ||
						hasBuff == ModContent.BuffType<Zerg>() || hasBuff == ModContent.BuffType<BloodyMaryBuff>() || hasBuff == ModContent.BuffType<CaribbeanRumBuff>() ||
						hasBuff == ModContent.BuffType<CinnamonRollBuff>() || hasBuff == ModContent.BuffType<EverclearBuff>() || hasBuff == ModContent.BuffType<EvergreenGinBuff>() ||
						hasBuff == ModContent.BuffType<FabsolVodkaBuff>() || hasBuff == ModContent.BuffType<FireballBuff>() || hasBuff == ModContent.BuffType<GrapeBeerBuff>() ||
						hasBuff == ModContent.BuffType<MargaritaBuff>() || hasBuff == ModContent.BuffType<MoonshineBuff>() || hasBuff == ModContent.BuffType<MoscowMuleBuff>() ||
						hasBuff == ModContent.BuffType<RedWineBuff>() || hasBuff == ModContent.BuffType<RumBuff>() || hasBuff == ModContent.BuffType<ScrewdriverBuff>() ||
						hasBuff == ModContent.BuffType<StarBeamRyeBuff>() || hasBuff == ModContent.BuffType<TequilaBuff>() || hasBuff == ModContent.BuffType<TequilaSunriseBuff>() ||
						hasBuff == ModContent.BuffType<Trippy>() || hasBuff == ModContent.BuffType<VodkaBuff>() || hasBuff == ModContent.BuffType<WhiskeyBuff>() ||
						hasBuff == ModContent.BuffType<WhiteWineBuff>())
					{
						if (amalgam)
						{
							// Every other frame, increase the buff timer by one frame. Thus, the buff lasts twice as long.
							if (player.miscCounter % 2 == 0)
								player.buffTime[l] += 1;

							// Buffs will not go away when you die, to prevent wasting potions.
							if (!Main.persistentBuff[hasBuff])
								Main.persistentBuff[hasBuff] = true;
						}
						else
						{
							// Reset buff persistence if Amalgam is removed.
							if (Main.persistentBuff[hasBuff])
								Main.persistentBuff[hasBuff] = false;
						}
					}
				}
			}

			// Laudanum boosts
			if (laudanum)
			{
				if (Main.myPlayer == player.whoAmI)
				{
					for (int l = 0; l < Player.MaxBuffs; l++)
					{
						int hasBuff = player.buffType[l];
						if (hasBuff == ModContent.BuffType<ArmorCrunch>() || hasBuff == ModContent.BuffType<WarCleave>() || hasBuff == BuffID.Obstructed ||
							hasBuff == BuffID.Ichor || hasBuff == BuffID.Chilled || hasBuff == BuffID.BrokenArmor || hasBuff == BuffID.Weak ||
							hasBuff == BuffID.Slow || hasBuff == BuffID.Confused || hasBuff == BuffID.Blackout || hasBuff == BuffID.Darkness)
						{
							// Every other frame, increase the buff timer by one frame. Thus, the buff lasts twice as long.
							if (player.miscCounter % 2 == 0)
								player.buffTime[l] += 1;
						}

						// See later as Laud cancels out the normal effects
						if (hasBuff == ModContent.BuffType<ArmorCrunch>())
						{
							// +15 defense
							player.statDefense += ArmorCrunch.DefenseReduction;
						}
						if (hasBuff == ModContent.BuffType<WarCleave>())
						{
							// +10% damage reduction
							player.endurance += 0.1f;
						}

						switch (hasBuff)
						{
							case BuffID.Obstructed:
								player.headcovered = false;
								player.statDefense += 50;
								player.allDamage += 0.5f;
								AllCritBoost(25);
								break;
							case BuffID.Ichor:
								player.statDefense += 40;
								break;
							case BuffID.Chilled:
								player.chilled = false;
								player.moveSpeed *= 1.3f;
								break;
							case BuffID.BrokenArmor:
								player.brokenArmor = false;
								player.statDefense += (int)(player.statDefense * 0.25);
								break;
							case BuffID.Weak:
								player.meleeDamage += 0.151f;
								player.statDefense += 14;
								player.moveSpeed += 0.3f;
								break;
							case BuffID.Slow:
								player.slow = false;
								player.moveSpeed *= 1.5f;
								break;
							case BuffID.Confused:
								player.confused = false;
								player.statDefense += 30;
								player.allDamage += 0.25f;
								AllCritBoost(10);
								break;
							case BuffID.Blackout:
								player.blackout = false;
								player.statDefense += 30;
								player.allDamage += 0.25f;
								AllCritBoost(10);
								break;
							case BuffID.Darkness:
								player.blind = false;
								player.statDefense += 15;
								player.allDamage += 0.1f;
								AllCritBoost(5);
								break;
						}
					}
				}
			}

			// Draedon's Heart bonus
			if (draedonsHeart)
			{
				if (player.StandingStill() && player.itemAnimation == 0)
					player.statDefense += (int)(player.statDefense * 0.75);
			}

			// Endurance reductions
			EnduranceReductions();

			if (spectralVeilImmunity > 0)
			{
				int numDust = 2;
				for (int i = 0; i < numDust; i++)
				{
					int dustIndex = Dust.NewDust(player.position, player.width, player.height, 21, 0f, 0f);
					Dust dust = Main.dust[dustIndex];
					dust.position.X += Main.rand.Next(-5, 6);
					dust.position.Y += Main.rand.Next(-5, 6);
					dust.velocity *= 0.2f;
					dust.noGravity = true;
					dust.noLight = true;
				}
			}

			// Gem Tech stats based on gems.
			GemTechState.ProvideGemBoosts();
		}
		#endregion

		#region Defense Effects
		private void DefenseEffects()
		{
			//
			// Defense Damage
			//
			// Current defense damage can be calculated at any time using the accessor property CurrentDefenseDamage.
			// However, it CANNOT be written to. You can only set the total defense damage.
			// CalamityPlayer has a function called DealDefenseDamage to handle everything for you, when dealing defense damage.
			//
			// The player's current recovery through defense damage is tracked through two frame counts:
			// defenseDamageRecoveryFrames = How many more frames the player will still be recovering from defense damage
			// totalDefenseDamageRecoveryFrames = The total timer for defense damage recovery that the player is undergoing
			//
			// Defense damage heals over a fixed time (CalamityPlayer.DefenseDamageRecoveryTime).
			// This is independent of how much defense the player started with, or how much they lost.
			// If hit again while recovering from defense damage, that fixed time is ADDED to the current recovery timer
			// (in addition to the player taking more defense damage, of course).
			if (totalDefenseDamage > 0)
			{
				// Defense damage is capped at your maximum defense, no matter what.
				if (totalDefenseDamage > player.statDefense)
					totalDefenseDamage = player.statDefense;

				// You cannot begin recovering from defense damage until your iframes wear off.
				bool hasIFrames = false;
				for (int i = 0; i < player.hurtCooldowns.Length; i++)
					if (player.hurtCooldowns[i] > 0)
						hasIFrames = true;

				// Delay before defense damage recovery can start. While this delay is ticking down, defense damage doesn't recover at all.
				if (!hasIFrames && defenseDamageDelayFrames > 0)
					--defenseDamageDelayFrames;

				// Once the delay is up, defense damage recovery occurs.
				else if (defenseDamageDelayFrames <= 0)
				{
					// Make one frame's worth of progress towards recovery.
					--defenseDamageRecoveryFrames;

					// If completely recovered, reset defense damage to nothing.
					if (defenseDamageRecoveryFrames <= 0)
					{
						totalDefenseDamage = 0;
						defenseDamageRecoveryFrames = 0;
						totalDefenseDamageRecoveryFrames = DefenseDamageBaseRecoveryTime;
						defenseDamageDelayFrames = 0;
					}
				}

				// Get current amount of defense damage to apply this frame.
				int currentDefenseDamage = CurrentDefenseDamage;

				// Apply DR Damage.
				//
				// DR Damage is applied at exactly the same ratio as defense damage;
				// if you lose half your defense to defense damage, you also lose half your DR.
				// This is applied first because the math would be wrong if the player's defense was already reduced by defense damage.
				if (player.statDefense > 0 && player.endurance > 0f)
				{
					float drDamageRatio = currentDefenseDamage / (float)player.statDefense;
					player.endurance *= 1f - drDamageRatio;
				}

				// Apply defense damage
				player.statDefense -= currentDefenseDamage;
			}

			// Bloodflare Core's defense reduction
			// This is intentionally after defense damage.
			// This defense still comes back over time if you take off Bloodflare Core while you're missing defense.
			// However, removing the item means you won't get healed as the defense comes back.
			ref int lostDef = ref bloodflareCoreLostDefense;
			if (lostDef > 0)
			{
				// Defense regeneration occurs every four frames while defense is missing
				if (player.miscCounter % 4 == 0)
				{
					--lostDef;
					if (bloodflareCore)
					{
						player.statLife += 1;
						player.HealEffect(1, false);

						// Produce an implosion of blood themed dust so it's obvious an effect is occurring
						for (int i = 0; i < 3; ++i)
						{
							Vector2 offset = Main.rand.NextVector2Unit() * Main.rand.NextFloat(23f, 33f);
							Vector2 dustPos = player.Center + offset;
							Vector2 dustVel = offset * -0.08f;
							Dust d = Dust.NewDustDirect(dustPos, 0, 0, 90, 0.08f, 0.08f);
							d.velocity = dustVel;
							d.noGravity = true;
						}
					}
				}

				// Actually apply Bloodflare Core defense reduction
				player.statDefense -= lostDef;
			}

			// Defense can never be reduced below zero, no matter what
			if (player.statDefense < 0)
				player.statDefense = 0;

			// Multiplicative defense reductions.
			// These are done last because they need to be after the defense lower cap at 0.
			if (fabsolVodka)
			{
				if (player.statDefense > 0)
					player.statDefense -= (int)(player.statDefense * 0.1);
			}

			if (vodka)
			{
				if (player.statDefense > 0)
					player.statDefense -= (int)(player.statDefense * 0.05);
			}

			if (grapeBeer)
			{
				if (player.statDefense > 0)
					player.statDefense -= (int)(player.statDefense * 0.03);
			}

			if (rum)
			{
				if (player.statDefense > 0)
					player.statDefense -= (int)(player.statDefense * 0.05);
			}

			if (whiskey)
			{
				if (player.statDefense > 0)
					player.statDefense -= (int)(player.statDefense * 0.05);
			}

			if (everclear)
			{
				if (player.statDefense > 0)
					player.statDefense -= (int)(player.statDefense * 0.3);
			}

			if (bloodyMary)
			{
				if (Main.bloodMoon)
				{
					if (player.statDefense > 0)
						player.statDefense -= (int)(player.statDefense * 0.04);
				}
			}

			if (caribbeanRum)
			{
				if (player.statDefense > 0)
					player.statDefense -= (int)(player.statDefense * 0.1);
			}

			if (cinnamonRoll)
			{
				if (player.statDefense > 0)
					player.statDefense -= (int)(player.statDefense * 0.1);
			}

			if (margarita)
			{
				if (player.statDefense > 0)
					player.statDefense -= (int)(player.statDefense * 0.06);
			}

			if (starBeamRye)
			{
				if (player.statDefense > 0)
					player.statDefense -= (int)(player.statDefense * 0.06);
			}

			if (whiteWine)
			{
				if (player.statDefense > 0)
					player.statDefense -= (int)(player.statDefense * 0.06);
			}
		}
		#endregion

		#region Limits
		private void Limits()
		{
			//not sure where else this should go
			if (forbiddenCirclet)
			{
				float rogueDmg = player.thrownDamage + throwingDamage - 1f;
				float minionDmg = player.minionDamage;
				if (minionDmg < rogueDmg)
				{
					player.minionDamage = rogueDmg;
				}
				if (rogueDmg < minionDmg)
				{
					throwingDamage = minionDmg - player.thrownDamage + 1f;
				}
			}

			// 10% is converted to 9%, 25% is converted to 20%, 50% is converted to 33%, 75% is converted to 43%, 100% is converted to 50%
			if (player.endurance > 0f)
				player.endurance = 1f - (1f / (1f + player.endurance));
			
			// Do not apply reduced aggro if there are any bosses alive and it's singleplayer
			if (areThereAnyDamnBosses && Main.netMode == NetmodeID.SinglePlayer)
			{
				if (player.aggro < 0)
					player.aggro = 0;
			}
		}
		#endregion

		#region Endurance Reductions
		private void EnduranceReductions()
		{
			if (vHex)
				player.endurance -= 0.1f;

			if (irradiated)
					player.endurance -= 0.1f;

			if (corrEffigy)
				player.endurance -= 0.05f;
		}
		#endregion

		#region Stat Meter
		private void UpdateStatMeter()
		{
			float allDamageStat = player.allDamage - 1f;
			damageStats[0] = (int)((player.meleeDamage + allDamageStat - 1f) * 100f);
			damageStats[1] = (int)((player.rangedDamage + allDamageStat - 1f) * 100f);
			damageStats[2] = (int)((player.magicDamage + allDamageStat - 1f) * 100f);
			damageStats[3] = (int)((player.minionDamage + allDamageStat - 1f) * 100f);
			damageStats[4] = (int)((throwingDamage + allDamageStat - 1f) * 100f);
			damageStats[5] = (int)(trueMeleeDamage * 100D);
			critStats[0] = player.meleeCrit;
			critStats[1] = player.rangedCrit;
			critStats[2] = player.magicCrit;
			critStats[3] = player.thrownCrit + throwingCrit;
			ammoReductionRanged = (int)(100f *
				(player.ammoBox ? 0.8f : 1f) *
				(player.ammoPotion ? 0.8f : 1f) *
				(player.ammoCost80 ? 0.8f : 1f) *
				(player.ammoCost75 ? 0.75f : 1f) *
				rangedAmmoCost);
			ammoReductionRogue = (int)(throwingAmmoCost * 100);
			// Cancel out defense damage for the purposes of the stat meter.
			defenseStat = player.statDefense + CurrentDefenseDamage;
			DRStat = (int)(player.endurance * 100f);
			meleeSpeedStat = (int)((1f - player.meleeSpeed) * (100f / player.meleeSpeed));
			manaCostStat = (int)(player.manaCost * 100f);
			rogueVelocityStat = (int)((throwingVelocity - 1f) * 100f);

			// Max stealth 1f is actually "100 stealth", so multiply by 100 to get visual stealth number.
			stealthStat = (int)(rogueStealthMax * 100f);
			// Then divide by 3, because it takes 3 seconds to regen full stealth.
			// Divide by 3 again for moving, because it recharges at 1/3 speed (so divide by 9 overall).
			// Then multiply by stealthGen variables, which start at 1f and increase proportionally to your boosts.
			standingRegenStat = (rogueStealthMax * 100f / 3f) * stealthGenStandstill;
			movingRegenStat = (rogueStealthMax * 100f / 9f) * stealthGenMoving * stealthAcceleration;

			minionSlotStat = player.maxMinions;
			manaRegenStat = player.manaRegen;
			armorPenetrationStat = player.armorPenetration;
			moveSpeedStat = (int)((player.moveSpeed - 1f) * 100f);
			wingFlightTimeStat = player.wingTimeMax / 60f;
			float trueJumpSpeedBoost = player.jumpSpeedBoost + 
				(player.wereWolf ? 0.2f : 0f) + 
				(player.jumpBoost ? 0.75f : 0f);
			jumpSpeedStat = trueJumpSpeedBoost * 20f;
			rageDamageStat = (int)(100D * RageDamageBoost);
			adrenalineDamageStat = (int)(100D * player.Calamity().GetAdrenalineDamage());
			int extraAdrenalineDR = 0 +
				(adrenalineBoostOne ? 5 : 0) +
				(adrenalineBoostTwo ? 5 : 0) +
				(adrenalineBoostThree ? 5 : 0);
			adrenalineDRStat = 50 + extraAdrenalineDR;
		}
		#endregion

		#region Double Jumps
		private void DoubleJumps()
		{
			if (CalamityUtils.CountHookProj() > 0 || player.sliding || player.autoJump && player.justJumped)
			{
				jumpAgainSulfur = true;
				jumpAgainStatigel = true;
				return;
			}

			bool mountCheck = true;
			if (player.mount != null && player.mount.Active)
				mountCheck = player.mount.BlockExtraJumps;
			bool carpetCheck = true;
			if (player.carpet)
				carpetCheck = player.carpetTime <= 0 && player.canCarpet;
			bool wingCheck = player.wingTime == player.wingTimeMax || player.autoJump;
			Tile tileBelow = CalamityUtils.ParanoidTileRetrieval((int)(player.Bottom.X / 16f), (int)(player.Bottom.Y / 16f));

			if (player.position.Y == player.oldPosition.Y && wingCheck && mountCheck && carpetCheck && tileBelow.IsTileSolidGround())
			{
				jumpAgainSulfur = true;
				jumpAgainStatigel = true;
			}
		}
		#endregion

		#region Mouse Item Checks
		public void CheckIfMouseItemIsSchematic()
		{
			if (Main.myPlayer != player.whoAmI)
				return;

			bool shouldSync = false;

			// ActiveItem doesn't need to be checked as the other possibility involves
			// the item in question already being in the inventory.
			if (Main.mouseItem != null && !Main.mouseItem.IsAir)
			{
				if (Main.mouseItem.type == ModContent.ItemType<EncryptedSchematicSunkenSea>() && !RecipeUnlockHandler.HasFoundSunkenSeaSchematic)
				{
					RecipeUnlockHandler.HasFoundSunkenSeaSchematic = true;
					shouldSync = true;
				}

				if (Main.mouseItem.type == ModContent.ItemType<EncryptedSchematicPlanetoid>() && !RecipeUnlockHandler.HasFoundPlanetoidSchematic)
				{
					RecipeUnlockHandler.HasFoundPlanetoidSchematic = true;
					shouldSync = true;
				}

				if (Main.mouseItem.type == ModContent.ItemType<EncryptedSchematicJungle>() && !RecipeUnlockHandler.HasFoundJungleSchematic)
				{
					RecipeUnlockHandler.HasFoundJungleSchematic = true;
					shouldSync = true;
				}

				if (Main.mouseItem.type == ModContent.ItemType<EncryptedSchematicHell>() && !RecipeUnlockHandler.HasFoundHellSchematic)
				{
					RecipeUnlockHandler.HasFoundHellSchematic = true;
					shouldSync = true;
				}

				if (Main.mouseItem.type == ModContent.ItemType<EncryptedSchematicIce>() && !RecipeUnlockHandler.HasFoundIceSchematic)
				{
					RecipeUnlockHandler.HasFoundIceSchematic = true;
					shouldSync = true;
				}
			}

			if (shouldSync)
				CalamityNetcode.SyncWorld();
		}
		#endregion

		#region Potion Handling
		private void HandlePotions()
		{
			if (potionTimer > 0)
				potionTimer--;
			if (potionTimer > 0 && player.potionDelay == 0)
				player.potionDelay = potionTimer;
			if (potionTimer == 1)
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

					if (player.potionDelay > 0 || potionTimer > 0)
						break;
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
