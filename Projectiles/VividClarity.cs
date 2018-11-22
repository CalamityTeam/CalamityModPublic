using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class VividClarity : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ray");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 10;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 300;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 2;
        }

        public override void AI()
        {
        	projectile.localAI[1] += 1f;
        	if (projectile.localAI[1] >= 29f && projectile.owner == Main.myPlayer)
        	{
        		projectile.localAI[1] = 0f;
            	Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.35f, projectile.velocity.Y * 0.35f, mod.ProjectileType("VividOrb"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
        	}
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 4f)
			{
				for (int num447 = 0; num447 < 2; num447++)
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
				return;
			}
        }
        
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
        	projectile.damage = (int)((double)projectile.damage * 1.1f);
        	projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                projectile.ai[0] += 0.1f;
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	if (target.type == NPCID.TargetDummy)
			{
				return;
			}
        	projectile.damage = (int)((double)projectile.damage * 1.05f);
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