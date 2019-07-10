using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class VividBolt : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bolt");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.extraUpdates = 100;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.timeLeft = 30;
            projectile.magic = true;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 4f)
			{
				for (int num447 = 0; num447 < 1; num447++)
				{
					Vector2 vector33 = projectile.position;
					vector33 -= projectile.velocity * ((float)num447 * 0.25f);
					projectile.alpha = 255;
					int num448 = Dust.NewDust(vector33, 1, 1, 66, 0f, 0f, 0, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.25f);
					Main.dust[num448].noGravity = true;
					Main.dust[num448].position = vector33;
					Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.013f;
					Main.dust[num448].velocity *= 0.2f;
				}
			}
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(mod.BuffType("ExoFreeze"), 30);
			target.AddBuff(mod.BuffType("BrimstoneFlames"), 120);
			target.AddBuff(mod.BuffType("GlacialState"), 120);
			target.AddBuff(mod.BuffType("Plague"), 120);
			target.AddBuff(mod.BuffType("HolyLight"), 120);
			target.AddBuff(BuffID.CursedInferno, 120);
			target.AddBuff(BuffID.Frostburn, 120);
			target.AddBuff(BuffID.OnFire, 120);
			target.AddBuff(BuffID.Ichor, 120);
		}
    }
}
