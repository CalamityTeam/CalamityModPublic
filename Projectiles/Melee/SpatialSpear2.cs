using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class SpatialSpear2 : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spear");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = 27;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.penetrate = 2;
            projectile.timeLeft = 120;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.05f) / 255f, ((255 - projectile.alpha) * 0.05f) / 255f, ((255 - projectile.alpha) * 1f) / 255f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("BrimstoneFlames"), 60);
        	target.AddBuff(mod.BuffType("GlacialState"), 60);
        	target.AddBuff(mod.BuffType("Plague"), 60);
        	target.AddBuff(mod.BuffType("HolyLight"), 60);
			int numProj = 2;
            float rotation = MathHelper.ToRadians(20);
            if (projectile.owner == Main.myPlayer)
            {
	            for (int i = 0; i < numProj + 1; i++)
	            {
	                Vector2 perturbedSpeed = new Vector2(projectile.velocity.X, projectile.velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
	                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, mod.ProjectileType("SpatialSpear3"), (int)((double)projectile.damage * 0.2), projectile.knockBack * 0.2f, projectile.owner, 0f, 0f);
	            }
            }
        }
    }
}