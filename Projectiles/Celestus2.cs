using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class Celestus2 : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Celestus");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.extraUpdates = 4;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 40;
            projectile.timeLeft = 85;
        }
        
        public override void AI()
        {
            projectile.rotation += 2f;
            projectile.velocity.X *= 1.02f;
            projectile.velocity.Y *= 1.02f;
        	Lighting.AddLight(projectile.Center, ((Main.DiscoR - projectile.alpha) * 0.5f) / 255f, ((Main.DiscoG - projectile.alpha) * 0.5f) / 255f, ((Main.DiscoB - projectile.alpha) * 0.5f) / 255f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.Next(30) == 0)
            {
                target.AddBuff(mod.BuffType("ExoFreeze"), 240);
            }
            target.AddBuff(mod.BuffType("BrimstoneFlames"), 100);
            target.AddBuff(mod.BuffType("GlacialState"), 100);
            target.AddBuff(mod.BuffType("Plague"), 100);
            target.AddBuff(mod.BuffType("HolyLight"), 100);
            target.AddBuff(BuffID.CursedInferno, 100);
            target.AddBuff(BuffID.Frostburn, 100);
            target.AddBuff(BuffID.OnFire, 100);
            target.AddBuff(BuffID.Ichor, 100);
        }
    }
}