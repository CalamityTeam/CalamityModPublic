using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SirenSong : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Song");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 1800;
        }

        public override void AI()
        {
        	projectile.velocity.X *= 0.985f;
        	projectile.velocity.Y *= 0.985f;
        	if (projectile.localAI[0] == 0f)
			{
				projectile.scale += 0.01f;
				if (projectile.scale >= 1.1f)
				{
					projectile.localAI[0] = 1f;
				}
			}
			else if (projectile.localAI[0] == 1f)
			{
				projectile.scale -= 0.01f;
				if (projectile.scale <= 0.9f)
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

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
        	target.AddBuff(BuffID.Confused, 60);
        }
    }
}