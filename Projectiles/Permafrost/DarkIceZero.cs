using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Permafrost
{
	public class DarkIceZero : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 28;
			projectile.height = 28;
			projectile.aiStyle = 1;
			aiType = ProjectileID.Bullet;
			projectile.friendly = true;
            projectile.melee = true;
			projectile.penetrate = 1;
			projectile.ignoreWater = true;
			projectile.extraUpdates = 1;
            projectile.coldDamage = true;
        }
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dark Ice");
		}

		public override void AI()
		{
            if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 16f)
            {
                projectile.velocity *= 1.035f;
            }

            //make pretty dust
            int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 172, projectile.velocity.X, projectile.velocity.Y, 0, default(Color), 1.25f);
            Main.dust[index2].noGravity = true;
        }
		
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
            target.AddBuff(BuffID.Frostburn, 480);
            target.AddBuff(mod.BuffType("GlacialState"), 240);
        }

		public override Color? GetAlpha (Color lightColor)
		{
			return new Color(198, 197, 246);
		}

		public override void Kill(int timeLeft)
		{
            if (timeLeft > 0)
            {
                timeLeft = 0;
                Main.PlaySound(SoundID.Item27, projectile.position);
                for (int i = 0; i < 30; i++)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 172, 0f, 0f, 0, default(Color), Main.rand.NextFloat(1f, 2f));
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 4f;
                }
                projectile.position = projectile.Center;
                projectile.width = 200;
                projectile.height = 200;
                projectile.position.X -= projectile.width / 2;
                projectile.position.Y -= projectile.height / 2;
                for (int index1 = 0; index1 < 20; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 68, 0f, 0f, 0, new Color(), 1.3f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 1.5f;
                }
                projectile.penetrate = -1;
                projectile.damage /= 2;
                projectile.Damage();
            }
        }
	}
}