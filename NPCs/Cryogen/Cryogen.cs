using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace CalamityMod.NPCs.Cryogen
{
    [AutoloadBossHead]
    public class Cryogen : ModNPC
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private int time = 0;
        private int iceShard = 0;
        private int currentPhase = 1;
        private int teleportLocationX = 0;

        public override string Texture => "CalamityMod/NPCs/Cryogen/Cryogen_Phase1";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryogen");
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 24f;
            NPC.GetNPCDamage();
            NPC.width = 86;
            NPC.height = 88;
            NPC.defense = 12;
            NPC.DR_NERD(0.3f);
            NPC.LifeMaxNERB(30000, 36000, 300000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 40, 0, 0);
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath15;
            Music = CalamityMod.Instance.GetMusicFromMusicMod("Cryogen") ?? MusicID.FrostMoon;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = false;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(biomeEnrageTimer);
            writer.Write(time);
            writer.Write(iceShard);
            writer.Write(teleportLocationX);
            writer.Write(NPC.dontTakeDamage);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
            time = reader.ReadInt32();
            iceShard = reader.ReadInt32();
            teleportLocationX = reader.ReadInt32();
            NPC.dontTakeDamage = reader.ReadBoolean();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0f, 1f, 1f);

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Enrage
            if (!player.ZoneSnow && !BossRushEvent.BossRushActive)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0 || malice;

            float enrageScale = death ? 0.5f : 0f;
            if (biomeEnraged)
            {
                NPC.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
                enrageScale += 2f;
            }

            if (enrageScale > 2f)
                enrageScale = 2f;

            if (BossRushEvent.BossRushActive)
                enrageScale = 3f;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < (revenge ? 0.85f : 0.8f) || death;
            bool phase3 = lifeRatio < (death ? 0.8f : revenge ? 0.7f : 0.6f);
            bool phase4 = lifeRatio < (death ? 0.6f : revenge ? 0.55f : 0.4f);
            bool phase5 = lifeRatio < (death ? 0.5f : revenge ? 0.45f : 0.3f);
            bool phase6 = lifeRatio < (death ? 0.35f : 0.25f) && revenge;
            bool phase7 = lifeRatio < (death ? 0.25f : 0.15f) && revenge;

            if ((int)NPC.ai[0] + 1 > currentPhase)
                HandlePhaseTransition((int)NPC.ai[0] + 1);

            if (NPC.ai[2] == 0f && NPC.localAI[1] == 0f && Main.netMode != NetmodeID.MultiplayerClient && (NPC.ai[0] < 3f || BossRushEvent.BossRushActive || (death && NPC.ai[0] > 3f))) //spawn shield for phase 0 1 2, not 3 4 5
            {
                int num6 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<CryogenIce>(), NPC.whoAmI);
                NPC.ai[2] = num6 + 1;
                NPC.localAI[1] = -1f;
                NPC.netUpdate = true;
                Main.npc[num6].ai[0] = NPC.whoAmI;
                Main.npc[num6].netUpdate = true;
            }

            int num7 = (int)NPC.ai[2] - 1;
            if (num7 != -1 && Main.npc[num7].active && Main.npc[num7].type == ModContent.NPCType<CryogenIce>())
            {
                NPC.dontTakeDamage = true;
            }
            else
            {
                NPC.dontTakeDamage = false;
                NPC.ai[2] = 0f;

                if (NPC.localAI[1] == -1f)
                    NPC.localAI[1] = death ? 540f : expertMode ? 720f : 1080f;
                if (NPC.localAI[1] > 0f)
                    NPC.localAI[1] -= 1f;
            }

            CalamityMod.StopRain();

            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    if (NPC.velocity.Y > 3f)
                        NPC.velocity.Y = 3f;
                    NPC.velocity.Y -= 0.1f;
                    if (NPC.velocity.Y < -12f)
                        NPC.velocity.Y = -12f;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    if (NPC.ai[1] != 0f)
                    {
                        NPC.ai[1] = 0f;
                        teleportLocationX = 0;
                        iceShard = 0;
                        NPC.netUpdate = true;
                    }
                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            float chargeGateValue = malice ? 240f : 360f;
            bool charging = NPC.ai[1] >= chargeGateValue;

            if (Main.netMode != NetmodeID.MultiplayerClient && expertMode && (NPC.ai[0] < 5f || !phase6) && !charging)
            {
                time++;
                if (time >= (malice ? 480 : 600))
                {
                    time = 0;
                    SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                    int totalProjectiles = 3;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    int type = ModContent.ProjectileType<IceBomb>();
                    int damage = NPC.GetProjectileDamage(type);
                    float velocity = 2f + NPC.ai[0];
                    double angleA = radians * 0.5;
                    double angleB = MathHelper.ToRadians(90f) - angleA;
                    float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                    Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vector255, type, damage, 0f, Main.myPlayer, 0f, NPC.ai[0] * 0.5f);
                    }
                }
            }

            if (NPC.ai[0] == 0f)
            {
                NPC.rotation = NPC.velocity.X * 0.1f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.localAI[0] += 1f;
                    if (NPC.localAI[0] >= 120f)
                    {
                        NPC.localAI[0] = 0f;
                        NPC.TargetClosest();
                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                        {
                            SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                            int totalProjectiles = malice ? 24 : 16;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            int type = ModContent.ProjectileType<IceBlast>();
                            int damage = NPC.GetProjectileDamage(type);
                            float velocity = 9f + enrageScale;
                            Vector2 spinningPoint = new Vector2(0f, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vector255, type, damage, 0f, Main.myPlayer);
                            }
                        }
                    }
                }

                Vector2 vector142 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num1243 = player.Center.X - vector142.X;
                float num1244 = player.Center.Y - vector142.Y;
                float num1245 = (float)Math.Sqrt(num1243 * num1243 + num1244 * num1244);

                float num1246 = revenge ? 5f : 4f;
                num1246 += 4f * enrageScale;

                num1245 = num1246 / num1245;
                num1243 *= num1245;
                num1244 *= num1245;
                NPC.velocity.X = (NPC.velocity.X * 50f + num1243) / 51f;
                NPC.velocity.Y = (NPC.velocity.Y * 50f + num1244) / 51f;

                if (phase2)
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 1f;
                    NPC.localAI[0] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                if (NPC.ai[1] < chargeGateValue)
                {
                    NPC.ai[1] += 1f;

                    NPC.rotation = NPC.velocity.X * 0.1f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.localAI[0] += 1f;
                        if (NPC.localAI[0] >= 120f)
                        {
                            NPC.localAI[0] = 0f;
                            NPC.TargetClosest();
                            if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                            {
                                SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                                int totalProjectiles = malice ? 18 : 12;
                                float radians = MathHelper.TwoPi / totalProjectiles;
                                int type = ModContent.ProjectileType<IceBlast>();
                                int damage = NPC.GetProjectileDamage(type);
                                float velocity2 = 9f + enrageScale;
                                Vector2 spinningPoint = new Vector2(0f, -velocity2);
                                for (int k = 0; k < totalProjectiles; k++)
                                {
                                    Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vector255, type, damage, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }

                    float velocity = revenge ? 3.5f : 4f;
                    float acceleration = 0.15f;
                    velocity -= enrageScale * 0.8f;
                    acceleration += 0.07f * enrageScale;

                    if (NPC.position.Y > player.position.Y - 375f)
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y *= 0.98f;

                        NPC.velocity.Y -= acceleration;

                        if (NPC.velocity.Y > velocity)
                            NPC.velocity.Y = velocity;
                    }
                    else if (NPC.position.Y < player.position.Y - 425f)
                    {
                        if (NPC.velocity.Y < 0f)
                            NPC.velocity.Y *= 0.98f;

                        NPC.velocity.Y += acceleration;

                        if (NPC.velocity.Y < -velocity)
                            NPC.velocity.Y = -velocity;
                    }

                    if (NPC.position.X + (NPC.width / 2) > player.position.X + (player.width / 2) + 300f)
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X *= 0.98f;

                        NPC.velocity.X -= acceleration;

                        if (NPC.velocity.X > velocity)
                            NPC.velocity.X = velocity;
                    }
                    if (NPC.position.X + (NPC.width / 2) < player.position.X + (player.width / 2) - 300f)
                    {
                        if (NPC.velocity.X < 0f)
                            NPC.velocity.X *= 0.98f;

                        NPC.velocity.X += acceleration;

                        if (NPC.velocity.X < -velocity)
                            NPC.velocity.X = -velocity;
                    }
                }
                else if (NPC.ai[1] < chargeGateValue + 30f)
                {
                    NPC.ai[1] += 1f;

                    calamityGlobalNPC.newAI[0] += 0.025f;
                    if (calamityGlobalNPC.newAI[0] > 0.5f)
                        calamityGlobalNPC.newAI[0] = 0.5f;

                    NPC.rotation += calamityGlobalNPC.newAI[0];
                    NPC.velocity *= 0.98f;
                }
                else
                {
                    if (NPC.ai[1] == chargeGateValue + 30f)
                    {
                        NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * (18f + enrageScale * 2f);

                        NPC.ai[1] = chargeGateValue + 90f;
                        calamityGlobalNPC.newAI[0] = 0f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                            {
                                SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                                int type = ModContent.ProjectileType<IceRain>();
                                int damage = NPC.GetProjectileDamage(type);
                                float velocity = 9f + enrageScale;
                                for (int i = 0; i < 2; i++)
                                {
                                    int totalProjectiles = 10;
                                    float radians = MathHelper.TwoPi / totalProjectiles;
                                    float newVelocity = velocity - (velocity * 0.5f * i);
                                    double angleA = radians * 0.5;
                                    double angleB = MathHelper.ToRadians(90f) - angleA;
                                    float velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                                    Vector2 spinningPoint = i == 0 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
                                    for (int k = 0; k < totalProjectiles; k++)
                                    {
                                        Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vector255, type, damage, 0f, Main.myPlayer, 0f, velocity);
                                    }
                                }
                            }
                        }
                    }

                    NPC.ai[1] -= 1f;
                    if (NPC.ai[1] == chargeGateValue + 30f)
                    {
                        NPC.TargetClosest();

                        NPC.ai[1] = 0f;
                        NPC.localAI[0] = 0f;

                        NPC.rotation = NPC.velocity.X * 0.1f;
                    }
                    else if (NPC.ai[1] <= chargeGateValue + 45f)
                    {
                        NPC.velocity *= 0.95f;
                        NPC.rotation = NPC.velocity.X * 0.15f;
                    }
                    else
                        NPC.rotation += NPC.direction * 0.5f;
                }

                if (phase3)
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 2f;
                    NPC.ai[1] = 0f;
                    NPC.localAI[0] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;
                    iceShard = 0;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 2f)
            {
                if (NPC.ai[1] < chargeGateValue)
                {
                    NPC.ai[1] += 1f;

                    NPC.rotation = NPC.velocity.X * 0.1f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.localAI[0] += 1f;
                        if (NPC.localAI[0] >= 120f)
                        {
                            NPC.localAI[0] = 0f;
                            NPC.TargetClosest();
                            if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                            {
                                SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                                int totalProjectiles = malice ? 18 : 12;
                                float radians = MathHelper.TwoPi / totalProjectiles;
                                int type = ModContent.ProjectileType<IceBlast>();
                                int damage = NPC.GetProjectileDamage(type);
                                float velocity = 9f + enrageScale;
                                Vector2 spinningPoint = new Vector2(0f, -velocity);
                                for (int k = 0; k < totalProjectiles; k++)
                                {
                                    Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vector255, type, damage, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }

                    Vector2 vector142 = new Vector2(NPC.Center.X, NPC.Center.Y);
                    float num1243 = player.Center.X - vector142.X;
                    float num1244 = player.Center.Y - vector142.Y;
                    float num1245 = (float)Math.Sqrt(num1243 * num1243 + num1244 * num1244);

                    float num1246 = revenge ? 7f : 6f;
                    num1246 += 4f * enrageScale;

                    num1245 = num1246 / num1245;
                    num1243 *= num1245;
                    num1244 *= num1245;
                    NPC.velocity.X = (NPC.velocity.X * 50f + num1243) / 51f;
                    NPC.velocity.Y = (NPC.velocity.Y * 50f + num1244) / 51f;
                }
                else if (NPC.ai[1] < chargeGateValue + 20f)
                {
                    NPC.ai[1] += 1f;

                    calamityGlobalNPC.newAI[0] += 0.025f;
                    if (calamityGlobalNPC.newAI[0] > 0.5f)
                        calamityGlobalNPC.newAI[0] = 0.5f;

                    NPC.rotation += calamityGlobalNPC.newAI[0];
                    NPC.velocity *= 0.98f;
                }
                else
                {
                    if (NPC.ai[1] == chargeGateValue + 20f)
                    {
                        NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * (18f + enrageScale * 2f);

                        NPC.ai[1] = chargeGateValue + 80f;
                        calamityGlobalNPC.newAI[0] = 0f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                            {
                                SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                                int type = ModContent.ProjectileType<IceRain>();
                                int damage = NPC.GetProjectileDamage(type);
                                float velocity = 9f + enrageScale;
                                for (int i = 0; i < 3; i++)
                                {
                                    int totalProjectiles = 8;
                                    float radians = MathHelper.TwoPi / totalProjectiles;
                                    float newVelocity = velocity - (velocity * 0.33f * i);
                                    double angleA = radians * 0.5;
                                    double angleB = MathHelper.ToRadians(90f) - angleA;
                                    float velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                                    Vector2 spinningPoint = i == 1 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
                                    for (int k = 0; k < totalProjectiles; k++)
                                    {
                                        Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vector255, type, damage, 0f, Main.myPlayer, 0f, velocity);
                                    }
                                }
                            }
                        }
                    }

                    NPC.ai[1] -= 1f;
                    if (NPC.ai[1] == chargeGateValue + 20f)
                    {
                        NPC.TargetClosest();

                        calamityGlobalNPC.newAI[1] += 1f;
                        if (calamityGlobalNPC.newAI[1] > 1f)
                        {
                            NPC.ai[1] = 0f;
                            NPC.localAI[0] = 0f;
                            calamityGlobalNPC.newAI[1] = 0f;
                        }

                        NPC.rotation = NPC.velocity.X * 0.1f;
                    }
                    else if (NPC.ai[1] <= chargeGateValue + 35f)
                    {
                        NPC.velocity *= 0.95f;
                        NPC.rotation = NPC.velocity.X * 0.15f;
                    }
                    else
                        NPC.rotation += NPC.direction * 0.5f;
                }

                if (phase4)
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 3f;
                    NPC.ai[1] = 0f;
                    NPC.localAI[0] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                NPC.rotation = NPC.velocity.X * 0.1f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.localAI[0] += 1f;
                    if (NPC.localAI[0] >= 90f && NPC.Opacity == 1f)
                    {
                        NPC.localAI[0] = 0f;
                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                        {
                            SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                            int totalProjectiles = malice ? 18 : 12;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            int type = ModContent.ProjectileType<IceBlast>();
                            int damage = NPC.GetProjectileDamage(type);
                            float velocity = 10f + enrageScale;
                            Vector2 spinningPoint = new Vector2(0f, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vector255, type, damage, 0f, Main.myPlayer);
                            }
                        }
                    }
                }

                Vector2 vector142 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num1243 = player.Center.X - vector142.X;
                float num1244 = player.Center.Y - vector142.Y;
                float num1245 = (float)Math.Sqrt(num1243 * num1243 + num1244 * num1244);

                float speed = revenge ? 5.5f : 5f;
                speed += 3f * enrageScale;

                num1245 = speed / num1245;
                num1243 *= num1245;
                num1244 *= num1245;
                NPC.velocity.X = (NPC.velocity.X * 50f + num1243) / 51f;
                NPC.velocity.Y = (NPC.velocity.Y * 50f + num1244) / 51f;

                if (NPC.ai[1] == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.localAI[2] += 1f;
                        if (NPC.localAI[2] >= 180f)
                        {
                            NPC.TargetClosest();
                            NPC.localAI[2] = 0f;
                            int num1249 = 0;
                            int num1250;
                            int num1251;
                            while (true)
                            {
                                num1249++;
                                num1250 = (int)player.Center.X / 16;
                                num1251 = (int)player.Center.Y / 16;

                                int min = 16;
                                int max = 20;

                                if (Main.rand.NextBool(2))
                                    num1250 += Main.rand.Next(min, max);
                                else
                                    num1250 -= Main.rand.Next(min, max);

                                if (Main.rand.NextBool(2))
                                    num1251 += Main.rand.Next(min, max);
                                else
                                    num1251 -= Main.rand.Next(min, max);

                                if (!WorldGen.SolidTile(num1250, num1251) && Collision.CanHit(new Vector2(num1250 * 16, num1251 * 16), 1, 1, player.position, player.width, player.height))
                                    break;

                                if (num1249 > 100)
                                    goto Block;
                            }
                            NPC.ai[1] = 1f;
                            teleportLocationX = num1250;
                            iceShard = num1251;
                            NPC.netUpdate = true;
                            Block:
                            ;
                        }
                    }
                }
                else if (NPC.ai[1] == 1f)
                {
                    Vector2 position = new Vector2(teleportLocationX * 16f - (NPC.width / 2), iceShard * 16f - (NPC.height / 2));
                    for (int m = 0; m < 5; m++)
                    {
                        int dust = Dust.NewDust(position, NPC.width, NPC.height, 67, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].noGravity = true;
                    }

                    NPC.Opacity -= 0.008f;
                    if (NPC.Opacity <= 0f)
                    {
                        NPC.Opacity = 0f;
                        NPC.position = position;

                        for (int n = 0; n < 15; n++)
                        {
                            int num39 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 67, 0f, 0f, 100, default, 3f);
                            Main.dust[num39].noGravity = true;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                            {
                                NPC.localAI[0] = 0f;
                                SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                                int type = ModContent.ProjectileType<IceRain>();
                                int damage = NPC.GetProjectileDamage(type);
                                float velocity = 9f + enrageScale;
                                for (int i = 0; i < 3; i++)
                                {
                                    int totalProjectiles = malice ? 9 : 6;
                                    float radians = MathHelper.TwoPi / totalProjectiles;
                                    float newVelocity = velocity - (velocity * 0.33f * i);
                                    float velocityX = 0f;
                                    if (i > 0)
                                    {
                                        double angleA = radians * 0.33 * (3 - i);
                                        double angleB = MathHelper.ToRadians(90f) - angleA;
                                        velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                                    }
                                    Vector2 spinningPoint = i == 0 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
                                    for (int k = 0; k < totalProjectiles; k++)
                                    {
                                        Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vector255, type, damage, 0f, Main.myPlayer, 0f, velocity);
                                    }
                                }
                            }
                        }

                        NPC.ai[1] = 2f;
                        NPC.netUpdate = true;
                    }
                }
                else if (NPC.ai[1] == 2f)
                {
                    NPC.Opacity += 0.2f;
                    if (NPC.Opacity >= 1f)
                    {
                        NPC.Opacity = 1f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }
                }

                if (phase5)
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 4f;
                    NPC.ai[1] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.localAI[0] = 0f;
                    NPC.localAI[2] = 0f;
                    NPC.Opacity = 1f;
                    teleportLocationX = 0;
                    iceShard = 0;
                    NPC.netUpdate = true;

                    int chance = 100;
                    if (DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
                        chance = 20;

                    if (Main.rand.NextBool(chance))
                    {
                        string key = "Mods.CalamityMod.CryogenBossText";
                        Color messageColor = Color.Cyan;
                        CalamityUtils.DisplayLocalizedText(key, messageColor);
                    }
                }
            }
            else if (NPC.ai[0] == 4f)
            {
                NPC.damage = NPC.defDamage;

                if (phase6)
                {
                    if (NPC.ai[1] == 60f)
                    {
                        NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * (18f + enrageScale * 2f);

                        if (phase7)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                                {
                                    SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                                    int type = ModContent.ProjectileType<IceRain>();
                                    int damage = NPC.GetProjectileDamage(type);
                                    float velocity = 9f + enrageScale;
                                    for (int i = 0; i < 4; i++)
                                    {
                                        int totalProjectiles = malice ? 6 : 4;
                                        float radians = MathHelper.TwoPi / totalProjectiles;
                                        float newVelocity = velocity - (velocity * 0.25f * i);
                                        float velocityX = 0f;
                                        if (i > 0)
                                        {
                                            double angleA = radians * 0.25 * (4 - i);
                                            double angleB = MathHelper.ToRadians(90f) - angleA;
                                            velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                                        }
                                        Vector2 spinningPoint = i == 0 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
                                        for (int k = 0; k < totalProjectiles; k++)
                                        {
                                            Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vector255, type, damage, 0f, Main.myPlayer, 0f, velocity);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    NPC.ai[1] -= 1f;
                    if (NPC.ai[1] <= 0f)
                    {
                        NPC.ai[3] += 1f;
                        NPC.TargetClosest();
                        if (NPC.ai[3] > 2f)
                        {
                            NPC.damage = 0;
                            NPC.defense = 0;
                            NPC.ai[0] = 5f;
                            NPC.ai[1] = 0f;
                            NPC.ai[3] = 0f;
                            time = 0;
                        }
                        else
                            NPC.ai[1] = 60f;

                        NPC.rotation = NPC.velocity.X * 0.1f;
                    }
                    else if (NPC.ai[1] <= 15f)
                    {
                        NPC.velocity *= 0.95f;
                        NPC.rotation = NPC.velocity.X * 0.15f;
                    }
                    else
                        NPC.rotation += NPC.direction * 0.5f;

                    return;
                }

                float num1372 = 16f + enrageScale * 2f;

                Vector2 vector167 = new Vector2(NPC.Center.X + (NPC.direction * 20), NPC.Center.Y + 6f);
                float num1373 = player.position.X + player.width * 0.5f - vector167.X;
                float num1374 = player.Center.Y - vector167.Y;
                float num1375 = (float)Math.Sqrt(num1373 * num1373 + num1374 * num1374);
                float num1376 = num1372 / num1375;
                num1373 *= num1376;
                num1374 *= num1376;
                iceShard--;

                if (num1375 < 200f || iceShard > 0)
                {
                    if (num1375 < 200f)
                        iceShard = 20;

                    NPC.rotation += NPC.direction * 0.3f;
                    return;
                }

                NPC.velocity.X = (NPC.velocity.X * 50f + num1373) / 51f;
                NPC.velocity.Y = (NPC.velocity.Y * 50f + num1374) / 51f;
                if (num1375 < 350f)
                {
                    NPC.velocity.X = (NPC.velocity.X * 10f + num1373) / 11f;
                    NPC.velocity.Y = (NPC.velocity.Y * 10f + num1374) / 11f;
                }
                if (num1375 < 300f)
                {
                    NPC.velocity.X = (NPC.velocity.X * 7f + num1373) / 8f;
                    NPC.velocity.Y = (NPC.velocity.Y * 7f + num1374) / 8f;
                }

                NPC.rotation = NPC.velocity.X * 0.15f;
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.1f;

                time++;
                if (time >= (malice ? 50 : 75))
                {
                    time = 0;
                    SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                    int totalProjectiles = 4;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    int type = ModContent.ProjectileType<IceBomb>();
                    int damage = NPC.GetProjectileDamage(type);
                    float velocity2 = 6f;
                    double angleA = radians * 0.5;
                    double angleB = MathHelper.ToRadians(90f) - angleA;
                    float velocityX = (float)(velocity2 * Math.Sin(angleA) / Math.Sin(angleB));
                    Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity2) : new Vector2(velocityX, -velocity2);
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vector255, type, damage, 0f, Main.myPlayer, 0f, 1f);
                    }
                }

                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= (malice ? 120f : 180f))
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 4f;
                    NPC.ai[1] = 60f;
                    time = 0;
                    iceShard = 0;
                    NPC.netUpdate = true;
                }

                float velocity = revenge ? 5f : 6f;
                float acceleration = 0.2f;
                velocity -= enrageScale;
                acceleration += 0.07f * enrageScale;

                if (NPC.position.Y > player.position.Y - 375f)
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= 0.98f;

                    NPC.velocity.Y -= acceleration;

                    if (NPC.velocity.Y > velocity)
                        NPC.velocity.Y = velocity;
                }
                else if (NPC.position.Y < player.position.Y - 400f)
                {
                    if (NPC.velocity.Y < 0f)
                        NPC.velocity.Y *= 0.98f;

                    NPC.velocity.Y += acceleration;

                    if (NPC.velocity.Y < -velocity)
                        NPC.velocity.Y = -velocity;
                }

                if (NPC.position.X + (NPC.width / 2) > player.position.X + (player.width / 2) + 350f)
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X *= 0.98f;

                    NPC.velocity.X -= acceleration;

                    if (NPC.velocity.X > velocity)
                        NPC.velocity.X = velocity;
                }
                if (NPC.position.X + (NPC.width / 2) < player.position.X + (player.width / 2) - 350f)
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X *= 0.98f;

                    NPC.velocity.X += acceleration;

                    if (NPC.velocity.X < -velocity)
                        NPC.velocity.X = -velocity;
                }
            }
        }

        private void HandlePhaseTransition(int newPhase)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath15, NPC.Center);
            if (Main.netMode != NetmodeID.Server)
            {
                int chipGoreAmount = newPhase >= 5 ? 3 : newPhase >= 3 ? 2 : 1;
                for (int i = 1; i < chipGoreAmount; i++)
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("CryoChipGore" + i).Type, 1f);
            }

            currentPhase = newPhase;

            switch (currentPhase)
            {
                case 0:
                case 1:
                    break;
                case 2:
                    NPC.Calamity().DR = 0.27f;
                    break;
                case 3:
                    NPC.Calamity().DR = 0.21f;
                    break;
                case 4:
                    NPC.Calamity().DR = 0.12f;
                    break;
                case 5:
                case 6:
                    NPC.Calamity().DR = 0f;
                    break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (currentPhase > 1)
            {
                string phase = "CalamityMod/NPCs/Cryogen/Cryogen_Phase" + currentPhase;
                Texture2D texture = ModContent.Request<Texture2D>(phase).Value;

                SpriteEffects spriteEffects = SpriteEffects.None;
                if (NPC.spriteDirection == 1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                Vector2 origin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
                Vector2 drawPos = NPC.Center - screenPos;
                drawPos -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                drawPos += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                spriteBatch.Draw(texture, drawPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
                return false;
            }
            return true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 67, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 67, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 67, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 67, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    float randomSpread = Main.rand.Next(-200, 200) / 100;
                    for (int i = 1; i < 4; i++)
                    {
                        Gore.NewGore(NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("CryoDeathGore" + i).Type, 1f);
                        Gore.NewGore(NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("CryoChipGore" + i).Type, 1f);
                    }
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Boss bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CryogenBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<Avalanche>(),
                    ModContent.ItemType<GlacialCrusher>(),
                    ModContent.ItemType<EffluviumBow>(),
                    ModContent.ItemType<SnowstormStaff>(),
                    ModContent.ItemType<Icebreaker>(),
                    ModContent.ItemType<CryoStone>(),
                    ModContent.ItemType<FrostFlare>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));
                normalOnly.Add(ModContent.ItemType<ColdDivinity>(), 10);

                // Vanity
                normalOnly.Add(ModContent.ItemType<CryogenMask>(), 7);

                // Materials
                normalOnly.Add(ModContent.ItemType<EssenceofEleum>(), 1, 4, 8);

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<SoulofCryogen>()));
            }

            npcLoot.Add(ItemID.FrozenKey, 3);

            // Trophy (always directly from boss, never in bag)
            npcLoot.Add(ModContent.ItemType<CryogenTrophy>(), 10);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedCryogen, ModContent.ItemType<KnowledgeCryogen>());
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // Spawn Permafrost if he isn't in the world
            int permafrostNPC = NPC.FindFirstNPC(ModContent.NPCType<DILF>());
            if (permafrostNPC == -1)
                NPC.NewNPC(NPC.GetSpawnSource_NPCHurt(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DILF>(), 0, 0f, 0f, 0f, 0f, 255);

            // If Cryogen has not been killed, notify players about Cryonic Ore
            if (!DownedBossSystem.downedCryogen)
            {
                string key = "Mods.CalamityMod.IceOreText";
                Color messageColor = Color.LightSkyBlue;
                CalamityUtils.SpawnOre(ModContent.TileType<CryonicOre>(), 15E-05, 0.45f, 0.65f, 3, 8, TileID.SnowBlock, TileID.IceBlock, TileID.CorruptIce, TileID.FleshIce, TileID.HallowedIce, ModContent.TileType<AstralSnow>(), ModContent.TileType<AstralIce>());

                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            // Mark Cryogen as dead
            DownedBossSystem.downedCryogen = true;
            CalamityNetcode.SyncWorld();
        }

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= 40f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Frostburn, 120, true);
            player.AddBuff(BuffID.Chilled, 90, true);
        }
    }
}
