using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.NPCs
{
    [AutoloadBossHead]
    public class Siren : ModNPC
    {
        private bool spawnedLevi = false;
        private bool secondClone = false;
        private bool forceChargeFrames = false;
        private int frameUsed = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Siren");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.damage = 70;
            npc.npcSlots = 16f;
            npc.width = 100;
            npc.height = 100;
            npc.defense = 20;
            npc.Calamity().RevPlusDR(0.05f);
			npc.LifeMaxNERD(27400, 41600, 58650, 2600000, 2800000);
            double HPBoost = Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.boss = true;
            npc.value = Item.buyPrice(0, 15, 0, 0);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[ModContent.BuffType<MarkedforDeath>()] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[BuffID.Daybreak] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<HolyFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Plague>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Siren");
            else
                music = MusicID.Boss3;
            bossBag = ModContent.ItemType<LeviathanBag>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(spawnedLevi);
            writer.Write(secondClone);
            writer.Write(forceChargeFrames);
            writer.Write(npc.localAI[0]);
            writer.Write(frameUsed);
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            spawnedLevi = reader.ReadBoolean();
            secondClone = reader.ReadBoolean();
            forceChargeFrames = reader.ReadBoolean();
            npc.localAI[0] = reader.ReadSingle();
            frameUsed = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void AI()
        {
            // whoAmI variable
            CalamityGlobalNPC.siren = npc.whoAmI;

            // Light
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0f, 0.5f, 0.3f);

            // Variables
            Player player = Main.player[npc.target];
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            Vector2 vector = npc.Center;
            Vector2 spawnAt = vector + new Vector2(0f, (float)npc.height / 2f);
            bool isNotOcean = player.position.Y < 800f || (double)player.position.Y > Main.worldSurface * 16.0 || (player.position.X > 6400f && player.position.X < (float)(Main.maxTilesX * 16 - 6400));

            // Percent life remaining
            float lifeRatio = (float)npc.life / (float)npc.lifeMax;

            // Phases
            bool phase2 = lifeRatio < 0.66f;
            bool phase3 = lifeRatio < 0.33f;

            // Check for Leviathan
            bool leviAlive = false;
            if (CalamityGlobalNPC.leviathan != -1)
                leviAlive = Main.npc[CalamityGlobalNPC.leviathan].active;

            // Spawn Leviathan and Clones, change music
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (phase2 || CalamityWorld.death || CalamityWorld.bossRushActive)
                {
                    if (!spawnedLevi)
                    {
                        if (revenge)
                            NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y - 200, ModContent.NPCType<SirenClone>());

                        Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
                        if (calamityModMusic != null)
                            music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/LeviathanAndSiren");
                        else
                            music = MusicID.Boss3;

                        NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Leviathan>());
                        spawnedLevi = true;
                    }
                }

                // Spawn Clone
                if (phase3 && revenge)
                {
                    if (!secondClone)
                    {
                        NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y - 200, ModContent.NPCType<SirenClone>());
                        secondClone = true;
                    }
                }
            }

            // Ice Shield
            if (npc.ai[3] == 0f && npc.localAI[1] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int num6 = NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<SirenIce>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                npc.ai[3] = (float)(num6 + 1);
                npc.localAI[1] = -1f;
                npc.netUpdate = true;
                Main.npc[num6].ai[0] = (float)npc.whoAmI;
                Main.npc[num6].netUpdate = true;
            }

            int num7 = (int)npc.ai[3] - 1;
            if (num7 != -1 && Main.npc[num7].active && Main.npc[num7].type == ModContent.NPCType<SirenIce>())
                npc.dontTakeDamage = true;
            else
            {
                npc.dontTakeDamage = isNotOcean && !CalamityWorld.bossRushActive;
                npc.ai[3] = 0f;

                if (npc.localAI[1] == -1f)
                    npc.localAI[1] = revenge ? 600f : 1200f;
                if (npc.localAI[1] > 0f)
                    npc.localAI[1] -= 1f;
            }

            // Alpha
            if (isNotOcean)
            {
                npc.alpha += 3;
                if (npc.alpha >= 150)
                    npc.alpha = 150;
            }
            else
            {
                npc.alpha -= 5;
                if (npc.alpha <= 0)
                    npc.alpha = 0;
            }

            // Play sound
            if (Main.rand.NextBool(300))
                Main.PlaySound(29, (int)npc.position.X, (int)npc.position.Y, 35);

            // Time left
            if (npc.timeLeft < 3000)
                npc.timeLeft = 3000;

            // Rotation when charging
            if (npc.ai[0] > 2f)
                ChargeRotation(player, vector);

            // Target
            if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
                npc.TargetClosest(true);

            // Phase switch
            else if (npc.ai[0] == -1f)
            {
                int random = phase2 ? 3 : 2;
                int num871 = Main.rand.Next(random);

                if (num871 == 0)
                    num871 = phase3 ? 3 : 0;
                else if (num871 == 1)
                    num871 = 2;
                else
                    num871 = 3;

                npc.ai[0] = (float)num871;

                if (npc.ai[0] != 3f)
                {
                    forceChargeFrames = false;
                    float playerLocation = vector.X - player.Center.X;
                    npc.direction = playerLocation < 0 ? 1 : -1;
                    npc.spriteDirection = npc.direction;
                }
                else
                    ChargeRotation(player, vector);

                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
            }

            // Get in position for bubble spawn
            else if (npc.ai[0] == 0f)
            {
                npc.TargetClosest(true);
                npc.rotation = npc.velocity.X * 0.02f;
                npc.spriteDirection = npc.direction;

                Vector2 vector118 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num1055 = player.position.X + (float)(player.width / 2) - vector118.X;
                float num1056 = player.position.Y + (float)(player.height / 2) - 200f - vector118.Y;
                float num1057 = (float)Math.Sqrt((double)(num1055 * num1055 + num1056 * num1056));

                if (num1057 < 600f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                    return;
                }

                if (npc.position.Y > player.position.Y - 350f)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y = npc.velocity.Y * 0.98f;
                    npc.velocity.Y = npc.velocity.Y - (CalamityWorld.bossRushActive ? 0.15f : 0.1f);
                    if (npc.velocity.Y > 4f)
                        npc.velocity.Y = 4f;
                }
                else if (npc.position.Y < player.position.Y - 450f)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y = npc.velocity.Y * 0.98f;
                    npc.velocity.Y = npc.velocity.Y + (CalamityWorld.bossRushActive ? 0.15f : 0.1f);
                    if (npc.velocity.Y < -4f)
                        npc.velocity.Y = -4f;
                }
                if (npc.position.X + (float)(npc.width / 2) > player.position.X + (float)(player.width / 2) + 100f)
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X = npc.velocity.X * 0.98f;
                    npc.velocity.X = npc.velocity.X - (CalamityWorld.bossRushActive ? 0.15f : 0.1f);
                    if (npc.velocity.X > 8f)
                        npc.velocity.X = 8f;
                }
                if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)(player.width / 2) - 100f)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X = npc.velocity.X * 0.98f;
                    npc.velocity.X = npc.velocity.X + (CalamityWorld.bossRushActive ? 0.15f : 0.1f);
                    if (npc.velocity.X < -8f)
                        npc.velocity.X = -8f;
                }
            }

            // Bubble spawn
            else if (npc.ai[0] == 1f)
            {
                npc.rotation = npc.velocity.X * 0.02f;
                npc.TargetClosest(true);

                Vector2 vector119 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);
                Vector2 vector120 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num1058 = player.position.X + (float)(player.width / 2) - vector120.X;
                float num1059 = player.position.Y + (float)(player.height / 2) - vector120.Y;
                float num1060 = (float)Math.Sqrt((double)(num1058 * num1058 + num1059 * num1059));

                npc.ai[1] += 1f;
                if (phase2 || CalamityWorld.bossRushActive)
                    npc.ai[1] += 0.25f;
                if (phase3 || CalamityWorld.bossRushActive)
                    npc.ai[1] += 0.25f;

                bool flag103 = false;
                if (npc.ai[1] > 20f)
                {
                    npc.ai[1] = 0f;
                    npc.ai[2] += 1f;
                    flag103 = true;
                }

                if (Collision.CanHit(vector119, 1, 1, player.position, player.width, player.height) && flag103)
                {
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 85);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int num1061 = 371;
                        int num1062 = NPC.NewNPC((int)vector119.X, (int)vector119.Y - 30, num1061, 0, 0f, 0f, 0f, 0f, 255);
                        Main.npc[num1062].velocity.X = (float)Main.rand.Next(-200, 201) * (CalamityWorld.bossRushActive ? 0.02f : 0.01f);
                        Main.npc[num1062].velocity.Y = (float)Main.rand.Next(-200, 201) * (CalamityWorld.bossRushActive ? 0.02f : 0.01f);
                        Main.npc[num1062].localAI[0] = 60f;
                        Main.npc[num1062].netUpdate = true;
                    }
                }

                if (num1060 > 600f)
                {
                    if (npc.position.Y > player.position.Y - 350f)
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y = npc.velocity.Y * 0.98f;
                        npc.velocity.Y = npc.velocity.Y - (CalamityWorld.bossRushActive ? 0.15f : 0.1f);
                        if (npc.velocity.Y > 4f)
                            npc.velocity.Y = 4f;
                    }
                    else if (npc.position.Y < player.position.Y - 450f)
                    {
                        if (npc.velocity.Y < 0f)
                            npc.velocity.Y = npc.velocity.Y * 0.98f;
                        npc.velocity.Y = npc.velocity.Y + (CalamityWorld.bossRushActive ? 0.15f : 0.1f);
                        if (npc.velocity.Y < -4f)
                            npc.velocity.Y = -4f;
                    }
                    if (npc.position.X + (float)(npc.width / 2) > player.position.X + (float)(player.width / 2) + 100f)
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X = npc.velocity.X * 0.98f;
                        npc.velocity.X = npc.velocity.X - (CalamityWorld.bossRushActive ? 0.15f : 0.1f);
                        if (npc.velocity.X > 8f)
                            npc.velocity.X = 8f;
                    }
                    if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)(player.width / 2) - 100f)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X = npc.velocity.X * 0.98f;
                        npc.velocity.X = npc.velocity.X + (CalamityWorld.bossRushActive ? 0.15f : 0.1f);
                        if (npc.velocity.X < -8f)
                            npc.velocity.X = -8f;
                    }
                }
                else
                    npc.velocity *= 0.9f;

                npc.spriteDirection = npc.direction;

                if (npc.ai[2] > 4f)
                {
                    npc.ai[0] = -1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }

            // Float around and fire projectiles
            else if (npc.ai[0] == 2f)
            {
                npc.rotation = npc.velocity.X * 0.02f;
                Vector2 vector121 = new Vector2(npc.position.X + (float)(npc.width / 2), npc.position.Y + (float)(npc.height / 2));

                npc.ai[1] += 1f;
                bool flag104 = false;
                if (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
                {
                    if (npc.ai[1] % 10f == 9f)
                        flag104 = true;
                }
                else
                {
                    if (((phase3 || CalamityWorld.death) && !leviAlive) || CalamityWorld.bossRushActive)
                    {
                        if (npc.ai[1] % 20f == 19f)
                            flag104 = true;
                    }
                    else if (phase2)
                    {
                        if (npc.ai[1] % 25f == 24f)
                            flag104 = true;
                    }
                    else if (npc.ai[1] % 30f == 29f)
                        flag104 = true;
                }

                if (flag104 && (npc.position.Y + (float)npc.height < player.position.Y || (!leviAlive && phase2)) && Collision.CanHit(vector121, 1, 1, player.position, player.width, player.height))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float num1070 = revenge ? 15f : 13f;
                        if (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
                            num1070 = 24f;
                        else if (isNotOcean || (!leviAlive && phase2) || CalamityWorld.death || CalamityWorld.bossRushActive)
                            num1070 = revenge ? 17f : 16f;
                        else
                        {
                            if (phase3)
                                num1070 += 2f;
                        }

                        float num1071 = player.position.X + (float)player.width * 0.5f - vector121.X;
                        float num1072 = player.position.Y + (float)player.height * 0.5f - vector121.Y;
                        float num1073 = (float)Math.Sqrt((double)(num1071 * num1071 + num1072 * num1072));
                        num1073 = num1070 / num1073;
                        num1071 *= num1073;
                        num1072 *= num1073;

                        int num1074 = expertMode ? 26 : 32;
                        int num1075 = ModContent.ProjectileType<WaterSpear>();
                        switch (Main.rand.Next(6))
                        {
                            case 0:
                                num1075 = ModContent.ProjectileType<SirenSong>();
                                break;
                            case 1:
                                num1075 = ModContent.ProjectileType<FrostMist>();
                                break;
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                                num1075 = ModContent.ProjectileType<WaterSpear>();
                                break;
                        }
                        if (isNotOcean)
                            num1074 *= 2;

                        Projectile.NewProjectile(vector121.X, vector121.Y, num1071, num1072, num1075, num1074, 0f, Main.myPlayer, 0f, 0f);
                    }
                }

                if (npc.position.Y > player.position.Y - 300f)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y = npc.velocity.Y * 0.98f;
                    npc.velocity.Y = npc.velocity.Y - (CalamityWorld.bossRushActive ? 0.15f : 0.1f);
                    if (npc.velocity.Y > 4f)
                        npc.velocity.Y = 4f;
                }
                else if (npc.position.Y < player.position.Y - 500f)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y = npc.velocity.Y * 0.98f;
                    npc.velocity.Y = npc.velocity.Y + (CalamityWorld.bossRushActive ? 0.15f : 0.1f);
                    if (npc.velocity.Y < -4f)
                        npc.velocity.Y = -4f;
                }
                if (npc.position.X + (float)(npc.width / 2) > player.position.X + (float)(player.width / 2) + 100f)
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X = npc.velocity.X * 0.98f;
                    npc.velocity.X = npc.velocity.X - (CalamityWorld.bossRushActive ? 0.15f : 0.1f);
                    if (npc.velocity.X > 8f)
                        npc.velocity.X = 8f;
                }
                if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)(player.width / 2) - 100f)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X = npc.velocity.X * 0.98f;
                    npc.velocity.X = npc.velocity.X + (CalamityWorld.bossRushActive ? 0.15f : 0.1f);
                    if (npc.velocity.X < -8f)
                        npc.velocity.X = -8f;
                }

                float playerLocation = vector.X - player.Center.X;
                npc.direction = playerLocation < 0 ? 1 : -1;
                npc.spriteDirection = npc.direction;

                if (npc.ai[1] >= 300f)
                {
                    npc.ai[0] = -1f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
            }

            // Set up charge
            else if (npc.ai[0] == 3f)
            {
                ChargeLocation(player, vector, leviAlive, expertMode);

                npc.ai[1] += 1f;

                if (npc.ai[1] >= (expertMode ? 40f : 60f))
                {
                    forceChargeFrames = true;
                    npc.ai[0] = npc.ai[2] >= (leviAlive ? 2f : 3f) ? -1f : 4f;
                    npc.ai[1] = 0f;
                    npc.localAI[0] = 0f;

                    // Velocity and rotation
                    float chargeVelocity = leviAlive ? 16f : 21f;
                    if (CalamityWorld.bossRushActive)
                        chargeVelocity = 26f;

                    npc.velocity = Vector2.Normalize(player.Center - vector) * chargeVelocity;
                    npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);

                    // Direction
                    int num22 = Math.Sign(player.Center.X - vector.X);
                    if (num22 != 0)
                    {
                        npc.direction = num22;

                        if (npc.spriteDirection == 1)
                            npc.rotation += 3.14159274f;

                        npc.spriteDirection = -npc.direction;
                    }
                }
            }

            // Charge
            else if (npc.ai[0] == 4f)
            {
                // Spawn dust
                int num24 = 7;
                for (int j = 0; j < num24; j++)
                {
                    Vector2 arg_E1C_0 = (Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(j - (num24 / 2 - 1)) * 3.1415926535897931 / (double)(float)num24, default) + vector;
                    Vector2 vector4 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                    int num25 = Dust.NewDust(arg_E1C_0 + vector4, 0, 0, 172, vector4.X * 2f, vector4.Y * 2f, 100, default, 1.4f);
                    Main.dust[num25].noGravity = true;
                    Main.dust[num25].noLight = true;
                    Main.dust[num25].velocity /= 4f;
                    Main.dust[num25].velocity -= npc.velocity;
                }

                npc.ai[1] += 1f;
                if (npc.ai[1] >= 30f)
                {
                    npc.ai[0] = 3f;
                    npc.ai[1] = 0f;
                    npc.ai[2] += 1f;
                    npc.netUpdate = true;
                }
            }

            // Despawn
            if (player.dead || Vector2.Distance(player.Center, vector) > 5600f)
            {
                if (npc.localAI[3] < 120f)
                    npc.localAI[3] += 1f;

                if (npc.localAI[3] > 60f)
                {
                    npc.velocity.Y = npc.velocity.Y + (npc.localAI[3] - 60f) * 0.25f;

                    if ((double)npc.position.Y > Main.rockLayer * 16.0)
                    {
                        for (int x = 0; x < 200; x++)
                        {
                            if (Main.npc[x].type == ModContent.NPCType<Leviathan>())
                            {
                                Main.npc[x].active = false;
                                Main.npc[x].netUpdate = true;
                            }
                        }

                        npc.active = false;
                        npc.netUpdate = true;
                    }
                }
                return;
            }
            if (npc.localAI[3] > 0f)
                npc.localAI[3] -= 1f;
        }

        // Rotation when charging
        private void ChargeRotation(Player player, Vector2 vector)
        {
            float num17 = (float)Math.Atan2((double)(player.Center.Y - vector.Y), (double)(player.Center.X - vector.X));
            if (npc.spriteDirection == 1)
                num17 += 3.14159274f;
            if (num17 < 0f)
                num17 += 6.28318548f;
            if (num17 > 6.28318548f)
                num17 -= 6.28318548f;

            float num18 = 0.04f;
            if (npc.ai[0] == 4f)
                num18 = 0f;

            if (npc.rotation < num17)
            {
                if ((double)(num17 - npc.rotation) > 3.1415926535897931)
                    npc.rotation -= num18;
                else
                    npc.rotation += num18;
            }
            if (npc.rotation > num17)
            {
                if ((double)(npc.rotation - num17) > 3.1415926535897931)
                    npc.rotation += num18;
                else
                    npc.rotation -= num18;
            }

            if (npc.rotation > num17 - num18 && npc.rotation < num17 + num18)
                npc.rotation = num17;
            if (npc.rotation < 0f)
                npc.rotation += 6.28318548f;
            if (npc.rotation > 6.28318548f)
                npc.rotation -= 6.28318548f;
            if (npc.rotation > num17 - num18 && npc.rotation < num17 + num18)
                npc.rotation = num17;
        }

        // Move to charge location
        private void ChargeLocation(Player player, Vector2 vector, bool leviAlive, bool expertMode)
        {
            float distance = leviAlive ? 400f : 350f;

            // Velocity
            if (npc.localAI[0] == 0f)
                npc.localAI[0] = (float)((int)distance * Math.Sign((vector - player.Center).X));

            Vector2 vector3 = Vector2.Normalize(player.Center + new Vector2(npc.localAI[0], -distance) - vector - npc.velocity) * 9f;
            float acceleration = expertMode ? 0.75f : 0.5f;
            if (CalamityWorld.bossRushActive)
                acceleration = 1f;

            if (npc.velocity.X < vector3.X)
            {
                npc.velocity.X = npc.velocity.X + acceleration;
                if (npc.velocity.X < 0f && vector3.X > 0f)
                    npc.velocity.X = npc.velocity.X + acceleration;
            }
            else if (npc.velocity.X > vector3.X)
            {
                npc.velocity.X = npc.velocity.X - acceleration;
                if (npc.velocity.X > 0f && vector3.X < 0f)
                    npc.velocity.X = npc.velocity.X - acceleration;
            }
            if (npc.velocity.Y < vector3.Y)
            {
                npc.velocity.Y = npc.velocity.Y + acceleration;
                if (npc.velocity.Y < 0f && vector3.Y > 0f)
                    npc.velocity.Y = npc.velocity.Y + acceleration;
            }
            else if (npc.velocity.Y > vector3.Y)
            {
                npc.velocity.Y = npc.velocity.Y - acceleration;
                if (npc.velocity.Y > 0f && vector3.Y < 0f)
                    npc.velocity.Y = npc.velocity.Y - acceleration;
            }

            // Rotation
            int num22 = Math.Sign(player.Center.X - vector.X);
            if (num22 != 0)
            {
                if (npc.ai[1] == 0f && num22 != npc.direction)
                    npc.rotation += 3.14159274f;

                npc.direction = num22;

                if (npc.spriteDirection != -npc.direction)
                    npc.rotation += 3.14159274f;

                npc.spriteDirection = -npc.direction;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);

            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SirenGores/Siren"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SirenGores/Siren2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SirenGores/Siren3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SirenGores/Siren4"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SirenGores/Siren5"), 1f);

                for (int k = 0; k < 50; k++)
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor) // 2 total states (ice shield or no ice shield)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            Texture2D texture = Main.npcTexture[npc.type];
            if (npc.dontTakeDamage)
            {
                switch (frameUsed)
                {
                    case 0:
                        texture = mod.GetTexture("NPCs/Leviathan/SirenAlt");
                        break;
                    case 1:
                        texture = mod.GetTexture("NPCs/Leviathan/SirenAltSinging");
                        break;
                    case 2:
                        texture = mod.GetTexture("NPCs/Leviathan/SirenAltStabbing");
                        break;
                }
            }
            else
            {
                switch (frameUsed)
                {
                    case 0:
                        texture = Main.npcTexture[npc.type];
                        break;
                    case 1:
                        texture = mod.GetTexture("NPCs/Leviathan/SirenSinging");
                        break;
                    case 2:
                        texture = mod.GetTexture("NPCs/Leviathan/SirenStabbing");
                        break;
                }
            }

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            if (npc.ai[0] > 2f || forceChargeFrames)
            {
                int width = 106;
                Vector2 vector = new Vector2((float)(width / 2), (float)(texture.Height / Main.npcFrameCount[npc.type] / 2));
                Microsoft.Xna.Framework.Rectangle frame = new Rectangle(0, 0, width, texture.Height / Main.npcFrameCount[npc.type]);
                frame.Y = 146 * (int)(npc.frameCounter / 12.0); // 1 to 6
                if (frame.Y >= 146 * 6 || npc.ai[0] == 4f)
                    frame.Y = 0;

                Main.spriteBatch.Draw(texture,
                    new Vector2(npc.position.X - Main.screenPosition.X + (float)(npc.width / 2) - (float)width * npc.scale / 2f + vector.X * npc.scale,
                    npc.position.Y - Main.screenPosition.Y + (float)npc.height - (float)texture.Height * npc.scale / (float)Main.npcFrameCount[npc.type] + 4f + vector.Y * npc.scale + 0f + npc.gfxOffY),
                    new Microsoft.Xna.Framework.Rectangle?(frame),
                    npc.GetAlpha(drawColor),
                    npc.rotation,
                    vector,
                    npc.scale,
                    spriteEffects,
                    0f);
                return false;
            }

            Microsoft.Xna.Framework.Rectangle frame2 = npc.frame;
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
            Main.spriteBatch.Draw(texture,
                new Vector2(npc.position.X - Main.screenPosition.X + (float)(npc.width / 2) - (float)Main.npcTexture[npc.type].Width * npc.scale / 2f + vector11.X * npc.scale,
                npc.position.Y - Main.screenPosition.Y + (float)npc.height - (float)Main.npcTexture[npc.type].Height * npc.scale / (float)Main.npcFrameCount[npc.type] + 4f + vector11.Y * npc.scale + 0f + npc.gfxOffY),
                new Microsoft.Xna.Framework.Rectangle?(frame2),
                npc.GetAlpha(drawColor),
                npc.rotation,
                vector11,
                npc.scale,
                spriteEffects,
                0f);
            return false;
        }

        public override void FindFrame(int frameHeight) // 6 total frames, 3 total texture types
        {
            if (forceChargeFrames)
                frameUsed = 2;
            else if (npc.ai[0] == 2f)
                frameUsed = 0;
            else if (npc.ai[0] <= 1f)
                frameUsed = 1;
            else
                frameUsed = 2;

            npc.frameCounter += 1.0;
            if (npc.ai[0] == 3f || forceChargeFrames)
            {
                if (npc.frameCounter > 72.0)
                    npc.frameCounter = 0.0;
            }
            else if (npc.ai[0] != 4f)
            {
                int frameY = 146;
                if (npc.frameCounter > 72.0)
                    npc.frameCounter = 0.0;

                npc.frame.Y = frameY * (int)(npc.frameCounter / 12.0);
                if (npc.frame.Y >= frameHeight * 6)
                    npc.frame.Y = 0;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        // Anahita runs the same loot code as the Leviathan, but only if she dies last.
        public override void NPCLoot()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<Leviathan>()))
                Leviathan.DropSirenLeviLoot(npc);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Wet, 120, true);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }
    }
}
