using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class AuraRain : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aura Rain");
		}

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.aiStyle = 45;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = true;
            projectile.timeLeft = 300;
            projectile.alpha = 255;
            projectile.scale = 1.1f;
            projectile.magic = true;
            projectile.extraUpdates = 1;
            aiType = 239;
        }

		public override void AI()
		{
			Dust dust4 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 14, projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 1f)];
			dust4.velocity = Vector2.Zero;
			dust4.position -= projectile.velocity / 5f;
			dust4.noGravity = true;
			dust4.scale = 0.8f;
			dust4.noLight = true;
		}
    }
}
