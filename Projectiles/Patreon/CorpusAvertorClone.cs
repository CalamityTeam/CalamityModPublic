using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Patreon
{
    public class CorpusAvertorClone : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Corpus Avertor");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.width = 24;
			projectile.height = 24;
			projectile.friendly = true;
			projectile.penetrate = 1;
			projectile.timeLeft = 180;
			projectile.melee = true;
			projectile.tileCollide = false;
		}

		public override void AI()
		{
			if (projectile.ai[1] == 1f)
			{
				projectile.melee = false;
				projectile.Calamity().rogue = true;
			}

			projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.04f;

			projectile.velocity.X *= 1.005f;
			projectile.velocity.Y *= 1.005f;

			switch ((int)projectile.ai[0])
			{
				case 20:
					projectile.scale = 0.7f;
					break;
				case 40:
					projectile.scale = 0.8f;
					break;
				case 60:
					projectile.scale = 0.9f;
					break;
				default:
					break;
			}
			projectile.width = (projectile.height = (int)(24f * projectile.scale));

			float num472 = projectile.Center.X;
			float num473 = projectile.Center.Y;
			float num474 = 300f;
			bool flag17 = false;

			for (int num475 = 0; num475 < 200; num475++)
			{
				if (Main.npc[num475].CanBeChasedBy(projectile, false))
				{
					float num476 = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
					float num477 = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
					float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
					if (num478 < num474)
					{
						num474 = num478;
						num472 = num476;
						num473 = num477;
						flag17 = true;
					}
				}
			}

			if (flag17)
			{
				float num483 = 16f;
				Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num484 = num472 - vector35.X;
				float num485 = num473 - vector35.Y;
				float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
				num486 = num483 / num486;
				num484 *= num486;
				num485 *= num486;
				projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
				projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			if (projectile.timeLeft < 85)
			{
				return new Color((int)(150f * ((float)projectile.timeLeft / 85f)), 0, 0, (projectile.timeLeft / 5) * 3);
			}
			return new Color(150, 0, 0, 50);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
			return false;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (target.type == NPCID.TargetDummy)
				return;

			float heal = (float)damage * 0.05f;
			if ((int)heal == 0)
				return;
			if (Main.player[Main.myPlayer].lifeSteal <= 0f)
				return;

			Main.player[Main.myPlayer].lifeSteal -= heal * 1.5f;
			int owner = projectile.owner;
			Projectile.NewProjectile(target.position.X, target.position.Y, 0f, 0f, ProjectileID.VampireHeal, 0, 0f, projectile.owner, (float)owner, heal);
		}
	}
}
