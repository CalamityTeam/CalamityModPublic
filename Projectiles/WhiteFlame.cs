using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class WhiteFlame : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flame");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 120;
            projectile.magic = true;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.5f) / 255f, ((255 - projectile.alpha) * 0.5f) / 255f, ((255 - projectile.alpha) * 0.5f) / 255f);
			for (int num457 = 0; num457 < 5; num457++)
			{
				int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 91, 0f, 0f, 100, default(Color), 1f);
				Main.dust[num458].noGravity = true;
				Main.dust[num458].velocity *= 0.5f;
				Main.dust[num458].velocity += projectile.velocity * 0.1f;
			}
			return;
        }
        
        public override void Kill(int timeLeft)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 122);
        	int num251 = 4;
        	if (projectile.owner == Main.myPlayer)
        	{
				for (int num252 = 0; num252 < num251; num252++)
				{
					Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					while (value15.X == 0f && value15.Y == 0f)
					{
						value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					}
					value15.Normalize();
					value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
					Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), value15.X, value15.Y, mod.ProjectileType("WhiteFlame2"), (int)((double)projectile.damage), 0f, projectile.owner, 0f, 0f);
				}
        	}
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("HolyLight"), 360);
        }
    }
}