using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class OrichalcumSpikedGemstoneProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gemstone");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.thrown = true;
            projectile.aiStyle = 2;
            projectile.penetrate = 6;
            projectile.timeLeft = 600;
            aiType = 48;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
        	Vector2 velocity = projectile.velocity;
            if (projectile.velocity.Y != velocity.Y && (velocity.Y < -3f || velocity.Y > 3f))
			{
				Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
				Main.PlaySound(0, (int)projectile.Center.X, (int)projectile.Center.Y, 1);
			}
        	if (projectile.velocity.X != velocity.X)
			{
				projectile.velocity.X = velocity.X * -0.5f;
			}
			if (projectile.velocity.Y != velocity.Y && velocity.Y > 1f)
			{
				projectile.velocity.Y = velocity.Y * -0.5f;
			}
            return false;
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        
        public override void Kill(int timeLeft)
        {
        	if (Main.rand.Next(2) == 0)
        	{
        		Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, mod.ItemType("OrichalcumSpikedGemstone"));
        	}
        }
    }
}