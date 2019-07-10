using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class AccretionDisk2 : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Accretion Disk");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 56;
            projectile.height = 56;
            projectile.alpha = 120;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 60;
            aiType = 52;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
		}
        
        public override void AI()
        {
            if (Main.rand.Next(10) == 0)
            {
                int num250 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 66, (float)(projectile.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 0.5f);
                Main.dust[num250].noGravity = true;
                Main.dust[num250].velocity *= 0f;
            }
			Lighting.AddLight(projectile.Center, 0.15f, 1f, 0.25f);
		}
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.immune[projectile.owner] = 5;
        	target.AddBuff(mod.BuffType("BrimstoneFlames"), 120);
        	target.AddBuff(mod.BuffType("GlacialState"), 120);
        	target.AddBuff(mod.BuffType("Plague"), 120);
        	target.AddBuff(mod.BuffType("HolyLight"), 120);
        }
    }
}
