using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Balancing;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Cooldowns;
using CalamityMod.CustomRecipes;
using CalamityMod.DataStructures;
using CalamityMod.Dusts;
using CalamityMod.Enums;
using CalamityMod.Events;
using CalamityMod.ExtraTextures;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Armor.Aerospec;
using CalamityMod.Items.Armor.Auric;
using CalamityMod.Items.Armor.Bloodflare;
using CalamityMod.Items.Armor.Brimflame;
using CalamityMod.Items.Armor.DesertProwler;
using CalamityMod.Items.Armor.Empyrean;
using CalamityMod.Items.Armor.GodSlayer;
using CalamityMod.Items.Armor.Hydrothermic;
using CalamityMod.Items.Armor.LunicCorps;
using CalamityMod.Items.Armor.OmegaBlue;
using CalamityMod.Items.Armor.PlagueReaper;
using CalamityMod.Items.Armor.Prismatic;
using CalamityMod.Items.Armor.Reaver;
using CalamityMod.Items.Armor.Silva;
using CalamityMod.Items.Armor.Tarragon;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.Mounts.Minecarts;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.Potions.Food;
using CalamityMod.Items.Tools;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Other;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Packets;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Abyss.AbyssAmbient;
using CalamityMod.Tiles.FurnitureAuric;
using CalamityMod.Tiles.Ores;
using CalamityMod.UI;
using CalamityMod.UI.DialogueDisplay;
using CalamityMod.UI.DialogueDisplay.DisplayEffects;
using CalamityMod.Utilities;
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
using Terraria.Localization;
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

            // Give the player a 20% jump speed boost while wings are equipped
            if (Player.wingsLogic > 0)
                Player.jumpSpeedBoost += 1f;

            // Decrease the counter on Fearmonger set turbo regeneration
            if (fearmongerRegenFrames > 0)
                fearmongerRegenFrames--;

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

            // Potions (Quick Buff && Potion Sickness)
            HandlePotions();

            // Display the Music Mod Reminder text in chat.
            HandleTextChatMessages();

            // Check if schematics are present on the mouse, for the sake of registering their recipes.
            CheckIfMouseItemIsSchematic();

            // Handle Androomba's Right Click function
            AndroombaRightClick();

            // Drawing parameters
            UpdateDrawingParameters();

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

                if (syncMouseRightClick)
                {
                    syncMouseRightClick = false;
                    MouseRightClickSync();
                }

                mouseWorldPacketTimer = Math.Min(mouseWorldPacketTimer + 1, MouseWorldPacketInterval);
                if (mouseWorldPacketTimer >= MouseWorldPacketInterval)
                {
                    if (syncMousePosition)
                    {
                        mouseWorldPacketTimer = 0;
                        syncMousePosition = false;
                        syncMouseRotation = false; // Rotation also get update on position packet
                        MousePositionSync();
                    }

                    if (syncMouseRotation)
                    {
                        mouseWorldPacketTimer = 0;
                        syncMouseRotation = false;
                        MouseRotationSync();
                    }
                }
            }

            if (Player.HeldItem.type != ModContent.ItemType<SaharaSlicers>())
                saharaSlicersBolts = 0;

            // Spawning for items that have holdouts active when held
            if (Player.whoAmI == Main.myPlayer && !Player.dead && !Main.mapFullscreen && !Player.mouseInterface)
            {
                if (Player.HeldItem.type == ModContent.ItemType<Starfleet>() && (Player.ownedProjectileCounts[ModContent.ProjectileType<StarfleetHoldout>()] == 0))
                {
                    int damage = (int)Player.GetTotalDamage<RangedDamageClass>().ApplyTo(Player.HeldItem.damage);
                    Projectile.NewProjectile(Player.GetProjectileSource_Item_WithPotentialAmmo(Player.HeldItem, 0), Player.Center, Player.Center.DirectionTo(Player.Calamity().mouseWorld), ModContent.ProjectileType<StarfleetHoldout>(), damage, Player.HeldItem.knockBack, Player.whoAmI);
                }
                if (Player.HeldItem.type == ModContent.ItemType<Starmada>() && (Player.ownedProjectileCounts[ModContent.ProjectileType<StarmadaHoldout>()] == 0))
                {
                    int damage = (int)Player.GetTotalDamage<RangedDamageClass>().ApplyTo(Player.HeldItem.damage);
                    Projectile.NewProjectile(Player.GetProjectileSource_Item_WithPotentialAmmo(Player.HeldItem, 0), Player.Center, Player.Center.DirectionTo(Player.Calamity().mouseWorld), ModContent.ProjectileType<StarmadaHoldout>(), damage, Player.HeldItem.knockBack, Player.whoAmI);
                }
                if (Player.HeldItem.type == ModContent.ItemType<Basher>() && (Player.ownedProjectileCounts[ModContent.ProjectileType<BasherHoldout>()] == 0))
                {
                    int damage = (int)Player.GetTotalDamage<TrueMeleeDamageClass>().ApplyTo(Player.HeldItem.damage);
                    Projectile.NewProjectile(Player.GetProjectileSource_Item_WithPotentialAmmo(Player.HeldItem, 0), Player.Center, Player.Center.DirectionTo(Player.Calamity().mouseWorld), ModContent.ProjectileType<BasherHoldout>(), damage, Player.HeldItem.knockBack, Player.whoAmI);
                }
                if (Player.HeldItem.type == ModContent.ItemType<GrandDad>() && (Player.ownedProjectileCounts[ModContent.ProjectileType<GrandDadHoldout>()] == 0))
                {
                    int damage = (int)Player.GetTotalDamage<TrueMeleeDamageClass>().ApplyTo(Player.HeldItem.damage);
                    Projectile.NewProjectile(Player.GetProjectileSource_Item_WithPotentialAmmo(Player.HeldItem, 0), Player.Center, Player.Center.DirectionTo(Player.Calamity().mouseWorld), ModContent.ProjectileType<GrandDadHoldout>(), damage, Player.HeldItem.knockBack, Player.whoAmI);
                }
            }

            // De-equipping Gael's Greatsword deletes all rage.
            if (Player.HeldItem.type == ModContent.ItemType<GaelsGreatsword>())
                heldGaelsLastFrame = true;
            else if (heldGaelsLastFrame)
            {
                heldGaelsLastFrame = false;
                rage = 0f;
            }

            bool holdingElephantKiller = Player.HeldItem.type == ModContent.ItemType<ElephantKiller>();
            if (holdingElephantKiller && !heldElephantKillerLastFrame)
            {
                Player.Calamity().rogueStealth = 0;
                heldElephantKillerLastFrame = true;
            }
            else if (!holdingElephantKiller)
                heldElephantKillerLastFrame = false;

            if (!drawingElephantKillerJoke)
                elephantKillerJoke = 0;
            else
                elephantKillerJoke++;
            drawingElephantKillerJoke = false;

            if (furyFuel < FuryFuelMax && furyRefuelTimer >= 0)
            {
                furyFuel += (int)furyRefuelTimer;
                furyRefuelTimer = MathHelper.Lerp(furyRefuelTimer, 25, 0.01f);
                if (furyFuel > FuryFuelMax)
                    furyFuel = FuryFuelMax;
            }
            else if (furyRefuelTimer < 0)
                furyRefuelTimer++;

            // De-equipping Draedon's Heart deletes all Adrenaline.
            if (!draedonsHeart && hadNanomachinesLastFrame)
            {
                hadNanomachinesLastFrame = false;
                adrenaline = 0f;
            }

            // Apply stealth damage to rogue.
            bool dontProvideStealthDamage = Player.HeldItem.type == ModContent.ItemType<ElephantKiller>();
            if (!dontProvideStealthDamage)
                Player.GetDamage<RogueDamageClass>() += stealthDamage;

            if ((XykVisualsBlue || XykVisualsOrange))
            {
                bool Orange = XykVisualsOrange;
                Color effectColor = Orange ? XyksBlessingOrange.baseMainColor : XyksBlessingBlue.baseMainColor;

                float rate = Main.GlobalTimeWrappedHourly * 12;
                List<Color> eColors = new List<Color>()
                {
                    Orange ? XyksBlessingOrange.baseMainColor : XyksBlessingBlue.baseMainColor,
                    Orange ? XyksBlessingOrange.baseAccentColor : XyksBlessingBlue.baseAccentColor,
                    Orange ? XyksBlessingOrange.baseEffectColor : XyksBlessingBlue.baseEffectColor
                };
                int colorIndex = (int)(rate / 2 % eColors.Count);
                Color currentColor = eColors[colorIndex];
                Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
                effectColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);

                bool rageOrAdren = (Player.Calamity().rageModeActive || Player.Calamity().adrenalineModeActive);
                bool rageAndAdren = (Player.Calamity().rageModeActive && Player.Calamity().adrenalineModeActive);
                Color dashColor = Orange ? XyksBlessingOrange.animEffectColor : XyksBlessingBlue.animEffectColor;
                Color attemptColor = (rageAndAdren ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB) : 
                    Player.Calamity().adrenalineModeActive ? Color.MediumSpringGreen : Player.Calamity().rageModeActive ? Color.Crimson :
                    Player.dashDelay == -1 ? dashColor : effectColor);
                XykFXColor = Color.Lerp(XykFXColor, attemptColor, rageOrAdren ? 0.05f : 0.25f);


                int maxWingPieces = 7;
                int numOfActiveWings = 0;
                foreach (Projectile p in Main.ActiveProjectiles)
                    if (p.type == ModContent.ProjectileType<XykWings>() && p.owner == Player.whoAmI && p.ai[1] == 0)
                        numOfActiveWings++;
                bool spawnWings = numOfActiveWings < maxWingPieces && !Player.dead && Player.wingsLogic > 0 && ((!(Player.wingTime == Player.wingTimeMax && Player.velocity.Y == 0) && Player.wingTime > 0) || XykWingTimer >= 3);
                if (spawnWings)
                {
                    if (XykWingTimer >= 3)
                    {
                        int wingCount = numOfActiveWings;
                        Projectile wings = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<XykWings>(), 0, 0f, Player.whoAmI, wingCount);
                        if (numOfActiveWings + 1 == maxWingPieces)
                            XykWingTimer = 0;
                    }
                    else
                        XykWingTimer++;
                }
                else
                    XykWingTimer = 0;
            }

            // First Frame dash effects
            bool dashStart = (Player.Calamity().DashID == DefaultDash.ID && Player.timeSinceLastDashStarted == 1 && Player.dashDelay != 0 ) || (Player.dashDelay == -1 && ((!HasCustomDash && IsFirstDashFrame) || (HasCustomDash && UsedDash.DashTimeAdjustedForStartup == 1)));
            int dir = MathF.Sign(Player.velocity.X);

            if (dashStart)
            {
                lastDashWasTabi = false;

                if ((tortShell && tortShellPostHit == 0))
                    Player.velocity.X *= GiantTortoiseShell.DashVelocityMult;
                if ((gShell && giantShellPostHit == 0))
                    Player.velocity.X *= GiantShell.DashVelocityMult;
            }

            // Tabi/Master Ninja Gear dash change
            if (Player.dashDelay != -1)
                lastDashWasTabi = false;
            else if (lastDashWasTabi)
            {
                Player.dashType = 1;
            }
            if (Player.dashType == 1 && !Player.Calamity().statisNinjaBelt && !Player.Calamity().statisVoidSash)
            {
                if (dashStart)
                {
                    Player.velocity.X *= 2f;
                    lastDashWasTabi = true;
                }
                if (Player.dashDelay == -1)
                {
                    Player.velocity.X *= 0.95f;
                    if (Player.timeSinceLastDashStarted > 12)
                    {
                        Player.velocity.X *= 0.75f;
                    }
                }
            }
            if ((devilsDevastationKillMode || exaltedKillMode) && !Player.mount.Active)
            {
                float fxScale = 1;
                if (exaltedKillMode)
                {
                    if (Player.wingTime > 0 && Player.miscCounter % 2 == 0)
                        Player.wingTime++;

                    if (Player.miscCounter % 4 == 0)
                        Player.HealPlayer(1, HealTextType.None);

                    if (Player.dashDelay > 1) // Reduced dash cooldown
                    {
                        Player.dashDelay = 1;
                    }
                    if (Player.dashDelay == -1)
                    {
                        fxScale = 1.5f;
                    }
                }
                else
                {
                    if (Player.wingTime > 0 && Player.miscCounter % 3 == 0)
                        Player.wingTime += 2;

                    if (Player.miscCounter % 5 == 0)
                        Player.HealPlayer(2, HealTextType.None);

                    if (Player.dashDelay > 1) // Reduced dash cooldown
                    {
                        Player.dashDelay = 1;
                    }
                    if (Player.dashDelay == -1)
                    {
                        fxScale = 1.5f;
                    }
                }

                if (Player.velocity.Length() > 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Particle spark2 = new CustomSpark(Player.Center + Main.rand.NextVector2Circular(20, 20), -Player.velocity * Main.rand.NextFloat(0.3f, 0.8f) * fxScale, "CalamityMod/Particles/DemonSigilParticle", false, 22, Main.rand.NextFloat(0.2f, 0.3f) * fxScale, Color.Lerp(Color.MediumOrchid, Color.BlueViolet, Main.rand.NextFloat(0, 0.7f)) * 0.8f, new Vector2(1, 1), true, false, 0, false, false, fxScale - 1);
                        GeneralParticleHandler.SpawnParticle(spark2);
                    }
                    else
                    {
                        // Spawn in a helix-style pattern
                        float sine = (float)Math.Sin(Player.miscCounter * 0.575f / MathHelper.Pi);
                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 offset = Player.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * sine * 16f;
                            float scale = Main.rand.NextFloat(0.8f, 1.6f) * fxScale;

                            Dust dust2 = Dust.NewDustPerfect(Player.Center + offset * (i == 0 ? -1 : 1) * fxScale, ModContent.DustType<LightDust>(), Player.velocity * Main.rand.NextFloat(0.2f, 0.5f));
                            dust2.noGravity = true;
                            dust2.scale = scale;
                            dust2.color = Main.rand.NextBool() ? Color.MediumOrchid : Color.BlueViolet;
                        }
                    }
                }

                Lighting.AddLight(Player.Center, Color.MediumOrchid.ToVector3());
            }

            // Restore flight time during Vortex stealth with Vortex Booster
            if (Player.vortexStealthActive && Player.wingsLogic == (int)VanillaWingID.WingsVortex)
            {
                if (Player.wingTime > 0 && Player.miscCounter % 3 == 0)
                    Player.wingTime++;
            }

            if (Player.HeldItem.type == ModContent.ItemType<UnstableCastersGauntlet>() && unstableCastersGauntletVis < 100)
            {

                unstableCastersGauntletVisTimer++;
                // Gain 0.1% charge once every 4 ticks
                if (unstableCastersGauntletVisTimer >= 4)
                {
                    unstableCastersGauntletVis += 0.1f;
                    unstableCastersGauntletVisTimer = 0;
                }

            }
            if (unstableCastersGauntletVis >= 100)
                unstableCastersGauntletVis = 100;

            if (lAmbergris)
            {
                if (dashStart)
                {
                    Player.velocity.X *= 1 + LeviathanAmbergris.DashSpeedIncrease;
                    int damage = (int)Player.GetBestClassDamage().ApplyTo(LeviathanAmbergris.ambergrisDashDamage);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<LeviAmberDash>(), damage, 0f, Player.whoAmI);
                }

                if (Player.miscCounter % 3 == 2 && Player.dashDelay > 0) // Reduced dash cooldown by 33%
                    Player.dashDelay--;

                if (Player.dashDelay == -1 && !(HasCustomDash && UsedDash.DashTimeAdjustedForStartup < 1))
                    Player.velocity.X *= 0.9f;
            }

            if (sPauldron)
            {
                if (dashStart)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { Volume = 0.4f, PitchVariance = 0.4f }, Player.Center);
                    int damage = (int)Player.GetBestClassDamage().ApplyTo(SlagsplitterPauldron.PauldronSlamDamage);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + Player.velocity * 1.5f, Vector2.Zero, ModContent.ProjectileType<PauldronDash>(), damage, 0, Player.whoAmI);
                }
                if (Player.dashDelay == -1)
                {
                    Player.endurance += 0.1f;
                    Player.noKnockback = true;
                }

            }

            if (v8Engine || v8000Engine)
            {
                if (Player.dashDelay == 1 && !v8EngineFXPlayed)
                {

                    SoundStyle maxEnergyReached = new("CalamityMod/Sounds/Custom/ScornJump");
                    SoundEngine.PlaySound(maxEnergyReached with { Volume = 0.7f }, Player.Center);

                    for (int i = 0; i < 10; i++) // Circular ring of particles burst from player
                    {
                        float angle = MathHelper.TwoPi * (i / 10f);
                        Vector2 spawnDirection = angle.ToRotationVector2();
                        Vector2 velocity = spawnDirection * 18f;

                        CritSpark spark = new CritSpark(Player.Center + spawnDirection * 3f, velocity, Color.Lerp(Player.Calamity().DashID == V8EngineDash.ID ? Color.OrangeRed : Color.Cyan, Color.White, Main.rand.NextFloat(1f)), Color.White * 0.5f, 1.2f, 12, 0.3f, 1.2f);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    v8EngineFXPlayed = true;
                }

                if (Player.dashDelay == -1)
                    v8EngineFXPlayed = false;
            }

            if (XykVisualsBlue || XykVisualsOrange)
            {
                bool Orange = XykVisualsOrange;

                if (dashStart)
                {
                    SoundStyle dash = new("CalamityMod/Sounds/Item/DashSound");
                    SoundEngine.PlaySound(dash with { Volume = 0.7f, Pitch = Main.rand.NextFloat(0f, 0.2f) + (Orange ? 0 : 0.2f) }, Player.Center);
                    if (Orange)
                    {
                        Particle spark1 = new CustomPulse(Player.Center, Vector2.Zero, XykFXColor, "CalamityMod/Particles/GlowSquareParticleBig", Vector2.One, MathHelper.PiOver4, 0.45f, 0.25f, 47);
                        GeneralParticleHandler.SpawnParticle(spark1);

                        Particle spark2 = new CustomPulse(Player.Center, Vector2.Zero, XykFXColor, "CalamityMod/Particles/GlowSquareParticleBig", Vector2.One, MathHelper.PiOver4, 0, 0.8f, 17);
                        GeneralParticleHandler.SpawnParticle(spark2);
                    }
                    else
                    {
                        Particle spark1 = new CustomPulse(Player.Center, Player.velocity * 0.3f, XykFXColor, "CalamityMod/Particles/BloomRing", new Vector2(0.4f, 1f), Player.velocity.ToRotation(), 0, 1f, 24);
                        GeneralParticleHandler.SpawnParticle(spark1);

                        Particle spark2 = new CustomPulse(Player.Center, Player.velocity * 0.5f, XykFXColor, "CalamityMod/Particles/BloomRing", new Vector2(0.5f, 1f), Player.velocity.ToRotation(), 0, 0.75f, 17);
                        GeneralParticleHandler.SpawnParticle(spark2);
                    }
                }
                if (Player.dashDelay == -1)
                {
                    float sparkscale1 = MathF.Min(Player.velocity.X * dir * 0.08f, 1.2f);
                    Vector2 SparkVelocity1 = -Player.velocity.SafeNormalize(Vector2.UnitX) * 5;

                    if (!Orange)
                    {
                        float sine = (float)Math.Sin(Player.miscCounter * 0.875f / MathHelper.Pi);
                        for (int i = -1; i <= 1; i += 2)
                        {
                            Vector2 offset = Player.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * sine * 18f * i;

                            Particle spark1 = new CustomSpark(Player.Center + offset + SparkVelocity1, SparkVelocity1 * 2 * sparkscale1, "CalamityMod/Particles/BloomCircle", false, 15, 0.3f * sparkscale1, XykFXColor * 0.85f, new Vector2(0.9f, 1.4f), true, true, 0, shrinkSpeed: 0.2f, glowOpacity: 0.85f);
                            GeneralParticleHandler.SpawnParticle(spark1);
                        }
                    }
                    else
                    {
                        float sparkscale2 = MathF.Min(Player.velocity.X * dir * 0.07f, 1.1f);
                        for (int i = -1; i <= 1; i += 2)
                        {
                            Vector2 offset = Player.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * 17f * i;

                            Particle spark1 = new CustomSpark(Player.Center + offset + SparkVelocity1, SparkVelocity1 * 2 * sparkscale1, "CalamityMod/Particles/BloomCircle", false, 20, 0.4f * sparkscale1, XykFXColor * 0.95f, new Vector2(0.3f, 1.2f), true, true, 0, shrinkSpeed: 0.2f, glowOpacity: 0.95f);
                            GeneralParticleHandler.SpawnParticle(spark1);
                        }
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        bool altDust = Main.rand.NextBool(3);
                        Dust dust = Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2Circular(20, 30) + SparkVelocity1, altDust ? (Orange ? ModContent.DustType<SquareDust>() : ModContent.DustType<SquashDustHollow>()) : ModContent.DustType<SquashDust>(), SparkVelocity1 * Main.rand.NextFloat(0.5f, 3f) * sparkscale1, 0, default, Main.rand.NextFloat(1.5f, 1.9f) * sparkscale1);
                        dust.noGravity = true;
                        dust.color = XykFXColor;
                        dust.fadeIn = altDust ? 0 : 2;
                        if (altDust)
                            dust.scale *= 0.5f;
                    }
                }
            }

            if (sandCloak && dashStart)
            {
                // Spawn sand veil when dashing if it does not exist and you do not have the cooldown
                if (!(CalamityUtils.AnyProjectiles(ModContent.ProjectileType<SandCloakVeil>()) || Player.HasCooldown(Cooldowns.SandCloak.ID)))
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<SandCloakVeil>(), 10, 2.5f, Player.whoAmI);
                    SoundEngine.PlaySound(SoundID.Item45, Player.Center);
                }
            }


            if (cinnamonRoll && !(Main.getGoodWorld && Main.npc.Any(x=> x.active && x.type == ModContent.NPCType<DevourerofGodsHead>())))
            {
                if (dashStart)
                    Player.velocity.X *= 2f;
                else if  (Player.dashDelay == -1)
                    Player.velocity.X *= 0.825f;
            }
            if (Player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.CinnamonRoll) && !(Main.getGoodWorld && Main.npc.Any(x => x.active && x.type == ModContent.NPCType<DevourerofGodsHead>())))
            {
                if (dashStart)
                    Player.velocity.X *= 2f;
                else if (Player.dashDelay == -1)
                    Player.velocity.X *= 0.825f;
            }

            if (Player.dashDelay == -1)
                IsFirstDashFrame = false;
            else
                IsFirstDashFrame = true;

            // THIS MUST BE NEAR THE END OF PostUpdateMiscEffects SO ALL OTHER RUN SPEED IS DONE FIRST. DO NOT PUT ANY RUN SPEED AFTER THIS

            // Multiplies movement speed by 1.5x so that you don't feel like a snail in the early game.
            // This applies to movement speed boosts as well as base speed to ensure they are actually worth their listed value compared to base speed
            //
            // Disabled while Overhaul is enabled, because Overhaul does very similar things to make movement more snappy
            // 22JUN2025: Ozzatron: Disabled while Remnants is enabled, because Remnants has its own move speed reworks.

            bool ignoreSpeedConfig = ExternalMods.overhaul is not null || ExternalMods.remnants is not null;
            if (!ignoreSpeedConfig && CalamityServerConfig.Instance.FasterBaseSpeed)
                Player.moveSpeed *= BalancingConstants.DefaultMoveSpeedBoost;

            // This is used to increase horizontal velocity based on the player's movement speed stat.
            moveSpeedBonus = Player.moveSpeed - 1f;
        }
        #endregion

        #region Revengeance Effects
        private void RevengeanceModeMiscEffects()
        {
            if (CalamityWorld.revenge)
            {
                if (Player.whoAmI == Main.myPlayer)
                {
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

                // Shattered Community provides constant rage generation and overrides the value from Heart of Darkness.
                if (shatteredCommunity)
                {
                    float scRageGen = rageMax * ShatteredCommunity.RagePerSecond / 60f;
                    if (rageGen < scRageGen)
                        rageGen = scRageGen;
                }

                // 26MAR2025: Ozzatron: Shattered Community and Heart of Darkness have equal rage generation now.
                // It's easier to not change the code here to unify the cases, because there is no reason to.
                //
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

            float rate = Main.GlobalTimeWrappedHourly * 29;
            List<Color> eColors = new List<Color>()
            {
                Color.PaleVioletRed,
                Color.Coral,
                Color.Khaki,
                Color.PaleGreen,
                Color.Turquoise,
                Color.Violet
            };

            int colorIndex = (int)(rate / 2 % eColors.Count);
            Color currentColor = eColors[colorIndex];
            Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
            lightRGB = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);

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
                        if (npc.IsABoss() && !CalamityNPCSets.BossSegmentThatDoesNotGenerateRageFaster[npc.type])
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
            // CIT 24NOV2025: Fixed an exploit which allowed Rage to persist forever if Revengeance is toggled off while active.
            if (RageEnabled || rageDiff < 0f)
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
                // 26AUG2024: Ozzatron: Cut Draedon's Heart healing in half by making it heal every other frame.
                if (draedonsHeart && Player.miscCounter % 2 == 1)
                {
                    Player.HealPlayer(DraedonsHeart.NanomachinesHealPerFrame, HealTextType.None);

                    // Old Draedon's Heart dust effect from its standing still regen. Works just fine.
                    int dustID = DustID.TerraBlade;
                    {
                        Dust regen = Dust.NewDustDirect(Player.position, Player.width, Player.height, dustID, 0f, 0f, 200, default, 1f);
                        regen.noGravity = true;
                        regen.fadeIn = 1.3f;
                        Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                        regen.velocity = velocity;
                        velocity.Normalize();
                        velocity *= 34f;
                        regen.position = Player.Center - velocity;
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
            //
            // CIT 24NOV2025: Adrenaline cannot be exploited the same way as Rage, since difficulty cannot be changed while a boss is alive.
            // Nevertheless, I will still give it the same exploit fix as Rage just in case there is some method to do it.
            if ((AdrenalineEnabled || adrenalineDiff < 0f) && adrenalinePauseTimer == 0)
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

            // Auric Ore causes an Auric Rejection unless you are wearing Auric Armor or have God Mode
            // Auric Rejection causes an electrical explosion that yeets the player a considerable distance
            // CIT 17AUG2024: Despite providing full invulnerability, Silva armor revive intentionally does not prevent Auric rejection's yeeting.
            // 25FEB2025 Ozzatron: Added external bool to control Auric Rejection immunity from the ore.
            bool rejectionImmunity = auricSet || seraphTracers || Player.creativeGodMode || externalAuricRejectionImmunity;
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

                    Dust dust = Dust.NewDustDirect(Player.Center, 16, 16, DustID.Firefly, 0.2f, 0f, 0, new Color(117, 55, 15), Main.rand.NextFloat(1f, 2f));
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.fadeIn = 2.5f;
                }

                // Ores below here
                // Seraph Tracers give immunity to block contact effects
                if (!seraphTracers)
                {
                    // Astral Ore inflicts Astral Infection briefly on contact
                    if (tile.TileType == astralOreID)
                        Player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 2);

                    // You will need to set each resistant item here for burning as it gets bypassed for somereason
                    if (tile.TileType == scoriaOreID && !Player.fireWalk)
                        Player.AddBuff(BuffID.Burning, 2);
                }

                bool oreRejection = (tile.TileType == auricOreID) && !rejectionImmunity;

                // Repulsers always perform this effect because they are player-placed tiles made for this exact purpose
                bool repulserRejection = tile.TileType == auricRepulserID;

                if (oreRejection || repulserRejection)
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
                        Player.Hurt(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.AuricRejection").ToNetworkText(Player.name)), auricRejectionDamage, 0);
                        Player.AddBuff(ModContent.BuffType<AuricRebuke>(), 120);
                    }
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/ExoMechs/TeslaShoot1"), Player.Center);
                }
            }

            if (Player.sitting.TryGetSittingBlock(Player, out Tile AuricToiletTile) && !rejectionImmunity)
            {
                if (AuricToiletTile.TileType == ModContent.TileType<AuricToiletTile>())
                {
                    Player.Hurt(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.AuricRejection").ToNetworkText(Player.name)), auricRejectionDamage, 0);
                    Player.AddBuff(ModContent.BuffType<AuricRebuke>(), 120);
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/ExoMechs/TeslaShoot1"), Player.Center);
                    Player.sitting.SitUp(Player, false);
                    Vector2 yeetVec = new Vector2(Player.direction, -1.5f);
                    Player.velocity += yeetVec * auricRejectionKB;
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

            // 14NOV2024: Ozzatron: was not sure whether to let Calamity function outside normal cursor range. decided on no
            //
            // Check whether the mouse cursor is within the acceptable clamped range. If it's not, don't allow for damage.
            Vector2 mouseTruePosition = Player.Calamity().mouseWorld;
            Vector2 mouseClampedPosition = Player.ClampedMouseWorld();
            if (mouseTruePosition != mouseClampedPosition)
                return;

            Rectangle sigilHitbox = Utils.CenteredRectangle(mouseClampedPosition, new Vector2(35f, 62f));
            int sigilDamage = (int)Player.GetBestClassDamage().ApplyTo(Calamity.BaseDamage);

            bool brightenedSigil = false;
            foreach (NPC target in Main.ActiveNPCs)
            {
                if (!target.Hitbox.Intersects(sigilHitbox) || target.immortal || target.dontTakeDamage || target.friendly || NPCID.Sets.CountsAsCritter[target.type])
                    continue;

                // Increment the cursor focus counter. Note this actually has a net of increasing by 2 per frame, due to the 1 per frame falloff in GlobalNPC.
                // If this counter reaches 300 (which takes 2.5 seconds without interruptions), True VHex is activated.
                if (target.Calamity().cursorFocus < CalamityGlobalNPC.cursorFocusMax)
                {
                    target.Calamity().cursorFocus += 3;

                    // Draw an expanding orb effect on the cursor based on the cursor focus value.
                    float cursorFocusRatio = target.Calamity().cursorFocus / (float)CalamityGlobalNPC.cursorFocusMax;
                    StrongBloom indicator = new(mouseClampedPosition, Vector2.Zero, Color.Lerp(Color.Magenta, Color.Red, cursorFocusRatio), cursorFocusRatio * 0.7f, 2);
                    GeneralParticleHandler.SpawnParticle(indicator);

                    if (target.Calamity().cursorFocus >= CalamityGlobalNPC.cursorFocusMax)
                    {
                        int vHexDuration = 0;
                        if (target.HasBuff<VulnerabilityHex>())
                            vHexDuration = target.buffTime[target.FindBuffIndex(ModContent.BuffType<VulnerabilityHex>())];
                        target.AddBuff(ModContent.BuffType<TrueVulnerabilityHex>(), vHexDuration <= 300 ? vHexDuration : 300);
                        target.RequestBuffRemoval(ModContent.BuffType<VulnerabilityHex>());
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/WeaponEnchant"), target.Center);

                        for (int i = 0; i < 18; i++)
                        {
                            Vector2 orbVel = new Vector2(15, 15).RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(0.33f, 1f);
                            GlowOrbParticle orb = new(target.Center + orbVel, orbVel, false, 60, Main.rand.NextFloat(0.95f, 1.75f), Color.Lerp(Color.Red, Color.Magenta, 0.3f));
                            GeneralParticleHandler.SpawnParticle(orb);
                        }
                    }
                }

                // miscCounter is used to limit Calamity's hit rate.
                if (Player.miscCounter % Calamity.FramesPerHit != 1)
                    return;

                // Brighten the sigil because it is dealing damage. This can only happen once per hit event.
                if (!brightenedSigil)
                {
                    blazingMouseAuraFade = MathHelper.Clamp(blazingMouseAuraFade + 0.2f, 0.25f, 1f);
                    brightenedSigil = true;
                }

                // Create a direct strike to hit this specific NPC.
                var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<Calamity>()));
                Projectile sigilStrike = Projectile.NewProjectileDirect(source, target.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), sigilDamage, 0f, Player.whoAmI, target.whoAmI, 255f);

                // Enable crits by setting the sigil's damage class to be whatever the player's strongest damage class is.
                sigilStrike.DamageType = Player.GetBestClass();

                // Incinerate the target with either Vulnerability Hex or True Vulnerability Hex, depending on current cursor focus.
                // This adds 8 to the buff duration, which results in a net increase of 3 frames every time damage is dealt, due to damage occurring every 5 frames.
                int buffToInflict = target.Calamity().trueVulnerabilityHex ? ModContent.BuffType<TrueVulnerabilityHex>() : ModContent.BuffType<VulnerabilityHex>(); 
                if (!target.buffImmune[buffToInflict]) 
                { 
                    if (!target.HasBuff(buffToInflict)) 
                        target.AddBuff(buffToInflict, 52); 

                    int index = target.FindBuffIndex(buffToInflict); 

                    if (index != -1) 
                        target.buffTime[index] = Math.Max(target.buffTime[index] + 8, VulnerabilityHex.CalamityDuration); 
                }

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

        private void DemonAltarTalking()
        {

            for (var h = -15; h < 15; h++)
            {
                for (var j = -15; j < 15; j++)
                {
                    var tilePos = Player.Center.ToTileCoordinates() + new Point(h, j);
                    if (Main.tile[tilePos.X, tilePos.Y].TileType == TileID.DemonAltar)
                    {
                        string type = WorldGen.crimson ? "Mods.CalamityMod.EvilSmasher.CrimsonAltar" : "Mods.CalamityMod.EvilSmasher.DemonAltar";
                        if (DialogueDisplaySystem.ContainsDialogueKey(type))
                            DialogueDisplaySystem.RemoveDialogue(DialogueDisplaySystem.GetSlot(type));
                        DialogueDisplaySystem.StartDialogueOnClient(type, new Vector2(tilePos.X * 16 + 16, tilePos.Y * 16), DemonAltarDialogueCounter, 180, false, new AltarText());
                        DemonAltarDialogueCounter++;
                        DemonAltarDialogueCooldown = 300;
                        if (DemonAltarDialogueCounter >= 4)
                        {
                            DemonAltarDialogueCooldown = 3600;
                            DemonAltarDialogueCounter = 0;
                        }
                        return;
                    }
                }
            }
        }
        private void MiscEffects()
        {
            if (Player.inventory.Any(x => x.type == ItemID.Pwnhammer) && !Player.inventory.Any(x => x.type == ModContent.ItemType<EvilSmasher>()) && Player.adjTile[TileID.DemonAltar] && DemonAltarDialogueCooldown <= 0 && Player.miscCounter % 30 == 0)
            {
                DemonAltarTalking();
            }

            //Mana Burn update
            if (ManaBurnFireDrawer != null)
            {
                ManaBurnFireDrawer.LocalTimer = 0;
                ManaBurnFireDrawer.RelativePower = MathHelper.Lerp(0.25f, 0.5f, -Player.statMana / (float)Player.statManaMax2);
                ManaBurnFireDrawer.Update();
            }

            // Update textures
            if (!Main.dedServ && Player.whoAmI == Main.myPlayer)
            {
                Asset<Texture2D> carpetAuric = ExtraTextureRefs.FlyingCarpetAuric;
                Asset<Texture2D> carpetOriginal = ExtraTextureRefs.FlyingCarpetVanilla;
                TextureAssets.FlyingCarpet = (auricSet ? carpetAuric : carpetOriginal);

                for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    if (Player.buffType[l] != BuffID.Lucky)
                        continue;

                    if (Player.buffTime[l] > CalamityUtils.SecondsToFrames(600f))
                        TextureAssets.Buff[BuffID.Lucky] = ExtraTextureRefs.LuckIconGreater;
                    else if (Player.buffTime[l] > CalamityUtils.SecondsToFrames(300f))
                        TextureAssets.Buff[BuffID.Lucky] = ExtraTextureRefs.LuckIconVanilla;
                    else
                        TextureAssets.Buff[BuffID.Lucky] = ExtraTextureRefs.LuckIconLesser;

                    break;
                }
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
            if (Player.HeldItem.type == ModContent.ItemType<PhosphorescentGauntlet>())
                PhosphorescentGauntletPunches.GenerateDustOnOwnerHand(Player);
            if (temporaryStealthTimer > 0)
            {
                stealthUIAlpha = 1;
            }
            else if (stealthUIAlpha > 0f && (rogueStealth <= 0f || rogueStealthMax <= 0f))
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
                    {
                        Projectile.NewProjectile(source, target.Center, Vector2.Zero, bullseyeType, 0, 0f, Player.whoAmI, target.whoAmI);

                        foreach (var proj in Main.ActiveProjectiles)
                        {
                            if (proj.owner != Player.whoAmI || proj.type != ModContent.ProjectileType<DaawnlightSpiritOriginMinion>())
                                continue;

                            DaawnlightSpiritOriginMinion dsoPet = proj.ModProjectile<DaawnlightSpiritOriginMinion>();
                            dsoPet.Projectile.spriteDirection = MathF.Sign(dsoPet.Projectile.Center.X - target.Center.X);
                            if (dsoPet.CurrentAnimation != DaawnlightSpiritOriginMinion.AnimationState.Pointing)
                                dsoPet.CurrentAnimation = DaawnlightSpiritOriginMinion.AnimationState.Pointing;

                            break;
                        }
                    }
                }
            }

            if (brittleStar && brittleStarBuffMode)
                Player.statDefense += BrittleStarStaff.DefenseBoostPerBuffStar * Player.ownedProjectileCounts[ModContent.ProjectileType<BrittleStarMinion>()];

            // Reduce the rate of recovery of the Lifesteal variable
            // Classic Mode: 36 HP/s to 12 HP/s
            // Expert Mode: 30 HP/s to 9 HP/s
            float lifeStealRecoveryRateReduction = Main.expertMode ? BalancingConstants.LifeStealRecoveryRateReduction_Expert : BalancingConstants.LifeStealRecoveryRateReduction_Classic;
            float lifeStealCap = Main.expertMode ? BalancingConstants.LifeStealCap_Expert : BalancingConstants.LifeStealCap_Classic;

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
                        int buffID = Player.buffType[l];
                        if (Player.buffTime[l] > 2 && CalamityBuffSets.IsDebuff[buffID])
                            Player.buffTime[l]--;
                    }
                }
            }
            if (Player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.Margarita))
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    for (int l = 0; l < Player.MaxBuffs; l++)
                    {
                        int buffID = Player.buffType[l];
                        if (Player.buffTime[l] > 2 && CalamityBuffSets.IsDebuff[buffID])
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
            if (holyInferno)
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

            if (holyInferno && holyInfernoFadeIntensity < 1f)
            {
                holyInfernoFadeIntensity = MathHelper.Clamp(holyInfernoFadeIntensity + 0.015f, 0f, 1f);
            }
            else if (!holyInferno && holyInfernoFadeIntensity > 0f)
            {
                holyInfernoFadeIntensity = MathHelper.Clamp(holyInfernoFadeIntensity - 0.01f, 0f, 1f);
            }

            // Reduce breath meter while in icy water instead of chilling
            bool canBreath = (aquaticHeart && NPC.downedBoss3) || Player.gills || Player.merman;
            if (Player.arcticDivingGear || canBreath)
            {
                Player.buffImmune[ModContent.BuffType<FrozenLungs>()] = true;
            }
            if (CalamityServerConfig.Instance.ChilledWaterRework)
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
                if (frozenLungs)
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
            // Extra DoT in the lava of the crags. Negated by Flame-licked Shell.
            else if (ZoneCalamity && !flameLickedShell)
                    Player.AddBuff(ModContent.BuffType<SearingLava>(), 2, false);

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

            if (Player.whoAmI == Main.myPlayer)
            {
                if (hydrothermalSmoke && !Main.dedServ)
                {
                    // Release effects when the player moves.
                    if (Math.Abs(Player.velocity.X) > 0.1f || Math.Abs(Player.velocity.Y) > 0.1f)
                    {
                        if (Main.rand.NextBool(6))
                        {
                            int fieryDust = Dust.NewDust(Player.Top + Vector2.UnitY, 20, 42, DustID.Flare, 0f, 0f, 100, default, 0.7f);
                            if (Main.rand.NextBool(4))
                            {
                                Main.dust[fieryDust].scale *= 0.35f;
                            }
                            Main.dust[fieryDust].velocity *= 0f;
                        }

                        if (Main.rand.NextBool(2))
                        {
                            float upwardVariation = Main.rand.NextFloat(-4.5f, -8f);
                            MediumMistParticle mist = new MediumMistParticle(Player.Top + Vector2.UnitY, -Player.velocity + new Vector2(0.5f, upwardVariation), Main.rand.NextBool(3) ? Color.LightSteelBlue : Color.Black, Color.Black, Main.rand.NextFloat(0.4f, 0.65f), 130);
                            GeneralParticleHandler.SpawnParticle(mist);
                        }
                    }
                }
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
                if (!Player.wet)
                {
                    if (aeroSet)
                        Player.maxFallSpeed = AerospecBreastplate.SetBonusFallSpeed;
                    if (Player.PortalPhysicsEnabled)
                        Player.maxFallSpeed = 20f;
                }

                if (Player.controlDown && !Player.controlJump && (ironBoots || gSabaton) && !gSabatonFalling)
                {
                    Player.maxFallSpeed *= 2; //This ties Slimy Saddle. It is made viable by also allowing the player to move horizontal, unlike Saddle, and curbing existing vertical speed very fast
                    if (Player.gravDir == 1 ? Player.velocity.Y <= 0 : Player.velocity.Y >= 0)
                        Player.velocity.Y *= 0.7f;
                }

                if (LungingDown)
                {
                    Player.maxFallSpeed = 80f;
                    Player.noFallDmg = true;
                }

                if (CalamityClientConfig.Instance.FasterFallHotkey)
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
                        if (Player.velocity.Y * Player.gravDir > Player.maxFallSpeed)
                            Player.velocity.Y = Player.maxFallSpeed * Player.gravDir;
                    }
                }
            }
            else
            {
                if (Player.mount.Type == MountID.Slime)
                    Player.velocity.X *= 0.91f;
                else if (Player.mount.Type == MountID.QueenSlime)
                    Player.velocity.X *= 0.95f;
                else if (Player.mount.Type == MountID.PogoStick)
                {
                    if (Player.velocity.X > 10 || Player.velocity.X < -10)
                        Player.velocity.X *= 0.99f; //Stops infinite high-speed movement, but allows fun pogo movement tech still
                    Player.maxFallSpeed *= 0.75f; // 1.5x fall speed instead of 2x to counterbalance
                }
            }

            // Increase Rope climb velocities, if enabled
            if (CalamityServerConfig.Instance.FasterRopeClimbSpeed)
            {
                if (Player.pulley)
                {
                    int xPos = (int)(Player.position.X + (float)(Player.width / 2)) / 16;
                    int yPos = (int)(Player.position.Y - 16f) / 16;
                    int yPos2 = (int)(Player.position.Y - 8f) / 16;
                    bool ropeAbove = true;
                    bool onRope = false;
                    if (WorldGen.IsRope(xPos, yPos2 - 1) || WorldGen.IsRope(xPos, yPos2 + 1))
                        onRope = true;

                    if (!WorldGen.IsRope(xPos, yPos))
                    {
                        ropeAbove = false;
                        if (Player.velocity.Y < 0f)
                            Player.velocity.Y = 0f;
                    }

                    if (onRope)
                    {
                        if (Player.controlUp && ropeAbove)
                        {
                            // Base multiplier is 0.7f
                            // Add an additional multiplier of the same value to make it decelerate much faster
                            if (Player.velocity.Y > 0f)
                                Player.velocity.Y *= 0.7f;

                            // Base acceleration values are 0.2f and 0.02f
                            // New acceleration values are 0.2f + 0.2f (0.4f) before hitting -3f velocity and 0.02f + 0.18f (0.2f) after hitting -6f velocity
                            if (Player.velocity.Y > -3f)
                                Player.velocity.Y -= 0.2f;
                            else
                                Player.velocity.Y -= 0.18f;

                            if (Player.velocity.Y < -8f)
                                Player.velocity.Y = -8f;
                        }
                        else if (Player.controlDown)
                        {
                            // Base multiplier is 0.7f
                            // Add an additional multiplier of the same value to make it decelerate much faster
                            if (Player.velocity.Y < 0f)
                                Player.velocity.Y *= 0.7f;

                            // Base acceleration values are 0.2f and 0.1f
                            // New acceleration values are 0.2f + 0.4f (0.6f) before hitting 3f velocity and 0.1f + 0.2f (0.3f) after hitting 3f velocity
                            if (Player.velocity.Y < 3f)
                                Player.velocity.Y += 0.4f;
                            else
                                Player.velocity.Y += 0.2f;

                            if (Player.velocity.Y > Player.maxFallSpeed)
                                Player.velocity.Y = Player.maxFallSpeed;
                        }
                        else if (Math.Abs(Player.velocity.Y) > 0f)
                        {
                            // Base multiplier is 0.7f
                            // Add an additional multiplier of the same value to make it decelerate much faster
                            Player.velocity.Y *= 0.7f;
                            if (Math.Abs(Player.velocity.Y) < 0.1f)
                                Player.velocity.Y = 0f;
                        }
                    }
                }
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
                            int damage = (int)Player.GetBestClassDamage().ApplyTo(OmegaBlueHelmet.TentacleDamage);

                            var source = Player.GetSource_FromThis(OmegaBlueHelmet.TentacleEntitySourceContext);
                            Vector2 vel = new Vector2(Main.rand.Next(-13, 14), Main.rand.Next(-13, 14)) * 0.25f;
                            Projectile.NewProjectile(source, Player.Center, vel, ModContent.ProjectileType<OmegaBlueTentacle>(), damage, 8f, Main.myPlayer, Main.rand.Next(120), i);
                        }
                    }
                }

                if (omegaBlueAbyssalMadness)
                {
                    Player.GetDamage<GenericDamageClass>() += OmegaBlueHelmet.MadnessDamageBoost;
                    Player.GetCritChance<GenericDamageClass>() += OmegaBlueHelmet.MadnessCritBoost;
                }
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
                        Player.HealPlayer(10);

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
                                        Dust holyDust = Dust.NewDustDirect(currentDustPos, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, dustColor, 1f);
                                        holyDust.position = currentDustPos;
                                        holyDust.velocity = spinningpoint.RotatedBy(MathHelper.TwoPi * i / maxHealDustIterations) * healerDustVel * (0.8f + Main.rand.NextFloat() * 0.4f) + Player.velocity;
                                        holyDust.noGravity = true;
                                        holyDust.scale = 1f;
                                        holyDust.fadeIn = Main.rand.NextFloat() * 2f;
                                        Dust dustClone = Dust.BetterCloneDust(holyDust);
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
            float minVis = 0.001f;
            if (fishStockVisual > minVis) // Fish stock effects are active as long as the UI exists, even if the player removes the item
            {
                fishStockSlidingPower = MathHelper.Lerp(fishStockSlidingPower, fishStockPower, 0.05f);
                if (Player.miscCounter % 60 == 0)
                {
                    bool isExtreme = fishStockPower > 1.5f || fishStockPower < -1.5f;
                    float smallJump = Main.rand.NextBool(4) ? 0.55f : 0;
                    float bigJump = Main.rand.NextBool(isExtreme ? 7 : 10) ? 1.8f : 0;
                    int riseOrFall = Main.rand.NextBool() ? -1 : 1;
                    float newFishStockPower = Math.Clamp(fishStockPower + (Main.rand.NextFloat(0.1f, 0.2f) + Math.Max(smallJump, bigJump)) * riseOrFall, -2, 2);
                    fishStockOldPower = // Cycle each power point up as the new point is gotten
                        (fishStockPower,
                        fishStockOldPower.Item1,
                        fishStockOldPower.Item2,
                        fishStockOldPower.Item3,
                        fishStockOldPower.Item4);
                    fishStockPower = newFishStockPower;
                }
                // Stats
                Player.GetDamage<GenericDamageClass>() += 0.15f * fishStockPower;
                Player.GetCritChance<GenericDamageClass>() += 10 * fishStockPower;
                Player.Calamity().critDamage += 0.15f * fishStockPower;
                Player.statDefense += (int)(10 * fishStockPower);
                Player.endurance += 0.1f * fishStockPower;
                Player.lifeRegen += (int)(6 * fishStockPower);
                Player.pickSpeed -= 0.25f * fishStockPower;
                Player.fishingSkill += (int)(50 * fishStockPower);
                Player.luck += 0.55f * fishStockPower;
                float CoinMult = MathF.Abs(fishStockPower) * 1.5f;
                float givenMult = (fishStockPower < 0 ? (1 / CoinMult) : CoinMult);
                Player.Calamity().coinDropMult = givenMult;
            }
            // Only put away fish stocks if the stocks are even or higher, or if they've already been started to be put away
            float goalVis = (!fishStocks && (fishStockPower >= 0 || fishStockVisual < 0.9f)) ? 0 : 1;
            fishStockVisual = MathF.Round(MathHelper.Lerp(fishStockVisual, goalVis, 0.05f), 4); // Fade in and out the U

            // The fire boots debuff boosts
            // bootLevel exists SO THAT THEY DO NOT STACK. Please help me maintain my sanity so we're not "fixing" this issue seventy times
            if (bootLevel > 0)
                HeatDebuffMultiplier += 0.25f * bootLevel;

            bool holdingRoR = Player.HeldItem.type == ModContent.ItemType<RelicOfResilience>();
            bool shieldIsSetup = (rOfResilienceEffect >= RelicOfResilience.baseTimeMax);
            bool chargedButNotHolding = (rOfResilienceEffect > RelicOfResilience.baseTimeMax && !holdingRoR);
            bool reduceEffect = (!shieldIsSetup || chargedButNotHolding);
            if (rOfResilienceEffect > 0 && reduceEffect)
                { rOfResilienceEffect -= (chargedButNotHolding ? 4 : 1); if (shieldIsSetup && rOfResilienceEffect < RelicOfResilience.baseTimeMax) rOfResilienceEffect = RelicOfResilience.baseTimeMax; if (rOfResilienceEffect < 0) rOfResilienceEffect = 0; }
            if (holdingRoR) // All players close to a player holding RoR get the benefits
            {
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<RelicGuard>()] < 1 && !Player.dead && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile relic = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<RelicGuard>(), 0, 0f, Player.whoAmI);
                }
                for (int index = 0; index < Main.player.Length; index++)
                {
                    Player fella = Main.player[index];
                    float maxDistancePlayersCanBe = 1000;
                    bool validEffectBoost = Utils.Distance(fella.Center, Player.Center) < maxDistancePlayersCanBe && fella.team == Player.team && fella.Calamity().rOfResilienceEffect < RelicOfResilience.maxPowerTime
                        && fella.Calamity().rOfResilienceCooldown == 0 && (fella.Calamity().rOfResilienceEffect > RelicOfResilience.baseTimeMax ? holdingRoR : true);
                    if (validEffectBoost)
                    {
                        fella.Calamity().rOfResilienceEffect += (reduceEffect ? 2 : 1) + (Player.Calamity().profanedSoulRelicBuff ? 1 : 0); // gets subtracted by 1 every frame, so this goes up by more to keep up
                    }
                }
            }
            // Adds a floor for defense and dr, will not ignore defense damage
            if (Player.Calamity().rOfResilienceCooldown >= RelicOfResilience.baseCooldown / 2|| Player.Calamity().rOfResilienceCooldown == 0)
            {
                float fadeInStats = Utils.GetLerpValue(0, RelicOfResilience.baseTimeMax, rOfResilienceEffect, true);
                float fadeStats = (Player.Calamity().rOfResilienceCooldown == 0 ? fadeInStats : MathF.Pow(Utils.GetLerpValue(RelicOfResilience.baseCooldown / 2, RelicOfResilience.baseCooldown, Player.Calamity().rOfResilienceCooldown, true), 3));
                float overchargeBoost = 1 + (Utils.GetLerpValue(RelicOfResilience.baseTimeMax, RelicOfResilience.maxPowerTime, rOfResilienceEffect, true) * (Player.Calamity().profanedSoulRelicBuff ? RelicOfResilience.additionalMaxPowerDefensesMult * 5 : RelicOfResilience.additionalMaxPowerDefensesMult)); // The amount of bulk this gives with artifact is way overkill but it is REALLY funny
                int def = (int)(RelicOfResilience.baseDefenseFloor * overchargeBoost);
                float dr = RelicOfResilience.baseDrFloor * overchargeBoost;
                int maxDefFloor = (int)(def * fadeStats);
                float MaxDRFloor = dr * fadeStats;
                if (Player.statDefense < maxDefFloor)
                    Player.statDefense += maxDefFloor - Player.statDefense;
                if (Player.endurance < MaxDRFloor)
                    Player.endurance += MaxDRFloor - Player.endurance;
            }
            if (rOfResilienceEffect > 0)
            {
                if (Player.Calamity().mouseRight && !Player.mouseInterface && rOfResilienceCooldown == 0 && holdingRoR)
                {
                    int cooldownTime = RelicOfResilience.baseCooldown;
                    rOfResilienceCooldown = cooldownTime;
                    Player.AddCooldown(Cooldowns.RelicOfResilienceCooldown.ID, cooldownTime);
                    SoundStyle y = new("CalamityMod/Sounds/Custom/ProfanedGuardians/GuardianRockShieldActivate");
                    SoundEngine.PlaySound(y with { Volume = 0.7f, Pitch = -0.1f }, Player.Center);
                }

                int shardBaseCap = (Player.Calamity().profanedSoulRelicBuff ? (int)(RelicOfResilience.baseMaxShardCount * 1.5f) : RelicOfResilience.baseMaxShardCount);
                float postMaxedShards = MathHelper.Lerp(shardBaseCap, shardBaseCap * RelicOfResilience.maxPowerShardMult, MathF.Pow(Utils.GetLerpValue(RelicOfResilience.baseTimeMax, RelicOfResilience.maxPowerTime, rOfResilienceEffect, true), 2));
                int maxShards = (int)(postMaxedShards * Utils.GetLerpValue(0, RelicOfResilience.baseTimeMax, rOfResilienceEffect, true));
                int numOfShards = 0;
                for (int x = 0; x < Main.maxProjectiles; x++)
                {
                    Projectile projectile = Main.projectile[x];
                    if (projectile.active && projectile.type == ModContent.ProjectileType<ArtifactOfResilienceShards>() && projectile.ai[1] == 0 && projectile.owner == Player.whoAmI)
                    {
                        numOfShards++;
                    }
                }
                float lowestNum = 0;
                int whoAmI = 0;
                if (numOfShards > maxShards) // Kill the most recent shard if there's too many
                {
                    for (int x = 0; x < Main.maxProjectiles; x++)
                    {
                        Projectile projectile = Main.projectile[x];
                        if (projectile.active && projectile.type == ModContent.ProjectileType<ArtifactOfResilienceShards>() && projectile.ai[1] == 0 && projectile.owner == Player.whoAmI && projectile.ai[2] > lowestNum)
                        {
                            whoAmI = projectile.whoAmI;
                            lowestNum = projectile.ai[2];
                        }
                    }
                    if (whoAmI != 0)
                    {
                        Projectile shard = Main.projectile[whoAmI];
                        shard.ai[1] = -1;
                    }
                }
                if (numOfShards < maxShards && Player.Calamity().rOfResilienceCooldown == 0 && (rOfResilienceEffect >= RelicOfResilience.baseTimeMax || holdingRoR))
                {
                    if (numOfShards == 0)
                    {
                        rOfResilienceOrbitOffset = Main.rand.Next(0, 100 + 1);
                        SoundStyle sound = new("CalamityMod/Sounds/Custom/ProfanedGuardians/GuardianDash");
                        SoundEngine.PlaySound(sound with { Volume = 0.5f, Pitch = -0.3f }, Player.Center);
                    }
                    int shardDamage = (int)Player.GetBestClassDamage().ApplyTo(Player.Calamity().profanedSoulRelicBuff ? RelicOfResilience.shardBaseDamage * 5 : RelicOfResilience.shardBaseDamage);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, new Vector2(1, 0), ModContent.ProjectileType<ArtifactOfResilienceShards>(), shardDamage, 0f, Player.whoAmI, 0, 0, numOfShards + 1);
                }
            }
            if (rOfResilienceCooldown > 0)
            { rOfResilienceCooldown -= (holdingRoR ? 2 : 1); if (rOfResilienceCooldown < 0) rOfResilienceCooldown = 0; }

            if (Player.Calamity().friendlyMinions > 0)
            {
                int numOfPigs = Player.ownedProjectileCounts[ModContent.ProjectileType<Pigion>()];
                if (numOfPigs < Player.Calamity().friendlyMinions)
                {
                    int pigDamage = 7;
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, new Vector2(0, -16).RotatedByRandom(MathHelper.Pi), ModContent.ProjectileType<Pigion>(), pigDamage, 0f, Player.whoAmI, 0, numOfPigs + 1);
                }
            }

            if (transformer && Player.Calamity().transformerCooldown == 0 && transformerDelay == 0) // The code for this acursed thing took much... MUCH, too long to make
            {
                float zoneSize = 150;
                int blobDamage = (int)Player.GetBestClassDamage().ApplyTo(TheTransformer.blobDamage);
                int numOfBlobs = Player.ownedProjectileCounts[ModContent.ProjectileType<TransformerBlob>()];
                if (numOfBlobs >= TheTransformer.blobCap && !Main.zenithWorld)
                    Player.Calamity().transformerStoredKills = 0;
                if (Player.Calamity().transformerStoredKills > 0 && (numOfBlobs < TheTransformer.blobCap || Main.zenithWorld))
                {
                    transformerDelay = 2;

                    int layer = (int)(Utils.GetLerpValue(0, 10, numOfBlobs + 1) + 0.9f);
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, new Vector2(0, 16).RotatedByRandom(MathHelper.Pi), ModContent.ProjectileType<TransformerBlob>(), blobDamage, 0f, Player.whoAmI, layer, numOfBlobs + 1);

                    if (Player.Calamity().transformerVisual)
                    {
                        Particle orb2 = new CustomPulse(Player.Center, Vector2.Zero, Color.LightSkyBlue, "CalamityMod/Particles/BloomRing", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0, 0.5f, 11);
                        GeneralParticleHandler.SpawnParticle(orb2);

                        SoundStyle transform = new("CalamityMod/Sounds/Item/NullImpact");
                        SoundEngine.PlaySound(transform with { Volume = 0.1f, Pitch = Main.rand.NextFloat(-0.3f, -0.4f) + numOfBlobs * 0.015f, MaxInstances = -1 }, Player.Center);
                    }

                    foreach (Projectile p in Main.ActiveProjectiles)
                    {
                        float angleMax = MathHelper.ToRadians(360 * layer);
                        if (numOfBlobs == 0)
                            angleMax = 0f;

                        if (p.type == ModContent.ProjectileType<TransformerBlob>() && p.owner == Player.whoAmI)
                        {
                            int blobNum = Player.ownedProjectileCounts[ModContent.ProjectileType<TransformerBlob>()] + 1;
                            float insanityValue = (p.ai[1] % 10);
                            if (p.ai[0] == layer)
                                p.ai[2] = (insanityValue / blobNum) * angleMax - angleMax / 2f;

                            p.netUpdate = true;
                        }
                    }
                    Player.Calamity().transformerStoredKills--;
                }
                else
                {
                    // GFB changes are that projectiles slow and have lifetime decay when in the aura, also they can add charges way faster than normal, deal more damage the more you collect, and the number of blobs is uncapped
                    for (int x = 0; x < Main.maxProjectiles; x++)
                    {
                        Projectile projectile = Main.projectile[x];
                        if (Main.zenithWorld && Vector2.Distance(Player.Center, projectile.Center) <= zoneSize && projectile.active && projectile.hostile)
                        {
                            projectile.velocity = Vector2.Lerp(projectile.velocity, Utils.DirectionTo(Player.Center, projectile.Center) * 4, Utils.GetLerpValue(300, 230, projectile.timeLeft, true));
                            projectile.timeLeft = (int)MathHelper.Lerp(projectile.timeLeft, 0, 0.25f);
                            if (projectile.damage > 1)
                                projectile.damage = (int)(projectile.damage * 0.85f);
                            projectile.friendly = true;
                        }

                        if (Vector2.Distance(Player.Center, projectile.Center) <= zoneSize && projectile.active && projectile.hostile && projectile.Calamity().TransformerTimer == 0 && (numOfBlobs < TheTransformer.blobCap || Main.zenithWorld) && transformerDelay == 0)
                        {
                            transformerDelay = 2; // 2 frame delay between projectile transformation

                            projectile.Calamity().TransformerTimer = Main.zenithWorld ? 3 : 30;

                            int layer = (int)(Utils.GetLerpValue(0, 10, numOfBlobs + 1) + 0.9f);
                            Projectile.NewProjectileDirect(Player.GetSource_FromThis(), projectile.Center, projectile.velocity.SafeNormalize(Vector2.UnitX) * 16f, ModContent.ProjectileType<TransformerBlob>(), blobDamage, 0f, Player.whoAmI, layer, numOfBlobs + 1);

                            if (Player.Calamity().transformerVisual)
                            {
                                Particle orb2 = new CustomPulse(projectile.Center, Vector2.Zero, Color.LightSkyBlue, "CalamityMod/Particles/BloomRing", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0, 0.5f, 11);
                                GeneralParticleHandler.SpawnParticle(orb2);

                                SoundStyle transform = new("CalamityMod/Sounds/Item/NullImpact");
                                SoundEngine.PlaySound(transform with { Volume = 0.1f, Pitch = Main.rand.NextFloat(-0.3f, -0.4f) + numOfBlobs * 0.015f, MaxInstances = -1 }, projectile.Center);
                            }

                            float index = 1f;
                            foreach (Projectile p in Main.ActiveProjectiles)
                            {
                                float angleMax = MathHelper.ToRadians(360 * layer);
                                if (numOfBlobs == 0)
                                    angleMax = 0f;

                                if (p.type == ModContent.ProjectileType<TransformerBlob>() && p.owner == Player.whoAmI)
                                {
                                    int blobNum = Player.ownedProjectileCounts[ModContent.ProjectileType<TransformerBlob>()] + 1;
                                    float insanityValue = (p.ai[1] % 10);
                                    if (p.ai[0] == layer)
                                        p.ai[2] = (insanityValue / blobNum) * angleMax - angleMax / 2f;

                                    p.netUpdate = true;
                                    index++;
                                }
                            }

                        }
                    }
                }
            }
            if (transformerDelay > 0)
                transformerDelay--;

            if (sSpiritAmulet)
            {
                int spawnTime = 75; // Time between energy spawns
                int energyCap = 8; // Max number of energies that can be alive at a time

                Projectile projectile = null;
                if (Player.dashDelay == -1) // When the player dashes, make all the energies exit idle phase
                {
                    int energyCount = 0;
                    for (int x = 0; x < Main.maxProjectiles; x++)
                    {
                        projectile = Main.projectile[x];
                        if (projectile.active && projectile.type == ModContent.ProjectileType<AmuletEnergy>() && projectile.ai[2] == 0 && projectile.owner == Player.whoAmI)
                        {
                            projectile.ai[2] = 5;
                            energyCount++;
                        }
                    }
                    if (energyCount > 0)
                        sSpiritAmuletTimer = -spawnTime * 3; // The spawn cooldown after launching energies is much longer
                }
                int numOfEnergy = 0;
                for (int x = 0; x < Main.maxProjectiles; x++) // Get a count of energies in idle mode
                {
                    projectile = Main.projectile[x];
                    if (projectile.active && projectile.type == ModContent.ProjectileType<AmuletEnergy>() && projectile.ai[2] == 0 && projectile.owner == Player.whoAmI)
                        numOfEnergy++;
                }
                if (sSpiritAmuletTimer >= spawnTime)
                {
                    if (numOfEnergy < energyCap)
                    {
                        int energyDamage = (int)Player.GetBestClassDamage().ApplyTo(12);
                        Projectile energy = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, (Vector2.One * 4).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<AmuletEnergy>(), energyDamage, 0f, Player.whoAmI, 0, numOfEnergy);
                        if (numOfEnergy + 1 == energyCap && Player.Calamity().sSpiritAmuletVisual)
                        {
                            SoundStyle transform = new("CalamityMod/Sounds/Item/WaterSplash1");
                            SoundEngine.PlaySound(transform with { Volume = 0.25f, Pitch = 0.9f, MaxInstances = -1 }, Player.Center);

                            for (int i = 0; i <= 14; i++)
                            {
                                Vector2 vel = (Vector2.One * 5).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.4f, 1.2f);

                                Dust dust2 = Dust.NewDustPerfect(Player.Center, ModContent.DustType<LightDust>(), vel);
                                dust2.scale = Main.rand.NextFloat(0.8f, 1.4f);
                                dust2.noGravity = false;
                                dust2.alpha = 180;
                                dust2.color = Main.rand.NextBool() ? Color.Turquoise : Color.Aquamarine;
                                dust2.noLight = true;
                                dust2.noLightEmittence = true;
                            }
                        }
                    }
                    sSpiritAmuletTimer = 0;
                }
                else if (!Player.dead)
                {
                    sSpiritAmuletTimer++;
                }
            }

            if (ironBoots)
            {
                if (Player.controlDown && !Player.controlJump)
                {
                    if (Player.gravDir == 1 ? Player.velocity.Y > 0 : Player.velocity.Y < 0)
                        fallingBootVelCheckTimer++;

                    if (Player.velocity.Y == 0 && fallingBootVelCheckTimer > 10)
                    {
                        float power = Utils.Remap(fallingBootVelCheckTimer, 10, 40, 0.1f, 1);
                        float scaledPower = (float)Math.Pow(power, 3);
                        int damage = (int)Player.GetBestClassDamage().ApplyTo(1500 * scaledPower); // Damage scales down thrice
                        Vector2 playerFeet = Player.Center + Vector2.UnitY * 26 * Player.gravDir;

                        float blastSize = 120 * power;
                        float minMultiplier = 0.1f;
                        int hitsToMinMult = 6;
                        int debuff = ModContent.BuffType<ArmorCrunch>();
                        int debuffTime = (int)(300 * power);
                        Projectile blast = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), playerFeet, Vector2.Zero, ModContent.ProjectileType<BasicBurst>(), (int)(damage), -25 * power, Player.whoAmI, blastSize, minMultiplier, hitsToMinMult);
                        blast.localAI[0] = debuff;
                        blast.localAI[1] = debuffTime;
                        blast.timeLeft = 5;

                        int particleNumber = (int)Math.Max(15 * power, 2);
                        for (int i = -particleNumber; i <= particleNumber; i++)
                        {
                            Particle sparks = new AltSparkParticle(playerFeet, (Vector2.UnitX * (5 + Math.Abs(i)) * Math.Sign(i)).RotatedByRandom(0.25f) * Main.rand.NextFloat(0.5f, 1) * power, false, Main.rand.Next(17, 30 + 1), Main.rand.NextFloat(0.2f, 0.8f), Main.rand.NextBool() ? Color.Lerp(Color.Gold, Color.Silver, 0.3f) : Color.Silver);
                            GeneralParticleHandler.SpawnParticle(sparks);
                        }

                        SoundStyle sound = new("CalamityMod/Sounds/NPCHit/ExoHit3");
                        SoundEngine.PlaySound(sound with { Volume = 0.6f * power, Pitch = Main.rand.NextFloat(0.6f, 0.8f) }, playerFeet);

                        Player.SetScreenshake(4 * scaledPower);
                        fallingBootVelCheckTimer = 0;
                    }
                }

                if (Player.gravDir == 1 ? Player.velocity.Y <= 0 : Player.velocity.Y >= 0)
                    fallingBootVelCheckTimer = 0;
            }

            if (dOfTheDeep)
            {
                if (dOfTheDeepDefenseBuffTimer > 0)
                {
                    int maxDefense = 10;
                    int givenDefense = (int)Utils.Remap(dOfTheDeepDefenseBuffTimer, 0, dOfTheDeepDefenseBuffMax * 0.5f, 0, maxDefense);
                    Player.statDefense += givenDefense;
                    dOfTheDeepDefenseBuffTimer--;
                }

                int spawnTime = 150; // Time between energy spawns
                int energyCap = 3; // Max number of energies that can be alive at a time

                Projectile projectile = null;
                if (Player.dashDelay == -1 && dOfTheDeepTimer > 0) // When the player dashes, make all the energies exit idle phase
                {
                    int energyCount = 0;
                    for (int x = 0; x < Main.maxProjectiles; x++)
                    {
                        projectile = Main.projectile[x];
                        if (projectile.active && projectile.type == ModContent.ProjectileType<DiamondOfTheDeepProjectile>() && projectile.ai[2] == 0 && projectile.owner == Player.whoAmI)
                        {
                            projectile.ai[2] = 5;
                            energyCount++;
                        }
                    }
                    if (energyCount > 0)
                        dOfTheDeepTimer = (int)(-spawnTime * 0.5f); // The spawn cooldown after launching energies is longer
                }
                int numOfEnergy = 0;
                for (int x = 0; x < Main.maxProjectiles; x++) // Get a count of energies in idle mode
                {
                    projectile = Main.projectile[x];
                    if (projectile.active && projectile.type == ModContent.ProjectileType<DiamondOfTheDeepProjectile>() && projectile.ai[2] == 0 && projectile.owner == Player.whoAmI)
                        numOfEnergy++;
                }
                if (dOfTheDeepTimer >= spawnTime)
                {
                    if (numOfEnergy < energyCap)
                    {
                        int energyDamage = (int)Player.GetBestClassDamage().ApplyTo(400);
                        int energyType = (numOfEnergy % 3);
                        Projectile energy = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, (Vector2.One * 4).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<DiamondOfTheDeepProjectile>(), energyDamage, 0f, Player.whoAmI, 0, numOfEnergy);
                        energy.localAI[2] = energyType;
                        if (numOfEnergy + 1 == energyCap && Player.Calamity().dOfTheDeepVisual)
                        {
                            SoundStyle max = new("CalamityMod/Sounds/Item/WaterSplash1");
                            SoundEngine.PlaySound(max with { Volume = 0.35f, Pitch = 0.8f, MaxInstances = -1 }, Player.Center);

                            for (int i = 0; i <= 14; i++)
                            {
                                Vector2 vel = (Vector2.One * 5).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.4f, 1.2f);

                                Dust dust2 = Dust.NewDustPerfect(Player.Center, ModContent.DustType<LightDust>(), vel);
                                dust2.scale = Main.rand.NextFloat(0.8f, 1.4f);
                                dust2.noGravity = false;
                                dust2.alpha = 180;
                                dust2.color = Main.rand.NextBool() ? Color.Turquoise : Color.Aquamarine;
                                dust2.noLight = true;
                                dust2.noLightEmittence = true;
                            }
                        }
                    }
                    dOfTheDeepTimer = 0;
                }
                else if (!Player.dead)
                {
                    dOfTheDeepTimer++;
                }
            }
            if (hookPullVisuals > 0)
            {
                if (bloomStone && Player.Calamity().bloomStoneHookVisuals) // Visual flowers while being pulled
                {
                    Vector2 spawnPos = Player.Center + Main.rand.NextVector2Circular(20f, 20f);
                    float fade = (float)Math.Pow(Utils.GetLerpValue(0, 30, hookPullVisuals, true), 2);

                    Particle trail = new CustomSpark(Player.Center, -Player.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(5f, 8f),
                        "CalamityMod/Particles/BloomCircle", false, Main.rand.Next(7, 10 + 1), 0.65f,
                        Color.Lerp(Color.HotPink, Color.Gold, Utils.GetLerpValue(60, 30, hookPullVisuals, true)) * 0.75f * fade, new Vector2(1f, 0.7f), true, shrinkSpeed: 0.6f);
                    GeneralParticleHandler.SpawnParticle(trail);

                    if (Main.rand.NextBool(3))
                    {
                        Particle floweyFromHitGameUndertale = new CustomSpark(spawnPos, -Player.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(5f, 8f),
                            "CalamityMod/Particles/MiniFlower", false, Main.rand.Next(25, 28 + 1), Main.rand.NextFloat(2.3f, 2.6f) * fade,
                            Color.Lerp(Color.HotPink, Color.Plum, Main.rand.NextFloat(0, 0.65f)), new Vector2(1f, 1f), true, extraRotation: Main.rand.NextFloat(0, MathHelper.TwoPi));
                        GeneralParticleHandler.SpawnParticle(floweyFromHitGameUndertale);
                    }
                    if (Main.rand.NextBool())
                    {
                        Dust pollenDust = Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2Circular(20f, 20f), ModContent.DustType<SquashDust>());
                        pollenDust.noLightEmittence = true;
                        pollenDust.noGravity = true;
                        pollenDust.scale = Main.rand.NextFloat(0.9f, 1.4f);
                        pollenDust.color = Color.Lerp(Color.Gold, Color.HotPink, Utils.GetLerpValue(60, 30, hookPullVisuals, true));
                        pollenDust.velocity = -Player.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(2f, 5f);
                        pollenDust.fadeIn = -0.5f * fade;
                    }
                }

                hookPullVisuals--;
                if (Player.velocity.Length() < 6)
                    hookPullVisuals = 0;
            }
            if (featherCrown)
            {
                int MaxSigils = 4; //Starts at 0, so cap is 5
                if (mageCrownTimer == 0)
                {
                    if (mageCrownCount <= MaxSigils && Main.myPlayer == Player.whoAmI)
                    {
                        mageCrownCount += 1;
                        float start = 360f / mageCrownCount;
                        Projectile.NewProjectile(Player.GetSource_Accessory(FindAccessory<FeatherCrown>()), new Vector2((int)(Player.Center.X + (Math.Sin(0 * start) * 300)), (int)(Player.Center.Y + (Math.Cos(0 * start) * 300))), Vector2.Zero, ModContent.ProjectileType<SpectralFeather>(), 0, 0, Player.whoAmI, 0, mageCrownCount);
                    }
                    mageCrownTimer = 120;
                }
                else if (!Player.dead)
                {
                    mageCrownTimer--;
                }
            }
            if (moonCrown)
            {
                int MaxSigils = 9; //Starts at 0, so cap is 10
                if (mageCrownTimer == 0)
                {
                    if (mageCrownCount <= MaxSigils && Main.myPlayer == Player.whoAmI)
                    {
                        mageCrownCount += 1;
                        float start = 360f / mageCrownCount;
                        Projectile.NewProjectile(Player.GetSource_Accessory(FindAccessory<MoonstoneCrown>()), new Vector2((int)(Player.Center.X + (Math.Sin(0 * start) * 300)), (int)(Player.Center.Y + (Math.Cos(0 * start) * 300))), Vector2.Zero, ModContent.ProjectileType<MoonSigil>(), 0, 0, Player.whoAmI, 0, mageCrownCount);
                    }
                    mageCrownTimer = 120;
                }
                else if (!Player.dead)
                {
                    mageCrownTimer--;
                }
            }

            if (unstableGraniteCore)
            {
                zapActivity += 1;
                if (zapActivity <= 300 && zapActivity % 30 == 0)
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
                        unstableSelectedTarget.Calamity().arcZapCooldown = 25;
                        int damage = (int)Player.GetBestClassDamage().ApplyTo(15);

                        Projectile.NewProjectile(Player.GetSource_FromThis(), new Vector2(Player.Center.X, Player.Center.Y - 20f), new Vector2(0f, -2f), ModContent.ProjectileType<ArcZap>(), damage, 0f, Player.whoAmI, target, 5f);
                        target = -1;
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

            if (whitewaterHeal > 0)
            {
                if (whitewaterHeal % 20 == 0)
                    Player.HealPlayer(1);
                whitewaterHeal--;

                Vector2 vel = (MathHelper.TwoPi * whitewaterHeal * Player.direction / 25).ToRotationVector2() * 3f;
                for (int i = -1; i <= 1; i += 2)
                {
                    Dust c = Dust.NewDustPerfect(Player.Center + vel * 4 * i, ModContent.DustType<LightDust>());
                    c.velocity = vel * i;
                    c.scale = Main.rand.NextFloat(0.9f, 1f);
                    c.noGravity = true;
                    c.alpha = 150;
                    c.color = Color.SkyBlue;
                    c.noLightEmittence = true;
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
                        SoundEngine.PlaySound(handler.EndSound.GetValueOrDefault(), Player.Center);
                    expiredCooldowns.Add(id);
                }
            }
            cdIterator.Dispose();

            // Remove all expired cooldowns.
            foreach (string cdID in expiredCooldowns)
                cooldowns.Remove(cdID);

            // If any cooldowns were removed, send a cooldown removal packet that lists all cooldowns to remove.
            if (expiredCooldowns.Count > 0)
                SyncCooldownRemoval(Main.dedServ, expiredCooldowns);

            // Grant the player 5 seconds of immunity to immobilizing debuffs after an immobilizing debuff wears off.
            if (Player.stoned || Player.frozen || Player.webbed)
            {
                ImmobilityDebuffImmunityTimer = ImmobilityDebuffImmunityTimerMax;
            }
            else if (ImmobilityDebuffImmunityTimer > 0)
            {
                ImmobilityDebuffImmunityTimer--;
                Player.buffImmune[BuffID.Stoned] = true;
                Player.buffImmune[BuffID.Frozen] = true;
                Player.buffImmune[BuffID.Webbed] = true;
            }

            if (arsenalCooldown > 0)
                arsenalCooldown--;
            if (killModeCooldown > 0)
                killModeCooldown--;
            if (ascendantInsigniaCooldown > 0 && ascendantInsigniaBuffTime <= 0)
                ascendantInsigniaCooldown--;
            if (transformerCooldown > 0)
                transformerCooldown--;
            if (DragonsBreathAudioCooldown > 0)
                DragonsBreathAudioCooldown--;
            if (DragonsBreathAudioCooldown2 > 0)
                DragonsBreathAudioCooldown2--;
            if (PhotoAudioCooldown > 0)
                PhotoAudioCooldown--;
            if (arpeggioCooldown > 0)
                arpeggioCooldown--;
            if (fullRageSoundCountdownTimer > 0)
                --fullRageSoundCountdownTimer;
            if (plagueTaintedSMGDroneCooldown > 0)
                plagueTaintedSMGDroneCooldown--;
            if (flareGunOverheat > 0)
                flareGunOverheat--;
            if (momentumCapacitorTime > 0)
                --momentumCapacitorTime;
            if (phantomicHeartRegen > 0 && phantomicHeartRegen < 1000)
                phantomicHeartRegen--;
            if (phantomicBulwarkCooldown > 0)
                phantomicBulwarkCooldown--;
            if (galileoCooldown > 0)
                galileoCooldown--;
            if (gladiatorTimer > 0)
                gladiatorTimer--;
            if (dragonRageCooldown > 0)
                dragonRageCooldown--;
            if (soundCooldown > 0)
                soundCooldown--;
            if (raiderCritLifespan > 0f)
                raiderCritLifespan--;
            if (raiderSoundCooldown > 0)
                raiderSoundCooldown--;
            if (astralStarRainCooldown > 0)
                astralStarRainCooldown--;
            if (VoidCooldown > 0)
                VoidCooldown--;
            if (ursaSergeantCooldown > 0)
                ursaSergeantCooldown--;
            if (generalBandCooldown > 0)
                generalBandCooldown--;
            if (AlchFlaskCooldown > 0)
                AlchFlaskCooldown--;
            if (tarraRangedCooldown > 0)
                tarraRangedCooldown--;
            if (bloodflareMageCooldown > 0)
                bloodflareMageCooldown--;
            if (silvaMageCooldown > 0)
                silvaMageCooldown--;
            if (nanotechHitCooldown > 0)
                nanotechHitCooldown--;
            if (spectralVeilImmunity > 0)
                spectralVeilImmunity--;
            if (jetPackDash > 0)
                jetPackDash--;
            if (theBeeCooldown > 0)
                theBeeCooldown--;

            if (bloomStoneTotalHeal > 0)
            {
                float healRateDiv = (Player.statLife >= Player.statLifeMax ? 2 : // 2 times as slow if at max hp already (overheal prevention)
                bloomStoneBuffedHealRateTimer > 0 ? Utils.Remap(bloomStoneBuffedHealRateTimer, 90, 0, 0.5f, 1f, true) // 2 times faster if pollen buffed, scales back down to regular speed as the buff fades
                : 1); // Regular speed
                bloomStoneHealRate = 1 / healRateDiv;

                if (!bloomStone) // If you take it off, all your healing is instantly gone
                    bloomStoneTotalHeal = 0;

                float secondsOfHealing = 16; // Number of seconds healing occurs over at base, this can be met slower or faster depending on heal rate
                if (bloomStoneHealTimer >= 60) // heal the player a portion of their potion every second
                {
                    int healAmount = (int)(Math.Max(Math.Min(bloomStoneHealPool, (int)(bloomStoneTotalHeal / secondsOfHealing)), 1));
                    Player.HealPlayer(healAmount);
                    bloomStoneHealPool -= healAmount; // Subtract the amount healed from the pool until it's empty

                    // Special specifically programmed interaction with Chalice of the Blood God: Healing over time clears bits of the bleedout buffer.
                    if (chaliceOfTheBloodGod && chaliceBleedoutBuffer > 0D)
                    {
                        float amountOfBleedToClear = ChaliceOfTheBloodGod.HealingPotionRatioForBufferClear * healAmount;
                        chaliceBleedoutBuffer -= amountOfBleedToClear;
                        // Display text indicating that healing was applied to the bleedout buffer.
                        if (!Main.dedServ)
                        {
                            string text = $"(+{amountOfBleedToClear})";
                            Rectangle location = new Rectangle((int)Player.position.X + 4, (int)Player.position.Y - 3, Player.width - 4, Player.height - 4);
                            CombatText.NewText(location, ChaliceOfTheBloodGod.BleedoutBufferDamageTextColor, Language.GetTextValue(text), dot: true);
                        }
                    }

                    if (bloomStoneHealPool == 0) // When you're out of healing, reset values
                    {
                        bloomStoneTotalHeal = 0;
                    }
                    bloomStoneHealTimer = 0;
                }
                bloomStoneHealTimer += bloomStoneHealRate;
            }
            if (bloomStoneBuffedHealRateTimer > 0)
                bloomStoneBuffedHealRateTimer--;

            if (summonProjCooldown > 0f)
                summonProjCooldown -= 1f;
            if (ataxiaDmg > 0f)
                ataxiaDmg -= 1.5f;
            if (ataxiaDmg < 0f)
                ataxiaDmg = 0f;
            if (hydroHealTimer > 0f)
                hydroHealTimer -= 0.05f;
            if (ataxiaDmg < 0f)
                ataxiaDmg = 0f;
            if (xerocDmg > 0f)
                xerocDmg -= 2f;
            if (xerocDmg < 0f)
                xerocDmg = 0f;
            if (hideOfDeusMeleeBoostTimer > 0)
                hideOfDeusMeleeBoostTimer--;
            if (hurtSoundTimer > 0)
                hurtSoundTimer--;
            if (wingProjectileCooldown > 0)
                wingProjectileCooldown--;
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
            if (burningSeaBurnOut > 0)
            {
                burningSeaBurnOut--;
                if (Main.rand.NextBool())
                {
                    Vector2 dustSpawnPos = Player.position + new Vector2(Main.rand.NextFloat(Player.width), 0f);
                    Vector2 dustVelocity = -Vector2.UnitY * Main.rand.NextFloat(3f, 6f);
                    Dust burnOutDust = Dust.NewDustPerfect(dustSpawnPos, (int)CalamityDusts.Brimstone, dustVelocity);
                    burnOutDust.noGravity = true;
                }
            }
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
            if (Player.miscCounter % HydrothermicHeadRanged.FlareCooldown == 0)
                canFireAtaxiaRangedProjectile = true;
            if (Player.miscCounter % HydrothermicHeadRogue.VolleyCooldown == 0)
                canFireAtaxiaRogueProjectile = true;
            if (Player.miscCounter % BloodflareHeadMagic.GhostBoltCooldown == 0)
                canFireBloodflareMageProjectile = true;
            if (Player.miscCounter % BloodflareHeadRanged.BloodBombCooldown == 0)
                canFireBloodflareRangedProjectile = true;
            if (Player.miscCounter % GodSlayerHeadRanged.ShrapnelRoundCooldown == 0)
                canFireGodSlayerRangedProjectile = true;

            if (auralisAuroraCounter > 300)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    GlowSquareParticle fancySquares = new(Player.Center, Vector2.Zero, false, 2, 6.25f, new Color(92, 89, 251), true, MathHelper.ToRadians(auralisAuroraCounter * 2f * i));
                    GeneralParticleHandler.SpawnParticle(fancySquares);
                }
                auralisAuroraCounter++;
            }

            if (auralisAuroraCounter > 1500)
            {
                auralisAuroraCounter = 0;
                auralisAuroraCooldown = CalamityUtils.SecondsToFrames(30f);
            }
            if (auralisAuroraCooldown > 0)
                auralisAuroraCooldown--;

            if (blazingCore)
            {
                if (blazingCoreSuccessfulParry > 0)
                    BlazingCore.HandleStars(Player);
                else if (blazingCoreParry > 0)
                    BlazingCore.HandleParryCountdown(Player);
            }
            else if (blazingCoreParry > 0)
                blazingCoreParry--;
            else if (flameLickedShellParry > 0)
            {
                if (flameLickedShell)
                    FlameLickedShell.HandleParryCountdown(Player);
                else
                    flameLickedShellParry--;
            }

            if (!flameLickedShell && flameLickedShellParry > 0)
                flameLickedShellParry--;

            // Silver Armor "Medkit" effect
            if (silverMedkitTimer > 0)
            {
                --silverMedkitTimer;
                if (silverMedkitTimer == 0)
                {
                    Player.HealPlayer(SilverArmorSetChange.SetBonusHealAmount);

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
            if (MiniSwarmerCooldown > 0)
                MiniSwarmerCooldown--;

            // God Slayer Armor dash debuff immunity
            if (LastUsedDashID == GodslayerArmorDash.ID && Player.dashDelay < 0)
            {
                for (int d = 0; d < Player.buffImmune.Length; d++)
                {
                    Player.buffImmune[d] |= CalamityBuffSets.IsDebuff[d];
                }
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
                {
                    Player.KillMe(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.NecroRevive").ToNetworkText(Player.name)), 1000, -1);
                    necroReviveCounter = -1;
                }
                else if (necroReviveCounter % 60 == 59)
                    SoundEngine.PlaySound(NecroArmorSetChange.TimerSound, Player.Center);
            }

            // Silva invincibility effects
            if (silvaCountdown > 0 && hasSilvaEffect && silvaSet)
            {
                // You become immune to all debuffs
                foreach (int debuff in Player.buffType)
                {
                    if (CalamityBuffSets.IsDebuff[debuff])
                        Player.buffImmune[debuff] = true;
                }

                // Prevent thorns effects from being abused during invincibility
                Player.thorns = 0f;

                silvaCountdown -= 1;
                if (silvaCountdown <= 0)
                {
                    SoundEngine.PlaySound(SilvaArmor.DispelSound, Player.Center);
                    Player.AddCooldown(SilvaRevive.ID, SilvaArmor.ReviveCooldown);
                }

                for (int j = 0; j < 2; j++)
                {
                    Dust green = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.ChlorophyteWeapon, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 2f);
                    green.position.X += (float)Main.rand.Next(-20, 21);
                    green.position.Y += (float)Main.rand.Next(-20, 21);
                    green.velocity *= 0.9f;
                    green.noGravity = true;
                    green.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    green.shader = GameShaders.Armor.GetSecondaryShader(Player.ArmorSetDye(), Player);
                    if (Main.rand.NextBool())
                        green.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                }
            }
            if (!Player.HasCooldown(SilvaRevive.ID) && hasSilvaEffect && silvaCountdown <= 0 && !areThereAnyDamnBosses && !areThereAnyDamnEvents)
            {
                silvaCountdown = SilvaArmor.ReviveDuration;
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
                        Player.AddCooldown(Cooldowns.TarragonCloak.ID, TarragonHeadMelee.CloakCooldown);
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

                if (tarraThrowingCrits >= TarragonHeadRogue.CritsToActivateImmunity)
                {
                    tarraThrowingCrits = 0;
                    if (Player.whoAmI == Main.myPlayer && !disableAllDodges)
                        Player.AddBuff(ModContent.BuffType<Buffs.StatBuffs.TarragonImmunity>(), TarragonHeadRogue.ImmunityDuration, false);
                }

                for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    int buffID = Player.buffType[l];
                    if (Player.buffTime[l] <= 2 && buffID == ModContent.BuffType<Buffs.StatBuffs.TarragonImmunity>())
                        if (Player.whoAmI == Main.myPlayer)
                            Player.AddCooldown(Cooldowns.TarragonImmunity.ID, TarragonHeadRogue.ImmunityCooldown);

                    if (CalamityBuffSets.IsDebuff[buffID])
                        Player.GetDamage<RogueDamageClass>() += TarragonHeadRogue.RogueDamageBoostWhileDebuffed;
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
                        Player.AddBuff(ModContent.BuffType<BloodflareBloodFrenzy>(), BloodflareHeadMelee.FrenzyDuration, false);
                }

                if (bloodflareFrenzy)
                {
                    for (int l = 0; l < Player.MaxBuffs; l++)
                    {
                        int hasBuff = Player.buffType[l];
                        if (Player.buffTime[l] <= 2 && hasBuff == ModContent.BuffType<BloodflareBloodFrenzy>() && Player.whoAmI == Main.myPlayer)
                            Player.AddCooldown(BloodflareFrenzy.ID, BloodflareHeadMelee.FrenzyCooldown);
                    }

                    Player.GetCritChance<MeleeDamageClass>() += BloodflareHeadMelee.FrenzyMeleeCritBoost;
                    Player.GetDamage<MeleeDamageClass>() += BloodflareHeadMelee.FrenzyMeleeDamageBoost;

                    for (int j = 0; j < 2; j++)
                    {
                        Dust blood = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                        blood.position.X += (float)Main.rand.Next(-20, 21);
                        blood.position.Y += (float)Main.rand.Next(-20, 21);
                        blood.velocity *= 0.9f;
                        blood.noGravity = true;
                        blood.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                        blood.shader = GameShaders.Armor.GetSecondaryShader(Player.ArmorSetDye(), Player);
                        if (Main.rand.NextBool())
                            blood.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    }
                }
            }

            if (brimflameFrenzy)
            {
                Player.GetDamage<MagicDamageClass>() += BrimflameCowl.FrenzyMagicDamageBoost;

                for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    if (Player.buffTime[l] <= 2 && Player.buffType[l] == ModContent.BuffType<BrimflameFrenzyBuff>() && Player.whoAmI == Main.myPlayer)
                        Player.AddCooldown(BrimflameFrenzy.ID, BrimflameCowl.FrenzyCooldown);
                }
            }

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
                    Player.lifeRegen += 2;
                    Player.statDefense += 4;
                    Player.moveSpeed += 0.1f;
                }
            }

            // Absorber bonus
            if (absorber)
            {
                Player.moveSpeed += TheAbsorber.MoveSpeedBoost;
                Player.jumpSpeedBoost += TheAbsorber.JumpSpeedBoost;
            }

            // Affliction bonus
            if (affliction || afflicted)
            {
                Player.endurance += Affliction.DamageReductionBoost;
                Player.statDefense += Affliction.DefenseBoost;
                Player.GetDamage<GenericDamageClass>() += Affliction.DamageBoost;
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
                Player.endurance += AquaticHeart.IceShieldDamageReductionBoost;
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
                Player.manaCost -= 0.15f;
                Player.statManaMax2 += 40;
            }

            if (encased)
            {
                Player.statDefense += PermafrostsConcoction.EncasedDefenseBoost;
                Player.endurance += PermafrostsConcoction.EncasedDamageReductionBoost;
                Player.frozen = true;
                Player.velocity.X = 0f;
                Player.velocity.Y = -0.4f; // Should negate gravity

                Dust ice = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.GemSapphire);
                ice.noGravity = true;
                ice.velocity *= 2f;

                Player.buffImmune[BuffID.Frozen] = true;
                Player.buffImmune[BuffID.Chilled] = true;
            }

            // Cosmic Discharge Cosmic Freeze buff, gives surrounding enemies the Frozen debuff
            if (cFreeze)
            {
                int buffType = BuffID.Frozen;
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
            if (Player.vortexStealthActive && Player.HeldItem.type != ItemID.PsychoKnife)
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
                int velocityMult = (int)((blunderBooster ? 35 : 25) * Utils.GetLerpValue(-4, 5, jetPackDash, true));
                Player.velocity = new Vector2(jetPackDirection, -1f) * velocityMult;

                if (blunderBooster)
                {
                    int lightningCount = 4;
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<BlunderBooster>()));
                    for (int i = 0; i < lightningCount; i++)
                    {
                        Vector2 lightningVel = Player.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.3f) * Main.rand.NextFloat(7f, 10f);
                        int damage = (int)Player.GetTotalDamage<RogueDamageClass>().ApplyTo(35);

                        int projectile = Projectile.NewProjectile(source, Player.Center, lightningVel, ModContent.ProjectileType<BlunderBoosterLightning>(), damage, 0, Player.whoAmI, Main.rand.Next(2), 0f);
                        if (projectile.WithinBounds(Main.maxProjectiles))
                            Main.projectile[projectile].DamageType = DamageClass.Generic;
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Player.Center, 1, 1, DustID.RedTorch, Player.velocity.X * -0.1f, Player.velocity.Y * -0.1f, 100, default, 3.5f);
                        dust.noGravity = true;
                        dust.velocity *= 1.2f;
                        dust.velocity.Y -= 0.15f;
                    }
                }
                else if (plaguedFuelPack)
                {
                    int numClouds = 3;
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<PlaguedFuelPack>()));
                    for (int i = 0; i < numClouds; i++)
                    {
                        Vector2 cloudVelocity = Player.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.3f) * Main.rand.NextFloat(5f, 7f);
                        int damage = (int)Player.GetTotalDamage<RogueDamageClass>().ApplyTo(30);

                        int projectile = Projectile.NewProjectile(source, Player.Center, cloudVelocity, ModContent.ProjectileType<PlaguedFuelPackCloud>(), damage, 0, Player.whoAmI, 0, 0);
                        if (projectile.WithinBounds(Main.maxProjectiles))
                            Main.projectile[projectile].DamageType = DamageClass.Generic;
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Player.Center, 1, 1, DustID.GemEmerald, Player.velocity.X * -0.1f, Player.velocity.Y * -0.1f, 100, default, 3.5f);
                        dust.noGravity = true;
                        dust.velocity *= 1.2f;
                        dust.velocity.Y -= 0.15f;
                    }
                }
            }

            // This section of code ensures set bonuses and accessories with cooldowns go on cooldown immediately if the armor or accessory is removed.
            if (!ascendantInsignia && ascendantInsigniaBuffTime > 0)
            {
                ascendantInsigniaBuffTime = 0;
                ascendantInsigniaCooldown = AscendantInsignia.AbilityCooldown;
                Player.AddCooldown(AscendEffect.ID, AscendantInsignia.AbilityCooldown);
            }

            if (!brimflameSet && brimflameFrenzy)
            {
                brimflameFrenzy = false;
                Player.ClearBuff(ModContent.BuffType<BrimflameFrenzyBuff>());
                Player.AddCooldown(BrimflameFrenzy.ID, BrimflameCowl.FrenzyCooldown);
            }
            if (!bloodflareMelee && bloodflareFrenzy)
            {
                bloodflareFrenzy = false;
                Player.ClearBuff(ModContent.BuffType<BloodflareBloodFrenzy>());
                Player.AddCooldown(BloodflareFrenzy.ID, BloodflareHeadMelee.FrenzyCooldown);
            }
            if (!tarraMelee && tarragonCloak)
            {
                tarragonCloak = false;
                Player.ClearBuff(ModContent.BuffType<Buffs.StatBuffs.TarragonCloak>());
                Player.AddCooldown(Cooldowns.TarragonCloak.ID, TarragonHeadMelee.CloakCooldown);
            }
            if (!tarraThrowing && tarragonImmunity)
            {
                tarragonImmunity = false;
                Player.ClearBuff(ModContent.BuffType<Buffs.StatBuffs.TarragonImmunity>());
                Player.AddCooldown(Cooldowns.TarragonImmunity.ID, TarragonHeadRogue.ImmunityCooldown);
            }

            bool hasOmegaBlueCooldown = cooldowns.TryGetValue(OmegaBlue.ID, out CooldownInstance omegaBlueCD);
            if (!omegaBlueSet && hasOmegaBlueCooldown && omegaBlueCD.timeLeft > OmegaBlueHelmet.MadnessCooldown)
            {
                Player.ClearBuff(ModContent.BuffType<AbyssalMadness>());
                omegaBlueCD.timeLeft = OmegaBlueHelmet.MadnessCooldown;
            }

            bool hasKillMode = cooldowns.TryGetValue(KillMode.ID, out CooldownInstance killModeCD);
            if (hasKillMode && killModeCD.timeLeft > KillMode.cooldownMax && !(Player.HeldItem.type == ModContent.ItemType<ForbiddenOathblade>() || Player.HeldItem.type == ModContent.ItemType<ExaltedOathblade>() || Player.HeldItem.type == ModContent.ItemType<DevilsDevastation>()))
            {
                killModeCD.timeLeft = KillMode.cooldownMax - 1;
                Player.Calamity().killModeCooldown = KillMode.cooldownMax - 1;
                Player.Calamity().demonSwordKillMode = false;
            }

            bool hasPlagueBlackoutCD = cooldowns.TryGetValue(PlagueBlackout.ID, out CooldownInstance plagueBlackoutCD);
            if (!plagueReaper && hasPlagueBlackoutCD && plagueBlackoutCD.timeLeft > PlagueReaperMask.BlackoutCooldown)
                plagueBlackoutCD.timeLeft = PlagueReaperMask.BlackoutCooldown;

            if (!prismaticSet && prismaticLasers > PrismaticHelmet.LaserCooldown)
            {
                prismaticLasers = PrismaticHelmet.LaserCooldown;
                Player.AddCooldown(PrismaticLaser.ID, PrismaticHelmet.LaserCooldown);
            }
            if (!angelicAlliance && divineBless)
            {
                divineBless = false;
                Player.ClearBuff(ModContent.BuffType<Buffs.StatBuffs.DivineBless>());
                Player.AddCooldown(Cooldowns.DivineBless.ID, AngelicAlliance.DivineBlessCooldown);
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
            Player.SetAbyssLightLevels();

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
                    darknessIntensity = abyssDarkness + (float)depthRatio * 3;

                    // Nebula Headcrab darkness effect
                    if (!Player.headcovered)
                    {
                        float screenObstructionAmt = MathHelper.Clamp(caveDarkness, 0f, 0.95f);
                        float targetValue = MathHelper.Clamp(screenObstructionAmt * 0.7f, 0.1f, 0.3f);
                        ScreenObstruction.screenObstruction = MathHelper.Lerp(ScreenObstruction.screenObstruction, screenObstructionAmt, targetValue);
                    }

                    // Breath lost while at zero breath
                    double breathLoss = Main.remixWorld ? (point.Y < abyssLevel1 ? 1D : 0D) : (point.Y > abyssLevel1 ? 1D : 0D);

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

                    // Ticks (frames) until breath is deducted from the breath meter
                    double tick = 10D * (1D - depthRatio);

                    // Prevent 0
                    if (tick < 1D)
                        tick = 1D;

                    // Tick (frame) multiplier, depending on gear
                    double tickMult = 1D +
                        (Player.gills ? 2D : 0D) +
                        (oceanCrest ? 2D : 0D) +
                        (Player.ignoreWater ? 3D : 0D) +
                        (Player.accDivingHelm ? 5D : 0D) +
                        (Player.arcticDivingGear ? 5D : 0D) +
                        (aquaticEmblem ? 5D : 0D) +
                        (Player.accMerman ? 8D : 0D) +
                        (victideSet ? 2D : 0D) +
                        ((aquaticHeart && NPC.downedBoss3) ? 8D : 0D) +
                        (abyssalDivingSuit ? 8D : 0D) +
                        externalBreathTickBoost;

                    // Limit the multiplier to 50
                    if (tickMult > 50D)
                        tickMult = 50D;

                    // Increase ticks (frames) until breath is deducted, depending on gear
                    tick *= tickMult;

                    // Record the final breath loss rate for the stat meter
                    abyssBreathLossRateStat = (float)tick;

                    float resistanceSlowdownFactor = 1f;
                    if (hadopelagicPressure)
                        resistanceSlowdownFactor -= abyssalDivingSuit ? 0.2f : 0.5f;

                    // Reduce breath over ticks (frames)
                    abyssBreathCD++;
                    if (abyssBreathCD >= (int)(tick * resistanceSlowdownFactor))
                    {
                        // Reset modded breath variable
                        abyssBreathCD = 0;

                        // Reduce breath
                        if (Player.breath > 0)
                        {
                            Player.breath -= (int)(crushDepth && !depthCharm ? breathLoss + 1D : breathLoss);
                        }
                    }

                    // If breath is greater than 0 and player has gills or is merfolk, balance out the effects by reducing breath
                    if (Player.breath > 0)
                    {
                        if (Player.gills || Player.merman || Player.accMerman)
                            Player.breath -= 3;
                    }

                    // Life loss at zero breath
                    int lifeLossAtZeroBreath = (int)(12D * depthRatio);

                    // Resistance to life loss at zero breath
                    int lifeLossAtZeroBreathResist = 0 +
                        (depthCharm ? 4 : 0) +
                        (abyssalDivingSuit ? 5 : 0);

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
                if (Main.zenithWorld)
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
                if (countsAsAnyWet && !Player.lavaWet && !Player.honeyWet)
                {
                    if (aquaticBoost < AquaticEmblem.TimeToReachMaxBoost)
                    {
                        aquaticBoost++;
                        if (aquaticBoost > AquaticEmblem.TimeToReachMaxBoost)
                        {
                            aquaticBoost = AquaticEmblem.TimeToReachMaxBoost;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, Player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
                else
                {
                    aquaticBoost--;
                    if (aquaticBoost <= 0f)
                        aquaticBoost = 0f;
                }
                //Because mounts are unchanged by move speed we also don'y let them have the defense.
                if (!Player.mount.Active)
                {
                    Player.statDefense += (int)Utils.Remap(aquaticBoost, 0, AquaticEmblem.TimeToReachMaxBoost, 0, AquaticEmblem.MaxDefenseBoost);
                    Player.moveSpeed -= Utils.Remap(aquaticBoost, 0, AquaticEmblem.TimeToReachMaxBoost, 0, AquaticEmblem.MaxMoveSpeedReduction);
                }
            }
            else
                aquaticBoost = 0f;

            if (Player.HeldItem.type == ModContent.ItemType<Auralis>() && Player.StandingStill(0.1f))
            {
                if (auralisStealthCounter < 300f)
                    auralisStealthCounter++;

                bool usingScope = false;
                if (!Main.gameMenu && !Main.dedServ)
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

                if (usingScope && auralisAuroraCounter < chargeDuration && auralisAuroraCooldown == 0)
                    auralisAuroraCounter++;


                if (auralisAuroraCounter > 0 && auralisAuroraCounter < chargeDuration && !usingScope)
                    auralisAuroraCounter--;
            }
            else
            {
                auralisStealthCounter = 0f;
                if (auralisAuroraCounter > 0 && auralisAuroraCounter < 300)
                    auralisAuroraCounter--;
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
                        Dust blue = Dust.NewDustDirect(source + dustVel, 0, 0, DustID.Vortex, dustVel.X, dustVel.Y, 100, default, 1.2f);
                        blue.noGravity = true;
                        blue.noLight = false;
                        blue.velocity = dustVel;
                    }
                    for (int d = 0; d < dustAmt; d++)
                    {
                        Vector2 source = Vector2.Normalize(Player.velocity) * new Vector2((float)Player.width / 2f, (float)Player.height) * 0.75f;
                        source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Player.Center;
                        Vector2 dustVel = source - Player.Center;
                        Dust green = Dust.NewDustDirect(source + dustVel, 0, 0, DustID.TerraBlade, dustVel.X, dustVel.Y, 100, default, 1.2f);
                        green.noGravity = true;
                        green.noLight = false;
                        green.velocity = dustVel;
                    }
                }
                auralisAuroraCounter = 0;
            }
        }
        #endregion

        #region Other Buff Effects
        private void OtherBuffEffects()
        {
            var dripPlayer = Player.GetModPlayer<IVDripPlayer>();
            if (gravityNormalizer)
            {
                Player.buffImmune[BuffID.VortexDebuff] = true;
                if (Player.ReducedSpaceGravity())
                {
                    Player.gravity = Player.defaultGravity;
                    if (Player.wet)
                    {
                        if (Player.honeyWet)
                            Player.gravity = 0.1f;
                        else if (Player.merman)
                            Player.gravity = 0.3f;
                        else if (Player.trident && !Player.lavaWet)
                            Player.gravity = Player.controlUp ? 0.1f : 0.25f;
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

            if (astralInjection)
            {
                if (Player.statMana < Player.statManaMax2)
                    Player.statMana += AstralInjection.ManaPerFrame;
                if (Player.statMana > Player.statManaMax2)
                    Player.statMana = Player.statManaMax2;
            }

            if (irradiated)
                Player.statDefense -= 10;

            if (rRage)
            {
                Player.GetDamage<GenericDamageClass>() += ReaverHeadTank.ReaverRageDamageBoost;
                Player.statDefense += ReaverHeadTank.ReaverRageDefenseBoost;
            }

            if (xWrath)
            {
                Player.GetDamage<ThrowingDamageClass>() += EmpyreanMask.WrathRogueDamageBoost;
                Player.GetCritChance<RogueDamageClass>() += EmpyreanMask.WrathRogueCritBoost;
            }

            if (graxDefense)
            {
                Player.statDefense += Grax.DefenseBoost;
                Player.GetDamage<GenericDamageClass>() += Grax.DamageBoost;
            }

            // Trinket of Chi bonus
            if (trinketOfChi)
            {
                if (chiBuffTimer < TrinketofChi.ChiBuffHitlessTime)
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
                    Player.statDefense += Main.eclipse ? 8 : 16;
            }

            if (AbsorberRegen)
            {
                Player.GetDamage<GenericDamageClass>() += TheAbsorber.AuraDamageBoost;
                Player.endurance += TheAbsorber.AuraDamageReductionBoost;
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
                    Player.statDefense -= GiantShell.DefenseBoost;
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
                    Player.statDefense -= GiantTortoiseShell.DefenseBoost;
                    tortShellPostHit--;
                }
                else
                    Player.endurance += GiantTortoiseShell.DamageReductionBoost;

                if (tortShellPostHit < 0)
                {
                    tortShellPostHit = 0;
                }
            }

            if (eGauntlet)
            {
                Player.GetDamage<MeleeDamageClass>() += 0.15f;
                Player.GetCritChance<MeleeDamageClass>() += 5;
            }

            // Gauntlet Melee Speed, prevents glove stacking for melee speed
            if (gloveLevel > 0)
            {
                // Determine the glove the player benefits from in priority of latest in progression
                float gloveAttackSpeed = (gloveLevel == 5 ? 0.15f : gloveLevel == 4 ? 0.14f : gloveLevel >= 2 ? 0.12f : gloveLevel == 1 ? 0.10f : 0);
                Player.GetAttackSpeed<MeleeDamageClass>() += gloveAttackSpeed; // Give the player attack speed based on the glove they have
            }

            // Bloodflare Core's heal over time
            if (bloodflareCore && bloodflareCoreRemainingHealOverTime > 0 && Player.miscCounter % BloodflareCore.HealFrameCooldown == 0)
            {
                Player.HealPlayer(1, HealTextType.Local);

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


            // Reduce how slow Chilled makes the player, because it's cancerous right now
            // The moveSpeed multiplier for Chilled in vanilla is 0.75, so we just multiply by 1.166667 here to make it 0.875, effectively cutting the reduction in half
            if (Player.chilled)
                Player.moveSpeed *= 1f + (1f / 6f);

            if (purpleHazeStealthTimer > 0)
            {
                //this is so janky looking but it's the only way I could get it to work properly
                if (!(StealthStrikeAvailable() && Player.HeldItem.DamageType == RogueDamageClass.Instance))
                    Player.GetDamage(DamageClass.Generic) += PurpleHaze.DamageBoost + ((dripPlayer.HasAlcohol(AlcoholType.PurpleHaze) && purpleHaze) ? PurpleHaze.DamageBoost : 0);
                else 
                    stealthDamage -= PurpleHaze.StealthDamageLoss + ((dripPlayer.HasAlcohol(AlcoholType.PurpleHaze) && purpleHaze) ? PurpleHaze.StealthDamageLoss : 0);
            }

            if (everclear)
                Player.GetDamage<GenericDamageClass>() += Everclear.DamageBoost;
            if (dripPlayer.HasAlcohol(AlcoholType.Everclear))
                Player.GetDamage<GenericDamageClass>() += Everclear.DamageBoost;

            if (caribbeanRum)
            {
                Player.gravity *= CaribbeanRum.GravityMultiplier;
                Player.moveSpeed += CaribbeanRum.MoveSpeedBoost;
            }
            if (dripPlayer.HasAlcohol(AlcoholType.CaribbeanRum))
            {
                Player.gravity *= CaribbeanRum.GravityMultiplier;
                Player.moveSpeed += CaribbeanRum.MoveSpeedBoost;
            }

            if (starBeamRye)
            {
                Player.manaRegenCount += StarBeamRye.ManaRegenBoost;
                Player.GetDamage<MagicDamageClass>() *= StarBeamRye.MagicDmgMult;
            }
            if (dripPlayer.HasAlcohol(AlcoholType.StarBeamRye))
            {
                Player.manaRegenCount += StarBeamRye.ManaRegenBoost;
                Player.GetDamage<MagicDamageClass>() *= StarBeamRye.MagicDmgMult;
            }

            if (dripPlayer.HasAlcohol(AlcoholType.Ale) || dripPlayer.HasAlcohol(AlcoholType.Sake))
            {
                Player.GetAttackSpeed<MeleeDamageClass>() -= 0.1f;
                // See CalamityGlobalItem and PlayerUtils for Ale's melee size increase
            }

            if (whiteWine || dripPlayer.HasAlcohol(AlcoholType.WhiteWine))
            {
                if (whiteWine)
                    Player.wingTimeMax = (int)(Player.wingTimeMax * (1f - WhiteWine.FlightTimeLoss));
                if (Player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.WhiteWine))
                    Player.wingTimeMax = (int)(Player.wingTimeMax * (1f - WhiteWine.FlightTimeLoss));

                float bonus = 0f;
                float MaxDistance = 640f;
                NPC closestTarget = Player.Center.ClosestNPCAt(MaxDistance * 7); // extra range is to account for bonus range from massive targets
                if (closestTarget != null)
                {
                    float generousHitboxWidth = Math.Max(closestTarget.Hitbox.Width / 2f, closestTarget.Hitbox.Height / 2f) + 100; // Adds some room so max bonus isnt when you're ON the hitbox
                    bonus = Utils.Remap(Utils.Distance(Player.Center, closestTarget.Center), MaxDistance + generousHitboxWidth, generousHitboxWidth, 0, 1, true);
                }
                else
                    bonus = 0;

                if (whiteWine)
                    whiteWineTimer += bonus * WhiteWine.FlightTimeRecoveryAmount;
                if (Player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.WhiteWine))
                    whiteWineTimer += bonus * WhiteWine.FlightTimeRecoveryAmount;

                while (whiteWineTimer > 1)
                {
                    if (Player.wingTime < Player.wingTimeMax)
                        Player.wingTime++;
                    whiteWineTimer--;
                }
             }

            if (redWine)
                Player.wingTimeMax = (int)(Player.wingTimeMax * (1f - RedWine.FlightTimeLoss));
            if (Player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.RedWine))
                Player.wingTimeMax = (int)(Player.wingTimeMax * (1f - RedWine.FlightTimeLoss));

            if (giantPearl)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.friendly || npc.dontTakeDamage)
                            continue;
                        float distance = (npc.Center - Player.Center).Length();
                        if (distance < GiantPearl.AuraRadius)
                            npc.AddBuff(ModContent.BuffType<PearlAura>(), 20, false);
                    }
                }
            }

            if (CalamityItemSets.FishingPoleThatNeverBreaks[Player.HeldItem.type])
                Player.accFishingLine = true;

            if (planarSpeedBoost != 0)
            {
                if (Player.HeldItem.type != ModContent.ItemType<PridefulHuntersPlanarRipper>())
                    planarSpeedBoost = 0;
            }

            // Flight time boosts
            double flightTimeMult = 1D +
                (harpyRing ? 0.2 : 0D) +
                (reaverSpeed ? ReaverHeadMobility.SetBonusFlightBoost : 0D) +
                (angelTreads ? AngelTreads.FlightTimeBoost : 0D) +
                (blueCandle ? WeightlessCandle.WingTimeBoost : 0D) +
                (soaring ? SoaringPotion.FlightBoost : 0D) +
                (prismaticGreaves ? PrismaticGreaves.FlightTimeBoost : 0D) +
                (plagueReaper ? PlagueReaperMask.SetBonusFlightTimeBoost : 0D) +
                (ascendantInsignia ? AscendantInsignia.FlightTimeBoost : Player.empressBrooch ? 0.33D : 0D) +
                externalFlightTimeMultBoost;

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

            // Reaver Tank set nuke flight time
            if (reaverDefense)
                flightTimeMult -= ReaverHeadTank.SetBonusMobilityReduction;

            // Increase wing time
            if (Player.wingTimeMax > 0)
                Player.wingTimeMax = (int)(Player.wingTimeMax * flightTimeMult);

            if (vHex)
                Player.statDefense -= 20;

            if (icarusFolly)
            {
                if (Player.wingTimeMax < 0)
                    Player.wingTimeMax = 0;

                if (Player.wingTimeMax > IcarusFolly.MaxFlightTimeCap)
                    Player.wingTimeMax = IcarusFolly.MaxFlightTimeCap;

                Player.wingTimeMax = (int)(Player.wingTimeMax * (1f - IcarusFolly.FlightTimeLossPercent));
            }

            if (DoGExtremeGravity)
            {
                if (Player.wingTimeMax < 0)
                    Player.wingTimeMax = 0;

                if (Player.wingTimeMax > Buffs.StatDebuffs.DoGExtremeGravity.MaxFlightTimeCap)
                    Player.wingTimeMax = Buffs.StatDebuffs.DoGExtremeGravity.MaxFlightTimeCap;

                Player.wingTimeMax = (int)(Player.wingTimeMax * (1f - Buffs.StatDebuffs.DoGExtremeGravity.FlightTimeLossPercent));
            }

            if (bounding)
            {
                Player.jumpSpeedBoost += BoundingPotion.JumpSpeedBoost;
                Player.jumpHeight += (int)(BoundingPotion.JumpHeightPercentBoost * 15);
            }

            if (mushy)
            {
                Player.statDefense += Mushy.DefenseBoost;
                if (fungalSymbiote)
                    Player.GetDamage<GenericDamageClass>() += 0.1f;
            }

            if (omniscience)
            {
                Player.detectCreature = true;
                Player.dangerSense = true;
                Player.findTreasure = true;
            }

            if (tarraSet)
                Player.lifeMagnet = true;

            if (whisperingDeath && !laudanum)
                Player.GetDamage<GenericDamageClass>() -= WhisperingDeath.PlayerDamageReduction;

            if (armorCrunch && !laudanum)
            {
                Player.statDefense -= ArmorCrunch.DefenseReduction;
                Player.endurance *= ArmorCrunch.MultiplicativeDamageReductionPlayer;
            }

            if (wither)
            {
                Player.statDefense -= RemsRevenge.WitherDefenseReduction;
            }

            if (eutrophication)
                Player.velocity = Vector2.Zero;

            if (vaporfied || galvanicCorrosion || windChilled)
                Player.velocity *= 0.98f;

            if (molluskHelmet)
                Player.velocity.X *= 0.996f;
            if (molluskChest)
                Player.velocity.X *= 0.996f;
            if (molluskLegs)
                Player.velocity.X *= 0.996f;

            if (warped && !Player.slowFall && !Player.mount.Active)
            {
                float velocityYMultiplier = 1.01f;
                Player.velocity.Y *= velocityYMultiplier;
            }

            if (corrEffigy)
            {
                Player.moveSpeed += CorruptionEffigy.MoveSpeedBoost;
                Player.GetCritChance<GenericDamageClass>() += CorruptionEffigy.CritBoost;
            }

            if (crimEffigy)
            {
                Player.GetDamage<GenericDamageClass>() += CrimsonEffigy.DamageBoost;
                Player.statDefense += CrimsonEffigy.DefenseBoost;
            }

            // The player's true max life value with Calamity adjustments
            actualMaxLife = Player.statLifeMax2;

            if (!Player.dead && healToFull)
            {
                healToFull = false;
                Player.statLife = actualMaxLife;
            }

            if (manaOverloader)
            {
                float manaRatio = Player.statMana / (float)Player.statManaMax2;
                Player.GetDamage<MagicDamageClass>() += MathHelper.Lerp(0.05f, 0.15f, manaRatio);
            }

            if (bloodyWormTooth)
            {
                Player.GetDamage<MeleeDamageClass>() += 0.1f;
            }

            // While making the rogue update verify if we should allow these to stack again - Shade
            if (filthyGlove)
            {
                bonusStealthDamage += nanotech ? 0.05f : 0.08f;
            }

            if (rottenDogTooth && !nanotech)
            {
                bonusStealthDamage += 0.08f;
            }

            if (sandsWindBuff)
            {
                Player.GetDamage<GenericDamageClass>() += PrimordialEarth.BuffDamageBoost;
                Player.statDefense += PrimordialEarth.BuffDefenseBoost;
                Player.manaRegenDelayBonus += 1;
                Player.manaRegenBonus += 50 + (int)(400 * (float)Math.Pow((1 - ((float)Player.statMana / (float)Player.statManaMax2)), 2));
            }
            if (aeolianEarthBuff)
            {
                Player.GetDamage<GenericDamageClass>() += PrimordialAncient.BuffDamageBoost;
                Player.endurance += PrimordialAncient.BuffDamageReductionBoost;
                Player.manaRegenDelayBonus += 1;
                Player.manaRegenBonus += 75 + (int)(600 * (float)Math.Pow((1 - ((float)Player.statMana / (float)Player.statManaMax2)), 2));
            }

            if (frostFlare)
            {
                Player.resistCold = true;
                Player.buffImmune[BuffID.Frostburn] = true;
                Player.buffImmune[BuffID.Chilled] = true;
                Player.buffImmune[BuffID.Frozen] = true;

                if (Player.statLife > (int)(Player.statLifeMax2 * FrostFlare.HealthRatioThreshold))
                    Player.GetDamage<GenericDamageClass>() += FrostFlare.DamageBoost;
                else
                    Player.statDefense += FrostFlare.DefenseBoost;
            }

            if (vexation)
            {
                Player.GetDamage<GenericDamageClass>() += 0.3f * (1 - Player.statLife / (float)Player.statLifeMax2);
            }

            if (ataxiaBlaze)
            {
                if (Player.statLife <= (int)(Player.statLifeMax2 * HydrothermicArmor.InfernoHealthThreshold))
                    Player.AddBuff(BuffID.Inferno, 2);
            }

            if (bloodflareThrowing)
            {
                if (Player.statLife > (int)(Player.statLifeMax2 * BloodflareHeadRogue.DefenseBoostHealthThreshold))
                    Player.statDefense += BloodflareHeadRogue.DefenseBoostAboveHealthThreshold;
            }

            if (bloodflareSummon)
            {
                if (Player.statLife <= (int)(Player.statLifeMax2 * BloodflareHeadSummon.DefenseBoostHealthThreshold))
                    Player.statDefense += BloodflareHeadSummon.DefenseBoostBelowHealthThreshold;

                if (bloodflareSummonTimer > 0)
                    bloodflareSummonTimer--;

                if (Player.whoAmI == Main.myPlayer && bloodflareSummonTimer <= 0)
                {
                    bloodflareSummonTimer = BloodflareHeadSummon.MineCooldown;
                    // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                    var source = Player.GetSource_FromThis(BloodflareHeadSummon.GhostMineEntitySourceContext);
                    for (int I = 0; I < 3; I++)
                    {
                        float ai1 = I * 120;

                        int damage = (int)Player.CalcIntDamage<SummonDamageClass>(BloodflareHeadSummon.MineDamage);

                        int projectile = Projectile.NewProjectile(source, Player.Center.X + (float)(Math.Sin(I * 120) * 550), Player.Center.Y + (float)(Math.Cos(I * 120) * 550), 0f, 0f,
                            ModContent.ProjectileType<GhostlyMine>(), damage, 1f, Player.whoAmI, ai1, 0f);
                        if (projectile.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[projectile].originalDamage = BloodflareHeadSummon.MineDamage;
                            Main.projectile[projectile].DamageType = DamageClass.Generic;
                        }
                    }
                }
            }

            if (silvaSummon && Player.whoAmI == Main.myPlayer)
            {
                var source = Player.GetSource_FromThis(SilvaHeadSummon.SilvaCrystalEntitySourceContext);
                if (Player.FindBuffIndex(ModContent.BuffType<SilvaCrystalBuff>()) == -1)
                {
                    Player.AddBuff(ModContent.BuffType<SilvaCrystalBuff>(), 3600, true);
                }
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SilvaCrystal>()] < 1)
                {
                    int baseDmg = auricSet ? AuricTeslaHeadSummon.CrystalDamage : SilvaHeadSummon.CrystalDamage;
                    int damage = (int)Player.GetTotalDamage<SummonDamageClass>().ApplyTo(baseDmg);

                    var p = Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, 0f, -1f, ModContent.ProjectileType<SilvaCrystal>(), damage, 0f, Main.myPlayer, -20f, 0f);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = baseDmg;
                }
            }

            if (ascendantInsignia && ascendantInsigniaBuffTime > 0)
            {
                infiniteFlight = true;
                if (ascendantInsigniaBuffTime == 1)
                    Player.AddCooldown(AscendEffect.ID, AscendantInsignia.AbilityCooldown);
                ascendantInsigniaBuffTime--;
            }

            if (abyssalDivingSuit && !Player.IsUnderwater())
            {
                Player.moveSpeed -= 0.6f;
            }

            if (godSlayerThrowing)
            {
                if (Player.statLife >= Player.statLifeMax2)
                {
                    Player.GetDamage<ThrowingDamageClass>() += GodSlayerHeadRogue.RogueDamageBoostAtFullHealth;
                    Player.GetCritChance<RogueDamageClass>() += GodSlayerHeadRogue.RogueCritBoostAtFullHealth;
                    rogueVelocity += GodSlayerHeadRogue.RogueVelocityBoostAtFullHealth;
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
                    int damage = (int)Player.CalcDamage<SummonDamageClass>(TarragonHeadSummon.AuraDamage);

                    // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                    var source = Player.GetSource_FromThis(TarragonHeadSummon.LifeAuraEntitySourceContext);
                    float range = 300f;

                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.friendly || npc.dontTakeDamage)
                            continue;

                        if (Vector2.Distance(Player.Center, npc.Center) <= range)
                            Projectile.NewProjectile(source, npc.Center, Vector2.Zero, ModContent.ProjectileType<TarragonAura>(), damage, 0f, Player.whoAmI, npc.whoAmI);
                    }
                }
            }

            // Inferno potion boost
            if (ataxiaBlaze && Player.inferno)
            {
                // Constantly increment the timer every frame.
                hydrothermicInfernoTimer = (hydrothermicInfernoTimer + 1) % HydrothermicArmor.InfernoHitRate;

                // Only run this code for the client which is wearing the armor.
                // Brimstone flames is applied every single frame, but direct damage is only dealt twice per second.
                if (Player.whoAmI == Main.myPlayer)
                {
                    int damage = (int)Player.GetBestClassDamage().ApplyTo(HydrothermicArmor.InfernoDamage);

                    // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                    var source = Player.GetSource_FromThis(HydrothermicArmor.InfernoPotionEntitySourceContext);
                    float range = HydrothermicArmor.InfernoRange;

                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.friendly || npc.damage <= 0 || npc.dontTakeDamage)
                            continue;

                        if (Vector2.Distance(Player.Center, npc.Center) <= range)
                        {
                            npc.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
                            if (hydrothermicInfernoTimer == 0)
                                Projectile.NewProjectile(source, npc.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), damage, 0f, Player.whoAmI, npc.whoAmI);
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

            if (pSoulArtifact)
            {
                if (Player.whoAmI == Main.myPlayer)
                {
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<ProfanedSoulArtifact>()));
                    if (Player.HasBuff(ModContent.BuffType<ProfanedSoulGuardians>()))
                        Player.buffTime[Player.FindBuffIndex(ModContent.BuffType<ProfanedSoulGuardians>())] = 3600;
                    else
                        Player.AddBuff(ModContent.BuffType<ProfanedSoulGuardians>(), 3600, true);

                    pSoulGuardians = true;

                    // 08DEC2023: Ozzatron: PSA/PSC "babs" spawned with Old Fashioned active will retain their bonus damage indefinitely. Oops. Don't care.
                    int guardianAmt = 1;
                    float babCheck = profanedCrystal ? 1f : 0f;
                    int babDamage = profanedCrystal ? 346 : 52;

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

            if (profanedCrystal)
            {
                ProfanedSoulCrystal.DetermineTransformationEligibility(Player);
                var calPlayer = Player.Calamity();
                bool vanity = calPlayer.pscState == (int)ProfanedSoulCrystal.ProfanedSoulCrystalState.Vanity;
                if (!vanity)
                {

                    bool empowered = calPlayer.pscState == (int)ProfanedSoulCrystal.ProfanedSoulCrystalState.Empowered;
                    bool night = empowered || calPlayer.pscState == (int)ProfanedSoulCrystal.ProfanedSoulCrystalState.Enraged;
                    bool day = empowered || calPlayer.pscState == (int)ProfanedSoulCrystal.ProfanedSoulCrystalState.Buffs;

                    Player.lavaImmune = true;
                    Player.fireWalk = true;
                    Player.buffImmune[ModContent.BuffType<HolyFlames>()] = true;
                    Player.buffImmune[BuffID.OnFire] = true;
                    Player.buffImmune[BuffID.Burning] = true;
                    Player.buffImmune[BuffID.Daybreak] = true;

                    if (Player.wingTimeMax > 0)
                        Player.wingTimeMax = (int)(Player.wingTimeMax * 1.1D);
                    Player.GetDamage<SummonDamageClass>() += 0.15f;
                    if (day)
                    {
                        Player.GetKnockback<SummonDamageClass>() += 0.15f;
                        Player.moveSpeed += 0.1f;
                        Player.ignoreWater = true;
                        Player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 1f; //this only ever affects psc whip and should not be problematic
                    }
                    else if (night)
                    {
                        Player.endurance += 0.05f;
                        Player.statDefense += 15;
                        Player.lifeRegen += 5;
                    }

                    if (!calPlayer.ZoneAbyss) //No abyss memes.
                        Lighting.AddLight(Player.Center, night ? 1.2f : day ? 1f : 0.2f, night ? 0.21f : day ? 0.2f : 0.01f, 0);
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
                    int damage = (int)Player.CalcDamage<RogueDamageClass>(30);

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
                    bool check = true;
                    // Summon the aura (this check is here to prevent out of bounds errors).
                    if (check)
                    {
                        // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                        var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<TeslasAmulet>()));
                        int damage = (int)Player.GetBestClassDamage().ApplyTo(12);
                        if (Player.ownedProjectileCounts[ModContent.ProjectileType<TeslaAura>()] < 1)
                            Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<TeslaAura>(), damage, 0f, Player.whoAmI);
                    }

                    // Reduce duration of Static Discharge
                    for (int l = 0; l < Player.MaxBuffs; l++)
                    {
                        if (Player.buffType[l] == ModContent.BuffType<StaticDischarge>())
                        {
                            if (Player.buffTime[l] > 2)
                            {
                                Player.buffTime[l]--;
                                break;
                            }
                        }
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

            if (prismaticLasers > PrismaticHelmet.LaserCooldown && Player.whoAmI == Main.myPlayer)
            {
                int dmg = (int)Player.GetTotalDamage<MagicDamageClass>().ApplyTo(PrismaticHelmet.LaserDamage);

                // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                var source = Player.GetSource_FromThis(PrismaticHelmet.LaserEntitySourceContext);
                int laserAmt = Main.rand.Next(2);
                for (int index = 0; index < laserAmt; index++)
                {
                    Vector2 newPos = new Vector2(Player.ClampedMouseWorld().X + Main.rand.NextFloat(-240f, 240f), Player.MountedCenter.Y - 960f);
                    Vector2 newVel = (Player.ClampedMouseWorld() + Main.rand.NextVector2CircularEdge(24f, 24f) - newPos).SafeNormalize(Vector2.Zero) * 18f;
                    Projectile laser = Projectile.NewProjectileDirect(source, newPos, newVel, ModContent.ProjectileType<DeathhailBeam>(), dmg, 4f, Player.whoAmI);
                    laser.localNPCHitCooldown = 5;
                    laser.DamageType = DamageClass.Generic;
                }
                SoundEngine.PlaySound(SoundID.Item12, Player.Center);
            }
            if (prismaticLasers == PrismaticHelmet.LaserCooldown)
            {
                // At the exact moment the lasers stop, set the cooldown to appear
                Player.AddCooldown(PrismaticLaser.ID, PrismaticHelmet.LaserCooldown);
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
                    Dust dusty = Dust.NewDustDirect(source + dustVel, 0, 0, DustID.RainbowMk2, dustVel.X * 1f, dustVel.Y * 1f, 100, color, 1f);
                    dusty.noGravity = true;
                    dusty.noLight = true;
                    dusty.velocity = dustVel;
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
                    Player.AddCooldown(Cooldowns.DivineBless.ID, AngelicAlliance.DivineBlessCooldown);
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
                    int buffID = Player.buffType[l];
                    if (CalamityBuffSets.BuffedByAmalgam[buffID])
                    {
                        if (amalgam)
                        {
                            // Every other frame, increase the buff timer by one frame. Thus, the buff lasts twice as long.
                            // CIT 1NOV2024: Amalgam does not add to the buff time if it's at 2 or lower,
                            // to prevent buff duration showing when using infinite buff features from other mods.
                            if (Player.miscCounter % 2 == 0 && Player.buffTime[l] > 2)
                                Player.buffTime[l] += 1;

                            // Buffs will not go away when you die, to prevent wasting potions.
                            if (!Main.persistentBuff[buffID])
                                Main.persistentBuff[buffID] = true;
                        }
                        else
                        {
                            // Reset buff persistence if Amalgam is removed.
                            if (Main.persistentBuff[buffID] && !CalamityBuffSets.IsPersistentBuff[buffID])
                                Main.persistentBuff[buffID] = false;
                        }
                    }
                }
            }

            // Laudanum boosts
            if (laudanum)
            {
                // Laudanum removes immunity to debuffs that it counters, so that you can get inflicted with them
                int[] buffsAffected = [ModContent.BuffType<ArmorCrunch>(), ModContent.BuffType<WhisperingDeath>(), BuffID.VortexDebuff, BuffID.Ichor, BuffID.Bleeding,
                    BuffID.Chilled, BuffID.BrokenArmor, BuffID.Weak, BuffID.Slow, BuffID.Confused, BuffID.Cursed, BuffID.Silenced, BuffID.Blackout, BuffID.Darkness];

                for (int i = 0; i < buffsAffected.Length; i++)
                    Player.buffImmune[buffsAffected[i]] = false;

                if (Main.myPlayer == Player.whoAmI)
                {
                    for (int l = 0; l < Player.MaxBuffs; l++)
                    {
                        int hasBuff = Player.buffType[l];
                        if (buffsAffected.Contains(hasBuff))
                        {
                            // Every other frame, increase the buff timer by one frame. Thus, the buff lasts twice as long.
                            if (Player.miscCounter % 2 == 0)
                                Player.buffTime[l] += 1;
                        }

                        // Calamity buffs cannot be handled inside the switch case
                        // Their negative effects are canceled out earlier
                        if (hasBuff == ModContent.BuffType<ArmorCrunch>())
                            Player.statDefense += ArmorCrunch.DefenseReduction; // +15 net defense
                        else if (hasBuff == ModContent.BuffType<WhisperingDeath>())
                        {
                            Player.lifeRegenCount += 5;
                            Player.GetDamage<GenericDamageClass>() += WhisperingDeath.PlayerDamageReduction;
                        }

                        switch (hasBuff)
                        {
                            case BuffID.VortexDebuff:
                                Player.vortexDebuff = false;
                                Player.moveSpeed += 0.2f;
                                Player.jumpSpeedBoost += 1f;
                                // Also gives acceleration, which is done in PostUpdateRunSpeeds
                                break;
                            case BuffID.Ichor:
                                Player.statDefense += 20; // +10 net defense
                                break;
                            case BuffID.Bleeding:
                                Player.bleed = false;
                                Player.lifeRegen += 4;
                                Player.lifeRegenTime += 4;
                                break;
                            case BuffID.Chilled:
                                Player.chilled = false;
                                Player.moveSpeed *= 1.25f;
                                break;
                            case BuffID.BrokenArmor:
                                Player.brokenArmor = false;
                                Player.statDefense += (int)(Player.statDefense * 0.25);
                                break;
                            case BuffID.Weak:
                                Player.GetDamage<MeleeDamageClass>() += 0.051f; // Cancel melee damage nerf, add 10% all damage
                                Player.GetDamage<GenericDamageClass>() += 0.1f;
                                Player.statDefense += 10; // +6 net defense
                                Player.moveSpeed += 0.25f; // +15% net move speed
                                break;
                            case BuffID.Slow:
                                Player.slow = false;
                                Player.moveSpeed *= 1.5f;
                                break;
                            case BuffID.Confused:
                                Player.confused = false;
                                Player.statDefense += 15;
                                break;
                            case BuffID.Cursed:
                                Player.cursed = false;
                                if (Player.mount.Type != MountID.Drill) // DCU also uses the noItems variable, make sure to not break that
                                    Player.noItems = false;
                                Player.GetDamage<GenericDamageClass>() += 0.1f;
                                break;
                            case BuffID.Silenced:
                                Player.silence = false;
                                Player.GetDamage<MagicDamageClass>() += 0.1f;
                                break;
                            case BuffID.Blackout:
                                Player.blackout = false;
                                Player.GetCritChance<GenericDamageClass>() += 15;
                                break;
                            case BuffID.Darkness:
                                Player.blind = false;
                                Player.GetCritChance<GenericDamageClass>() += 10;
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
                    Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.VilePowder, 0f, 0f);
                    dust.position.X += Main.rand.Next(-5, 6);
                    dust.position.Y += Main.rand.Next(-5, 6);
                    dust.velocity *= 0.2f;
                    dust.noGravity = true;
                    dust.noLight = true;
                }
            }

            // Gem Tech stats based on gems.
            GemTechState.ProvideGemBoosts();

            // Add any multiplicative damage bonuses here
            float multiplicativeDamage = 1;
            if (crushingEgo)
                multiplicativeDamage += 0.3f;
            if (WarbanneroftheRighteous)
                multiplicativeDamage += warbannerDamageMult;
            if (multiplicativeDamage != 1)
                Player.GetDamage<GenericDamageClass>() *= multiplicativeDamage;
        }
        #endregion

        #region Energy Shields
        private void EnergyShields()
        {
            if (Player.whoAmI != Main.myPlayer)
                return;

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

                // As Rover Drive's shield can be left in a partially recharged state, this is for safety.
                // If the player does not have the accessory equipped for even one frame, discharge all shields.
                RoverDriveShieldDurability = 0;
            }

            // Stuff to do if the Rover Drive is equipped
            else
            {
                // If the Rover Drive shield is discharged and hasn't started its recharge delay, start that.
                if (RoverDriveShieldDurability == 0 && !cooldowns.ContainsKey(WulfrumRoverDriveRecharge.ID))
                    Player.AddCooldown(WulfrumRoverDriveRecharge.ID, RoverDrive.ShieldRechargeDelay);

                // If the shield has greater than zero durability but that durability is not on the cooldown rack, add it to the cooldown rack.
                if (RoverDriveShieldDurability > 0 && !cooldowns.ContainsKey(WulfrumRoverDriveDurability.ID))
                {
                    CooldownInstance durabilityCooldown = Player.AddCooldown(WulfrumRoverDriveDurability.ID, RoverDrive.ShieldDurabilityMax);
                    durabilityCooldown.timeLeft = RoverDriveShieldDurability;
                }

                // If the shield has greater than zero durability and isn't in its recharge delay, actively replenish shield points.
                // Play a sound on the first frame this occurs.
                if (RoverDriveShieldDurability > 0 && !cooldowns.ContainsKey(WulfrumRoverDriveRecharge.ID))
                {
                    if (!playedRoverDriveShieldSound)
                        SoundEngine.PlaySound(RoverDrive.ActivationSound, Player.Center);
                    playedRoverDriveShieldSound = true;

                    // This number is not an integer, and stores exact per-frame recharge progress.
                    roverDriveShieldPartialRechargeProgress += RoverDrive.ShieldDurabilityMax / (float)RoverDrive.TotalShieldRechargeTime;

                    // Floor the value to get whole number of shield points recharged this frame.
                    int pointsActuallyRecharged = (int)MathF.Floor(roverDriveShieldPartialRechargeProgress);

                    // Give those points to the real shield durability, capping the result. Then remove them from recharge progress.
                    RoverDriveShieldDurability = Math.Min(RoverDriveShieldDurability + pointsActuallyRecharged, RoverDrive.ShieldDurabilityMax);
                    roverDriveShieldPartialRechargeProgress -= pointsActuallyRecharged;

                    // Update the cooldown rack's durability indicator.
                    if (cooldowns.TryGetValue(WulfrumRoverDriveDurability.ID, out var cdDurability))
                        cdDurability.timeLeft = RoverDriveShieldDurability;
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
            if (everclear)
            {
                if (Player.statDefense > 0)
                    Player.statDefense -= (int)(Player.statDefense * Everclear.DefenseLossPercent);
            }
            if (Player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.Everclear))
            {
                if (Player.statDefense > 0)
                    Player.statDefense -= (int)(Player.statDefense * Everclear.DefenseLossPercent);
            }

            if (DesertProwlerHat.ShroudedInSmoke(Player, out _))
                Player.statDefense -= (int)(Player.statDefense * DesertProwlerHat.SmokeDefenseMult);
        }
        #endregion

        #region Limits
        private void Limits()
        {
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
                Player.endurance -= CorruptionEffigy.DamageReductionLoss;
        }
        #endregion

        #region Text Chat Messages
        private void HandleTextChatMessages()
        {
            if (Player.whoAmI != Main.myPlayer || Main.dedServ)
                return;

            if (startMessageDisplayDelay >= 0)
            {
                if (startMessageDisplayDelay == 0)
                {
                    if (CalamityClientConfig.Instance.WikiStatusMessage)
                    {
                        CalamityUtils.BroadcastLocalizedText("Mods.CalamityMod.Misc.WikiStatus1");
                        CalamityUtils.BroadcastLocalizedText("Mods.CalamityMod.Misc.WikiStatus2");
                    }

                    if (CalamityClientConfig.Instance.VCMMStatusMessage && !ExternalMods.VCMMAvailable)
                    {
                        CalamityUtils.BroadcastLocalizedText("Mods.CalamityMod.Misc.VCMMStatus");
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

                if (Main.mouseItem.type == ModContent.ItemType<LuxorsGift>() && !RecipeUnlockHandler.HasFoundLuxorsGift)
                {
                    RecipeUnlockHandler.HasFoundLuxorsGift = true;
                    shouldSync = true;
                }

                if (Main.mouseItem.type == ModContent.ItemType<FungalSymbiote>() && !RecipeUnlockHandler.HasFoundFungalSymbiote)
                {
                    RecipeUnlockHandler.HasFoundFungalSymbiote = true;
                    shouldSync = true;
                }

                if (Main.mouseItem.type == ModContent.ItemType<TrinketofChi>() && !RecipeUnlockHandler.HasFoundTrinketOfChi)
                {
                    RecipeUnlockHandler.HasFoundTrinketOfChi = true;
                    shouldSync = true;
                }

                if (Main.mouseItem.type == ModContent.ItemType<FrozenCube>() && !RecipeUnlockHandler.HasFoundFrozenCube)
                {
                    RecipeUnlockHandler.HasFoundFrozenCube = true;
                    shouldSync = true;
                }

                if (Main.mouseItem.type == ModContent.ItemType<GladiatorsLocket>() && !RecipeUnlockHandler.HasFoundGladiatorsLocket)
                {
                    RecipeUnlockHandler.HasFoundGladiatorsLocket = true;
                    shouldSync = true;
                }
                if (Main.mouseItem.type == ModContent.ItemType<UnstableGraniteCore>() && !RecipeUnlockHandler.HasFoundUnstableGraniteCore)
                {
                    RecipeUnlockHandler.HasFoundUnstableGraniteCore = true;
                    shouldSync = true;
                }

                if (Main.mouseItem.type == ModContent.ItemType<CrimsonEffigy>() && !RecipeUnlockHandler.HasFoundCrimsonEffigy)
                {
                    RecipeUnlockHandler.HasFoundCrimsonEffigy = true;
                    shouldSync = true;
                }

                if (Main.mouseItem.type == ModContent.ItemType<CorruptionEffigy>() && !RecipeUnlockHandler.HasFoundCorruptionEffigy)
                {
                    RecipeUnlockHandler.HasFoundCorruptionEffigy = true;
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
                                SyncAndroombaSolutionPacket.Send(npc.ModNPC<AndroombaFriendly>(), soltype);
                            }
                            if (npc.ai[0] == 0f)
                            {
                                if (Main.netMode == NetmodeID.SinglePlayer)
                                {
                                    AndroombaFriendly.ChangeAI(npc.whoAmI, 1);
                                }
                                else
                                {
                                    SyncAndroombaAIPacket.Send(npc.ModNPC<AndroombaFriendly>(), phase: 1);
                                }
                            }
                            if (Main.dedServ)
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
            if (PlayerInput.Triggers.JustPressed.QuickBuff)
            {
                for (int i = 0; i < Main.InventorySlotsTotal; ++i)
                {
                    Item item = Player.inventory[i];

                    if (Player.potionDelay > 0)
                        break;
                    if (item is null || item.stack <= 0)
                        continue;

                    if (item.type == ModContent.ItemType<HadalStew>())
                        CalamityUtils.ConsumeItemViaQuickBuff(Player, item, HadalStew.BuffType, HadalStew.BuffDuration, true);
                }
            }
        }
        #endregion
    }
}
