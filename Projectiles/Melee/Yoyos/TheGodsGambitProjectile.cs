using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class TheGodsGambitProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("God's Gambit");
		}
    	
        public override void SetDefaults()
        {
        	projectile.CloneDefaults(ProjectileID.Kraken);
            projectile.width = 16;
            projectile.scale = 1.15f;
            projectile.height = 16;
            projectile.penetrate = 6;
            projectile.melee = true;
            aiType = 554;
        }
        
        public override void AI()
        {
        	if (Main.rand.Next(8) == 0)
        	{
        		if (projectile.owner == Main.myPlayer)
        		{
            		Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, 406, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
        		}
        	}
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
	    {
			target.AddBuff(BuffID.Slimed, 200);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = Main.projectileTexture[projectile.type];
			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
	}
}