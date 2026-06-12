using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Sounds;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Bumblebirb
{
    [AutoloadBossHead]
    public class Dragonfolly : ModNPC
    {
        public static Asset<Texture2D> GlowTexture;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.5f,
                PortraitScale = 0.85f,
                PortraitPositionYOverride = 14f
            };
            value.Position.X += 20f;
            value.Position.Y += 8f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Bumblebirb/BirbGlow", AssetRequestMode.AsyncLoad);
            }
        }

        public override string Texture => "CalamityMod/NPCs/Bumblebirb/Birb";
        public override string BossHeadTexture => "CalamityMod/NPCs/Bumblebirb/Birb_Head_Boss";

        public static float DashDamageMult = 1.5f; // 240
        public static int FeatherDamage = 36; // 144
        public static int LightningDamage = 64; // 256

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 80; // 160
            NPC.npcSlots = 32f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.width = 130;
            NPC.height = 100;
            NPC.defense = 40;
            NPC.DR_NERD(0.1f);
            NPC.LifeMaxNERB(150000, 225000, 300000); // Old HP - 227500, 252500
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(gold: 50);
            NPC.HitSound = SoundID.NPCHit51;
            NPC.DeathSound = SoundID.NPCDeath46;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Bumblefuck")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            // Variables
            float rotationMult = 3f;
            float rotationAmt = 0.03f;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // If target is outside the jungle for more than 5 seconds, enrage
            if (!player.ZoneJungle && !BossRushEvent.BossRushActive)
            {
                if (NPC.localAI[1] < CalamityGlobalNPC.biomeEnrageTimerMax)
                    NPC.localAI[1] += 1f;
            }
            else
                NPC.localAI[1] = 0f;

            // If dragonfolly is off screen, enrage for the next couple attacks
            if (Vector2.Distance(player.Center, NPC.Center) > 1200f)
                NPC.localAI[2] = 2f;

            // Enrage scale
            float enrageScale = death ? 1.5f : 1f;
            if (NPC.localAI[1] >= CalamityGlobalNPC.biomeEnrageTimerMax)
            {
                NPC.Calamity().CurrentlyEnraged = true;
                enrageScale += 1f;
            }

            if (NPC.localAI[2] > 0f)
                enrageScale += 1f;

            if (Main.getGoodWorld)
                enrageScale += 0.5f;

            if (enrageScale > 3f)
                enrageScale = 3f;

            // Despawn
            if (!player.active || player.dead || Vector2.Distance(player.Center, NPC.Center) > 5600f)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];

                if (!player.active || player.dead || Vector2.Distance(player.Center, NPC.Center) > 5600f)
                {
                    NPC.rotation = (NPC.rotation * rotationMult + NPC.velocity.X * rotationAmt) / 10f;

                    if (NPC.velocity.Y > 3f)
                        NPC.velocity.Y = 3f;
                    NPC.velocity.Y -= 0.2f;
                    if (NPC.velocity.Y < -12f)
                        NPC.velocity.Y = -12f;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    if (NPC.ai[0] != 0f)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;

                        NPC.ForceNetUpdate(false);
                    }

                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < (revenge ? 0.75f : 0.5f);
            bool phase3 = lifeRatio < (death ? 0.4f : revenge ? 0.25f : 0.1f) && expertMode;

            float birbSpawnPhaseTimer = 180f;
            float newPhaseTimer = 180f;
            bool phaseSwitchPhase = (phase2 && calamityGlobalNPC.newAI[0] < newPhaseTimer && calamityGlobalNPC.newAI[2] != 1f) ||
                (phase3 && calamityGlobalNPC.newAI[1] < newPhaseTimer && calamityGlobalNPC.newAI[3] != 1f);

            calamityGlobalNPC.DR = phaseSwitchPhase || NPC.ai[0] == 5f || enrageScale == 3f ? 0.55f : 0.1f;
            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = phaseSwitchPhase || NPC.ai[0] == 5f || enrageScale == 3f;

            if (phaseSwitchPhase)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                if (NPC.velocity.X < 0f)
                    NPC.direction = -1;
                else if (NPC.velocity.X > 0f)
                    NPC.direction = 1;

                NPC.spriteDirection = NPC.direction;
                NPC.rotation = (NPC.rotation * rotationMult + NPC.velocity.X * rotationAmt) / 10f;

                if (phase3)
                {
                    calamityGlobalNPC.newAI[1] += 1f;

                    // Sound
                    if (calamityGlobalNPC.newAI[1] == newPhaseTimer - 60f)
                    {
                        float squawkpitch = Main.zenithWorld ? 1.3f : 0.25f;
                        SoundEngine.PlaySound(SoundID.DD2_BetsyScream with { Pitch = squawkpitch }, NPC.Center);

                        if (Main.zenithWorld)
                        {
                            int spacing = 20;
                            int amt = 5;
                            SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, NPC.Center - Vector2.UnitY * 300f);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < amt; i++)
                                {
                                    Vector2 fireFrom = new Vector2(NPC.Center.X + (spacing * i) - (spacing * amt / 2), NPC.Center.Y - 900f);
                                    Vector2 ai0 = NPC.Center - fireFrom;
                                    float ai = Main.rand.Next(100);
                                    Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom.X, fireFrom.Y, velocity.X, velocity.Y, ModContent.ProjectileType<RedLightning>(), LightningDamage, 0f, Main.myPlayer, ai0.ToRotation(), ai);
                                }
                            }
                        }
                    }

                    if (calamityGlobalNPC.newAI[1] >= newPhaseTimer)
                    {
                        calamityGlobalNPC.newAI[1] = 0f;
                        calamityGlobalNPC.newAI[2] = 1f;
                        calamityGlobalNPC.newAI[3] = 1f;
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.SyncExtraAI();
                        NPC.ForceNetUpdate(false);
                    }
                }
                else
                {
                    calamityGlobalNPC.newAI[0] += 1f;

                    // Sound
                    if (calamityGlobalNPC.newAI[0] == newPhaseTimer - 60f)
                    {
                        float squawkpitch = Main.zenithWorld ? 1.3f : 0.25f;
                        SoundEngine.PlaySound(SoundID.DD2_BetsyScream with { Pitch = squawkpitch }, NPC.Center);

                        if (Main.zenithWorld)
                        {
                            int spacing = 20;
                            int amt = 3;
                            SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, NPC.Center - Vector2.UnitY * 300f);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < amt; i++)
                                {
                                    Vector2 fireFrom = new Vector2(NPC.Center.X + (spacing * i) - (spacing * amt / 2), NPC.Center.Y - 900f);
                                    Vector2 ai0 = NPC.Center - fireFrom;
                                    float ai = Main.rand.Next(100);
                                    Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom.X, fireFrom.Y, velocity.X, velocity.Y, ModContent.ProjectileType<RedLightning>(), LightningDamage, 0f, Main.myPlayer, ai0.ToRotation(), ai);
                                }
                            }
                        }
                    }

                    if (calamityGlobalNPC.newAI[0] >= newPhaseTimer)
                    {
                        calamityGlobalNPC.newAI[0] = 0f;
                        calamityGlobalNPC.newAI[2] = 1f;
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.SyncExtraAI();
                        NPC.ForceNetUpdate(false);
                    }
                }

                Vector2 follyTargetDirection = player.Center - NPC.Center;
                float follyTargetDistance = 4f + follyTargetDirection.Length() / 100f;
                float follyVelocityMult = 25f;
                follyTargetDirection.Normalize();
                follyTargetDirection *= follyTargetDistance;
                NPC.velocity = (NPC.velocity * (follyVelocityMult - 1f) + follyTargetDirection) / follyVelocityMult;
                return;
            }

            // Max spawn amount
            int maxBirbs = Main.zenithWorld ? 12 : revenge ? 3 : 2;

            // Variable for charging
            float chargeDistance = 600f;
            if (phase2)
                chargeDistance -= 50f;
            if (phase3)
                chargeDistance -= 50f;
            chargeDistance -= (enrageScale - 1f) * 100f;

            // Phase switch
            if (NPC.ai[0] == 0f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                if (NPC.Center.X < player.Center.X - 2f)
                    NPC.direction = 1;
                if (NPC.Center.X > player.Center.X + 2f)
                    NPC.direction = -1;

                // Direction and rotation
                NPC.spriteDirection = NPC.direction;
                NPC.rotation = (NPC.rotation * rotationMult + NPC.velocity.X * rotationAmt * 1.25f) / 10f;

                // Fly to target if target is too far away, otherwise get close to target and then slow down
                Vector2 follyFlyTargetDirection = player.Center - NPC.Center;
                follyFlyTargetDirection.Y -= 200f;
                if (follyFlyTargetDirection.Length() > 2800f)
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                }
                else if (follyFlyTargetDirection.Length() > 240f)
                {
                    float follyFlySpeed = 12f + (enrageScale - 1f) * 6f;
                    float follyFlyVelocityMult = 30f;
                    follyFlyTargetDirection.Normalize();
                    follyFlyTargetDirection *= follyFlySpeed;
                    NPC.velocity = (NPC.velocity * (follyFlyVelocityMult - 1f) + follyFlyTargetDirection) / follyFlyVelocityMult;
                }
                else if (NPC.velocity.Length() > 2f)
                    NPC.velocity *= 0.95f;
                else if (NPC.velocity.Length() < 1f)
                    NPC.velocity *= 1.05f;

                // Phase switch
                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= 30f)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;

                    while (NPC.ai[0] == 0f)
                    {
                        if (phase2)
                            NPC.localAI[0] += 1f;

                        if (NPC.localAI[0] >= (phase3 ? 7 : 9))
                        {
                            NPC.TargetClosest();
                            NPC.ai[0] = 5f;
                            NPC.localAI[0] = 0f;

                            // Decrease the feather variable, feathers can be used again if it's at 0
                            if (NPC.ai[3] > 0f)
                                NPC.ai[3] -= 1f;

                            // Decrease enraged attacks by 1
                            if (NPC.localAI[2] > 0f)
                                NPC.localAI[2] -= 1f;

                            // Decrease amount of attacks until able to spawn small birbs again
                            if (NPC.localAI[3] > 0f)
                                NPC.localAI[3] -= 1f;
                        }
                        else
                        {
                            int follyAttackPicker = phase2 ? Main.rand.Next(2) + 1 : Main.rand.Next(3);
                            if (phase3)
                                follyAttackPicker = 1;

                            float featherVelocity = 2f + (enrageScale - 1f);
                            int type = ModContent.ProjectileType<RedLightningFeather>();

                            if (follyAttackPicker == 0 && NPC.localAI[3] == 0f)
                            {
                                NPC.TargetClosest();
                                NPC.ai[0] = 2f;

                                // Decrease the feather variable, feathers can be used again if it's at 0
                                if (NPC.ai[3] > 0f)
                                    NPC.ai[3] -= 1f;

                                // Decrease enraged attacks by 1
                                if (NPC.localAI[2] > 0f)
                                    NPC.localAI[2] -= 1f;

                                // Birb will do at least 1 different attack before entering this phase again
                                NPC.localAI[3] = 1f;
                            }
                            else if (follyAttackPicker == 1)
                            {
                                NPC.TargetClosest();
                                NPC.ai[0] = 3f;

                                // Decrease enraged attacks by 1
                                if (NPC.localAI[2] > 0f)
                                    NPC.localAI[2] -= 1f;

                                // Decrease amount of attacks until able to use other attacks again
                                if (NPC.localAI[3] > 0f)
                                    NPC.localAI[3] -= 1f;

                                if (phase2 && NPC.ai[3] == 0f)
                                {
                                    NPC.ai[3] = 3f;
                                    SoundEngine.PlaySound(SoundID.Item102, player.Center);

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int totalProjectiles = 40;
                                        float radians = MathHelper.TwoPi / totalProjectiles;
                                        int distance = 800;
                                        bool spawnRight = player.velocity.X > 0f;
                                        for (int i = 0; i < totalProjectiles; i++)
                                        {
                                            if (Main.getGoodWorld)
                                            {
                                                if (i >= (int)(totalProjectiles * 0.125) && i <= (int)(totalProjectiles * 0.375))
                                                {
                                                    Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -featherVelocity).RotatedBy(radians * i)) * distance;
                                                    Vector2 velocity = Vector2.Normalize(player.Center - spawnVector) * featherVelocity;
                                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnVector, velocity, type, FeatherDamage, 0f, Main.myPlayer);
                                                }
                                                if (i >= (int)(totalProjectiles * 0.625) && i <= (int)(totalProjectiles * 0.875))
                                                {
                                                    Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -featherVelocity).RotatedBy(radians * i)) * distance;
                                                    Vector2 velocity = Vector2.Normalize(player.Center - spawnVector) * featherVelocity;
                                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnVector, velocity, type, FeatherDamage, 0f, Main.myPlayer);
                                                }
                                            }
                                            else
                                            {
                                                if (spawnRight)
                                                {
                                                    if (i >= (int)(totalProjectiles * 0.125) && i <= (int)(totalProjectiles * 0.375))
                                                    {
                                                        Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -featherVelocity).RotatedBy(radians * i)) * distance;
                                                        Vector2 velocity = Vector2.Normalize(player.Center - spawnVector) * featherVelocity;
                                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnVector, velocity, type, FeatherDamage, 0f, Main.myPlayer);
                                                    }
                                                }
                                                else
                                                {
                                                    if (i >= (int)(totalProjectiles * 0.625) && i <= (int)(totalProjectiles * 0.875))
                                                    {
                                                        Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -featherVelocity).RotatedBy(radians * i)) * distance;
                                                        Vector2 velocity = Vector2.Normalize(player.Center - spawnVector) * featherVelocity;
                                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnVector, velocity, type, FeatherDamage, 0f, Main.myPlayer);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // Decrease the feather variable, feathers can be used again if it's at 0
                                    if (NPC.ai[3] > 0f)
                                        NPC.ai[3] -= 1f;
                                }
                            }
                            else if (NPC.CountNPCS(ModContent.NPCType<DraconicSwarmer>()) < maxBirbs && NPC.localAI[3] == 0f)
                            {
                                NPC.TargetClosest();
                                NPC.ai[0] = 4f;

                                // Birb will do at least 2 different attacks before entering this phase again
                                NPC.localAI[3] = 2f;

                                // Decrease enraged attacks by 1
                                if (NPC.localAI[2] > 0f)
                                    NPC.localAI[2] -= 1f;

                                if (NPC.ai[3] == 0f)
                                {
                                    NPC.ai[3] = 3f;
                                    SoundEngine.PlaySound(SoundID.Item102, player.Center);

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int totalProjectiles = phase2 ? 40 : 48;

                                        if (Main.getGoodWorld)
                                            totalProjectiles *= 2;

                                        float radians = MathHelper.TwoPi / totalProjectiles;
                                        int distance = phase2 ? 1200 : 1320;

                                        if (Main.getGoodWorld)
                                            distance *= 2;

                                        bool spawnRight = player.velocity.X > 0f;
                                        for (int i = 0; i < totalProjectiles; i++)
                                        {
                                            if (spawnRight)
                                            {
                                                if (i >= totalProjectiles / 2)
                                                    break;

                                                Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -featherVelocity).RotatedBy(radians * i)) * distance;
                                                Vector2 velocity = Vector2.Normalize(player.Center - spawnVector) * featherVelocity;
                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnVector, velocity, type, FeatherDamage, 0f, Main.myPlayer);
                                            }
                                            else
                                            {
                                                if (i >= totalProjectiles / 2)
                                                {
                                                    Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -featherVelocity).RotatedBy(radians * i)) * distance;
                                                    Vector2 velocity = Vector2.Normalize(player.Center - spawnVector) * featherVelocity;
                                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnVector, velocity, type, FeatherDamage, 0f, Main.myPlayer);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // Decrease the feather variable, feathers can be used again if it's at 0
                                    if (NPC.ai[3] > 0f)
                                        NPC.ai[3] -= 1f;
                                }
                            }
                        }
                    }

                    NPC.ForceNetUpdate(false);
                    NPC.SyncExtraAI();
                }
            }

            // Fly to target
            else if (NPC.ai[0] == 1f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                if (NPC.velocity.X < 0f)
                    NPC.direction = -1;
                else if (NPC.velocity.X > 0f)
                    NPC.direction = 1;

                NPC.spriteDirection = NPC.direction;
                NPC.rotation = (NPC.rotation * rotationMult + NPC.velocity.X * rotationAmt) / 10f;

                Vector2 follyTargetDirection = player.Center - NPC.Center;
                if (follyTargetDirection.Length() < 800f)
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;

                    NPC.ForceNetUpdate(false);
                    NPC.SyncExtraAI();
                }

                float velocity = 14f + (enrageScale - 1f) * 4f;
                float follyTargetDistance = velocity + follyTargetDirection.Length() / 100f;
                float follyVelocityMult = 25f;
                follyTargetDirection.Normalize();
                follyTargetDirection *= follyTargetDistance;
                NPC.velocity = (NPC.velocity * (follyVelocityMult - 1f) + follyTargetDirection) / follyVelocityMult;
            }

            // Fly towards target quickly
            else if (NPC.ai[0] == 2f)
            {
                NPC.damage = NPC.defDamage;

                if (NPC.target < 0 || !player.active || player.dead)
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;

                    NPC.ForceNetUpdate(false);
                    NPC.SyncExtraAI();
                }

                if (player.Center.X - 10f < NPC.Center.X)
                    NPC.direction = -1;
                else if (player.Center.X + 10f > NPC.Center.X)
                    NPC.direction = 1;

                NPC.spriteDirection = NPC.direction;
                NPC.rotation = (NPC.rotation * rotationMult * 0.5f + NPC.velocity.X * rotationAmt * 1.25f) / 5f;

                Vector2 follyQuickFlyTargetDirection = player.Center - NPC.Center;
                follyQuickFlyTargetDirection.Y -= 20f;
                NPC.ai[2] += 0.0222222228f;
                if (expertMode)
                    NPC.ai[2] += 0.0166666675f;

                float velocity = 8f + (enrageScale - 1f) * 2f;
                float follyQuickFlySpeed = velocity + NPC.ai[2] + follyQuickFlyTargetDirection.Length() / 120f;
                if (Main.getGoodWorld)
                    follyQuickFlySpeed *= 2f;

                float follyQuickFlyVelMult = 20f;
                follyQuickFlyTargetDirection.Normalize();
                follyQuickFlyTargetDirection *= follyQuickFlySpeed;
                NPC.velocity = (NPC.velocity * (follyQuickFlyVelMult - 1f) + follyQuickFlyTargetDirection) / follyQuickFlyVelMult;

                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= (Main.getGoodWorld ? 90f : 180f))
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;

                    NPC.ForceNetUpdate(false);
                    NPC.SyncExtraAI();
                }
            }

            // Line up charge
            else if (NPC.ai[0] == 3f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                if (NPC.velocity.X < 0f)
                    NPC.direction = -1;
                else
                    NPC.direction = 1;

                NPC.spriteDirection = NPC.direction;
                NPC.rotation = (NPC.rotation * rotationMult * 0.5f + NPC.velocity.X * rotationAmt * 0.85f) / 5f;

                Vector2 follyLineUpTargetDirection = player.Center - NPC.Center;
                follyLineUpTargetDirection.Y -= 12f;
                if (NPC.Center.X > player.Center.X)
                    follyLineUpTargetDirection.X += chargeDistance;
                else
                    follyLineUpTargetDirection.X -= chargeDistance;

                float verticalDistanceGateValue = (phase3 ? 100f : 20f) + (enrageScale - 1f) * 20f;
                if (Math.Abs(NPC.Center.X - player.Center.X) > chargeDistance - 50f && Math.Abs(NPC.Center.Y - player.Center.Y) < verticalDistanceGateValue)
                {
                    NPC.ai[0] = 3.1f;
                    NPC.ai[1] = 0f;

                    NPC.ForceNetUpdate(false);
                    NPC.SyncExtraAI();
                }

                NPC.ai[1] += 0.0333333351f;
                float velocity = 16f + (enrageScale - 1f) * 4f;
                float follyLineUpSpeed = velocity + NPC.ai[1];
                float follyLineUpVelMult = 4f;
                follyLineUpTargetDirection.Normalize();
                follyLineUpTargetDirection *= follyLineUpSpeed;
                NPC.velocity = (NPC.velocity * (follyLineUpVelMult - 1f) + follyLineUpTargetDirection) / follyLineUpVelMult;
            }

            // Prepare to charge
            else if (NPC.ai[0] == 3.1f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.rotation = (NPC.rotation * rotationMult * 0.5f + NPC.velocity.X * rotationAmt * 0.85f) / 5f;

                Vector2 follyChargePrepareTargetDirection = player.Center - NPC.Center;
                follyChargePrepareTargetDirection.Y -= 12f;
                float follyChargePrepareSpeed = 28f + (enrageScale - 1f) * 4f;
                float follyChargePrepareVelMult = 8f;
                follyChargePrepareTargetDirection.Normalize();
                follyChargePrepareTargetDirection *= follyChargePrepareSpeed;
                NPC.velocity = (NPC.velocity * (follyChargePrepareVelMult - 1f) + follyChargePrepareTargetDirection) / follyChargePrepareVelMult;

                if (NPC.velocity.X < 0f)
                    NPC.direction = -1;
                else
                    NPC.direction = 1;

                NPC.spriteDirection = NPC.direction;

                NPC.ai[1] += 1f;
                if (NPC.ai[1] > 10f)
                {
                    NPC.damage = (int)Math.Round(NPC.defDamage * DashDamageMult);

                    NPC.velocity = follyChargePrepareTargetDirection;

                    if (NPC.velocity.X < 0f)
                        NPC.direction = -1;
                    else
                        NPC.direction = 1;

                    NPC.ai[0] = 3.2f;
                    NPC.ai[1] = NPC.direction;

                    NPC.ForceNetUpdate(false);
                    NPC.SyncExtraAI();
                }
            }

            // Charge
            else if (NPC.ai[0] == 3.2f)
            {
                NPC.damage = (int)Math.Round(NPC.defDamage * DashDamageMult);

                NPC.ai[2] += 0.0333333351f;
                float velocity = 28f + (enrageScale - 1f) * 4f;
                NPC.velocity.X = (velocity + NPC.ai[2]) * NPC.ai[1];

                if ((NPC.ai[1] > 0f && NPC.Center.X > player.Center.X + (chargeDistance - 140f)) || (NPC.ai[1] < 0f && NPC.Center.X < player.Center.X - (chargeDistance - 140f)))
                {
                    if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        NPC.TargetClosest();
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ForceNetUpdate(false);
                    }
                    else if (Math.Abs(NPC.Center.X - player.Center.X) > chargeDistance + 200f)
                    {
                        NPC.TargetClosest();
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ForceNetUpdate(false);
                    }
                }

                NPC.rotation = (NPC.rotation * rotationMult * 0.5f + NPC.velocity.X * rotationAmt * 0.85f) / 5f;
            }

            // Birb spawn
            else if (NPC.ai[0] == 4f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                if (NPC.ai[1] == 0f)
                {
                    Vector2 destination2 = player.Center + new Vector2(0f, -200f);
                    Vector2 desiredVelocity2 = NPC.SafeDirectionTo(destination2, -Vector2.UnitY) * 18f;
                    NPC.SimpleFlyMovement(desiredVelocity2, 1.5f);

                    if (NPC.velocity.X < 0f)
                        NPC.direction = -1;
                    else
                        NPC.direction = 1;

                    NPC.spriteDirection = NPC.direction;

                    NPC.ai[2] += 1f;
                    if (NPC.Distance(player.Center) < 600f || NPC.ai[2] >= 180f)
                    {
                        NPC.ai[1] = 1f;
                        NPC.ForceNetUpdate(false);
                    }
                }
                else
                {
                    if (NPC.ai[1] < 90f)
                        NPC.velocity *= 0.95f;
                    else
                        NPC.velocity *= 0.98f;

                    if (NPC.ai[1] == 90f)
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y /= 3f;

                        NPC.velocity.Y -= 3f;
                    }

                    // Sound
                    if (NPC.ai[1] == birbSpawnPhaseTimer - 60f)
                    {
                        float squawkpitch = Main.zenithWorld ? 1.3f : 0.25f;
                        SoundEngine.PlaySound(SoundID.DD2_BetsyScream with { Pitch = squawkpitch }, NPC.Center);

                        if (Main.zenithWorld)
                        {
                            int spacing = 30;
                            int amt = 3;
                            SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, NPC.Center - Vector2.UnitY * 300f);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < amt; i++)
                                {
                                    Vector2 fireFrom = new Vector2(NPC.Center.X + (spacing * i) - (spacing * amt / 2), NPC.Center.Y - 900f);
                                    Vector2 ai0 = NPC.Center - fireFrom;
                                    float ai = Main.rand.Next(100);
                                    Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom.X, fireFrom.Y, velocity.X, velocity.Y, ModContent.ProjectileType<RedLightning>(), LightningDamage, 0f, Main.myPlayer, ai0.ToRotation(), ai);
                                }
                            }
                        }
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        bool gfbSpawnFlag = Main.zenithWorld && (NPC.ai[1] == 145f || NPC.ai[1] == 150f || NPC.ai[1] == 160f || NPC.ai[1] == 165f);
                        bool spawnFlag = NPC.CountNPCS(ModContent.NPCType<DraconicSwarmer>()) < maxBirbs && (NPC.ai[1] == 140f || (revenge && NPC.ai[1] == 155f) || NPC.ai[1] == 170f || gfbSpawnFlag);
                        if (spawnFlag)
                        {
                            Vector2 follySpawnCenter = NPC.Center + (MathHelper.TwoPi * Main.rand.NextFloat()).ToRotationVector2() * new Vector2(2f, 1f) * 50f * (0.6f + Main.rand.NextFloat() * 0.4f);
                            if (Vector2.Distance(follySpawnCenter, player.Center) > 150f)
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)follySpawnCenter.X, (int)follySpawnCenter.Y, ModContent.NPCType<DraconicSwarmer>(), NPC.whoAmI);

                            NPC.ForceNetUpdate(false);
                        }
                    }

                    NPC.ai[1] += 1f;
                }

                if (NPC.ai[1] >= birbSpawnPhaseTimer)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.TargetClosest();
                    NPC.ForceNetUpdate(false);
                }
            }

            // Spit homing aura sphere
            else if (NPC.ai[0] == 5f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                // Velocity
                NPC.velocity *= 0.98f;
                NPC.rotation = (NPC.rotation * rotationMult + NPC.velocity.X * rotationAmt) / 10f;

                // Play sound
                float aiGateValue = 120f;
                if (NPC.ai[1] == aiGateValue - 30f)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 follySpawnCenter = NPC.rotation.ToRotationVector2() * (Vector2.UnitX * NPC.direction) * (NPC.width + 20) / 2f + NPC.Center;
                        float ai0 = (phase3 ? 2f : 0f) + (enrageScale - 1f);
                        if (ai0 > 3f)
                            ai0 = 3f;

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), follySpawnCenter.X, follySpawnCenter.Y, 0f, 0f, ModContent.ProjectileType<BirbAuraFlare>(), 0, 0f, Main.myPlayer, ai0, NPC.target + 1);
                        NPC.ForceNetUpdate(false);
                    }

                    if (Main.zenithWorld)
                    {
                        int spacing = 30;
                        int amt = 3;
                        SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, NPC.Center - Vector2.UnitY * 300f);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < amt; i++)
                            {
                                Vector2 fireFrom = new Vector2(NPC.Center.X + (spacing * i) - (spacing * amt / 2), NPC.Center.Y - 900f);
                                Vector2 ai0 = NPC.Center - fireFrom;
                                float ai = Main.rand.Next(100);
                                Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom.X, fireFrom.Y, velocity.X, velocity.Y, ModContent.ProjectileType<RedLightning>(), LightningDamage, 0f, Main.myPlayer, ai0.ToRotation(), ai);
                            }
                        }
                    }
                }

                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= aiGateValue)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ForceNetUpdate(false);
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < (revenge ? 0.75f : 0.5f) || death;
            bool phase3 = lifeRatio < (death ? 0.4f : revenge ? 0.25f : 0.1f);
            bool birbSpawn = NPC.ai[0] == 4f && NPC.ai[1] > 0f;

            // Animation goes nyoom
            if (Main.zenithWorld)
            {
                NPC.frameCounter += 4;
            }

            float newPhaseTimer = 180f;
            bool phaseSwitchPhase = (phase2 && calamityGlobalNPC.newAI[0] < newPhaseTimer && calamityGlobalNPC.newAI[2] != 1f) ||
                (phase3 && calamityGlobalNPC.newAI[1] < newPhaseTimer && calamityGlobalNPC.newAI[3] != 1f);

            if (phaseSwitchPhase || birbSpawn)
            {
                float frameGateValue = birbSpawn ? NPC.ai[1] : phase3 ? calamityGlobalNPC.newAI[1] : calamityGlobalNPC.newAI[0];
                int frameTimer = 180;
                if (frameGateValue < (frameTimer - 60) || frameGateValue > (frameTimer - 20))
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 5D)
                    {
                        NPC.frameCounter = 0D;
                        NPC.frame.Y += frameHeight;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                    {
                        NPC.frame.Y = 0;
                    }
                }
                else
                {
                    NPC.frame.Y = frameHeight * 4;
                    if (frameGateValue > (frameTimer - 50) && frameGateValue < (frameTimer - 25))
                    {
                        NPC.frame.Y = frameHeight * 5;
                    }
                }
            }
            else if (NPC.ai[0] == 5f)
            {
                int otherFrameTimer = 120;
                if (NPC.ai[1] < (otherFrameTimer - 50) || NPC.ai[1] > (otherFrameTimer - 10))
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 5D)
                    {
                        NPC.frameCounter = 0D;
                        NPC.frame.Y += frameHeight;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                    {
                        NPC.frame.Y = 0;
                    }
                }
                else
                {
                    NPC.frame.Y = frameHeight * 4;
                    if (NPC.ai[1] > (otherFrameTimer - 40) && NPC.ai[1] < (otherFrameTimer - 15))
                    {
                        NPC.frame.Y = frameHeight * 5;
                    }
                }
            }
            else
            {
                NPC.frameCounter += (NPC.ai[0] == 3.2f ? 1.5 : 1D);
                if (NPC.frameCounter > 4D) //iban said the time between frames was 5 so using that as a base
                {
                    NPC.frameCounter = 0D;
                    NPC.frame.Y += frameHeight;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < (revenge ? 0.75f : 0.5f) || death;
            bool phase3 = lifeRatio < (death ? 0.4f : revenge ? 0.25f : 0.1f);

            float newPhaseTimer = 180f;
            bool phaseSwitchPhase = (phase2 && calamityGlobalNPC.newAI[0] < newPhaseTimer && calamityGlobalNPC.newAI[2] != 1f) ||
                (phase3 && calamityGlobalNPC.newAI[1] < newPhaseTimer && calamityGlobalNPC.newAI[3] != 1f);

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[Type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[Type].Value.Width / 2), (float)(TextureAssets.Npc[Type].Value.Height / Main.npcFrameCount[Type] / 2));
            Color color = drawColor;
            Color altColor = Color.White;

            float lerpDrawTransition = 0f;
            int newAITracker = 120;
            int buffColorDampener = 60;

            if (phase3 && calamityGlobalNPC.newAI[3] == 1f)
            {
                color = CalamityGlobalNPC.buffColor(color, 0.9f, 0.6f, 0.2f, 1f);
            }
            else if (phase2 && calamityGlobalNPC.newAI[2] == 1f)
            {
                color = CalamityGlobalNPC.buffColor(color, 0.7f, 0.7f, 0.3f, 1f);
            }
            else if (phase2 && calamityGlobalNPC.newAI[0] > (float)newAITracker)
            {
                float phase2TranBuff = calamityGlobalNPC.newAI[0] - (float)newAITracker;
                phase2TranBuff /= (float)buffColorDampener;
                color = CalamityGlobalNPC.buffColor(color, 1f - 0.3f * phase2TranBuff, 1f - 0.3f * phase2TranBuff, 1f - 0.7f * phase2TranBuff, 1f);
            }

            int afterimageAmt = 10;
            int afterimageIncrement = 2;
            if (NPC.ai[0] == 0f || NPC.ai[0] == 3.1f || NPC.ai[0] == 4f || NPC.ai[0] == 4.2f)
            {
                afterimageAmt = 4;
            }
            if (NPC.ai[0] == 1f || NPC.ai[0] == 3f || NPC.ai[0] == 4.1f)
            {
                afterimageAmt = 7;
            }
            if (NPC.ai[0] == 2f || NPC.ai[0] == 3.2f || (phase2 && calamityGlobalNPC.newAI[2] == 1f))
            {
                altColor = Color.Yellow;
                lerpDrawTransition = 0.5f;
            }
            else
            {
                color = altColor;
            }

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += afterimageIncrement)
                {
                    Color afterimageColor = color;
                    afterimageColor = Color.Lerp(afterimageColor, altColor, lerpDrawTransition);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (float)(afterimageAmt - i) / 15f;
                    Vector2 afterimageDrawPos = NPC.oldPos[i] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    afterimageDrawPos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                    afterimageDrawPos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimageDrawPos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            int extraAfterimageAmt = 0;
            float extraAfterimageDampener = 0f;
            float afterimageScaler = 0f;

            if (NPC.ai[0] == 0f || NPC.ai[0] == 3.1f || NPC.ai[0] == 4f || NPC.ai[0] == 4.2f)
            {
                extraAfterimageAmt = 4;
            }

            if (NPC.ai[0] == 5f)
            {
                if (NPC.ai[1] > 60f)
                {
                    extraAfterimageAmt = 6;
                    extraAfterimageDampener = 1f - (float)Math.Cos((double)((NPC.ai[1] - 60f) / 30f * MathHelper.TwoPi));
                    extraAfterimageDampener /= 3f;
                    afterimageScaler = 40f;
                }
            }

            if (phaseSwitchPhase)
            {
                if (phase3 && calamityGlobalNPC.newAI[1] > (float)newAITracker)
                {
                    extraAfterimageAmt = 6;
                    extraAfterimageDampener = 1f - (float)Math.Cos((double)((calamityGlobalNPC.newAI[1] - (float)newAITracker) / (float)buffColorDampener * MathHelper.TwoPi));
                    extraAfterimageDampener /= 3f;
                    afterimageScaler = 60f;
                }
                else if (phase2 && calamityGlobalNPC.newAI[0] > (float)newAITracker)
                {
                    extraAfterimageAmt = 6;
                    extraAfterimageDampener = 1f - (float)Math.Cos((double)((calamityGlobalNPC.newAI[0] - (float)newAITracker) / (float)buffColorDampener * MathHelper.TwoPi));
                    extraAfterimageDampener /= 3f;
                    afterimageScaler = 60f;
                }
            }

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int j = 0; j < extraAfterimageAmt; j++)
                {
                    Color extraAfterimageColor = altColor;
                    extraAfterimageColor = Color.Lerp(extraAfterimageColor, altColor, lerpDrawTransition);
                    extraAfterimageColor = NPC.GetAlpha(extraAfterimageColor);
                    extraAfterimageColor *= 1f - extraAfterimageDampener;
                    Vector2 extraAfterimageDrawPos = NPC.Center + ((float)j / (float)extraAfterimageAmt * MathHelper.TwoPi + NPC.rotation).ToRotationVector2() * afterimageScaler * extraAfterimageDampener - screenPos;
                    extraAfterimageDrawPos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                    extraAfterimageDrawPos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, extraAfterimageDrawPos, NPC.frame, extraAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Color mainDrawingColor = altColor;
            mainDrawingColor = Color.Lerp(mainDrawingColor, altColor, lerpDrawTransition);
            mainDrawingColor = NPC.GetAlpha(mainDrawingColor);
            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, (phase3 && calamityGlobalNPC.newAI[3] == 1f ? mainDrawingColor : NPC.GetAlpha(altColor)), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            if (phase2)
            {
                texture2D15 = GlowTexture.Value;
                Color glowmaskColor = Color.Lerp(Color.White, Color.Red, 0.5f);
                altColor = Color.Red;

                lerpDrawTransition = 1f;
                extraAfterimageDampener = 0.5f;
                afterimageScaler = 10f;
                afterimageIncrement = 1;

                if (phaseSwitchPhase)
                {
                    float glowmaskDampener = (phase3 ? calamityGlobalNPC.newAI[1] : calamityGlobalNPC.newAI[0]) - (float)newAITracker;
                    glowmaskDampener /= (float)buffColorDampener;
                    altColor *= glowmaskDampener;
                    glowmaskColor *= glowmaskDampener;
                }

                if (CalamityClientConfig.Instance.Afterimages)
                {
                    for (int k = 1; k < afterimageAmt; k += afterimageIncrement)
                    {
                        Color glowmaskAfterimageColor = glowmaskColor;
                        glowmaskAfterimageColor = Color.Lerp(glowmaskAfterimageColor, altColor, lerpDrawTransition);
                        glowmaskAfterimageColor *= (float)(afterimageAmt - k) / 15f;
                        Vector2 glowmaskAfterimageDrawPos = NPC.oldPos[k] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                        glowmaskAfterimageDrawPos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                        glowmaskAfterimageDrawPos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                        spriteBatch.Draw(texture2D15, glowmaskAfterimageDrawPos, NPC.frame, glowmaskAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                    }

                    for (int l = 1; l < extraAfterimageAmt; l++)
                    {
                        Color extraGlowmaskAfterimageColor = glowmaskColor;
                        extraGlowmaskAfterimageColor = Color.Lerp(extraGlowmaskAfterimageColor, altColor, lerpDrawTransition);
                        extraGlowmaskAfterimageColor = NPC.GetAlpha(extraGlowmaskAfterimageColor);
                        extraGlowmaskAfterimageColor *= 1f - extraAfterimageDampener;
                        Vector2 extraGlowmaskAfterimageDrawPos = NPC.Center + ((float)l / (float)extraAfterimageAmt * MathHelper.TwoPi + NPC.rotation).ToRotationVector2() * afterimageScaler * extraAfterimageDampener - screenPos;
                        extraGlowmaskAfterimageDrawPos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                        extraGlowmaskAfterimageDrawPos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                        spriteBatch.Draw(texture2D15, extraGlowmaskAfterimageDrawPos, NPC.frame, extraGlowmaskAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                    }
                }

                spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, glowmaskColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
            }

            return false;
        }

        private static Color buffColor(Color newColor, float R, float G, float B, float A)
        {
            newColor.R = (byte)((float)newColor.R * R);
            newColor.G = (byte)((float)newColor.G * G);
            newColor.B = (byte)((float)newColor.B * B);
            newColor.A = (byte)((float)newColor.A * A);
            return newColor;
        }

        public override void BossLoot(ref int potionType) => potionType = ItemID.SuperHealingPotion;

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DragonfollyBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] items = new int[]
                {
                    ModContent.ItemType<GildedProboscis>(),
                    ModContent.ItemType<GoldenEagle>(),
                    ModContent.ItemType<RougeSlash>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, items));

                // Materials
                normalOnly.Add(ModContent.ItemType<EffulgentFeather>(), 1, 25, 30);

                // Equipment
                // 16NOV2025: Ozzatron: item has been chosen as the "Expert gatekept" item for this Calamity boss
                // normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<DynamoStemCells>()));
                normalOnly.Add(ModContent.ItemType<FollyFeed>(), DropHelper.NormalWeaponDropRateFraction);

                // Vanity
                normalOnly.Add(ModContent.ItemType<BumblefuckMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<DragonfollyTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<DragonfollyRelic>());

            // GFB Omega Healing Potion drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(DropHelper.PerPlayer(ModContent.ItemType<OmegaHealingPotion>(), 1, 50, 100), true);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedDragonfolly, ModContent.ItemType<LoreDragonfolly>(), desc: DropHelper.FirstKillText);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<VermillionFlux>(), Main.zenithWorld ? 360 : 180);
        }

        public override void OnKill()
        {
            // Don't bother running any of this in Boss Rush.
            if (BossRushEvent.BossRushActive)
                return;

            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // Mark The Dragonfolly as dead
            DownedBossSystem.downedDragonfolly = true;
            CalamityNetcode.SyncWorld();

            if (Main.zenithWorld)
            {
                int spacing = 40;
                int amt = 7;
                SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, NPC.Center - Vector2.UnitY * 300f);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < amt; i++)
                    {
                        Vector2 fireFrom = new Vector2(NPC.Center.X + (spacing * i) - (spacing * amt / 2), NPC.Center.Y - 900f);
                        Vector2 ai0 = NPC.Center - fireFrom;
                        float ai = Main.rand.Next(100);
                        Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom.X, fireFrom.Y, velocity.X, velocity.Y, ModContent.ProjectileType<RedLightning>(), LightningDamage, 0f, Main.myPlayer, ai0.ToRotation(), ai);
                    }
                }
            }

        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }

        public override void ModifyTypeName(ref string typeName)
        {
            if (Main.zenithWorld)
            {
                typeName = CalamityUtils.GetTextValue("NPCs.Bumblebirb");
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CopperCoin, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CopperCoin, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 6; i++) // 1 head, 1 wing, 4 legs = 6. one wing due to them being chonky boyes now
                    {
                        string gore = "Bumble";
                        float randomSpread = Main.rand.Next(-200, 201) / 100f;
                        gore += i == 0 ? "Head" : i > 1 ? "Leg" : "Wing";
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>(gore).Type, 1f);
                    }
                }
            }
        }
    }
}
