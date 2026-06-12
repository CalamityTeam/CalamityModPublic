using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Potions.Food;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Packets;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems.Graphic;
using CalamityMod.Tiles.Ores;
using CalamityMod.Utilities;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Filters = Terraria.Graphics.Effects.Filters;

namespace CalamityMod.NPCs.Providence
{
    [AutoloadBossHead]
    public class Providence : ModNPC
    {
        private enum Phase : sbyte
        {
            PhaseChange = -1,
            HolyBlast = 0,
            HolyFire = 1,
            FlameCocoon = 2,
            MoltenBlobs = 3,
            HolyBomb = 4,
            SpearCocoon = 5,
            Crystal = 6,
            Laser = 7
        }

        private float AIState
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        public enum BossMode
        {
            Enraged = -1,
            Normal = 0,
            Rainbow = 1,
        }

        Color HighFireColor = new Color(255, 191, 73);
        Color LowFireColor = new Color(116, 45, 23);

        private bool text = false;
        private bool useDefenseFrames = false;
        private float bossLife;
        private byte biomeType = 0;
        private int flightPath = 0;
        private sbyte phaseChange = 0;
        private byte frameUsed = 0;
        private int healTimer = 0;
        internal bool challenge = Main.expertMode; // Used to determine if Profaned Soul Crystal should drop, couldn't figure out mp mems always dropping it so challenge is singleplayer only.
        public bool hasBeenGivenFullPower = false;
        public static bool shouldDrawInfernoBorder = true; // This is only here for other mods to disable it if they don't want it drawing.
        public bool Dying = false;
        public int DeathAnimationTimer;
        public static float borderRadius = 3000f;
        int spearDir = 1;
        int starDir = 1;
        public Vector2? borderPosition = null;

        // Sounds
        public static readonly SoundStyle SpawnSound = new("CalamityMod/Sounds/Custom/Providence/ProvidenceSpawn") { Volume = 1.2f };
        public static readonly SoundStyle HolyRaySound = new("CalamityMod/Sounds/Custom/Providence/ProvidenceHolyRay") { Volume = 1.25f }; // NOTE : Volume gets clamped between 0 and 1. I don't think this does anything, but it was in the original ModSound so im keeping it just in case
        public static readonly SoundStyle HurtSound = new("CalamityMod/Sounds/NPCHit/ProvidenceHurt");
        public static readonly SoundStyle DeathAnimationSound = new("CalamityMod/Sounds/Custom/Providence/ProvidenceDeathAnimation");

        public static readonly SoundStyle NearBurnSound = new("CalamityMod/Sounds/Custom/Providence/ProvidenceSizzle");
        public static readonly SoundStyle BurnStartSound = new("CalamityMod/Sounds/Custom/Providence/ProvidenceBurn");
        public static readonly SoundStyle BurnLoopSound = new SoundStyle("CalamityMod/Sounds/Custom/Providence/ProvidenceBurnLoop") with { IsLooped = true };

        // Sound slot for the burning damage over time effect
        public SlotId BurningSoundSlot;

        // Level of sound playing
        public float SoundWarningLevel = -1f;

        public static float normalDR = 0.3f;
        public static float cocoonDR = 0.9f;

        private const float TimeForStarDespawn = 120f;
        private const float TimeForShieldDespawn = 120f;

        // Every single one of Providence's sprites, both normal and enraged, and their glowmasks
        #region Textures
        public static Asset<Texture2D> TextureAlt;
        public static Asset<Texture2D> TextureAltNight;
        public static Asset<Texture2D> TextureAttack;
        public static Asset<Texture2D> TextureAttackNight;
        public static Asset<Texture2D> TextureAttackAlt;
        public static Asset<Texture2D> TextureAttackAltNight;
        public static Asset<Texture2D> TextureDefense;
        public static Asset<Texture2D> TextureDefenseNight;
        public static Asset<Texture2D> TextureDefenseAlt;
        public static Asset<Texture2D> TextureDefenseAltNight;
        public static Asset<Texture2D> TextureNight;

        public static Asset<Texture2D> Texture_Glow;
        public static Asset<Texture2D> TextureAlt_Glow;
        public static Asset<Texture2D> TextureAltNight_Glow;
        public static Asset<Texture2D> TextureAttack_Glow;
        public static Asset<Texture2D> TextureAttackNight_Glow;
        public static Asset<Texture2D> TextureAttackAlt_Glow;
        public static Asset<Texture2D> TextureAttackAltNight_Glow;
        public static Asset<Texture2D> TextureDefense_Glow;
        public static Asset<Texture2D> TextureDefenseNight_Glow;
        public static Asset<Texture2D> TextureDefenseAlt_Glow;
        public static Asset<Texture2D> TextureDefenseAltNight_Glow;
        public static Asset<Texture2D> TextureNight_Glow;
        public static Asset<Texture2D> Texture_Glow_2;
        public static Asset<Texture2D> TextureAlt_Glow_2;
        public static Asset<Texture2D> TextureAltNight_Glow_2;
        public static Asset<Texture2D> TextureAttack_Glow_2;
        public static Asset<Texture2D> TextureAttackNight_Glow_2;
        public static Asset<Texture2D> TextureAttackAlt_Glow_2;
        public static Asset<Texture2D> TextureAttackAltNight_Glow_2;
        public static Asset<Texture2D> TextureDefense_Glow_2;
        public static Asset<Texture2D> TextureDefenseNight_Glow_2;
        public static Asset<Texture2D> TextureDefenseAlt_Glow_2;
        public static Asset<Texture2D> TextureDefenseAltNight_Glow_2;
        public static Asset<Texture2D> TextureNight_Glow_2;
        #endregion

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<HolyAura>(), 0, 0f, -1);
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NeedsExpertScaling[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.2f,
                PortraitScale = 0.32f,
                PortraitPositionYOverride = 16f
            };
            value.Position.Y += 6f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            #region Texture Loading
            if (!Main.dedServ)
            {
                string ProviPath = "CalamityMod/NPCs/Providence/Providence";
                string GlowPath = "CalamityMod/NPCs/Providence/Glowmasks/Providence";

                // Normal textures
                TextureAlt = ModContent.Request<Texture2D>(ProviPath + "Alt", AssetRequestMode.AsyncLoad);
                TextureAltNight = ModContent.Request<Texture2D>(ProviPath + "AltNight", AssetRequestMode.AsyncLoad);
                TextureAttack = ModContent.Request<Texture2D>(ProviPath + "Attack", AssetRequestMode.AsyncLoad);
                TextureAttackNight = ModContent.Request<Texture2D>(ProviPath + "AttackNight", AssetRequestMode.AsyncLoad);
                TextureAttackAlt = ModContent.Request<Texture2D>(ProviPath + "AttackAlt", AssetRequestMode.AsyncLoad);
                TextureAttackAltNight = ModContent.Request<Texture2D>(ProviPath + "AttackAltNight", AssetRequestMode.AsyncLoad);
                TextureDefense = ModContent.Request<Texture2D>(ProviPath + "Defense", AssetRequestMode.AsyncLoad);
                TextureDefenseNight = ModContent.Request<Texture2D>(ProviPath + "DefenseNight", AssetRequestMode.AsyncLoad);
                TextureDefenseAlt = ModContent.Request<Texture2D>(ProviPath + "DefenseAlt", AssetRequestMode.AsyncLoad);
                TextureDefenseAltNight = ModContent.Request<Texture2D>(ProviPath + "DefenseAltNight", AssetRequestMode.AsyncLoad);
                TextureNight = ModContent.Request<Texture2D>(ProviPath + "Night", AssetRequestMode.AsyncLoad);

                // Fire glowmasks
                Texture_Glow = ModContent.Request<Texture2D>(GlowPath + "Glow", AssetRequestMode.AsyncLoad);
                TextureAlt_Glow = ModContent.Request<Texture2D>(GlowPath + "AltGlow", AssetRequestMode.AsyncLoad);
                TextureAltNight_Glow = ModContent.Request<Texture2D>(GlowPath + "AltGlowNight", AssetRequestMode.AsyncLoad);
                TextureAttack_Glow = ModContent.Request<Texture2D>(GlowPath + "AttackGlow", AssetRequestMode.AsyncLoad);
                TextureAttackNight_Glow = ModContent.Request<Texture2D>(GlowPath + "AttackGlowNight", AssetRequestMode.AsyncLoad);
                TextureAttackAlt_Glow = ModContent.Request<Texture2D>(GlowPath + "AttackAltGlow", AssetRequestMode.AsyncLoad);
                TextureAttackAltNight_Glow = ModContent.Request<Texture2D>(GlowPath + "AttackAltGlowNight", AssetRequestMode.AsyncLoad);
                TextureDefense_Glow = ModContent.Request<Texture2D>(GlowPath + "DefenseGlow", AssetRequestMode.AsyncLoad);
                TextureDefenseNight_Glow = ModContent.Request<Texture2D>(GlowPath + "DefenseGlowNight", AssetRequestMode.AsyncLoad);
                TextureDefenseAlt_Glow = ModContent.Request<Texture2D>(GlowPath + "DefenseAltGlow", AssetRequestMode.AsyncLoad);
                TextureDefenseAltNight_Glow = ModContent.Request<Texture2D>(GlowPath + "DefenseAltGlowNight", AssetRequestMode.AsyncLoad);
                TextureNight_Glow = ModContent.Request<Texture2D>(GlowPath + "GlowNight", AssetRequestMode.AsyncLoad);

                // Crystal glowmasks
                Texture_Glow_2 = ModContent.Request<Texture2D>(GlowPath + "Glow2", AssetRequestMode.AsyncLoad);
                TextureAlt_Glow_2 = ModContent.Request<Texture2D>(GlowPath + "AltGlow2", AssetRequestMode.AsyncLoad);
                TextureAttack_Glow_2 = ModContent.Request<Texture2D>(GlowPath + "AttackGlow2", AssetRequestMode.AsyncLoad);
                TextureAttackAlt_Glow_2 = ModContent.Request<Texture2D>(GlowPath + "AttackAltGlow2", AssetRequestMode.AsyncLoad);
                TextureDefense_Glow_2 = ModContent.Request<Texture2D>(GlowPath + "DefenseGlow2", AssetRequestMode.AsyncLoad);
                TextureDefenseAlt_Glow_2 = ModContent.Request<Texture2D>(GlowPath + "DefenseAltGlow2", AssetRequestMode.AsyncLoad);
            }
            #endregion
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            // Is in spawning animation
            float spawnAnimationTime = 180f;
            bool spawnAnimation = NPC.Calamity().newAI[3] < spawnAnimationTime;

            if (spawnAnimation)
            {
                boundingBox = new Rectangle(0, 0, 0, 0);
            }

            base.ModifyHoverBoundingBox(ref boundingBox);
        }

        public static int FireDamage = 36; // 144; HolyFire, HolyFire2, HolyFlare
        public static int BlobDamage = 36; // 144
        public static int FireSentryDamage = 54; // 216; HolyBomb
        public static int MoltenBlastDamage = 54; // 216
        public static int StarDamage = 54; // 216; HolyBurnOrb
        public static int SpearDamage = 42; // 168
        public static int CrystalDamage = 48; // 192
        public static int HolyBlastDamage = 60; // 240
        public static int RayDamage = 100; // 400

        public static int StarHeal = Main.expertMode ? 50 : 35; // HolyLight

        public override void Load()
        {
            GeneralDrawLayerSystem.OnBeforeAllTiles += DrawHolyInferno;
        }

        public override void Unload()
        {
            GeneralDrawLayerSystem.OnBeforeAllTiles -= DrawHolyInferno;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 36f;
            NPC.damage = 0; // No contact damage
            NPC.width = 600;
            NPC.height = 450;
            NPC.defense = 50;
            NPC.DR_NERD(normalDR);
            NPC.LifeMaxNERB(250000, 375000, 1250000); // Old HP - 440000, 500000
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(platinum: 1, gold: 50);
            NPC.boss = true;
            NPC.Opacity = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.DeathSound = null;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;

            if (Main.getGoodWorld)
                NPC.scale *= 0.25f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Providence")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            var bits = new BitsByte();
            bits[0] = text;
            bits[1] = useDefenseFrames;
            bits[2] = NPC.dontTakeDamage;
            bits[3] = NPC.chaseable;
            bits[4] = Dying;
            bits[5] = shouldDrawInfernoBorder;
            bits[6] = flightPath != 0;
            bits[7] = flightPath == 1;
            writer.Write(bits);

            writer.Write(biomeType);
            writer.Write(phaseChange);
            writer.Write(frameUsed);
            writer.Write(healTimer);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            writer.Write((Half)SoundWarningLevel);
            writer.Write(DeathAnimationTimer);
            writer.Write(borderRadius);
            writer.WriteVector2(borderPosition ?? Vector2.Zero);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            bool wasDyingBefore = Dying;

            var bits = reader.ReadBitsByte();
            text = bits[0];
            useDefenseFrames = bits[1];
            NPC.dontTakeDamage = bits[2];
            NPC.chaseable = bits[3];
            Dying = bits[4];
            shouldDrawInfernoBorder = bits[5];
            if (bits[6]) flightPath = bits[7] ? 1 : -1;
            else flightPath = 0;

            biomeType = reader.ReadByte();
            phaseChange = reader.ReadSByte();
            frameUsed = reader.ReadByte();
            healTimer = reader.ReadInt32();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            SoundWarningLevel = (float)reader.ReadHalf();
            DeathAnimationTimer = reader.ReadInt32();
            borderRadius = reader.ReadSingle();
            borderPosition = reader.ReadVector2();
            if (borderPosition == Vector2.Zero)
                borderPosition = null;

            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();

            // Be sure to inform clients of the fact that Providence is dying if only the server recieved this packet.
            if (Main.dedServ && !wasDyingBefore && Dying)
                NPC.ForceNetUpdate();
        }

        public override void AI()
        {
            //ensure projectiles all despawn when she does
            if (NPC.timeLeft == 1)
                DespawnSpecificProjectiles(true);

            // Set the border drawing to true if it isn't set to true
            // Can happen when another mod sets to false for a difficulty and that difficulty is then toggled off.
            shouldDrawInfernoBorder = true;

            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // whoAmI variable for Guardians and other things
            CalamityGlobalNPC.holyBoss = NPC.whoAmI;

            // Rotation
            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.006f, 0.1f);

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            // Target variable and boss center
            Player player = Main.player[NPC.target];

            // Enraged bool and Color shifting
            bool getFuckedAI = Main.zenithWorld;
            if (getFuckedAI)
                NPC.localAI[1] = (float)BossMode.Rainbow;
            else if (hasBeenGivenFullPower) // Enraged behavior
                NPC.localAI[1] = (float)BossMode.Enraged;
            else
                NPC.localAI[1] = (float)BossMode.Normal;

            // Fully powered up AI if it's any color except normal
            bool fullPowerAI = NPC.localAI[1] != (float)BossMode.Normal;

            // Difficulty bools
            bool death = CalamityWorld.death || fullPowerAI;
            bool revenge = CalamityWorld.revenge || fullPowerAI;
            bool expertMode = Main.expertMode || fullPowerAI;

            // Target's current biome
            bool isHoly = player.ZoneHallow;
            bool isHell = player.ZoneUnderworldHeight;

            // Fire projectiles at normal rate or not
            bool normalAttackRate = true;

            // Is in spawning animation
            float spawnAnimationTime = 180f;
            bool spawnAnimation = calamityGlobalNPC.newAI[3] < spawnAnimationTime;

            if (!spawnAnimation)
            {
                NPC.Opacity = 1f;
            }

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Play enrage animation if she gets angy
            if (!getFuckedAI && fullPowerAI && calamityGlobalNPC.newAI[3] == spawnAnimationTime)
            {
                AIState = (int)Phase.HolyBlast;
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
                calamityGlobalNPC.newAI[1] = 0f;
                calamityGlobalNPC.newAI[2] = 0f;
                calamityGlobalNPC.newAI[3] = 0f;

                // Despawn existing projectiles and summon the aura again
                DespawnSpecificProjectiles(true);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<HolyAura>(), 0, 0f, -1);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    ProvidenceDyeConditionSyncPacket.Send(this);
                NPC.ForceNetUpdate(false);
            }

            NPC.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && fullPowerAI;

            // Change dust type while enraged
            int dustType = ProvUtils.GetDustID();

            // Phase times
            float phaseTime = fullPowerAI ? (240f - 60f * (1f - lifeRatio)) : 300f;
            float crystalPhaseTime = fullPowerAI ? (float)Math.Round(60f * lifeRatio) : death ? 60f : 120f;
            int enragedCrystalTime = 210;
            int gfbCrystalTime = 1500 + enragedCrystalTime;
            float attackDelayAfterCocoon = phaseTime * 0.3f;

            // Phases
            bool ignoreGuardianAmt = lifeRatio < (death ? 0.2f : 0.15f);
            bool phase2 = lifeRatio < 0.75f && !fullPowerAI;
            bool delayAttacks = NPC.localAI[2] > 0f;

            // Spear phase
            float spearRateIncrease = 1f - lifeRatio;
            float baseSpearRate = 18f;
            float spearRate = 1f + spearRateIncrease;

            // Projectile fire rate multiplier
            double attackRateMult = 1D;

            // Where projectiles are fired from during cocoon phases
            Vector2 fireFrom = new Vector2(NPC.Center.X, NPC.Center.Y + 20f * NPC.scale);

            // Cocoon projectile initial velocity
            float cocoonProjVelocity = 3f + (death ? 2f * (1f - lifeRatio) : 0f);

            // Distance X needed from target in order to fire holy or molten blasts
            float distanceNeededToShoot = (death ? 300f : revenge ? 360f : 420f) * NPC.scale;

            // X distance from target
            float distanceX = Math.Abs(NPC.Center.X - player.Center.X);

            // Inflict Holy Inferno if target is too far away
            float burnIntensity = CalculateBurnIntensity(attackDelayAfterCocoon);

            // Color determination
            Color hiColor = ProvUtils.GetProjectileColor(0);
            Color medColor = ProvUtils.GetProjectileColor(255);
            Color loColor = ProvUtils.GetProjectileColor(255, true);

            if (!player.dead && player.active && !player.creativeGodMode && !Dying)
            {
                // The debuff applies
                if (burnIntensity >= 1f)
                {
                    if (SoundWarningLevel < 2f)
                    {
                        // Initialize sound
                        SoundEngine.PlaySound(BurnStartSound, player.Center);
                        BurningSoundSlot = SoundEngine.PlaySound(BurnLoopSound, player.Center);
                        SoundWarningLevel = 2f;
                    }
                    player.AddBuff(ModContent.BuffType<HolyInferno>(), 2);
                }
                // If the sound is still playing, make it go slowly kinda
                else if (SoundWarningLevel > 1f)
                {
                    SoundWarningLevel -= 1 / 100f;
                    if (SoundWarningLevel < 1f)
                        SoundWarningLevel = 1f;
                }
                // The player starts to get fire particles
                else if (burnIntensity > 0.45f)
                {
                    // If the player goes from 0 to 1, then play the sound. Doesn't play when descending.
                    if (SoundWarningLevel < 1f)
                        SoundEngine.PlaySound(NearBurnSound, player.Center);

                    SoundWarningLevel = 1f;
                }
                // The player has sparks if intensity is above 0, otherwise nothing happens
                else if (burnIntensity <= 0f)
                {
                    // Reset the sound
                    SoundWarningLevel = 0f;
                }
            }
            else if (SoundWarningLevel > 0f)
                SoundWarningLevel -= 1 / 50f;

            // Updating the looping sound
            if (SoundEngine.TryGetActiveSound(BurningSoundSlot, out var burningSound) && burningSound.IsPlaying)
                burningSound.Position = player.Center;

            // Adjust the volume or break the loop accordingly
            if (burningSound is not null)
            {
                if (SoundWarningLevel <= 1f)
                    burningSound?.Stop();
                else if (SoundWarningLevel <= 2f)
                    burningSound.Volume = SoundWarningLevel - 1f;
            }

            // Count the remaining Guardians, healer especially because it allows the boss to heal
            int guardianAmt = 0;
            bool attackerAlive = false;
            bool defenderAlive = false;
            bool healerAlive = false;
            if (CalamityGlobalNPC.holyBossAttacker != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBossAttacker].active)
                {
                    guardianAmt++;
                    attackerAlive = true;
                }
            }
            if (CalamityGlobalNPC.holyBossDefender != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBossDefender].active)
                {
                    guardianAmt++;
                    defenderAlive = true;
                }
            }
            if (CalamityGlobalNPC.holyBossHealer != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBossHealer].active)
                {
                    guardianAmt++;
                    healerAlive = true;
                }
            }

            // Makes it so star shit can only happen if the attacker has spawned
            if (attackerAlive && NPC.localAI[0] == 0f)
                NPC.localAI[0] = 1f;

            // Can only run after the attacker has spawned and died
            if (!attackerAlive && NPC.localAI[0] > 0f)
            {
                if (NPC.localAI[0] < TimeForStarDespawn)
                {
                    // Star Wrath use sound
                    if (NPC.localAI[0] == 1f)
                        SoundEngine.PlaySound(SoundID.Item105, NPC.Center);

                    NPC.localAI[0] += 1f;
                }
            }

            // Makes it so shield shit can only happen if the defender has spawned
            if (defenderAlive && NPC.localAI[3] == 0f)
                NPC.localAI[3] = 1f;

            // Can only run after the defender has spawned and died
            if (!defenderAlive && NPC.localAI[3] > 0f)
            {
                if (NPC.localAI[3] < TimeForShieldDespawn)
                {
                    // Star Wrath use sound
                    if (NPC.localAI[3] == 1f)
                        SoundEngine.PlaySound(SoundID.Item105, NPC.Center);

                    NPC.localAI[3] += 1f;
                }
            }

            // Change projectile fire rate depending on Guardian amount
            if (guardianAmt > 0)
            {
                normalAttackRate = ignoreGuardianAmt;
                if (!normalAttackRate)
                {
                    switch (guardianAmt)
                    {
                        case 1:
                            attackRateMult = 1.15;
                            break;
                        case 2:
                            attackRateMult = 1.3;
                            break;
                        case 3:
                            attackRateMult = 1.45;
                            break;
                        default:
                            break;
                    }
                }
            }

            // Whether the boss can be homed in on or healed off of
            NPC.chaseable = normalAttackRate;

            // Prevent lag by stopping rain
            if (CalamityServerConfig.Instance.BossesStopWeather)
                CalamityWorld.StopRain();

            // Set target biome type
            if (biomeType == 0)
            {
                if (isHell)
                    biomeType = 2;
                else if (isHoly)
                    biomeType = 1;
            }

            // Do the death animation once killed.
            if (Dying)
            {
                DoDeathAnimation();
                return;
            }
            // Trigger the death animation
            else if (NPC.life <= 1)
            {
                NPC.life = 1;
                DespawnSpecificProjectiles(true);
                Dying = true;
                NPC.dontTakeDamage = true;
                NPC.ForceNetUpdate(false);

                return;
            }

            // Defense
            if (defenderAlive)
                NPC.defense = NPC.defDefense * 2;
            else
                NPC.defense = NPC.defDefense;

            // Healing
            if (healerAlive)
            {
                float distanceFromHealer = Vector2.Distance(Main.npc[CalamityGlobalNPC.holyBossHealer].Center, NPC.Center);
                bool dontHeal = Main.npc[CalamityGlobalNPC.holyBossHealer].justHit || NPC.life == NPC.lifeMax;
                if (dontHeal)
                {
                    healTimer = 0;
                }
                else
                {
                    float healGateValue = revenge ? 60f : 90f;
                    healTimer++;
                    if (healTimer >= healGateValue)
                    {
                        SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                        int maxHealDustIterations = (int)distanceFromHealer;
                        int maxDust = 100;
                        int dustDivisor = maxHealDustIterations / maxDust;
                        if (dustDivisor < 2)
                            dustDivisor = 2;

                        Vector2 dustLineStart = Main.npc[CalamityGlobalNPC.holyBossHealer].Center;
                        Vector2 dustLineEnd = NPC.Center;
                        Vector2 currentDustPos = default;
                        Vector2 spinningpoint = new Vector2(0f, -3f).RotatedByRandom(MathHelper.Pi);
                        Vector2 dustVelocityMult = new Vector2(2.1f, 2f);
                        Color dustColor = Main.hslToRgb(Main.rgbToHsl(new Color(255, 200, Main.DiscoB)).X, 1f, 0.5f);
                        dustColor.A = 255;
                        for (int i = 0; i < maxHealDustIterations; i++)
                        {
                            if (i % dustDivisor == 0)
                            {
                                currentDustPos = Vector2.Lerp(dustLineStart, dustLineEnd, i / (float)maxHealDustIterations);
                                int dust = Dust.NewDust(currentDustPos, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, dustColor, 1f);
                                Main.dust[dust].position = currentDustPos;
                                Main.dust[dust].velocity = spinningpoint.RotatedBy(MathHelper.TwoPi * i / maxHealDustIterations) * dustVelocityMult * (0.8f + Main.rand.NextFloat() * 0.4f) + NPC.velocity;
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].scale = 1f;
                                Main.dust[dust].fadeIn = Main.rand.NextFloat() * 2f;
                                Dust dust2 = Dust.BetterCloneDust(dust);
                                Dust dust3 = dust2;
                                dust3.scale /= 2f;
                                dust3 = dust2;
                                dust3.fadeIn /= 2f;
                                dust2.color = new Color(255, 255, 255, 255);
                            }
                        }

                        healTimer = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int healAmt = NPC.lifeMax / 200;
                            if (healAmt > NPC.lifeMax - NPC.life)
                                healAmt = NPC.lifeMax - NPC.life;

                            if (healAmt > 0)
                            {
                                NPC.life += healAmt;
                                NPC.HealEffect(healAmt, true);
                                NPC.ForceNetUpdate(false);
                            }
                        }
                    }
                }
            }

            // Despawn
            bool targetDead = false;
            if (!player.active || player.dead)
            {
                if (!player.active || player.dead)
                {
                    NPC.TargetClosest(false);
                    player = Main.player[NPC.target];
                }
                if (!player.active || player.dead)
                {
                    targetDead = true;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X += 0.2f;
                    else
                        NPC.velocity.X -= 0.2f;

                    NPC.velocity.Y -= 0.2f;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            // Guardian spawn unless she's enraged specifically (can still spawn on Zenith)
            if (NPC.localAI[1] != (float)BossMode.Enraged)
            {
                if (bossLife == 0f && NPC.life > 0)
                    bossLife = NPC.lifeMax;

                if (NPC.life > 0)
                {
                    int guardianHealthThreshold = (int)(NPC.lifeMax * 0.66);
                    if ((NPC.life + guardianHealthThreshold) < bossLife)
                    {
                        bossLife = NPC.life;
                        SoundEngine.PlaySound(SoundID.Item74, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int guardianRingAmt = 3;
                            int guardianSpread = 360 / guardianRingAmt;
                            int guardianDistance = 400;
                            for (int i = 0; i < guardianRingAmt; i++)
                            {
                                int type = i == 0 ? ModContent.NPCType<ProvSpawnDefense>() : i == 1 ? ModContent.NPCType<ProvSpawnHealer>() : ModContent.NPCType<ProvSpawnOffense>();
                                int spawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (Math.Sin(i * guardianSpread) * guardianDistance)), (int)(NPC.Center.Y + (Math.Cos(i * guardianSpread) * guardianDistance)), type, NPC.whoAmI, 0, 0, 0, -1);
                                Main.npc[spawn].ai[0] = i * guardianSpread;
                            }
                        }
                    }
                }
            }

            // Set DR based on current attack phase
            NPC.Calamity().DR = (AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon || AIState == (int)Phase.Laser || spawnAnimation) ?
                cocoonDR : delayAttacks ?
                MathHelper.Lerp(normalDR, cocoonDR, NPC.localAI[2] / attackDelayAfterCocoon) : normalDR;

            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon || AIState == (int)Phase.Laser || spawnAnimation;

            // Providence fires predictive holy/molten blasts if she is ahead of her target's movement
            bool predictiveShots = false;

            // Movement
            if (getFuckedAI || (AIState != (int)Phase.FlameCocoon && AIState != (int)Phase.SpearCocoon))
            {
                // Slowly drift down when spawning
                if (spawnAnimation)
                {
                    NPC.velocity = Vector2.Zero;
                }
                else
                {
                    // Slows down while firing Holy Rays. It would've not slowed down for the Zenith seed but apparently it was too fast (shockers).
                    bool laserPhaseSlow = AIState == (int)Phase.Laser;

                    // Change X direction of movement
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (flightPath == 0)
                        {
                            if (NPC.Center.X < player.Center.X)
                            {
                                flightPath = 1;
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                flightPath = -1;
                                NPC.netUpdate = true;
                            }
                        }
                    }

                    // Distance needed from target to change direction
                    float changeDirectionThreshold = 800f;

                    // Increase distance from target when firing molten blasts or holy bombs
                    bool stayAwayFromTarget = AIState == (int)Phase.MoltenBlobs || AIState == (int)Phase.HolyBomb;
                    if (stayAwayFromTarget)
                        changeDirectionThreshold += death ? 240f : revenge ? 180f : 120f;

                    // Change X movement path if far enough away from target
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (NPC.Center.X < player.Center.X && flightPath < 0 && distanceX > changeDirectionThreshold)
                        {
                            flightPath = 0;
                            NPC.netUpdate = true;
                        }

                        if (NPC.Center.X > player.Center.X && flightPath > 0 && distanceX > changeDirectionThreshold)
                        {
                            flightPath = 0;
                            NPC.netUpdate = true;
                        }
                    }


                    // Predictive shot checks
                    if ((NPC.velocity.X > 0f && (NPC.Center.X - player.Center.X) > 0f && player.velocity.X > 0f) || (NPC.velocity.X < 0f && (NPC.Center.X - player.Center.X) < 0f && player.velocity.X < 0f))
                        predictiveShots = true;

                    // Velocity and acceleration
                    float speedIncreaseTimer = fullPowerAI ? 75f : death ? 120f : 150f;
                    float accelerationBoost = death ? 0.3f * (1f - lifeRatio) : 0.2f * (1f - lifeRatio);
                    float velocityBoost = death ? 6f * (1f - lifeRatio) : 4f * (1f - lifeRatio);
                    float acceleration = (expertMode ? 1.1f : 1.05f) + accelerationBoost;
                    float velocity = (expertMode ? 16f : 15f) + velocityBoost;
                    if (fullPowerAI)
                    {
                        acceleration = 1.5f;
                        velocity = 25f;
                    }
                    if (laserPhaseSlow)
                    {
                        acceleration *= getFuckedAI ? 0.6f : 0.2f;
                        velocity *= getFuckedAI ? 0.6f : 0.2f;
                    }

                    if (Main.getGoodWorld)
                    {
                        velocity *= 1.2f;
                        acceleration *= 1.2f;
                    }

                    if (!targetDead)
                    {
                        NPC.velocity.X += flightPath * acceleration;
                        if (NPC.velocity.X > velocity)
                            NPC.velocity.X = velocity;
                        if (NPC.velocity.X < -velocity)
                            NPC.velocity.X = -velocity;

                        float moveUpThreshold = player.position.Y - (NPC.position.Y + NPC.height);
                        if (moveUpThreshold < (laserPhaseSlow ? 150f : 200f)) // 150
                            NPC.velocity.Y -= Main.getGoodWorld ? 0.4f : 0.2f;
                        if (moveUpThreshold > (laserPhaseSlow ? 200f : 250f)) // 200
                            NPC.velocity.Y += Main.getGoodWorld ? 0.4f : 0.2f;

                        float speedCap = laserPhaseSlow ? 2f : 6f;
                        if (Main.getGoodWorld)
                            speedCap *= 1.5f;

                        if (NPC.velocity.Y > speedCap)
                            NPC.velocity.Y = speedCap;
                        if (NPC.velocity.Y < -speedCap)
                            NPC.velocity.Y = -speedCap;
                    }
                }
            }

            // Phase switch
            switch ((int)AIState)
            {
                case (int)Phase.PhaseChange when Main.netMode != NetmodeID.MultiplayerClient: // Only Server or SP should handle Phase Transition

                    phaseChange++;
                    if (phaseChange > 14)
                        phaseChange = 0;

                    int phase = 0;

                    // Holy ray in hallow, Crystal in hell
                    bool useLaser = (phase2 && biomeType == 1) || BossRushEvent.BossRushActive;
                    bool useCrystal = (phase2 && biomeType == 2) || BossRushEvent.BossRushActive;

                    // Unique pattern for Death Mode and Boss Rush
                    if (death)
                    {
                        switch (phaseChange)
                        {
                            case 0:
                                phase = (int)Phase.MoltenBlobs;
                                break;
                            case 1:
                                phase = (int)Phase.SpearCocoon;
                                break;
                            case 2:
                                phase = (int)Phase.HolyBlast;
                                break;
                            case 3:
                                phase = (useCrystal || fullPowerAI) ? (int)Phase.Crystal : (int)Phase.MoltenBlobs;
                                break;
                            case 4:
                                phase = useCrystal ? (int)Phase.MoltenBlobs : (int)Phase.FlameCocoon;
                                break;
                            case 5:
                                phase = useCrystal ? (int)Phase.FlameCocoon : (int)Phase.HolyFire;
                                break;
                            case 6:
                                phase = (useLaser || fullPowerAI) ? (int)Phase.Laser : (int)Phase.HolyBomb;
                                break;
                            case 7:
                                phase = (useLaser || fullPowerAI) ? (int)Phase.HolyBomb : (int)Phase.MoltenBlobs;
                                break;
                            case 8:
                                phase = (useLaser || fullPowerAI) ? (int)Phase.MoltenBlobs : (int)Phase.SpearCocoon;
                                break;
                            case 9:
                                phase = (int)Phase.HolyBlast;
                                break;
                            case 10:
                                phase = (useCrystal || fullPowerAI) ? (int)Phase.Crystal : (int)Phase.FlameCocoon;
                                break;
                            case 11:
                                phase = fullPowerAI ? (int)Phase.FlameCocoon : (int)Phase.MoltenBlobs;
                                break;
                            case 12:
                                phase = (useLaser || fullPowerAI) ? (int)Phase.Laser : (int)Phase.HolyBomb;
                                break;
                            case 13:
                                phase = (int)Phase.SpearCocoon;
                                break;
                            case 14:
                                phase = (useLaser || fullPowerAI) ? (int)Phase.HolyBomb : (int)Phase.HolyBlast;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (phaseChange)
                        {
                            case 0:
                                phase = (int)Phase.HolyBlast;
                                break;
                            case 1:
                                phase = useLaser ? (int)Phase.Laser : (int)Phase.HolyFire;
                                break;
                            case 2:
                                phase = (int)Phase.HolyBomb;
                                break;
                            case 3:
                                phase = (int)Phase.MoltenBlobs;
                                break;
                            case 4:
                                phase = (int)Phase.SpearCocoon;
                                break;
                            case 5:
                                phase = useCrystal ? (int)Phase.Crystal : (int)Phase.HolyBomb;
                                break;
                            case 6:
                                phase = (int)Phase.HolyFire;
                                break;
                            case 7:
                                phase = (int)Phase.HolyBlast;
                                break;
                            case 8:
                                phase = (int)Phase.MoltenBlobs;
                                break;
                            case 9:
                                phase = (int)Phase.FlameCocoon;
                                break;
                            case 10:
                                phase = (int)Phase.HolyBomb;
                                break;
                            case 11:
                                phase = useLaser ? (int)Phase.Laser : (int)Phase.HolyBlast;
                                break;
                            case 12:
                                phase = (int)Phase.HolyFire;
                                break;
                            case 13:
                                phase = (int)Phase.MoltenBlobs;
                                break;
                            case 14:
                                phase = (int)Phase.SpearCocoon;
                                break;
                            default:
                                break;
                        }
                    }

                    // If too far from target, set phase to 0
                    if (Math.Abs(NPC.Center.X - player.Center.X) > 5600f)
                        phase = (int)Phase.HolyBlast;

                    // Reset attack delay for laser
                    if (phase == (int)Phase.Laser)
                        NPC.localAI[2] = 0f;

                    // Reset arrays
                    AIState = phase;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    NPC.ForceNetUpdate(false);

                    break;

                case (int)Phase.HolyBlast:

                    if (spawnAnimation)
                    {
                        CalamityUtils.AddScreenshakeAt(NPC.Center, MathHelper.Lerp(0f, 0.25f, calamityGlobalNPC.newAI[3] / spawnAnimationTime), 2000);

                        NPC.dontTakeDamage = true;

                        if (calamityGlobalNPC.newAI[3] == spawnAnimationTime - 1)
                        {
                            NPC.dontTakeDamage = false;

                            CalamityUtils.AddScreenshakeAt(NPC.Center, 8, 2000);
                            SoundEngine.PlaySound(HolyRaySound, NPC.Center);
                            bool photos = CalamityClientConfig.Instance.Photosensitivity;

                            for (int i = 0; i < 20; i++)
                            {
                                Particle p = new FlameParticle(NPC.Center + new Vector2(Main.rand.NextFloat(150), 0).RotatedByRandom(MathHelper.TwoPi), 40, Main.rand.NextFloat(1f, 1.6f), Main.rand.NextFloat(2f, 5f), hiColor * (photos ? 0.5f : 1f), loColor * (photos ? 0.5f : 1f));
                                p.Velocity = new Vector2(Main.rand.NextFloat(3f, 19f), 0).RotatedByRandom(MathHelper.TwoPi);
                                GeneralParticleHandler.SpawnParticle(p);
                                GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(NPC.Center, new Vector2(Main.rand.NextFloat(12f, 40f), 0).RotatedByRandom(MathHelper.TwoPi), loColor * (photos ? 0.5f : 1f), 60, Main.rand.NextFloat(2.5f, 5.5f), 2f, Main.rand.NextFloat(-0.05f, 0.05f), true));
                            }
                            CalamityUtils.AddScreenshakeAt(NPC.Center, 10, 2000);

                            Color hColor = ProvUtils.GetProjectileColor(255, false) * (photos ? 0.5f : 1f);
                            Color lColor = ProvUtils.GetProjectileColor(0, true) * (photos ? 0.5f : 1f);

                            for (int i = 0; i < 20; i++)
                            {
                                Particle p = new FlameParticle(NPC.Center + new Vector2(Main.rand.NextFloat(150), 0).RotatedByRandom(MathHelper.TwoPi), 40, Main.rand.NextFloat(0.5f, 0.75f), Main.rand.NextFloat(1f, 2.5f), hColor, lColor);
                                p.Velocity = new Vector2(Main.rand.NextFloat(3f, 19f), 0).RotatedByRandom(MathHelper.TwoPi);
                                GeneralParticleHandler.SpawnParticle(p);
                                GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(NPC.Center, new Vector2(Main.rand.NextFloat(12f, 40f), 0).RotatedByRandom(MathHelper.TwoPi), loColor * (photos ? 0.5f : 1f), 60, Main.rand.NextFloat(0.75f, 1.75f), 1f, Main.rand.NextFloat(-0.05f, 0.05f), true));
                            }

                            GeneralParticleHandler.SpawnParticle(new CustomPulse(NPC.Center, Vector2.Zero, hColor, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.1f, 15));

                            for (float i = 0; i < 2.8f; i += 0.35f)
                            {
                                GeneralParticleHandler.SpawnParticle(new CustomPulse(NPC.Center, Vector2.Zero, hColor, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One * i, Main.rand.NextFloat(MathHelper.TwoPi), 0.05f, 0.35f, 35));
                                GeneralParticleHandler.SpawnParticle(new CustomPulse(NPC.Center, Vector2.Zero, hColor, "CalamityMod/Particles/ShatteredExplosion", Vector2.One * i, Main.rand.NextFloat(MathHelper.TwoPi), 0.05f, 0.475f, 25));
                            }
                        }

                        float sc = calamityGlobalNPC.newAI[3] / spawnAnimationTime;

                        if (calamityGlobalNPC.newAI[3] > 10f && calamityGlobalNPC.newAI[3] < spawnAnimationTime)
                        {
                            // Move effects slightly lower during enrage animation to appear as if they're converging on her core
                            Vector2 destination = NPC.Center + (NPC.localAI[1] != (float)BossMode.Normal ? new Vector2(0f, 40f) : Vector2.Zero);

                            if (calamityGlobalNPC.newAI[3] % (CalamityClientConfig.Instance.Photosensitivity ? 15f : 10f) == 0)
                            {
                                GeneralParticleHandler.SpawnParticle(new CustomPulse(destination, Vector2.Zero, Color.Lerp(new Color(25, 25, 25, 0), medColor, sc), "CalamityMod/Particles/SoftRoundExplosion", new Vector2(1.5f, 1f), Main.rand.NextBool() ? 0f : MathHelper.Pi, sc * 0.5f, sc * 0.1f, 20));

                                SoundStyle SpawnFlareSound = SoundID.Item74;
                                SpawnFlareSound.MaxInstances = 10;
                                SoundEngine.PlaySound(SpawnFlareSound.WithVolumeScale(calamityGlobalNPC.newAI[3] / spawnAnimationTime).WithPitchOffset(-1 + (calamityGlobalNPC.newAI[3] / spawnAnimationTime)), NPC.Center);
                            }

                            Vector2 startPos = destination + (new Vector2(Main.rand.NextFloat(80, 300) * (sc * 1.6f), 0).RotatedByRandom(Main.rand.NextFloat(MathHelper.TwoPi)) * new Vector2(1.5f, 1f));
                            GeneralParticleHandler.SpawnParticle(new SparkParticle(startPos, startPos.DirectionTo(destination) * (startPos.Distance(destination) / 10), false, 10, Main.rand.NextFloat(0.2f, 0.5f) * (sc * 2), medColor));
                        }

                        // Used to heal her back to full HP during enrage animation
                        if (fullPowerAI && calamityGlobalNPC.newAI[3] % 9f == 0f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int healAmt = NPC.lifeMax / 400;
                                if (healAmt > NPC.lifeMax - NPC.life)
                                    healAmt = NPC.lifeMax - NPC.life;

                                if (healAmt > 0)
                                {
                                    NPC.life += healAmt;
                                    NPC.HealEffect(healAmt, true);
                                    NPC.ForceNetUpdate(false);
                                }
                            }
                        }

                        calamityGlobalNPC.newAI[3] += 1f;

                        if (fullPowerAI && calamityGlobalNPC.newAI[3] >= spawnAnimationTime)
                            calamityGlobalNPC.newAI[3] += 1f;

                        return;
                    }

                    // Attack delay after cocoon phase
                    if (delayAttacks)
                    {
                        NPC.localAI[2] -= 1f;
                        return;
                    }

                    if (distanceX > distanceNeededToShoot && NPC.position.Y < player.position.Y)
                    {
                        NPC.ai[3] += 1f;

                        int shootBoost = death ? (int)Math.Round(5f * (1f - lifeRatio)) : (int)Math.Round(4f * (1f - lifeRatio));
                        int projectileShootGateValue = (expertMode ? 24 : 26) - shootBoost;

                        projectileShootGateValue = (int)(projectileShootGateValue * attackRateMult);

                        if (NPC.ai[3] >= projectileShootGateValue)
                            NPC.ai[3] = -projectileShootGateValue;

                        if (NPC.ai[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 projectileFirePosition = new Vector2(NPC.Center.X + NPC.velocity.SafeNormalize(Vector2.UnitX).X * 120f, NPC.Center.Y);
                            float velocityBoost = death ? 4f * (1f - lifeRatio) : 2.5f * (1f - lifeRatio);
                            float projSpeed = (revenge ? 12f : expertMode ? 10.5f : 9f) + velocityBoost;
                            Vector2 predictionAmount = player.velocity * 100f;
                            Vector2 projectileVelocity = (player.Center + (predictiveShots ? predictionAmount : Vector2.Zero) - projectileFirePosition).SafeNormalize(Vector2.UnitY) * projSpeed * 0.1f;
                            Vector2 explodePosition = predictiveShots ? (player.position + predictionAmount) : player.position;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileFirePosition, projectileVelocity, ModContent.ProjectileType<HolyBlast>(), HolyBlastDamage.CalculateProvidenceDamage(), 0f, Main.myPlayer, explodePosition.X, explodePosition.Y);
                        }
                    }
                    else if (NPC.ai[3] < 0f)
                        NPC.ai[3] += 1f;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= phaseTime)
                    {
                        AIState = (int)Phase.PhaseChange;
                        NPC.TargetClosest();
                    }

                    break;

                case (int)Phase.HolyFire:

                    // Attack delay after cocoon phase
                    if (delayAttacks)
                    {
                        NPC.localAI[2] -= 1f;
                        return;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.ai[3] += 1f;

                        int shootBoost = death ? (int)Math.Round(6f * (1f - lifeRatio)) : (int)Math.Round(5f * (1f - lifeRatio));
                        int projectileShootGateValue = (expertMode ? 36 : 39) - shootBoost;

                        projectileShootGateValue = (int)(projectileShootGateValue * attackRateMult);

                        if (NPC.ai[3] >= projectileShootGateValue)
                        {
                            NPC.ai[3] = 0f;

                            Vector2 shootFrom = new Vector2(NPC.Center.X, NPC.position.Y + NPC.height - 64f * NPC.scale);

                            float projectileVelocityY = NPC.velocity.Y;
                            if (projectileVelocityY < 0f)
                                projectileVelocityY = 0f;

                            projectileVelocityY += expertMode ? 4f : 3f;

                            if (fullPowerAI)
                                projectileVelocityY *= 2f;

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom.X, shootFrom.Y, NPC.velocity.X * 0.25f, projectileVelocityY, ModContent.ProjectileType<HolyFire>(), FireDamage.CalculateProvidenceDamage(), 0f, Main.myPlayer);
                        }
                    }

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= phaseTime)
                    {
                        AIState = (int)Phase.PhaseChange;
                        NPC.TargetClosest();
                    }

                    break;

                case (int)Phase.FlameCocoon:

                    borderPosition ??= NPC.Center;
                    borderPosition = Vector2.Lerp(borderPosition.Value, NPC.Center, 0.02f);
                    Vector2 fireSparklesFrom = fireFrom + new Vector2(0, -30);
                    if (!targetDead && !getFuckedAI)
                    {
                        if (NPC.velocity.Length() <= 2f)
                            NPC.velocity = Vector2.Zero;

                        if (NPC.velocity.Length() > 2f)
                        {
                            NPC.velocity *= 0.9f;
                            return;
                        }
                    }

                    float divisor = (expertMode ? 2f : 3f) + (float)Math.Floor(3f * lifeRatio) + (attackRateMult > 1D ? (float)Math.Ceiling(attackRateMult * 1.6) : 0f);
                    int totalFlameProjectiles = 36;
                    int chains = 4;
                    float interval = totalFlameProjectiles / chains * divisor;
                    double patternInterval = Math.Floor(NPC.ai[3] / interval);
                    int healingStarChance = revenge ? 8 : expertMode ? 6 : 4;

                    if (patternInterval % 2 == 0)
                    {
                        if (NPC.ai[3] % divisor == 0f)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, NPC.Center);
                            bool normalSpread = calamityGlobalNPC.newAI[1] % 2f == 0f;
                            double radians = MathHelper.TwoPi / chains;
                            double angleA = radians * 0.5;
                            double angleB = MathHelper.ToRadians(90f) - angleA;
                            float velocityX = (float)(cocoonProjVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                            Vector2 spinningPoint = normalSpread ? new Vector2(0f, -cocoonProjVelocity) : new Vector2(-velocityX, -cocoonProjVelocity);
                            for (int i = 0; i < chains; i++)
                            {
                                Vector2 vector2 = spinningPoint.RotatedBy(radians * i + MathHelper.ToRadians(NPC.ai[2]) * starDir);

                                if (Main.rand.NextBool(healingStarChance) && !death)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), fireSparklesFrom, vector2, ModContent.ProjectileType<HolyLight>(), 0, 0f, Main.myPlayer, 0f, StarHeal);
                                }
                                else if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), fireSparklesFrom, vector2, ModContent.ProjectileType<HolyBurnOrb>(), StarDamage.CalculateProvidenceDamage(), 0f, Main.myPlayer);
                            }

                            // Radial offset
                            NPC.ai[2] += 10f;
                        }

                        NPC.ForceNetUpdate(false);
                    }
                    else
                    {
                        NPC.ai[2] = 0f;

                        totalFlameProjectiles = 16;
                        if (NPC.ai[3] % (divisor * totalFlameProjectiles) == 0f)
                        {
                            calamityGlobalNPC.newAI[1] += 1f;
                            double radians = MathHelper.TwoPi / totalFlameProjectiles;
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, NPC.Center);
                            double angleA = radians * 0.5;
                            double angleB = MathHelper.ToRadians(90f) - angleA;
                            float velocityX = (float)(cocoonProjVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                            Vector2 spinningPoint = NPC.ai[3] % (divisor * totalFlameProjectiles * 2f) == 0f ? new Vector2(-velocityX, -cocoonProjVelocity) : new Vector2(0f, -cocoonProjVelocity);
                            for (int i = 0; i < totalFlameProjectiles; i++)
                            {
                                Vector2 vector2 = spinningPoint.RotatedBy(radians * i);

                                if (Main.rand.NextBool(healingStarChance) && !death)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), fireSparklesFrom, vector2, ModContent.ProjectileType<HolyLight>(), 0, 0f, Main.myPlayer, 0f, StarHeal);
                                }
                                else if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), fireSparklesFrom, vector2, ModContent.ProjectileType<HolyBurnOrb>(), StarDamage.CalculateProvidenceDamage(), 0f, Main.myPlayer);
                            }
                        }
                    }

                    // Fire a flame towards every player, with a limit of 5
                    if (NPC.ai[3] % 60f == 0f && expertMode)
                    {
                        List<int> targets = new List<int>();
                        foreach (Player plr in Main.ActivePlayers)
                        {
                            if (!plr.dead)
                                targets.Add(plr.whoAmI);

                            if (targets.Count > 4)
                                break;
                        }
                        foreach (int t in targets)
                        {
                            Vector2 velocity2 = Vector2.Normalize(Main.player[t].Center - fireSparklesFrom) * cocoonProjVelocity * 1.5f;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), fireSparklesFrom, velocity2, ModContent.ProjectileType<HolyBurnOrb>(), StarDamage.CalculateProvidenceDamage(), 0f, Main.myPlayer);

                            Color dustColor = Main.hslToRgb(Main.rgbToHsl(fullPowerAI ? new Color(100, 200, 250) : Color.Orange).X, 1f, 0.5f);
                            dustColor.A = 255;
                            int maxDust = 3;
                            for (int j = 0; j < maxDust; j++)
                            {
                                int dust = Dust.NewDust(fireFrom, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, dustColor, 1f);
                                Main.dust[dust].position = fireFrom;
                                Main.dust[dust].velocity = velocity2 * cocoonProjVelocity * 2f;
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].scale = 3f;
                                Main.dust[dust].fadeIn = Main.rand.NextFloat() * 2f;
                                Dust dust2 = Dust.BetterCloneDust(dust);
                                Dust dust3 = dust2;
                                dust3.scale /= 2f;
                                dust3 = dust2;
                                dust3.fadeIn /= 2f;
                                dust2.color = new Color(255, 255, 255, 255);
                            }
                        }
                    }

                    if (NPC.ai[3] == 0f)
                        DespawnSpecificProjectiles();

                    // Air is burning text
                    NPC.ai[3] += 1f;
                    if (NPC.ai[3] >= (phaseTime * 1.5f) && !text)
                    {
                        text = true;
                        string key = "Mods.CalamityMod.Status.Boss.ProfanedBossText";
                        Color messageColor = Color.Orange;

                        CalamityUtils.BroadcastLocalizedText(key, messageColor);
                    }

                    // Inflict Icarus Folly
                    if (NPC.ai[3] >= (phaseTime * 2f))
                    {
                        if (!Main.dedServ)
                        {
                            Player player2 = Main.LocalPlayer;
                            bool inLiquid = player2.Calamity().countsAsAnyWet && !player2.lavaWet;

                            if (!player2.dead && player2.active && Vector2.Distance(player2.Center, NPC.Center) < 2800f && !inLiquid)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, player2.Center);
                                player2.AddBuff(ModContent.BuffType<IcarusFolly>(), 3000, true);

                                for (int i = 0; i < 40; i++)
                                {
                                    int icarusFollyDust = Dust.NewDust(new Vector2(player2.position.X, player2.position.Y),
                                        player2.width, player2.height, dustType, 0f, 0f, 100, default, 2f);
                                    Main.dust[icarusFollyDust].velocity *= 3f;
                                    Main.dust[icarusFollyDust].noGravity = true;
                                    if (Main.rand.NextBool())
                                    {
                                        Main.dust[icarusFollyDust].scale = 0.5f;
                                        Main.dust[icarusFollyDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                                    }
                                }

                                for (int j = 0; j < 60; j++)
                                {
                                    int icarusFollyDust2 = Dust.NewDust(new Vector2(player2.position.X, player2.position.Y),
                                        player2.width, player2.height, dustType, 0f, 0f, 100, default, 3f);
                                    Main.dust[icarusFollyDust2].noGravity = true;
                                    Main.dust[icarusFollyDust2].velocity *= 5f;
                                    icarusFollyDust2 = Dust.NewDust(new Vector2(player2.position.X, player2.position.Y),
                                        player2.width, player2.height, dustType, 0f, 0f, 100, default, 2f);
                                    Main.dust[icarusFollyDust2].velocity *= 2f;
                                    Main.dust[icarusFollyDust2].noGravity = true;
                                }
                            }
                        }

                        text = false;
                        starDir *= -1;
                        AIState = (int)Phase.PhaseChange;
                        NPC.localAI[2] = attackDelayAfterCocoon;
                        NPC.TargetClosest();
                    }

                    break;

                case (int)Phase.MoltenBlobs:

                    // Attack delay after cocoon phase
                    if (delayAttacks)
                    {
                        NPC.localAI[2] -= 1f;
                        return;
                    }

                    if (distanceX > distanceNeededToShoot && NPC.position.Y < player.position.Y)
                    {
                        NPC.ai[3] += 1f;

                        int shootBoost = death ? (int)Math.Round(5f * (1f - lifeRatio)) : (int)Math.Round(4f * (1f - lifeRatio));
                        int projectileShootGateValue = (expertMode ? 24 : 26) - shootBoost;

                        projectileShootGateValue = (int)(projectileShootGateValue * attackRateMult);

                        if (NPC.ai[3] >= projectileShootGateValue)
                            NPC.ai[3] = -projectileShootGateValue;

                        if (NPC.ai[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 projectileFirePosition = new Vector2(NPC.Center.X + NPC.velocity.SafeNormalize(Vector2.UnitX).X * 120f, NPC.Center.Y);
                            float velocityBoost = death ? 4f * (1f - lifeRatio) : 2.5f * (1f - lifeRatio);
                            float projSpeed = (revenge ? 12f : expertMode ? 10.5f : 9f) + velocityBoost;
                            Vector2 predictionAmount = player.velocity * 100f;
                            Vector2 projectileVelocity = (player.Center + (predictiveShots ? predictionAmount : Vector2.Zero) - projectileFirePosition).SafeNormalize(Vector2.UnitY) * projSpeed * 0.1f;
                            Vector2 explodePosition = predictiveShots ? (player.position + predictionAmount) : player.position;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileFirePosition, projectileVelocity, ModContent.ProjectileType<MoltenBlast>(), MoltenBlastDamage.CalculateProvidenceDamage(), 0f, Main.myPlayer, explodePosition.X, explodePosition.Y);
                        }
                    }
                    else if (NPC.ai[3] < 0f)
                        NPC.ai[3] += 1f;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= phaseTime)
                    {
                        AIState = (int)Phase.PhaseChange;
                        NPC.TargetClosest();
                    }

                    break;

                case (int)Phase.HolyBomb:

                    // Attack delay after cocoon phase
                    if (delayAttacks)
                    {
                        NPC.localAI[2] -= 1f;
                        return;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.ai[3] += 1f;

                        int shootBoost = death ? (int)Math.Round(12f * (1f - lifeRatio)) : (int)Math.Round(10f * (1f - lifeRatio));
                        int projectileShootGateValue = (expertMode ? 73 : 77) - shootBoost;

                        projectileShootGateValue = (int)(projectileShootGateValue * attackRateMult);

                        if (NPC.ai[3] >= projectileShootGateValue)
                        {
                            NPC.ai[3] = 0f;

                            Vector2 shootFrom = new Vector2(NPC.Center.X, NPC.position.Y + NPC.height - 14f * NPC.scale);

                            float projectileVelocityY = NPC.velocity.Y;
                            if (projectileVelocityY < 0f)
                                projectileVelocityY = 0f;

                            projectileVelocityY += expertMode ? 4f : 3f;

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom.X, shootFrom.Y, NPC.velocity.X * 0.25f, projectileVelocityY, ModContent.ProjectileType<HolyBomb>(), FireSentryDamage.CalculateProvidenceDamage(), 0f, Main.myPlayer);
                        }
                    }

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= phaseTime)
                    {
                        AIState = (int)Phase.PhaseChange;
                        NPC.TargetClosest();
                    }

                    break;

                case (int)Phase.SpearCocoon:

                    
                    borderPosition ??= NPC.Center;
                    borderPosition = Vector2.Lerp(borderPosition.Value, NPC.Center, 0.02f);

                    if (!targetDead && !getFuckedAI)
                    {
                        if (NPC.velocity.Length() <= 2f)
                            NPC.velocity = Vector2.Zero;

                        if (NPC.velocity.Length() > 2f)
                        {
                            NPC.velocity *= 0.9f;
                            return;
                        }
                    }

                    if (NPC.ai[1] == 0f)
                        DespawnSpecificProjectiles();

                    NPC.ai[2] += spearRate;
                    if (NPC.ai[2] >= (float)(baseSpearRate * attackRateMult))
                    {
                        NPC.ai[2] = 0f;

                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, fireFrom);

                        int projectileType = ModContent.ProjectileType<HolySpear>();

                        int totalDustPerSpear = 15;

                        if (calamityGlobalNPC.newAI[2] % 2f == 0f)
                        {
                            int totalSpearProjectiles = 12;
                            double radians = MathHelper.TwoPi / totalSpearProjectiles;
                            Vector2 spinningPoint = Vector2.Normalize(new Vector2(-calamityGlobalNPC.newAI[1], -cocoonProjVelocity));

                            for (int i = 0; i < totalSpearProjectiles; i++)
                            {
                                Vector2 vector2 = spinningPoint.RotatedBy(radians * i) * cocoonProjVelocity;

                                for (int k = 0; k < totalDustPerSpear; k++)
                                {
                                    int dust = Dust.NewDust(fireFrom, 30, 30, dustType, vector2.X, vector2.Y, 0, default, 1f);
                                    Main.dust[dust].noGravity = true;
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom, vector2, projectileType, SpearDamage.CalculateProvidenceDamage(), 0f, Main.myPlayer);

                                    if (Main.getGoodWorld)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom, -vector2, projectileType, SpearDamage.CalculateProvidenceDamage(), 0f, Main.myPlayer);
                                }
                            }

                            if (spearRateIncrease > 1f)
                                spearRateIncrease = 1f;

                            float radialOffset = MathHelper.Lerp(0.2f, 0.4f, spearRateIncrease);
                            calamityGlobalNPC.newAI[1] += radialOffset * spearDir;
                        }

                        calamityGlobalNPC.newAI[2] += 1f;

                        cocoonProjVelocity = death ? 14f : revenge ? 13f : expertMode ? 12f : 10f;
                        Vector2 velocity2 = Vector2.Normalize(player.Center - fireFrom) * cocoonProjVelocity;

                        for (int k = 0; k < totalDustPerSpear; k++)
                        {
                            int dust = Dust.NewDust(fireFrom, 30, 30, dustType, velocity2.X, velocity2.Y, 0, default, 1f);
                            Main.dust[dust].noGravity = true;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom, velocity2, projectileType, SpearDamage.CalculateProvidenceDamage(), 0f, Main.myPlayer, 1f, 0f);

                            if (Main.getGoodWorld)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom, -velocity2, projectileType, SpearDamage.CalculateProvidenceDamage(), 0f, Main.myPlayer, 1f, 0f);
                        }
                    }

                    NPC.ai[3] += 1f;
                    if (NPC.ai[3] >= phaseTime)
                    {

                        AIState = (int)Phase.PhaseChange;
                        spearDir *= -1;
                        NPC.localAI[2] = attackDelayAfterCocoon;
                        NPC.TargetClosest();
                    }

                    break;

                case (int)Phase.Crystal:

                    if (!targetDead && !getFuckedAI)
                        NPC.velocity *= 0.9f;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= crystalPhaseTime)
                    {
                        if (NPC.ai[1] == crystalPhaseTime)
                        {
                            Vector2 crystalSpawnPos = new Vector2(player.Center.X, player.Center.Y - 360f);
                            float distanceFromCrystalSpawnPos = Vector2.Distance(crystalSpawnPos, NPC.Center);

                            int maxHealDustIterations = (int)distanceFromCrystalSpawnPos;
                            int maxDust = 100;
                            int dustDivisor = maxHealDustIterations / maxDust;
                            if (dustDivisor < 2)
                                dustDivisor = 2;

                            Vector2 dustLineStart = new Vector2(NPC.Center.X, NPC.Center.Y + 64f * NPC.scale);
                            Vector2 dustLineEnd = crystalSpawnPos;
                            Vector2 currentDustPos = default;
                            Vector2 spinningpoint = new Vector2(0f, -3f).RotatedByRandom(MathHelper.Pi);
                            Vector2 dustVelocityMult = new Vector2(2.1f, 2f);
                            int dustSpawned = 0;
                            int maxDustLines = 3;
                            int blue = Main.DiscoB;
                            for (int i = 0; i < maxDustLines; i++)
                            {
                                for (int j = 0; j < maxHealDustIterations; j++)
                                {
                                    if (j % dustDivisor == 0)
                                    {
                                        currentDustPos = Vector2.Lerp(dustLineStart, dustLineEnd, j / (float)maxHealDustIterations);
                                        Color dustColor = Main.hslToRgb(Main.rgbToHsl(fullPowerAI ? new Color(100, 200, 250) : new Color(255, 200, Math.Abs(Math.Abs(blue) - (int)(dustSpawned * 2.55f)))).X, 1f, 0.5f);
                                        dustColor.A = 255;
                                        int dust = Dust.NewDust(currentDustPos, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, dustColor, 1f);
                                        Main.dust[dust].position = currentDustPos + new Vector2(32f, 32f).RotatedByRandom(MathHelper.TwoPi) * i;
                                        Main.dust[dust].velocity = spinningpoint.RotatedBy(MathHelper.TwoPi * j / maxHealDustIterations) * dustVelocityMult * (0.8f + Main.rand.NextFloat() * 0.4f);
                                        Main.dust[dust].noGravity = true;
                                        Main.dust[dust].scale = 1f + i;
                                        Main.dust[dust].fadeIn = Main.rand.NextFloat() * 2f;
                                        Dust dust2 = Dust.BetterCloneDust(dust);
                                        Dust dust3 = dust2;
                                        dust3.scale /= 2f;
                                        dust3 = dust2;
                                        dust3.fadeIn /= 2f;
                                        dust2.color = new Color(255, 255, 255, 255);
                                        dustSpawned++;
                                    }
                                }

                                if (!fullPowerAI)
                                    blue -= 255 / (maxDustLines - 1);
                            }

                            int totalDust = 36;
                            int circleDustSpawned = 0;
                            for (int k = 0; k < totalDust; k++)
                            {
                                Vector2 dustSpawnPos = Vector2.Normalize(NPC.velocity) * new Vector2(80f, 160f);
                                dustSpawnPos = dustSpawnPos.RotatedBy((double)((k - (totalDust / 2 - 1)) * MathHelper.TwoPi / totalDust), default) + dustLineEnd;
                                Vector2 dustVelocity = dustSpawnPos - dustLineEnd;
                                Color dustColor = Main.hslToRgb(Main.rgbToHsl(fullPowerAI ? new Color(100, 200, 250) : new Color(255, 200, Math.Abs(Math.Abs(blue) - (int)(circleDustSpawned * 7.08f)))).X, 1f, 0.5f);
                                dustColor.A = 255;
                                int dust = Dust.NewDust(dustSpawnPos + dustVelocity, 0, 0, DustID.RainbowMk2, dustVelocity.X, dustVelocity.Y, 0, dustColor, 1.4f);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].noLight = true;
                                Main.dust[dust].velocity = dustVelocity * 0.33f;
                                circleDustSpawned++;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float timeLeft = fullPowerAI ? (float)(getFuckedAI ? gfbCrystalTime : enragedCrystalTime) : 0f;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), crystalSpawnPos, Vector2.Zero, ModContent.ProjectileType<ProvidenceCrystal>(), CrystalDamage.CalculateProvidenceDamage(), 0f, Main.myPlayer, lifeRatio, 0f, timeLeft);
                            }
                        }

                        if (NPC.ai[1] >= crystalPhaseTime + enragedCrystalTime || !fullPowerAI)
                        {
                            AIState = (int)Phase.PhaseChange;
                            NPC.TargetClosest();
                        }
                    }

                    break;

                case (int)Phase.Laser:

                    Vector2 dustPosOffset = new Vector2(27f, 59f);

                    float rotation = (fullPowerAI ? 445f : 460f) + (guardianAmt * 5);

                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] < 120f)
                    {
                        if (NPC.ai[2] >= 40f)
                        {
                            int extraDustAmt = 0;
                            if (NPC.ai[2] >= 80f)
                                extraDustAmt = 1;

                            for (int d = 0; d < 1 + extraDustAmt; d++)
                            {
                                float scalar = 1.2f;
                                if (d % 2 == 1)
                                    scalar = 2.8f;

                                Vector2 dustPos = new Vector2(NPC.Center.X, NPC.Center.Y + 64f * NPC.scale) + ((float)Main.rand.NextDouble() * MathHelper.TwoPi).ToRotationVector2() * dustPosOffset / 2f;
                                int index = Dust.NewDust(dustPos - Vector2.One * 8f, 16, 16, dustType, NPC.velocity.X / 2f, NPC.velocity.Y / 2f, 0, default, 1f);
                                Main.dust[index].velocity = Vector2.Normalize(NPC.Center - dustPos) * 3.5f * (10f - extraDustAmt * 2f) / 10f;
                                Main.dust[index].noGravity = true;
                                Main.dust[index].scale = scalar;
                            }
                        }
                    }
                    else if (NPC.ai[2] < (revenge ? 220f : 300f))
                    {
                        if (NPC.ai[2] == 120f)
                        {
                            if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && Vector2.Distance(Main.LocalPlayer.Center, NPC.Center) < 2800f)
                            {
                                SoundEngine.PlaySound(HolyRaySound, Main.LocalPlayer.Center);
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 velocity = player.Center - NPC.Center;
                                velocity.Normalize();

                                float beamDirection = -1f;
                                if (velocity.X < 0f)
                                    beamDirection = 1f;

                                // 60 degrees offset
                                velocity = velocity.RotatedBy(-(double)beamDirection * MathHelper.TwoPi / 6f);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 64f * NPC.scale, velocity.X, velocity.Y, ModContent.ProjectileType<ProvidenceHolyRay>(), RayDamage.CalculateProvidenceDamage(), 0f, Main.myPlayer, beamDirection * MathHelper.TwoPi / rotation, NPC.whoAmI, ai2: 2f);

                                // -60 degrees offset
                                if (revenge)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 64f * NPC.scale, -velocity.X, -velocity.Y, ModContent.ProjectileType<ProvidenceHolyRay>(), RayDamage.CalculateProvidenceDamage(), 0f, Main.myPlayer, -beamDirection * MathHelper.TwoPi / rotation, NPC.whoAmI, ai2: 2f);

                                if (fullPowerAI && lifeRatio < 0.5f)
                                {
                                    rotation *= 0.33f;
                                    velocity = velocity.RotatedBy(-(double)beamDirection * MathHelper.TwoPi / 2f);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 64f * NPC.scale, velocity.X, velocity.Y, ModContent.ProjectileType<ProvidenceHolyRay>(), RayDamage.CalculateProvidenceDamage(), 0f, Main.myPlayer, beamDirection * MathHelper.TwoPi / rotation, NPC.whoAmI, ai2: 2f);

                                    if (revenge)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 64f * NPC.scale, -velocity.X, -velocity.Y, ModContent.ProjectileType<ProvidenceHolyRay>(), RayDamage.CalculateProvidenceDamage(), 0f, Main.myPlayer, -beamDirection * MathHelper.TwoPi / rotation, NPC.whoAmI, ai2: 2f);
                                }

                                NPC.ForceNetUpdate(false);
                            }
                        }
                    }

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= (revenge ? 235f : 315f))
                    {
                        AIState = (int)Phase.PhaseChange;
                        NPC.TargetClosest();
                    }

                    break;
            }
        }

        public void DoDeathAnimation()
        {
            AIState = (int)Phase.HolyFire;
            useDefenseFrames = false;
            DeathAnimationTimer++;

            // Slow down to a halt and define rotation based off of that.
            NPC.velocity *= 0.9f;
            NPC.rotation = NPC.velocity.X * 0.004f;

            // Play an animation sound immediately. Also delete various projectiles.
            if (DeathAnimationTimer == 1f)
            {
                if (!Main.dedServ && Main.LocalPlayer.WithinRange(NPC.Center, 4800f))
                    SoundEngine.PlaySound(DeathAnimationSound with { Volume = 1.65f });

                DespawnSpecificProjectiles();

                int laserType = ModContent.ProjectileType<ProvidenceHolyRay>();
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.type != laserType)
                        continue;
                    p.Kill();
                }
            }

            // Begin fading out before the exploding sun animation happens.
            if (DeathAnimationTimer >= 370f)
                NPC.Opacity *= 0.97f;

            // Create an explosive wave shortly after the death animation begins.
            // The temporal offset coincides with the point at which the crystal shatter sound happens in the
            // above defeat scene sound.
            if (DeathAnimationTimer == 92f)
            {
                CalamityUtils.AddScreenshakeAt(NPC.Center, 5, 2000);

                Color hiColor = ProvUtils.GetProjectileColor(255, false);
                Color loColor = ProvUtils.GetProjectileColor(0, true);

                for (int i = 0; i < 30; i++)
                {
                    Particle p = new FlameParticle(NPC.Center + new Vector2(Main.rand.NextFloat(150), 0).RotatedByRandom(MathHelper.TwoPi), 40, Main.rand.NextFloat(0.5f, 0.75f), Main.rand.NextFloat(1f, 2.5f), hiColor, loColor);
                    p.Velocity = new Vector2(Main.rand.NextFloat(3f, 19f), 0).RotatedByRandom(MathHelper.TwoPi);
                    GeneralParticleHandler.SpawnParticle(p);
                    GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(NPC.Center, new Vector2(Main.rand.NextFloat(12f, 40f), 0).RotatedByRandom(MathHelper.TwoPi), loColor, 60, Main.rand.NextFloat(0.75f, 1.75f), 1f, Main.rand.NextFloat(-0.05f, 0.05f), true));
                }

                GeneralParticleHandler.SpawnParticle(new CustomPulse(NPC.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.1f, 15));

                for (float i = 0; i < 3; i += 0.25f)
                {
                    GeneralParticleHandler.SpawnParticle(new CustomPulse(NPC.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One * i, Main.rand.NextFloat(MathHelper.TwoPi), 0.05f, 0.25f, 35));
                    GeneralParticleHandler.SpawnParticle(new CustomPulse(NPC.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/ShatteredExplosion", Vector2.One * i, Main.rand.NextFloat(MathHelper.TwoPi), 0.05f, 0.175f, 25));
                }

                SoundEngine.PlaySound(HolyBlast.ImpactSound, NPC.Center);
            }

            // Explode as an enormous holy star before dying and dropping loot.
            if (Main.netMode != NetmodeID.MultiplayerClient && DeathAnimationTimer == 310f)
            {
                for (int i = 0; i < 80; i++)
                {
                    Vector2 sparkleVelocity = Main.rand.NextVector2Circular(23f, 23f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, sparkleVelocity, ModContent.ProjectileType<MajesticSparkle>(), 0, 0f);
                }
            }

            // Idly release harmless cindiers.
            if (DeathAnimationTimer >= 92f)
            {
                CalamityUtils.AddScreenshakeAt(NPC.Center, MathHelper.Lerp(0, 0.6f, ((float)DeathAnimationTimer - 92f) / 300f), 1000);

                int shootRate = (int)MathHelper.Lerp(12f, 5f, Utils.GetLerpValue(0f, 250f, DeathAnimationTimer, true));
                if (DeathAnimationTimer % shootRate == shootRate - 1f)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 shootVelocity = Main.rand.NextVector2CircularEdge(13f, 13f) * Main.rand.NextFloat(1.4f, 2.3f);
                        Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, shootVelocity, ModContent.ProjectileType<SwirlingFire>(), 0, 0f, 255, ai1: (int)NPC.localAI[1]);
                        proj.ai[2] = MathHelper.Lerp(0.5f, 2.5f, (float)DeathAnimationTimer / 300f);
                    }
                }
            }

            // Do periodic syncs.
            if (Main.dedServ && DeathAnimationTimer % 45f == 44f)
                NPC.ForceNetUpdate(false);

            // Die and create drops after the star is gone.
            if (DeathAnimationTimer >= 345f)
            {
                NPC.active = false;
                NPC.HitEffect();
                NPC.NPCLoot();
                NPC.ForceNetUpdate(false);
            }
        }

        public float CalculateBurnIntensity(float attackDelayAfterCocoon = 1f)
        {
            float distanceToTarget = Vector2.Distance(Main.player[NPC.target].Center, borderPosition ?? NPC.Center);
            float aiTimer = NPC.ai[3];

            // This bool is only relevant for non-Zenith enraged AI
            bool fullPower = NPC.localAI[1] == (float)BossMode.Enraged;

            float baseDistance = 2800f;
            float shorterFlameCocoonDistance = (CalamityWorld.death || fullPower) ? 600f : CalamityWorld.revenge ? 400f : Main.expertMode ? 200f : 0f;
            float shorterSpearCocoonDistance = (CalamityWorld.death || fullPower) ? 1000f : CalamityWorld.revenge ? 650f : Main.expertMode ? 300f : 0f;
            float shorterDistance = baseDistance - (AIState == (int)Phase.FlameCocoon ? shorterFlameCocoonDistance : shorterSpearCocoonDistance);

            bool guardianAlive = false;
            if (CalamityGlobalNPC.holyBossAttacker != -1 && Main.npc[CalamityGlobalNPC.holyBossAttacker].active)
                guardianAlive = true;


            if (CalamityGlobalNPC.holyBossDefender != -1 && Main.npc[CalamityGlobalNPC.holyBossDefender].active)
                guardianAlive = true;

            if (CalamityGlobalNPC.holyBossHealer != -1 && Main.npc[CalamityGlobalNPC.holyBossHealer].active)
                guardianAlive = true;

            float maxDistance = baseDistance;

            // A factor which measures how much of the distance shortening shave-off should be taken into account.
            // It is determined based on how much time has elapsed during the attack thus far, specifically for the two cocoon attacks.
            // This shave-off does not happen when guardians are present.
            float shorterDistanceFade = Utils.GetLerpValue(0f, 120f, aiTimer, true);

            // Distance does not get shorter if in GFB / Guardians are alive
            if (!guardianAlive && NPC.localAI[1] < (float)BossMode.Rainbow)
            {
                if (AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon)
                    maxDistance = MathHelper.Lerp(baseDistance, shorterDistance, shorterDistanceFade);
                else if (attackDelayAfterCocoon > 1f)
                    maxDistance = MathHelper.Lerp(baseDistance, shorterDistance, (NPC.localAI[2] / attackDelayAfterCocoon));
            }

            float drawFireDistanceStart = maxDistance - 800f;
            float previousBorderEnd = borderRadius;
            float clampedDistance = MathHelper.Clamp(maxDistance, previousBorderEnd - 10, previousBorderEnd + 10);

            // Only set the border distance if it's not called from playermisceffects, that way it has mod compatability
            borderRadius = clampedDistance;
            return Utils.GetLerpValue(drawFireDistanceStart, clampedDistance, distanceToTarget, true);
        }

        private void DespawnSpecificProjectiles(bool everything = false)
        {
            for (int x = 0; x < Main.maxProjectiles; x++)
            {
                Projectile projectile = Main.projectile[x];
                if (projectile.active)
                {
                    if (projectile.type == ModContent.ProjectileType<HolyFire2>() || projectile.type == ModContent.ProjectileType<HolyFlare>() || projectile.type == ModContent.ProjectileType<HolyBlastFrags>())
                        projectile.Kill();
                    else if (projectile.type == ModContent.ProjectileType<HolyBlast>() || projectile.type == ModContent.ProjectileType<HolyFire>())
                        projectile.active = false;

                    if (everything)
                    {
                        if (projectile.type == ModContent.ProjectileType<ProvidenceHolyRay>() || projectile.type == ModContent.ProjectileType<ProvidenceCrystal>() ||
                            projectile.type == ModContent.ProjectileType<ProvidenceCrystalShard>() || projectile.type == ModContent.ProjectileType<HolySpear>() ||
                            projectile.type == ModContent.ProjectileType<HolyBomb>() || projectile.type == ModContent.ProjectileType<MoltenBlob>() ||
                            projectile.type == ModContent.ProjectileType<HolyBurnOrb>() || projectile.type == ModContent.ProjectileType<HolyLight>())
                            projectile.Kill();
                        else if (projectile.type == ModContent.ProjectileType<MoltenBlast>())
                            projectile.active = false;
                    }
                }
            }
        }

        public override bool CheckDead()
        {
            NPC.life = 1;
            DespawnSpecificProjectiles(true);
            Dying = true;
            NPC.active = true;
            NPC.dontTakeDamage = true;
            NPC.ForceNetUpdate(false);

            return false;
        }

        public override void OnKill()
        {
            CalamityUtils.AddScreenshakeAt(NPC.Center, 10, 2000);

            Color hiColor = ProvUtils.GetProjectileColor(255, false);
            Color loColor = ProvUtils.GetProjectileColor(0, true);

            for (int i = 0; i < 30; i++)
            {
                Particle p = new FlameParticle(NPC.Center + new Vector2(Main.rand.NextFloat(150), 0).RotatedByRandom(MathHelper.TwoPi), 40, Main.rand.NextFloat(0.5f, 0.75f), Main.rand.NextFloat(1f, 2.5f), hiColor, loColor);
                p.Velocity = new Vector2(Main.rand.NextFloat(3f, 19f), 0).RotatedByRandom(MathHelper.TwoPi);
                GeneralParticleHandler.SpawnParticle(p);
                GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(NPC.Center, new Vector2(Main.rand.NextFloat(12f, 40f), 0).RotatedByRandom(MathHelper.TwoPi), loColor, 60, Main.rand.NextFloat(0.75f, 1.75f), 1f, Main.rand.NextFloat(-0.05f, 0.05f), true));
            }

            GeneralParticleHandler.SpawnParticle(new CustomPulse(NPC.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.1f, 15));

            for (float i = 0; i < 3; i += 0.25f)
            {
                GeneralParticleHandler.SpawnParticle(new CustomPulse(NPC.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One * i, Main.rand.NextFloat(MathHelper.TwoPi), 0.05f, 0.35f, 35));
                GeneralParticleHandler.SpawnParticle(new CustomPulse(NPC.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/ShatteredExplosion", Vector2.One * i, Main.rand.NextFloat(MathHelper.TwoPi), 0.05f, 0.275f, 25));
            }

            // Don't bother running any of this in Boss Rush.
            if (BossRushEvent.BossRushActive)
                return;

            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // If Providence has not been killed, notify players of Uelibloom Ore
            if (!DownedBossSystem.downedProvidence)
            {
                string key2 = "Mods.CalamityMod.Status.Progression.ProfanedBossText3";
                Color messageColor2 = Color.Orange;
                string key3 = "Mods.CalamityMod.Status.Progression.TreeOreText";
                Color messageColor3 = Color.LightGreen;

                CalamityUtils.SpawnOre(ModContent.TileType<UelibloomOre>(), 17E-05, 0.55f, 0.9f, 8, 14, TileID.Mud);

                CalamityUtils.BroadcastLocalizedText(key2, messageColor2);
                CalamityUtils.BroadcastLocalizedText(key3, messageColor3);
            }

            if (challenge)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue("Mods.CalamityMod.Status.Progression.ProfanedBossText4"), Color.DarkOrange);
                }
            }

            // Mark Providence as dead
            DownedBossSystem.downedProvidence = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ProvidenceBag>()));

            // Drops Mark of Providence on first kill
            npcLoot.AddIf(() => !DownedBossSystem.downedProvidence, ModContent.ItemType<MarkofProvidence>(), desc: DropHelper.FirstKillText);

            // Elysian Wings and Elysian Aegis drop outside of the Treasure Bag, for all players
            npcLoot.Add(DropHelper.PerPlayer(ModContent.ItemType<ElysianWings>()));
            npcLoot.Add(DropHelper.PerPlayer(ModContent.ItemType<ElysianAegis>()));

            npcLoot.DefineConditionalDropSet(DropHelper.If((info) =>
            {
                Providence prov = info.npc.ModNPC<Providence>();
                return prov.challenge;
            }, () => Main.expertMode, DropHelper.ProvidenceChallengeText)).Add(ModContent.ItemType<ProfanedSoulCrystal>());

            npcLoot.AddIf(info =>
            {
                Providence prov = info.npc.ModNPC<Providence>();
                return prov.hasBeenGivenFullPower;
            }, ModContent.ItemType<ProfanedMoonlightDye>(), 1, 4, 4, desc: DropHelper.ProvidenceEnragedText);

            npcLoot.AddIf(info =>
            {
                Providence prov = info.npc.ModNPC<Providence>();
                return prov.hasBeenGivenFullPower;
            }, ModContent.ItemType<DivineGeode>(), 1, 75, 90);

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<HolyCollider>(),
                    ModContent.ItemType<BurningRevelation>(),
                    ModContent.ItemType<TelluricGlare>(),
                    ModContent.ItemType<BlissfulBombardier>(),
                    ModContent.ItemType<PurgeGuzzler>(),
                    ModContent.ItemType<DazzlingStabberStaff>(),
                    ModContent.ItemType<MoltenAmputator>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));
                normalOnly.Add(ModContent.ItemType<PristineFury>(), 10);

                // Equipment
                // 16NOV2025: Ozzatron: item has been chosen as the "Expert gatekept" item for this Calamity boss
                // normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<BlazingCore>()));

                // Materials
                normalOnly.Add(ModContent.ItemType<DivineGeode>(), 1, 50, 60);
                normalOnly.Add(ModContent.ItemType<UnholyEssence>(), 1, 30, 40);

                // Vanity
                normalOnly.Add(ModContent.ItemType<ProvidenceMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<ProvidenceTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<ProvidenceRelic>());

            // GFB ASE and Blasphemous Donut drops
            var GFBOnly = npcLoot.DefineConditionalDropSet(DropHelper.GFB);
            {
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<AscendantSpiritEssence>(), 1, 1, 99), true);
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<BlasphemousDonut>(), 1, 1117, 2201), true); // Reference to the versions the guards were added and got their latest resprites
            }

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedProvidence, ModContent.ItemType<LoreProvidence>(), desc: DropHelper.FirstKillText);
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ModContent.ItemType<SupremeHealingPotion>();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool offColor = NPC.localAI[1] != (float)BossMode.Normal && (NPC.Calamity().newAI[3] >= 180f || Main.zenithWorld);

            Texture2D texture = offColor ? TextureNight.Value : TextureAssets.Npc[Type].Value;
            Texture2D textureGlow = offColor ? TextureNight_Glow.Value : Texture_Glow.Value;
            Texture2D textureGlow2 = Texture_Glow_2.Value;

            void drawProvidenceInstance(Vector2 drawOffset, Color? colorOverride)
            {
                // This night bool is used for any off-color activity

                string baseTextureString = "CalamityMod/NPCs/Providence/";
                string baseGlowTextureString = baseTextureString + "Glowmasks/";


                float spawnAnimationTime = 180f;
                bool spawnAnimation = NPC.Calamity().newAI[3] < spawnAnimationTime;

                // Bloom circle effect should only appear on spawn animation and not enrage animation, since you can't see it
                // Okay it should also appear in Boss Rush as well
                if (spawnAnimation && (NPC.localAI[1] == (float)BossMode.Normal || BossRushEvent.BossRushActive))
                {
                    Asset<Texture2D> orbTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
                    float sc = CalamityUtils.CircInEasing((float)NPC.Calamity().newAI[3] / (float)spawnAnimationTime, 1);

                    for (int i = 0; i < 3; i++)
                    {
                        int frameY = (int)(NPC.Calamity().newAI[3] / 4) % 4;
                        float sc1 = MathHelper.Lerp(1, 0, sc);
                        Main.EntitySpriteDraw(orbTex.Value, NPC.Center - Main.screenPosition + new Vector2(Main.rand.NextFloat(MathHelper.Lerp(0f, 10f, sc)), 0).RotatedByRandom(MathHelper.TwoPi), orbTex.Frame(), ProvUtils.GetProjectileColor(0, false).MultiplyRGBA(new Color(sc * 45f, sc * 11f, sc * 22f, 0f)), 0f, orbTex.Frame().Center(), (sc * 4.4f) + (float)(Math.Cos((float)NPC.Calamity().newAI[3] / 10) * sc), SpriteEffects.None);
                    }
                }

                {
                    if (AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon)
                    {
                        if (!useDefenseFrames)
                        {
                            texture = offColor ? TextureDefenseNight.Value : TextureDefense.Value;
                            textureGlow = offColor ? TextureDefenseNight_Glow.Value : TextureDefense_Glow.Value;
                            textureGlow2 = TextureDefense_Glow_2.Value;
                        }
                        else
                        {
                            texture = offColor ? TextureDefenseAltNight.Value : TextureDefenseAlt.Value;
                            textureGlow = offColor ? TextureDefenseAltNight_Glow.Value : TextureDefenseAlt_Glow.Value;
                            textureGlow2 = TextureDefenseAlt_Glow_2.Value;
                        }
                    }
                    else
                    {
                        switch (frameUsed)
                        {
                            case 1:
                                texture = offColor ? TextureAltNight.Value : TextureAlt.Value;
                                textureGlow = offColor ? TextureAltNight_Glow.Value : TextureAlt_Glow.Value;
                                textureGlow2 = TextureAlt_Glow_2.Value;
                                break;

                            case 2:
                                texture = offColor ? TextureAttackNight.Value : TextureAttack.Value;
                                textureGlow = offColor ? TextureAttackNight_Glow.Value : TextureAttack_Glow.Value;
                                textureGlow2 = TextureAttack_Glow_2.Value;
                                break;

                            case 3:
                                texture = offColor ? TextureAttackAltNight.Value : TextureAttackAlt.Value;
                                textureGlow = offColor ? TextureAttackAltNight_Glow.Value : TextureAttackAlt_Glow.Value;
                                textureGlow2 = TextureAttackAlt_Glow_2.Value;
                                break;

                            default:
                                break;
                        }
                    }

                    SpriteEffects spriteEffects = SpriteEffects.None;
                    if (NPC.spriteDirection == 1)
                        spriteEffects = SpriteEffects.FlipHorizontally;

                    // Draw the main boss texture + its afterimages
                    Vector2 RotationCenter = new Vector2(TextureAssets.Npc[Type].Value.Width / 2, TextureAssets.Npc[Type].Value.Height / Main.npcFrameCount[Type] / 2);
                    Color BaseColor = Color.White;
                    float Brightness = 0.5f; // Ranges from 0 (full vibrance) to 1 (pure white)
                    int maxAfterimages = 5;

                    if (CalamityClientConfig.Instance.Afterimages)
                    {
                        for (int i = 1; i < maxAfterimages; i += 2)
                        {
                            Color AfterimageColor = drawColor;
                            AfterimageColor = Color.Lerp(AfterimageColor, BaseColor, Brightness);
                            AfterimageColor = NPC.GetAlpha(AfterimageColor);
                            AfterimageColor *= (maxAfterimages - i) / 15f;
                            if (colorOverride != null)
                                AfterimageColor = colorOverride.Value;

                            Vector2 AfterimageBodyPosition = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                            AfterimageBodyPosition -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
                            AfterimageBodyPosition += RotationCenter * NPC.scale + new Vector2(0f, NPC.gfxOffY) + drawOffset;
                            spriteBatch.Draw(texture, AfterimageBodyPosition, NPC.frame, AfterimageColor.MultiplyRGBA(Lighting.GetColor((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16)), NPC.rotation, RotationCenter, NPC.scale, spriteEffects, 0f);
                        }
                    }

                    Vector2 BasePosition = NPC.Center - screenPos;
                    BasePosition -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
                    BasePosition += RotationCenter * NPC.scale + new Vector2(0f, NPC.gfxOffY) + drawOffset;
                    Color finalDrawColor = NPC.IsABestiaryIconDummy ? Color.White : (colorOverride ?? Lighting.GetColor((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16)) * NPC.Opacity;
                    spriteBatch.Draw(texture, BasePosition, NPC.frame, finalDrawColor, NPC.rotation, RotationCenter, NPC.scale, spriteEffects, 0f);

                    // Draw the glowmask textures + their afterimages
                    // These are the colors at their strongest point. It'll shift towards white by the brightness value used earlier.
                    Color WingColor = ProvUtils.GetProjectileColor(0); // Default to normal
                    Color CrystalColor = Color.Violet;

                    if (NPC.localAI[1] == (float)BossMode.Rainbow)
                    {
                        if (Main.GlobalTimeWrappedHourly % 6f >= 5f) // Violet
                        {
                            WingColor = Color.Magenta;
                            CrystalColor = Color.GreenYellow;
                        }
                        else if (Main.GlobalTimeWrappedHourly % 6f >= 4f) // Blue
                        {
                            WingColor = Color.Cyan;
                            CrystalColor = Color.BlueViolet;
                        }
                        else if (Main.GlobalTimeWrappedHourly % 6f >= 3f) // Green
                        {
                            WingColor = Color.Green;
                            CrystalColor = Color.Gold;
                        }
                        else if (Main.GlobalTimeWrappedHourly % 6f >= 2f) // Yellow
                            CrystalColor = Color.Violet;
                        else if (Main.GlobalTimeWrappedHourly % 6f >= 1f) // Orange
                        {
                            WingColor = Color.Orange;
                            CrystalColor = Color.HotPink;
                        }
                        else // Red
                        {
                            WingColor = Color.Red;
                            CrystalColor = Color.BlueViolet;
                        }
                    }
                    else if (NPC.localAI[1] == (float)BossMode.Enraged && !spawnAnimation)
                    {
                        WingColor = Color.Cyan;
                        CrystalColor = Color.BlueViolet;
                    }

                    Color BaseWingColor = Color.Lerp(WingColor, BaseColor, Brightness) * NPC.Opacity;
                    Color BaseCrystalColor = Color.Lerp(CrystalColor, BaseColor, Brightness) * NPC.Opacity;
                    if (colorOverride != null)
                    {
                        BaseWingColor = colorOverride.Value;
                        BaseCrystalColor = colorOverride.Value;
                    }

                    Color GlowWingColor = ProvUtils.GetProjectileColor(NPC.GetAlpha(drawColor), true);

                    if (CalamityClientConfig.Instance.Afterimages)
                    {
                        for (int j = 1; j < maxAfterimages; j++)
                        {
                            Color AfterimageWingColor = ProvUtils.GetProjectileColor(0, true);
                            AfterimageWingColor = Color.Lerp(AfterimageWingColor, BaseColor, Brightness);
                            AfterimageWingColor = NPC.GetAlpha(AfterimageWingColor);
                            AfterimageWingColor *= (maxAfterimages - j) / 15f;

                            if (colorOverride != null)
                                AfterimageWingColor = colorOverride.Value;

                            Vector2 AfterimageGlowPosition = NPC.oldPos[j] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                            AfterimageGlowPosition -= new Vector2(textureGlow.Width, textureGlow.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
                            AfterimageGlowPosition += RotationCenter * NPC.scale + new Vector2(0f, NPC.gfxOffY) + drawOffset;
                            spriteBatch.Draw(textureGlow, AfterimageGlowPosition, NPC.frame, AfterimageWingColor, NPC.rotation, RotationCenter, NPC.scale, spriteEffects, 0f);

                            Color AfterimageCrystalColor = BaseCrystalColor;
                            AfterimageCrystalColor = Color.Lerp(AfterimageCrystalColor, BaseColor, Brightness);
                            AfterimageCrystalColor = NPC.GetAlpha(AfterimageCrystalColor);
                            AfterimageCrystalColor *= (maxAfterimages - j) / 15f;

                            if (colorOverride != null)
                                AfterimageCrystalColor = colorOverride.Value;

                            spriteBatch.Draw(textureGlow2, AfterimageGlowPosition, NPC.frame, AfterimageCrystalColor, NPC.rotation, RotationCenter, NPC.scale, spriteEffects, 0f);
                        }
                    }

                    if (!Dying)
                    {
                        NPC.DrawBackglow(GlowWingColor, 4f, SpriteEffects.None, NPC.frame, Main.screenPosition, textureGlow);

                        spriteBatch.Draw(textureGlow, BasePosition, NPC.frame, BaseWingColor, NPC.rotation, RotationCenter, NPC.scale, spriteEffects, 0f);

                        spriteBatch.Draw(textureGlow2, BasePosition, NPC.frame, BaseCrystalColor, NPC.rotation, RotationCenter, NPC.scale, spriteEffects, 0f);
                    }
                }
            }

            float burnIntensity = Utils.GetLerpValue(0f, 45f, DeathAnimationTimer, true);
            int totalProvidencesToDraw = (int)MathHelper.Lerp(1f, 30f, burnIntensity);
            for (int i = 0; i < totalProvidencesToDraw; i++)
            {
                float offsetAngle = MathHelper.TwoPi * i * 2f / totalProvidencesToDraw;
                float drawOffsetFactor = (float)Math.Sin(offsetAngle * 6f + Main.GlobalTimeWrappedHourly * MathHelper.Pi);
                drawOffsetFactor *= (float)Math.Pow(burnIntensity, 3f) * 50f;

                Vector2 drawOffset = offsetAngle.ToRotationVector2() * drawOffsetFactor;
                Color baseColor = Color.White * (MathHelper.Lerp(0.4f, 0.8f, burnIntensity) / totalProvidencesToDraw * 1.5f);
                baseColor.A = 0;

                baseColor = Color.Lerp(Color.White, baseColor, burnIntensity);
                drawProvidenceInstance(drawOffset, totalProvidencesToDraw == 1 ? null : (Color?)baseColor);
            }

            if (NPC.IsABestiaryIconDummy)
                return false;

            // Draw orange star while attacker is alive
            if (NPC.localAI[0] > 0f && NPC.localAI[0] < TimeForStarDespawn)
            {
                float lerpMult = MathHelper.Lerp(0.5f, 1.5f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi) / 2f + 1f);
                Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/StarProj").Value;
                float drawOffsetAmt = (AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon) ? 20f : 64f;
                Vector2 drawPos = NPC.Center + Vector2.UnitY * drawOffsetAmt * NPC.scale - Main.screenPosition;
                Color baseColor = Color.Lerp(Color.Yellow, Color.OrangeRed, (float)Math.Sin(Main.GlobalTimeWrappedHourly) / 2f + 1f);
                baseColor *= 0.5f;
                baseColor.A = 0;
                Color colorA = baseColor;
                Color colorB = baseColor * 0.5f;
                float opacityScaleDuringStarDespawn = (TimeForStarDespawn - NPC.localAI[0]) / TimeForStarDespawn;
                float scaleDuringStarDespawnScale = 1.8f;
                float scaleDuringStarDespawn = (1f - opacityScaleDuringStarDespawn) * scaleDuringStarDespawnScale;
                float colorScale = MathHelper.Lerp(0f, lerpMult, opacityScaleDuringStarDespawn);
                colorA *= colorScale;
                colorB *= colorScale;
                Vector2 origin = tex.Size() / 2f;
                Vector2 scale = new Vector2(1.5f + scaleDuringStarDespawn, 2.5f + scaleDuringStarDespawn) * lerpMult;
                float upRight = MathHelper.PiOver4 + NPC.rotation;
                float up = MathHelper.PiOver2 + NPC.rotation;
                float upLeft = 3f * MathHelper.PiOver4 + NPC.rotation;
                float left = MathHelper.Pi + NPC.rotation;
                Main.EntitySpriteDraw(tex, drawPos, null, colorA, upLeft, origin, scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(tex, drawPos, null, colorA, upRight, origin, scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(tex, drawPos, null, colorB, upLeft, origin, scale * 0.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(tex, drawPos, null, colorB, upRight, origin, scale * 0.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(tex, drawPos, null, colorA, up, origin, scale * 0.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(tex, drawPos, null, colorA, left, origin, scale * 0.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(tex, drawPos, null, colorB, up, origin, scale * 0.36f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(tex, drawPos, null, colorB, left, origin, scale * 0.36f, SpriteEffects.None, 0);
            }

            // Draw shields while defender is alive
            if (NPC.localAI[3] > 0f && NPC.localAI[3] < TimeForShieldDespawn)
            {
                float maxOscillation = 60f;
                float minScale = 0.9f;
                float maxPulseScale = 1f - minScale;
                float minOpacity = 0.5f;
                float maxOpacityScale = 1f - minOpacity;
                float currentOscillation = MathHelper.Lerp(0f, maxOscillation, ((float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.Pi) + 1f) * 0.5f);
                float shieldOpacity = minOpacity + maxOpacityScale * Utils.Remap(currentOscillation, 0f, maxOscillation, 1f, 0f);
                float oscillationRatio = currentOscillation / maxOscillation;
                float invertedOscillationRatio = 1f - (1f - oscillationRatio) * (1f - oscillationRatio);
                float oscillationScale = 1f - (1f - invertedOscillationRatio) * (1f - invertedOscillationRatio);
                float remappedOscillation = Utils.Remap(currentOscillation, maxOscillation - 15f, maxOscillation, 0f, 1f);
                float twoOscillationsMultipliedTogetherForScaleCalculation = remappedOscillation * remappedOscillation;
                float invertedOscillationUsedForScale = MathHelper.Lerp(minScale, 1f, 1f - twoOscillationsMultipliedTogetherForScaleCalculation);
                float shieldScale = (minScale + maxPulseScale * oscillationScale) * invertedOscillationUsedForScale;
                float smallerRemappedOscillation = Utils.Remap(currentOscillation, 20f, maxOscillation, 0f, 1f);
                float invertedSmallerOscillationRatio = 1f - (1f - smallerRemappedOscillation) * (1f - smallerRemappedOscillation);
                float smallerOscillationScale = 1f - (1f - invertedSmallerOscillationRatio) * (1f - invertedSmallerOscillationRatio);
                float shieldScale2 = (minScale + maxPulseScale * smallerOscillationScale) * invertedOscillationUsedForScale;
                Texture2D shieldTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleOpenCircleButBigger").Value;
                Rectangle shieldFrame = shieldTexture.Frame();
                Vector2 origin = shieldFrame.Size() * 0.5f;
                Vector2 shieldDrawPos = NPC.Center - screenPos;
                shieldDrawPos -= new Vector2(shieldTexture.Width, shieldTexture.Height) * NPC.scale / 2f;
                shieldDrawPos += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                float minHue = 0.06f;
                float maxHue = 0.18f;
                float opacityScaleDuringShieldDespawn = (TimeForShieldDespawn - NPC.localAI[3]) / TimeForShieldDespawn;
                float scaleDuringShieldDespawnScale = 1.8f;
                float scaleDuringShieldDespawn = (1f - opacityScaleDuringShieldDespawn) * scaleDuringShieldDespawnScale;
                float colorScale = MathHelper.Lerp(0f, shieldOpacity, opacityScaleDuringShieldDespawn);
                Color color = Main.hslToRgb(MathHelper.Lerp(maxHue - minHue, maxHue, ((float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi) + 1f) * 0.5f), 1f, 0.5f) * colorScale;
                Color color2 = Main.hslToRgb(MathHelper.Lerp(minHue, maxHue - minHue, ((float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.Pi * 3f) + 1f) * 0.5f), 1f, 0.5f) * colorScale;
                color2.A = 0;
                color *= 0.6f;
                color2 *= 0.6f;
                float scaleMult = 2.75f + scaleDuringShieldDespawn;
                spriteBatch.Draw(shieldTexture, shieldDrawPos, shieldFrame, color2, NPC.rotation, origin, shieldScale2 * scaleMult * 0.45f, SpriteEffects.None, 0f);
                spriteBatch.Draw(shieldTexture, shieldDrawPos, shieldFrame, color2, NPC.rotation, origin, shieldScale2 * scaleMult * 0.5f, SpriteEffects.None, 0f);

                // The shield for the border MUST be drawn before the main shield, it becomes incredibly visually obnoxious otherwise.

                // The scale used for the noise overlay polygons also grows and shrinks
                // This is intentionally out of sync with the shield, and intentionally desynced per player
                // Don't put this anywhere less than 0.25f or higher than 1f. The higher it is, the denser / more zoomed out the noise overlay is.
                float noiseScale = MathHelper.Lerp(0.4f, 0.8f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.3f) * 0.5f + 0.5f);

                // Define shader parameters
                Effect shieldEffect = Filters.Scene["CalamityMod:RoverDriveShield"].GetShader().Shader;
                shieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.058f); // Scrolling speed of polygonal overlay
                shieldEffect.Parameters["blowUpPower"].SetValue(2.8f);
                shieldEffect.Parameters["blowUpSize"].SetValue(0.4f);
                shieldEffect.Parameters["noiseScale"].SetValue(noiseScale);

                shieldEffect.Parameters["shieldOpacity"].SetValue(opacityScaleDuringShieldDespawn);
                shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);

                Color edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, color, color2);

                // Define shader parameters for shield color
                shieldEffect.Parameters["shieldColor"].SetValue(color.ToVector3());
                shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());

                var matrix = Main.GameViewMatrix.TransformationMatrix;
                using (Main.spriteBatch.Scope())
                {
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, shieldEffect, matrix);
                    // Fetch shield heat overlay texture (this is the neutrons fed to the shader)
                    Texture2D heatTex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Neurons2").Value;
                    Vector2 pos = NPC.Center + NPC.gfxOffY * Vector2.UnitY - Main.screenPosition;
                    Main.spriteBatch.Draw(heatTex, shieldDrawPos, null, Color.White, 0, heatTex.Size() / 2f, shieldScale * scaleMult * 0.5f, 0, 0);
                    Main.spriteBatch.End();
                }
            }
            return false;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return ProvUtils.GetProjectileColor(drawColor) * NPC.Opacity;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/NPCs/Providence/Providence_DeathSilhouette").Value;

            if (Dying)
            {
                if (DeathAnimationTimer > 91f)
                {
                    // Without these four lines there is this weird overlap in the center over darker backgrounds
                    Main.EntitySpriteDraw(tex, NPC.Center + new Vector2(0, 10) - Main.screenPosition, new Rectangle(0, 0, tex.Width, tex.Height), Color.White, 0f, tex.Size() / 2f, 1f, SpriteEffects.None);
                    Main.EntitySpriteDraw(tex, NPC.Center + new Vector2(0, 15) - Main.screenPosition, new Rectangle(0, 0, tex.Width, tex.Height), Color.White, 0f, tex.Size() / 2f, 1f, SpriteEffects.None);
                    Main.EntitySpriteDraw(tex, NPC.Center + new Vector2(0, 20) - Main.screenPosition, new Rectangle(0, 0, tex.Width, tex.Height), Color.White, 0f, tex.Size() / 2f, 1f, SpriteEffects.None);
                    Main.EntitySpriteDraw(tex, NPC.Center + new Vector2(0, 25) - Main.screenPosition, new Rectangle(0, 0, tex.Width, tex.Height), Color.White, 0f, tex.Size() / 2f, 1f, SpriteEffects.None);

                    Vector2 vec = new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, 2));

                    float progress = MathHelper.Clamp(((float)DeathAnimationTimer - 200f) / 300f, 0f, 1f);

                    for (int i = 0; i < NPC.frame.Height / 2; i++)
                    {
                        int outset = ((i - NPC.frame.Height / 4) * 2);

                        int pr = (int)(progress * 30f);

                        Color col = ProvUtils.GetProjectileColor(Color.DarkGray, true);
                        col.A = 255;

                        Main.EntitySpriteDraw(tex, NPC.Center + vec + new Vector2(0, 310) - Main.screenPosition + new Vector2(Main.rand.Next(-pr, pr) * 2, outset), new Rectangle(0, i * 2, tex.Width, 2), Color.Lerp(col, Color.White, progress * 2f).MultiplyRGBA(new Color(255, 255, 255, MathHelper.Lerp(progress, 1f, 0.2f) * 2f)), 0f, tex.Size() / 2f, 1f, SpriteEffects.None);
                    }
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            int totalFrames = 3;
            if (AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon)
            {
                if (!useDefenseFrames)
                {
                    NPC.frameCounter += Dying ? 0.25 : 1D;
                    if (NPC.frameCounter > 10D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }

                    if (NPC.frame.Y >= frameHeight * totalFrames)
                    {
                        NPC.frame.Y = 0;
                        useDefenseFrames = true;
                    }
                }
                else
                {
                    NPC.frameCounter += Dying ? 0.25 : 1D;
                    if (NPC.frameCounter > 10D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }

                    if (NPC.frame.Y >= frameHeight * 2)
                        NPC.frame.Y = frameHeight * 2;
                }
            }
            else
            {
                if (useDefenseFrames)
                    useDefenseFrames = false;

                NPC.frameCounter += Dying ? 0.25 : (NPC.Calamity().newAI[3] < 180f) ? 0.625 : 1D;
                if (NPC.frameCounter > 5D)
                {
                    NPC.frameCounter = 0D;
                    NPC.frame.Y += frameHeight;
                }

                if (NPC.frame.Y >= frameHeight * totalFrames)
                {
                    NPC.frame.Y = 0;
                    frameUsed++;
                }

                int totalSheets = 4;
                if (frameUsed >= totalSheets)
                    frameUsed = 0;
            }
        }

        private static Asset<Texture2D> DiagonalNoise => field ??= ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/HarshNoise");
        private static Asset<Texture2D> UpwardPerlinNoise => field ??= ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Perlin");
        private static Asset<Texture2D> UpwardNoise => field ??= ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/MeltyNoise");

        private static void DrawHolyInferno()
        {
            if (Main.gameMenu || !shouldDrawInfernoBorder)
                return;

            if (CalamityGlobalNPC.holyBoss == -1)
                return;

            var npc = Main.npc[CalamityGlobalNPC.holyBoss];
            var borderDistance = borderRadius;
            if (!npc.active || !npc.HasValidTarget)
                return;

            var target = Main.LocalPlayer;
            var holyInfernoIntensity = target.Calamity().holyInfernoFadeIntensity;
            var prov = npc.ModNPC<Providence>();
            if (prov == null)
                return;

            //Begin drawing the inferno
            var blackTile = TextureAssets.MagicPixel;

            var maxOpacity = 1f;
            if (prov.Dying)
            {
                //Death animation timer ends at 345f.
                maxOpacity = MathHelper.Lerp(1f, 0f, Utils.GetLerpValue(0f, 344f, prov.DeathAnimationTimer));
            }

            var shader = GameShaders.Misc["CalamityMod:HolyInfernoShader"].Shader;
            shader.Parameters["colorMult"].SetValue(prov.hasBeenGivenFullPower ? 7.65f : 7.35f); //I want you to know it took considerable restraint to deliberately misspell colour.
            shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
            shader.Parameters["radius"].SetValue(borderDistance);
            shader.Parameters["anchorPoint"].SetValue(prov.borderPosition ?? npc.Center);
            shader.Parameters["screenPosition"].SetValue(Main.screenPosition);
            shader.Parameters["screenSize"].SetValue(Main.ScreenSize.ToVector2());
            shader.Parameters["burnIntensity"].SetValue(holyInfernoIntensity*0.5f + 0.5f);
            shader.Parameters["playerPosition"].SetValue(target.Center);
            shader.Parameters["maxOpacity"].SetValue(maxOpacity);
            shader.Parameters["day"].SetValue(!prov.hasBeenGivenFullPower);

            Main.spriteBatch.GraphicsDevice.Textures[1] = DiagonalNoise.Value;
            Main.spriteBatch.GraphicsDevice.Textures[2] = UpwardNoise.Value;
            Main.spriteBatch.GraphicsDevice.Textures[3] = UpwardPerlinNoise.Value;

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, shader, Main.Transform);

            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
            Main.spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);

            //Inferno drawing complete
            Main.spriteBatch.End();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 2f;
            return null;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (challenge)
            {
                List<int> exceptionList = new List<int>()
                {
                    ModContent.ProjectileType<MiniGuardianDefense>(),
                    ModContent.ProjectileType<MiniGuardianAttack>(),
                    ModContent.ProjectileType<MiniGuardianRock>(),
                    ModContent.ProjectileType<MiniGuardianSpear>(),
                    ModContent.ProjectileType<SilvaCrystalExplosion>(),
                    ModContent.ProjectileType<GhostlyMine>(),
                    ModContent.ProjectileType<EnergyOrb>(),
                    ModContent.ProjectileType<IrradiatedAura>(),
                    ModContent.ProjectileType<SummonAstralExplosion>(),
                    ModContent.ProjectileType<ApparatusExplosion>(),
                    ModContent.ProjectileType<TarragonAura>()
                };

                bool allowedClass = projectile.CountsAsClass<SummonDamageClass>() || (!projectile.CountsAsClass<MeleeDamageClass>() && !projectile.CountsAsClass<RangedDamageClass>() &&
                    !projectile.CountsAsClass<MagicDamageClass>() && !projectile.CountsAsClass<ThrowingDamageClass>() && !projectile.CountsAsClass<SummonMeleeSpeedDamageClass>());

                bool allowedDamage = allowedClass && hit.Damage <= 75; // Flat 75 regardless of difficulty.

                // Absorber on-hit effects likely won't proc this but Deific Amulet and Astral Bulwark stars will proc this.
                bool allowedBabs = Main.player[projectile.owner].Calamity().pSoulArtifact && !Main.player[projectile.owner].Calamity().profanedCrystalBuffs;

                if ((exceptionList.TrueForAll(x => projectile.type != x) && !allowedDamage) || !allowedBabs)
                {
                    challenge = false;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        PSCChallengeSyncPacket.Send(this);
                    }
                }
            }
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (challenge)
            {
                challenge = false;

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    PSCChallengeSyncPacket.Send(this);
                }
            }
        }

        // This will always put the boss to 1 health before dying, which makes external checks work.
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers) => modifiers.SetMaxDamage(NPC.life - 1);

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0 && !Dying)
            {
                NPC.soundDelay = 8;
                SoundEngine.PlaySound(HurtSound, NPC.Center);
            }

            int dustType = ProvUtils.GetDustID();
            for (int k = 0; k < 15; k++)
            {
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, hit.HitDirection, -1f, 0, default, 1f);
                Main.dust[dust].noGravity = true;
            }

            if (NPC.life <= 0)
            {
                if (!Main.dedServ)
                {
                    float randomSpread = Main.rand.Next(-200, 201) / 100f;
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread * Main.rand.NextFloat(), Mod.Find<ModGore>("Providence").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread * Main.rand.NextFloat(), Mod.Find<ModGore>("Providence2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread * Main.rand.NextFloat(), Mod.Find<ModGore>("Providence3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread * Main.rand.NextFloat(), Mod.Find<ModGore>("Providence4").Type, NPC.scale);
                }

                NPC.position = NPC.Center;
                NPC.width = (int)(400 * NPC.scale);
                NPC.height = (int)(350 * NPC.scale);
                NPC.position -= NPC.Size * 0.5f;

                for (int d = 0; d < 60; d++)
                {
                    int fire = Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 100, default, 2f);
                    Main.dust[fire].velocity *= 3f;
                    Main.dust[fire].noGravity = true;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[fire].scale = 0.5f;
                        Main.dust[fire].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int d = 0; d < 90; d++)
                {
                    int fire = Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 100, default, 3f);
                    Main.dust[fire].noGravity = true;
                    Main.dust[fire].velocity *= 5f;
                    fire = Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 100, default, 2f);
                    Main.dust[fire].velocity *= 2f;
                    Main.dust[fire].noGravity = true;
                }
            }
        }
    }

    /// <summary>
    /// Handles the holy flame effect on items just dropped by Providence in Hell.
    /// </summary>
    public class ProvItemFloating : GlobalItem
    {
        // Leaving this for mod compatibility
        public static readonly List<int> FlameItemTypes = [];

        public override void SetStaticDefaults()
        {
            FlameItemTypes.AddRange([
                // Resources
                ModContent.ItemType<UnholyEssence>(),
                ModContent.ItemType<DivineGeode>(),
                ModContent.ItemType<MarkofProvidence>(),
                ModContent.ItemType<ProvidenceBag>(),

                // Weapons
                ModContent.ItemType<HolyCollider>(),
                ModContent.ItemType<BurningRevelation>(),
                ModContent.ItemType<BlissfulBombardier>(),
                ModContent.ItemType<TelluricGlare>(),
                ModContent.ItemType<PurgeGuzzler>(),
                ModContent.ItemType<DazzlingStabberStaff>(),
                ModContent.ItemType<MoltenAmputator>(),
                ModContent.ItemType<PristineFury>(),

                // Equipment
                ModContent.ItemType<ElysianWings>(),
                ModContent.ItemType<ElysianAegis>(),
                ModContent.ItemType<BlazingCore>(),
                ModContent.ItemType<ProfanedSoulCrystal>(),

                // Vanity
                ModContent.ItemType<ProfanedMoonlightDye>(),
                ModContent.ItemType<ProvidenceMask>(),
                ModContent.ItemType<ThankYouPainting>(),
                ModContent.ItemType<ProvidenceTrophy>(),
                ModContent.ItemType<ProvidenceRelic>(),
                ModContent.ItemType<LoreProvidence>(),

                // GFB
                ModContent.ItemType<AscendantSpiritEssence>(),
                ModContent.ItemType<BlasphemousDonut>()
            ]);
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            if (FlameItemTypes.Contains(entity.type))
            {
                return true;
            }

            return false;
        }

        public override bool InstancePerEntity => true;

        public float HolyFlame = 0f;
        public float FlameTimer = 0f;
        public bool ProviWasEnraged = false;

        public override void OnSpawn(Item item, IEntitySource source)
        {
            if (!BossRushEvent.BossRushActive && source is EntitySource_Loot loot && loot.Entity is NPC npc && npc.ModNPC is Providence provi)
            {
                HolyFlame = 2f;
                ProviWasEnraged = provi.hasBeenGivenFullPower;
            }
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            writer.Write((Half)HolyFlame);
            writer.Write(ProviWasEnraged);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            HolyFlame = (float)reader.ReadHalf();
            ProviWasEnraged = reader.ReadBoolean();
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            float clp = MathHelper.Clamp(HolyFlame, 0f, 1f);

            maxFallSpeed *= MathHelper.Lerp(1, 0f, clp);

            if (maxFallSpeed > 0.3f)
            {
                HolyFlame *= 0.95f;
            }

            HolyFlame *= 0.9975f;
            HolyFlame = MathHelper.Clamp(HolyFlame, 0f, 2f);

            if (item.beingGrabbed)
            {
                HolyFlame = 0f;
            }
        }

        public override void UpdateInventory(Item item, Player player)
        {
            HolyFlame = 0f;
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            FlameTimer++;

            if (HolyFlame > 0f)
            {
                Color lColor = lightColor * MathHelper.Lerp(1f, 0f, MathHelper.Clamp(HolyFlame - 1f, 0f, 1f));
                lColor.A = 255;

                Color alph = new Color(255f * (HolyFlame / 2f), 255f * (HolyFlame / 2f), 0f, 0f);
                Color alph2 = new Color(155f * (HolyFlame / 2f), 0f, 0f, 0f);

                if (ProviWasEnraged)
                {
                    lColor.B = 255;

                    alph = new Color(0f, 255f * (HolyFlame / 2f), 255f * (HolyFlame / 2f), 0f);
                    alph2 = new Color(0f, 0f, 155f * (HolyFlame / 2f), 0f);
                }

                Rectangle frame = Item.GetDrawHitbox(item.type, Main.LocalPlayer);

                if (HolyFlame > 0)
                {
                    for (float i = 0f; i < 360f; i += 90f)
                    {
                        Main.EntitySpriteDraw(TextureAssets.Item[item.type].Value, item.Center - Main.screenPosition + new Vector2(4 * MathHelper.Clamp(HolyFlame, 0f, 1f), 0).RotatedBy(MathHelper.ToRadians(i)), frame, alph, rotation, frame.Size() / 2, scale, SpriteEffects.None);
                    }
                }

                float maxIterations = 20;

                for (float i = 0; i < maxIterations; i++)
                {
                    Main.EntitySpriteDraw(TextureAssets.Item[item.type].Value, item.Center - Main.screenPosition + (new Vector2((float)Math.Sin((FlameTimer / 20) + (item.whoAmI * 13098.125f) - (i / 5)) * i, -i * 1.5f) * (HolyFlame)), frame, Color.Lerp(alph, alph2, i / maxIterations), rotation, frame.Size() / 2, MathHelper.Lerp(scale, 0f, (float)(i / maxIterations) * (HolyFlame / 2)), SpriteEffects.None);
                }

                Main.EntitySpriteDraw(TextureAssets.Item[item.type].Value, item.Center - Main.screenPosition, frame, lColor, rotation, frame.Size() / 2, scale, SpriteEffects.None);

                return false;
            }

            return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }
    }

    // These will be used for almost every single one of her projectiles, so it's useful to have.
    public static class ProvUtils
    {
        /// <summary>
        /// This lease should be used to draw all of a projectile's effects onto directly, then be drawn onto the screen as one. This is for opacity reasons
        /// </summary>
        public static RenderTargetLease PrimaryLease { get => field ??= ScreenspaceTargetPool.Shared.Rent(Main.instance.GraphicsDevice); set; }
        /// <summary>
        /// This should be used for secondary effects like trails, then drawn to PrimaryLease. This is for opacity reasons.
        /// </summary>
        public static RenderTargetLease SecondaryLease { get => field ??= ScreenspaceTargetPool.Shared.Rent(Main.instance.GraphicsDevice); set; }
        public static bool StandardAI() => (CalamityGlobalNPC.holyBoss == -1 || !Main.npc[CalamityGlobalNPC.holyBoss].Calamity().CurrentlyEnraged) && !Main.zenithWorld;

        public static int CalculateProvidenceDamage(this int damage)
        {
            // GFB replaces conventional damage with negative healing
            if (Main.zenithWorld)
                return 0;

            // Enrage
            if (CalamityGlobalNPC.holyBoss != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBoss].Calamity().CurrentlyEnraged)
                    damage *= 2;
            }

            // Offense Guardian
            if (CalamityGlobalNPC.holyBossAttacker != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBossAttacker].active)
                    damage = (int)(damage * 1.25f);
            }
            return damage;
        }

        // Simplified to day/night only. For PSC
        public static Color GetColorBasedOnEnrage(int Alpha, bool Outline = false) => GetColorBasedOnEnrage(!Main.IsItDay() && !Main.remixWorld, Alpha, Outline);
        public static Color GetColorBasedOnEnrage(bool Night, int Alpha, bool Outline = false)
        {
            Color FinalColor = new Color(255, Outline ? 0 : 155, Outline ? 0 : 25, Alpha); // Default to day

            if (Night)
                FinalColor = new Color(100, Outline ? 250 : 200, Outline ? 200 : 250, Alpha);

            return FinalColor;
        }

        public static Color GetProjectileColor(Color givenLightColor, bool Outline = false)
        {
            int alpha = 0;

            // Custom set alpha for GFB Yellow Mode
            if (!Outline && Main.zenithWorld && Main.GlobalTimeWrappedHourly % 6f >= 2f && Main.GlobalTimeWrappedHourly % 6f < 3f)
            {
                float colorBrightness = (givenLightColor.R + givenLightColor.G + givenLightColor.B / 3) / 255f;
                alpha = (int)MathHelper.Lerp(0f, 155f, colorBrightness);
            }
            else
            {
                alpha = 100;
            }

            Color FinalColor = new Color(255, Outline ? 0 : 255, Outline ? 0 : 255, alpha); // Default to normal
            // Color changing should only occur with Providence's projectiles
            if (CalamityGlobalNPC.holyBoss == -1)
                return FinalColor;

            if (Main.zenithWorld)
            {
                if (Main.GlobalTimeWrappedHourly % 6f >= 5f) // Violet
                    FinalColor = new Color(Outline ? 100 : 150, Outline ? 150 : 100, 250, alpha);
                else if (Main.GlobalTimeWrappedHourly % 6f >= 4f) // Blue
                    FinalColor = new Color(100, Outline ? 250 : 200, Outline ? 200 : 250, alpha);
                else if (Main.GlobalTimeWrappedHourly % 6f >= 3f) // Green
                    FinalColor = new Color(Outline ? 200 : 100, 250, 100, alpha);
                else if (Main.GlobalTimeWrappedHourly % 6f >= 2f) // Yellow
                    FinalColor = new Color(255, Outline ? 0 : 255, Outline ? 0 : 255, alpha);
                else if (Main.GlobalTimeWrappedHourly % 6f >= 1f) // Orange
                    FinalColor = new Color(250, 150, Outline ? 150 : 100, alpha);
                else // Red
                    FinalColor = new Color(250, 100, Outline ? 200 : 100, alpha);
            }
            else if (!StandardAI())
                FinalColor = new Color(100, Outline ? 250 : 200, Outline ? 200 : 250, alpha);

            if (Outline)
                FinalColor *= 0.1f;

            return FinalColor;
        }

        public static Color GetProjectileColor(int Alpha, bool Outline = false)
        {
            Color FinalColor = new Color(255, Outline ? 0 : 155, Outline ? 0 : 25, Alpha); // Default to normal
            // Color changing should only occur with Providence's projectiles
            if (CalamityGlobalNPC.holyBoss == -1)
                return FinalColor;

            if (Main.zenithWorld)
            {
                if (Main.GlobalTimeWrappedHourly % 6f >= 5f) // Violet
                    FinalColor = new Color(Outline ? 100 : 150, Outline ? 150 : 100, 250, Alpha);
                else if (Main.GlobalTimeWrappedHourly % 6f >= 4f) // Blue
                    FinalColor = new Color(100, Outline ? 250 : 200, Outline ? 200 : 250, Alpha);
                else if (Main.GlobalTimeWrappedHourly % 6f >= 3f) // Green
                    FinalColor = new Color(Outline ? 200 : 100, 250, 100, Alpha);
                else if (Main.GlobalTimeWrappedHourly % 6f >= 2f) // Yellow
                    FinalColor = new Color(255, Outline ? 0 : 155, Outline ? 0 : 25, Alpha);
                else if (Main.GlobalTimeWrappedHourly % 6f >= 1f) // Orange
                    FinalColor = new Color(250, 150, Outline ? 150 : 100, Alpha);
                else // Red
                    FinalColor = new Color(250, 100, Outline ? 200 : 100, Alpha);
            }
            else if (!StandardAI())
                FinalColor = new Color(100, Outline ? 250 : 200, Outline ? 200 : 250, Alpha);

            if (Outline)
                FinalColor *= 0.1f;

            return FinalColor;
        }

        // Assign the night bool to turn it into a binary day/night state without accounting for GFB, used for PSC
        public static int GetDustID(bool? Night = null)
        {
            int DustType = (int)CalamityDusts.ProfanedFire; // Default to normal

            if (Night.HasValue)
            {
                if (Night.Value)
                    DustType = (int)CalamityDusts.Nightwither;
            }
            else if (Main.zenithWorld)
            {
                if (Main.GlobalTimeWrappedHourly % 6f >= 5f) // Violet
                    DustType = DustID.PurpleTorch;
                else if (Main.GlobalTimeWrappedHourly % 6f >= 4f) // Blue
                    DustType = (int)CalamityDusts.Nightwither;
                else if (Main.GlobalTimeWrappedHourly % 6f >= 3f) // Green
                    DustType = DustID.GreenTorch;
                else if (Main.GlobalTimeWrappedHourly % 6f >= 2f) // Yellow
                    DustType = (int)CalamityDusts.ProfanedFire;
                else if (Main.GlobalTimeWrappedHourly % 6f >= 1f) // Orange
                    DustType = DustID.OrangeTorch;
                else // Red
                    DustType = DustID.RedTorch;
            }
            else if (!StandardAI())
                DustType = (int)CalamityDusts.Nightwither;

            return DustType;
        }

        // Shortcut function for applying Burden Breaker-like negative healing to GFB Providence
        public static void ApplyGFBDamage(Projectile proj, int BaseDuration, int NegativeHealValue)
        {
            if (!Main.zenithWorld)
                return;

            int index = Player.FindClosest(proj.position, proj.width, proj.height);
            Player player = Main.player[index];
            if (player is null)
                return;
            if (proj.Colliding(proj.Hitbox, player.Hitbox))
            {
                ApplyDebuffs(player, BaseDuration, NegativeHealValue);

                if (proj.type == ModContent.ProjectileType<HolyBurnOrb>())
                    proj.Kill();
            }
        }

        // Include debuffs inflicted by Providence's projectiles for all her forms
        // In the GFB seed, also includes negative healing
        public static void ApplyDebuffs(Player Target, int BaseDuration, int NegativeHealValue = 0)
        {
            int BuffType = ModContent.BuffType<HolyFlames>(); // Default to non-GFB

            // All debuffs are adjusted to be 50% more powerful in GFB
            float Multiplier = 1f;
            if (Main.zenithWorld)
            {
                if (Main.GlobalTimeWrappedHourly % 6f >= 5f) // Violet
                {
                    BuffType = ModContent.BuffType<Shadowflame>();
                    Multiplier = 2f;
                }
                else if (Main.GlobalTimeWrappedHourly % 6f >= 4f) // Blue
                {
                    BuffType = ModContent.BuffType<Nightwither>();
                    Multiplier = 1.5f;
                }
                else if (Main.GlobalTimeWrappedHourly % 6f >= 3f) // Green
                {
                    BuffType = BuffID.CursedInferno;
                    Multiplier = 2.5f;
                }
                else if (Main.GlobalTimeWrappedHourly % 6f >= 2f) // Yellow
                {
                    BuffType = ModContent.BuffType<HolyFlames>();
                    Multiplier = 1.5f;
                }
                else if (Main.GlobalTimeWrappedHourly % 6f >= 1f) // Orange
                    BuffType = ModContent.BuffType<Dragonfire>();
                else // Red
                {
                    BuffType = ModContent.BuffType<BrimstoneFlames>();
                    Multiplier = 2f;
                }
            }

            Target.AddBuff(BuffType, (int)(BaseDuration * Multiplier));

            // A. Specifically inflicts Vaporfied in quirky RGB Mode because it's a colorful debuff
            // B. Apply the negative healing
            if (Main.zenithWorld)
            {
                // Obligatory offensive guardian boosting negative heals
                if (CalamityGlobalNPC.holyBossAttacker != -1)
                {
                    if (Main.npc[CalamityGlobalNPC.holyBossAttacker].active)
                        NegativeHealValue *= 2;
                }

                Target.HealEffect(-1 * NegativeHealValue, false);
                Target.statLife -= NegativeHealValue;
                if (Target.statLife < 0)
                {
                    PlayerDeathReason CustomSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.ProvidenceAntiHealing").ToNetworkText(Target.name));
                    Target.KillMe(CustomSource, NegativeHealValue, 0);
                }

                NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, Target.whoAmI, NegativeHealValue);

                Target.AddBuff(ModContent.BuffType<Vaporfied>(), (int)(BaseDuration * Multiplier));
            }
        }
    }
}
