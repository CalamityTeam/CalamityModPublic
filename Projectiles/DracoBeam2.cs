using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class DracoBeam2 : ModProjectile
    {
    	public int start = 60;
    	public int speedTimer = 120;
    	
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Beam");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.aiStyle = 27;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 240;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 1;
        }

        public override void AI()
        {
        	start--;
        	if (start <= 0)
        	{
        		speedTimer--;
        		if (speedTimer > 60)
        		{
        			projectile.velocity.X = 0f;
        			projectile.velocity.Y = 10f;
        		}
        		else if (speedTimer <= 60)
        		{
        			projectile.velocity.X = 0f;
        			projectile.velocity.Y = -10f;
        		}
        		if (speedTimer <= 0)
        		{
        			speedTimer = 120;
        		}
        	}
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.25f) / 255f, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0.75f) / 255f);
            if (Main.rand.Next(4) == 0)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, 0f, 0f);
            }
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, 0f, 0f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 500);
        }
    }
}