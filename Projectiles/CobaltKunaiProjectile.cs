using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class CobaltKunaiProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Kunai");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.scale = 0.75f;
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
        
        public override void Kill(int timeLeft)
        {
        	if (Main.rand.Next(2) == 0)
        	{
        		Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, mod.ItemType("CobaltKunai"));
        	}
        }
    }
}