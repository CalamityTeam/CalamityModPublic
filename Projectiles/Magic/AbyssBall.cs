using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class AbyssBall : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Abyss Ball");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.alpha = 60;
            projectile.penetrate = 2;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.magic = true;
        }

        public override void AI()
        {
        	projectile.velocity.X *= 0.985f;
        	projectile.velocity.Y *= 0.985f;
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
        	projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
			projectile.width = 150;
			projectile.height = 150;
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 30; num621++)
			{
				int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default(Color), 1.2f);
				Main.dust[num622].velocity *= 3f;
				if (Main.rand.Next(2) == 0)
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}
			for (int num623 = 0; num623 < 60; num623++)
			{
				int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default(Color), 1.7f);
				Main.dust[num624].noGravity = true;
				Main.dust[num624].velocity *= 5f;
				num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default(Color), 1f);
				Main.dust[num624].velocity *= 2f;
			}
			projectile.Damage();
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("BrimstoneFlames"), 120);
        }
    }
}