using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ContagionArrow : ModProjectile
    {
    	public int addBallTimer = 10;
    	
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Contagion Arrow");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.ranged = true;
        }

        public override void AI()
        {
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
        	addBallTimer--;
        	if (addBallTimer <= 0)
        	{
        		if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[mod.ProjectileType("ContagionBall")] < 100)
        		{
        			Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("ContagionBall"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
        		}
        		addBallTimer = 10;
        	}
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.15f) / 255f, ((255 - projectile.alpha) * 0.25f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
			if (projectile.ai[0] <= 60f)
			{
				projectile.ai[0] += 1f;
				return;
			}
			projectile.velocity.Y = projectile.velocity.Y + 0.075f;
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("Plague"), 600);
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
        	Texture2D texture2D13 = Main.projectileTexture[projectile.type];
			int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
			int y6 = num214 * projectile.frame;
			Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
			return false;
        }
    }
}