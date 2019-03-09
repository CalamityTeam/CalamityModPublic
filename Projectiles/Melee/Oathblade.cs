using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class Oathblade : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Oathblade");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.aiStyle = 18;
            projectile.alpha = 100;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.scale = 0.9f;
            projectile.tileCollide = true;
            projectile.penetrate = 3;
            aiType = 45;
        }
        
        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.35f) / 255f, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0.35f) / 255f);
        	if (Main.rand.Next(3) == 0)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	int immuneTime = 10 -
        		(NPC.downedMoonlord ? 8 : 0);
        	target.immune[projectile.owner] = immuneTime;
            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(BuffID.ShadowFlame, 180);
        }
    }
}