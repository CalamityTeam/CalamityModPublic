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

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Weaver");
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.GetNPCDamage();
			npc.npcSlots = 5f;
            npc.width = 74;
            npc.height = 74;
            npc.defense = 0;
            CalamityGlobalNPC global = npc.Calamity();
            global.DR = 0.999999f;
            global.unbreakableDR = true;
			bool notDoGFight = CalamityWorld.DoGSecondStageCountdown <= 0 || !CalamityWorld.downedSentinel2;
			npc.LifeMaxNERB(notDoGFight ? 65000 : 13000, notDoGFight ? 65000 : 13000, 17000);

            // If fought alone, Storm Weaver plays its own theme
            if (notDoGFight)
                music = CalamityMod.Instance.GetMusicFromMusicMod("Weaver") ?? MusicID.Boss3;
            // If fought as a DoG interlude, keep the DoG music playing
            else
                music = CalamityMod.Instance.GetMusicFromMusicMod("ScourgeofTheUniverse") ?? MusicID.Boss3;

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

			if (CalamityWorld.death || BossRushEvent.BossRushActive || CalamityWorld.malice)
				npc.scale = 1.2f;
			else if (CalamityWorld.revenge)
				npc.scale = 1.15f;
			else if (Main.expertMode)
				npc.scale = 1.1f;
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
			bool enraged = npc.Calamity().enraged > 0;
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool death = CalamityWorld.death || malice;
            if (!Main.raining && !BossRushEvent.BossRushActive && CalamityWorld.DoGSecondStageCountdown <= 0)
            {
				CalamityUtils.StartRain();
            }
            Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
            if (npc.ai[2] > 0f)
            {
                npc.realLife = (int)npc.ai[2];
            }

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

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
					int totalLength = death ? 60 : revenge ? 50 : expertMode ? 40 : 30;
					for (int num36 = 0; num36 < totalLength; num36++)
                    {
                        int lol;
                        if (num36 >= 0 && num36 < totalLength - 1)
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<StormWeaverBody>(), npc.whoAmI);
                        }
                        else
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<StormWeaverTail>(), npc.whoAmI);
                        }
                        Main.npc[lol].realLife = npc.whoAmI;
                        Main.npc[lol].ai[2] = npc.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        npc.netUpdate = true;
                        Previous = lol;
                    }
                    tail = true;
                }

				if (expertMode)
				{
					npc.localAI[0] += 1f;
					if (npc.localAI[0] >= 360f)
					{
						npc.localAI[0] = 0f;
						npc.TargetClosest();
						npc.netUpdate = true;
						float xPos = Main.rand.NextBool(2) ? Main.player[npc.target].position.X + 500f : Main.player[npc.target].position.X - 500f;
						Vector2 spawnPos = new Vector2(xPos, Main.player[npc.target].position.Y + Main.rand.Next(-500, 501));
						int type = ProjectileID.CultistBossLightningOrb;
						int damage = npc.GetProjectileDamage(type);
						Projectile.NewProjectile(spawnPos, Vector2.Zero, type, damage, 0f, Main.myPlayer, 0f, 0f);
					}
				}

                if (BoltCountdown == 0)
                {
                    BoltCountdown = malice ? 480 : 600;
                }
                if (BoltCountdown > 0)
                {
                    BoltCountdown--;
                    if (BoltCountdown == 0)
                    {
                        int speed2 = enraged ? 10 : revenge ? 8 : 7;
                        float spawnX2 = Main.rand.Next(2001) - 1000f + Main.player[npc.target].Center.X;
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
                            Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-BoltAngleSpread / 2 + (BoltAngleSpread * i / BoltProjectiles)));
                            velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
                            Vector2 vector94 = Main.player[npc.target].Center - source;
                            float ai = Main.rand.Next(100);
							int type = ProjectileID.CultistBossLightningOrbArc;
							int damage = npc.GetProjectileDamage(type);
							Projectile.NewProjectile(source, velocity, type, damage, 0f, Main.myPlayer, vector94.ToRotation(), ai);
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
                num188 = enraged ? 15f : malice ? 13f : revenge ? 11f : 10f;
                num189 = enraged ? 0.45f : malice ? 0.38f : revenge ? 0.31f : 0.28f;
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

		public override bool PreNPCLoot() => false;

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
        }
    }
}
