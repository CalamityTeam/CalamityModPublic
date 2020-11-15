using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Hybrid
{
	public class AccretionDiskProj : ModProjectile
	{
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/AccretionDisk";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Elemental Disk");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			projectile.width = 56;
			projectile.height = 56;
			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 6;
			projectile.penetrate = -1;
			projectile.aiStyle = 3;
			projectile.timeLeft = 400;
			aiType = ProjectileID.WoodenBoomerang;
			projectile.melee = true;
		}

		public override void AI()
		{
			if (Main.rand.NextBool(3))
			{
				int rainbow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, (float)(projectile.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
				Main.dust[rainbow].noGravity = true;
				Main.dust[rainbow].velocity *= 0f;
			}

			Lighting.AddLight(projectile.Center, 0.15f, 1f, 0.25f);

			float maxDistance = 300f;
			bool homeIn = false;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.CanBeChasedBy(projectile, false))
				{
					float extraDistance = (npc.width / 2) + (npc.height / 2);

					bool canHit = true;
					if (extraDistance < maxDistance)
						canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);

					if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance) && canHit)
					{
						homeIn = true;
						break;
					}
				}
			}

			if (!projectile.friendly)
			{
				homeIn = false;
			}

			if (homeIn)
			{
				if (Main.player[projectile.owner].miscCounter % 50 == 0)
				{
					int splitProj = ModContent.ProjectileType<AccretionDisk2>();
					if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[splitProj] < 25)
					{
						float spread = 45f * 0.0174f;
						double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
						double deltaAngle = spread / 8f;
						double offsetAngle;
						for (int i = 0; i < 4; i++)
						{
							offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
							int disk = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), splitProj, projectile.damage, projectile.knockBack, projectile.owner);
							Main.projectile[disk].Calamity().forceMelee = projectile.melee;
							Main.projectile[disk].Calamity().forceRogue = projectile.Calamity().rogue;
							int disk2 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), splitProj, projectile.damage, projectile.knockBack, projectile.owner);
							Main.projectile[disk2].Calamity().forceMelee = projectile.melee;
							Main.projectile[disk2].Calamity().forceRogue = projectile.Calamity().rogue;
						}
					}
				}
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
			target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
			target.AddBuff(ModContent.BuffType<Plague>(), 120);
			target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
		}

		public override void OnHitPvp(Player target, int damage, bool crit)
		{
			target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
			target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
			target.AddBuff(ModContent.BuffType<Plague>(), 120);
			target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
			return false;
		}
	}
}
