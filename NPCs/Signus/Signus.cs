using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
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

namespace CalamityMod.NPCs.Signus
{
    [AutoloadBossHead]
    public class Signus : ModNPC
    {
        private int spawnX = 750;
        private int spawnY = 120;
        private int lifeToAlpha = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Signus, Envoy of the Devourer");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                PortraitPositionYOverride = 30f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 32f;
            NPC.GetNPCDamage();
            NPC.width = 130;
            NPC.height = 130;
            NPC.defense = 60;

            bool notDoGFight = CalamityWorld.DoGSecondStageCountdown <= 0 || !DownedBossSystem.downedSignus;
            NPC.LifeMaxNERB(notDoGFight ? 297000 : 73000, notDoGFight ? 356400 : 87600, 240000);

            // If fought alone, Signus plays his own theme
            if (notDoGFight)
            {
                NPC.value = Item.buyPrice(2, 0, 0, 0);
                Music = CalamityMod.Instance.GetMusicFromMusicMod("Signus") ?? MusicID.Boss4;
            }
            // If fought as a DoG interlude, keep the DoG music playing
            else
                Music = CalamityMod.Instance.GetMusicFromMusicMod("ScourgeofTheUniverse") ?? MusicID.Boss3;

            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(spawnX);
            writer.Write(spawnY);
            writer.Write(lifeToAlpha);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            spawnX = reader.ReadInt32();
            spawnY = reader.ReadInt32();
            lifeToAlpha = reader.ReadInt32();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            CalamityGlobalNPC.signus = NPC.whoAmI;

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            Vector2 vectorCenter = NPC.Center;

            double lifeRatio = NPC.life / (double)NPC.lifeMax;

            lifeToAlpha = (int)(100.0 * (1.0 - lifeRatio));
            int maxCharges = death ? 1 : revenge ? 2 : expertMode ? 3 : 4;
            int maxTeleports = (death && lifeRatio < 0.9) ? 1 : revenge ? 2 : expertMode ? 3 : 4;
            float inertia = malice ? 9f : death ? 10f : revenge ? 11f : expertMode ? 12f : 14f;
            float chargeVelocity = malice ? 16f : death ? 14f : revenge ? 13f : expertMode ? 12f : 10f;
            bool phase2 = lifeRatio < 0.75f && expertMode;
            bool phase3 = lifeRatio < 0.5f;
            bool phase4 = lifeRatio < 0.33f;

            NPC.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && malice;

            NPC.damage = NPC.defDamage;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, vectorCenter) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (!player.active || player.dead || Vector2.Distance(player.Center, vectorCenter) > 6400f)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || Vector2.Distance(player.Center, vectorCenter) > 6400f)
                {
                    if (NPC.timeLeft < 10)
                    {
                        CalamityWorld.DoGSecondStageCountdown = 0;
                        if (Main.netMode == NetmodeID.Server)
                        {
                            var netMessage = Mod.GetPacket();
                            netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                            netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                            netMessage.Send();
                        }
                    }

                    NPC.rotation = NPC.velocity.X * 0.04f;

                    if (NPC.velocity.Y > 3f)
                        NPC.velocity.Y = 3f;
                    NPC.velocity.Y -= 0.15f;
                    if (NPC.velocity.Y < -12f)
                        NPC.velocity.Y = -12f;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    if (NPC.ai[0] != 0f)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        calamityGlobalNPC.newAI[0] = 0f;
                        calamityGlobalNPC.newAI[1] = 0f;
                        spawnY = 120;
                        NPC.netUpdate = true;
                    }
                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            if (lifeToAlpha < 50 && NPC.ai[0] != 1f)
            {
                for (int num1011 = 0; num1011 < 2; num1011++)
                {
                    if (Main.rand.Next(3) < 1)
                    {
                        int num1012 = Dust.NewDust(vectorCenter - new Vector2(70f), 70 * 2, 70 * 2, (int)CalamityDusts.PurpleCosmilite, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 90, default, 1.5f);
                        Main.dust[num1012].noGravity = true;
                        Main.dust[num1012].velocity *= 0.2f;
                        Main.dust[num1012].fadeIn = 1f;
                    }
                }
            }

            if (NPC.ai[0] <= 2f)
            {
                NPC.rotation = NPC.velocity.X * 0.04f;
                float playerLocation = vectorCenter.X - player.Center.X;
                NPC.direction = playerLocation < 0f ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                NPC.knockBackResist = 0.05f;
                if (expertMode)
                    NPC.knockBackResist *= Main.RegisteredGameModes[GameModeID.Expert].KnockbackToEnemiesMultiplier;
                if (phase3 || revenge)
                    NPC.knockBackResist = 0f;

                float speed = malice ? 20f : revenge ? 15f : expertMode ? 14f : 12f;
                if (expertMode)
                    speed += death ? 6f * (float)(1D - lifeRatio) : 4f * (float)(1D - lifeRatio);

                float num795 = player.Center.X - vectorCenter.X;
                float num796 = player.Center.Y - vectorCenter.Y;
                float num797 = (float)Math.Sqrt(num795 * num795 + num796 * num796);
                num797 = speed / num797;
                num795 *= num797;
                num796 *= num797;
                NPC.velocity.X = (NPC.velocity.X * 50f + num795) / 51f;
                NPC.velocity.Y = (NPC.velocity.Y * 50f + num796) / 51f;
            }
            else
                NPC.knockBackResist = 0f;

            if (NPC.ai[0] == -1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int phase;
                    do phase = Main.rand.Next(5);
                    while (phase == NPC.ai[1] || (phase == 0 && phase4) || phase == 1 || phase == 2);

                    NPC.ai[0] = phase;
                    NPC.ai[1] = 0f;

                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 0f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.localAI[1] += malice ? 1.5f : 1f;

                    if (expertMode)
                        NPC.localAI[1] += death ? 3f * (float)(1D - lifeRatio) : 2f * (float)(1D - lifeRatio);

                    if (NPC.localAI[1] >= 120f)
                    {
                        NPC.localAI[1] = 0f;

                        NPC.TargetClosest();

                        int num1249 = 0;
                        int num1250;
                        int num1251;
                        while (true)
                        {
                            num1249++;
                            num1250 = (int)player.Center.X / 16;
                            num1251 = (int)player.Center.Y / 16;

                            int min = 14;
                            int max = 18;

                            if (Main.rand.NextBool(2))
                                num1250 += Main.rand.Next(min, max);
                            else
                                num1250 -= Main.rand.Next(min, max);

                            if (Main.rand.NextBool(2))
                                num1251 += Main.rand.Next(min, max);
                            else
                                num1251 -= Main.rand.Next(min, max);

                            if (!WorldGen.SolidTile(num1250, num1251))
                                break;

                            if (num1249 > 100)
                                return;
                        }

                        NPC.ai[0] = 1f;
                        NPC.ai[1] = num1250;
                        NPC.ai[2] = num1251;

                        NPC.netUpdate = true;

                        return;
                    }
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                Vector2 position = new Vector2(NPC.ai[1] * 16f - (NPC.width / 2), NPC.ai[2] * 16f - (NPC.height / 2));
                for (int m = 0; m < 5; m++)
                {
                    int dust = Dust.NewDust(position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 90, default, 2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].fadeIn = 1f;
                }

                NPC.alpha += malice ? 3 : 2;
                if (expertMode)
                    NPC.alpha += death ? (int)Math.Round(4.5D * (1D - lifeRatio)) : (int)Math.Round(3D * (1D - lifeRatio));

                if (NPC.alpha >= 255)
                {
                    SoundEngine.PlaySound(SoundID.Item8, vectorCenter);

                    NPC.alpha = 255;

                    NPC.position = position;

                    for (int n = 0; n < 15; n++)
                    {
                        int num39 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 90, default, 3f);
                        Main.dust[num39].noGravity = true;
                    }

                    NPC.ai[0] = 2f;

                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 2f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.alpha -= 50;
                if (NPC.alpha <= lifeToAlpha)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && revenge)
                    {
                        SoundEngine.PlaySound(SoundID.Item, (int)NPC.position.X, (int)NPC.position.Y, 122);

                        int num660 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(player.position.X + 750f), (int)player.position.Y, ModContent.NPCType<SignusBomb>());
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num660, 0f, 0f, 0f, 0, 0, 0);

                        int num661 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(player.position.X - 750f), (int)player.position.Y, ModContent.NPCType<SignusBomb>());
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num661, 0f, 0f, 0f, 0, 0, 0);

                        for (int num621 = 0; num621 < 5; num621++)
                        {
                            int num622 = Dust.NewDust(new Vector2(player.position.X + 750f, player.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                            Main.dust[num622].velocity *= 3f;
                            Main.dust[num622].noGravity = true;
                            if (Main.rand.NextBool(2))
                            {
                                Main.dust[num622].scale = 0.5f;
                                Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                            int num623 = Dust.NewDust(new Vector2(player.position.X - 750f, player.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                            Main.dust[num623].velocity *= 3f;
                            Main.dust[num623].noGravity = true;
                            if (Main.rand.NextBool(2))
                            {
                                Main.dust[num623].scale = 0.5f;
                                Main.dust[num623].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }

                        for (int num623 = 0; num623 < 20; num623++)
                        {
                            int num624 = Dust.NewDust(new Vector2(player.position.X + 750f, player.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                            Main.dust[num624].noGravity = true;
                            Main.dust[num624].velocity *= 5f;
                            num624 = Dust.NewDust(new Vector2(player.position.X + 750f, player.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                            Main.dust[num624].velocity *= 2f;
                            int num625 = Dust.NewDust(new Vector2(player.position.X - 750f, player.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                            Main.dust[num625].noGravity = true;
                            Main.dust[num625].velocity *= 5f;
                            num625 = Dust.NewDust(new Vector2(player.position.X - 750f, player.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                            Main.dust[num625].velocity *= 2f;
                        }
                    }

                    NPC.ai[3] += 1f;
                    NPC.alpha = lifeToAlpha;
                    if (NPC.ai[3] >= maxTeleports)
                    {
                        NPC.ai[0] = -1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                    }
                    else
                        NPC.ai[0] = 0f;

                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                NPC.rotation = NPC.velocity.X * 0.04f;
                float playerLocation = vectorCenter.X - player.Center.X;
                NPC.direction = playerLocation < 0f ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                float divisor = expertMode ? (malice ? 10f : death ? 12f : revenge ? 15f : 20f) - (float)Math.Ceiling(5D * (1D - lifeRatio)) : 20f;
                float scytheBarrageTime = divisor * 3f;
                float scytheBarrageCooldown = divisor * 3f;

                NPC.ai[1] += 1f;
                if (NPC.ai[2] > 0f)
                    NPC.ai[2] -= 1f;
                else
                    NPC.ai[2] = scytheBarrageTime + scytheBarrageCooldown;

                if (NPC.ai[2] <= scytheBarrageTime)
                {
                    if (NPC.ai[1] % divisor == divisor - 1f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float num1070 = 15f;
                            float num1071 = player.Center.X - vectorCenter.X;
                            float num1072 = player.Center.Y - vectorCenter.Y;
                            float num1073 = (float)Math.Sqrt(num1071 * num1071 + num1072 * num1072);
                            num1073 = num1070 / num1073;
                            num1071 *= num1073;
                            num1072 *= num1073;
                            int type = ModContent.ProjectileType<SignusScythe>();
                            int damage = NPC.GetProjectileDamage(type);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), vectorCenter.X, vectorCenter.Y, num1071, num1072, type, damage, 0f, Main.myPlayer, 0f, NPC.target + 1);
                        }
                    }
                }

                float maxVelocityY = malice ? 1.5f : death ? 2.5f : 3f;
                float maxVelocityX = malice ? 5f : death ? 7f : 8f;

                if (NPC.position.Y > player.position.Y - 250f)
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= 0.9f;

                    NPC.velocity.Y -= death ? 0.12f : 0.1f;

                    if (NPC.velocity.Y > maxVelocityY)
                        NPC.velocity.Y = maxVelocityY;
                }
                else if (NPC.position.Y < player.position.Y - 350f)
                {
                    if (NPC.velocity.Y < 0f)
                        NPC.velocity.Y *= 0.9f;

                    NPC.velocity.Y += death ? 0.12f : 0.1f;

                    if (NPC.velocity.Y < -maxVelocityY)
                        NPC.velocity.Y = -maxVelocityY;
                }

                if (vectorCenter.X > player.Center.X + 600f)
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X *= 0.9f;

                    NPC.velocity.X -= death ? 0.12f : 0.1f;

                    if (NPC.velocity.X > maxVelocityX)
                        NPC.velocity.X = maxVelocityX;
                }

                if (vectorCenter.X < player.Center.X - 600f)
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X *= 0.9f;

                    NPC.velocity.X += death ? 0.12f : 0.1f;

                    if (NPC.velocity.X < -maxVelocityX)
                        NPC.velocity.X = -maxVelocityX;
                }

                if (NPC.ai[1] >= divisor * 20f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 3f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 4f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (NPC.CountNPCS(ModContent.NPCType<CosmicLantern>()) < 5)
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            int num660 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(player.position.X + spawnX), (int)(player.position.Y + spawnY), ModContent.NPCType<CosmicLantern>());
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num660, 0f, 0f, 0f, 0, 0, 0);

                            int num661 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(player.position.X - spawnX), (int)(player.position.Y + spawnY), ModContent.NPCType<CosmicLantern>());
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num661, 0f, 0f, 0f, 0, 0, 0);

                            spawnY -= 60;
                        }
                        spawnY = 120;
                    }
                }

                NPC.rotation = NPC.velocity.ToRotation();

                if (Math.Sign(NPC.velocity.X) != 0)
                    NPC.spriteDirection = -Math.Sign(NPC.velocity.X);

                if (NPC.rotation < -MathHelper.PiOver2)
                    NPC.rotation += MathHelper.Pi;
                if (NPC.rotation > MathHelper.PiOver2)
                    NPC.rotation -= MathHelper.Pi;

                NPC.spriteDirection = Math.Sign(NPC.velocity.X);

                if (calamityGlobalNPC.newAI[0] == 0f) // Line up the charge
                {
                    float velocity = malice ? 18f : revenge ? 16f : expertMode ? 15f : 14f;
                    if (expertMode)
                        velocity += death ? 6f * (float)(1D - lifeRatio) : 4f * (float)(1D - lifeRatio);

                    Vector2 vector126 = player.Center - vectorCenter;
                    Vector2 vector127 = vector126 - Vector2.UnitY * 300f;

                    float num1013 = vector126.Length();

                    vector126 = Vector2.Normalize(vector126) * velocity;
                    vector127 = Vector2.Normalize(vector127) * velocity;

                    bool flag64 = Collision.CanHit(vectorCenter, 1, 1, player.Center, 1, 1) || NPC.ai[3] >= 120f;
                    float num1014 = 8f;
                    flag64 = flag64 && vector126.ToRotation() > MathHelper.Pi / num1014 && vector126.ToRotation() < MathHelper.Pi - MathHelper.Pi / num1014;
                    if (num1013 > 1400f || !flag64)
                    {
                        NPC.velocity = (NPC.velocity * (inertia - 1f) + vector127) / inertia;

                        if (!flag64)
                        {
                            NPC.ai[3] += 1f;
                            if (NPC.ai[3] == 120f)
                                NPC.netUpdate = true;
                        }
                        else
                            NPC.ai[3] = 0f;
                    }
                    else
                    {
                        calamityGlobalNPC.newAI[0] = 1f;
                        NPC.ai[2] = vector126.X;
                        NPC.ai[3] = vector126.Y;
                        NPC.netUpdate = true;
                    }
                }
                else if (calamityGlobalNPC.newAI[0] == 1f) // Pause before charge
                {
                    NPC.velocity *= 0.8f;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= 5f)
                    {
                        calamityGlobalNPC.newAI[0] = 2f;

                        NPC.netUpdate = true;

                        Vector2 velocity = new Vector2(NPC.ai[2], NPC.ai[3]);
                        velocity.Normalize();
                        velocity *= chargeVelocity;
                        NPC.velocity = velocity;

                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                    }
                }
                else if (calamityGlobalNPC.newAI[0] == 2f) // Charging
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.ai[2] += 1f;
                        if (phase2 && NPC.ai[2] % 3f == 0f)
                        {
                            SoundEngine.PlaySound(SoundID.Item73, NPC.position);
                            int type = ModContent.ProjectileType<EssenceDust>();
                            int damage = NPC.GetProjectileDamage(type);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), vectorCenter, Vector2.Zero, type, damage, 0f, Main.myPlayer);
                        }
                    }

                    NPC.ai[1] += 1f;
                    bool flag65 = vectorCenter.Y + 50f > player.Center.Y;
                    if ((NPC.ai[1] >= 90f && flag65) || NPC.velocity.Length() < 8f)
                    {
                        calamityGlobalNPC.newAI[0] = 3f;
                        NPC.ai[1] = 30f;
                        NPC.ai[2] = 0f;
                        NPC.velocity /= 2f;
                        NPC.netUpdate = true;
                    }
                    else
                    {
                        Vector2 vec2 = player.Center - vectorCenter;
                        vec2.Normalize();
                        if (vec2.HasNaNs())
                            vec2 = new Vector2(NPC.direction, 0f);

                        NPC.velocity = (NPC.velocity * (inertia - 1f) + vec2 * (NPC.velocity.Length() + 0.111111117f * inertia)) / inertia;
                    }
                }
                else if (calamityGlobalNPC.newAI[0] == 3f) // Slow down after charging and reset
                {
                    NPC.ai[1] -= 1f;
                    if (NPC.ai[1] <= 0f)
                    {
                        NPC.TargetClosest();
                        calamityGlobalNPC.newAI[1] += 1f;
                        if (calamityGlobalNPC.newAI[1] >= maxCharges)
                        {
                            NPC.ai[0] = -1f;
                            NPC.ai[1] = 4f;
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 0f;
                            calamityGlobalNPC.newAI[1] = 0f;
                        }
                        else
                        {
                            NPC.ai[1] = 0f;
                        }
                        calamityGlobalNPC.newAI[0] = 0f;
                        NPC.netUpdate = true;
                    }
                    NPC.velocity *= 0.97f;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += NPC.IsABestiaryIconDummy ? 1.65 : 1.0;
            if (NPC.ai[0] == 4f)
            {
                if (NPC.frameCounter > 72.0) //12
                {
                    NPC.frameCounter = 0.0;
                }
            }
            else
            {
                int frameY = 196;
                if (NPC.frameCounter > 72.0)
                {
                    NPC.frameCounter = 0.0;
                }
                NPC.frame.Y = frameY * (int)(NPC.frameCounter / 12.0); //1 to 6
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D NPCTexture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowMaskTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Signus/SignusGlow").Value;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            int num153 = 5;
            Rectangle frame = NPC.frame;
            int frameCount = Main.npcFrameCount[NPC.type];

            if (NPC.ai[0] == 4f)
            {
                NPCTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Signus/SignusAlt2").Value;
                glowMaskTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Signus/SignusAlt2Glow").Value;
                num153 = 10;
                int frameY = 94 * (int)(NPC.frameCounter / 12.0);
                if (frameY >= 94 * 6)
                    frameY = 0;
                frame = new Rectangle(0, frameY, NPCTexture.Width, NPCTexture.Height / frameCount);
            }
            else if (NPC.ai[0] == 3f)
            {
                NPCTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Signus/SignusAlt").Value;
                glowMaskTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Signus/SignusAltGlow").Value;
                num153 = 7;
            }
            else
            {
                NPCTexture = TextureAssets.Npc[NPC.type].Value;
                glowMaskTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Signus/SignusGlow").Value;
            }

            Vector2 vector11 = new Vector2(NPCTexture.Width / 2, NPCTexture.Height / frameCount / 2);
            Color color36 = Color.White;
            float amount9 = 0.5f;
            float scale = NPC.scale;
            float rotation = NPC.rotation;
            float offsetY = NPC.gfxOffY;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = drawColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector41 -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * scale / 2f;
                    vector41 += vector11 * scale + new Vector2(0f, 4f + offsetY);
                    spriteBatch.Draw(NPCTexture, vector41, new Rectangle?(frame), color38, rotation, vector11, scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * scale / 2f;
            vector43 += vector11 * scale + new Vector2(0f, 4f + offsetY);
            spriteBatch.Draw(NPCTexture, vector43, new Rectangle?(frame), NPC.GetAlpha(drawColor), rotation, vector11, scale, spriteEffects, 0f);

            Color color40 = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color40;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 = NPC.GetAlpha(color41);
                    color41 *= (num153 - num163) / 15f;
                    Vector2 vector44 = NPC.oldPos[num163] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector44 -= new Vector2(glowMaskTexture.Width, glowMaskTexture.Height / frameCount) * scale / 2f;
                    vector44 += vector11 * scale + new Vector2(0f, 4f + offsetY);
                    spriteBatch.Draw(glowMaskTexture, vector44, new Rectangle?(frame), color41, rotation, vector11, scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(glowMaskTexture, vector43, new Rectangle?(frame), color40, rotation, vector11, scale, spriteEffects, 0f);

            return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<SupremeHealingPotion>();
        }

        public static bool AtFullStrength() => !DownedBossSystem.downedSignus || CalamityWorld.DoGSecondStageCountdown <= 0;

        public static bool LastSentinelKilled() => !DownedBossSystem.downedSignus && DownedBossSystem.downedStormWeaver && DownedBossSystem.downedCeaselessVoid;

        public override void OnKill()
        {
            bool fullStrength = AtFullStrength();
            if (fullStrength)
                CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // If DoG's fight is active, set the timer precisely for DoG phase 2 to spawn
            if (CalamityWorld.DoGSecondStageCountdown > 600)
            {
                CalamityWorld.DoGSecondStageCountdown = 600;
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = Mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                    netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                    netMessage.Send();
                }
            }

            // Mark Ceaseless Void as dead
            if (fullStrength)
            {
                DownedBossSystem.downedSignus = true;
                CalamityNetcode.SyncWorld();
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            var fullStrengthDrops = npcLoot.DefineConditionalDropSet(AtFullStrength);
            fullStrengthDrops.Add(ItemDropRule.BossBag(ModContent.ItemType<SignusBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            LeadingConditionRule normalOnly = new LeadingConditionRule(new Conditions.NotExpert());
            fullStrengthDrops.Add(normalOnly);
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<CosmicKunai>(),
                    ModContent.ItemType<Cosmilamp>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));

                // Materials
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<TwistingNether>(), 1, 2, 3));

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<SpectralVeil>()));

                // Vanity
                normalOnly.Add(ModContent.ItemType<SignusMask>(), 7);
                normalOnly.Add(ItemDropRule.Common(ModContent.ItemType<AncientGodSlayerHelm>(), 20).
                    OnSuccess(ItemDropRule.Common(ModContent.ItemType<AncientGodSlayerChestplate>())).
                    OnSuccess(ItemDropRule.Common(ModContent.ItemType<AncientGodSlayerLeggings>())));
            }

            fullStrengthDrops.Add(ModContent.ItemType<SignusTrophy>(), 10);

            // Lore
            npcLoot.AddConditionalPerPlayer(LastSentinelKilled, ModContent.ItemType<KnowledgeSentinels>());
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
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 200;
                NPC.height = 150;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 60; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    float randomSpread = Main.rand.Next(-200, 200) / 100;
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Signus").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Signus2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Signus3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Signus4").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Signus5").Type, 1f);
                }
            }
        }

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;

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

            return minDist <= 60f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<WhisperingDeath>(), 420, true);
        }
    }
}
