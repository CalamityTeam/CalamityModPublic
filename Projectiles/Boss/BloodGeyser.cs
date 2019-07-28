using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Projectiles.Boss
{
    public class BloodGeyser : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blood Geyser");
		}

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.hostile = true;
            if (CalamityWorld.death)
            {
                projectile.tileCollide = false;
            }
            projectile.timeLeft = 300;
            projectile.penetrate = 1;
            projectile.aiStyle = 1;
            aiType = 1;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 5, 0f, 0f, 100, default(Color), 0.5f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
        	target.AddBuff(mod.BuffType("BurningBlood"), 120);
        }
    }
}
