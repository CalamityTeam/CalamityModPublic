using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SandslasherProj : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sandslasher");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.penetrate = 3;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 30;
            projectile.Calamity().rogue = true;
			projectile.timeLeft = 600;
		}
        
        public override void AI()
        {
			projectile.ai[0] += 1f;
			if (projectile.ai[0] == 3f)
				projectile.tileCollide = true;
			if(projectile.velocity.X < 0f)
			{
				projectile.velocity.X -= 0.05f;
				if ((projectile.ai[0] %= 30f) == 0f)
					projectile.damage -= (int)projectile.velocity.X;
			}
			else if(projectile.velocity.X > 0f)
			{
				projectile.velocity.X += 0.05f;
				if ((projectile.ai[0] %= 30f) == 0f)
					projectile.damage += (int)projectile.velocity.X;
			}
			projectile.rotation += 0.75f;
        }
		
		public override void Kill(int timeLeft)
        {
			for (int i = 0; i < 15; i++)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 85, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, default(Color), 1f);
			}
        }
    }
}