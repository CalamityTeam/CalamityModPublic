using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.ProfanedGuardianBoss
{
    [AutoloadBossHead]
    public class ProfanedGuardianBoss3 : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Profaned Guardian");
			Main.npcFrameCount[npc.type] = 6;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 3f;
			npc.aiStyle = -1;
			npc.damage = 100;
			npc.width = 100; //324
			npc.height = 80; //216
			npc.defense = 25;
			npc.lifeMax = 25000;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 240000 : 200000;
            }
            npc.knockBackResist = 0f;
			npc.noGravity = true;
			npc.noTileCollide = true;
			aiType = -1;
			npc.boss = true;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Guardians");
            for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
				npc.buffImmune[BuffID.Ichor] = false;
                npc.buffImmune[BuffID.CursedInferno] = false;
                npc.buffImmune[mod.BuffType("AbyssalFlames")] = false;
                npc.buffImmune[mod.BuffType("ArmorCrunch")] = false;
                npc.buffImmune[mod.BuffType("DemonFlames")] = false;
                npc.buffImmune[mod.BuffType("GodSlayerInferno")] = false;
                npc.buffImmune[mod.BuffType("Nightwither")] = false;
                npc.buffImmune[mod.BuffType("Shred")] = false;
                npc.buffImmune[mod.BuffType("WhisperingDeath")] = false;
                npc.buffImmune[mod.BuffType("SilvaStun")] = false;
            }
			npc.value = Item.buyPrice(1, 0, 0, 0);
			npc.HitSound = SoundID.NPCHit52;
			npc.DeathSound = SoundID.NPCDeath55;
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
			bool expertMode = Main.expertMode;
			bool isHoly = Main.player[npc.target].ZoneHoly;
			bool isHell = Main.player[npc.target].ZoneUnderworldHeight;
			npc.defense = (isHoly || isHell || CalamityWorld.bossRushActive) ? 25 : 99999;
			Vector2 vectorCenter = npc.Center;
			Player player = Main.player[npc.target];
			npc.TargetClosest(false);
			if (!player.active || player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead)
				{
					npc.velocity = new Vector2(0f, -10f);
					if (npc.timeLeft > 150)
					{
						npc.timeLeft = 150;
					}
					return;
				}
			}
			else if (npc.timeLeft < 1800)
			{
				npc.timeLeft = 1800;
			}
			if (Math.Sign(npc.velocity.X) != 0) 
			{
				npc.spriteDirection = -Math.Sign(npc.velocity.X);
			}
			npc.spriteDirection = Math.Sign(npc.velocity.X);
			int num1009 = (npc.ai[0] == 0f) ? 1 : 2;
			int num1010 = (npc.ai[0] == 0f) ? 60 : 80;
			for (int num1011 = 0; num1011 < 2; num1011++) 
			{
				if (Main.rand.Next(3) < num1009) 
				{
					int dustType = Main.rand.Next(2);
					if (dustType == 0)
					{
						dustType = 244;
					}
					else
					{
						dustType = 107;
					}
					int num1012 = Dust.NewDust(npc.Center - new Vector2((float)num1010), num1010 * 2, num1010 * 2, dustType, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default(Color), 1.5f);
					Main.dust[num1012].noGravity = true;
					Main.dust[num1012].velocity *= 0.2f;
					Main.dust[num1012].fadeIn = 1f;
				}
			}
			if (CalamityGlobalNPC.doughnutBoss < 0) 
			{
				npc.active = false;
				npc.netUpdate = true;
				return;
			}
			if (npc.ai[0] == 0f) 
			{
				Vector2 vector96 = new Vector2(npc.Center.X, npc.Center.Y);
				float num784 = Main.npc[CalamityGlobalNPC.doughnutBoss].Center.X - vector96.X;
				float num785 = Main.npc[CalamityGlobalNPC.doughnutBoss].Center.Y - vector96.Y;
				float num786 = (float)Math.Sqrt((double)(num784 * num784 + num785 * num785));
				if (num786 > 90f) 
				{
					num786 = 24f / num786; //8f
					num784 *= num786;
					num785 *= num786;
					npc.velocity.X = (npc.velocity.X * 15f + num784) / 16f;
					npc.velocity.Y = (npc.velocity.Y * 15f + num785) / 16f;
					return;
				}
				if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < 24f) //8f
				{
					npc.velocity.Y = npc.velocity.Y * 1.15f; //1.05f
					npc.velocity.X = npc.velocity.X * 1.15f; //1.05f
				}
				if (Main.netMode != 1 && ((expertMode && Main.rand.Next(50) == 0) || Main.rand.Next(100) == 0)) 
				{
					npc.TargetClosest(true);
					vector96 = new Vector2(npc.Center.X, npc.Center.Y);
					num784 = player.Center.X - vector96.X;
					num785 = player.Center.Y - vector96.Y;
					num786 = (float)Math.Sqrt((double)(num784 * num784 + num785 * num785));
					num786 = 24f / num786; //8f
					npc.velocity.X = num784 * num786;
					npc.velocity.Y = num785 * num786;
					npc.ai[0] = 1f;
					npc.netUpdate = true;
					return;
				}
			} 
			else 
			{
				Vector2 value4 = player.Center - npc.Center;
				value4.Normalize();
				value4 *= 27f; //9f
				npc.velocity = (npc.velocity * 99f + value4) / 100f;
				Vector2 vector97 = new Vector2(npc.Center.X, npc.Center.Y);
				float num787 = Main.npc[CalamityGlobalNPC.doughnutBoss].Center.X - vector97.X;
				float num788 = Main.npc[CalamityGlobalNPC.doughnutBoss].Center.Y - vector97.Y;
				float num789 = (float)Math.Sqrt((double)(num787 * num787 + num788 * num788));
				if (num789 > 700f) 
				{
					npc.ai[0] = 0f;
					return;
				}
			}
			if (Main.netMode != 1) 
			{
				npc.localAI[0] += expertMode ? 2f : 1f;
				if (npc.localAI[0] >= 600f)
				{
					npc.localAI[0] = 0f;
					npc.TargetClosest(true);
					if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
					{
						Main.PlaySound(SoundID.Item20, npc.position);
						Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float spread = 45f * 0.0174f;
				    	double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y)- spread/2;
				    	double deltaAngle = spread/8f;
				    	double offsetAngle;
				    	int damage = 0;
				    	int projectileShot = mod.ProjectileType("HealOrbProv");
				    	int i;
				    	for (i = 0; i < 3; i++ )
				    	{
				   			offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
				        	Projectile.NewProjectile(value9.X, value9.Y, (float)( Math.Sin(offsetAngle) * 5f ), (float)( Math.Cos(offsetAngle) * 5f ), projectileShot, damage, 0f, Main.myPlayer, 0f, 0f);
				        	Projectile.NewProjectile(value9.X, value9.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), projectileShot, damage, 0f, Main.myPlayer, 0f, 0f);
				    	}
					}
				}
			}
		}
		
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;
			return true;
		}
		
		public override void BossLoot(ref string name, ref int potionType)
		{
			name = "A Profaned Guardian";
			potionType = ItemID.GreaterHealingPotion;
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.OnFire, 600, true);
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.7f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.7f);
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 50; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}