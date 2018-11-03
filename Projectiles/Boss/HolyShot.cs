using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyShot : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spear");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.timeLeft = 900;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            projectile.velocity.X *= 1.03f;
            projectile.velocity.Y *= 1.03f;
            Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 1.5f) / 255f, ((255 - projectile.alpha) * 0.75f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
            projectile.rotation = projectile.velocity.ToRotation() + 1.57079637f;
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                Main.PlayTrackedSound(SoundID.DD2_BetsyFireballShot, projectile.Center);
            }
        }
        
        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft > 883)
            {
                projectile.localAI[1] += 5f;
                byte b2 = (byte)(((int)projectile.localAI[1]) * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }
    }
}