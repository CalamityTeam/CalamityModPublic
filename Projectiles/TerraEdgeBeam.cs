using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class TerraEdgeBeam : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Beam");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = 27;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 600;
            aiType = 132;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 1.3f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(0, 200, 0, 0);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	if (target.type == NPCID.TargetDummy)
			{
				return;
			}
        	int healAmount = (Main.rand.Next(2) + 1);
            target.AddBuff(BuffID.Ichor, 60);
            Player player = Main.player[projectile.owner];
			player.statLife += healAmount;
    		player.HealEffect(healAmount);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            int num3;
            for (int num795 = 4; num795 < 31; num795 = num3 + 1)
            {
                float num796 = projectile.oldVelocity.X * (30f / (float)num795);
                float num797 = projectile.oldVelocity.Y * (30f / (float)num795);
                int num798 = Dust.NewDust(new Vector2(projectile.oldPosition.X - num796, projectile.oldPosition.Y - num797), 8, 8, 107, projectile.oldVelocity.X, projectile.oldVelocity.Y, 100, default(Color), 1.8f);
                Main.dust[num798].noGravity = true;
                Dust dust = Main.dust[num798];
                dust.velocity *= 0.5f;
                num798 = Dust.NewDust(new Vector2(projectile.oldPosition.X - num796, projectile.oldPosition.Y - num797), 8, 8, 107, projectile.oldVelocity.X, projectile.oldVelocity.Y, 100, default(Color), 1.4f);
                dust = Main.dust[num798];
                dust.velocity *= 0.05f;
                num3 = num795;
            }
        }
    }
}