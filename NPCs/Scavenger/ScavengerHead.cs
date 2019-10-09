using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.Scavenger
{
    public class ScavengerHead : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ravager");
		}

		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			npc.damage = 0;
			npc.width = 80;
			npc.height = 80;
			npc.defense = 50;
            npc.GetCalamityNPC().RevPlusDR(0.1f);
			npc.lifeMax = 32705;
			npc.knockBackResist = 0f;
			aiType = -1;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.buffImmune[BuffID.Ichor] = false;
			npc.buffImmune[BuffID.CursedInferno] = false;
			npc.buffImmune[BuffID.Daybreak] = false;
			npc.buffImmune[mod.BuffType("AbyssalFlames")] = false;
			npc.buffImmune[mod.BuffType("ArmorCrunch")] = false;
			npc.buffImmune[mod.BuffType("DemonFlames")] = false;
			npc.buffImmune[mod.BuffType("GodSlayerInferno")] = false;
			npc.buffImmune[mod.BuffType("HolyLight")] = false;
			npc.buffImmune[mod.BuffType("Nightwither")] = false;
			npc.buffImmune[mod.BuffType("Shred")] = false;
			npc.buffImmune[mod.BuffType("WhisperingDeath")] = false;
			npc.buffImmune[mod.BuffType("SilvaStun")] = false;
			npc.noGravity = true;
			npc.canGhostHeal = false;
			npc.noTileCollide = true;
			npc.alpha = 255;
			npc.value = Item.buyPrice(0, 0, 0, 0);
			npc.HitSound = SoundID.NPCHit41;
			npc.DeathSound = null;
			if (CalamityWorld.downedProvidence)
			{
				npc.defense = 150;
				npc.lifeMax = 260000;
			}
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = CalamityWorld.death ? 500000 : 450000;
			}
			double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
		}

		public override void AI()
		{
			bool provy = (CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive);
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			Player player = Main.player[npc.target];
			if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
			{
				npc.active = false;
				npc.netUpdate = true;
				return;
			}
			if (npc.timeLeft < 3000)
			{
				npc.timeLeft = 3000;
			}
			float speed = 12f;
			Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
			float centerX = Main.npc[CalamityGlobalNPC.scavenger].Center.X - center.X;
			float centerY = Main.npc[CalamityGlobalNPC.scavenger].Center.Y - center.Y;
			centerY -= 20f;
			centerX += 1f;
			float totalSpeed = (float)Math.Sqrt((double)(centerX * centerX + centerY * centerY));
			if (totalSpeed < 20f)
			{
				npc.rotation = 0f;
				npc.velocity.X = centerX;
				npc.velocity.Y = centerY;
			}
			else
			{
				totalSpeed = speed / totalSpeed;
				npc.velocity.X = centerX * totalSpeed;
				npc.velocity.Y = centerY * totalSpeed;
				npc.rotation = npc.velocity.X * 0.1f;
			}
			if (npc.alpha > 0)
			{
				npc.alpha -= 10;
				if (npc.alpha < 0)
				{
					npc.alpha = 0;
				}
				npc.ai[1] = 30f;
			}
			npc.ai[1] += 1f;
			int nukeTimer = 450;
			if (npc.ai[1] >= (float)nukeTimer)
			{
				Main.PlaySound(SoundID.Item62, npc.position);
				npc.TargetClosest(true);
				npc.ai[1] = 0f;
				Vector2 shootFromVector = new Vector2(npc.Center.X, npc.Center.Y - 20f);
				float nukeSpeed = 1f;
				float playerDistanceX = player.position.X + (float)player.width * 0.5f - shootFromVector.X;
				float playerDistanceY = player.position.Y + (float)player.height * 0.5f - shootFromVector.Y;
				float totalPlayerDistance = (float)Math.Sqrt((double)(playerDistanceX * playerDistanceX + playerDistanceY * playerDistanceY));
				totalPlayerDistance = nukeSpeed / totalPlayerDistance;
				playerDistanceX *= totalPlayerDistance;
				playerDistanceY *= totalPlayerDistance;
				int nukeDamage = expertMode ? 45 : 60;
				int projectileType = mod.ProjectileType("ScavengerNuke");
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int nuke = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, playerDistanceX, playerDistanceY, projectileType, nukeDamage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
					Main.projectile[nuke].velocity.Y = -15f;
				}
			}
		}

		public override bool PreNPCLoot()
		{
			return false;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life > 0)
			{
				int num285 = 0;
				while ((double)num285 < damage / (double)npc.lifeMax * 100.0)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)hitDirection, -1f, 0, default, 1f);
					num285++;
				}
			}
			else if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, mod.NPCType("ScavengerHead2"), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
			}
		}
	}
}
