using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.AstrumDeus
{
    public class AstrumDeusTail : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astrum Deus");
		}

		public override void SetDefaults()
		{
			npc.damage = 70;
			npc.npcSlots = 5f;
			npc.width = 52;
			npc.height = 68;
			npc.defense = 60;
			npc.lifeMax = CalamityWorld.revenge ? 18000 : 12000;
			if (CalamityWorld.death)
			{
				npc.lifeMax = 29100;
			}
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = CalamityWorld.death ? 420000 : 360000;
			}
			double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
			npc.aiStyle = 6;
			aiType = -1;
			npc.knockBackResist = 0f;
			npc.scale = 1.2f;
			if (Main.expertMode)
			{
				npc.scale = 1.35f;
			}
			npc.alpha = 255;
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.canGhostHeal = false;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.netAlways = true;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.dontCountMe = true;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(npc.dontTakeDamage);
			writer.Write(npc.chaseable);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc.dontTakeDamage = reader.ReadBoolean();
			npc.chaseable = reader.ReadBoolean();
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

		public override void AI()
		{
			if (CalamityGlobalNPC.astrumDeusHeadMain != -1)
			{
				if (Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].active)
				{
					npc.dontTakeDamage = !Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].dontTakeDamage;
					npc.chaseable = !Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].chaseable;
				}
			}
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
			if (!Main.npc[(int)npc.ai[1]].active)
			{
				npc.life = 0;
				npc.HitEffect(0, 10.0);
				npc.active = false;
			}
			if (Main.npc[(int)npc.ai[1]].alpha < 128)
			{
				if (npc.alpha != 0)
				{
					for (int num934 = 0; num934 < 2; num934++)
					{
						int num935 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
						Main.dust[num935].noGravity = true;
						Main.dust[num935].noLight = true;
					}
				}
				npc.alpha -= 42;
				if (npc.alpha < 0)
				{
					npc.alpha = 0;
				}
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return !npc.dontTakeDamage;
		}

		public override Color? GetAlpha(Color drawColor)
		{
			if (npc.dontTakeDamage)
				return new Color(125, 75, Main.DiscoB, npc.alpha);
			return null;
		}

		public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (projectile.type == mod.ProjectileType("RainbowBoom") || ProjectileID.Sets.StardustDragon[projectile.type])
			{
				damage = (int)((double)damage * 0.1);
			}
			else if (projectile.type == mod.ProjectileType("RainBolt") || projectile.type == mod.ProjectileType("AtlantisSpear2") || projectile.type == mod.ProjectileType("MalachiteBolt"))
			{
				damage = (int)((double)damage * 0.2);
			}
			else if (projectile.type == ProjectileID.DD2BetsyArrow)
			{
				damage = (int)((double)damage * 0.3);
			}
			else if (projectile.type == mod.ProjectileType("SpikecragSpike"))
			{
				damage = (int)((double)damage * 0.5);
			}

			if (projectile.penetrate == -1 && !projectile.minion)
			{
				if (projectile.type == mod.ProjectileType("CosmicFire") || projectile.type == mod.ProjectileType("BigNuke"))
					damage = (int)((double)damage * 0.3);
				else
					damage = (int)((double)damage * 0.2);
			}
			else if (projectile.penetrate > 1 && projectile.type != mod.ProjectileType("BrinySpout"))
			{
				damage /= projectile.penetrate;
			}
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 50;
				npc.height = 50;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 5; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.NextBool(2))
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 10; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default, 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default, 2f);
					Main.dust[num624].velocity *= 2f;
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

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("AstralInfectionDebuff"), 120, true);
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.85f);
		}
	}
}
