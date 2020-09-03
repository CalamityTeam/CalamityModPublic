using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.StormWeaver
{
    [AutoloadBossHead]
    public class StormWeaverHead : ModNPC
    {
        private const float BoltAngleSpread = 170;
        private int BoltCountdown = 0;
        private const float speed = 10f;
        private const float turnSpeed = 0.3f;
        private bool tail = false;
        private const int minLength = 30;
        private const int maxLength = 31;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Weaver");
        }

        public override void SetDefaults()
        {
			npc.GetNPCDamage();
			npc.npcSlots = 5f;
            npc.width = 74;
            npc.height = 74;
            npc.defense = 0;
            CalamityGlobalNPC global = npc.Calamity();
            global.DR = 0.999999f;
            global.unbreakableDR = true;
			bool notDoGFight = CalamityWorld.DoGSecondStageCountdown <= 0 || !CalamityWorld.downedSentinel2;
			npc.LifeMaxNERB(notDoGFight ? 100000 : 20000, notDoGFight ? 100000 : 20000, 170000);
			Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/ScourgeofTheUniverse");
            else
                music = MusicID.Boss3;
            if (notDoGFight)
            {
                if (calamityModMusic != null)
                    music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Weaver");
                else
                    music = MusicID.Boss3;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.boss = true;
            npc.alpha = 255;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.chaseable = false;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(BoltCountdown);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BoltCountdown = reader.ReadInt32();
        }

        public override void AI()
        {
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            if (npc.defense < 99999 && (CalamityWorld.DoGSecondStageCountdown <= 0 || !CalamityWorld.downedSentinel2))
            {
                npc.defense = 99999;
            }
            else
            {
                npc.defense = 0;
            }
            if (!Main.raining && !BossRushEvent.BossRushActive && CalamityWorld.DoGSecondStageCountdown <= 0)
            {
				CalamityUtils.StartRain();
            }
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
            if (npc.ai[3] > 0f)
            {
                npc.realLife = (int)npc.ai[3];
            }
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
            npc.velocity.Length();
            if (npc.alpha != 0)
            {
                for (int num934 = 0; num934 < 2; num934++)
                {
                    int num935 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
                    Main.dust[num935].noGravity = true;
                    Main.dust[num935].noLight = true;
                }
            }
            npc.alpha -= 12;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!tail && npc.ai[0] == 0f)
                {
                    int Previous = npc.whoAmI;
                    for (int num36 = 0; num36 < maxLength; num36++)
                    {
                        int lol;
                        if (num36 >= 0 && num36 < minLength)
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<StormWeaverBody>(), npc.whoAmI);
                        }
                        else
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<StormWeaverTail>(), npc.whoAmI);
                        }
                        Main.npc[lol].realLife = npc.whoAmI;
                        Main.npc[lol].ai[2] = (float)npc.whoAmI;
                        Main.npc[lol].ai[1] = (float)Previous;
                        Main.npc[Previous].ai[0] = (float)lol;
                        npc.netUpdate = true;
                        Previous = lol;
                    }
                    tail = true;
                }

                int damage = expertMode ? 62 : 75;
				if (expertMode)
				{
					npc.localAI[0] += 1f;
					if (npc.localAI[0] >= 360f)
					{
						npc.localAI[0] = 0f;
						npc.TargetClosest(true);
						npc.netUpdate = true;
						float xPos = Main.rand.NextBool(2) ? Main.player[npc.target].position.X + 500f : Main.player[npc.target].position.X - 500f;
						Vector2 spawnPos = new Vector2(xPos, Main.player[npc.target].position.Y + Main.rand.Next(-500, 501));
						Projectile.NewProjectile(spawnPos, Vector2.Zero, ProjectileID.CultistBossLightningOrb, damage, 0f, Main.myPlayer, 0f, 0f);
					}
				}

                if (BoltCountdown == 0)
                {
                    BoltCountdown = 600;
                }
                if (BoltCountdown > 0)
                {
                    BoltCountdown--;
                    if (BoltCountdown == 0)
                    {
                        int speed2 = revenge ? 8 : 7;
                        if (npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && BossRushEvent.BossRushActive))
                        {
                            speed2 += 1;
                        }
                        float spawnX2 = (float)Main.rand.Next(2001) - 1000f + Main.player[npc.target].Center.X;
                        float spawnY2 = -1000f + Main.player[npc.target].Center.Y;
                        Vector2 baseSpawn = new Vector2(spawnX2, spawnY2);
                        Vector2 baseVelocity = Main.player[npc.target].Center - baseSpawn;
                        baseVelocity.Normalize();
                        baseVelocity *= speed2;
						int BoltProjectiles = 1;
						for (int i = 0; i < BoltProjectiles; i++)
                        {
                            Vector2 source = baseSpawn;
                            source.X += i * 30 - (BoltProjectiles * 15);
                            Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-BoltAngleSpread / 2 + (BoltAngleSpread * i / (float)BoltProjectiles)));
                            velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
                            Vector2 vector94 = Main.player[npc.target].Center - source;
                            float ai = (float)Main.rand.Next(100);
                            Projectile.NewProjectile(source, velocity, ProjectileID.CultistBossLightningOrbArc, damage, 0f, Main.myPlayer, vector94.ToRotation(), ai);
                        }
                    }
                }
            }
            if (Main.player[npc.target].dead)
            {
                npc.TargetClosest(false);
                npc.velocity.Y = npc.velocity.Y - 10f;
                if ((double)npc.position.Y < Main.topWorld + 16f)
                {
                    npc.velocity.Y = npc.velocity.Y - 10f;
                }
                if ((double)npc.position.Y < Main.topWorld + 16f)
                {
                    CalamityWorld.DoGSecondStageCountdown = 0;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var netMessage = mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                        netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                        netMessage.Send();
                    }
                    for (int num957 = 0; num957 < Main.maxNPCs; num957++)
                    {
                        if (Main.npc[num957].active && (Main.npc[num957].type == ModContent.NPCType<StormWeaverBody>()
                            || Main.npc[num957].type == ModContent.NPCType<StormWeaverHead>()
                            || Main.npc[num957].type == ModContent.NPCType<StormWeaverTail>()))
                        {
                            Main.npc[num957].active = false;
                        }
                    }
                }
            }
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 10000f)
            {
                CalamityWorld.DoGSecondStageCountdown = 0;
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                    netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                    netMessage.Send();
                }
                for (int num957 = 0; num957 < Main.maxNPCs; num957++)
                {
                    if (Main.npc[num957].active && (Main.npc[num957].type == ModContent.NPCType<StormWeaverBody>()
                            || Main.npc[num957].type == ModContent.NPCType<StormWeaverHead>()
                            || Main.npc[num957].type == ModContent.NPCType<StormWeaverTail>()))
                    {
                        Main.npc[num957].active = false;
                    }
                }
            }
            if (npc.velocity.X < 0f)
            {
                npc.spriteDirection = -1;
            }
            else if (npc.velocity.X > 0f)
            {
                npc.spriteDirection = 1;
            }
            if (Main.player[npc.target].dead)
            {
                npc.TargetClosest(false);
            }
            float num188 = speed;
            float num189 = turnSpeed;
            Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num191 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
            float num192 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
            int num42 = -1;
            int num43 = (int)(Main.player[npc.target].Center.X / 16f);
            int num44 = (int)(Main.player[npc.target].Center.Y / 16f);
            for (int num45 = num43 - 2; num45 <= num43 + 2; num45++)
            {
                for (int num46 = num44; num46 <= num44 + 15; num46++)
                {
                    if (WorldGen.SolidTile2(num45, num46))
                    {
                        num42 = num46;
                        break;
                    }
                }
                if (num42 > 0)
                {
                    break;
                }
            }
            if (num42 > 0)
            {
                num42 *= 16;
                float num47 = (float)(num42 - 800);
                if (Main.player[npc.target].position.Y > num47)
                {
                    num192 = num47;
                    if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < 500f)
                    {
                        if (npc.velocity.X > 0f)
                        {
                            num191 = Main.player[npc.target].Center.X + 600f;
                        }
                        else
                        {
                            num191 = Main.player[npc.target].Center.X - 600f;
                        }
                    }
                }
            }
            else
            {
                num188 = revenge ? 11f : 10f;
                num189 = revenge ? 0.31f : 0.28f;
            }
            float num48 = num188 * 1.3f;
            float num49 = num188 * 0.7f;
            float num50 = npc.velocity.Length();
            if (num50 > 0f)
            {
                if (num50 > num48)
                {
                    npc.velocity.Normalize();
                    npc.velocity *= num48;
                }
                else if (num50 < num49)
                {
                    npc.velocity.Normalize();
                    npc.velocity *= num49;
                }
            }
            if (num42 > 0)
            {
                for (int num51 = 0; num51 < Main.maxNPCs; num51++)
                {
                    if (Main.npc[num51].active && Main.npc[num51].type == npc.type && num51 != npc.whoAmI)
                    {
                        Vector2 vector3 = Main.npc[num51].Center - npc.Center;
                        if (vector3.Length() < 400f)
                        {
                            vector3.Normalize();
                            vector3 *= 1000f;
                            num191 -= vector3.X;
                            num192 -= vector3.Y;
                        }
                    }
                }
            }
            else
            {
                for (int num52 = 0; num52 < Main.maxNPCs; num52++)
                {
                    if (Main.npc[num52].active && Main.npc[num52].type == npc.type && num52 != npc.whoAmI)
                    {
                        Vector2 vector4 = Main.npc[num52].Center - npc.Center;
                        if (vector4.Length() < 60f)
                        {
                            vector4.Normalize();
                            vector4 *= 200f;
                            num191 -= vector4.X;
                            num192 -= vector4.Y;
                        }
                    }
                }
            }
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            float num196 = System.Math.Abs(num191);
            float num197 = System.Math.Abs(num192);
            float num198 = num188 / num193;
            num191 *= num198;
            num192 *= num198;
            if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
            {
                if (npc.velocity.X < num191)
                {
                    npc.velocity.X = npc.velocity.X + num189;
                }
                else
                {
                    if (npc.velocity.X > num191)
                    {
                        npc.velocity.X = npc.velocity.X - num189;
                    }
                }
                if (npc.velocity.Y < num192)
                {
                    npc.velocity.Y = npc.velocity.Y + num189;
                }
                else
                {
                    if (npc.velocity.Y > num192)
                    {
                        npc.velocity.Y = npc.velocity.Y - num189;
                    }
                }
                if ((double)System.Math.Abs(num192) < (double)num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
                {
                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y + num189 * 2f;
                    }
                    else
                    {
                        npc.velocity.Y = npc.velocity.Y - num189 * 2f;
                    }
                }
                if ((double)System.Math.Abs(num191) < (double)num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
                {
                    if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X = npc.velocity.X + num189 * 2f; //changed from 2
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X - num189 * 2f; //changed from 2
                    }
                }
            }
            else
            {
                if (num196 > num197)
                {
                    if (npc.velocity.X < num191)
                    {
                        npc.velocity.X = npc.velocity.X + num189 * 1.1f; //changed from 1.1
                    }
                    else if (npc.velocity.X > num191)
                    {
                        npc.velocity.X = npc.velocity.X - num189 * 1.1f; //changed from 1.1
                    }
                    if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
                    {
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num189;
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y - num189;
                        }
                    }
                }
                else
                {
                    if (npc.velocity.Y < num192)
                    {
                        npc.velocity.Y = npc.velocity.Y + num189 * 1.1f;
                    }
                    else if (npc.velocity.Y > num192)
                    {
                        npc.velocity.Y = npc.velocity.Y - num189 * 1.1f;
                    }
                    if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
                    {
                        if (npc.velocity.X > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num189;
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X - num189;
                        }
                    }
                }
            }
            npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + MathHelper.PiOver2;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SWArmorHead1"), 1f);
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 30;
                npc.height = 30;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 20; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 40; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override bool CheckDead()
        {
            for (int num957 = 0; num957 < Main.maxNPCs; num957++)
            {
                if (Main.npc[num957].active && (Main.npc[num957].type == ModContent.NPCType<StormWeaverBody>() || Main.npc[num957].type == ModContent.NPCType<StormWeaverTail>()))
                {
                    Main.npc[num957].life = 0;
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.SpawnOnPlayer(Main.LocalPlayer.whoAmI, ModContent.NPCType<StormWeaverHeadNaked>());
            }
            return true;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
        }
    }
}
