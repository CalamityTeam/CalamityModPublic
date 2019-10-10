using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class GacruxianHome : ModProjectile
	{
		private int sparkTrailTimer = 10;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mollusk");
		}

		public override void SetDefaults()
		{
			projectile.width = 20;
			projectile.height = 20;
			projectile.aiStyle = 18;
			projectile.friendly = true;
			projectile.Calamity().rogue = true;
			projectile.penetrate = 5;
			projectile.timeLeft = 180;
			projectile.ignoreWater = true;
			aiType = 274;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 5;
		}

		public override void AI()
		{
			sparkTrailTimer--;
			if (Main.rand.NextBool(4))
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, mod.DustType("AstralOrange"), projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
			}
			if (sparkTrailTimer == 0)
			{
				if (projectile.owner == Main.myPlayer)
				{
					int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0f, projectile.velocity.Y * 0f, mod.ProjectileType("UltimusCleaverDust"), (int)((double)projectile.damage * 0.75), projectile.knockBack, projectile.owner, 0f, 0f);
					Main.projectile[proj].Calamity().forceRogue = true;
				}
				sparkTrailTimer = 10;
			}
			float num472 = projectile.Center.X;
			float num473 = projectile.Center.Y;
			float num474 = 600f;
			bool flag17 = false;
			for (int num475 = 0; num475 < 200; num475++)
			{
				if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1))
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
				float num483 = 25f;
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

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i <= 10; i++)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, mod.DustType("AstralOrange"), projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
			}
		}
	}
}
