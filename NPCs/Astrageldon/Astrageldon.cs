using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.Astrageldon
{
    [AutoloadBossHead]
    public class Astrageldon : ModNPC
	{
        private float bossLife;
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astrum Aureus");
			Main.npcFrameCount[npc.type] = 6;
		}
		
		public override void SetDefaults()
		{
            npc.npcSlots = 15f;
            npc.damage = 70;
			npc.width = 400;
			npc.height = 280;
			npc.defense = 120;
			npc.lifeMax = CalamityWorld.revenge ? 61075 : 51000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 90875;
            }
			npc.aiStyle = -1;
			aiType = -1;
			npc.knockBackResist = 0f;
			npc.value = Item.buyPrice(0, 20, 0, 0);
            for (int k = 0; k < npc.buffImmune.Length; k++)
			{
                npc.buffImmune[k] = true;
                npc.buffImmune[BuffID.Ichor] = false;
                npc.buffImmune[mod.BuffType("MarkedforDeath")] = false;
                npc.buffImmune[BuffID.CursedInferno] = false;
                npc.buffImmune[BuffID.Daybreak] = false;
                npc.buffImmune[mod.BuffType("AbyssalFlames")] = false;
                npc.buffImmune[mod.BuffType("ArmorCrunch")] = false;
                npc.buffImmune[mod.BuffType("DemonFlames")] = false;
                npc.buffImmune[mod.BuffType("HolyLight")] = false;
                npc.buffImmune[mod.BuffType("Nightwither")] = false;
                npc.buffImmune[mod.BuffType("Plague")] = false;
                npc.buffImmune[mod.BuffType("Shred")] = false;
                npc.buffImmune[mod.BuffType("WhisperingDeath")] = false;
                npc.buffImmune[mod.BuffType("SilvaStun")] = false;
            }
			npc.boss = true;
			npc.HitSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstrumAureusHit");
			npc.DeathSound = SoundID.NPCDeath14;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Astrageldon");
            bossBag = mod.ItemType("AstrageldonBag");
            if (NPC.downedMoonlord && CalamityWorld.revenge)
            {
                npc.lifeMax = 400000;
                npc.value = Item.buyPrice(3, 0, 0, 0);
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 1050000 : 940000;
            }
        }
		
		public override void AI()
		{
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
            int damageBuff = (int)(50f * (1f - (float)npc.life / (float)npc.lifeMax));
			int shootBuff = (int)(2f * (1f - (float)npc.life / (float)npc.lifeMax));
			float shootTimer = 1f + ((float)shootBuff);
            bool dayTime = Main.dayTime;
            Player player = Main.player[npc.target];
            npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
            if (!player.active || player.dead || dayTime)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    npc.noTileCollide = true;
                    npc.velocity = new Vector2(0f, 10f);
                    if (npc.timeLeft > 150)
                    {
                        npc.timeLeft = 150;
                    }
                    return;
                }
            }
            else
            {
                if (npc.timeLeft < 1800)
                {
                    npc.timeLeft = 1800;
                }
            }
            if (npc.ai[0] != 1f) //emit light when not recharging
            {
                Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 2.55f, 1f, 0f);
            }
            if (npc.ai[0] == 2f || npc.ai[0] >= 5f || (npc.ai[0] == 4f && npc.velocity.Y > 0f) || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged) //fire fast, latent homing, projectiles while walking.  lasers in a circle pattern while teleporting
            {
                if (Main.netMode != 1)
                {
                    npc.localAI[0] += ((npc.ai[0] == 2f || (npc.ai[0] == 4f && npc.velocity.Y > 0f && expertMode)) ? 4f : shootTimer);
                    if (npc.localAI[0] >= 180f) //6 seconds 3 seconds 2 seconds
                    {
                        npc.localAI[0] = 0f;
                        npc.TargetClosest(true);
                        int laserDamage = expertMode ? 35 : 45; //180 120
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 33);
                        if (NPC.downedMoonlord && revenge && !CalamityWorld.bossRushActive)
                        {
                            laserDamage *= 3;
                        }
                        if (npc.ai[0] >= 5f || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged) //teleporting
                        {
                            Vector2 shootFromVector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            float spread = 45f * 0.0174f;
                            double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
                            double deltaAngle = spread / 8f;
                            double offsetAngle;
                            int i;
                            for (i = 0; i < 4; i++)
                            {
                                offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
                                Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(Math.Sin(offsetAngle) * 7f), 
                                    (float)(Math.Cos(offsetAngle) * 7f), mod.ProjectileType("AstralFlame"), laserDamage, 0f, Main.myPlayer, 0f, 0f);
                                Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(-Math.Sin(offsetAngle) * 7f), 
                                    (float)(-Math.Cos(offsetAngle) * 7f), mod.ProjectileType("AstralFlame"), laserDamage, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                        else if ((npc.ai[0] == 4f && npc.velocity.Y > 0f && expertMode) || npc.ai[0] == 2f) //falling and walking
                        {
                            float num179 = 18.5f;
                            Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            float num180 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - value9.X;
                            float num181 = Math.Abs(num180) * 0.1f;
                            float num182 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - value9.Y - num181;
                            float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                            npc.netUpdate = true;
                            num183 = num179 / num183;
                            num180 *= num183;
                            num182 *= num183;
                            int num185 = mod.ProjectileType("AstralLaser");
                            value9.X += num180;
                            value9.Y += num182;
                            for (int num186 = 0; num186 < 5; num186++)
                            {
                                num180 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - value9.X;
                                num182 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - value9.Y;
                                num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                                num183 = num179 / num183;
                                num180 += (float)Main.rand.Next(-60, 61);
                                num182 += (float)Main.rand.Next(-60, 61);
                                num180 *= num183;
                                num182 *= num183;
                                Projectile.NewProjectile(value9.X, value9.Y, num180, num182, num185, laserDamage, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }
            }
            if (npc.ai[0] == 0f) //start up
            {
                npc.damage = 0;
                npc.ai[1] += 1f;
                if (npc.justHit || npc.ai[1] >= 60f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 1f) //use idle frames and regain fuel, become vulnerable
            {
                npc.damage = 0;
                npc.defense = 0;
                npc.velocity.X *= 0.98f;
                npc.velocity.Y *= 0.98f;
                npc.ai[1] += 1f;
                if (npc.ai[1] >= ((npc.life < npc.lifeMax / 4 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged) ? 90f : 150f))
                {
                    npc.defense = 120;
                    npc.noGravity = true;
                    npc.noTileCollide = true;
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 2f) //walk around and fire astral flames and lasers
            {
                npc.damage = npc.defDamage + damageBuff;
                float num823 = 4.5f;
                bool flag51 = false;
                if ((double)npc.life < (double)npc.lifeMax * 0.5)
                {
                    num823 = 5.5f;
                }
                if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
                {
                    num823 = 7f;
                }
                if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < 200f)
                {
                    flag51 = true;
                }
                if (flag51)
                {
                    npc.velocity.X = npc.velocity.X * 0.9f;
                    if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                    {
                        npc.velocity.X = 0f;
                    }
                }
                else
                {
                    float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
                    npc.direction = (playerLocation < 0 ? 1 : -1);
                    if (npc.direction > 0)
                    {
                        npc.velocity.X = (npc.velocity.X * 20f + num823) / 21f;
                    }
                    if (npc.direction < 0)
                    {
                        npc.velocity.X = (npc.velocity.X * 20f - num823) / 21f;
                    }
                }
                int num854 = 80;
                int num855 = 20;
                Vector2 position2 = new Vector2(npc.Center.X - (float)(num854 / 2), npc.position.Y + (float)npc.height - (float)num855);
                bool flag52 = false;
                if (npc.position.X < Main.player[npc.target].position.X && npc.position.X + (float)npc.width > Main.player[npc.target].position.X + (float)Main.player[npc.target].width && npc.position.Y + (float)npc.height < Main.player[npc.target].position.Y + (float)Main.player[npc.target].height - 16f)
                {
                    flag52 = true;
                }
                if (flag52)
                {
                    npc.velocity.Y = npc.velocity.Y + 0.5f;
                }
                else if (Collision.SolidCollision(position2, num854, num855))
                {
                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y = 0f;
                    }
                    if ((double)npc.velocity.Y > -0.2)
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.025f;
                    }
                    else
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.2f;
                    }
                    if (npc.velocity.Y < -4f)
                    {
                        npc.velocity.Y = -4f;
                    }
                }
                else
                {
                    if (npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y = 0f;
                    }
                    if ((double)npc.velocity.Y < 0.1)
                    {
                        npc.velocity.Y = npc.velocity.Y + 0.025f;
                    }
                    else
                    {
                        npc.velocity.Y = npc.velocity.Y + 0.5f;
                    }
                }
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 360f)
                {
                    npc.noGravity = false;
                    npc.noTileCollide = false;
                    npc.ai[0] = 3f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
                if (npc.velocity.Y > 10f)
                {
                    npc.velocity.Y = 10f;
                }
            }
            else if (npc.ai[0] == 3f) //leap upwards
            {
                npc.damage = npc.defDamage + damageBuff;
                npc.noTileCollide = false;
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X = npc.velocity.X * 0.8f;
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 0f)
                    {
                        npc.ai[1] += 1f;
                    }
                    if (npc.ai[1] >= 60f) //120
                    {
                        npc.ai[1] = -20f;
                    }
                    else if (npc.ai[1] == -1f)
                    {
                        npc.TargetClosest(true);
                        npc.velocity.X = (float)(4 * npc.direction); //4
                        npc.velocity.Y = -14.5f; //12.1
                        npc.ai[0] = 4f;
                        npc.ai[1] = 0f;
                    }
                }
            }
            else if (npc.ai[0] == 4f) //stomp
            {
                npc.damage = npc.defDamage + damageBuff;
                if (npc.velocity.Y == 0f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/LegStomp"), (int)npc.position.X, (int)npc.position.Y);
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 2f)
                    {
                        npc.ai[0] = ((npc.life < npc.lifeMax / 2 || revenge) ? 5f : 1f);
                        npc.ai[2] = 0f;
                    }
                    else
                    {
                        npc.ai[0] = 3f;
                    }
                    for (int num622 = (int)npc.position.X - 20; num622 < (int)npc.position.X + npc.width + 40; num622 += 20)
                    {
                        for (int num623 = 0; num623 < 4; num623++)
                        {
                            int num624 = Dust.NewDust(new Vector2(npc.position.X - 20f, npc.position.Y + (float)npc.height), npc.width + 20, 4, (Main.rand.Next(2) == 0 ? 55 : 173), 0f, 0f, 100, default(Color), 1.5f);
                            Main.dust[num624].velocity *= 0.2f;
                        }
                    }
                }
                else
                {
                    npc.TargetClosest(true);
                    if (npc.position.X < player.position.X && npc.position.X + (float)npc.width > player.position.X + (float)player.width)
                    {
                        npc.velocity.X = npc.velocity.X * 0.9f;
                        npc.velocity.Y = npc.velocity.Y + 0.6f; //0.2
                    }
                    else
                    {
                        if (npc.direction < 0)
                        {
                            npc.velocity.X = npc.velocity.X - 0.2f;
                        }
                        else if (npc.direction > 0)
                        {
                            npc.velocity.X = npc.velocity.X + 0.2f;
                        }
                        float num626 = 8f; //4
                        if (revenge)
                        {
                            num626 += 1f;
                        }
                        if (CalamityWorld.death || CalamityWorld.bossRushActive)
                        {
                            num626 += 2f;
                        }
                        if (npc.life < npc.lifeMax / 2 || CalamityWorld.bossRushActive)
                        {
                            num626 += 1f;
                        }
                        if (npc.life < npc.lifeMax / 10 || CalamityWorld.bossRushActive)
                        {
                            num626 += 1f;
                        }
                        if (npc.velocity.X < -num626)
                        {
                            npc.velocity.X = -num626;
                        }
                        if (npc.velocity.X > num626)
                        {
                            npc.velocity.X = num626;
                        }
                    }
                }
            }
            else if (npc.ai[0] == 5f) //start teleport and summon minions, jump a bit
            {
                npc.velocity.X *= 0.95f;
                npc.velocity.Y *= 0.95f;
                npc.chaseable = true;
                npc.dontTakeDamage = false;
                if (Main.netMode != 1)
                {
                    npc.localAI[1] += 1f;
                    if (!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        npc.localAI[1] += 5f;
                    }
                    if (npc.localAI[1] >= 60f)
                    {
                        npc.damage = npc.defDamage + damageBuff;
                    }
                    if (npc.localAI[1] >= 240f)
                    {
                        bool spawnFlag = revenge;
                        if (NPC.CountNPCS(mod.NPCType("AstrageldonSlime")) > 1)
                        {
                            spawnFlag = false;
                        }
                        if (spawnFlag && Main.netMode != 1)
                        {
                            NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 50, mod.NPCType("AstrageldonSlime"), 0, 0f, 0f, 0f, 0f, 255);
                        }
                        npc.localAI[1] = 0f;
                        npc.TargetClosest(true);
                        int num1249 = 0;
                        int num1250;
                        int num1251;
                        while (true)
                        {
                            num1249++;
                            num1250 = (int)Main.player[npc.target].Center.X / 16;
                            num1251 = (int)Main.player[npc.target].Center.Y / 16;
                            num1250 += Main.rand.Next(-30, 31);
                            num1251 += Main.rand.Next(-30, 31);
                            if (!WorldGen.SolidTile(num1250, num1251) && Collision.CanHit(new Vector2((float)(num1250 * 16), (float)(num1251 * 16)), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                            {
                                break;
                            }
                            if (num1249 > 100)
                            {
                                goto Block;
                            }
                        }
                        npc.ai[0] = 6f;
                        npc.ai[3] = (float)num1250;
                        npc.localAI[2] = (float)num1251;
                        npc.netUpdate = true;
                    Block:;
                    }
                }
            }
            else if (npc.ai[0] == 6f) //mid-teleport
            {
                npc.damage = 0;
                npc.chaseable = false;
                npc.dontTakeDamage = true;
                npc.alpha += 10;
                if (npc.alpha >= 255)
                {
                    npc.alpha = 255;
                    npc.position.X = npc.ai[3] * 16f - (float)(npc.width / 2);
                    npc.position.Y = npc.localAI[2] * 16f - (float)(npc.height / 2);
                    npc.ai[0] = 7f;
                    npc.netUpdate = true;
                }
                if (npc.soundDelay == 0)
                {
                    npc.soundDelay = 15;
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 109);
                }
                int num;
                for (int num245 = 0; num245 < 10; num245 = num + 1)
                {
                    int num244 = Dust.NewDust(npc.position, npc.width, npc.height, (Main.rand.Next(2) == 0 ? 173 : 55), npc.velocity.X, npc.velocity.Y, 255, default(Color), 2f);
                    Main.dust[num244].noGravity = true;
                    Main.dust[num244].velocity *= 0.5f;
                    num = num245;
                }
            }
            else if (npc.ai[0] == 7f) //end teleport
            {
                npc.alpha -= 10;
                if (npc.alpha <= 0)
                {
                    bool spawnFlag = revenge;
                    if (NPC.CountNPCS(mod.NPCType("AstrageldonSlime")) > 1)
                    {
                        spawnFlag = false;
                    }
                    if (spawnFlag && Main.netMode != 1)
                    {
                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 50, mod.NPCType("AstrageldonSlime"), 0, 0f, 0f, 0f, 0f, 255);
                    }
                    npc.chaseable = true;
                    npc.dontTakeDamage = false;
                    npc.alpha = 0;
                    npc.ai[0] = 1f;
                    npc.netUpdate = true;
                }
                if (npc.soundDelay == 0)
                {
                    npc.soundDelay = 15;
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 109);
                }
                int num;
                for (int num245 = 0; num245 < 10; num245 = num + 1)
                {
                    int num244 = Dust.NewDust(npc.position, npc.width, npc.height, (Main.rand.Next(2) == 0 ? 173 : 55), npc.velocity.X, npc.velocity.Y, 255, default(Color), 2f);
                    Main.dust[num244].noGravity = true;
                    Main.dust[num244].velocity *= 0.5f;
                    num = num245;
                }
            }
		}

        public override void FindFrame(int frameHeight)
        {
            if (npc.ai[0] == 3f || npc.ai[0] == 4f)
            {
                if (npc.velocity.Y == 0f && npc.ai[1] >= 0f && npc.ai[0] == 3f) //idle before jump
                {
                    if (npc.localAI[3] > 0f)
                    {
                        npc.localAI[3] = 0f;
                        npc.netUpdate = true;
                    }
                    npc.frameCounter += 1.0;
                    if (npc.frameCounter > 12.0)
                    {
                        npc.frame.Y = npc.frame.Y + frameHeight;
                        npc.frameCounter = 0.0;
                    }
                    if (npc.frame.Y >= frameHeight * 6)
                    {
                        npc.frame.Y = 0;
                    }
                }
                else if (npc.velocity.Y <= 0f || npc.ai[1] < 0f) //prepare to jump and then jump
                {
                    npc.frameCounter += 1.0;
                    if (npc.frameCounter > 12.0)
                    {
                        npc.frame.Y = npc.frame.Y + frameHeight;
                        npc.frameCounter = 0.0;
                    }
                    if (npc.frame.Y >= frameHeight * 5)
                    {
                        npc.frame.Y = frameHeight * 5;
                    }
                }
                else //stomping
                {
                    if (npc.localAI[3] == 0f)
                    {
                        npc.localAI[3] = 1f;
                        npc.frameCounter = 0.0;
                        npc.frame.Y = 0;
                        npc.netUpdate = true;
                    }
                    npc.frameCounter += 1.0;
                    if (npc.frameCounter > 12.0)
                    {
                        npc.frame.Y = npc.frame.Y + frameHeight;
                        npc.frameCounter = 0.0;
                    }
                    if (npc.frame.Y >= frameHeight * 5)
                    {
                        npc.frame.Y = frameHeight * 5;
                    }
                }
            }
            else if (npc.ai[0] >= 5f)
            {
                if (npc.localAI[3] > 0f)
                {
                    npc.localAI[3] = 0f;
                    npc.netUpdate = true;
                }
                if (npc.velocity.Y == 0f) //idle before teleport
                {
                    npc.frameCounter += 1.0;
                    if (npc.frameCounter > 12.0)
                    {
                        npc.frame.Y = npc.frame.Y + frameHeight;
                        npc.frameCounter = 0.0;
                    }
                    if (npc.frame.Y >= frameHeight * 6)
                    {
                        npc.frame.Y = 0;
                    }
                }
                else //in-air
                {
                    npc.frameCounter += 1.0;
                    if (npc.frameCounter > 12.0)
                    {
                        npc.frame.Y = npc.frame.Y + frameHeight;
                        npc.frameCounter = 0.0;
                    }
                    if (npc.frame.Y >= frameHeight * 5)
                    {
                        npc.frame.Y = frameHeight * 5;
                    }
                }
            }
            else
            {
                if (npc.localAI[3] > 0f)
                {
                    npc.localAI[3] = 0f;
                    npc.netUpdate = true;
                }
                npc.frameCounter += 1.0;
                if (npc.frameCounter > 12.0)
                {
                    npc.frame.Y = npc.frame.Y + frameHeight;
                    npc.frameCounter = 0.0;
                }
                if (npc.frame.Y >= frameHeight * 6)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            Texture2D NPCTexture = Main.npcTexture[npc.type];
            Texture2D GlowMaskTexture = Main.npcTexture[npc.type];
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
            if (npc.ai[0] == 0f)
            {
                NPCTexture = Main.npcTexture[npc.type];
                GlowMaskTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonGlow");
            }
            else if (npc.ai[0] == 1f) //nothing special done here
            {
                NPCTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonRecharge");
            }
            else if (npc.ai[0] == 2f) //nothing special done here
            {
                NPCTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonWalk");
                GlowMaskTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonWalkGlow");
            }
            else if (npc.ai[0] == 3f || npc.ai[0] == 4f) //needs to have an in-air frame
            {
                if (npc.velocity.Y == 0f && npc.ai[1] >= 0f && npc.ai[0] == 3f) //idle before jump
                {
                    NPCTexture = Main.npcTexture[npc.type]; //idle frames
                    GlowMaskTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonGlow");
                }
                else if (npc.velocity.Y <= 0f || npc.ai[1] < 0f) //jump frames if flying upward or if about to jump
                {
                    NPCTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonJump");
                    GlowMaskTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonJumpGlow");
                }
                else //stomping
                {
                    NPCTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonStomp");
                    GlowMaskTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonStompGlow");
                }
            }
            else if (npc.ai[0] >= 5f) //needs to have an in-air frame
            {
                if (npc.velocity.Y == 0f) //idle before teleport
                {
                    NPCTexture = Main.npcTexture[npc.type]; //idle frames
                    GlowMaskTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonGlow");
                }
                else //in-air frames
                {
                    NPCTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonJump");
                    GlowMaskTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonJumpGlow");
                }
            }
            Color lightColor = (drawColor != null ? (Color)drawColor : CalamityMod.GetNPCColor(((NPC)npc), npc.Center, false));
            int frameCount = Main.npcFrameCount[npc.type];
            Microsoft.Xna.Framework.Rectangle frame = npc.frame;
            float scale = npc.scale;
            float rotation = npc.rotation;
            float offsetY = npc.gfxOffY;
            Main.spriteBatch.Draw(NPCTexture,
                new Vector2(npc.position.X - Main.screenPosition.X + (float)(npc.width / 2) - (float)Main.npcTexture[npc.type].Width * scale / 2f + vector11.X * scale,
                npc.position.Y - Main.screenPosition.Y + (float)npc.height - (float)Main.npcTexture[npc.type].Height * scale / (float)Main.npcFrameCount[npc.type] + 4f + vector11.Y * scale + 0f + offsetY),
                new Microsoft.Xna.Framework.Rectangle?(frame),
                npc.GetAlpha(lightColor),
                rotation,
                vector11,
                scale,
                spriteEffects,
                0f);
            if (npc.ai[0] != 1) //draw only if not recharging
            {
                Vector2 center = new Vector2(npc.Center.X, npc.Center.Y - 30f); //30
                Vector2 vector = center - Main.screenPosition;
                vector -= new Vector2((float)GlowMaskTexture.Width, (float)(GlowMaskTexture.Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
                vector += vector11 * 1f + new Vector2(0f, 0f + 4f + offsetY);
                Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Gold);
                Main.spriteBatch.Draw(GlowMaskTexture, vector,
                    new Microsoft.Xna.Framework.Rectangle?(frame), color, rotation, vector11, 1f, spriteEffects, 0f);
            }
            return false;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (((projectile.type == ProjectileID.HallowStar || projectile.type == ProjectileID.CrystalShard) && projectile.ranged) ||
                projectile.type == mod.ProjectileType("TerraBulletSplit") || projectile.type == mod.ProjectileType("TerraArrow2"))
            {
                damage /= 2;
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
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("AstrageldonTrophy"));
            }
            if (Main.expertMode)
            {
                npc.DropBossBags();
            }
            else
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("AstralJelly"), Main.rand.Next(9, 13));
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Stardust"), Main.rand.Next(20, 31));
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.FallenStar, Main.rand.Next(25, 41));
            }
            if (NPC.downedMoonlord)
            {
                int amount = Main.rand.Next(25, 41) / 2;
                if (Main.expertMode)
                {
                    amount = (int)((float)amount * 1.5f);
                }
                for (int i = 0; i < amount; i++)
                {
                    Item.NewItem((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), 2, 2, 3459, Main.rand.Next(1, 4), false, 0, false, false);
                }
                for (int i = 0; i < amount; i++)
                {
                    Item.NewItem((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), 2, 2, 3458, Main.rand.Next(1, 4), false, 0, false, false);
                }
                for (int i = 0; i < amount; i++)
                {
                    Item.NewItem((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), 2, 2, 3457, Main.rand.Next(1, 4), false, 0, false, false);
                }
                for (int i = 0; i < amount; i++)
                {
                    Item.NewItem((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), 2, 2, 3456, Main.rand.Next(1, 4), false, 0, false, false);
                }
            }
        }
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 173, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 150;
				npc.height = 100;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 50; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 100; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 55, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 55, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("GodSlayerInferno"), 150, true);
		}
	}
}