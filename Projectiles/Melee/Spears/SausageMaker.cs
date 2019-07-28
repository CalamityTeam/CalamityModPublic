using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class SausageMaker : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spear");
		}

		public override void SetDefaults()
		{
			projectile.width = 44;  //The width of the .png file in pixels divided by 2.
			projectile.aiStyle = 19;
			projectile.melee = true;  //Dictates whether this is a melee-class weapon.
			projectile.timeLeft = 90;
			projectile.height = 42;  //The height of the .png file in pixels divided by 2.
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.penetrate = -1;
			projectile.ownerHitCheck = true;
			projectile.hide = true;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).trueMelee = true;
		}

		public override void AI()
		{
			Main.player[projectile.owner].direction = projectile.direction;
			Main.player[projectile.owner].heldProj = projectile.whoAmI;
			Main.player[projectile.owner].itemTime = Main.player[projectile.owner].itemAnimation;
			projectile.position.X = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - (float)(projectile.width / 2);
			projectile.position.Y = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - (float)(projectile.height / 2);
			projectile.position += projectile.velocity * projectile.ai[0];
			if (Main.rand.Next(5) == 0)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 5, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
			}
			if (projectile.ai[0] == 0f)
			{
				projectile.ai[0] = 3f;
				projectile.netUpdate = true;
			}
			if (Main.player[projectile.owner].itemAnimation < Main.player[projectile.owner].itemAnimationMax / 3)
			{
				projectile.ai[0] -= 1.1f;
			}
			else
			{
				projectile.ai[0] += 0.95f;
			}

			if (Main.player[projectile.owner].itemAnimation == 0)
			{
				projectile.Kill();
			}

			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
			if (projectile.spriteDirection == -1)
			{
				projectile.rotation -= 1.57f;
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (projectile.owner == Main.myPlayer)
			{
				for (int i = 0; i < 2; i++)
				{
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 1.2f, projectile.velocity.Y * 1.2f, mod.ProjectileType("Blood2"), projectile.damage / 2, projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
				}
			}
		}
	}
}
