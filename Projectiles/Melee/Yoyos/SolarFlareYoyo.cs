using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
	public class SolarFlareYoyo : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Solar Flare");
		}

		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.TheEyeOfCthulhu);
			projectile.width = 16;
			projectile.scale = 1.15f;
			projectile.height = 16;
			projectile.penetrate = -1;
			projectile.extraUpdates = 1;
			aiType = 555;
			projectile.melee = true;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 5;
		}

		public override void AI()
		{
			if (Main.rand.Next(5) == 0)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(mod.BuffType("HolyLight"), 300);
			if (projectile.owner == Main.myPlayer)
			{
				int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("HolyExplosionSupreme"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
				Main.projectile[proj].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceMelee = true;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = Main.projectileTexture[projectile.type];
			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
	}
}