using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.Perforator
{
	public class PerforatorBodyMedium : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Perforator");
		}
		
		public override void SetDefaults()
		{
			npc.damage = 25; //70
			npc.npcSlots = 5f;
			npc.width = 54; //324
			npc.height = 54; //216
			npc.defense = 10;
			npc.lifeMax = 3000; //250000
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 1100000 : 900000;
            }
            npc.aiStyle = 6; //new
            aiType = -1; //new
            animationType = 10; //new
			npc.knockBackResist = 0f;
			npc.alpha = 255;
			npc.buffImmune[mod.BuffType("GlacialState")] = true;
			npc.buffImmune[mod.BuffType("TemporalSadness")] = true;
            npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.canGhostHeal = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.netAlways = true;
			npc.dontCountMe = true;
		}
		
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
		
		public override void AI()
		{
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			if (Main.netMode != 1)
			{
				int shoot = revenge ? 5 : 4;
				npc.localAI[0] += (float)Main.rand.Next(shoot);
				if (npc.localAI[0] >= (float)Main.rand.Next(4000, 12000))
				{
					npc.localAI[0] = 0f;
					npc.TargetClosest(true);
					if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
					{
						float num941 = revenge ? 9f : 8f;
						Vector2 vector104 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
						float num942 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector104.X + (float)Main.rand.Next(-20, 21);
						float num943 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector104.Y + (float)Main.rand.Next(-20, 21);
						float num944 = (float)Math.Sqrt((double)(num942 * num942 + num943 * num943));
						num944 = num941 / num944;
						num942 *= num944;
						num943 *= num944;
						num942 += (float)Main.rand.Next(-50, 51) * 0.05f;
						num943 += (float)Main.rand.Next(-50, 51) * 0.05f;
						int num945 = expertMode ? 9 : 11;
						int num946 = mod.ProjectileType("BloodClot");
						vector104.X += num942 * 5f;
						vector104.Y += num943 * 5f;
                        if (Main.rand.Next(2) == 0)
                        {
                            int num947 = Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, num946, num945, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[num947].timeLeft = 160;
                        }
						npc.netUpdate = true;
					}
				}
			}
			if (!Main.npc[(int)npc.ai[1]].active)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.active = false;
            }
			if (Main.npc[(int)npc.ai[1]].alpha < 128)
			{
				npc.alpha -= 42;
				if (npc.alpha < 0)
				{
					npc.alpha = 0;
				}
			}
		}
		
		public override bool CheckActive()
		{
			return false;
		}
		
		public override bool PreNPCLoot()
		{
			return false;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 10; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
				}
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/MediumPerf2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/MediumPerf3"), 1f);
            }
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.7f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.7f);
		}

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(mod.BuffType("BurningBlood"), 120, true);
            player.AddBuff(BuffID.Bleeding, 120, true);
        }
    }
}