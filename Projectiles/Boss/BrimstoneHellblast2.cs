using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneHellblast2 : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimstone Hellblast");
            Main.projFrames[projectile.type] = 4;
        }
    	
        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
            projectile.penetrate = 1;
            projectile.timeLeft = 1500;
            projectile.alpha = 0;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 12)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
            projectile.alpha += 1;
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.9f) / 255f, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
			if (projectile.velocity.X < 0f)
			{
				projectile.spriteDirection = -1;
				projectile.rotation = (float)Math.Atan2((double)(-(double)projectile.velocity.Y), (double)(-(double)projectile.velocity.X));
			}
			else
			{
				projectile.spriteDirection = 1;
				projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
			}
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 50, 50, projectile.alpha);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
        	target.AddBuff(mod.BuffType("AbyssalFlames"), 180);
            if (NPC.AnyNPCs(mod.NPCType("SupremeCalamitas")))
                target.AddBuff(mod.BuffType("VulnerabilityHex"), 120, true);
        }
    }
}