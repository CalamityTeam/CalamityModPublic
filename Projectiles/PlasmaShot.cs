using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class PlasmaShot : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shot");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 600;
            projectile.light = 0.25f;
        }

        public override void AI()
        {
			for (int num121 = 0; num121 < 5; num121++)
			{
				Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 107, projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 1f)];
				dust.velocity = Vector2.Zero;
				dust.position -= projectile.velocity / 5f * (float)num121;
				dust.noGravity = true;
				dust.scale = 0.8f;
				dust.noLight = true;
			}
        }

        public override void Kill(int timeLeft)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
        	int num220 = Main.rand.Next(20, 31);
        	if (projectile.owner == Main.myPlayer)
        	{
				for (int num221 = 0; num221 < num220; num221++)
				{
					Vector2 value17 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					value17.Normalize();
					value17 *= (float)Main.rand.Next(10, 201) * 0.01f;
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value17.X, value17.Y, 511 + Main.rand.Next(3), projectile.damage, 1f, projectile.owner, 0f, (float)Main.rand.Next(-45, 1));
				}
        	}
            for (int k = 0; k < 5; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 107, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}