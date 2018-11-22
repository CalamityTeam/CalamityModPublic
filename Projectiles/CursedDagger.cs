using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class CursedDagger : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cursed Dagger");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.thrown = true;
            projectile.penetrate = 3;
            projectile.aiStyle = 2;
            projectile.timeLeft = 600;
            aiType = 48;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                projectile.ai[0] += 0.1f;
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
	    {
			target.AddBuff(BuffID.CursedInferno, 120);
		}
    }
}