using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Signus
{
    [AutoloadBossHead]
    public class Signus : ModNPC
    {
        private int phaseSwitch = 0;
        private int chargeSwitch = 0;
        private int dustTimer = 3;
        private int spawnX = 750;
        private int spawnY = 120;
        private int lifeToAlpha = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Signus, Envoy of the Devourer");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 32f;
            npc.damage = 175;
            npc.width = 130;
            npc.height = 130;
            npc.defense = 70;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/ScourgeofTheUniverse");
            else
                music = MusicID.Boss4;
            npc.lifeMax = CalamityWorld.revenge ? 109500 : 70000;
            if (CalamityWorld.DoGSecondStageCountdown <= 0)
            {
                npc.value = Item.buyPrice(0, 35, 0, 0);
                if (calamityModMusic != null)
                    music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Signus");
                else
                    music = MusicID.Boss4;
                npc.lifeMax = CalamityWorld.revenge ? 445500 : 280000;
                if (CalamityWorld.death)
                {
                    npc.lifeMax = 722250;
                }
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 2600000 : 2400000;
            }
            double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.boss = true;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[ModContent.BuffType<ExoFreeze>()] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.netAlways = true;
            npc.HitSound = SoundID.NPCHit49;
            npc.DeathSound = SoundID.NPCDeath51;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(phaseSwitch);
            writer.Write(chargeSwitch);
            writer.Write(dustTimer);
            writer.Write(spawnX);
            writer.Write(spawnY);
            writer.Write(lifeToAlpha);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            phaseSwitch = reader.ReadInt32();
            chargeSwitch = reader.ReadInt32();
            dustTimer = reader.ReadInt32();
            spawnX = reader.ReadInt32();
            spawnY = reader.ReadInt32();
            lifeToAlpha = reader.ReadInt32();
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;

            double lifeRatio = (double)npc.life / (double)npc.lifeMax;
            lifeToAlpha = (int)(100.0 * (1.0 - lifeRatio));

            double mult = 1.0 -
                (revenge ? 0.25 : 0.0) -
                (CalamityWorld.death ? 0.25 : 0.0);

            bool cosmicDust = lifeToAlpha > (int)(15D * mult) || CalamityWorld.bossRushActive;
            bool speedBoost = lifeToAlpha > (int)(25D * mult) || CalamityWorld.bossRushActive;
            bool cosmicRain = lifeToAlpha > (int)(35D * mult) || CalamityWorld.bossRushActive;
            bool cosmicSpeed = lifeToAlpha > (int)(50D * mult) || CalamityWorld.bossRushActive;

            Player player = Main.player[npc.target];
            npc.TargetClosest(true);
            Vector2 vector142 = new Vector2(npc.Center.X, npc.Center.Y);
            Vector2 vectorCenter = npc.Center;
            float num1243 = player.Center.X - vector142.X;
            float num1244 = player.Center.Y - vector142.Y;
            float num1245 = (float)Math.Sqrt((double)(num1243 * num1243 + num1244 * num1244));
            float num998 = 8f;
            float scaleFactor3 = 300f;
            float num999 = 800f;
            float num1000 = cosmicSpeed ? 12f : 15f;
            float num1001 = 5f;
            float scaleFactor4 = 0.75f;
            int num1002 = 0;
            float scaleFactor5 = 10f;
            float num1003 = 30f;
            float num1004 = 150f;
            float num1005 = cosmicSpeed ? 12f : 15f;
            float num1006 = 0.333333343f;
            float num1007 = 10f;
            float chargeSpeedDivisor = cosmicSpeed ? 11.85f : 14.85f;
            num1006 *= num1005;
            if (lifeToAlpha < 50 && npc.ai[0] != 1f)
            {
                for (int num1011 = 0; num1011 < 2; num1011++)
                {
                    if (Main.rand.Next(3) < 1)
                    {
                        int num1012 = Dust.NewDust(npc.Center - new Vector2(70f), 70 * 2, 70 * 2, 173, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default, 1.5f);
                        Main.dust[num1012].noGravity = true;
                        Main.dust[num1012].velocity *= 0.2f;
                        Main.dust[num1012].fadeIn = 1f;
                    }
                }
            }
            if (Vector2.Distance(player.Center, vectorCenter) > 6400f)
            {
                CalamityWorld.DoGSecondStageCountdown = 0;
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                    netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                    netMessage.Send();
                }
                if (npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
            }
            else if (npc.timeLeft < 1800)
            {
                npc.timeLeft = 1800;
            }
            if (npc.ai[0] <= 2f)
            {
                npc.rotation = npc.velocity.X * 0.04f;
                npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
                npc.knockBackResist = 0.05f;
                if (expertMode)
                {
                    npc.knockBackResist *= Main.expertKnockBack;
                }
                if (cosmicSpeed)
                {
                    npc.knockBackResist = 0f;
                }
                float speed = expertMode ? 14f : 12f;
                if (speedBoost)
                {
                    speed = expertMode ? 16f : 14f;
                }
                if (npc.Calamity().enraged > 0 || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
                {
                    speed += 3f;
                }
                Vector2 vector98 = new Vector2(npc.Center.X, npc.Center.Y);
                float num795 = player.Center.X - vector98.X;
                float num796 = player.Center.Y - vector98.Y;
                float num797 = (float)Math.Sqrt((double)(num795 * num795 + num796 * num796));
                num797 = speed / num797;
                num795 *= num797;
                num796 *= num797;
                npc.velocity.X = (npc.velocity.X * 50f + num795) / 51f;
                npc.velocity.Y = (npc.velocity.Y * 50f + num796) / 51f;
            }
            else
            {
                npc.knockBackResist = 0f;
            }
            if (npc.ai[0] == 0f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[1] += 1f;
                    if (npc.localAI[1] >= 120f)
                    {
                        npc.localAI[1] = 0f;
                        npc.TargetClosest(true);
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
                        npc.ai[0] = 1f;
                        npc.ai[1] = (float)num1250;
                        npc.ai[2] = (float)num1251;
                        npc.netUpdate = true;
                        return;
                    }
                }
            }
            else if (npc.ai[0] == 1f)
            {
                Vector2 position = new Vector2(npc.ai[1] * 16f - (float)(npc.width / 2), npc.ai[2] * 16f - (float)(npc.height / 2));
                for (int m = 0; m < 5; m++)
                {
                    int dust = Dust.NewDust(position, npc.width, npc.height, 173, 0f, 0f, 90, default, 2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].fadeIn = 1f;
                }
                npc.alpha += 2;
                if (npc.alpha >= 255)
                {
                    Main.PlaySound(SoundID.Item8, npc.Center);
                    npc.alpha = 255;
                    npc.position = position;
                    for (int n = 0; n < 15; n++)
                    {
                        int num39 = Dust.NewDust(npc.position, npc.width, npc.height, 173, 0f, 0f, 90, default, 3f);
                        Main.dust[num39].noGravity = true;
                    }
                    npc.ai[0] = 2f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                npc.alpha -= 50;
                if (npc.alpha <= lifeToAlpha)
                {
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 122);
                    if (Main.netMode != NetmodeID.MultiplayerClient && revenge)
                    {
                        int num660 = NPC.NewNPC((int)(Main.player[npc.target].position.X + 750f), (int)Main.player[npc.target].position.Y, ModContent.NPCType<SignusBomb>(), 0, 0f, 0f, 0f, 0f, 255);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(23, -1, -1, null, num660, 0f, 0f, 0f, 0, 0, 0);
                        }
                        int num661 = NPC.NewNPC((int)(Main.player[npc.target].position.X - 750f), (int)Main.player[npc.target].position.Y, ModContent.NPCType<SignusBomb>(), 0, 0f, 0f, 0f, 0f, 255);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(23, -1, -1, null, num661, 0f, 0f, 0f, 0, 0, 0);
                        }
                        for (int num621 = 0; num621 < 5; num621++)
                        {
                            int num622 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X + 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                            Main.dust[num622].velocity *= 3f;
                            Main.dust[num622].noGravity = true;
                            if (Main.rand.NextBool(2))
                            {
                                Main.dust[num622].scale = 0.5f;
                                Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                            }
                            int num623 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X - 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                            Main.dust[num623].velocity *= 3f;
                            Main.dust[num623].noGravity = true;
                            if (Main.rand.NextBool(2))
                            {
                                Main.dust[num623].scale = 0.5f;
                                Main.dust[num623].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                            }
                        }
                        for (int num623 = 0; num623 < 20; num623++)
                        {
                            int num624 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X + 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 3f);
                            Main.dust[num624].noGravity = true;
                            Main.dust[num624].velocity *= 5f;
                            num624 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X + 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                            Main.dust[num624].velocity *= 2f;
                            int num625 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X - 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 3f);
                            Main.dust[num625].noGravity = true;
                            Main.dust[num625].velocity *= 5f;
                            num625 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X - 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                            Main.dust[num625].velocity *= 2f;
                        }
                    }
                    npc.ai[3] += 1f;
                    npc.alpha = lifeToAlpha;
                    if (npc.ai[3] >= 3f)
                    {
                        npc.ai[0] = 3f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                    }
                    else
                        npc.ai[0] = 0f;

                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                npc.rotation = npc.velocity.X * 0.04f;
                npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
                Vector2 vector121 = new Vector2(npc.position.X + (float)(npc.width / 2), npc.position.Y + (float)(npc.height / 2));
                npc.ai[1] += 1f;
                bool flag104 = false;
                if (npc.life < npc.lifeMax / 4 || CalamityWorld.death || CalamityWorld.bossRushActive)
                {
                    if (npc.ai[1] % 30f == 29f)
                    {
                        flag104 = true;
                    }
                }
                else if (npc.life < npc.lifeMax / 2)
                {
                    if (npc.ai[1] % 35f == 34f)
                    {
                        flag104 = true;
                    }
                }
                else if (npc.ai[1] % 40f == 39f)
                {
                    flag104 = true;
                }
                if (flag104)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float num1070 = 15f; //changed from 10
                        if (npc.Calamity().enraged > 0 || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
                        {
                            num1070 += 3f;
                        }
                        if (cosmicRain)
                        {
                            num1070 += 1f; //changed from 3 not a prob
                        }
                        if (cosmicSpeed)
                        {
                            num1070 += 1f;
                        }
                        if (revenge)
                        {
                            num1070 += 1f;
                        }
                        if (CalamityWorld.death || CalamityWorld.bossRushActive)
                        {
                            num1070 += 1f;
                        }
                        float num1071 = player.position.X + (float)player.width * 0.5f - vector121.X;
                        float num1072 = player.position.Y + (float)player.height * 0.5f - vector121.Y;
                        float num1073 = (float)Math.Sqrt((double)(num1071 * num1071 + num1072 * num1072));
                        num1073 = num1070 / num1073;
                        num1071 *= num1073;
                        num1072 *= num1073;
                        int num1074 = expertMode ? 48 : 60; //projectile damage
                        int num1075 = ModContent.ProjectileType<SignusScythe>(); //projectile type
                        Projectile.NewProjectile(vector121.X, vector121.Y, num1071, num1072, num1075, num1074, 0f, Main.myPlayer, 0f, (float)(npc.target + 1));
                    }
                }
                if (npc.position.Y > player.position.Y - 200f) //200
                {
                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y * 0.975f;
                    }
                    npc.velocity.Y = npc.velocity.Y - (CalamityWorld.bossRushActive ? 0.15f : 0.1f);
                    if (npc.velocity.Y > 4f)
                    {
                        npc.velocity.Y = 4f;
                    }
                }
                else if (npc.position.Y < player.position.Y - 400f) //500
                {
                    if (npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y * 0.975f;
                    }
                    npc.velocity.Y = npc.velocity.Y + (CalamityWorld.bossRushActive ? 0.15f : 0.1f);
                    if (npc.velocity.Y < -4f)
                    {
                        npc.velocity.Y = -4f;
                    }
                }
                if (npc.position.X + (float)(npc.width / 2) > player.position.X + (float)(player.width / 2) + 500f) //100
                {
                    if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X = npc.velocity.X * 0.98f;
                    }
                    npc.velocity.X = npc.velocity.X - (CalamityWorld.bossRushActive ? 0.15f : 0.1f);
                    if (npc.velocity.X > 15f)
                    {
                        npc.velocity.X = 15f;
                    }
                }
                if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)(player.width / 2) - 500f) //100
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X * 0.98f;
                    }
                    npc.velocity.X = npc.velocity.X + (CalamityWorld.bossRushActive ? 0.15f : 0.1f);
                    if (npc.velocity.X < -15f)
                    {
                        npc.velocity.X = -15f;
                    }
                }
                if (npc.ai[1] > 300f)
                {
                    npc.ai[0] = 4f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 4f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (NPC.CountNPCS(ModContent.NPCType<CosmicLantern>()) < 5)
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            int num660 = NPC.NewNPC((int)(Main.player[npc.target].position.X + (float)spawnX), (int)(Main.player[npc.target].position.Y + (float)spawnY), ModContent.NPCType<CosmicLantern>(), 0, 0f, 0f, 0f, 0f, 255);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(23, -1, -1, null, num660, 0f, 0f, 0f, 0, 0, 0);
                            }
                            int num661 = NPC.NewNPC((int)(Main.player[npc.target].position.X - (float)spawnX), (int)(Main.player[npc.target].position.Y + (float)spawnY), ModContent.NPCType<CosmicLantern>(), 0, 0f, 0f, 0f, 0f, 255);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(23, -1, -1, null, num661, 0f, 0f, 0f, 0, 0, 0);
                            }
                            spawnY -= 60;
                        }
                        spawnY = 120;
                    }
                }
                npc.TargetClosest(false);
                npc.rotation = npc.velocity.ToRotation();
                if (Math.Sign(npc.velocity.X) != 0)
                {
                    npc.spriteDirection = -Math.Sign(npc.velocity.X);
                }
                if (npc.rotation < -1.57079637f)
                {
                    npc.rotation += 3.14159274f;
                }
                if (npc.rotation > 1.57079637f)
                {
                    npc.rotation -= 3.14159274f;
                }
                npc.spriteDirection = Math.Sign(npc.velocity.X);
                phaseSwitch += 1;
                if (chargeSwitch == 0) //line up the charge
                {
                    float scaleFactor6 = num998;
                    Vector2 center4 = npc.Center;
                    Vector2 center5 = player.Center;
                    Vector2 vector126 = center5 - center4;
                    Vector2 vector127 = vector126 - Vector2.UnitY * scaleFactor3;
                    float num1013 = vector126.Length();
                    vector126 = Vector2.Normalize(vector126) * scaleFactor6;
                    vector127 = Vector2.Normalize(vector127) * scaleFactor6;
                    bool flag64 = Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1);
                    if (npc.ai[3] >= 120f)
                    {
                        flag64 = true;
                    }
                    float num1014 = 8f;
                    flag64 = flag64 && vector126.ToRotation() > 3.14159274f / num1014 && vector126.ToRotation() < 3.14159274f - 3.14159274f / num1014;
                    if (num1013 > num999 || !flag64)
                    {
                        npc.velocity.X = (npc.velocity.X * (num1000 - 1f) + vector127.X) / chargeSpeedDivisor;
                        npc.velocity.Y = (npc.velocity.Y * (num1000 - 1f) + vector127.Y) / chargeSpeedDivisor;
                        if (!flag64)
                        {
                            npc.ai[3] += 1f;
                            if (npc.ai[3] == 120f)
                            {
                                npc.netUpdate = true;
                            }
                        }
                        else
                        {
                            npc.ai[3] = 0f;
                        }
                    }
                    else
                    {
                        chargeSwitch = 1;
                        npc.ai[2] = vector126.X;
                        npc.ai[3] = vector126.Y;
                        npc.netUpdate = true;
                    }
                }
                else if (chargeSwitch == 1) //pause before charge
                {
                    npc.velocity *= scaleFactor4;
                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= num1001)
                    {
                        chargeSwitch = 2;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                        Vector2 velocity = new Vector2(npc.ai[2], npc.ai[3]) + new Vector2((float)Main.rand.Next(-num1002, num1002 + 1), (float)Main.rand.Next(-num1002, num1002 + 1)) * 0.04f;
                        velocity.Normalize();
                        velocity *= scaleFactor5;
                        npc.velocity = velocity;
                    }
                }
                else if (chargeSwitch == 2) //charging
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        dustTimer--;
                        if (cosmicDust && dustTimer <= 0)
                        {
                            Main.PlaySound(SoundID.Item73, npc.position);
                            int damage = expertMode ? 60 : 70;
                            Vector2 vector173 = Vector2.Normalize(player.Center - vectorCenter) * (float)(npc.width + 20) / 2f + vectorCenter;
                            int projectile = Projectile.NewProjectile((int)vector173.X, (int)vector173.Y, (float)(npc.direction * 2), 4f, ModContent.ProjectileType<EssenceDust>(), damage, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[projectile].timeLeft = 60;
                            Main.projectile[projectile].velocity.X = 0f;
                            Main.projectile[projectile].velocity.Y = 0f;
                            dustTimer = 3;
                        }
                    }
                    float num1016 = num1003;
                    npc.ai[1] += 1f;
                    bool flag65 = Vector2.Distance(npc.Center, player.Center) > num1004 && npc.Center.Y > player.Center.Y;
                    if ((npc.ai[1] >= num1016 && flag65) || npc.velocity.Length() < num1007)
                    {
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.velocity /= 2f;
                        npc.netUpdate = true;
                        npc.ai[1] = 45f;
                        chargeSwitch = 3;
                    }
                    else
                    {
                        Vector2 center6 = npc.Center;
                        Vector2 center7 = player.Center;
                        Vector2 vec2 = center7 - center6;
                        vec2.Normalize();
                        if (vec2.HasNaNs())
                        {
                            vec2 = new Vector2((float)npc.direction, 0f);
                        }
                        npc.velocity = (npc.velocity * (num1005 - 1f) + vec2 * (npc.velocity.Length() + num1006)) / chargeSpeedDivisor;
                    }
                }
                else if (chargeSwitch == 3) //slow down after charging and reset
                {
                    npc.ai[1] -= 2f;
                    if (npc.ai[1] <= 0f)
                    {
                        chargeSwitch = 0;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }
                    npc.velocity *= 0.97f;
                }
                if (phaseSwitch > 300)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    chargeSwitch = 0;
                    phaseSwitch = 0;
                    npc.netUpdate = true;
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 1.0;
            if (npc.ai[0] == 4f)
            {
                if (npc.frameCounter > 72.0) //12
                {
                    npc.frameCounter = 0.0;
                }
            }
            else
            {
                int frameY = 196;
                if (npc.frameCounter > 72.0)
                {
                    npc.frameCounter = 0.0;
                }
                npc.frame.Y = frameY * (int)(npc.frameCounter / 12.0); //1 to 6
                if (npc.frame.Y >= frameHeight * 6)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D NPCTexture = Main.npcTexture[npc.type];
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            int frameCount = Main.npcFrameCount[npc.type];
            float scale = npc.scale;
            float rotation = npc.rotation;
            float offsetY = npc.gfxOffY;
            if (npc.ai[0] == 4f)
            {
                NPCTexture = ModContent.GetTexture("CalamityMod/NPCs/Signus/SignusAlt2");
                int height = 564;
                int width = 176;
                Vector2 vector = new Vector2((float)(width / 2), (float)(height / frameCount / 2));
                Rectangle frame = new Rectangle(0, 0, width, height / frameCount);
                frame.Y = 94 * (int)(npc.frameCounter / 12.0); //1 to 6
                if (frame.Y >= 94 * 6)
                {
                    frame.Y = 0;
                }
                Main.spriteBatch.Draw(NPCTexture,
                    new Vector2(npc.position.X - Main.screenPosition.X + (float)(npc.width / 2) - (float)width * scale / 2f + vector.X * scale,
                    npc.position.Y - Main.screenPosition.Y + (float)npc.height - (float)height * scale / (float)frameCount + 4f + vector.Y * scale + 0f + offsetY),
                    new Microsoft.Xna.Framework.Rectangle?(frame),
                    npc.GetAlpha(drawColor),
                    rotation,
                    vector,
                    scale,
                    spriteEffects,
                    0f);
                return false;
            }
            else if (npc.ai[0] == 3f)
            {
                NPCTexture = ModContent.GetTexture("CalamityMod/NPCs/Signus/SignusAlt");
            }
            else
            {
                NPCTexture = Main.npcTexture[npc.type];
            }
            Rectangle frame2 = npc.frame;
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / frameCount / 2));
            Main.spriteBatch.Draw(NPCTexture,
                new Vector2(npc.position.X - Main.screenPosition.X + (float)(npc.width / 2) - (float)Main.npcTexture[npc.type].Width * scale / 2f + vector11.X * scale,
                npc.position.Y - Main.screenPosition.Y + (float)npc.height - (float)Main.npcTexture[npc.type].Height * scale / (float)frameCount + 4f + vector11.Y * scale + 0f + offsetY),
                new Microsoft.Xna.Framework.Rectangle?(frame2),
                npc.GetAlpha(drawColor),
                rotation,
                vector11,
                scale,
                spriteEffects,
                0f);
            return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void NPCLoot()
        {
            // Only drop items if fought alone
            if (CalamityWorld.DoGSecondStageCountdown <= 0)
            {
                // Materials
                DropHelper.DropItem(npc, ModContent.ItemType<TwistingNether>(), true, 2, 3);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<Cosmilamp>(), 3);
                DropHelper.DropItemChance(npc, ModContent.ItemType<CosmicKunai>(), 3);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<SignusTrophy>(), 10);

                // Other
                bool lastSentinelKilled = CalamityWorld.downedSentinel1 && CalamityWorld.downedSentinel2 && !CalamityWorld.downedSentinel3;
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeSentinels>(), true, lastSentinelKilled);
                DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedSentinel3, 5, 2, 1);
            }

            // If DoG's fight is active, set the timer precisely for DoG phase 2 to spawn
            if (CalamityWorld.DoGSecondStageCountdown > 600)
            {
                CalamityWorld.DoGSecondStageCountdown = 600;
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                    netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                    netMessage.Send();
                }
            }

            // Mark Signus as dead
            CalamityWorld.downedSentinel3 = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 173, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 200;
                npc.height = 150;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 60; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Signus"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Signus2"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Signus3"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Signus4"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Signus5"), 1f);
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<WhisperingDeath>(), 420, true);
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<Horror>(), 300, true);
            }
        }
    }
}
