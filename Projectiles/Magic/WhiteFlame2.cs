using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class WhiteFlame2 : ModProjectile
    {
        float homeTimer = 100;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 3;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 1;
            projectile.timeLeft = 120;
            projectile.magic = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0.5f / 255f);
            for (int num457 = 0; num457 < 5; num457++)
            {
                int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 91, 0f, 0f, 100, default, 0.5f);
                Main.dust[num458].noGravity = true;
                Main.dust[num458].velocity *= 0.5f;
                Main.dust[num458].velocity += projectile.velocity * 0.1f;
            }

            homeTimer += 5f;
			float num472 = projectile.Center.X;
			float num473 = projectile.Center.Y;
			float distance = homeTimer;
			bool flag17 = false;
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
						num472 = Main.npc[num475].Center.X;
						num473 = Main.npc[num475].Center.Y;
						flag17 = true;
					}
				}
			}

			if (flag17)
            {
                float num483 = 25f;
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
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 360);
        }
    }
}
