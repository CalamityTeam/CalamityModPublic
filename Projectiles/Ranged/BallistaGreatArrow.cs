using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
	public class BallistaGreatArrow : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ballista Great Arrow");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 3;
            projectile.aiStyle = 1;
            aiType = 1;
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.immune[projectile.owner] = 12;
        	target.AddBuff(mod.BuffType("ArmorCrunch"), 360);
        }
        
        public override void Kill(int timeLeft)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
	        float spread = 90f * 0.0174f;
			double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y)- spread/2;
			double deltaAngle = spread/8f;
			double offsetAngle;
			int i;
			if (projectile.owner == Main.myPlayer)
			{
				for (i = 0; i < 3; i++ )
				{
					offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( Math.Sin(offsetAngle) * 5f ), (float)( Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("FossilShard"), (int)((double)projectile.damage * 0.75f), projectile.knockBack, projectile.owner, 0f, 0f);
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("FossilShard"), (int)((double)projectile.damage * 0.75f), projectile.knockBack, projectile.owner, 0f, 0f);
				}
			}
        }
    }
}