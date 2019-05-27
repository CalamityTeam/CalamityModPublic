using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class FlameScytheProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scythe");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 600;
            projectile.alpha = 55;
            aiType = 52;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
		}
        
        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.25f) / 255f, ((255 - projectile.alpha) * 0.15f) / 255f, ((255 - projectile.alpha) * 0.01f) / 255f);
            if (Main.rand.Next(5) == 0)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 127, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
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
            return false;
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.immune[projectile.owner] = 6;
			target.AddBuff(BuffID.OnFire, 300);
			target.AddBuff(BuffID.Bleeding, 600);
			if (projectile.owner == Main.myPlayer)
			{
				int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, 612, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
				Main.projectile[proj].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceRogue = true;
			}
        }
    }
}