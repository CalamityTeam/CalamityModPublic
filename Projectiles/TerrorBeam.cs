using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class TerrorBeam : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Beam");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 7;
            projectile.alpha = 255;
            projectile.timeLeft = 600;
            projectile.light = 1f;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 3;
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
            	if (projectile.owner == Main.myPlayer)
				{
        			Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("TerrorBoom"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            	}
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

        public override void AI()
        {
        	if (projectile.localAI[1] == 0f)
			{
				Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 60);
				projectile.localAI[1] += 1f;
			}
			projectile.alpha -= 40;
			if (projectile.alpha < 0) 
			{
				projectile.alpha = 0;
			}
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
			return;
        }
        
        public override Color? GetAlpha(Color lightColor)
        {
        	return new Color(255, 0, 0, projectile.alpha);
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 60);
			projectile.position = projectile.Center;
			projectile.width = (projectile.height = 400);
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			for (int num193 = 0; num193 < 6; num193++)
			{
				Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 100, default(Color), 1.5f);
			}
			for (int num194 = 0; num194 < 60; num194++)
			{
				int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 0, default(Color), 2.5f);
				Main.dust[num195].noGravity = true;
				Main.dust[num195].velocity *= 3f;
				num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 100, default(Color), 1.5f);
				Main.dust[num195].velocity *= 2f;
				Main.dust[num195].noGravity = true;
			}
			projectile.Damage();
        }
    }
}