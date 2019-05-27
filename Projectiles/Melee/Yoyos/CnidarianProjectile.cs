using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class CnidarianProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cnidarian");
		}
    	
        public override void SetDefaults()
        {
        	projectile.CloneDefaults(ProjectileID.CorruptYoyo);
            projectile.width = 16;
            projectile.scale = 1.15f;
            projectile.height = 16;
            projectile.penetrate = 6;
            projectile.melee = true;
            aiType = 542;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = Main.projectileTexture[projectile.type];
			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
	}
}