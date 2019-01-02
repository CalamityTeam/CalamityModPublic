using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.CeaselessVoid
{
	[AutoloadBossHead]
	public class CeaselessVoid : ModNPC
	{
        private float bossLife;
        private float beamPortal = 0f;
        private float shootBoost = 0;
        private float passedVar = 1f;
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ceaseless Void");
			Main.npcFrameCount[npc.type] = 4;
		}
		
		public override void SetDefaults()
		{
			npc.damage = 0;
			npc.npcSlots = 36f;
			npc.width = 100; //324
			npc.height = 100; //216
			npc.defense = 0;
			npc.lifeMax = 200;
            if (Main.expertMode)
            {
                npc.lifeMax = 400;
            }
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/ScourgeofTheUniverse");
            if (CalamityWorld.DoGSecondStageCountdown <= 0)
            {
                music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Void");
            }
            npc.aiStyle = -1; //new
            aiType = -1; //new
			npc.knockBackResist = 0f;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.boss = true;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
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
            Player player = Main.player[npc.target];
            bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
            bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
            CalamityGlobalNPC.voidBoss = npc.whoAmI;
            Vector2 vector = npc.Center;
            npc.TargetClosest(true);
            if (NPC.CountNPCS(mod.NPCType("DarkEnergy")) > 0 ||
                NPC.CountNPCS(mod.NPCType("DarkEnergy2")) > 0 ||
                NPC.CountNPCS(mod.NPCType("DarkEnergy3")) > 0)
            {
                npc.dontTakeDamage = true;
            }
            else
            {
                npc.dontTakeDamage = false;
            }
            if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    npc.velocity = new Vector2(0f, -10f);
                    CalamityWorld.DoGSecondStageCountdown = 0;
                    if (Main.netMode == 2)
                    {
                        var netMessage = mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                        netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                        netMessage.Send();
                    }
                    if (npc.timeLeft > 150)
                    {
                        npc.timeLeft = 150;
                    }
                    return;
                }
            }
            else if (npc.timeLeft < 2400)
            {
                npc.timeLeft = 2400;
            }
            if (Main.netMode != 1)
            {
                beamPortal += expertMode ? 2f : 1f;
                beamPortal += shootBoost;
                if (CalamityWorld.death || CalamityWorld.bossRushActive)
                {
                    beamPortal += 4f;
                }
                if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                {
                    beamPortal += 2f;
                }
                if (beamPortal >= 1200f)
                {
                    beamPortal = 0f;
                    npc.TargetClosest(true);
                    if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                    {
                        float num941 = 3f; //speed
                        Vector2 vector104 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
                        float num942 = player.position.X + (float)player.width * 0.5f - vector104.X + (float)Main.rand.Next(-20, 21);
                        float num943 = player.position.Y + (float)player.height * 0.5f - vector104.Y + (float)Main.rand.Next(-20, 21);
                        float num944 = (float)Math.Sqrt((double)(num942 * num942 + num943 * num943));
                        num944 = num941 / num944;
                        num942 *= num944;
                        num943 *= num944;
                        num942 += (float)Main.rand.Next(-10, 11) * 0.05f;
                        num943 += (float)Main.rand.Next(-10, 11) * 0.05f;
                        int num945 = expertMode ? 42 : 58;
                        int num946 = mod.ProjectileType("DoGBeamPortal");
                        vector104.X += num942 * 5f;
                        vector104.Y += num943 * 5f;
                        int num947 = Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, num946, num945, 0f, Main.myPlayer, 0f, 0f);
                        Main.projectile[num947].timeLeft = 300;
                        npc.netUpdate = true;
                    }
                    if (npc.life <= (int)((double)npc.lifeMax * 0.5) && revenge)
                    {
                        float spread = 45f * 0.0174f;
                        double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
                        double deltaAngle = spread / 8f;
                        double offsetAngle;
                        int damage = expertMode ? 42 : 58;
                        int i;
                        for (i = 0; i < 4; i++)
                        {
                            offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)(Math.Sin(offsetAngle) * 3f), (float)(Math.Cos(offsetAngle) * 3f), mod.ProjectileType("DarkEnergyBall"), damage, 0f, Main.myPlayer, passedVar, 0f);
                            passedVar += 1f;
                        }
                        passedVar = 1f;
                    }
                }
            }
            float num823 = 10f;
            float num824 = 0.2f;
            Vector2 vector82 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num825 = player.position.X + (float)(player.width / 2) - vector82.X;
            float num826 = player.position.Y + (float)(player.height / 2) - 300f - vector82.Y;
            float num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
            num827 = num823 / num827;
            num825 *= num827;
            num826 *= num827;
            if (npc.velocity.X < num825)
            {
                npc.velocity.X = npc.velocity.X + num824;
                if (npc.velocity.X < 0f && num825 > 0f)
                {
                    npc.velocity.X = npc.velocity.X + num824;
                }
            }
            else if (npc.velocity.X > num825)
            {
                npc.velocity.X = npc.velocity.X - num824;
                if (npc.velocity.X > 0f && num825 < 0f)
                {
                    npc.velocity.X = npc.velocity.X - num824;
                }
            }
            if (npc.velocity.Y < num826)
            {
                npc.velocity.Y = npc.velocity.Y + num824;
                if (npc.velocity.Y < 0f && num826 > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y + num824;
                }
            }
            else if (npc.velocity.Y > num826)
            {
                npc.velocity.Y = npc.velocity.Y - num824;
                if (npc.velocity.Y > 0f && num826 < 0f)
                {
                    npc.velocity.Y = npc.velocity.Y - num824;
                }
            }
            if (bossLife == 0f && npc.life > 0)
            {
                bossLife = (float)npc.lifeMax;
            }
            if (npc.life > 0)
            {
                if (Main.netMode != 1)
                {
                    int num660 = (int)((double)npc.lifeMax * 0.26);
                    if ((float)(npc.life + num660) < bossLife)
                    {
                        bossLife = (float)npc.life;
                        shootBoost += 1f;
                        int glob = revenge ? 8 : 4;
                        if (bossLife <= 0.5f)
                        {
                            glob = revenge ? 16 : 8;
                        }
                        for (int num662 = 0; num662 < glob; num662++)
                        {
                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 0f, mod.ProjectileType("DarkEnergySpawn"), 0, 0f, Main.myPlayer, 0f, 0f);
                        }
                        return;
                    }
                }
            }
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage = (crit ? 2 : 1);
            return false;
        }

        public override void NPCLoot()
		{
            npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("DarkPlasma"), Main.rand.Next(2, 4), true);
			if (Main.rand.Next(10) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CeaselessVoidTrophy"));
			}
			if (Main.rand.Next(3) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MirrorBlade"));
			}
			if (Main.rand.Next(5) == 0)
			{
                npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("ArcanumoftheVoid"), 1, true);
			}
		}
		
		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.SuperHealingPotion;
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
				npc.width = 100;
				npc.height = 100;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 40; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 70; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
                float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CeaselessVoid"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CeaselessVoid2"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CeaselessVoid3"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CeaselessVoid4"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CeaselessVoid5"), 1f);
            }
		}
	}
}