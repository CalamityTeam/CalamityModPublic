using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class ChaosFlameLarge : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flame");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 50;
            projectile.aiStyle = 44;
            projectile.alpha = 100;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.alpha = 100;
            projectile.timeLeft = 600;
            aiType = 228;
        }
        
        public override void AI()
		{
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.4f) / 255f, ((255 - projectile.alpha) * 0.01f) / 255f, ((255 - projectile.alpha) * 0.01f) / 255f);
        	if (Main.rand.Next(4) == 0)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 174, 0f, 0f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 174, 0f, 0f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 480);
        }
    }
}