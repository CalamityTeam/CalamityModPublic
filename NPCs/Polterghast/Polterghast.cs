using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.Polterghast
{
	[AutoloadBossHead]
	public class Polterghast : ModNPC
	{
        private int despawnTimer = 600;
        private bool spawnGhost = false;
        private bool boostDR = false;

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Polterghast");
			Main.npcFrameCount[npc.type] = 12;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 50f;
			npc.damage = 150;
			npc.width = 90;
			npc.height = 120;
			npc.defense = 150;
			npc.lifeMax = CalamityWorld.revenge ? 510000 : 440000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 705000;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 6600000 : 6000000;
            }
            npc.knockBackResist = 0f;
			npc.aiStyle = -1; //new
            aiType = -1; //new
			npc.value = Item.buyPrice(5, 0, 0, 0);
			npc.boss = true;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
				npc.buffImmune[BuffID.Ichor] = false;
                npc.buffImmune[BuffID.CursedInferno] = false;
                npc.buffImmune[mod.BuffType("AbyssalFlames")] = false;
                npc.buffImmune[mod.BuffType("DemonFlames")] = false;
                npc.buffImmune[mod.BuffType("GodSlayerInferno")] = false;
                npc.buffImmune[mod.BuffType("Nightwither")] = false;
                npc.buffImmune[mod.BuffType("Shred")] = false;
            }
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.netAlways = true;
			music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/RUIN");
			npc.HitSound = SoundID.NPCHit7;
			npc.DeathSound = SoundID.NPCDeath39;
			bossBag = mod.ItemType("PolterghastBag");
		}

        public override void AI()
        {
            if (Main.raining)
            {
                Main.raining = false;
                if (Main.netMode == 2) { NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0); }
            }
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.5f, 1.25f, 1.25f);
            npc.TargetClosest(true);
            Vector2 vector = npc.Center;
            if (Vector2.Distance(Main.player[npc.target].Center, vector) > 6000f) { npc.active = false; }
            bool speedBoost1 = false;
            bool despawnBoost = false;
            if (npc.timeLeft < 1500) { npc.timeLeft = 1500; }
            bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
            bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
            bool phase2 = (double)npc.life <= (double)npc.lifeMax * 0.75; //hooks detach and fire beams
            bool phase3 = (double)npc.life <= (double)npc.lifeMax * (revenge ? 0.5 : 0.33); //hooks die and begins charging with ghosts spinning around player
            bool phase4 = (double)npc.life <= (double)npc.lifeMax * (revenge ? 0.33 : 0.2); //starts spitting ghost dudes
            bool phase5 = (double)npc.life <= (double)npc.lifeMax * (revenge ? 0.1 : 0.05); //starts moving incredibly fast
            CalamityGlobalNPC.ghostBoss = npc.whoAmI;
            if (npc.localAI[0] == 0f && Main.netMode != 1)
            {
                npc.localAI[0] = 1f;
                int num729 = NPC.NewNPC((int)vector.X, (int)vector.Y, mod.NPCType("PolterghastHook"), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                num729 = NPC.NewNPC((int)vector.X, (int)vector.Y, mod.NPCType("PolterghastHook"), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                num729 = NPC.NewNPC((int)vector.X, (int)vector.Y, mod.NPCType("PolterghastHook"), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                num729 = NPC.NewNPC((int)vector.X, (int)vector.Y, mod.NPCType("PolterghastHook"), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
            }
            int[] array2 = new int[4];
            float num730 = 0f;
            float num731 = 0f;
            int num732 = 0;
            int num;
            for (int num733 = 0; num733 < 200; num733 = num + 1)
            {
                if (Main.npc[num733].active && Main.npc[num733].type == mod.NPCType("PolterghastHook"))
                {
                    num730 += Main.npc[num733].Center.X;
                    num731 += Main.npc[num733].Center.Y;
                    array2[num732] = num733;
                    num732++;
                    if (num732 > 3) { break; }
                }
                num = num733;
            }
            num730 /= (float)num732;
            num731 /= (float)num732;
            float num734 = 2.5f;
            float num735 = 0.025f;
            if (!Main.player[npc.target].ZoneDungeon && !CalamityWorld.bossRushActive)
            {
                despawnTimer--;
                if (despawnTimer <= 0) { despawnBoost = true; }
                speedBoost1 = true;
                num734 += 8f;
                num735 = 0.15f;
            }
            else { despawnTimer = 600; }
            if (phase2)
            {
                num734 = 3.5f;
                num735 = 0.035f;
            }
            if (phase3)
            {
                if (NPC.CountNPCS(mod.NPCType("PolterPhantom")) > 0)
                {
                    boostDR = true;
                    if (npc.ai[2] >= 300f)
                    {
                        num734 = phase5 ? 18f : 12f;
                        num735 = phase5 ? 0.12f : 0.08f;
                    }
                    else
                    {
                        if (phase5)
                        {
                            num734 = 5f;
                            num735 = 0.05f;
                        }
                        else if (phase4)
                        {
                            num734 = 4.5f;
                            num735 = 0.045f;
                        }
                        else
                        {
                            num734 = 4f;
                            num735 = 0.04f;
                        }
                    }
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 600f)
                    {
                        npc.ai[2] = 0f;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    boostDR = false;
                    num734 = phase5 ? 22f : 18f;
                    num735 = phase5 ? 0.15f : 0.12f;
                    npc.localAI[2] += 1f;
                    if (npc.localAI[2] >= 600f)
                    {
                        npc.localAI[2] = 0f;
                        NPC.NewNPC((int)vector.X, (int)vector.Y, mod.NPCType("PolterPhantom"), 0, 0f, 0f, 0f, 0f, 255);
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 122);
                        for (int num621 = 0; num621 < 10; num621++)
                        {
                            int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
                            Main.dust[num622].velocity *= 3f;
                            Main.dust[num622].noGravity = true;
                            if (Main.rand.Next(2) == 0)
                            {
                                Main.dust[num622].scale = 0.5f;
                                Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                            }
                        }
                        for (int num623 = 0; num623 < 30; num623++)
                        {
                            int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 3f);
                            Main.dust[num624].noGravity = true;
                            Main.dust[num624].velocity *= 5f;
                            num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default(Color), 2f);
                            Main.dust[num624].velocity *= 2f;
                        }
                        npc.netUpdate = true;
                    }
                }
            }
            if (expertMode)
            {
                num734 += revenge ? 1.5f : 1f;
                num734 *= revenge ? 1.25f : 1.1f;
                num735 += revenge ? 0.015f : 0.01f;
                num735 *= revenge ? 1.2f : 1.1f;
            }
            Vector2 vector91 = new Vector2(num730, num731);
            float num736 = Main.player[npc.target].Center.X - vector91.X;
            float num737 = Main.player[npc.target].Center.Y - vector91.Y;
            if (despawnBoost)
            {
                num737 *= -1f;
                num736 *= -1f;
                num734 += 8f;
            }
            float num738 = (float)Math.Sqrt((double)(num736 * num736 + num737 * num737));
            int num739 = 500;
            if (speedBoost1) { num739 += 500; }
            if (expertMode) { num739 += 150; }
            if (num738 >= (float)num739)
            {
                num738 = (float)num739 / num738;
                num736 *= num738;
                num737 *= num738;
            }
            num730 += num736;
            num731 += num737;
            vector91 = new Vector2(vector.X, vector.Y);
            num736 = num730 - vector91.X;
            num737 = num731 - vector91.Y;
            num738 = (float)Math.Sqrt((double)(num736 * num736 + num737 * num737));
            if (num738 < num734)
            {
                num736 = npc.velocity.X;
                num737 = npc.velocity.Y;
            }
            else
            {
                num738 = num734 / num738;
                num736 *= num738;
                num737 *= num738;
            }
            Vector2 vector92 = new Vector2(vector.X, vector.Y);
            float num740 = Main.player[npc.target].Center.X - vector92.X;
            float num741 = Main.player[npc.target].Center.Y - vector92.Y;
            npc.rotation = (float)Math.Atan2((double)num741, (double)num740) + 1.57f;
            if (npc.velocity.X < num736)
            {
                npc.velocity.X = npc.velocity.X + num735;
                if (npc.velocity.X < 0f && num736 > 0f)
                {
                    npc.velocity.X = npc.velocity.X + num735 * 2f;
                }
            }
            else if (npc.velocity.X > num736)
            {
                npc.velocity.X = npc.velocity.X - num735;
                if (npc.velocity.X > 0f && num736 < 0f)
                {
                    npc.velocity.X = npc.velocity.X - num735 * 2f;
                }
            }
            if (npc.velocity.Y < num737)
            {
                npc.velocity.Y = npc.velocity.Y + num735;
                if (npc.velocity.Y < 0f && num737 > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y + num735 * 2f;
                }
            }
            else if (npc.velocity.Y > num737)
            {
                npc.velocity.Y = npc.velocity.Y - num735;
                if (npc.velocity.Y > 0f && num737 < 0f)
                {
                    npc.velocity.Y = npc.velocity.Y - num735 * 2f;
                }
            }
            if (!phase2 && !phase3)
            {
                if (speedBoost1)
                {
                    npc.defense = 300;
                    npc.damage = (int)(200f * Main.damageMultiplier);
                }
                else
                {
                    npc.damage = expertMode ? 240 : 150;
                    npc.defense = 150;
                }
                if (Main.netMode != 1)
                {
                    npc.localAI[1] += 1f;
                    if ((double)npc.life < (double)npc.lifeMax * 0.9)
                    {
                        npc.localAI[1] += 1f;
                    }
                    if ((double)npc.life < (double)npc.lifeMax * 0.8)
                    {
                        npc.localAI[1] += 1f;
                    }
                    if (speedBoost1 || CalamityWorld.bossRushActive)
                    {
                        npc.localAI[1] += 6f;
                    }
                    if (expertMode)
                    {
                        npc.localAI[1] += 1f;
                    }
                    if (npc.localAI[1] > 120f)
                    {
                        npc.localAI[1] = 0f;
                        bool flag47 = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                        if (npc.localAI[3] > 0f)
                        {
                            flag47 = true;
                            npc.localAI[3] = 0f;
                        }
                        if (flag47)
                        {
                            Vector2 vector93 = new Vector2(vector.X, vector.Y);
                            float num742 = 11f;
                            if (expertMode)
                            {
                                num742 = (CalamityWorld.bossRushActive ? 18f : 12f);
                            }
                            if (speedBoost1 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                            {
                                num742 *= 2f;
                            }
                            float num743 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector93.X;
                            float num744 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector93.Y;
                            float num745 = (float)Math.Sqrt((double)(num743 * num743 + num744 * num744));
                            num745 = num742 / num745;
                            num743 *= num745;
                            num744 *= num745;
                            int num746 = expertMode ? 46 : 55;
                            int num747 = mod.ProjectileType("PhantomShot");
                            int maxValue2 = 2;
                            if (expertMode)
                            {
                                maxValue2 = 4;
                            }
                            if ((double)npc.life < (double)npc.lifeMax * 0.8 && Main.rand.Next(maxValue2) == 0)
                            {
                                num746 = expertMode ? 51 : 60;
                                npc.localAI[1] = -30f;
                                num747 = mod.ProjectileType("PhantomBlast");
                            }
                            if (speedBoost1 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                            {
                                num746 *= 2;
                            }
                            vector93.X += num743 * 3f;
                            vector93.Y += num744 * 3f;
                            int num748 = Projectile.NewProjectile(vector93.X, vector93.Y, num743, num744, num747, num746, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[num748].timeLeft = 300;
                            return;
                        }
                        else
                        {
                            Vector2 vector93 = new Vector2(vector.X, vector.Y);
                            float num742 = 11f;
                            if (expertMode)
                            {
                                num742 = (CalamityWorld.bossRushActive ? 18f : 12f);
                            }
                            if (speedBoost1 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                            {
                                num742 *= 2f;
                            }
                            float num743 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector93.X;
                            float num744 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector93.Y;
                            float num745 = (float)Math.Sqrt((double)(num743 * num743 + num744 * num744));
                            num745 = num742 / num745;
                            num743 *= num745;
                            num744 *= num745;
                            int num746 = expertMode ? 51 : 60;
                            int num747 = mod.ProjectileType("PhantomBlast");
                            if (speedBoost1 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                            {
                                num746 *= 2;
                            }
                            vector93.X += num743 * 3f;
                            vector93.Y += num744 * 3f;
                            int num748 = Projectile.NewProjectile(vector93.X, vector93.Y, num743, num744, num747, num746, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[num748].timeLeft = 180;
                            return;
                        }
                    }
                }
            }
            else if (!phase3)
            {
                if (npc.ai[0] == 0f)
                {
                    npc.ai[0] += 1f;
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 122);
                    for (int num621 = 0; num621 < 10; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 60, 0f, 0f, 100, default(Color), 2f);
                        Main.dust[num622].velocity *= 3f;
                        Main.dust[num622].noGravity = true;
                        if (Main.rand.Next(2) == 0)
                        {
                            Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int num623 = 0; num623 < 30; num623++)
                    {
                        int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default(Color), 3f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default(Color), 2f);
                        Main.dust[num624].velocity *= 2f;
                    }
                }
                npc.GivenName = "Necroghast";
                if (speedBoost1 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                {
                    npc.defense = 200;
                    npc.damage = (int)(300f * Main.damageMultiplier);
                }
                else
                {
                    npc.damage = expertMode ? 288 : 180;
                    npc.defense = 100;
                }
                if (Main.netMode != 1)
                {
                    npc.localAI[1] += 1f;
                    if (speedBoost1 || CalamityWorld.bossRushActive)
                    {
                        npc.localAI[1] += 8f;
                    }
                    if (expertMode)
                    {
                        npc.localAI[1] += 1f;
                    }
                    if (npc.localAI[1] > 80f)
                    {
                        npc.localAI[1] = 0f;
                        bool flag47 = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                        if (npc.localAI[3] > 0f)
                        {
                            flag47 = true;
                            npc.localAI[3] = 0f;
                        }
                        if (flag47)
                        {
                            Vector2 vector93 = new Vector2(vector.X, vector.Y);
                            float num742 = 11f;
                            if (expertMode)
                            {
                                num742 = (CalamityWorld.bossRushActive ? 18f : 12f);
                            }
                            if (speedBoost1 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                            {
                                num742 *= 2f;
                            }
                            float num743 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector93.X;
                            float num744 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector93.Y;
                            float num745 = (float)Math.Sqrt((double)(num743 * num743 + num744 * num744));
                            num745 = num742 / num745;
                            num743 *= num745;
                            num744 *= num745;
                            int num746 = expertMode ? 49 : 60;
                            int num747 = mod.ProjectileType("PhantomShot2");
                            int maxValue2 = 2;
                            if (expertMode)
                            {
                                maxValue2 = 4;
                            }
                            if (Main.rand.Next(maxValue2) == 0)
                            {
                                num746 = expertMode ? 54 : 65;
                                npc.localAI[1] = -30f;
                                num747 = mod.ProjectileType("PhantomBlast2");
                            }
                            if (speedBoost1 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                            {
                                num746 *= 2;
                            }
                            vector93.X += num743 * 3f;
                            vector93.Y += num744 * 3f;
                            int num748 = Projectile.NewProjectile(vector93.X, vector93.Y, num743, num744, num747, num746, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[num748].timeLeft = 400;
                            return;
                        }
                        else
                        {
                            Vector2 vector93 = new Vector2(vector.X, vector.Y);
                            float num742 = 11f;
                            if (expertMode)
                            {
                                num742 = (CalamityWorld.bossRushActive ? 18f : 12f);
                            }
                            if (speedBoost1 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                            {
                                num742 *= 2f;
                            }
                            float num743 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector93.X;
                            float num744 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector93.Y;
                            float num745 = (float)Math.Sqrt((double)(num743 * num743 + num744 * num744));
                            num745 = num742 / num745;
                            num743 *= num745;
                            num744 *= num745;
                            int num746 = expertMode ? 54 : 65;
                            int num747 = mod.ProjectileType("PhantomBlast2");
                            if (speedBoost1 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                            {
                                num746 *= 2;
                            }
                            vector93.X += num743 * 3f;
                            vector93.Y += num744 * 3f;
                            int num748 = Projectile.NewProjectile(vector93.X, vector93.Y, num743, num744, num747, num746, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[num748].timeLeft = 240;
                            return;
                        }
                    }
                }
            }
            else
            {
                npc.HitSound = SoundID.NPCHit36;
                if (!spawnGhost)
                {
                    spawnGhost = true;
                    if (Main.netMode != 1)
                    {
                        NPC.NewNPC((int)vector.X, (int)vector.Y, mod.NPCType("PolterPhantom"), 0, 0f, 0f, 0f, 0f, 255);
                        for (int I = 0; I < 3; I++)
                        {
                            int Phantom = NPC.NewNPC((int)(Main.player[npc.target].Center.X + (Math.Sin(I * 120) * 500)),
                                (int)(Main.player[npc.target].Center.Y + (Math.Cos(I * 120) * 500)), mod.NPCType("PhantomFuckYou"), 0, 0, 0, 0, -1);
                            NPC Eye = Main.npc[Phantom];
                            Eye.ai[0] = I * 120;
                            Eye.ai[3] = I * 120;
                        }
                    }
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 122);
                    for (int num621 = 0; num621 < 10; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 60, 0f, 0f, 100, default(Color), 2f);
                        Main.dust[num622].velocity *= 3f;
                        Main.dust[num622].noGravity = true;
                        if (Main.rand.Next(2) == 0)
                        {
                            Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int num623 = 0; num623 < 30; num623++)
                    {
                        int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default(Color), 3f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default(Color), 2f);
                        Main.dust[num624].velocity *= 2f;
                    }
                }
                npc.GivenName = "Necroplasm";
                if (speedBoost1 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                {
                    npc.defense = 200;
                    npc.damage = (int)(400f * Main.damageMultiplier);
                }
                else
                {
                    npc.damage = expertMode ? 336 : 210;
                    npc.defense = 0;
                }
                if (phase4)
                {
                    npc.localAI[1] += 1f;
                    if (npc.localAI[1] >= 150f)
                    {
                        float num757 = 8f;
                        Vector2 vector94 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                        float num758 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector94.X + (float)Main.rand.Next(-10, 11);
                        float num759 = Math.Abs(num758 * 0.2f);
                        float num760 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector94.Y + (float)Main.rand.Next(-10, 11);
                        if (num760 > 0f)
                        {
                            num759 = 0f;
                        }
                        num760 -= num759;
                        float num761 = (float)Math.Sqrt((double)(num758 * num758 + num760 * num760));
                        num761 = num757 / num761;
                        num758 *= num761;
                        num760 *= num761;
                        if (NPC.CountNPCS(mod.NPCType("PhantomSpiritL")) < (revenge ? 3 : 2) && Main.netMode != 1)
                        {
                            int num762 = NPC.NewNPC((int)vector.X, (int)vector.Y, mod.NPCType("PhantomSpiritL"), 0, 0f, 0f, 0f, 0f, 255);
                            Main.npc[num762].velocity.X = num758;
                            Main.npc[num762].velocity.Y = num760;
                            Main.npc[num762].netUpdate = true;
                        }
                        npc.localAI[1] = 0f;
                        return;
                    }
                }
            }
        }
		
		public override void NPCLoot()
		{
            if (CalamityWorld.armageddon)
            {
                for (int i = 0; i < 10; i++)
                {
                    npc.DropBossBags();
                }
            }
            if (Main.rand.Next(10) == 0)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PolterghastTrophy"));
            }
            if (Main.expertMode)
			{
				npc.DropBossBags();
			}
			else
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("RuinousSoul"), Main.rand.Next(5, 9));
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BansheeHook"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DaemonsFlame"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EtherealSubjugator"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("FatesReveal"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GhastlyVisage"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GhoulishGouger"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("TerrorBlade"));
				}
			}
		}
		
		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.SuperHealingPotion;
		}
		
		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
            double newDamage = (damage + (int)((double)defense * 0.25));
            float protection = 0.1f + //.1
                    ((double)npc.life <= (double)npc.lifeMax * 0.75 ? 0.05f : 0f) + //.15
                    ((double)npc.life <= (double)npc.lifeMax * (CalamityWorld.revenge ? 0.5 : 0.33) ? 0.05f : 0f) + //.2
                    (boostDR ? 0.6f : 0f); //.8
            if (npc.ichor)
            {
                protection *= 0.88f;
            }
            else if (npc.onFire2)
            {
                protection *= 0.9f;
            }
            if (newDamage < 1.0)
			{
				newDamage = 1.0;
			}
			if (newDamage >= 1.0)
			{
                newDamage = (double)((int)((double)(1f - (protection * (npc.ichor ? 0.88f : 1f))) * newDamage));
				if (newDamage < 1.0)
				{
					newDamage = 1.0;
				}
			}
			damage = newDamage;
			return true;
		}
		
		public override void FindFrame(int frameHeight)
		{
			bool phase2 = (double)npc.life > (double)npc.lifeMax * (CalamityWorld.revenge ? 0.5 : 0.33);
			npc.frameCounter += 1.0;
			if (npc.frameCounter > 6.0)
			{
				npc.frameCounter = 0.0;
				npc.frame.Y = npc.frame.Y + frameHeight;
			}
			if ((double)npc.life > (double)npc.lifeMax * 0.75)
			{
				if (npc.frame.Y > frameHeight * 3)
				{
					npc.frame.Y = 0;
				}
			}
			else if (phase2)
			{
				if (npc.frame.Y < frameHeight * 4)
				{
					npc.frame.Y = frameHeight * 4;
				}
				if (npc.frame.Y > frameHeight * 7)
				{
					npc.frame.Y = frameHeight * 4;
				}
			}
			else
			{
				if (npc.frame.Y < frameHeight * 8)
				{
					npc.frame.Y = frameHeight * 8;
				}
				if (npc.frame.Y > frameHeight * 11)
				{
					npc.frame.Y = frameHeight * 8;
				}
			}
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
            if (CalamityWorld.revenge)
			    player.AddBuff(mod.BuffType("Horror"), 300, true);
		}
		
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;
			return true;
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			Dust.NewDust(npc.position, npc.width, npc.height, 180, hitDirection, -1f, 0, default(Color), 1f);
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 90;
				npc.height = 90;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 10; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 60, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 60; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
	}
}