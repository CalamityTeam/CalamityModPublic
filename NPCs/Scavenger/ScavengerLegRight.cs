using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.Scavenger
{
    public class ScavengerLegRight : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ravager");
		}

		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			npc.damage = 0;
			npc.width = 60; //324
			npc.height = 60; //216
			npc.defense = 60;
			npc.lifeMax = 21010;
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
			npc.noGravity = true;
			npc.canGhostHeal = false;
			npc.noTileCollide = true;
			npc.alpha = 255;
			npc.value = Item.buyPrice(0, 0, 0, 0);
			npc.HitSound = SoundID.NPCHit41;
			npc.DeathSound = SoundID.NPCDeath14;
			if (CalamityWorld.downedProvidence)
			{
				npc.defense = 135;
				npc.lifeMax = 200000;
			}
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = CalamityWorld.death ? 450000 : 400000;
			}
			double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
		}

		public override void AI()
		{
			bool provy = (CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive);
			Vector2 center = npc.Center;
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
			if (npc.alpha > 0)
			{
				npc.alpha -= 10;
				if (npc.alpha < 0)
				{
					npc.alpha = 0;
				}
				npc.ai[1] = 0f;
			}
			if (Main.npc[CalamityGlobalNPC.scavenger].ai[0] == 1f && Main.npc[CalamityGlobalNPC.scavenger].velocity.Y == 0f)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int smash = Projectile.NewProjectile((float)center.X, (float)center.Y, 0f, 0f, mod.ProjectileType("HiveExplosion"), 35 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
					Main.projectile[smash].timeLeft = 30;
					Main.projectile[smash].hostile = true;
					Main.projectile[smash].friendly = false;
				}
			}
			if (npc.ai[0] == 0f)
			{
				float num659 = 14f;
				if (npc.life < npc.lifeMax / 2)
				{
					num659 += 3f;
				}
				if (npc.life < npc.lifeMax / 3)
				{
					num659 += 3f;
				}
				if (npc.life < npc.lifeMax / 5)
				{
					num659 += 8f;
				}
				Vector2 vector79 = new Vector2(center.X, center.Y);
				float num660 = Main.npc[CalamityGlobalNPC.scavenger].Center.X - vector79.X;
				float num661 = Main.npc[CalamityGlobalNPC.scavenger].Center.Y - vector79.Y;
				num661 += 88f;
				num660 += 70f;
				float num662 = (float)Math.Sqrt((double)(num660 * num660 + num661 * num661));
				if (num662 < 12f + num659)
				{
					npc.rotation = 0f;
					npc.velocity.X = num660;
					npc.velocity.Y = num661;
				}
				else
				{
					num662 = num659 / num662;
					npc.velocity.X = num660 * num662;
					npc.velocity.Y = num661 * num662;
				}
			}
		}

		public override bool PreNPCLoot()
		{
			return false;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
				Dust.NewDust(npc.position, npc.width, npc.height, 6, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerLegRight"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerLegRight2"), 1f);
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
					Dust.NewDust(npc.position, npc.width, npc.height, 6, hitDirection, -1f, 0, default, 1f);
				}
			}
		}
	}
}
