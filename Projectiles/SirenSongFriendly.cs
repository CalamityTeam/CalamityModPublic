using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class SirenSongFriendly : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Song");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
        	projectile.velocity.X *= 0.985f;
        	projectile.velocity.Y *= 0.985f;
        	if (projectile.localAI[0] == 0f)
			{
				projectile.scale += 0.02f;
				if (projectile.scale >= 1.25f)
				{
					projectile.localAI[0] = 1f;
				}
			}
			else if (projectile.localAI[0] == 1f)
			{
				projectile.scale -= 0.02f;
				if (projectile.scale <= 0.75f)
				{
					projectile.localAI[0] = 0f;
				}
			}
        	if (projectile.ai[1] == 0f)
        	{
        		projectile.ai[1] = 1f;
        		Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 26);
        	}
        }
    }
}