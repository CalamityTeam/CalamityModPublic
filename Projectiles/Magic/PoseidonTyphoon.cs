using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class PoseidonTyphoon : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poseidon Typhoon");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 10;
            projectile.timeLeft = 300;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            projectile.rotation += projectile.velocity.X * 0.05f;

			float centerX = projectile.Center.X;
			float centerY = projectile.Center.Y;
			float distance = 600f;
			bool homeIn = false;
			for (int num475 = 0; num475 < 200; num475++)
			{
				if (Main.npc[num475].CanBeChasedBy(projectile, false))
				{
					float extraDistance = (float)(Main.npc[num475].width / 2) + (float)(Main.npc[num475].height / 2);

					bool useCollisionDetection = extraDistance < distance;
					bool canHit = true;
					if (useCollisionDetection)
						canHit = Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1);

					if (Vector2.Distance(Main.npc[num475].Center, projectile.Center) < (distance + extraDistance) && canHit)
					{
						distance = Vector2.Distance(Main.npc[num475].Center, projectile.Center);
						centerX = Main.npc[num475].Center.X;
						centerY = Main.npc[num475].Center.Y;
						homeIn = true;
					}
				}
			}

			if (homeIn)
            {
                float num483 = 6f;
                Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                float num484 = centerX - vector35.X;
                float num485 = centerY - vector35.Y;
                float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
                num486 = num483 / num486;
                num484 *= num486;
                num485 *= num486;
                projectile.velocity.X = (projectile.velocity.X * 30f + num484) / 31f;
                projectile.velocity.Y = (projectile.velocity.Y * 30f + num485) / 31f;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 10);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
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
    }
}
