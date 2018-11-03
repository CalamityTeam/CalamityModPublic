using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class VanquisherArrow : ModProjectile
    {
    	public int projCount = 18;
    	
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Arrow");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 90;
            projectile.extraUpdates = 1;
        }
        
        public override void AI()
        {
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
        	projCount--;
        	if (projCount <= 0)
        	{
        		if (projectile.owner == Main.myPlayer)
        		{
        			Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, mod.ProjectileType("VanquisherArrow2"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
        		}
        		projCount = 18;
        	}
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("GodSlayerInferno"), 500);
        }
    }
}