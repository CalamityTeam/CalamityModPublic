using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
	public class FallenStarProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fallen Star");
		}

		public override void SetDefaults()
		{
			projectile.width = 16;
			projectile.height = 16;
			projectile.aiStyle = 0;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.penetrate = -1;
			projectile.alpha = 50;
			projectile.light = 1f;
			projectile.tileCollide = true;
			projectile.ignoreWater = false;
			projectile.usesLocalNPCImmunity = true;
		}

		public override void AI()
		{
			if (projectile.soundDelay == 0)
			{
				projectile.soundDelay = 20 + Main.rand.Next(40);
				Main.PlaySound(SoundID.Item9, projectile.position);
			}
			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;
			}
			projectile.alpha += (int)(25f * projectile.localAI[0]);
			if (projectile.alpha > 200)
			{
				projectile.alpha = 200;
				projectile.localAI[0] = -1f;
			}
			if (projectile.alpha < 0)
			{
				projectile.alpha = 0;
				projectile.localAI[0] = 1f;
			}
			projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			float newdamage = projectile.damage * 0.9375f;
			projectile.damage = (int)newdamage;
			if (projectile.damage < 1)
				projectile.damage = 1;
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.Item10, projectile.position);
			int dustAmt = 10;
			int goreAmt = 3;
			for (int i = 0; i < dustAmt; i++)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 58, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, default(Color), 1.2f);
			}
			for (int i = 0; i < goreAmt; i++)
			{
				Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
			}
			for (int i = 0; i < dustAmt; i++)
			{
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 57, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, default(Color), 1.2f);
			}
			for (int i = 0; i < goreAmt; i++)
			{
				Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
			}
		}

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, projectile.alpha);
        }
	}
}