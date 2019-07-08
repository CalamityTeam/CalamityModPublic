using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;
using CalamityMod.World;

namespace CalamityMod.NPCs.SlimeGod
{
	public class SlimeSpawnCrimson2 : ModNPC
	{
		public float spikeTimer = 60f;
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spiked Crimson Slime Spawn");
			Main.npcFrameCount[npc.type] = 2;
		}
		
		public override void SetDefaults()
		{
			npc.aiStyle = 1;
			npc.damage = 30;
			npc.width = 40; //324
			npc.height = 30; //216
			npc.defense = 10;
			npc.lifeMax = 130;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 120000;
            }
            npc.knockBackResist = 0f;
			animationType = 81;
			npc.alpha = 60;
			npc.lavaImmune = false;
			npc.noGravity = false;
			npc.noTileCollide = false;
			npc.canGhostHeal = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.buffImmune[24] = true;
		}
		
		public override void AI()
		{
			if (spikeTimer > 0f)
			{
				spikeTimer -= 1f;
			}
			if (!npc.wet)
			{
				Vector2 vector3 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num14 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector3.X;
				float num15 = Main.player[npc.target].position.Y - vector3.Y;
				float num16 = (float)Math.Sqrt((double)(num14 * num14 + num15 * num15));
				if (Main.expertMode && num16 < 120f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && npc.velocity.Y == 0f)
				{
					npc.ai[0] = -40f;
					if (npc.velocity.Y == 0f)
					{
						npc.velocity.X = npc.velocity.X * 0.9f;
					}
					if (Main.netMode != 1 && spikeTimer == 0f)
					{
						for (int n = 0; n < 5; n++)
						{
							Vector2 vector4 = new Vector2((float)(n - 2), -4f);
							vector4.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
							vector4.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
							vector4.Normalize();
							vector4 *= 4f + (float)Main.rand.Next(-50, 51) * 0.01f;
							Projectile.NewProjectile(vector3.X, vector3.Y, vector4.X, vector4.Y, mod.ProjectileType("CrimsonSpike"), 13, 0f, Main.myPlayer, 0f, 0f);
							spikeTimer = 30f;
						}
					}
				}
				else if (num16 < 360f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && npc.velocity.Y == 0f)
				{
					npc.ai[0] = -40f;
					if (npc.velocity.Y == 0f)
					{
						npc.velocity.X = npc.velocity.X * 0.9f;
					}
					if (Main.netMode != 1 && spikeTimer == 0f)
					{
						num15 = Main.player[npc.target].position.Y - vector3.Y - (float)Main.rand.Next(0, 200);
						num16 = (float)Math.Sqrt((double)(num14 * num14 + num15 * num15));
						num16 = 6.5f / num16;
						num14 *= num16;
						num15 *= num16;
						spikeTimer = 50f;
						Projectile.NewProjectile(vector3.X, vector3.Y, num14, num15, mod.ProjectileType("CrimsonSpike"), 12, 0f, Main.myPlayer, 0f, 0f);
					}
				}
			}
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
		
		public override void NPCLoot()
		{
			if (Main.expertMode)
			{
				if (Main.rand.Next(50) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Nazar);
				}
			}
			else if (Main.rand.Next(100) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Nazar);
			}
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.Cursed, 60, true);
		}
	}
}