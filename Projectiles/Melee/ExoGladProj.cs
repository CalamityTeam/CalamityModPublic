using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
	public class ExoGladProj : ModProjectile
    {
		private float counter = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Vector2 value7 = new Vector2(5f, 10f);
            counter += 1f;
            if (counter == 48f)
            {
                counter = 0f;
            }
            else
            {
                for (int num41 = 0; num41 < 2; num41++)
                {
                    int dustType = num41 == 0 ? 107 : 234;
					if (Main.rand.NextBool(4))
					{
						dustType = 269;
					}
                    Vector2 value8 = Vector2.UnitX * -12f;
                    value8 = -Vector2.UnitY.RotatedBy((double)(counter * 0.1308997f + (float)num41 * 3.14159274f), default) * value7;
                    int num42 = Dust.NewDust(projectile.Center, 0, 0, dustType, 0f, 0f, 160, default, 1.5f);
                    Main.dust[num42].noGravity = true;
                    Main.dust[num42].position = projectile.Center + value8;
                    Main.dust[num42].velocity = projectile.velocity;
                    int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 0.8f);
                    Main.dust[num458].noGravity = true;
                    Main.dust[num458].velocity *= 0f;
                }
            }

            float num472 = projectile.Center.X;
            float num473 = projectile.Center.Y;
            float num474 = 1000f;
            bool flag17 = false;
            for (int num475 = 0; num475 < Main.npc.Length; num475++)
            {
                if (Main.npc[num475].CanBeChasedBy(projectile, false))
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
                float num483 = 36f;
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

		public override void Kill(int timeLeft)
        {
			int dustType = Utils.SelectRandom(Main.rand, new int[]
			{
				107,
				234,
				269
			});
            for (int k = 0; k < 4; k++)
            {
                int num = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, dustType, (float)(projectile.direction * 2), 0f, 150, default, 1f);
                Main.dust[num].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int type = ModContent.ProjectileType<ExoGladiusBeam>();
            int numSwords = Main.rand.Next(1,4);
            float swordKB = projectile.knockBack;
            for (int i = 0; i < numSwords; ++i)
            {
                float startOffsetX = Main.rand.NextFloat(1000f, 1400f) * (Main.rand.NextBool() ? -1f : 1f);
                float startOffsetY = Main.rand.NextFloat(80f, 900f) * (Main.rand.NextBool() ? -1f : 1f);
                Vector2 startPos = new Vector2(target.Center.X + startOffsetX, target.Center.Y + startOffsetY);
                float dx = target.Center.X - startPos.X;
                float dy = target.Center.Y - startPos.Y;

                dx += Main.rand.NextFloat(-5f, 5f);
                dy += Main.rand.NextFloat(-5f, 5f);
                float speed = Main.rand.NextFloat(24f, 30f);
                float dist = (float)Math.Sqrt((double)(dx * dx + dy * dy));
                dist = speed / dist;
                dx *= dist;
                dy *= dist;
                Vector2 swordVel = new Vector2(dx, dy);
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                if (projectile.owner == Main.myPlayer)
                {
                    int idx = Projectile.NewProjectile(startPos, swordVel, type, (int)((double)projectile.damage * 0.25), swordKB, projectile.owner, 0f, 0f);
                    Main.projectile[idx].rotation = angle;
                }
            }

			int spearAmt = Main.rand.Next(1,4);
			for (int n = 0; n < spearAmt; n++)
			{
				float x = projectile.position.X + (float)Main.rand.Next(-400, 400);
				float y = projectile.position.Y + (float)Main.rand.Next(800, 1000);
				Vector2 vector = new Vector2(x, y);
				float num13 = projectile.position.X + (float)(projectile.width / 2) - vector.X;
				float num14 = projectile.position.Y + (float)(projectile.height / 2) - vector.Y;
				num13 += (float)Main.rand.Next(-100, 101);
				int num15 = 29;
				float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
				num16 = (float)num15 / num16;
				num13 *= num16;
				num14 *= num16;
				int num17 = Projectile.NewProjectile(x, y, num13, num14, ModContent.ProjectileType<ExoGladSpears>(), (int)((double)projectile.damage * 0.5), swordKB, projectile.owner, 0f, 0f);
			}
			target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            int type = ModContent.ProjectileType<ExoGladiusBeam>();
            int numSwords = Main.rand.Next(1,4);
            float swordKB = projectile.knockBack;
            for (int i = 0; i < numSwords; ++i)
            {
                float startOffsetX = Main.rand.NextFloat(1000f, 1400f) * (Main.rand.NextBool() ? -1f : 1f);
                float startOffsetY = Main.rand.NextFloat(80f, 900f) * (Main.rand.NextBool() ? -1f : 1f);
                Vector2 startPos = new Vector2(target.Center.X + startOffsetX, target.Center.Y + startOffsetY);
                float dx = target.Center.X - startPos.X;
                float dy = target.Center.Y - startPos.Y;

                dx += Main.rand.NextFloat(-5f, 5f);
                dy += Main.rand.NextFloat(-5f, 5f);
                float speed = Main.rand.NextFloat(24f, 30f);
                float dist = (float)Math.Sqrt((double)(dx * dx + dy * dy));
                dist = speed / dist;
                dx *= dist;
                dy *= dist;
                Vector2 swordVel = new Vector2(dx, dy);
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                if (projectile.owner == Main.myPlayer)
                {
                    int idx = Projectile.NewProjectile(startPos, swordVel, type, (int)((double)projectile.damage * 0.25), swordKB, projectile.owner, 0f, 0f);
                    Main.projectile[idx].rotation = angle;
                }
            }

			int spearAmt = Main.rand.Next(1,4);
			for (int n = 0; n < spearAmt; n++)
			{
				float x = projectile.position.X + (float)Main.rand.Next(-400, 400);
				float y = projectile.position.Y + (float)Main.rand.Next(800, 1000);
				Vector2 vector = new Vector2(x, y);
				float num13 = projectile.position.X + (float)(projectile.width / 2) - vector.X;
				float num14 = projectile.position.Y + (float)(projectile.height / 2) - vector.Y;
				num13 += (float)Main.rand.Next(-100, 101);
				int num15 = 29;
				float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
				num16 = (float)num15 / num16;
				num13 *= num16;
				num14 *= num16;
				int num17 = Projectile.NewProjectile(x, y, num13, num14, ModContent.ProjectileType<ExoGladSpears>(), (int)((double)projectile.damage * 0.5), swordKB, projectile.owner, 0f, 0f);
			}
			target.ExoDebuffs();
        }
    }
}
