using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class EmpyreanKnives : ModProjectile
    {
        private int bounce = 3;

    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Knife");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[projectile.type] = 1;
		}

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.melee = true;
            projectile.extraUpdates = 1;
		}

        public override void AI()
        {
            if (projectile.ai[1] == 1f)
            {
                projectile.melee = false;
				projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
			}
        	projectile.ai[0] += 1f;
			if (projectile.ai[0] >= 75f)
			{
				projectile.alpha += 10;
				projectile.damage = (int)((double)projectile.damage * 0.95);
				projectile.knockBack = (float)((int)((double)projectile.knockBack * 0.95));
                if (projectile.alpha >= 255)
                {
                    projectile.active = false;
                }
            }
			if (projectile.ai[0] < 75f)
			{
				projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
			}
            else
            {
                projectile.rotation += 0.5f;
            }
            float num472 = projectile.Center.X;
			float num473 = projectile.Center.Y;
			float num474 = 250f;
			bool flag17 = false;
			for (int num475 = 0; num475 < 200; num475++)
			{
				if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1))
				{
					float num476 = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
					float num477 = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
					float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
					if (num478 < num474)
					{
						num474 = num478;
						num472 = num476;
						num473 = num477;
						flag17 = true;
					}
				}
			}
			if (flag17)
			{
				float num483 = 15f;
				Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num484 = num472 - vector35.X;
				float num485 = num473 - vector35.Y;
				float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
				num486 = num483 / num486;
				num484 *= num486;
				num485 *= num486;
				projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
				projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
			}
            if (Main.rand.Next(6) == 0)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 58, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounce--;
            if (bounce <= 0)
            {
                projectile.Kill();
            }
            else
            {
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

        public override void Kill(int timeLeft)
        {
            for (int num303 = 0; num303 < 3; num303++)
			{
				int num304 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 58, 0f, 0f, 100, default(Color), 0.8f);
				Main.dust[num304].noGravity = true;
				Main.dust[num304].velocity *= 1.2f;
				Main.dust[num304].velocity -= projectile.oldVelocity * 0.3f;
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	if (target.type == NPCID.TargetDummy)
			{
				return;
			}
        	float num = (float)damage * 0.005f;
			if ((int)num == 0)
			{
				return;
			}
			if (Main.player[Main.myPlayer].lifeSteal <= 0f)
			{
				return;
			}
			Main.player[Main.myPlayer].lifeSteal -= num * 1.5f;
			int num2 = projectile.owner;
			Projectile.NewProjectile(target.position.X, target.position.Y, 0f, 0f, 305, 0, 0f, projectile.owner, (float)num2, num);
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
			return false;
		}
	}
}
