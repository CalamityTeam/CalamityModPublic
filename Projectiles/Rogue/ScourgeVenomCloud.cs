using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class ScourgeVenomCloud : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Venom Cloud");
			Main.projFrames[projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			projectile.width = 45;
			projectile.height = 45;
			projectile.friendly = true;
			projectile.penetrate = -1;
            projectile.alpha = 255;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.timeLeft = 3600;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 30;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
		}

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 9)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
            projectile.velocity *= 0.98f;
			projectile.ai[0] += 1f;
			if (projectile.ai[0] >= 120f)
			{
				if (projectile.alpha < 255)
				{
					projectile.alpha += 5;
					if (projectile.alpha > 255)
					{
						projectile.alpha = 255;
					}
				}
				else if (projectile.owner == Main.myPlayer)
				{
					projectile.Kill();
				}
			}
			else if (projectile.alpha > 80)
			{
				projectile.alpha -= 30;
				if (projectile.alpha < 80)
				{
					projectile.alpha = 80;
				}
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 240);
        }
    }
}
