
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Buffs;
using CalamityMod.Items;
namespace CalamityMod.NPCs
{
    [AutoloadBossHead]
    public class RavagerBody : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ravager");
            Main.npcFrameCount[npc.type] = 7;
        }

        public override void SetDefaults()
        {
            npc.lavaImmune = true;
            npc.npcSlots = 20f;
            npc.aiStyle = -1;
            npc.damage = 120;
            npc.width = 332;
            npc.height = 214;
            npc.defense = 55;
			npc.value = Item.buyPrice(0, 25, 0, 0);
			npc.Calamity().RevPlusDR(0.4f);
			npc.LifeMaxNERD(42700, 53500, 90000, 4600000, 4800000);
			if (CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive)
			{
				npc.damage *= 2;
				npc.defense *= 2;
				npc.lifeMax *= 7;
				npc.value *= 1.5f;
			}
			double HPBoost = Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)(npc.lifeMax * HPBoost);
			npc.knockBackResist = 0f;
            aiType = -1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[BuffID.Daybreak] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<HolyFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.boss = true;
            npc.alpha = 255;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath14;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Ravager");
            else
                music = MusicID.Boss4;
            bossBag = ModContent.ItemType<RavagerBag>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            bool provy = CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;

            // Percent life remaining
            float lifeRatio = (float)npc.life / (float)npc.lifeMax;

            // Large fire light
            Lighting.AddLight((int)(npc.Center.X - 110f) / 16, (int)(npc.Center.Y - 30f) / 16, 0f, 0.5f, 2f);
            Lighting.AddLight((int)(npc.Center.X + 110f) / 16, (int)(npc.Center.Y - 30f) / 16, 0f, 0.5f, 2f);

            // Small fire light
            Lighting.AddLight((int)(npc.Center.X - 40f) / 16, (int)(npc.Center.Y - 60f) / 16, 0f, 0.25f, 1f);
            Lighting.AddLight((int)(npc.Center.X + 40f) / 16, (int)(npc.Center.Y - 60f) / 16, 0f, 0.25f, 1f);

            CalamityGlobalNPC.scavenger = npc.whoAmI;

            if (npc.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] = 1f;
                NPC.NewNPC((int)npc.Center.X - 70, (int)npc.Center.Y + 88, ModContent.NPCType<RavagerLegLeft>(), 0, 0f, 0f, 0f, 0f, 255);
                NPC.NewNPC((int)npc.Center.X + 70, (int)npc.Center.Y + 88, ModContent.NPCType<RavagerLegRight>(), 0, 0f, 0f, 0f, 0f, 255);
                NPC.NewNPC((int)npc.Center.X - 120, (int)npc.Center.Y + 50, ModContent.NPCType<RavagerClawLeft>(), 0, 0f, 0f, 0f, 0f, 255);
                NPC.NewNPC((int)npc.Center.X + 120, (int)npc.Center.Y + 50, ModContent.NPCType<RavagerClawRight>(), 0, 0f, 0f, 0f, 0f, 255);
                NPC.NewNPC((int)npc.Center.X + 1, (int)npc.Center.Y - 20, ModContent.NPCType<RavagerHead>(), 0, 0f, 0f, 0f, 0f, 255);
            }

            if (npc.target >= 0 && Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
                if (Main.player[npc.target].dead)
                    npc.noTileCollide = true;
            }

            if (npc.alpha > 0)
            {
                npc.alpha -= 10;
                if (npc.alpha < 0)
                    npc.alpha = 0;

                npc.ai[1] = 0f;
            }

            bool leftLegActive = false;
            bool rightLegActive = false;
            bool headActive = false;
            bool rightClawActive = false;
            bool leftClawActive = false;

            for (int num619 = 0; num619 < 200; num619++)
            {
                if (Main.npc[num619].active && Main.npc[num619].type == ModContent.NPCType<RavagerHead>())
                    headActive = true;
                if (Main.npc[num619].active && Main.npc[num619].type == ModContent.NPCType<RavagerClawRight>())
                    rightClawActive = true;
                if (Main.npc[num619].active && Main.npc[num619].type == ModContent.NPCType<RavagerClawLeft>())
                    leftClawActive = true;
                if (Main.npc[num619].active && Main.npc[num619].type == ModContent.NPCType<RavagerLegRight>())
                    rightLegActive = true;
                if (Main.npc[num619].active && Main.npc[num619].type == ModContent.NPCType<RavagerLegLeft>())
                    leftLegActive = true;
            }

            bool enrage = false;
            if (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) > npc.position.Y + (float)(npc.height / 2) + 10f)
                enrage = true;

            if (headActive || rightClawActive || leftClawActive || rightLegActive || leftLegActive)
                npc.dontTakeDamage = true;
            else
            {
                npc.dontTakeDamage = false;
                if (Main.netMode != NetmodeID.Server)
                {
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
                        Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<WeakPetrification>(), 2);
                }
            }

            if (!headActive)
            {
                int rightDust = Dust.NewDust(new Vector2(npc.Center.X, npc.Center.Y - 30f), 8, 8, 5, 0f, 0f, 100, default, 2.5f);
                Main.dust[rightDust].alpha += Main.rand.Next(100);
                Main.dust[rightDust].velocity *= 0.2f;

                Dust rightDustExpr = Main.dust[rightDust];
                rightDustExpr.velocity.Y -= 3f + (float)Main.rand.Next(10) * 0.1f;
                Main.dust[rightDust].fadeIn = 0.5f + (float)Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    rightDust = Dust.NewDust(new Vector2(npc.Center.X, npc.Center.Y - 30f), 8, 8, 6, 0f, 0f, 0, default, 1.5f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[rightDust].noGravity = true;
                        Main.dust[rightDust].scale *= 1f + (float)Main.rand.Next(10) * 0.1f;
                        Dust rightDustExpr2 = Main.dust[rightDust];
                        rightDustExpr2.velocity.Y -= 4f;
                    }
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[1] += enrage ? 6f : 1f;
                    if (npc.localAI[1] >= 600f)
                    {
                        npc.localAI[1] = 0f;
                        npc.TargetClosest(true);
                        if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                        {
                            Vector2 shootFromVector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            float spread = 45f * 0.0174f;
                            double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
                            double deltaAngle = spread / 8f;
                            double offsetAngle;
                            int i;
                            int laserDamage = 45;
                            float velocity = CalamityWorld.bossRushActive ? 10f : 7f;
                            for (i = 0; i < 4; i++)
                            {
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                                Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(Math.Sin(offsetAngle) * velocity), (float)(Math.Cos(offsetAngle) * velocity), 259, laserDamage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
                                Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(-Math.Sin(offsetAngle) * velocity), (float)(-Math.Cos(offsetAngle) * velocity), 259, laserDamage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }
            }

            if (!rightClawActive)
            {
                int rightDust = Dust.NewDust(new Vector2(npc.Center.X + 80f, npc.Center.Y + 45f), 8, 8, 5, 0f, 0f, 100, default, 3f);
                Main.dust[rightDust].alpha += Main.rand.Next(100);
                Main.dust[rightDust].velocity *= 0.2f;

                Dust rightDustExpr = Main.dust[rightDust];
                rightDustExpr.velocity.X += 3f + (float)Main.rand.Next(10) * 0.1f;
                Main.dust[rightDust].fadeIn = 0.5f + (float)Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    rightDust = Dust.NewDust(new Vector2(npc.Center.X + 80f, npc.Center.Y + 45f), 8, 8, 6, 0f, 0f, 0, default, 2f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[rightDust].noGravity = true;
                        Main.dust[rightDust].scale *= 1f + (float)Main.rand.Next(10) * 0.1f;
                        Dust rightDustExpr2 = Main.dust[rightDust];
                        rightDustExpr2.velocity.X += 4f;
                    }
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[2] += enrage ? 2f : 1f;
                    if (npc.localAI[2] >= 480f)
                    {
                        Main.PlaySound(SoundID.Item20, npc.position);
                        npc.localAI[2] = 0f;
                        Vector2 shootFromVector = new Vector2(npc.Center.X + 80f, npc.Center.Y + 45f);
                        int damage = 40;
                        float velocity = CalamityWorld.bossRushActive ? 18f : 12f;
                        int laser = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, velocity, 0f, 258, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }

            if (!leftClawActive)
            {
                int leftDust = Dust.NewDust(new Vector2(npc.Center.X - 80f, npc.Center.Y + 45f), 8, 8, 5, 0f, 0f, 100, default, 3f);
                Main.dust[leftDust].alpha += Main.rand.Next(100);
                Main.dust[leftDust].velocity *= 0.2f;

                Dust leftDustExpr = Main.dust[leftDust];
                leftDustExpr.velocity.X -= 3f + (float)Main.rand.Next(10) * 0.1f;
                Main.dust[leftDust].fadeIn = 0.5f + (float)Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    leftDust = Dust.NewDust(new Vector2(npc.Center.X - 80f, npc.Center.Y + 45f), 8, 8, 6, 0f, 0f, 0, default, 2f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[leftDust].noGravity = true;
                        Main.dust[leftDust].scale *= 1f + (float)Main.rand.Next(10) * 0.1f;
                        Dust leftDustExpr2 = Main.dust[leftDust];
                        leftDustExpr2.velocity.X -= 4f;
                    }
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[3] += enrage ? 2f : 1f;
                    if (npc.localAI[3] >= 480f)
                    {
                        Main.PlaySound(SoundID.Item20, npc.position);
                        npc.localAI[3] = 0f;
                        Vector2 shootFromVector = new Vector2(npc.Center.X - 80f, npc.Center.Y + 45f);
                        int damage = 40;
                        float velocity = CalamityWorld.bossRushActive ? -18f : -12f;
                        int laser = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, velocity, 0f, 258, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }

            if (!rightLegActive)
            {
                int rightDust = Dust.NewDust(new Vector2(npc.Center.X + 60f, npc.Center.Y + 60f), 8, 8, 5, 0f, 0f, 100, default, 2f);
                Main.dust[rightDust].alpha += Main.rand.Next(100);
                Main.dust[rightDust].velocity *= 0.2f;

                Dust rightDustExpr = Main.dust[rightDust];
                rightDustExpr.velocity.Y += 0.5f + (float)Main.rand.Next(10) * 0.1f;
                Main.dust[rightDust].fadeIn = 0.5f + (float)Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    rightDust = Dust.NewDust(new Vector2(npc.Center.X + 60f, npc.Center.Y + 60f), 8, 8, 6, 0f, 0f, 0, default, 1.5f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[rightDust].noGravity = true;
                        Main.dust[rightDust].scale *= 1f + (float)Main.rand.Next(10) * 0.1f;
                        Dust rightDustExpr2 = Main.dust[rightDust];
                        rightDustExpr2.velocity.Y += 1f;
                    }
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 300f)
                    {
                        npc.ai[2] = 0f;
                        Vector2 shootFromVector = new Vector2(npc.Center.X + 60f, npc.Center.Y + 60f);
                        int damage = 35;
                        int fire = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, 0f, 2f, 326 + Main.rand.Next(3), damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
                        Main.projectile[fire].timeLeft = 180;
                    }
                }
            }

            if (!leftLegActive)
            {
                int leftDust = Dust.NewDust(new Vector2(npc.Center.X - 60f, npc.Center.Y + 60f), 8, 8, 5, 0f, 0f, 100, default, 2f);
                Main.dust[leftDust].alpha += Main.rand.Next(100);
                Main.dust[leftDust].velocity *= 0.2f;

                Dust leftDustExpr = Main.dust[leftDust];
                leftDustExpr.velocity.Y += 0.5f + (float)Main.rand.Next(10) * 0.1f;
                Main.dust[leftDust].fadeIn = 0.5f + (float)Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    leftDust = Dust.NewDust(new Vector2(npc.Center.X - 60f, npc.Center.Y + 60f), 8, 8, 6, 0f, 0f, 0, default, 1.5f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[leftDust].noGravity = true;
                        Main.dust[leftDust].scale *= 1f + (float)Main.rand.Next(10) * 0.1f;
                        Dust leftDustExpr2 = Main.dust[leftDust];
                        leftDustExpr2.velocity.Y += 1f;
                    }
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[3] += 1f;
                    if (npc.ai[3] >= 300f)
                    {
                        npc.ai[3] = 0f;
                        Vector2 shootFromVector = new Vector2(npc.Center.X - 60f, npc.Center.Y + 60f);
                        int damage = 35;
                        int fire = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, 0f, 2f, 326 + Main.rand.Next(3), damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
                        Main.projectile[fire].timeLeft = 180;
                    }
                }
            }

            if (npc.ai[0] == 0f)
            {
                npc.noTileCollide = false;

                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X = npc.velocity.X * 0.8f;

                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 0f)
                    {
                        if ((!rightClawActive && !leftClawActive) || npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
                            npc.ai[1] += 1f;
                        if (!headActive || npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
                            npc.ai[1] += 1f;
                        if ((!rightLegActive && !leftLegActive) || npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
                            npc.ai[1] += 1f;
                    }

                    if (npc.ai[1] >= 300f)
                        npc.ai[1] = -20f;
                    else if (npc.ai[1] == -1f)
                    {
                        npc.TargetClosest(true);

                        float velocityX = ((enrage || npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 8f : 4f) + (4f * (1f - lifeRatio));
                        npc.velocity.X = velocityX * (float)npc.direction;

                        if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
                        {
                            if (Main.player[npc.target].position.Y < npc.position.Y + (float)npc.height)
                                npc.velocity.Y = -15.2f;
                            else
                                npc.velocity.Y = 1f;

                            npc.noTileCollide = true;
                        }
                        else
                            npc.velocity.Y = -15.2f;

                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                    }
                }
            }
            else if (npc.ai[0] == 1f)
            {
                if (npc.velocity.Y == 0f)
                {
                    Main.PlaySound(SoundID.Item14, npc.position);

                    npc.ai[0] = 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (NPC.CountNPCS(ModContent.NPCType<RockPillar>()) < 2)
                        {
                            NPC.NewNPC((int)npc.Center.X - 360, (int)npc.Center.Y - 10, ModContent.NPCType<RockPillar>(), 0, 0f, 0f, 0f, 0f, 255);
                            NPC.NewNPC((int)npc.Center.X + 360, (int)npc.Center.Y - 10, ModContent.NPCType<RockPillar>(), 0, 0f, 0f, 0f, 0f, 255);
                        }

                        if (NPC.CountNPCS(ModContent.NPCType<FlamePillar>()) < 2)
                        {
                            NPC.NewNPC((int)Main.player[npc.target].Center.X - 180, (int)Main.player[npc.target].Center.Y - 10, ModContent.NPCType<FlamePillar>(), 0, 0f, 0f, 0f, 0f, 255);
                            NPC.NewNPC((int)Main.player[npc.target].Center.X + 180, (int)Main.player[npc.target].Center.Y - 10, ModContent.NPCType<FlamePillar>(), 0, 0f, 0f, 0f, 0f, 255);
                        }
                    }

                    for (int stompDustArea = (int)npc.position.X - 30; stompDustArea < (int)npc.position.X + npc.width + 60; stompDustArea += 30)
                    {
                        for (int stompDustAmount = 0; stompDustAmount < 6; stompDustAmount++)
                        {
                            int stompDust = Dust.NewDust(new Vector2(npc.position.X - 30f, npc.position.Y + (float)npc.height), npc.width + 30, 4, 31, 0f, 0f, 100, default, 1.5f);
                            Main.dust[stompDust].velocity *= 0.2f;
                        }

                        int stompGore = Gore.NewGore(new Vector2((float)(stompDustArea - 30), npc.position.Y + (float)npc.height - 12f), default, Main.rand.Next(61, 64), 1f);
                        Main.gore[stompGore].velocity *= 0.4f;
                    }
                }
                else
                {
                    npc.TargetClosest(true);

                    // Fall through
                    if (npc.target >= 0 && CalamityWorld.revenge &&
                        ((Main.player[npc.target].position.Y > npc.position.Y + (float)npc.height && npc.velocity.Y > 0f) || (Main.player[npc.target].position.Y < npc.position.Y + (float)npc.height && npc.velocity.Y < 0f)))
                        npc.noTileCollide = true;
                    else if (!Main.player[npc.target].dead)
                        npc.noTileCollide = false;

                    if (npc.position.X < Main.player[npc.target].position.X && npc.position.X + (float)npc.width > Main.player[npc.target].position.X + (float)Main.player[npc.target].width)
                    {
                        npc.velocity.X = npc.velocity.X * 0.9f;

                        if (Main.player[npc.target].position.Y > npc.position.Y + (float)npc.height)
                        {
                            float fallSpeed = 0.6f + (0.6f * (1f - lifeRatio));
                            npc.velocity.Y = npc.velocity.Y + fallSpeed;
                        }
                    }
                    else
                    {
                        if (npc.direction < 0)
                            npc.velocity.X = npc.velocity.X - 0.2f;
                        else if (npc.direction > 0)
                            npc.velocity.X = npc.velocity.X + 0.2f;

                        float velocityX = 3f + (4f * (1f - lifeRatio));
                        if (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
                            velocityX += 3f;
                        if (!rightClawActive)
                            velocityX += 1f;
                        if (!leftClawActive)
                            velocityX += 1f;
                        if (!headActive)
                            velocityX += 1f;
                        if (!rightLegActive)
                            velocityX += 1f;
                        if (!leftLegActive)
                            velocityX += 1f;

                        if (npc.velocity.X < -velocityX)
                            npc.velocity.X = -velocityX;
                        if (npc.velocity.X > velocityX)
                            npc.velocity.X = velocityX;
                    }
                }
            }

            if (npc.target <= 0 || npc.target == 255 || Main.player[npc.target].dead)
                npc.TargetClosest(true);

            int distanceFromTarget = 3000;
            if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) + Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) > (float)distanceFromTarget)
            {
                npc.TargetClosest(true);
                if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) + Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) > (float)distanceFromTarget)
                {
                    npc.active = false;
                    npc.netUpdate = true;
                    return;
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2((float)mod.GetTexture("NPCs/Ravager/RavagerBodyGlow").Width, (float)(mod.GetTexture("NPCs/Ravager/RavagerBodyGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 0f + 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Blue);
            Main.spriteBatch.Draw(mod.GetTexture("NPCs/Ravager/RavagerBodyGlow"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
            Color color2 = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
            Main.spriteBatch.Draw(mod.GetTexture("NPCs/Ravager/RavagerLegRight"), new Vector2(center.X - Main.screenPosition.X + 28f, center.Y - Main.screenPosition.Y + 20f), //72
                new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, mod.GetTexture("NPCs/Ravager/RavagerLegRight").Width, mod.GetTexture("NPCs/Ravager/RavagerLegRight").Height)),
                color2, 0f, default, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(mod.GetTexture("NPCs/Ravager/RavagerLegLeft"), new Vector2(center.X - Main.screenPosition.X - 112f, center.Y - Main.screenPosition.Y + 20f), //72
                new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, mod.GetTexture("NPCs/Ravager/RavagerLegLeft").Width, mod.GetTexture("NPCs/Ravager/RavagerLegLeft").Height)),
                color2, 0f, default, 1f, SpriteEffects.None, 0f);
            if (NPC.CountNPCS(ModContent.NPCType<RavagerHead>()) > 0)
            {
                Main.spriteBatch.Draw(mod.GetTexture("NPCs/Ravager/RavagerHead"), new Vector2(center.X - Main.screenPosition.X - 70f, center.Y - Main.screenPosition.Y - 75f),
                    new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, mod.GetTexture("NPCs/Ravager/RavagerHead").Width, mod.GetTexture("NPCs/Ravager/RavagerHead").Height)),
                    color2, 0f, default, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 2f);
                Dust.NewDust(npc.position, npc.width, npc.height, 6, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerBody"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerBody2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerBody3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerBody4"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerBody5"), 1f);
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 2f);
                    Dust.NewDust(npc.position, npc.width, npc.height, 6, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<Horror>(), 600, true);
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "Ravager";
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);

            DropHelper.DropItemChance(npc, ModContent.ItemType<RavagerTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeRavager>(), true, !CalamityWorld.downedScavenger);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedScavenger, 4, 2, 1);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Materials
                int barMin = CalamityWorld.downedProvidence ? 5 : 1;
                int barMax = CalamityWorld.downedProvidence ? 10 : 3;
                int coreMin = CalamityWorld.downedProvidence ? 1 : 1;
                int coreMax = CalamityWorld.downedProvidence ? 3 : 2;
                DropHelper.DropItemCondition(npc, ModContent.ItemType<Bloodstone>(), CalamityWorld.downedProvidence, 50, 60);
                DropHelper.DropItem(npc, ModContent.ItemType<VerstaltiteBar>(), barMin, barMax);
                DropHelper.DropItem(npc, ModContent.ItemType<DraedonBar>(), barMin, barMax);
                DropHelper.DropItem(npc, ModContent.ItemType<CruptixBar>(), barMin, barMax);
                DropHelper.DropItem(npc, ModContent.ItemType<CoreofCinder>(), coreMin, coreMax);
                DropHelper.DropItem(npc, ModContent.ItemType<CoreofEleum>(), coreMin, coreMax);
                DropHelper.DropItem(npc, ModContent.ItemType<CoreofChaos>(), coreMin, coreMax);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<BarofLife>(), CalamityWorld.downedProvidence, 2, 1, 1);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<CoreofCalamity>(), CalamityWorld.downedProvidence, 3, 1, 1);

                // Weapons
                DropHelper.DropItemFromSet(npc,
                    ModContent.ItemType<UltimusCleaver>(),
                    ModContent.ItemType<RealmRavager>(),
                    ModContent.ItemType<Hematemesis>(),
                    ModContent.ItemType<SpikecragStaff>(),
                    ModContent.ItemType<CraniumSmasher>());

                // Equipment
                DropHelper.DropItemChance(npc, ModContent.ItemType<BloodPact>(), 3);
                DropHelper.DropItemChance(npc, ModContent.ItemType<FleshTotem>(), 3);
            }

            // Mark Ravager as dead
            CalamityWorld.downedScavenger = true;
            CalamityMod.UpdateServerBoolean();
        }
    }
}
