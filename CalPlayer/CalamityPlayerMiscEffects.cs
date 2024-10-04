using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Balancing;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Placeables;
using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Summon;
using CalamityMod.Cooldowns;
using CalamityMod.CustomRecipes;
using CalamityMod.DataStructures;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Armor.Bloodflare;
using CalamityMod.Items.Armor.Brimflame;
using CalamityMod.Items.Armor.DesertProwler;
using CalamityMod.Items.Armor.Hydrothermic;
using CalamityMod.Items.Armor.LunicCorps;
using CalamityMod.Items.Armor.OmegaBlue;
using CalamityMod.Items.Armor.Prismatic;
using CalamityMod.Items.Armor.Silva;
using CalamityMod.Items.Armor.Tarragon;
using CalamityMod.Items.Armor.Wulfrum;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Mounts.Minecarts;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Other;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems;
using CalamityMod.Tiles.Abyss.AbyssAmbient;
using CalamityMod.Tiles.FurnitureAuric;
using CalamityMod.Tiles.Ores;
using CalamityMod.UI;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.GameInput;
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

            // Give the player a 24% jump speed boost while wings are equipped, otherwise grant 4% more jump speed so that players can jump 7 tiles high
            if (Player.wingsLogic > 0)
                Player.jumpSpeedBoost += 1.2f;
            else if (CalamityConfig.Instance.FasterJumpSpeed)
                Player.jumpSpeedBoost += 0.2f;

            // Decrease the counter on Fearmonger set turbo regeneration
            if (fearmongerRegenFrames > 0)
                fearmongerRegenFrames--;

            // Go through the old positions for the player.
            for (int i = Player.Calamity().OldPositions.Length - 1; i > 0; i--)
            {
                if (OldPositions[i - 1] == Vector2.Zero)
                    OldPositions[i - 1] = Player.position;

                OldPositions[i] = OldPositions[i - 1];
            }
            OldPositions[0] = Player.position;

            // Tile effects for touching tiles
            HandleTileEffects();

            // Hurt the nearest NPC to the mouse if using the burning mouse.
            if (blazingCursorDamage || blazingCursorVisuals)
                HandleBlazingMouseEffects();

            // Revengeance effects
            RevengeanceModeMiscEffects();

            // Rippers
            UpdateRippers();

            // Abyss effects
            AbyssEffects();

            // Misc effects, because I don't know what else to call it
            MiscEffects();

            // Standing still effects
            StandingStillEffects();

            // Other buff effects
            OtherBuffEffects();

            // Update energy shields
            EnergyShields();

            // Defense manipulation (Mostly defense damage, but also other defense changes)
            DefenseEffects();

            // Limits
            Limits();

            // This is used to increase horizontal velocity based on the player's movement speed stat.
            moveSpeedBonus = Player.moveSpeed - 1f;

            // Potions (Quick Buff && Potion Sickness)
            HandlePotions();

            // Display the Music Mod Reminder text in chat.
            HandleTextChatMessages();

            // Check if schematics are present on the mouse, for the sake of registering their recipes.
            CheckIfMouseItemIsSchematic();

            // Handle Androomba's Right Click function
            AndroombaRightClick();

            // Update all particle sets for items.
            // This must be done here instead of in the item logic because these sets are not properly instanced
            // in the global classes. Attempting to update them there will cause multiple updates to one set for multiple items.
            CalamityGlobalItem.UpdateAllParticleSets();
            BrokenBiomeBlade.UpdateAllParticleSets();
            TrueBiomeBlade.UpdateAllParticleSets();
            OmegaBiomeBlade.UpdateAllParticleSets();

            // Update the gem tech armor set.
            GemTechState.Update();

            // Regularly sync player stats & mouse control info during multiplayer
            if (Player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
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
                spiritOriginConvertedCrit = (int)(Player.GetTotalCritChance<RangedDamageClass>() - 4);
                Player.GetCritChance<RangedDamageClass>() = -spiritOriginConvertedCrit;
            }

            if (Player.ActiveItem().type == ModContent.ItemType<GaelsGreatsword>())
                heldGaelsLastFrame = true;

            if (Player.ActiveItem().type != ModContent.ItemType<SaharaSlicers>())
                saharaSlicersBolts = 0;

            // De-equipping Gael's Greatsword deletes all rage.
            else if (heldGaelsLastFrame)
            {
                heldGaelsLastFrame = false;
                rage = 0f;
            }

            // De-equipping Draedon's Heart deletes all Adrenaline.
            if (!draedonsHeart && hadNanomachinesLastFrame)
            {
                hadNanomachinesLastFrame = false;
                adrenaline = 0f;
            }

            // Apply stealth damage to rogue.
            Player.GetDamage<RogueDamageClass>() += stealthDamage;

            //Slow the player down after any other speed modifiers might have been applied.
            //Todo - Move this back to the wulfrum set class whenever statmodifiers are implemented for stats other than damage
            if (WulfrumHat.PowerModeEngaged(Player, out _))
                Player.moveSpeed *= 0.8f;

            if (gShell)
            {
                //reduce player dash velocity as long as you didn't just get hit
                if (Player.dashDelay == -1 && giantShellPostHit == 0)
                {
                    if (!HasReducedDashFirstFrame)
                    {
                        Player.velocity.X *= 0.9f;
                        HasReducedDashFirstFrame = true;
                    }
                }
                else
                    HasReducedDashFirstFrame = false;
            }

            if (lAmbergris)
            {
                if (Player.miscCounter % 3 == 2 && Player.dashDelay > 0) // Reduced dash cooldown by 33%
                    Player.dashDelay--;

                if (Player.dashDelay == -1)// TODO: prevent working with special dashes, this was inconsitent with my old solution so I didn't keep it. not huge deal)
                {
                    if (!HasIncreasedDashFirstFrame)
                    {
                        Player.velocity.X *= 1.2f;
                        int damage = Player.ApplyArmorAccDamageBonusesTo(Player.GetBestClassDamage().ApplyTo(75));

                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + Player.velocity * 1.5f, Vector2.Zero, ModContent.ProjectileType<LeviAmberDash>(), damage, 20f, Player.whoAmI);
                        HasIncreasedDashFirstFrame = true;
                    }
                    float numberOfDusts = 10f;
                    float rotFactor = 180f / numberOfDusts;
                    for (int i = 0; i < numberOfDusts; i++)
                    {
                        float rot = MathHelper.ToRadians(i * rotFactor);
                        Vector2 offset = new Vector2(Player.velocity.X * Player.direction * 0.7f + 6f, 0).RotatedBy(rot * Main.rand.NextFloat(4f, 5f));
                        Vector2 velOffset = Vector2.Zero;
                        Dust dust = Dust.NewDustPerfect(Player.Center + offset + Player.velocity, Main.rand.NextBool() ? 160 : 307, new Vector2(velOffset.X, velOffset.Y));
                        dust.noGravity = true;
                        dust.velocity = velOffset;
                        dust.alpha = 100;
                        dust.scale = (Player.velocity.X * Player.direction * 0.08f);
                    }
                    if (Player.miscCounter % 4 == 0)
                    {
                        float sparkscale = (Player.velocity.X * Player.direction * 0.07f);
                        Vector2 SparkVelocity1 = Player.velocity.RotatedBy(Player.direction * 2, default) * 0.1f - Player.velocity / 2f;
                        LineParticle spark = new LineParticle(Player.Center + Player.velocity.RotatedBy(2f * Player.direction) * 1.5f, SparkVelocity1, false, Main.rand.Next(11, 13), sparkscale, Main.rand.NextBool() ? Color.DarkTurquoise : Color.DodgerBlue);
                        GeneralParticleHandler.SpawnParticle(spark);
                        Vector2 SparkVelocity2 = Player.velocity.RotatedBy(Player.direction * -2, default) * 0.1f - Player.velocity / 2f;
                        LineParticle spark2 = new LineParticle(Player.Center + Player.velocity.RotatedBy(-2f * Player.direction) * 1.5f, SparkVelocity2, false, Main.rand.Next(11, 13), sparkscale, Main.rand.NextBool() ? Color.DarkTurquoise : Color.DodgerBlue);
                        GeneralParticleHandler.SpawnParticle(spark2);
                    }
                    if (Player.miscCounter % 4 == 0 && Player.velocity != Vector2.Zero) //every other frame spawn the hitbox
                    {
                        int damage = Player.ApplyArmorAccDamageBonusesTo(Player.GetBestClassDamage().ApplyTo(90));
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + Player.velocity * 1.5f, Vector2.Zero, ModContent.ProjectileType<LeviAmberDash>(), damage, 0f, Player.whoAmI);
                    }
                }
                else
                    HasIncreasedDashFirstFrame = false;
            }

            if (Pauldron)
            {
                if (Player.dashDelay == -1)// TODO: prevent working with special dashes, this was inconsitent with my old solution so I didn't keep it. not huge deal)
                {
                    Player.endurance += 0.1f;
                    if (!HasReducedDashFirstFrame) // Dash isn't reduced, this is used to determine the first frame of dashing
                    {
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { Volume = 0.4f, PitchVariance = 0.4f }, Player.Center);
                        int damage = Player.ApplyArmorAccDamageBonusesTo(Player.GetBestClassDamage().ApplyTo(67));

                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + Player.velocity * 1.5f, Vector2.Zero, ModContent.ProjectileType<PauldronDash>(), damage, 16f, Player.whoAmI);
                        HasReducedDashFirstFrame = true;
                    }
                    float numberOfDusts = 10f;
                    float rotFactor = 180f / numberOfDusts;
                    for (int i = 0; i < numberOfDusts; i++)
                    {
                        float rot = MathHelper.ToRadians(i * rotFactor);
                        Vector2 offset = new Vector2(MathF.Min(Player.velocity.X * Player.direction * 0.7f + 8f, 20f), 0).RotatedBy(rot * Main.rand.NextFloat(4f, 5f));
                        Vector2 velOffset = Vector2.Zero;
                        Dust dust = Dust.NewDustPerfect(Player.Center + offset + Player.velocity, Main.rand.NextBool() ? 35 : 127, new Vector2(velOffset.X, velOffset.Y));
                        dust.noGravity = true;
                        dust.velocity = velOffset;
                        dust.alpha = 100;
                        dust.scale = MathF.Min(Player.velocity.X * Player.direction * 0.08f, 1.2f);
                    }
                    float sparkscale = MathF.Min(Player.velocity.X * Player.direction * 0.08f, 1.2f);
                    Vector2 SparkVelocity1 = Player.velocity.RotatedBy(Player.direction * -3, default) * 0.1f - Player.velocity / 2f;
                    SparkParticle spark = new SparkParticle(Player.Center + Player.velocity.RotatedBy(2f * Player.direction) * 1.5f, SparkVelocity1, false, Main.rand.Next(11, 13), sparkscale, Main.rand.NextBool() ? Color.DarkOrange : Color.OrangeRed);
                    GeneralParticleHandler.SpawnParticle(spark);
                    Vector2 SparkVelocity2 = Player.velocity.RotatedBy(Player.direction * 3, default) * 0.1f - Player.velocity / 2f;
                    SparkParticle spark2 = new SparkParticle(Player.Center + Player.velocity.RotatedBy(-2f * Player.direction) * 1.5f, SparkVelocity2, false, Main.rand.Next(11, 13), sparkscale, Main.rand.NextBool() ? Color.DarkOrange : Color.OrangeRed);
                    GeneralParticleHandler.SpawnParticle(spark2);

                    if (Player.miscCounter % 6 == 0 && Player.velocity != Vector2.Zero)
                    {
                        int damage = Player.ApplyArmorAccDamageBonusesTo(Player.GetBestClassDamage().ApplyTo(170));
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + Player.velocity * 1.5f, Vector2.Zero, ModContent.ProjectileType<PauldronDash>(), damage, 10f, Player.whoAmI);
                    }
                }
                else
                    HasReducedDashFirstFrame = false;
            }

            if (tortShell)
            {
                //reduce player dash velocity as long as you didn't just get hit
                if (Player.dashDelay == -1 && tortShellPostHit == 0)
                {
                    if (!HasReducedDashFirstFrame)
                    {
                        Player.velocity.X *= 0.9f;
                        HasReducedDashFirstFrame = true;
                    }
                }
                else
                    HasReducedDashFirstFrame = false;
            }

            if (oceanCrest)
            {
                bool surface = Player.Center.Y < Main.worldSurface * 16.0;
                bool GetEffects = ((Main.raining && surface) || Player.dripping || (Player.wet && !Player.lavaWet && !Player.honeyWet));
                if (GetEffects)
                {
                    if (oceanCrestTimer < 300)
                        oceanCrestTimer += 5;
                    if (Player.StandingStill(0.1f) && !ZoneAbyss && Player.breath < 201 && Player.miscCounter % 2 == 0)
                        Player.breath += 1;
                }
                else
                    if (oceanCrestTimer > 0)
                    oceanCrestTimer--;

                if (oceanCrestTimer > 0 || GetEffects)
                    Player.pickSpeed -= 0.15f; // 15% mining speed

                Vector3 Light = new Vector3(0.090f, 0.180f, 0.200f);
                Lighting.AddLight(Player.Center, Light * (0.55f + (oceanCrestTimer * 0.0035f)));
            }
        }
        #endregion

        #region Revengeance Effects
        private void RevengeanceModeMiscEffects()
        {
            if (CalamityWorld.revenge)
            {
                if (Player.whoAmI == Main.myPlayer)
                {
                    // Hallowed Armor nerf
                    if (Player.onHitDodge)
                    {
                        for (int l = 0; l < Player.MaxBuffs; l++)
                        {
                            int hasBuff = Player.buffType[l];
                            if (Player.buffTime[l] > 360 && hasBuff == BuffID.ShadowDodge)
                                Player.buffTime[l] = 360;
                        }
                    }

                    // Immunity Frames nerf
                    int immuneTimeLimit = 150;
                    if (Player.immuneTime > immuneTimeLimit)
                        Player.immuneTime = immuneTimeLimit;

                    for (int k = 0; k < Player.hurtCooldowns.Length; k++)
                    {
                        if (Player.hurtCooldowns[k] > immuneTimeLimit)
                            Player.hurtCooldowns[k] = immuneTimeLimit;
                    }
                }
            }
        }

        private void UpdateRippers()
        {
            #region Rage
            // Figure out Rage's current duration based on boosts.
            if (rageBoostOne)
                RageDuration += BalancingConstants.RageDurationPerBooster;
            if (rageBoostTwo)
                RageDuration += BalancingConstants.RageDurationPerBooster;
            if (rageBoostThree)
                RageDuration += BalancingConstants.RageDurationPerBooster;

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

                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.type == NPCID.None || !npc.IsAnEnemy() || !npc.Calamity().ProvidesProximityRage)
                        continue;

                    // Take the longer of the two directions for the NPC's hitbox to be generous.
                    float generousHitboxWidth = Math.Max(npc.Hitbox.Width / 2f, npc.Hitbox.Height / 2f);
                    float hitboxEdgeDist = npc.Distance(Player.Center) - generousHitboxWidth;

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
                rageDiff -= rageMax / BalancingConstants.RageFadeTime;

            // Apply the rage change and cap rage in both directions.
            // Changes are only applied if the Rage mechanic is available.
            if (RageEnabled)
            {
                rage += rageDiff;
                if (rage < 0f)
                    rage = 0f;

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
                    if (Player.whoAmI == Main.myPlayer && fullRageSoundCountdownTimer <= 0)
                        SoundEngine.PlaySound(RageFilledSound);

                    // Regardless of whether a sound was played this time Rage reached 100%, set the delay before the sound can be played again.
                    fullRageSoundCountdownTimer = FullRageSoundDelay;
                }
            }
            #endregion

            #region Adrenaline
            // This is how much Adrenaline will be changed by this frame.
            float adrenalineDiff = 0;
            bool wofAndNotHell = Main.wofNPCIndex >= 0 && Player.position.Y < (float)(Main.UnderworldLayer * 16);

            // If Adrenaline Mode is currently active, you smoothly lose all adrenaline over the duration.
            if (adrenalineModeActive)
            {
                adrenalineDiff = -adrenalineMax / AdrenalineDuration;

                // If using Draedon's Heart, you get healing instead of damage.
                if (draedonsHeart)
                {
                    Player.statLife += DraedonsHeart.NanomachinesHealPerFrame;
                    if (Player.statLife >= Player.statLifeMax2)
                        Player.statLife = Player.statLifeMax2;

                    // Old Draedon's Heart dust effect from its standing still regen. Works just fine.
                    int dustID = DustID.TerraBlade;
                    {
                        int regen = Dust.NewDust(Player.position, Player.width, Player.height, dustID, 0f, 0f, 200, default, 1f);
                        Main.dust[regen].noGravity = true;
                        Main.dust[regen].fadeIn = 1.3f;
                        Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                        Main.dust[regen].velocity = velocity;
                        velocity.Normalize();
                        velocity *= 34f;
                        Main.dust[regen].position = Player.Center - velocity;
                    }
                }
            }
            else
            {
                // If any boss is alive, you gain adrenaline smoothly.
                // EXCEPTION: Wall of Flesh is alive and you are not in hell. Then you don't get anything.
                if (areThereAnyDamnBosses && !wofAndNotHell)
                    adrenalineDiff += adrenalineMax / AdrenalineChargeTime;

                // If you aren't actively in a boss fight, adrenaline rapidly fades away.
                // If Boss Rush is active, adrenaline is paused between boss fights and during the Exo Mechs "Make your choice".
                else if (!BossRushEvent.BossRushActive)
                    adrenalineDiff = -adrenalineMax / AdrenalineFadeTime;
            }

            // Adjustments to how fast Adrenaline charges
            if (adrenalineDiff > 0f)
            {
                // Stress Pills make Adrenaline charge 20% faster (meaning it takes 83.333% standard time to charge it).
                if (stressPills)
                    adrenalineDiff *= 1.2f;
            }


            // Apply the adrenaline change and cap adrenaline in both directions.
            // Changes are only applied if the Adrenaline mechanic is available.
            if (AdrenalineEnabled && adrenalinePauseTimer == 0)
            {
                adrenaline += adrenalineDiff;
                if (adrenaline < 0f)
                    adrenaline = 0f;

                if (adrenaline >= adrenalineMax)
                {
                    adrenaline = adrenalineMax;

                    // Play a sound when the Adrenaline Meter is full
                    if (Player.whoAmI == Main.myPlayer && playFullAdrenalineSound)
                    {
                        playFullAdrenalineSound = false;
                        SoundEngine.PlaySound(AdrenalineFilledSound);
                    }
                }
                else
                    playFullAdrenalineSound = true;
            }

            if (adrenalinePauseTimer > 0)
                adrenalinePauseTimer--;
            #endregion
        }
        #endregion

        #region Misc Effects
        private void HandleTileEffects()
        {
            int astralOreID = ModContent.TileType<AstralOre>();
            int auricOreID = ModContent.TileType<AuricOre>();
            int auricRepulserID = ModContent.TileType<AuricRepulserPanelTile>();
            int scoriaOreID = ModContent.TileType<ScoriaOre>();
            int abyssKelpID = ModContent.TileType<AbyssKelp>();

            int auricRejectionDamage = 300;
            float auricRejectionKB = Player.noKnockback ? 20f : 40f;

            // Get a list of tiles that are colliding with the player.
            List<Point> EdgeTiles = new List<Point>();
            Collision.GetEntityEdgeTiles(EdgeTiles, Player);
            foreach (Point touchedTile in EdgeTiles)
            {
                Tile tile = Framing.GetTileSafely(touchedTile);
                if (!tile.HasTile || !tile.HasUnactuatedTile)
                    continue;

                if (tile.TileType == abyssKelpID)
                {
                    if (Player.velocity.Length() == 0f)
                        return;

                    Dust dust;
                    dust = Main.dust[Dust.NewDust(Player.Center, 16, 16, DustID.Firefly, 0.23255825f, 10f, 0, new Color(117, 55, 15), 1.5116279f)];
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.fadeIn = 2.5813954f;
                }

                // Ores below here
                // Seraph Tracers give immunity to block contact effects
                if (!tracersSeraph)
                {
                    // Astral Ore inflicts Astral Infection briefly on contact
                    if (tile.TileType == astralOreID)
                        Player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 2);

                    // You will need to set each resistant item here for burning as it gets bypassed for somereason
                    if (tile.TileType == scoriaOreID && !Player.fireWalk)
                        Player.AddBuff(BuffID.Burning, 2);
                }

                // Auric Ore causes an Auric Rejection unless you are wearing Auric Armor
                // Auric Rejection causes an electrical explosion that yeets the player a considerable distance
                if ((tile.TileType == auricOreID && !(auricSet || tracersSeraph)) || tile.TileType == auricRepulserID)
                {
                    // Cut grappling hooks so the player is surely thrown
                    Player.RemoveAllGrapplingHooks();

                    // Force Auric Ore to animate with its crackling electricity
                    if (tile.TileType == auricOreID)
                    {
                        AuricOre.Animate = true;
                    }

                    var yeetVec = Vector2.Normalize(Player.Center - touchedTile.ToWorldCoordinates());
                    Player.velocity += yeetVec * auricRejectionKB;
                    if (tile.TileType == auricOreID)
                    {
                        Player.Hurt(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.AuricRejection").Format(Player.name)), auricRejectionDamage, 0);
                        Player.AddBuff(BuffID.Electrified, 300);
                    }
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/ExoMechs/TeslaShoot1"));
                }
            }
        }
        private void HandleBlazingMouseEffects()
        {
            // The sigil's brightness slowly fades away every frame if not incinerating anything.
            blazingMouseAuraFade = MathHelper.Clamp(blazingMouseAuraFade - 0.025f, 0.25f, 1f);

            // Allows the blazing aura to display if the accessory is vanity, but it deals no damage.
            if (!blazingCursorDamage)
                return;

            // miscCounter is used to limit Calamity's hit rate.
            int framesPerHit = 5;
            if (Player.miscCounter % framesPerHit != 1)
                return;

            Rectangle sigilHitbox = Utils.CenteredRectangle(Main.MouseWorld, new Vector2(35f, 62f));
            int sigilDamage = (int)Player.GetBestClassDamage().ApplyTo(Calamity.BaseDamage);
            sigilDamage = Player.ApplyArmorAccDamageBonusesTo(sigilDamage);

            bool brightenedSigil = false;
            foreach (NPC target in Main.ActiveNPCs)
            {
                if (!target.Hitbox.Intersects(sigilHitbox) || target.immortal || target.dontTakeDamage || target.townNPC || NPCID.Sets.ActsLikeTownNPC[target.type] || NPCID.Sets.CountsAsCritter[target.type])
                    continue;

                // Brighten the sigil because it is dealing damage. This can only happen once per hit event.
                if (!brightenedSigil)
                {
                    blazingMouseAuraFade = MathHelper.Clamp(blazingMouseAuraFade + 0.2f, 0.25f, 1f);
                    brightenedSigil = true;
                }

                // Create a direct strike to hit this specific NPC.
                var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<Calamity>()));
                Projectile sigilStrike = Projectile.NewProjectileDirect(source, target.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), sigilDamage, 0f, Player.whoAmI, target.whoAmI);

                // Enable crits by setting the sigil's damage class to be whatever the player's strongest damage class is.
                sigilStrike.DamageType = Player.GetBestClass();

                // Incinerate the target with Vulnerability Hex.
                target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), VulnerabilityHex.CalamityDuration);

                // Make some fancy dust to indicate damage is being done.
                for (int j = 0; j < 12; j++)
                {
                    Dust fire = Dust.NewDustDirect(target.position, target.width, target.height, DustID.RainbowMk2);
                    fire.velocity = Vector2.UnitY * -Main.rand.NextFloat(2f, 3.45f);
                    fire.scale = 1f + fire.velocity.Length() / 6f;
                    fire.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0.85f));
                    fire.noGravity = true;
                    fire.noLightEmittence = true;
                }
            }
        }

        private void MiscEffects()
        {
            // Update Carpet textures
            if (Main.netMode != NetmodeID.Server && Player.whoAmI == Main.myPlayer)
            {
                Asset<Texture2D> carpetAuric = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/AuricCarpet");
                Asset<Texture2D> carpetOriginal = CalamityMod.carpetOriginal;
                TextureAssets.FlyingCarpet = (auricSet ? carpetAuric : carpetOriginal);
            }

            // Calculate/reset DoG cart rotations based on whether the DoG cart is in use.
            if (Player.mount.Active && Player.mount.Type == ModContent.MountType<DoGCartMount>())
            {
                SmoothenedMinecartRotation = MathHelper.Lerp(SmoothenedMinecartRotation, DelegateMethods.Minecart.rotation, 0.05f);

                // Initialize segments from null if necessary.
                int direction = (Player.velocity.SafeNormalize(Vector2.UnitX * Player.direction).X > 0f).ToDirectionInt();
                if (Player.velocity.X == 0f)
                    direction = Player.direction;

                float idealRotation = DoGCartMount.CalculateIdealWormRotation(Player);
                float minecartRotation = DelegateMethods.Minecart.rotation;
                if (Math.Abs(minecartRotation) < 0.5f)
                    minecartRotation = 0f;
                Vector2 stickOffset = minecartRotation.ToRotationVector2() * Player.velocity.Length() * direction * 1.25f;
                for (int i = 0; i < DoGCartSegments.Length; i++)
                {
                    if (DoGCartSegments[i] is null)
                    {
                        DoGCartSegments[i] = new DoGCartSegment
                        {
                            Center = Player.Center - idealRotation.ToRotationVector2() * i * 20f
                        };
                    }
                }

                Vector2 startingStickPosition = Player.Center + stickOffset + new Vector2(direction * (float)Math.Cos(SmoothenedMinecartRotation * 2f) * -34f, 12f);
                DoGCartSegments[0].Update(Player, startingStickPosition, idealRotation);
                DoGCartSegments[0].Center = startingStickPosition;

                for (int i = 1; i < DoGCartSegments.Length; i++)
                {
                    Vector2 waveOffset = DoGCartMount.CalculateSegmentWaveOffset(i, Player);
                    DoGCartSegments[i].Update(Player, DoGCartSegments[i - 1].Center + waveOffset, DoGCartSegments[i - 1].Rotation);
                }
            }
            else
                DoGCartSegments = new DoGCartSegment[DoGCartSegments.Length];

            // Dust on hand when holding the phosphorescent gauntlet.
            if (Player.ActiveItem().type == ModContent.ItemType<PhosphorescentGauntlet>())
                PhosphorescentGauntletPunches.GenerateDustOnOwnerHand(Player);

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
                Player.ownedProjectileCounts[ModContent.ProjectileType<RelicOfDeliveranceSpear>()] > 0)
            {
                Player.controlHook = Player.releaseHook = false;
            }

            if (andromedaCripple > 0)
            {
                Player.velocity = Vector2.Clamp(Player.velocity, new Vector2(-11f, -8f), new Vector2(11f, 8f));
                andromedaCripple--;
            }

            if (Player.ownedProjectileCounts[ModContent.ProjectileType<GiantIbanRobotOfDoom>()] <= 0 &&
                andromedaState != AndromedaPlayerState.Inactive)
            {
                andromedaState = AndromedaPlayerState.Inactive;
            }

            if (andromedaState == AndromedaPlayerState.LargeRobot)
            {
                Player.width = 80;
                Player.height = 212;
                Player.position.Y -= 170;
                resetHeightandWidth = true;
            }
            else if (andromedaState == AndromedaPlayerState.SpecialAttack)
            {
                Player.width = 24;
                Player.height = 98;
                Player.position.Y -= 56;
                resetHeightandWidth = true;
            }
            else if (!Player.mount.Active && resetHeightandWidth)
            {
                Player.width = 20;
                Player.height = 42;
                resetHeightandWidth = false;
            }

            // Summon bullseyes on nearby targets.
            if (spiritOrigin)
            {
                int bullseyeType = ModContent.ProjectileType<SpiritOriginBullseye>();
                List<int> alreadyTargetedNPCs = new List<int>();
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.type != bullseyeType || p.owner != Player.whoAmI)
                        continue;

                    alreadyTargetedNPCs.Add((int)p.ai[0]);
                }

                foreach (NPC target in Main.ActiveNPCs)
                {
                    if (target.friendly || target.lifeMax < 5 || alreadyTargetedNPCs.Contains(target.whoAmI) || target.realLife >= 0 ||
                        target.dontTakeDamage || target.immortal || target.townNPC || NPCID.Sets.ActsLikeTownNPC[target.type] || NPCID.Sets.CountsAsCritter[target.type])
                        continue;

                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<DaawnlightSpiritOrigin>()));
                    if (Main.myPlayer == Player.whoAmI && target.WithinRange(Player.Center, 2000f))
                        Projectile.NewProjectile(source, target.Center, Vector2.Zero, bullseyeType, 0, 0f, Player.whoAmI, target.whoAmI);
                    if (spiritOriginBullseyeShootCountdown <= 0)
                        spiritOriginBullseyeShootCountdown = 45;
                }
            }

            // Life Steal nerf in difficulties above Expert
            // Reduces the max possible life steal before the cooldown occurs (this mostly just nerfs how much life steal the player can get in bursts)
            // Master Mode nerfs this by an additional 10
            float lifeStealCap =
                CalamityWorld.death ? BalancingConstants.LifeStealCap_Death :
                CalamityWorld.revenge ? BalancingConstants.LifeStealCap_Revengeance :
                Main.expertMode ? BalancingConstants.LifeStealCap_Expert :
                BalancingConstants.LifeStealCap_Classic;

            if (Main.masterMode)
                lifeStealCap -= BalancingConstants.LifeStealCapReduction_Master;

            if (Player.lifeSteal > lifeStealCap)
                Player.lifeSteal = lifeStealCap;

            // Normal Mode life steal recovery rate is 0.2/s
            // Expert Mode life steal recovery rate is 0.15/s
            // Revengeance Mode life steal recovery rate is 0.125/s
            // Death Mode life steal recovery rate is 0.1/s
            // Master Mode life steal recovery rate is nerfed by an additional 0.05/s
            float lifeStealRecoveryRateReduction =
                    CalamityWorld.death ? BalancingConstants.LifeStealRecoveryRateReduction_Death :
                    CalamityWorld.revenge ? BalancingConstants.LifeStealRecoveryRateReduction_Revengeance :
                    Main.expertMode ? BalancingConstants.LifeStealRecoveryRateReduction_Expert :
                    BalancingConstants.LifeStealRecoveryRateReduction_Classic;

            if (Main.masterMode)
                lifeStealRecoveryRateReduction += BalancingConstants.LifeStealRecoveryRateReduction_Master;

            if (Player.lifeSteal < lifeStealCap)
                Player.lifeSteal -= lifeStealRecoveryRateReduction;

            // Bool for drawing boss health bar small text or not
            if (Main.myPlayer == Player.whoAmI)
                BossHealthBarManager.CanDrawExtraSmallText = shouldDrawSmallText;

            // Margarita halved debuff duration
            if (margarita)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    for (int l = 0; l < Player.MaxBuffs; l++)
                    {
                        int hasBuff = Player.buffType[l];
                        if (Player.buffTime[l] > 2 && CalamityLists.debuffList.Contains(hasBuff))
                            Player.buffTime[l]--;
                    }
                }
            }

            // Update the Providence Burn effect drawer if applicable.
            float providenceBurnIntensity = 0f;
            int provID = ModContent.NPCType<ProvidenceBoss>();
            if (Main.npc.IndexInRange(CalamityGlobalNPC.holyBoss) && Main.npc[CalamityGlobalNPC.holyBoss].active && Main.npc[CalamityGlobalNPC.holyBoss].type == provID)
                providenceBurnIntensity = (Main.npc[CalamityGlobalNPC.holyBoss].ModNPC as ProvidenceBoss).CalculateBurnIntensity();
            ProvidenceBurnEffectDrawer.ParticleSpawnRate = int.MaxValue;

            // If the player has holy inferno, cause the player to ignite into flames.
            if (hInferno)
                ProvidenceBurnEffectDrawer.ParticleSpawnRate = 1;

            // Otherwise, if the intensity is too weak, but still present, cause the player to release holy cinders.
            else if (providenceBurnIntensity > 0f)
            {
                int cinderCount = (int)MathHelper.Lerp(1f, 4f, Utils.GetLerpValue(0f, 0.45f, providenceBurnIntensity, true));
                for (int i = 0; i < cinderCount; i++)
                {
                    if (!Main.rand.NextBool(3))
                        continue;

                    Dust holyCinder = Dust.NewDustDirect(Player.position, Player.width, Player.height, (int)CalamityDusts.ProfanedFire);
                    holyCinder.velocity = Main.rand.NextVector2Circular(3.5f, 3.5f);
                    holyCinder.velocity.Y -= Main.rand.NextFloat(1f, 3f);
                    holyCinder.scale = Main.rand.NextFloat(1.15f, 1.45f);
                    holyCinder.noGravity = true;
                }
            }
            ProvidenceBurnEffectDrawer.Update();

            if (hInferno && holyInfernoFadeIntensity < 1f)
            {
                holyInfernoFadeIntensity = MathHelper.Clamp(holyInfernoFadeIntensity + 0.015f, 0f, 1f);
            }
            else if (!hInferno && holyInfernoFadeIntensity > 0f)
            {
                holyInfernoFadeIntensity = MathHelper.Clamp(holyInfernoFadeIntensity - 0.01f, 0f, 1f);
            }
            // Transformer immunity to Electrified
            if (transformer)
                Player.buffImmune[BuffID.Electrified] = true;

            // Reduce breath meter while in icy water instead of chilling
            bool canBreath = (aquaticHeart && NPC.downedBoss3) || Player.gills || Player.merman;
            if (Player.arcticDivingGear || canBreath)
            {
                Player.buffImmune[ModContent.BuffType<FrozenLungs>()] = true;
            }
            if (CalamityConfig.Instance.ChilledWaterRework)
            {
                if (Main.expertMode && Player.ZoneSnow && Player.wet && !Player.lavaWet && !Player.honeyWet)
                {
                    Player.buffImmune[BuffID.Chilled] = true;
                    if (Player.IsUnderwater())
                    {
                        if (Main.myPlayer == Player.whoAmI)
                            Player.AddBuff(ModContent.BuffType<FrozenLungs>(), 2, false);
                    }
                }
                if (iCantBreathe)
                {
                    if (Player.breath > 0 && Player.miscCounter % 2 == 0)
                        Player.breath--;
                }
            }

            if (!Player.lavaWet)
            {
                if (Player.lavaImmune)
                {
                    if (Player.lavaTime < Player.lavaMax)
                        Player.lavaTime++;
                }
            }

            // Release irradiated slimes from the sky during the Acid Rain event.
            if (Player.whoAmI == Main.myPlayer)
            {
                if (AcidRainEvent.AcidRainEventIsOngoing && ZoneSulphur && !areThereAnyDamnBosses && Player.Center.Y < Main.worldSurface * 16f + 800f)
                {
                    int slimeRainRate = (int)(MathHelper.Clamp(Main.invasionSize * 0.4f, 13.5f, 50) * 2.25);
                    Vector2 spawnPoint = new Vector2(Player.Center.X + Main.rand.Next(-1000, 1001), Player.Center.Y - Main.rand.Next(700, 801));

                    if (Player.miscCounter % slimeRainRate == 0f)
                    {
                        if (DownedBossSystem.downedAquaticScourge && !DownedBossSystem.downedPolterghast && Main.rand.NextBool(12))
                        {
                            NPC.NewNPC(new EntitySource_SpawnNPC(), (int)spawnPoint.X, (int)spawnPoint.Y, ModContent.NPCType<IrradiatedSlime>());
                        }
                    }
                }
            }

            // Hydrothermal blue smoke effects but it doesn't work epicccccc
            if (Player.whoAmI == Main.myPlayer)
            {
                if (hydrothermalSmoke)
                {
                    if (Math.Abs(Player.velocity.X) > 0.1f || Math.Abs(Player.velocity.Y) > 0.1f)
                    {
                        // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                        var source = Player.GetSource_FromThis(HydrothermicArmor.VanitySmokeEntitySourceContext);
                        Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<HydrothermalSmoke>(), 0, 0f, Player.whoAmI);
                    }
                }
                // Trying to find a workaround because apparently putting the bool in ResetEffects prevents it from working
                if (!Player.armorEffectDrawOutlines)
                {
                    hydrothermalSmoke = false;
                }
            }

            // Death Mode effects
            caveDarkness = 0f;
            if (CalamityWorld.death || Main.getGoodWorld)
            {
                if (Player.whoAmI == Main.myPlayer)
                {
                    // Thorn and spike effects
                    // 10 = crimson/corruption thorns, 17 = jungle thorns, 80 = temple spikes
                    Collision.HurtTile collidedTile;
                    if (!Player.mount.Active || !Player.mount.Cart)
                        collidedTile = Collision.HurtTiles(Player.position, Player.width, Player.height, Player);
                    else
                        collidedTile = Collision.HurtTiles(Player.position, Player.width, Player.height - 16, Player);
                    switch (collidedTile.type)
                    {
                        case 10:
                            Player.AddBuff(BuffID.Weak, 300, false);
                            Player.AddBuff(BuffID.Bleeding, 300, false);
                            break;
                        case 17:
                            Player.AddBuff(BuffID.Poisoned, 300, false);
                            break;
                        case 80:
                            Player.AddBuff(BuffID.Venom, 300, false);
                            break;
                        default:
                            break;
                    }
                }
            }

            // Increase fall speed
            if (!Player.mount.Active)
            {
                if (Player.IsUnderwater() && ironBoots)
                    Player.maxFallSpeed = 9f;

                if (!Player.wet)
                {
                    if (cirrusDress)
                        Player.maxFallSpeed = 12f;
                    if (aeroSet)
                        Player.maxFallSpeed = 15f;
                    if (Player.PortalPhysicsEnabled)
                        Player.maxFallSpeed = 20f;
                }

                if (LungingDown)
                {
                    Player.maxFallSpeed = 80f;
                    Player.noFallDmg = true;
                }

                if (CalamityConfig.Instance.FasterFallHotkey)
                {
                    // Allow the player to double their gravity (but NOT max fall speed!) by holding the down button while in midair.
                    bool holdingDown = Player.controlDown && !Player.controlJump;
                    bool controlsEnabled = Player.ControlsEnabled();
                    bool notInLiquid = !Player.wet;
                    bool notOnRope = !Player.pulley && Player.ropeCount == 0;
                    bool notGrappling = Player.grappling[0] == -1;
                    bool airborne = Player.velocity.Y != 0;
                    if (holdingDown && Player.ControlsEnabled() && notInLiquid && notOnRope && notGrappling && airborne && !Player.Calamity().gSabatonFalling) //Player cannot further increase their ridiculous gravity during a Gravistar Slam
                    {
                        Player.velocity.Y += Player.gravity * Player.gravDir * (BalancingConstants.HoldingDownGravityMultiplier - 1f);
                        if (Player.Calamity().gSabaton)
                        {
                            Player.maxFallSpeed *= 1.5f;
                        }
                        if (Player.velocity.Y * Player.gravDir > Player.maxFallSpeed)
                            Player.velocity.Y = Player.maxFallSpeed * Player.gravDir;
                    }
                }
            }
            else
            {
                if (Player.mount.Type == MountID.Slime || Player.mount.Type == MountID.PogoStick)
                    Player.velocity.X *= 0.91f;
                else if (Player.mount.Type == MountID.QueenSlime)
                    Player.velocity.X *= 0.95f;
            }

            // Omega Blue Armor bonus
            if (omegaBlueSet)
            {
                // Add tentacles
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<OmegaBlueTentacle>()] < 6 && Main.myPlayer == Player.whoAmI)
                {
                    bool[] tentaclesPresent = new bool[6];
                    foreach (Projectile projectile in Main.ActiveProjectiles)
                    {
                        if (projectile.type == ModContent.ProjectileType<OmegaBlueTentacle>() && projectile.owner == Main.myPlayer && projectile.ai[1] >= 0f && projectile.ai[1] < 6f)
                            tentaclesPresent[(int)projectile.ai[1]] = true;
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        if (!tentaclesPresent[i])
                        {
                            int damage = (int)Player.GetBestClassDamage().ApplyTo(390);
                            damage = Player.ApplyArmorAccDamageBonusesTo(damage);

                            var source = Player.GetSource_FromThis(OmegaBlueHelmet.TentacleEntitySourceContext);
                            Vector2 vel = new Vector2(Main.rand.Next(-13, 14), Main.rand.Next(-13, 14)) * 0.25f;
                            Projectile.NewProjectile(source, Player.Center, vel, ModContent.ProjectileType<OmegaBlueTentacle>(), damage, 8f, Main.myPlayer, Main.rand.Next(120), i);
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
                Player.GetDamage<GenericDamageClass>() += damageUp;
                Player.GetCritChance<GenericDamageClass>() += critUp;
            }

            bool profanedSoulBuffs = profanedCrystalBuffs || (!profanedCrystal && pSoulArtifact) || (profanedCrystal && DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs);

            // Guardian bonuses
            if (profanedSoulBuffs)
            {
                // Offense bonus
                Player.maxMinions++;
                // Healer bonus
                if (healCounter > 0)
                    healCounter--;

                if (healCounter <= 0)
                {
                    healCounter = 300;

                    if (Player.whoAmI == Main.myPlayer)
                    {
                        Player.statLife += 15;
                        Player.HealEffect(15);
                        if (Player.statLife > Player.statLifeMax2)
                            Player.statLife = Player.statLifeMax2;

                        if (profanedCrystal)
                        {
                            var healerID = ModContent.ProjectileType<MiniGuardianHealer>();
                            var healer = Main.projectile.FirstOrDefault(proj => proj.active && proj.owner == Main.myPlayer && proj.type == healerID, null);
                            if (healer != null)
                            {
                                float distanceFromHealer = Vector2.Distance(healer.Center, Player.Center);
                                int maxHealDustIterations = (int)distanceFromHealer;
                                int maxDust = 40;
                                int dustDivisor = maxHealDustIterations / maxDust;
                                if (dustDivisor < 2)
                                    dustDivisor = 2;

                                Vector2 dustLineStart = healer.Center;
                                Vector2 dustLineEnd = Player.Center;
                                Vector2 currentDustPos = default;
                                Vector2 spinningpoint = new Vector2(0f, -3f).RotatedByRandom(MathHelper.Pi);
                                Vector2 healerDustVel = new Vector2(2.1f, 2f);
                                Color dustColor = Main.hslToRgb(Main.rgbToHsl(new Color(255, 200, Main.DiscoB)).X, 1f, 0.5f);
                                dustColor.A = 255;
                                for (int i = 0; i < maxHealDustIterations; i++)
                                {
                                    if (i % dustDivisor == 0)
                                    {
                                        currentDustPos = Vector2.Lerp(dustLineStart, dustLineEnd, i / (float)maxHealDustIterations);
                                        int holyDust = Dust.NewDust(currentDustPos, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, dustColor, 1f);
                                        Main.dust[holyDust].position = currentDustPos;
                                        Main.dust[holyDust].velocity = spinningpoint.RotatedBy(MathHelper.TwoPi * i / maxHealDustIterations) * healerDustVel * (0.8f + Main.rand.NextFloat() * 0.4f) + Player.velocity;
                                        Main.dust[holyDust].noGravity = true;
                                        Main.dust[holyDust].scale = 1f;
                                        Main.dust[holyDust].fadeIn = Main.rand.NextFloat() * 2f;
                                        Dust dustClone = Dust.CloneDust(holyDust);
                                        Dust extraDust = dustClone;
                                        extraDust.scale /= 2f;
                                        extraDust = dustClone;
                                        extraDust.fadeIn /= 2f;
                                        dustClone.color = new Color(255, 255, 255, 255);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (unstableGraniteCore)
            {
                zapActivity += 1;
                if (zapActivity <= 300 && zapActivity % 30 == 0)
                {
                    for (int arcProjCount = 0; arcProjCount < 3; arcProjCount++)
                    {
                        float maxDistance = 300f;
                        int target = -1;
                        foreach (NPC npc in Main.ActiveNPCs)
                        {
                            float targetDist = Vector2.Distance(npc.Center, Player.Center);
                            if (targetDist < maxDistance && npc.Calamity().arcZapCooldown == 0 && npc.CanBeChasedBy())
                            {
                                maxDistance = targetDist;
                                target = npc.whoAmI;
                            }
                        }

                        if (target > 0)
                        {
                            unstableSelectedTarget = Main.npc[target];
                            unstableSelectedTarget.Calamity().arcZapCooldown = 18;
                            int damage = Player.ApplyArmorAccDamageBonusesTo(Player.GetBestClassDamage().ApplyTo(18));

                            Projectile.NewProjectile(Player.GetSource_FromThis(), new Vector2(Player.Center.X, Player.Center.Y - 20f), new Vector2(0f, -2f), ModContent.ProjectileType<ArcZap>(), damage, 0f, Player.whoAmI, target, 3f);
                            target = -1;
                        }
                    }
                }
                else if (zapActivity > 600)
                    zapActivity = 0;
            }

            if (nucleogenesis)
            {
                Player.maxMinions += 4;
            }
            else
            {
                // First Shadowflame is +1, Statis' Blessing is +2, Statis' Curse inherits both for +3
                if (shadowMinions)
                    Player.maxMinions++;
                if (holyMinions)
                    Player.maxMinions += 2;

                if (starTaintedGenerator)
                {
                    Player.maxMinions += 2;
                }
                else
                {
                    if (starbusterCore)
                        Player.maxMinions++;

                    if (voltaicJelly)
                        Player.maxMinions++;
                    if (nuclearFuelRod)
                        Player.maxMinions++;
                }
            }

            // Tick all cooldowns.
            // Depending on the code for each individual cooldown, this isn't guaranteed to do anything.
            // It may not tick down the timer or not do anything at all.
            IList<string> expiredCooldowns = new List<string>(16);
            var cdIterator = cooldowns.GetEnumerator();
            while (cdIterator.MoveNext())
            {
                KeyValuePair<string, CooldownInstance> kv = cdIterator.Current;
                string id = kv.Key;
                CooldownInstance instance = kv.Value;
                CooldownHandler handler = instance.handler;

                // If applicable, tick down this cooldown instance's timer.
                if (handler.CanTickDown)
                    --instance.timeLeft;

                // Tick always runs, even if the timer does not decrement.
                handler.Tick();

                // Run on-completion code, play sounds and remove finished cooldowns.
                if (instance.timeLeft < 0)
                {
                    handler.OnCompleted();
                    if (handler.EndSound != null && handler.ShouldPlayEndSound)
                        SoundEngine.PlaySound(handler.EndSound.GetValueOrDefault());
                    expiredCooldowns.Add(id);
                }
            }
            cdIterator.Dispose();

            // Remove all expired cooldowns.
            foreach (string cdID in expiredCooldowns)
                cooldowns.Remove(cdID);

            // If any cooldowns were removed, send a cooldown removal packet that lists all cooldowns to remove.
            if (expiredCooldowns.Count > 0)
                SyncCooldownRemoval(Main.netMode == NetmodeID.Server, expiredCooldowns);

            // Grant the player 5 seconds of immunity to immobilizing debuffs after an immobilizing debuff wears off.
            if (Player.stoned || Player.frozen || Player.webbed || gState)
            {
                ImmobilityDebuffImmunityTimer = ImmobilityDebuffImmunityTimerMax;
            }
            else if (ImmobilityDebuffImmunityTimer > 0)
            {
                ImmobilityDebuffImmunityTimer--;
                Player.buffImmune[BuffID.Stoned] = true;
                Player.buffImmune[BuffID.Frozen] = true;
                Player.buffImmune[BuffID.Webbed] = true;
                Player.buffImmune[ModContent.BuffType<GlacialState>()] = true;
            }

            if (ascendantInsigniaCooldown > 0 && ascendantInsigniaBuffTime <= 0)
                ascendantInsigniaCooldown--;
            if (DragonsBreathAudioCooldown > 0)
                DragonsBreathAudioCooldown--;
            if (DragonsBreathAudioCooldown2 > 0)
                DragonsBreathAudioCooldown2--;
            if (PhotoAudioCooldown > 0)
                PhotoAudioCooldown--;
            if (fullRageSoundCountdownTimer > 0)
                --fullRageSoundCountdownTimer;
            if (plagueTaintedSMGDroneCooldown > 0)
                plagueTaintedSMGDroneCooldown--;
            if (momentumCapacitorTime > 0)
                --momentumCapacitorTime;
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
            if (dragonRageCooldown > 0)
                dragonRageCooldown--;
            if (soundCooldown > 0)
                soundCooldown--;
            if (shadowPotCooldown > 0)
                shadowPotCooldown--;
            if (raiderCritBonus > 0f)
                raiderCritBonus -= RaidersTalisman.RaiderBonus / (float)CalamityUtils.SecondsToFrames(RaidersTalisman.RaiderCooldown);
            if (raiderSoundCooldown > 0)
                raiderSoundCooldown--;
            if (astralStarRainCooldown > 0)
                astralStarRainCooldown--;
            if (AbaddonCooldown > 0)
                AbaddonCooldown--;
            if (AlchFlaskCooldown > 0)
                AlchFlaskCooldown--;
            if (tarraRangedCooldown > 0)
                tarraRangedCooldown--;
            if (bloodflareMageCooldown > 0)
                bloodflareMageCooldown--;
            if (silvaMageCooldown > 0)
                silvaMageCooldown--;
            if (scuttlerCooldown > 0)
                scuttlerCooldown--;
            if (rogueCrownCooldown > 0)
                rogueCrownCooldown--;
            if (spectralVeilImmunity > 0)
                spectralVeilImmunity--;
            if (jetPackDash > 0)
                jetPackDash--;
            if (theBeeCooldown > 0)
                theBeeCooldown--;
            if (summonProjCooldown > 0f)
                summonProjCooldown -= 1f;
            if (ataxiaDmg > 0f)
                ataxiaDmg -= 1.5f;
            if (ataxiaDmg < 0f)
                ataxiaDmg = 0f;
            if (xerocDmg > 0f)
                xerocDmg -= 2f;
            if (xerocDmg < 0f)
                xerocDmg = 0f;
            if (hideOfDeusMeleeBoostTimer > 0)
                hideOfDeusMeleeBoostTimer--;
            if (evolutionLifeRegenCounter > 0)
                evolutionLifeRegenCounter--;
            if (hurtSoundTimer > 0)
                hurtSoundTimer--;
            if (icicleCooldown > 0)
                icicleCooldown--;
            if (statisTimer > 0 && Player.dashDelay >= 0)
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
            if (prismaticLasers > 0)
                prismaticLasers--;
            if (dogTextCooldown > 0)
                dogTextCooldown--;
            if (titanCooldown > 0)
                titanCooldown--;
            if (hideOfDeusTimer > 0)
                hideOfDeusTimer--;
            if (murasamaHitCooldown > 0)
                murasamaHitCooldown--;
            if (hellbornBoost > 0)
                hellbornBoost--;
            if (persecutedEnchantSummonTimer < 1800)
                persecutedEnchantSummonTimer++;
            else
            {
                persecutedEnchantSummonTimer = 0;
                if (Main.myPlayer == Player.whoAmI && persecutedEnchant)
                {
                    if (NPC.CountNPCS(ModContent.NPCType<DemonPortal>()) < 2)
                    {
                        int tries = 0;
                        Vector2 spawnPosition;
                        Vector2 spawnPositionOffset = Vector2.One * 24f;
                        do
                        {
                            spawnPosition = Player.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(270f, 420f);
                            tries++;
                        }
                        while (Collision.SolidCollision(spawnPosition - spawnPositionOffset, 48, 24) && tries < 100);
                        CalamityNetcode.NewNPC_ClientSide(spawnPosition, ModContent.NPCType<DemonPortal>(), Player);
                    }
                }
            }
            if (Player.miscCounter % 20 == 0)
                canFireAtaxiaRangedProjectile = true;
            if (Player.miscCounter % 100 == 0)
                canFireBloodflareMageProjectile = true;
            if (Player.miscCounter % 120 == 0)
                canFireAtaxiaRogueProjectile = true;
            if (Player.miscCounter % 150 == 0)
            {
                canFireGodSlayerRangedProjectile = true;
                canFireBloodflareRangedProjectile = true;
            }
            if (reaverRegenCooldown < 60 && reaverRegen)
                reaverRegenCooldown++;
            else
                reaverRegenCooldown = 0;
            if (auralisAurora > 0)
                auralisAurora--;
            if (auralisAuroraCooldown > 0)
                auralisAuroraCooldown--;

            if (blazingCore)
            {
                if (blazingCoreSuccessfulParry > 0)
                    BlazingCore.HandleStars(Player);
                else if (blazingCoreParry > 0)
                    BlazingCore.HandleParryCountdown(Player);
            }
            else if (flameLickedShell && flameLickedShellParry > 0)
            {
                FlameLickedShell.HandleParryCountdown(Player);
            }

            // Silver Armor "Medkit" effect
            if (silverMedkitTimer > 0)
            {
                --silverMedkitTimer;
                if (silverMedkitTimer == 0)
                {
                    Player.HealEffect(SilverArmorSetChange.SetBonusHealAmount, true);
                    Player.statLife += SilverArmorSetChange.SetBonusHealAmount;
                    if (Player.statLife > Player.statLifeMax2)
                        Player.statLife = Player.statLifeMax2;

                    SilverArmorSetChange.OnHealEffects(Player);
                }
            }

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
            if (ChlorophyteHealDelay > 0)
                ChlorophyteHealDelay--;
            if (monolithAccursedShader > 0)
                monolithAccursedShader--;
            if (monolithBossRushShader > 0)
                monolithBossRushShader--;
            if (monolithExoShader > 0)
                monolithExoShader--;
            if (monolithLeviathanShader > 0)
                monolithLeviathanShader--;
            if (monolithCryogenShader > 0)
                monolithCryogenShader--;
            if (monolithDevourerBShader > 0)
                monolithDevourerBShader--;
            if (monolithDevourerPShader > 0)
                monolithDevourerPShader--;
            if (monolithYharonShader > 0)
                monolithYharonShader--;
            if (monolithPlagueShader > 0)
                monolithPlagueShader--;
            if (monolithAstralShader > 0)
                monolithAstralShader--;
            if (BrimstoneLavaFountainCounter > 0)
                BrimstoneLavaFountainCounter--;

            if (miningSetCooldown > 0)
                miningSetCooldown--;
            if (RustyMedallionCooldown > 0)
                RustyMedallionCooldown--;
            if (MiniSwamerCooldown > 0)
                MiniSwamerCooldown--;

            // God Slayer Armor dash debuff immunity
            if (DashID == GodSlayerDash.ID && Player.dashDelay < 0)
            {
                foreach (int debuff in CalamityLists.debuffList)
                    Player.buffImmune[debuff] = true;
            }

            // Shield of the High Ruler
            if (copyrightInfringementShield)
            {
                if (Player.dashType == 2 && DashID == string.Empty)
                {
                    // If the player hasn't hit anything with the shield and a dash is currently happening, increase velocity on the first frame of the dash to be on par with Tabi.
                    // EoC dash decelerates faster than Tabi, so compensate for it by increasing the Tabi dash velocity value by an approximate amount.
                    if (Player.eocHit == -1 && Player.dashDelay == -1)
                    {
                        if (!shieldOfTheHighRulerDashVelocityBoosted)
                        {
                            shieldOfTheHighRulerDashVelocityBoosted = true;

                            if (Math.Abs(Player.velocity.X) <= ShieldoftheHighRuler.TabiDashVelocity)
                                Player.velocity.X *= ShieldoftheHighRuler.TabiDashVelocity / ShieldoftheHighRuler.EoCDashVelocity;
                        }
                    }
                    else
                        shieldOfTheHighRulerDashVelocityBoosted = false;

                    // Dash delay reduced to 15 frames (half the original 30) if an enemy is bonked.
                    if (Player.eocHit != -1)
                    {
                        if (Player.dashDelay > 15)
                            Player.dashDelay = 15;
                    }
                }
            }
            else
                shieldOfTheHighRulerDashVelocityBoosted = false;

            // Auric dye cinders.
            int auricDyeCount = Player.dye.Count(dyeItem => dyeItem.type == ModContent.ItemType<AuricDye>());
            if (auricDyeCount > 0)
            {
                int sparkCreationChance = (int)MathHelper.Lerp(15f, 50f, Utils.GetLerpValue(4f, 1f, auricDyeCount, true));
                if (Main.rand.NextBool(sparkCreationChance))
                {
                    Dust spark = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.RainbowMk2);
                    spark.color = Color.Lerp(Color.Cyan, Color.SeaGreen, Main.rand.NextFloat(0.5f));
                    spark.velocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2 * 1.33f) * Main.rand.NextFloat(2f, 5.4f);
                    spark.noGravity = true;
                }
            }

            // Necro armor post-mortem effects. Activates regardless of having the armor equipped because it is a "delayed death"
            if (necroReviveCounter >= 0)
            {
                necroReviveCounter++;
                float ratioUntilDead = necroReviveCounter / (NecroArmorSetChange.PostMortemDuration * 60f);
                int upperHealthLimit = (int)MathHelper.Lerp(Player.statLifeMax2, 1, ratioUntilDead);

                if (Player.statLife > upperHealthLimit)
                    Player.statLife = upperHealthLimit;

                if (necroReviveCounter >= NecroArmorSetChange.PostMortemDuration * 60)
                    Player.KillMe(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.NecroRevive").Format(Player.name)), 1000, -1);
                else if (necroReviveCounter % 60 == 59)
                    SoundEngine.PlaySound(NecroArmorSetChange.TimerSound, Player.Center);
            }

            // Silva invincibility effects
            if (silvaCountdown > 0 && hasSilvaEffect && silvaSet)
            {
                foreach (int debuff in CalamityLists.debuffList)
                    Player.buffImmune[debuff] = true;

                silvaCountdown -= 1;
                if (silvaCountdown <= 0)
                {
                    SoundEngine.PlaySound(SilvaHeadSummon.DispelSound, Player.Center);
                    Player.AddCooldown(SilvaRevive.ID, CalamityUtils.SecondsToFrames(5 * 60));
                }

                for (int j = 0; j < 2; j++)
                {
                    int green = Dust.NewDust(Player.position, Player.width, Player.height, DustID.ChlorophyteWeapon, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 2f);
                    Main.dust[green].position.X += (float)Main.rand.Next(-20, 21);
                    Main.dust[green].position.Y += (float)Main.rand.Next(-20, 21);
                    Main.dust[green].velocity *= 0.9f;
                    Main.dust[green].noGravity = true;
                    Main.dust[green].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    Main.dust[green].shader = GameShaders.Armor.GetSecondaryShader(Player.ArmorSetDye(), Player);
                    if (Main.rand.NextBool())
                        Main.dust[green].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                }
            }
            if (!Player.HasCooldown(SilvaRevive.ID) && hasSilvaEffect && silvaCountdown <= 0 && !areThereAnyDamnBosses && !areThereAnyDamnEvents)
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
                    if (Player.whoAmI == Main.myPlayer)
                        Player.AddCooldown(Cooldowns.TarragonCloak.ID, CalamityUtils.SecondsToFrames(30));
                }

                for (int j = 0; j < 2; j++)
                {
                    Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.ChlorophyteWeapon, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 2f);
                    dust.position.X += (float)Main.rand.Next(-20, 21);
                    dust.position.Y += (float)Main.rand.Next(-20, 21);
                    dust.velocity *= 0.9f;
                    dust.noGravity = true;
                    dust.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(Player.ArmorSetDye(), Player);
                    if (Main.rand.NextBool())
                        dust.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                }
            }

            // Tarragon immunity effects
            if (tarraThrowing)
            {
                // The iframes from the evasion are disabled by dodge disabling effects.
                // 17APR2024: Ozzatron: Tarragon Immunity is meant to be a full invulnerability effect, so universal iframes are granted throughout its duration.
                // It has no interaction with Cross Necklace.
                if (tarragonImmunity && !disableAllDodges)
                    Player.GiveUniversalIFrames(2, true);

                if (tarraThrowingCrits >= 50)
                {
                    tarraThrowingCrits = 0;
                    if (Player.whoAmI == Main.myPlayer && !disableAllDodges)
                        Player.AddBuff(ModContent.BuffType<Buffs.StatBuffs.TarragonImmunity>(), 150, false);
                }

                for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    int hasBuff = Player.buffType[l];
                    if (Player.buffTime[l] <= 2 && hasBuff == ModContent.BuffType<Buffs.StatBuffs.TarragonImmunity>())
                        if (Player.whoAmI == Main.myPlayer)
                            Player.AddCooldown(Cooldowns.TarragonImmunity.ID, CalamityUtils.SecondsToFrames(30));

                    bool shouldAffect = CalamityLists.debuffList.Contains(hasBuff);
                    if (shouldAffect)
                        Player.GetDamage<RogueDamageClass>() += 0.1f;
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
                    if (Player.whoAmI == Main.myPlayer)
                        Player.AddBuff(ModContent.BuffType<BloodflareBloodFrenzy>(), 302, false);
                }

                if (bloodflareFrenzy)
                {
                    for (int l = 0; l < Player.MaxBuffs; l++)
                    {
                        int hasBuff = Player.buffType[l];
                        if (Player.buffTime[l] <= 2 && hasBuff == ModContent.BuffType<BloodflareBloodFrenzy>() && Player.whoAmI == Main.myPlayer)
                            Player.AddCooldown(BloodflareFrenzy.ID, CalamityUtils.SecondsToFrames(30));
                    }

                    Player.GetCritChance<MeleeDamageClass>() += 25;
                    Player.GetDamage<MeleeDamageClass>() += 0.25f;

                    for (int j = 0; j < 2; j++)
                    {
                        int blood = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                        Dust dust = Main.dust[blood];
                        dust.position.X += (float)Main.rand.Next(-20, 21);
                        dust.position.Y += (float)Main.rand.Next(-20, 21);
                        dust.velocity *= 0.9f;
                        dust.noGravity = true;
                        dust.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                        dust.shader = GameShaders.Armor.GetSecondaryShader(Player.ArmorSetDye(), Player);
                        if (Main.rand.NextBool())
                            dust.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    }
                }
            }

            // Raider Talisman bonus
            if (raiderTalisman && !StealthStrikeAvailable())
                Player.GetCritChance<ThrowingDamageClass>() += raiderCritBonus;

            if (kamiBoost)
                Player.GetDamage<GenericDamageClass>() += 0.15f;

            if (avertorBonus)
                Player.GetDamage<GenericDamageClass>() += 0.1f;

            // Reduce Ichor debuff defense reduction from -15 to -10
            if (Player.ichor)
                Player.statDefense += 5;

            // Fairy Boots bonus
            if (fairyBoots)
            {
                if (Player.isNearFairy())
                {
                    Player.lifeRegen += 4;
                    Player.statDefense += 10;
                    Player.moveSpeed += 0.1f;
                }
            }

            // Absorber bonus
            if (absorber)
            {
                Player.moveSpeed += 0.12f;
                Player.jumpSpeedBoost += 0.6f;
                Player.thorns += 3.5f;
            }

            // Affliction bonus
            if (affliction || afflicted)
            {
                Player.endurance += 0.07f;
                Player.statDefense += 13;
                Player.GetDamage<GenericDamageClass>() += 0.1f;
            }

            float[] light = new float[3];
            if (cFreeze)
            {
                light[0] += 0.3f;
                light[1] += Main.DiscoG / 400f;
                light[2] += 0.5f;
            }
            if (aquaticHeartIce)
            {
                light[0] += 0.35f;
                light[1] += 1f;
                light[2] += 1.25f;
            }
            if (aquaticHeart)
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
            Lighting.AddLight((int)(Player.Center.X / 16f), (int)(Player.Center.Y / 16f), light[0], light[1], light[2]);

            // Permafrost's Concoction bonuses/debuffs
            if (permafrostsConcoction)
            {
                Player.manaCost *= 0.85f;
                Player.statManaMax2 += 50;
            }

            if (encased)
            {
                Player.statDefense += 30;
                Player.frozen = true;
                Player.velocity.X = 0f;
                Player.velocity.Y = -0.4f; // Should negate gravity

                int ice = Dust.NewDust(Player.position, Player.width, Player.height, DustID.GemSapphire);
                Main.dust[ice].noGravity = true;
                Main.dust[ice].velocity *= 2f;

                Player.buffImmune[BuffID.Frozen] = true;
                Player.buffImmune[BuffID.Chilled] = true;
                Player.buffImmune[ModContent.BuffType<GlacialState>()] = true;
            }

            // Cosmic Discharge Cosmic Freeze buff, gives surrounding enemies the Glacial State debuff
            if (cFreeze)
            {
                int buffType = ModContent.BuffType<GlacialState>();
                float freezeDist = 200f;
                if (Player.whoAmI == Main.myPlayer)
                {
                    if (Main.rand.NextBool(5))
                    {
                        foreach (NPC npc in Main.ActiveNPCs)
                        {
                            if (npc.friendly || npc.damage <= 0 || npc.dontTakeDamage)
                                continue;

                            if (!npc.buffImmune[buffType] && Vector2.Distance(Player.Center, npc.Center) <= freezeDist)
                            {
                                if (npc.FindBuffIndex(buffType) == -1)
                                    npc.AddBuff(buffType, 60, false);
                            }
                        }
                    }
                }
            }

            // Vortex Armor nerf
            if (Player.vortexStealthActive)
            {
                Player.GetDamage<RangedDamageClass>() -= (1f - Player.stealth) * 0.4f; // Change 80 to 40
                Player.GetCritChance<RangedDamageClass>() -= (int)((1f - Player.stealth) * 5f); // Change 20 to 15
            }

            // Haste buff
            if (hasteLevel > 0)
            {
                // capped out at 3
                if (hasteLevel > 3)
                    hasteLevel = 3;
                // if the haste counter hits 5 seconds, subtract a haste level, and if there are none left afterwards, delete the buff
                if (++hasteCounter == 300)
                {
                    hasteLevel--;
                    if (hasteLevel <= 0)
                    {
                        if (Player.FindBuffIndex(ModContent.BuffType<Haste>()) > -1)
                            Player.ClearBuff(ModContent.BuffType<Haste>());
                    }
                    hasteCounter = 0;
                }
            }

            // Calcium Potion buff
            if (calcium)
                Player.noFallDmg = true;

            // Ceaseless Hunger Potion buff
            if (ceaselessHunger)
            {
                foreach (Item item in Main.ActiveItems)
                {
                    if (item.noGrabDelay == 0 && item.playerIndexTheItemIsReservedFor == Player.whoAmI)
                    {
                        item.beingGrabbed = true;
                        if (Player.Center.X > item.Center.X)
                        {
                            if (item.velocity.X < 90f + Player.velocity.X)
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
                            if (item.velocity.X > -90f + Player.velocity.X)
                            {
                                item.velocity.X -= 9f;
                            }
                            if (item.velocity.X > 0f)
                            {
                                item.velocity.X -= 9f * 0.75f;
                            }
                        }

                        if (Player.Center.Y > item.Center.Y)
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
            if (jetPackDash > 0 && Player.whoAmI == Main.myPlayer)
            {
                int velocityAmt = blunderBooster ? 35 : 25;
                int velocityMult = jetPackDash > 1 ? velocityAmt : 5;
                Player.velocity = new Vector2(jetPackDirection, -1) * velocityMult;

                if (blunderBooster)
                {
                    int lightningCount = Main.rand.Next(5, 7);
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<BlunderBooster>()));
                    for (int i = 0; i < lightningCount; i++)
                    {
                        Vector2 lightningVel = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                        lightningVel.Normalize();
                        lightningVel *= Main.rand.NextFloat(1f, 2f);
                        int damage = (int)Player.GetTotalDamage<RogueDamageClass>().ApplyTo(30);
                        damage = Player.ApplyArmorAccDamageBonusesTo(damage);

                        int projectile = Projectile.NewProjectile(source, Player.Center, lightningVel, ModContent.ProjectileType<BlunderBoosterLightning>(), damage, 0, Player.whoAmI, Main.rand.Next(2), 0f);
                        Main.projectile[projectile].timeLeft = Main.rand.Next(180, 240);
                        if (projectile.WithinBounds(Main.maxProjectiles))
                            Main.projectile[projectile].DamageType = DamageClass.Generic;
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        int dust = Dust.NewDust(Player.Center, 1, 1, DustID.RedTorch, Player.velocity.X * -0.1f, Player.velocity.Y * -0.1f, 100, default, 3.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 1.2f;
                        Main.dust[dust].velocity.Y -= 0.15f;
                    }
                }
                else if (plaguedFuelPack)
                {
                    int numClouds = Main.rand.Next(8, 10);
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<PlaguedFuelPack>()));
                    for (int i = 0; i < numClouds; i++)
                    {
                        Vector2 cloudVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                        cloudVelocity.Normalize();
                        cloudVelocity *= Main.rand.NextFloat(0f, 1f);
                        int damage = (int)Player.GetTotalDamage<RogueDamageClass>().ApplyTo(20);
                        damage = Player.ApplyArmorAccDamageBonusesTo(damage);

                        int projectile = Projectile.NewProjectile(source, Player.Center, cloudVelocity, ModContent.ProjectileType<PlaguedFuelPackCloud>(), damage, 0, Player.whoAmI, 0, 0);
                        Main.projectile[projectile].timeLeft = Main.rand.Next(180, 240);
                        if (projectile.WithinBounds(Main.maxProjectiles))
                            Main.projectile[projectile].DamageType = DamageClass.Generic;
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        int dust = Dust.NewDust(Player.Center, 1, 1, DustID.GemEmerald, Player.velocity.X * -0.1f, Player.velocity.Y * -0.1f, 100, default, 3.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 1.2f;
                        Main.dust[dust].velocity.Y -= 0.15f;
                    }
                }
            }

            // This section of code ensures set bonuses and accessories with cooldowns go on cooldown immediately if the armor or accessory is removed.
            if (!ascendantInsignia && ascendantInsigniaBuffTime > 0)
            {
                ascendantInsigniaBuffTime = 0;
                ascendantInsigniaCooldown = 2400;
                Player.AddCooldown(AscendEffect.ID, 2400);
            }

            if (!brimflameSet && brimflameFrenzy)
            {
                brimflameFrenzy = false;
                Player.ClearBuff(ModContent.BuffType<BrimflameFrenzyBuff>());
                Player.AddCooldown(BrimflameFrenzy.ID, BrimflameScowl.CooldownLength);
            }
            if (!bloodflareMelee && bloodflareFrenzy)
            {
                bloodflareFrenzy = false;
                Player.ClearBuff(ModContent.BuffType<BloodflareBloodFrenzy>());
                Player.AddCooldown(BloodflareFrenzy.ID, CalamityUtils.SecondsToFrames(30));
            }
            if (!tarraMelee && tarragonCloak)
            {
                tarragonCloak = false;
                Player.ClearBuff(ModContent.BuffType<Buffs.StatBuffs.TarragonCloak>());
                Player.AddCooldown(Cooldowns.TarragonCloak.ID, CalamityUtils.SecondsToFrames(30));
            }
            if (!tarraThrowing && tarragonImmunity)
            {
                tarragonImmunity = false;
                Player.ClearBuff(ModContent.BuffType<Buffs.StatBuffs.TarragonImmunity>());
                Player.AddCooldown(Cooldowns.TarragonImmunity.ID, CalamityUtils.SecondsToFrames(25));
            }

            bool hasOmegaBlueCooldown = cooldowns.TryGetValue(OmegaBlue.ID, out CooldownInstance omegaBlueCD);
            if (!omegaBlueSet && hasOmegaBlueCooldown && omegaBlueCD.timeLeft > 1500)
            {
                Player.ClearBuff(ModContent.BuffType<AbyssalMadness>());
                omegaBlueCD.timeLeft = 1500;
            }

            bool hasPlagueBlackoutCD = cooldowns.TryGetValue(PlagueBlackout.ID, out CooldownInstance plagueBlackoutCD);
            if (!plagueReaper && hasPlagueBlackoutCD && plagueBlackoutCD.timeLeft > 1500)
                plagueBlackoutCD.timeLeft = 1500;

            if (!prismaticSet && prismaticLasers > 1800)
            {
                prismaticLasers = 1800;
                Player.AddCooldown(PrismaticLaser.ID, 1800);
            }
            if (!angelicAlliance && divineBless)
            {
                divineBless = false;
                Player.ClearBuff(ModContent.BuffType<Buffs.StatBuffs.DivineBless>());
                Player.AddCooldown(Cooldowns.DivineBless.ID, CalamityUtils.SecondsToFrames(60));
            }

            // Armageddon's Dodge Disable feature puts Shadow Dodge/Holy Protection on permanent cooldown
            if (disableAllDodges)
            {
                if (Player.shadowDodgeTimer < 2)
                    Player.shadowDodgeTimer = 2;
            }
        }
        #endregion

        #region Abyss Effects
        private void AbyssEffects()
        {
            int lightStrength = Player.GetCurrentAbyssLightLevel();

            if (ZoneAbyss)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    // Abyss depth variables
                    Point point = Player.Center.ToTileCoordinates();
                    double abyssSurface = Main.remixWorld ? SulphurousSea.YStart : (Main.rockLayer - Main.maxTilesY * 0.05);
                    double abyssLevel1 = Main.remixWorld ? (SulphurousSea.YStart - Main.maxTilesY * 0.05) : (Main.rockLayer + Main.maxTilesY * 0.03);
                    double totalAbyssDepth = Main.remixWorld ? SulphurousSea.YStart : (Main.maxTilesY - 250D - abyssSurface);
                    double totalAbyssDepthFromLayer1 = Main.remixWorld ? (SulphurousSea.YStart - Main.maxTilesY * 0.05) : (Main.maxTilesY - 250D - abyssLevel1);
                    double playerAbyssDepth = Main.remixWorld ? (totalAbyssDepth - point.Y) : (point.Y - abyssSurface);
                    double playerAbyssDepthFromLayer1 = Main.remixWorld ? (abyssLevel1 - point.Y) : (point.Y - abyssLevel1);
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

                    // Modify darkness variable
                    caveDarkness = darknessStrength * multiplier;

                    // Nebula Headcrab darkness effect
                    if (!Player.headcovered)
                    {
                        float screenObstructionAmt = MathHelper.Clamp(caveDarkness, 0f, 0.95f);
                        float targetValue = MathHelper.Clamp(screenObstructionAmt * 0.7f, 0.1f, 0.3f);
                        ScreenObstruction.screenObstruction = MathHelper.Lerp(ScreenObstruction.screenObstruction, screenObstructionAmt, targetValue);
                    }

                    // Breath lost while at zero breath
                    double breathLoss = Main.remixWorld ? (point.Y < abyssLevel1 ? 50D * depthRatioFromAbyssLayer1 : 0D) : (point.Y > abyssLevel1 ? 50D * depthRatioFromAbyssLayer1 : 0D);

                    // Breath Loss Multiplier, depending on gear
                    double breathLossMult = 1D -
                        (Player.gills ? 0.2 : 0D) - // 0.8
                        (oceanCrest ? 0.2 : 0D) - // 0.8
                        (Player.accDivingHelm ? 0.25 : 0D) - // 0.75
                        (Player.arcticDivingGear ? 0.25 : 0D) - // 0.75
                        (aquaticEmblem ? 0.25 : 0D) - // 0.75
                        (Player.accMerman ? 0.3 : 0D) - // 0.7
                        (victideSet ? 0.2 : 0D) - // 0.85
                        ((aquaticHeart && NPC.downedBoss3) ? 0.3 : 0D) - // 0.7
                        (abyssalDivingSuit ? 0.3 : 0D); // 0.7

                    // Limit the multiplier to 5%
                    if (breathLossMult < 0.05)
                        breathLossMult = 0.05;

                    // Reduce breath lost while at zero breath, depending on gear
                    breathLoss *= breathLossMult;

                    // Record the final breath loss for the stat meter
                    abyssBreathLossStat = (float)breathLoss;

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
                    Player.statDefense -= defenseLoss;

                    // Record the final defense reduction for the stat meter
                    abyssDefenseLossStat = defenseLoss;

                    // Bleed effect based on abyss layer
                    if (ZoneAbyssLayer4)
                    {
                        Player.bleed = true;
                    }
                    else if (ZoneAbyssLayer3)
                    {
                        if (!abyssalDivingSuit)
                            Player.bleed = true;
                    }
                    else if (ZoneAbyssLayer2)
                    {
                        if (!depthCharm)
                            Player.bleed = true;
                    }

                    // Ticks (frames) until breath is deducted from the breath meter
                    double tick = 12D * (1D - depthRatio);

                    // Prevent 0
                    if (tick < 1D)
                        tick = 1D;

                    // Tick (frame) multiplier, depending on gear
                    double tickMult = 1D +
                        (Player.gills ? 4D : 0D) + // 5
                        (oceanCrest ? 4D : 0D) + // 5
                        (Player.ignoreWater ? 5D : 0D) + // 10
                        (Player.accDivingHelm ? 10D : 0D) + // 20
                        (Player.arcticDivingGear ? 10D : 0D) + // 30
                        (aquaticEmblem ? 10D : 0D) + // 40
                        (Player.accMerman ? 15D : 0D) + // 55
                        (victideSet ? 5D : 0D) + // 60
                        ((aquaticHeart && NPC.downedBoss3) ? 15D : 0D) + // 75
                        (abyssalDivingSuit ? 15D : 0D); // 90

                    // Limit the multiplier to 50
                    if (tickMult > 50D)
                        tickMult = 50D;

                    // Increase ticks (frames) until breath is deducted, depending on gear
                    tick *= tickMult;

                    // Record the final breath loss rate for the stat meter
                    abyssBreathLossRateStat = (float)tick;

                    // Reduce breath over ticks (frames)
                    abyssBreathCD++;
                    if (abyssBreathCD >= (int)tick)
                    {
                        // Reset modded breath variable
                        abyssBreathCD = 0;

                        // Reduce breath
                        if (Player.breath > 0)
                            Player.breath -= (int)(cDepth ? breathLoss + 1D : breathLoss);
                    }

                    // If breath is greater than 0 and player has gills or is merfolk, balance out the effects by reducing breath
                    if (Player.breath > 0)
                    {
                        if (Player.gills || Player.merman)
                            Player.breath -= 3;
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

                    // Record the final life loss at zero breath for the stat meter
                    abyssLifeLostAtZeroBreathStat = lifeLossAtZeroBreath;

                    // Check breath value
                    if (Player.breath <= 0)
                    {
                        // Reduce life
                        Player.statLife -= lifeLossAtZeroBreath;

                        // Special kill code if the life loss kills the player
                        if (Player.statLife <= 0)
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

                // Signus headcrab darkness
                if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                {
                    if (CalamityGlobalNPC.signus != -1)
                    {
                        if (Main.npc[CalamityGlobalNPC.signus].active)
                        {
                            if (Vector2.Distance(Main.LocalPlayer.Center, Main.npc[CalamityGlobalNPC.signus].Center) <= 5200f)
                            {
                                float darkRatio = MathHelper.Clamp(caveDarkness, 0f, 1f);
                                float signusLifeRatio = 1f - (Main.npc[CalamityGlobalNPC.signus].life / Main.npc[CalamityGlobalNPC.signus].lifeMax);

                                // Reduce the power of Signus darkness based on your light level.
                                float multiplier = 1f;
                                switch (Main.LocalPlayer.GetCurrentAbyssLightLevel())
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

                                float signusDarkness = signusLifeRatio * multiplier;
                                darkRatio = MathHelper.Clamp(signusDarkness, 0f, 1f);
                                ScreenObstruction.screenObstruction = MathHelper.Lerp(ScreenObstruction.screenObstruction, LightingEffectsSystem.MaxGFBSignusDarkness * -darkRatio, 0.3f);
                            }
                        }
                    }
                }
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

        #region Standing Still Effects
        private void StandingStillEffects()
        {
            // Rogue Stealth
            UpdateRogueStealth();

            // Aquatic Emblem bonus
            if (aquaticEmblem)
            {
                if (Player.IsUnderwater() && Player.wet && !Player.lavaWet && !Player.honeyWet &&
                    !Player.mount.Active)
                {
                    if (aquaticBoost > 0f)
                    {
                        aquaticBoost -= 2f;
                        if (aquaticBoost <= 0f)
                        {
                            aquaticBoost = 0f;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, Player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
                else
                {
                    aquaticBoost += 2f;
                    if (aquaticBoost > aquaticBoostMax)
                        aquaticBoost = aquaticBoostMax;
                    if (Player.mount.Active)
                        aquaticBoost = aquaticBoostMax;
                }

                Player.statDefense += (int)((1f - aquaticBoost * 0.0001f) * 50f);
                Player.moveSpeed -= (1f - aquaticBoost * 0.0001f) * 0.1f;
            }
            else
                aquaticBoost = aquaticBoostMax; modStealth = 1f;

            if (Player.ActiveItem().type == ModContent.ItemType<Auralis>() && Player.StandingStill(0.1f))
            {
                if (auralisStealthCounter < 300f)
                    auralisStealthCounter++;

                bool usingScope = false;
                if (!Main.gameMenu && Main.netMode != NetmodeID.Server)
                {
                    if (Player.noThrow <= 0 && !Player.lastMouseInterface || !(Main.CurrentPan == Vector2.Zero))
                    {
                        if (PlayerInput.UsingGamepad)
                        {
                            if (PlayerInput.GamepadThumbstickRight.Length() != 0f || !Main.SmartCursorIsUsed)
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
                        Vector2 source = Vector2.Normalize(Player.velocity) * new Vector2((float)Player.width / 2f, (float)Player.height) * 1f; //0.75
                        source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Player.Center;
                        Vector2 dustVel = source - Player.Center;
                        int blue = Dust.NewDust(source + dustVel, 0, 0, DustID.Vortex, dustVel.X, dustVel.Y, 100, default, 1.2f);
                        Main.dust[blue].noGravity = true;
                        Main.dust[blue].noLight = false;
                        Main.dust[blue].velocity = dustVel;
                    }
                    for (int d = 0; d < dustAmt; d++)
                    {
                        Vector2 source = Vector2.Normalize(Player.velocity) * new Vector2((float)Player.width / 2f, (float)Player.height) * 0.75f;
                        source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Player.Center;
                        Vector2 dustVel = source - Player.Center;
                        int green = Dust.NewDust(source + dustVel, 0, 0, DustID.TerraBlade, dustVel.X, dustVel.Y, 100, default, 1.2f);
                        Main.dust[green].noGravity = true;
                        Main.dust[green].noLight = false;
                        Main.dust[green].velocity = dustVel;
                    }
                }
                auralisAuroraCounter = 0;
            }
        }
        #endregion

        #region Other Buff Effects
        private void OtherBuffEffects()
        {

            if (gravityNormalizer)
            {
                Player.buffImmune[BuffID.VortexDebuff] = true;
                if (Player.InSpace())
                {
                    Player.gravity = Player.defaultGravity;
                    if (Player.wet)
                    {
                        if (Player.honeyWet)
                            Player.gravity = 0.1f;
                        else if (Player.merman)
                            Player.gravity = 0.3f;
                        else
                            Player.gravity = 0.2f;
                    }
                }
            }

            // Effigy of Decay effects
            if (decayEffigy)
            {
                Player.buffImmune[ModContent.BuffType<SulphuricPoisoning>()] = true;
                if (!ZoneAbyss && Player.IsUnderwater())
                {
                    Player.gills = true;
                }
            }

            // Cobalt armor set effects
            if (CobaltSet)
                CobaltArmorSetChange.ApplyMovementSpeedBonuses(Player);

            // Adamantite armor set effects
            if (AdamantiteSet)
                Player.statDefense += AdamantiteSetDefenseBoost;

            // Warmth makes Chilled, Frozen, and Glacial State tick down 3x as fast
            if (Player.HasBuff(BuffID.Warmth))
            {
                for (int b = 0; b < Player.MaxBuffs; b++)
                {
                    if (Player.buffType[b] == BuffID.Chilled || Player.buffType[b] == BuffID.Frozen || Player.buffType[b] == ModContent.BuffType<GlacialState>())
                    {
                        if (Player.buffTime[b] > 3)
                        {
                            Player.buffTime[b] -= 2;
                        }
                    }
                }
            }

            if (astralInjection)
            {
                if (Player.statMana < Player.statManaMax2)
                    Player.statMana += 2;
                if (Player.statMana > Player.statManaMax2)
                    Player.statMana = Player.statManaMax2;
            }

            if (irradiated)
                Player.statDefense -= 10;

            if (rRage)
            {
                Player.GetDamage<GenericDamageClass>() += 0.1f;
                Player.statDefense += 5;
            }

            if (xRage)
                Player.GetDamage<ThrowingDamageClass>() += 0.1f;

            if (xWrath)
                Player.GetCritChance<RogueDamageClass>() += 5;

            if (graxDefense)
            {
                Player.statDefense += 30;
                Player.endurance += 0.1f;
                Player.GetDamage<MeleeDamageClass>() += 0.2f;
            }

            if (brutalCarnage)
            {
                Player.GetDamage<MeleeDamageClass>() += 0.2f;
            }

            // Trinket of Chi bonus
            if (trinketOfChi)
            {
                if (chiBuffTimer < TrinketofChi.ChiBuffTimerMax)
                    chiBuffTimer++;
                else
                    Player.AddBuff(ModContent.BuffType<ChiBuff>(), 6);
            }
            else
                chiBuffTimer = 0;

            if (darkSunRing)
            {
                Player.maxMinions += 2;
                Player.GetDamage<GenericDamageClass>() += 0.12f;
                Player.GetKnockback<SummonDamageClass>() += 1.2f;
                if (Main.eclipse || !Main.dayTime)
                    Player.statDefense += Main.eclipse ? 10 : 20;
            }

            if (AbsorberRegen)
            {
                Player.GetDamage<GenericDamageClass>() += 0.08f;
                Player.endurance += 0.05f;
            }

            if (crawCarapace)
                Player.GetDamage<GenericDamageClass>() += 0.07f;

            if (baroclaw)
            {
                Player.endurance += 0.05f;
                Player.GetDamage<GenericDamageClass>() += 0.1f;
            }

            if (aeroStone && !Player.slowFall && Player.wingTime < Player.wingTimeMax)
            {
                if (!Player.controlJump && Player.miscCounter % 4 == 0)
                    Player.wingTime += 1;
            }

            if (gShell)
            {
                if (giantShellPostHit == 1)
                    SoundEngine.PlaySound(SoundID.Zombie58, Player.Center);

                if (giantShellPostHit > 0)
                {
                    Player.statDefense -= 3;
                    giantShellPostHit--;
                }
                if (giantShellPostHit < 0)
                {
                    giantShellPostHit = 0;
                }
            }

            if (tortShell)
            {
                if (tortShellPostHit == 1)
                    SoundEngine.PlaySound(SoundID.NPCHit24 with { Volume = 0.5f }, Player.Center);

                if (tortShellPostHit > 0)
                {
                    Player.statDefense -= 10;
                    tortShellPostHit--;
                }
                else
                    Player.endurance += 0.05f;

                if (tortShellPostHit < 0)
                {
                    tortShellPostHit = 0;
                }
            }


            // Ancient Chisel nerf
            if (Player.chiselSpeed)
                Player.pickSpeed += 0.1f;

            if (eGauntlet)
            {
                Player.GetDamage<MeleeDamageClass>() += 0.15f;
                Player.GetCritChance<MeleeDamageClass>() += 5;
            }

            // Gauntlet Melee Speed, prevents glove stacking for melee speed
            if (gloveLevel > 0)
            {
                // Determine the glove the player benefits from in priority of latest in progression
                float gloveAttackSpeed = (gloveLevel == 5 ? 0.15f : gloveLevel == 4 ? 0.14f : gloveLevel == 3 ? 0.12f : gloveLevel <= 2 ? 0.10f : 0);
                Player.GetAttackSpeed<MeleeDamageClass>() += gloveAttackSpeed; // Give the player attack speed based on the glove they have
            }

            // Bloodflare Core's heal over time
            if (bloodflareCore && bloodflareCoreRemainingHealOverTime > 0 && Player.miscCounter % BloodflareCore.HealFrameCooldown == 0)
            {
                Player.statLife += 1;
                Player.HealEffect(1, false);
                if (Player.statLife > Player.statLifeMax2)
                    Player.statLife = Player.statLifeMax2;

                // Produce an implosion of blood themed dust so it's obvious an effect is occurring
                for (int i = 0; i < 3; ++i)
                {
                    Vector2 offset = Main.rand.NextVector2Unit() * Main.rand.NextFloat(23f, 33f);
                    Vector2 dustPos = Player.Center + offset;
                    Vector2 dustVel = offset * -0.08f;
                    Dust d = Dust.NewDustDirect(dustPos, 0, 0, DustID.GemRuby, 0.08f, 0.08f);
                    d.velocity = dustVel;
                    d.noGravity = true;
                }

                // Decrement the remaining possible heal over time
                --bloodflareCoreRemainingHealOverTime;
            }

            // 50% movement speed bonus so that you don't feel like a snail in the early game
            // Disabled while Overhaul is enabled, because Overhaul does very similar things to make movement more snappy
            if (CalamityMod.Instance.overhaul is null && CalamityConfig.Instance.FasterBaseSpeed)
                Player.moveSpeed += BalancingConstants.DefaultMoveSpeedBoost;

            // Reduce how slow Chilled makes the player, because it's cancerous right now
            // The moveSpeed multiplier for Chilled in vanilla is 0.75, so we just multiply by 1.166667 here to make it 0.875, effectively cutting the reduction in half
            if (Player.chilled)
                Player.moveSpeed *= 1f + (1f / 6f);

            if (cirrusDress)
                Player.moveSpeed -= 0.2f;

            if (fabsolVodka)
                Player.GetDamage<GenericDamageClass>() += 0.08f;

            if (vodka)
            {
                Player.GetDamage<GenericDamageClass>() += 0.06f;
                Player.GetCritChance<GenericDamageClass>() += Vodka.CritBoost;
            }

            if (moonshine)
            {
                Player.statDefense += 10;
                Player.endurance += 0.03f;
            }

            if (rum)
                Player.moveSpeed += 0.1f;

            if (whiskey)
            {
                Player.GetDamage<GenericDamageClass>() += 0.04f;
                Player.GetCritChance<GenericDamageClass>() += Whiskey.CritBoost;
            }

            if (everclear)
                Player.GetDamage<GenericDamageClass>() += 0.25f;

            if (bloodyMary)
            {
                if (Main.bloodMoon)
                {
                    Player.GetDamage<GenericDamageClass>() += 0.1f;
                    Player.moveSpeed += 0.1f;
                }
            }

            if (tequila)
            {
                if (Main.dayTime)
                {
                    Player.statDefense += 5;
                    Player.GetCritChance<GenericDamageClass>() += Tequila.CritBoost;
                }
            }

            if (tequilaSunrise)
            {
                if (Main.dayTime)
                {
                    Player.statDefense += 10;
                    Player.GetCritChance<GenericDamageClass>() += TequilaSunrise.CritBoost;
                }
            }

            if (caribbeanRum)
                Player.moveSpeed += 0.1f;

            if (cinnamonRoll)
            {
                Player.manaRegenDelay--;
                Player.manaRegenBonus += 10;
            }

            if (starBeamRye)
            {
                Player.GetDamage<MagicDamageClass>() += 0.08f;
                Player.manaCost *= 0.9f;
                Player.statManaMax2 += 50;
            }

            if (moscowMule)
            {
                Player.GetDamage<GenericDamageClass>() += 0.09f;
                Player.GetCritChance<GenericDamageClass>() += MoscowMule.CritBoost;
            }

            if (oldFashioned)
                Player.GetDamage<GenericDamageClass>() -= OldFashioned.DamageStatReduction;

            if (whiteWine)
                Player.GetDamage<MagicDamageClass>() += 0.08f;

            // Adjustment to the Tipsy debuff
            if (Player.tipsy)
            {
                Player.statDefense += 4;
                Player.GetCritChance<MeleeDamageClass>() -= 2;
                Player.GetAttackSpeed<MeleeDamageClass>() -= 0.1f;
            }

            if (giantPearl)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && !areThereAnyDamnBosses)
                {
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.friendly || npc.dontTakeDamage)
                            continue;
                        float distance = (npc.Center - Player.Center).Length();
                        if (distance < 120f)
                            npc.AddBuff(ModContent.BuffType<PearlAura>(), 20, false);
                    }
                }
            }

            if (CalamityLists.highTestFishList.Contains(Player.ActiveItem().type))
                Player.accFishingLine = true;

            if (planarSpeedBoost != 0)
            {
                if (Player.ActiveItem().type != ModContent.ItemType<PridefulHuntersPlanarRipper>())
                    planarSpeedBoost = 0;
            }
            if (evilSmasherBoost > 0)
            {
                if (Player.ActiveItem().type != ModContent.ItemType<EvilSmasher>())
                    evilSmasherBoost = 0;
            }
            if (searedPanCounter > 0)
            {
                if (Player.ActiveItem().type != ModContent.ItemType<SearedPan>())
                {
                    searedPanCounter = 0;
                    searedPanTimer = 0;
                }
                else if (searedPanTimer < SearedPan.ConsecutiveHitOpening)
                    searedPanTimer++;
                else
                    searedPanCounter = 0;
            }

            // Flight time boosts
            double flightTimeMult = 1D +
                (ZoneAstral ? 0.05 : 0D) +
                (harpyRing ? 0.2 : 0D) +
                (reaverSpeed ? 0.1 : 0D) +
                (angelTreads ? 0.1 : 0D) +
                (blueCandle ? CirrusBlueCandleBuff.WingTimeBoost : 0D) +
                (soaring ? 0.1 : 0D) +
                (prismaticGreaves ? 0.1 : 0D) +
                (plagueReaper ? 0.05 : 0D) +
                (ascendantInsignia ? 0.05 : 0D) + // Added to soaring insignia's flight to get 30%
                (Player.empressBrooch ? 0.25 : 0D);

            if (community)
            {
                float baseBoost = TheCommunity.CalculatePower();
                Player.endurance += baseBoost * TheCommunity.DRMultiplier;
                Player.statDefense += (int)(baseBoost * TheCommunity.DefenseMultiplier);
                Player.GetDamage<GenericDamageClass>() += baseBoost * TheCommunity.DamageMultiplier;
                Player.GetCritChance<GenericDamageClass>() += baseBoost * TheCommunity.CritMultiplier;
                Player.moveSpeed += baseBoost * TheCommunity.SpeedMultiplier;
                flightTimeMult += baseBoost * TheCommunity.FlightMultiplier;
            }
            // Shattered Community gives the same wing time boost as normal Community
            if (shatteredCommunity)
                flightTimeMult += 0.2f;

            if (profanedCrystalBuffs)
            {
                bool offenseBuffs = pscState == (int)ProfanedSoulCrystal.ProfanedSoulCrystalState.Buffs || (Main.dayTime && !Player.wet) || Player.lavaWet;
                if (offenseBuffs || pscState == (int)ProfanedSoulCrystal.ProfanedSoulCrystalState.Empowered)
                    flightTimeMult += 0.1;
            }

            // Reaver Tank set nuke flight time
            if (reaverDefense)
                flightTimeMult -= 0.3f;

            // Increase wing time
            if (Player.wingTimeMax > 0)
                Player.wingTimeMax = (int)(Player.wingTimeMax * flightTimeMult);

            if (vHex)
            {
                Player.statDefense -= 20;

                if (Player.wingTimeMax < 0)
                    Player.wingTimeMax = 0;

                Player.wingTimeMax = (int)(Player.wingTimeMax * 0.75);
            }

            if (icarusFolly)
            {
                if (Player.wingTimeMax < 0)
                    Player.wingTimeMax = 0;

                if (Player.wingTimeMax > 400)
                    Player.wingTimeMax = 400;

                Player.wingTimeMax = (int)(Player.wingTimeMax * 0.66);
            }

            if (DoGExtremeGravity)
            {
                if (Player.wingTimeMax < 0)
                    Player.wingTimeMax = 0;

                if (Player.wingTimeMax > 400)
                    Player.wingTimeMax = 400;

                Player.wingTimeMax = (int)(Player.wingTimeMax * 0.75);
            }

            if (bounding)
            {
                Player.jumpSpeedBoost += 0.25f;
                Player.jumpHeight += 10;
                Player.extraFall += 25;
            }

            if (mushy)
                Player.statDefense += 6;

            if (omniscience)
            {
                Player.detectCreature = true;
                Player.dangerSense = true;
                Player.findTreasure = true;
            }

            if (tarraSet)
            {
                if (!tarraMelee)
                    Player.calmed = true;
                Player.lifeMagnet = true;
            }

            if (wDeath && !purity)
                Player.GetDamage<GenericDamageClass>() -= 0.2f;

            if (astralInfection && !(infectedJewel || purity))
                Player.GetDamage<GenericDamageClass>() -= 0.1f;

            if (pFlames && !purity)
            {
                Player.GetDamage<GenericDamageClass>() -= 0.1f;
            }

            if (aCrunch && !laudanum && !purity)
            {
                Player.statDefense -= ArmorCrunch.DefenseReduction;
                Player.endurance *= ArmorCrunch.MultiplicativeDamageReductionPlayer;
            }

            if (wither && !purity)
            {
                Player.statDefense -= WitherDebuff.DefenseReduction;
            }

            if (gState)
            {
                Player.velocity.X *= 0.5f;
                Player.velocity.Y += 0.05f;
                if (Player.velocity.Y > 15f)
                    Player.velocity.Y = 15f;
            }

            if (eutrophication && !purity)
                Player.velocity = Vector2.Zero;

            if ((vaporfied && !purity) || galvanicCorrosion)
                Player.velocity *= 0.98f;

            if (molluskSet)
                Player.velocity.X *= 0.985f;

            if ((warped || caribbeanRum) && !Player.slowFall && !Player.mount.Active)
            {
                float velocityYMultiplier = (warped && Main.getGoodWorld) ? 1.02f : 1.01f;
                Player.velocity.Y *= velocityYMultiplier;
            }

            if (corrEffigy)
            {
                Player.moveSpeed += 0.1f;
                Player.GetCritChance<GenericDamageClass>() += 10;
            }

            if (crimEffigy)
            {
                Player.GetDamage<GenericDamageClass>() += 0.15f;
                Player.statDefense += 10;
            }

            // The player's true max life value with Calamity adjustments
            actualMaxLife = Player.statLifeMax2;

            if (!Player.dead && healToFull)
            {
                healToFull = false;
                Player.statLife = actualMaxLife;
            }

            if (manaOverloader)
                Player.GetDamage<MagicDamageClass>() += 0.06f;

            if (rBrain)
            {
                if (Player.statLife <= (int)(Player.statLifeMax2 * 0.75))
                    Player.GetDamage<GenericDamageClass>() += 0.1f;
                if (Player.statLife <= (int)(Player.statLifeMax2 * 0.5))
                    Player.moveSpeed -= 0.05f;
            }

            if (bloodyWormTooth)
            {
                Player.GetDamage<MeleeDamageClass>() += 0.1f;
            }

            if (filthyGlove)
            {
                bonusStealthDamage += nanotech ? 0.05f : 0.08f;
            }

            if (frostFlare)
            {
                Player.resistCold = true;
                Player.buffImmune[BuffID.Frostburn] = true;
                Player.buffImmune[BuffID.Chilled] = true;
                Player.buffImmune[BuffID.Frozen] = true;

                if (Player.statLife > (int)(Player.statLifeMax2 * 0.5))
                    Player.GetDamage<GenericDamageClass>() += 0.1f;
                if (Player.statLife <= (int)(Player.statLifeMax2 * 0.5))
                    Player.statDefense += 20;
            }

            if (vexation)
            {
                if (Player.statLife <= (int)(Player.statLifeMax2 * 0.5))
                    Player.GetDamage<GenericDamageClass>() += 0.2f;
            }

            if (ataxiaBlaze)
            {
                if (Player.statLife <= (int)(Player.statLifeMax2 * 0.5))
                    Player.AddBuff(BuffID.Inferno, 2);
            }

            if (bloodflareThrowing)
            {
                if (Player.statLife > (int)(Player.statLifeMax2 * 0.8))
                {
                    Player.GetCritChance<RogueDamageClass>() += 5;
                    Player.statDefense += 30;
                }
                else
                    Player.GetDamage<ThrowingDamageClass>() += 0.1f;
            }

            if (bloodflareSummon)
            {
                if (Player.statLife >= (int)(Player.statLifeMax2 * 0.9))
                    Player.GetDamage<SummonDamageClass>() += 0.1f;
                else if (Player.statLife <= (int)(Player.statLifeMax2 * 0.5))
                    Player.statDefense += 20;

                if (bloodflareSummonTimer > 0)
                    bloodflareSummonTimer--;

                if (Player.whoAmI == Main.myPlayer && bloodflareSummonTimer <= 0)
                {
                    bloodflareSummonTimer = 900;
                    // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                    var source = Player.GetSource_FromThis(BloodflareHeadSummon.GhostMineEntitySourceContext);
                    for (int I = 0; I < 3; I++)
                    {
                        float ai1 = I * 120;

                        // 08DEC2023: Ozzatron: Bloodflare mines spawned with Old Fashioned active will retain their bonus damage indefinitely. Oops. Don't care.
                        int baseDamage = Player.ApplyArmorAccDamageBonusesTo(3750);
                        int damage = Player.CalcIntDamage<SummonDamageClass>(baseDamage);

                        int projectile = Projectile.NewProjectile(source, Player.Center.X + (float)(Math.Sin(I * 120) * 550), Player.Center.Y + (float)(Math.Cos(I * 120) * 550), 0f, 0f,
                            ModContent.ProjectileType<GhostlyMine>(), damage, 1f, Player.whoAmI, ai1, 0f);
                        if (projectile.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[projectile].originalDamage = baseDamage;
                            Main.projectile[projectile].DamageType = DamageClass.Generic;
                        }
                    }
                }
            }

            if (ascendantInsignia && ascendantInsigniaBuffTime > 0)
            {
                ascendantTrail = true;
                infiniteFlight = true;
                if (ascendantInsigniaBuffTime == 1)
                    Player.AddCooldown(AscendEffect.ID, 2400);
                ascendantInsigniaBuffTime--;
            }

            if (abyssalDivingSuit && !Player.IsUnderwater())
            {
                float moveSpeedLoss = (3 - abyssalDivingSuitPlateHits) * 0.2f;
                Player.moveSpeed -= moveSpeedLoss;
            }

            if (ursaSergeant)
                Player.moveSpeed -= 0.15f;

            if (godSlayerThrowing)
            {
                if (Player.statLife >= Player.statLifeMax2)
                {
                    Player.GetCritChance<RogueDamageClass>() += 10;
                    Player.GetDamage<ThrowingDamageClass>() += 0.1f;
                    rogueVelocity += 0.1f;
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
                if (tarraLifeAuraTimer == 0 && Player.whoAmI == Main.myPlayer)
                {
                    int damage = Player.ApplyArmorAccDamageBonusesTo(Player.CalcDamage<SummonDamageClass>(120));

                    // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                    var source = Player.GetSource_FromThis(TarragonHeadSummon.LifeAuraEntitySourceContext);
                    float range = 300f;

                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.friendly || npc.dontTakeDamage)
                            continue;

                        if (Vector2.Distance(Player.Center, npc.Center) <= range)
                            Projectile.NewProjectileDirect(source, npc.Center, Vector2.Zero, ModContent.ProjectileType<TarragonAura>(), damage, 0f, Player.whoAmI, npc.whoAmI);
                    }
                }
            }

            // Navy Fishing Rod's electric aura when in-use
            if (Player.ActiveItem().type == ModContent.ItemType<NavyFishingRod>() && Player.ownedProjectileCounts[ModContent.ProjectileType<NavyBobber>()] != 0)
            {
                const int FramesPerHit = 120;

                // Constantly increment the timer every frame.
                navyRodAuraTimer = (navyRodAuraTimer + 1) % FramesPerHit;

                // If the timer rolls over, it's time to deal damage. Only run this code for the client which is holding the fishing rod,
                if (navyRodAuraTimer == 0 && Player.whoAmI == Main.myPlayer)
                {
                    const int BaseDamage = 10;
                    int damage = (int)Player.GetBestClassDamage().ApplyTo(BaseDamage);
                    var source = Player.GetSource_ItemUse(Player.ActiveItem());
                    float range = 200f;

                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.friendly || npc.damage <= 0 || npc.dontTakeDamage)
                            continue;

                        if (Vector2.Distance(Player.Center, npc.Center) <= range)
                            Projectile.NewProjectileDirect(source, npc.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), damage, 0f, Player.whoAmI, npc.whoAmI);

                        // Occasionally spawn cute sparks so it looks like an electrical aura
                        if (Main.rand.NextBool(10))
                        {
                            Vector2 velocity = CalamityUtils.RandomVelocity(50f, 30f, 60f);
                            int spark = Projectile.NewProjectile(source, npc.Center, velocity, ModContent.ProjectileType<EutrophicSpark>(), damage / 2, 0f, Player.whoAmI);
                            if (spark.WithinBounds(Main.maxProjectiles))
                            {
                                Main.projectile[spark].DamageType = DamageClass.Generic;
                                Main.projectile[spark].localNPCHitCooldown = -2;
                                Main.projectile[spark].penetrate = 5;
                            }
                        }
                    }
                }
            }

            // Inferno potion boost
            if (ataxiaBlaze && Player.inferno)
            {
                const int FramesPerHit = 30;

                // Constantly increment the timer every frame.
                brimLoreInfernoTimer = (brimLoreInfernoTimer + 1) % FramesPerHit;

                // Only run this code for the client which is wearing the armor.
                // Brimstone flames is applied every single frame, but direct damage is only dealt twice per second.
                if (Player.whoAmI == Main.myPlayer)
                {
                    int damage = Player.ApplyArmorAccDamageBonusesTo(Player.GetBestClassDamage().ApplyTo(50));

                    // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                    var source = Player.GetSource_FromThis(HydrothermicArmor.InfernoPotionEntitySourceContext);
                    float range = 300f;

                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.friendly || npc.damage <= 0 || npc.dontTakeDamage)
                            continue;

                        if (Vector2.Distance(Player.Center, npc.Center) <= range)
                        {
                            npc.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
                            if (brimLoreInfernoTimer == 0)
                                Projectile.NewProjectileDirect(source, npc.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), damage, 0f, Player.whoAmI, npc.whoAmI);
                        }
                    }
                }
            }
            #endregion

            if (royalGel)
            {
                Player.npcTypeNoAggro[ModContent.NPCType<AeroSlime>()] = true;
                Player.npcTypeNoAggro[ModContent.NPCType<BloomSlime>()] = true;
                Player.npcTypeNoAggro[ModContent.NPCType<InfernalCongealment>()] = true;
                Player.npcTypeNoAggro[ModContent.NPCType<CrimulanBlightSlime>()] = true;
                Player.npcTypeNoAggro[ModContent.NPCType<CryoSlime>()] = true;
                Player.npcTypeNoAggro[ModContent.NPCType<EbonianBlightSlime>()] = true;
                Player.npcTypeNoAggro[ModContent.NPCType<IrradiatedSlime>()] = true;
                Player.npcTypeNoAggro[ModContent.NPCType<PerennialSlime>()] = true;
                Player.npcTypeNoAggro[ModContent.NPCType<PestilentSlime>()] = true;
                Player.npcTypeNoAggro[ModContent.NPCType<AstralSlime>()] = true;
                Player.npcTypeNoAggro[ModContent.NPCType<GammaSlime>()] = true;
            }

            if (dArtifact)
                Player.GetDamage<GenericDamageClass>() *= 1.2f;

            if (trippy)
                Player.GetDamage<GenericDamageClass>() += 0.5f;

            if (eArtifact)
            {
                Player.manaCost *= 0.75f;
                Player.maxMinions++;
            }

            if (auricSArtifact && Player.FindBuffIndex(ModContent.BuffType<FieryDraconidBuff>()) != -1)
                Player.maxMinions += Player.ownedProjectileCounts[ModContent.ProjectileType<FieryDraconid>()];

            if (pSoulArtifact)
            {
                if (Player.whoAmI == Main.myPlayer)
                {
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<ProfanedSoulArtifact>()));
                    if (Player.FindBuffIndex(ModContent.BuffType<ProfanedSoulGuardians>()) == -1)
                        Player.AddBuff(ModContent.BuffType<ProfanedSoulGuardians>(), 3600, true);

                    pSoulGuardians = true;

                    // 08DEC2023: Ozzatron: PSA/PSC "babs" spawned with Old Fashioned active will retain their bonus damage indefinitely. Oops. Don't care.
                    int guardianAmt = 1;
                    float babCheck = profanedCrystal ? 1f : 0f;
                    int babDamage = profanedCrystal ? 346 : 52;
                    babDamage = Player.ApplyArmorAccDamageBonusesTo(babDamage);

                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianHealer>()] < guardianAmt)
                    {
                        var babH = Projectile.NewProjectileDirect(source, Player.Center, Vector2.UnitY * -6f, ModContent.ProjectileType<MiniGuardianHealer>(), 0, 0f, Main.myPlayer, babCheck);
                        babH.originalDamage = babDamage;
                    }


                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianDefense>()] < guardianAmt)
                    {
                        var babD = Projectile.NewProjectileDirect(source, Player.Center, Vector2.UnitY * -3f, ModContent.ProjectileType<MiniGuardianDefense>(), 1, 1f, Main.myPlayer, babCheck);
                        babD.originalDamage = babDamage;
                    }

                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<MiniGuardianAttack>()] < guardianAmt)
                    {
                        float spearCounter = profanedCrystal ? 60 * 8 : 15f;
                        var babO = Projectile.NewProjectileDirect(source, Player.Center, Vector2.UnitY * -1f, ModContent.ProjectileType<MiniGuardianAttack>(), 1, 1f, Main.myPlayer, babCheck, spearCounter);
                        babO.originalDamage = babDamage;
                    }
                }
            }

            if (profanedCrystalBuffs)
            {
                if (Player.whoAmI == Main.myPlayer)
                {
                    bool empowered = pscState == (int)ProfanedSoulCrystal.ProfanedSoulCrystalState.Empowered;
                    Player.lavaImmune = true;
                    Player.fireWalk = true;
                    Player.buffImmune[ModContent.BuffType<HolyFlames>()] = Main.dayTime || empowered;
                    Player.buffImmune[ModContent.BuffType<Nightwither>()] = !Main.dayTime || empowered;
                    Player.buffImmune[BuffID.OnFire] = true;
                    Player.buffImmune[BuffID.Burning] = true;
                    Player.buffImmune[BuffID.Daybreak] = true;
                    bool offenseBuffs = (Main.dayTime && !Player.wet) || Player.lavaWet || empowered;
                    if (offenseBuffs)
                    {
                        Player.GetDamage<SummonDamageClass>() += 0.15f;
                        Player.GetKnockback<SummonDamageClass>() += 0.15f;
                        Player.moveSpeed += 0.1f;
                        if (!empowered)
                            Player.statDefense -= 15;
                        Player.ignoreWater = true;
                        Player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 1f; //this only ever affects psc whip and should not be problematic
                    }
                    else if (empowered || !offenseBuffs)
                    {
                        Player.endurance += 0.05f;
                        Player.statDefense += 15;
                        Player.lifeRegen += 5;
                    }
                    bool enrage = pscState >= (int)ProfanedSoulCrystal.ProfanedSoulCrystalState.Enraged;
                    if (!ZoneAbyss) //No abyss memes.
                        Lighting.AddLight(Player.Center, enrage ? 1.2f : offenseBuffs ? 1f : 0.2f, enrage ? 0.21f : offenseBuffs ? 0.2f : 0.01f, 0);

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
                if (Player.ownedProjectileCounts[summonDeleteList[i]] > projAmt)
                {
                    for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
                    {
                        Projectile proj = Main.projectile[projIndex];
                        if (proj.active && proj.owner == Player.whoAmI)
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
                if (Player.whoAmI == Main.myPlayer)
                {
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<BlunderBooster>()));
                    int damage = Player.ApplyArmorAccDamageBonusesTo(Player.CalcDamage<RogueDamageClass>(30));

                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<BlunderBoosterAura>()] < 1)
                        Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<BlunderBoosterAura>(), damage, 0f, Player.whoAmI, 0f, 0f);
                }
            }
            else if (Player.ownedProjectileCounts[ModContent.ProjectileType<BlunderBoosterAura>()] != 0)
            {
                if (Player.whoAmI == Main.myPlayer)
                {
                    foreach (Projectile p in Main.ActiveProjectiles)
                    {
                        if (p.type == ModContent.ProjectileType<BlunderBoosterAura>() && p.owner == Player.whoAmI)
                        {
                            p.Kill();
                            break;
                        }
                    }
                }
            }

            if (tesla)
            {
                if (Player.whoAmI == Main.myPlayer)
                {
                    // Reduce the buffTime of Electrified.
                    bool reduceElectrifiedDuration = false;
                    for (int l = 0; l < Player.MaxBuffs; l++)
                    {
                        if (Player.buffType[l] == ModContent.BuffType<TeslaBuff>())
                        {
                            if (Player.buffTime[l] >= 5)
                                reduceElectrifiedDuration = true;
                        }
                    }

                    if (reduceElectrifiedDuration)
                    {
                        for (int l = 0; l < Player.MaxBuffs; l++)
                        {
                            if (Player.buffType[l] == BuffID.Electrified)
                            {
                                if (Player.buffTime[l] > 2)
                                {
                                    Player.buffTime[l]--;
                                    break;
                                }
                            }
                        }
                    }

                    // Summon the aura (this check is here to prevent out of bounds errors).
                    if (reduceElectrifiedDuration)
                    {
                        // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                        var source = Player.GetSource_Buff(Player.FindBuffIndex(ModContent.BuffType<TeslaBuff>()));
                        int damage = (int)Player.GetBestClassDamage().ApplyTo(10);
                        if (Player.ownedProjectileCounts[ModContent.ProjectileType<TeslaAura>()] < 1)
                            Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<TeslaAura>(), damage, 0f, Player.whoAmI);
                    }
                }
            }
            else if (Player.ownedProjectileCounts[ModContent.ProjectileType<TeslaAura>()] > 0)
            {
                if (Player.whoAmI == Main.myPlayer)
                {
                    int auraType = ModContent.ProjectileType<TeslaAura>();
                    foreach (Projectile p in Main.ActiveProjectiles)
                    {
                        if (p.type != auraType || p.owner != Player.whoAmI)
                            continue;

                        p.Kill();
                        break;
                    }
                }
            }

            if (CryoStone || CryoStoneVanity)
            {
                var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<CryoStone>()));
                int damage = (int)Player.GetBestClassDamage().ApplyTo(70);
                damage = Player.ApplyArmorAccDamageBonusesTo(damage);

                if (Player.whoAmI == Main.myPlayer && Player.ownedProjectileCounts[ModContent.ProjectileType<CryonicShield>()] == 0)
                    Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<CryonicShield>(), damage, 0f, Player.whoAmI);
            }
            else if (Player.whoAmI == Main.myPlayer)
            {
                int shieldType = ModContent.ProjectileType<CryonicShield>();
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.type != shieldType || p.owner != Player.whoAmI)
                        continue;

                    p.Kill();
                    break;
                }
            }

            if (prismaticLasers > 1800 && Player.whoAmI == Main.myPlayer)
            {
                float shootSpeed = 18f;
                int dmg = (int)Player.GetTotalDamage<MagicDamageClass>().ApplyTo(30);
                dmg = Player.ApplyArmorAccDamageBonusesTo(dmg);

                Vector2 startPos = Player.RotatedRelativePoint(Player.MountedCenter, true);
                Vector2 velocity = Main.MouseWorld - startPos;
                if (Player.gravDir == -1f)
                {
                    velocity.Y = Main.screenPosition.Y + Main.screenHeight - Main.mouseY - startPos.Y;
                }
                float travelDist = velocity.Length();
                if ((float.IsNaN(velocity.X) && float.IsNaN(velocity.Y)) || (velocity.X == 0f && velocity.Y == 0f))
                {
                    velocity.X = Player.direction;
                    velocity.Y = 0f;
                    travelDist = shootSpeed;
                }
                else
                {
                    travelDist = shootSpeed / travelDist;
                }

                // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                var source = Player.GetSource_FromThis(PrismaticHelmet.LaserEntitySourceContext);
                int laserAmt = Main.rand.Next(2);
                for (int index = 0; index < laserAmt; index++)
                {
                    startPos = new Vector2(Player.Center.X + (Main.rand.Next(201) * -(float)Player.direction) + (Main.mouseX + Main.screenPosition.X - Player.position.X), Player.MountedCenter.Y - 600f);
                    startPos.X = (startPos.X + Player.Center.X) / 2f + Main.rand.Next(-200, 201);
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
                    int laser = Projectile.NewProjectile(source, startPos, velocity, ModContent.ProjectileType<DeathhailBeam>(), dmg, 4f, Player.whoAmI, 0f, 0f);
                    Main.projectile[laser].localNPCHitCooldown = 5;
                    if (laser.WithinBounds(Main.maxProjectiles))
                        Main.projectile[laser].DamageType = DamageClass.Generic;
                }
                SoundEngine.PlaySound(SoundID.Item12, Player.Center);
            }
            if (prismaticLasers == 1800)
            {
                // At the exact moment the lasers stop, set the cooldown to appear
                Player.AddCooldown(PrismaticLaser.ID, 1800);
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
                    Vector2 source = Vector2.Normalize(Player.velocity) * new Vector2(Player.width / 2f, Player.height) * 0.75f;
                    source = source.RotatedBy((dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / dustAmt, default) + Player.Center;
                    Vector2 dustVel = source - Player.Center;
                    int dusty = Dust.NewDust(source + dustVel, 0, 0, DustID.RainbowMk2, dustVel.X * 1f, dustVel.Y * 1f, 100, color, 1f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].noLight = true;
                    Main.dust[dusty].velocity = dustVel;
                }
            }

            if (angelicAlliance && Main.myPlayer == Player.whoAmI)
            {
                for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    int hasBuff = Player.buffType[l];
                    if (hasBuff == ModContent.BuffType<Buffs.StatBuffs.DivineBless>())
                        angelicActivate = Player.buffTime[l];
                }

                if (Player.FindBuffIndex(ModContent.BuffType<Buffs.StatBuffs.DivineBless>()) == -1)
                    angelicActivate = -1;

                if (angelicActivate == 1)
                    Player.AddCooldown(Cooldowns.DivineBless.ID, CalamityUtils.SecondsToFrames(60));
            }

            if (theBee && Player.statLife >= Player.statLifeMax2 && (!HasAnyEnergyShield || TotalEnergyShielding >= TotalMaxShieldDurability))
            {
                float beeBoost = Player.endurance / 2f;
                Player.GetDamage<GenericDamageClass>() += beeBoost;
            }

            if (badgeOfBravery)
            {
                Player.GetDamage<MeleeDamageClass>() += 0.1f;
                Player.GetCritChance<MeleeDamageClass>() += 10;
            }

            // Amalgam boosts
            if (Main.myPlayer == Player.whoAmI)
            {
                for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    int hasBuff = Player.buffType[l];
                    if (CalamityLists.amalgamBuffList.Contains(hasBuff))
                    {
                        if (amalgam)
                        {
                            // Every other frame, increase the buff timer by one frame. Thus, the buff lasts twice as long.
                            if (Player.miscCounter % 2 == 0)
                                Player.buffTime[l] += 1;

                            // Buffs will not go away when you die, to prevent wasting potions.
                            if (!Main.persistentBuff[hasBuff])
                                Main.persistentBuff[hasBuff] = true;
                        }
                        else
                        {
                            // Reset buff persistence if Amalgam is removed.
                            if (Main.persistentBuff[hasBuff] && !CalamityLists.persistentBuffList.Contains(hasBuff))
                                Main.persistentBuff[hasBuff] = false;
                        }
                    }
                }
            }

            // Laudanum boosts
            if (laudanum)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    for (int l = 0; l < Player.MaxBuffs; l++)
                    {
                        int hasBuff = Player.buffType[l];
                        if (hasBuff == ModContent.BuffType<ArmorCrunch>() || hasBuff == BuffID.Obstructed ||
                            hasBuff == BuffID.Ichor || hasBuff == BuffID.Chilled || hasBuff == BuffID.BrokenArmor || hasBuff == BuffID.Weak ||
                            hasBuff == BuffID.Slow || hasBuff == BuffID.Confused || hasBuff == BuffID.Blackout || hasBuff == BuffID.Darkness)
                        {
                            // Every other frame, increase the buff timer by one frame. Thus, the buff lasts twice as long.
                            if (Player.miscCounter % 2 == 0)
                                Player.buffTime[l] += 1;
                        }

                        // See later as Laud cancels out the normal effects
                        if (hasBuff == ModContent.BuffType<ArmorCrunch>())
                        {
                            // +15 defense
                            Player.statDefense += ArmorCrunch.DefenseReduction;
                        }

                        switch (hasBuff)
                        {
                            case BuffID.Obstructed:
                                Player.headcovered = false;
                                Player.statDefense += 50;
                                Player.GetDamage<GenericDamageClass>() += 0.5f;
                                Player.GetCritChance<GenericDamageClass>() += 25;
                                break;
                            case BuffID.Ichor:
                                Player.statDefense += 40;
                                break;
                            case BuffID.Chilled:
                                Player.chilled = false;
                                Player.moveSpeed *= 1.3f;
                                break;
                            case BuffID.BrokenArmor:
                                Player.brokenArmor = false;
                                Player.statDefense += (int)(Player.statDefense * 0.25);
                                break;
                            case BuffID.Weak:
                                Player.GetDamage<MeleeDamageClass>() += 0.151f;
                                Player.statDefense += 14;
                                Player.moveSpeed += 0.3f;
                                break;
                            case BuffID.Slow:
                                Player.slow = false;
                                Player.moveSpeed *= 1.5f;
                                break;
                            case BuffID.Confused:
                                Player.confused = false;
                                Player.statDefense += 30;
                                Player.GetDamage<GenericDamageClass>() += 0.25f;
                                Player.GetCritChance<GenericDamageClass>() += 10;
                                break;
                            case BuffID.Blackout:
                                Player.blackout = false;
                                Player.statDefense += 30;
                                Player.GetDamage<GenericDamageClass>() += 0.25f;
                                Player.GetCritChance<GenericDamageClass>() += 10;
                                break;
                            case BuffID.Darkness:
                                Player.blind = false;
                                Player.statDefense += 15;
                                Player.GetDamage<GenericDamageClass>() += 0.1f;
                                Player.GetCritChance<GenericDamageClass>() += 5;
                                break;
                        }
                    }
                }
            }

            // Endurance reductions
            EnduranceReductions();

            if (spectralVeilImmunity > 0)
            {
                int numDust = 2;
                for (int i = 0; i < numDust; i++)
                {
                    int dustIndex = Dust.NewDust(Player.position, Player.width, Player.height, DustID.VilePowder, 0f, 0f);
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

        #region Energy Shields
        private void EnergyShields()
        {
            // Because later tier shields are brighter, shields are handled from highest tier to lowest tier here.
            bool shieldAddedLight = false;

            // If The Sponge is not equipped, obliterate its durability cooldown.
            // The recharge cooldown is intentionally left in place to prevent hot swapping to recharge the shield
            if (!sponge)
            {
                if (cooldowns.TryGetValue(SpongeDurability.ID, out var cdDurability))
                    cdDurability.timeLeft = 0;

                // As The Sponge's shield can be left in a partially recharged state, this is for safety.
                // If the player does not have the accessory equipped for even one frame, discharge all shields.
                SpongeShieldDurability = 0;
            }
            else
            {
                // If The Sponge's shield is discharged and hasn't started its recharge delay, start that.
                if (SpongeShieldDurability == 0 && !cooldowns.ContainsKey(SpongeRecharge.ID))
                    Player.AddCooldown(SpongeRecharge.ID, TheSponge.ShieldRechargeDelay);

                // If the shield has greater than zero durability but that durability is not on the cooldown rack, add it to the cooldown rack.
                if (SpongeShieldDurability > 0 && !cooldowns.ContainsKey(SpongeDurability.ID))
                {
                    var durabilityCooldown = Player.AddCooldown(SpongeDurability.ID, TheSponge.ShieldDurabilityMax);
                    durabilityCooldown.timeLeft = SpongeShieldDurability;
                }

                // If the shield has greater than zero durability and isn't in its recharge delay, actively replenish shield points.
                // Play a sound on the first frame this occurs.
                if (SpongeShieldDurability > 0 && !cooldowns.ContainsKey(SpongeRecharge.ID))
                {
                    if (!playedSpongeShieldSound)
                        SoundEngine.PlaySound(TheSponge.ActivationSound, Player.Center);
                    playedSpongeShieldSound = true;

                    // This number is not an integer, and stores exact per-frame recharge progress.
                    spongeShieldPartialRechargeProgress += TheSponge.ShieldDurabilityMax / (float)TheSponge.TotalShieldRechargeTime;

                    // Floor the value to get whole number of shield points recharged this frame.
                    int pointsActuallyRecharged = (int)MathF.Floor(spongeShieldPartialRechargeProgress);

                    // Give those points to the real shield durability, capping the result. Then remove them from recharge progress.
                    SpongeShieldDurability = Math.Min(SpongeShieldDurability + pointsActuallyRecharged, TheSponge.ShieldDurabilityMax);
                    spongeShieldPartialRechargeProgress -= pointsActuallyRecharged;

                    // Update the cooldown rack's durability indicator.
                    if (cooldowns.TryGetValue(SpongeDurability.ID, out var cdDurability))
                        cdDurability.timeLeft = SpongeShieldDurability;
                }

                // Add light if this shield is currently active
                if (SpongeShieldDurability > 0 && !shieldAddedLight)
                {
                    // The Sponge is much brigher than other shields
                    Lighting.AddLight(Player.Center, Color.White.ToVector3() * 0.75f);
                    shieldAddedLight = true;
                }
            }

            // If PSA/PSC is not equipped, obliterate its durability cooldown.
            // The recharge cooldown is intentionally left in place to prevent hot swapping to recharge the shield
            if (!pSoulArtifact)
            {
                if (cooldowns.TryGetValue(Cooldowns.ProfanedSoulShield.ID, out var cdDurability))
                    cdDurability.timeLeft = 0;

                // As PSA/PSC's shield can be left in a partially recharged state, this is for safety.
                // If the player does not have the accessory equipped for even one frame, discharge all shields.
                pSoulShieldDurability = 0;
            }
            // Stuff to do if PSA/PSC is equipped
            else
            {
                //Force check if profaned crystal buffs are active
                ProfanedSoulCrystal.DetermineTransformationEligibility(Player);
                int maxDurability = profanedCrystalBuffs
                    ? ProfanedSoulCrystal.ShieldDurabilityMax
                    : ProfanedSoulArtifact.ShieldDurabilityMax;
                int delay = profanedCrystalBuffs
                    ? ProfanedSoulCrystal.ShieldRechargeDelay
                    : ProfanedSoulArtifact.ShieldRechargeDelay;
                int totalRecharge = profanedCrystalBuffs
                    ? ProfanedSoulCrystal.TotalShieldRechargeTime
                    : ProfanedSoulArtifact.TotalShieldRechargeTime;

                if (pSoulShieldDurability == 0 && !cooldowns.ContainsKey(Cooldowns.ProfanedSoulShieldRecharge.ID))
                    Player.AddCooldown(ProfanedSoulShieldRecharge.ID, delay);

                // If the shield has greater than zero durability but that durability is not on the cooldown rack, add it to the cooldown rack.
                if (pSoulShieldDurability > 0 && !cooldowns.ContainsKey(Cooldowns.ProfanedSoulShield.ID))
                {
                    var durabilityCooldown = Player.AddCooldown(Cooldowns.ProfanedSoulShield.ID, maxDurability);
                    durabilityCooldown.timeLeft = pSoulShieldDurability;
                }

                // If the shield has greater than zero durability and isn't in its recharge delay, actively replenish shield points.
                // Play a sound on the first frame this occurs.
                if (pSoulShieldDurability > 0 && !cooldowns.ContainsKey(ProfanedSoulShieldRecharge.ID))
                {
                    if (!playedProfanedSoulShieldSound)
                        SoundEngine.PlaySound(ProvidenceBoss.BurnStartSound, Player.Center);
                    playedProfanedSoulShieldSound = true;

                    // This number is not an integer, and stores exact per-frame recharge progress.
                    pSoulShieldPartialRechargeProgress += maxDurability / (float)totalRecharge;

                    // Floor the value to get whole number of shield points recharged this frame.
                    int pointsActuallyRecharged = (int)MathF.Floor(pSoulShieldPartialRechargeProgress);

                    // Give those points to the real shield durability, capping the result. Then remove them from recharge progress.
                    pSoulShieldDurability = Math.Min(pSoulShieldDurability + pointsActuallyRecharged, maxDurability);
                    pSoulShieldPartialRechargeProgress -= pointsActuallyRecharged;

                    // Update the cooldown rack's durability indicator.
                    if (cooldowns.TryGetValue(Cooldowns.ProfanedSoulShield.ID, out var cdDurability))
                        cdDurability.timeLeft = pSoulShieldDurability;
                }

                // Add light if this shield is currently active
                if (pSoulShieldDurability > 0 && !shieldAddedLight)
                {
                    Lighting.AddLight(Player.Center, Color.Orange.ToVector3() * 0.4f);
                    shieldAddedLight = true;
                }
            }
            // If the Lunic Corps armor is not equipped, obliterate its durability cooldown.
            // The recharge cooldown is intentionally left in place to prevent hot swapping to recharge the shield
            if (!lunicCorpsSet)
            {
                if (cooldowns.TryGetValue(Cooldowns.LunicCorpsShieldDurability.ID, out var cdDurability))
                    cdDurability.timeLeft = 0;

                // As the Lunic Corps armor's shield can be left in a partially recharged state, this is for safety.
                // If the player does not have the armor equipped for even one frame, discharge all shields.
                LunicCorpsShieldDurability = 0;
            }

            // Stuff to do if the Lunic Corps armor is equipped
            else
            {
                // If the Lunic Corps shield is discharged and hasn't started its recharge delay, start that.
                if (LunicCorpsShieldDurability == 0 && !cooldowns.ContainsKey(LunicCorpsShieldRecharge.ID))
                    Player.AddCooldown(LunicCorpsShieldRecharge.ID, LunicCorpsHelmet.ShieldRechargeDelay);

                // If the shield has greater than zero durability but that durability is not on the cooldown rack, add it to the cooldown rack.
                if (LunicCorpsShieldDurability > 0 && !cooldowns.ContainsKey(Cooldowns.LunicCorpsShieldDurability.ID))
                {
                    var durabilityCooldown = Player.AddCooldown(Cooldowns.LunicCorpsShieldDurability.ID, LunicCorpsHelmet.ShieldDurabilityMax);
                    durabilityCooldown.timeLeft = LunicCorpsShieldDurability;
                }

                // If the shield has greater than zero durability and isn't in its recharge delay, actively replenish shield points.
                // Play a sound on the first frame this occurs.
                if (LunicCorpsShieldDurability > 0 && !cooldowns.ContainsKey(LunicCorpsShieldRecharge.ID))
                {
                    if (!playedLunicCorpsShieldSound)
                        SoundEngine.PlaySound(LunicCorpsHelmet.ActivationSound, Player.Center);
                    playedLunicCorpsShieldSound = true;

                    // This number is not an integer, and stores exact per-frame recharge progress.
                    lunicCorpsShieldPartialRechargeProgress += LunicCorpsHelmet.ShieldDurabilityMax / (float)LunicCorpsHelmet.TotalShieldRechargeTime;

                    // Floor the value to get whole number of shield points recharged this frame.
                    int pointsActuallyRecharged = (int)MathF.Floor(lunicCorpsShieldPartialRechargeProgress);

                    // Give those points to the real shield durability, capping the result. Then remove them from recharge progress.
                    LunicCorpsShieldDurability = Math.Min(LunicCorpsShieldDurability + pointsActuallyRecharged, LunicCorpsHelmet.ShieldDurabilityMax);
                    lunicCorpsShieldPartialRechargeProgress -= pointsActuallyRecharged;

                    // Update the cooldown rack's durability indicator.
                    if (cooldowns.TryGetValue(Cooldowns.LunicCorpsShieldDurability.ID, out var cdDurability))
                        cdDurability.timeLeft = LunicCorpsShieldDurability;
                }

                // Add light if this shield is currently active
                if (LunicCorpsShieldDurability > 0 && !shieldAddedLight)
                {
                    Lighting.AddLight(Player.Center, Color.DeepSkyBlue.ToVector3() * 0.2f);
                    shieldAddedLight = true;
                }
            }

            // If the Rover Drive is not equipped, obliterate its durability cooldown.
            // The recharge cooldown is intentionally left in place to prevent hot swapping to recharge the shield
            if (!roverDrive)
            {
                if (cooldowns.TryGetValue(WulfrumRoverDriveDurability.ID, out var cdDurability))
                    cdDurability.timeLeft = 0;
                // if (cooldowns.TryGetValue(WulfrumRoverDriveRecharge.ID, out var cdRecharge)) cdRecharge.timeLeft = 0;
            }

            // Stuff to do if the Rover Drive is equipped
            else
            {
                // If the Rover Drive shield is discharged and hasn't started its recharge delay, start that.
                if (RoverDriveShieldDurability == 0 && !cooldowns.ContainsKey(WulfrumRoverDriveRecharge.ID))
                    Player.AddCooldown(WulfrumRoverDriveRecharge.ID, RoverDrive.ShieldRechargeTime);

                // If the shield has greater than zero durability but that durability is not on the cooldown rack,
                // add it to the cooldown rack and play a sound of the shield turning on.
                if (RoverDriveShieldDurability > 0 && !cooldowns.ContainsKey(WulfrumRoverDriveDurability.ID))
                {
                    CooldownInstance durabilityCooldown = Player.AddCooldown(WulfrumRoverDriveDurability.ID, RoverDrive.ShieldDurabilityMax);
                    durabilityCooldown.timeLeft = RoverDriveShieldDurability;
                }

                // Add light if this shield is currently active
                if (RoverDriveShieldDurability > 0 && !shieldAddedLight)
                {
                    Lighting.AddLight(Player.Center, Color.DeepSkyBlue.ToVector3() * 0.2f);
                    shieldAddedLight = true;
                }
            }
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
            // CalamityPlayer has a function called DealDefenseDamage to handle everything for you when dealing defense damage.
            //
            // The player's current recovery through defense damage is tracked through two frame counts:
            // defenseDamageRecoveryFrames = How many more frames the player will still be recovering from defense damage
            // totalDefenseDamageRecoveryFrames = The total timer for defense damage recovery that the player is undergoing
            //
            // Defense damage does not heal during iframes, and has a delay after they end before it starts recovering.
            if (totalDefenseDamage > 0)
            {
                // Defense damage is capped at your maximum defense, except in GFB.
                if (!Main.getGoodWorld && totalDefenseDamage > Player.statDefense)
                    totalDefenseDamage = Player.statDefense;

                // You cannot begin recovering from defense damage until your iframes wear off.
                if (!Player.HasIFrames())
                {
                    // Delay before defense damage recovery can start. While this delay is ticking down, defense damage doesn't recover at all.
                    if (defenseDamageDelayFrames > 0)
                        --defenseDamageDelayFrames;

                    // Once the delay is up, defense damage recovery actually occurs.
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
                }

                // Get current amount of defense damage to apply this frame.
                int currentDefenseDamage = CurrentDefenseDamage;

                // Apply DR Damage.
                //
                // DR Damage is applied at exactly the same ratio as defense damage;
                // if you lose half your defense to defense damage, you also lose half your DR.
                // This is applied first because the math would be wrong if the player's defense was already reduced by defense damage.
                if (Player.statDefense > 0 && Player.endurance > 0f)
                {
                    float drDamageRatio = currentDefenseDamage / (float)Player.statDefense;
                    Player.endurance *= 1f - drDamageRatio;
                }

                // Apply defense damage
                Player.statDefense -= currentDefenseDamage;
                
            }

            // Defense can never be reduced below zero, no matter what
            if (Player.statDefense < 0)
                Player.statDefense *= 0;

            // Multiplicative defense reductions.
            // These are done last because they need to be after the defense lower cap at 0.
            if (fabsolVodka)
            {
                if (Player.statDefense > 0)
                    Player.statDefense -= (int)(Player.statDefense * 0.05);
            }

            if (vodka)
            {
                if (Player.statDefense > 0)
                    Player.statDefense -= (int)(Player.statDefense * 0.05);
            }

            if (grapeBeer)
            {
                if (Player.statDefense > 0)
                    Player.statDefense -= (int)(Player.statDefense * 0.03);
            }

            if (rum)
            {
                if (Player.statDefense > 0)
                    Player.statDefense -= (int)(Player.statDefense * 0.05);
            }

            if (whiskey)
            {
                if (Player.statDefense > 0)
                    Player.statDefense -= (int)(Player.statDefense * 0.05);
            }

            if (everclear)
            {
                if (Player.statDefense > 0)
                    Player.statDefense -= (int)(Player.statDefense * 0.3);
            }

            if (bloodyMary)
            {
                if (Main.bloodMoon)
                {
                    if (Player.statDefense > 0)
                        Player.statDefense -= (int)(Player.statDefense * 0.04);
                }
            }

            if (caribbeanRum)
            {
                if (Player.statDefense > 0)
                    Player.statDefense -= (int)(Player.statDefense * 0.1);
            }

            if (cinnamonRoll)
            {
                if (Player.statDefense > 0)
                    Player.statDefense -= (int)(Player.statDefense * 0.1);
            }

            if (margarita)
            {
                if (Player.statDefense > 0)
                    Player.statDefense -= (int)(Player.statDefense * 0.06);
            }

            if (starBeamRye)
            {
                if (Player.statDefense > 0)
                    Player.statDefense -= (int)(Player.statDefense * 0.06);
            }

            if (whiteWine)
            {
                if (Player.statDefense > 0)
                    Player.statDefense -= (int)(Player.statDefense * 0.06);
            }

            if (Player.tipsy)
            {
                if (Player.statDefense > 0)
                    Player.statDefense -= (int)(Player.statDefense * 0.05);
            }

            if (DesertProwlerHat.ShroudedInSmoke(Player, out _))
                Player.statDefense -= (int)(Player.statDefense * 0.75);
        }
        #endregion

        #region Limits
        private void Limits()
        {
            // TODO -- what is Forbidden Circlet actually supposed to do?
            if (forbiddenCirclet)
            {
                ref StatModifier summon = ref Player.GetDamage<SummonDamageClass>();
                ref StatModifier rogue = ref Player.GetDamage<RogueDamageClass>();
                float boostToSummonFromRogue = 0f;
                float boostToRogueFromSummon = 0f;

                if (summon.Additive < rogue.Additive)
                    boostToSummonFromRogue = rogue.Additive - summon.Additive;
                if (rogue.Additive < summon.Additive)
                    boostToRogueFromSummon = summon.Additive - rogue.Additive;

                summon += boostToSummonFromRogue;
                rogue += boostToRogueFromSummon;
            }

            // 10% is converted to 9%, 25% is converted to 20%, 50% is converted to 33%, 75% is converted to 43%, 100% is converted to 50%
            if (Player.endurance > 0f)
                Player.endurance = 1f - (1f / (1f + Player.endurance));

            // Do not apply reduced aggro if there are any bosses alive and it's singleplayer
            if (areThereAnyDamnBosses && Main.netMode == NetmodeID.SinglePlayer)
            {
                if (Player.aggro < 0)
                    Player.aggro = 0;
            }
        }
        #endregion

        #region Endurance Reductions
        private void EnduranceReductions()
        {
            if (vHex)
                Player.endurance -= 0.1f;

            if (irradiated)
                Player.endurance -= 0.1f;

            if (corrEffigy)
                Player.endurance -= 0.05f;
        }
        #endregion

        #region Text Chat Messages
        private void HandleTextChatMessages()
        {
            if (Player.whoAmI != Main.myPlayer || Main.netMode == NetmodeID.Server)
                return;

            if (startMessageDisplayDelay >= 0)
            {
                if (startMessageDisplayDelay == 0)
                {
                    if (CalamityConfig.Instance.WikiStatusMessage)
                    {
                        CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Misc.WikiStatus1");
                        CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Misc.WikiStatus2");
                    }
                }

                --startMessageDisplayDelay;
            }
        }
        #endregion

        #region Mouse Item Checks
        public void CheckIfMouseItemIsSchematic()
        {
            if (Main.myPlayer != Player.whoAmI)
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

        #region Androomba Right Click
        public void AndroombaRightClick()
        {
            if (Main.myPlayer != Player.whoAmI)
                return;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type != ModContent.NPCType<AndroombaFriendly>())
                    continue;

                bool holdingsol = ((Player.HeldItem.type >= ItemID.GreenSolution && Player.HeldItem.type <= ItemID.RedSolution) || (Player.HeldItem.type >= ItemID.SandSolution && Player.HeldItem.type <= ItemID.DirtSolution) || Player.HeldItem.type == ModContent.ItemType<AstralSolution>());
                int heldType = -1;
                for (int e = 0; e < AndroombaFriendly.customConversionTypes.Count; e++)
                {
                    var entry = AndroombaFriendly.customConversionTypes[e];
                    if (Player.HeldItem.type == entry.Item1)
                    {
                        holdingsol = true;
                        heldType = e;
                        break;
                    }
                }
                if (npc.Hitbox.Contains(Main.MouseWorld.ToPoint()) && holdingsol && Player.Distance(npc.Center) < 450)
                {
                    Player.cursorItemIconEnabled = true;
                    Player.cursorItemIconID = Player.HeldItem.type;
                    Player.cursorItemIconText = "";
                    npc.ShowNameOnHover = false;

                    if (Main.mouseRight && Main.mouseRightRelease && Player.Distance(npc.Center) < 300)
                    {
                        npc.netUpdate = true;

                        int soltype = 0;
                        if (Player.HeldItem.type == ModContent.ItemType<AstralSolution>())
                        {
                            soltype = 8;
                        }
                        else
                        {
                            switch (Player.HeldItem.type)
                            {
                                case ItemID.GreenSolution:
                                    soltype = 0;
                                    break;
                                case ItemID.PurpleSolution:
                                    soltype = 1;
                                    break;
                                case ItemID.BlueSolution:
                                    soltype = 2;
                                    break;
                                case ItemID.DarkBlueSolution:
                                    soltype = 3;
                                    break;
                                case ItemID.RedSolution:
                                    soltype = 4;
                                    break;
                                case ItemID.SandSolution:
                                    soltype = 5;
                                    break;
                                case ItemID.SnowSolution:
                                    soltype = 6;
                                    break;
                                case ItemID.DirtSolution:
                                    soltype = 7;
                                    break;
                                default:
                                    soltype = heldType + 9;
                                    break;
                            }
                        }
                        if (npc.ai[3] != soltype || npc.ai[0] == 0)
                        {
                            Player.ConsumeItem(Player.HeldItem.type);
                            SoundEngine.PlaySound(SoundID.Item87);
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                AndroombaFriendly.SwapSolution(npc.whoAmI, soltype);
                            }
                            else
                            {
                                var netMessage = Mod.GetPacket();
                                netMessage.Write((byte)CalamityModMessageType.SyncAndroombaSolution);
                                netMessage.Write(npc.whoAmI);
                                netMessage.Write(soltype);
                                netMessage.Send();
                            }
                            if (npc.ai[0] == 0f)
                            {
                                if (Main.netMode == NetmodeID.SinglePlayer)
                                {
                                    AndroombaFriendly.ChangeAI(npc.whoAmI, 1);
                                }
                                else
                                {
                                    var netMessage = Mod.GetPacket();
                                    netMessage.Write((byte)CalamityModMessageType.SyncAndroombaAI);
                                    netMessage.Write(npc.whoAmI);
                                    netMessage.Write(1);
                                    netMessage.Send();
                                }
                            }
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                        }
                    }
                }

                else
                    npc.ShowNameOnHover = true;
            }
        }
        #endregion

        #region Potion Handling
        private void HandlePotions()
        {
            // Hadal Stew
            if (potionTimer > 0)
                potionTimer--;
            if (potionTimer > 0 && Player.potionDelay == 0)
                Player.potionDelay = potionTimer;
            if (potionTimer == 1)
            {
                // Reduced duration than normal
                int duration = HadalStew.SicknessDuration;
                if (Player.pStone)
                    duration = (int)(duration * 0.75);
                Player.ClearBuff(BuffID.PotionSickness);
                Player.AddBuff(BuffID.PotionSickness, duration);
            }

            if (PlayerInput.Triggers.JustPressed.QuickBuff)
            {
                for (int i = 0; i < Main.InventorySlotsTotal; ++i)
                {
                    Item item = Player.inventory[i];

                    if (Player.potionDelay > 0 || potionTimer > 0)
                        break;
                    if (item is null || item.stack <= 0)
                        continue;

                    if (item.type == ModContent.ItemType<HadalStew>())
                        CalamityUtils.ConsumeItemViaQuickBuff(Player, item, HadalStew.BuffType, HadalStew.BuffDuration, true);
                    if (item.type == ModContent.ItemType<Margarita>())
                        CalamityUtils.ConsumeItemViaQuickBuff(Player, item, Margarita.BuffType, Margarita.BuffDuration, false);
                }
            }
        }
        #endregion
    }
}
