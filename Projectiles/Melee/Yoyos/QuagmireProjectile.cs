using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
	public class QuagmireProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Quagmire");
		}

		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.HelFire);
			projectile.width = 16;
			projectile.scale = 1.25f;
			projectile.height = 16;
			projectile.penetrate = 8;
			projectile.melee = true;
			aiType = 553;
		}

		public override void AI()
		{
			if (Main.rand.Next(5) == 0)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 44, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
			}
			if (projectile.owner == Main.myPlayer)
			{
				if (Main.rand.Next(10) == 0)
				{
					int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.35f, projectile.velocity.Y * 0.35f, 569, (int)((double)projectile.damage * 0.65), projectile.knockBack, projectile.owner, 0f, 0f);
					Main.projectile[proj].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceMelee = true;
				}
				if (Main.rand.Next(30) == 0)
				{
					int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 570, (int)((double)projectile.damage * 0.75), projectile.knockBack, projectile.owner, 0f, 0f);
					Main.projectile[proj].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceMelee = true;
				}
				if (Main.rand.Next(50) == 0)
				{
					int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.15f, projectile.velocity.Y * 0.15f, 571, (int)((double)projectile.damage * 0.85), projectile.knockBack, projectile.owner, 0f, 0f);
					Main.projectile[proj].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceMelee = true;
				}
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.Venom, 300);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = Main.projectileTexture[projectile.type];
			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
	}
}