using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class VividBeam : ModProjectile
    {
		private bool counter = true;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Vector2 value7 = new Vector2(5f, 10f);
            projectile.ai[0] += 1f;
            if (projectile.ai[0] == 48f)
            {
                projectile.ai[0] = 0f;
            }
            else
            {
                for (int num41 = 0; num41 < 2; num41++)
                {
                    Vector2 value8 = Vector2.UnitX * -12f;
                    value8 = -Vector2.UnitY.RotatedBy((double)(projectile.ai[0] * 0.1308997f + (float)num41 * 3.14159274f), default) * value7 - projectile.rotation.ToRotationVector2() * 10f;
                    int num42 = Dust.NewDust(projectile.Center, 0, 0, 66, 0f, 0f, 160, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                    Main.dust[num42].scale = 0.75f;
                    Main.dust[num42].noGravity = true;
                    Main.dust[num42].position = projectile.Center + value8;
                    Main.dust[num42].velocity = projectile.velocity;
                }
            }
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                for (int num447 = 0; num447 < 2; num447++)
                {
                    Vector2 vector33 = projectile.position;
                    vector33 -= projectile.velocity * ((float)num447 * 0.25f);
                    int num448 = Dust.NewDust(vector33, 1, 1, 66, 0f, 0f, 0, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                    Main.dust[num448].noGravity = true;
                    Main.dust[num448].position = vector33;
                    Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.013f;
					Main.dust[num448].velocity *= 0.1f;
				}
            }

            if (counter)
            {
                counter = false;
                float num98 = 16f;
                int num99 = 0;
                while ((float)num99 < num98)
                {
                    Vector2 vector11 = Vector2.UnitX * 0f;
                    vector11 += -Vector2.UnitY.RotatedBy((double)((float)num99 * (6.28318548f / num98)), default) * new Vector2(1f, 4f);
                    vector11 = vector11.RotatedBy((double)projectile.velocity.ToRotation(), default);
                    int num100 = Dust.NewDust(projectile.Center, 0, 0, 66, 0f, 0f, 0, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                    Main.dust[num100].scale = 1.5f;
                    Main.dust[num100].noGravity = true;
                    Main.dust[num100].position = projectile.Center + vector11;
                    Main.dust[num100].velocity = projectile.velocity * 0f + vector11.SafeNormalize(Vector2.UnitY) * 1f;
                    num99++;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.owner == Main.myPlayer)
            {
				SummonLasers();
			}
			return true;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
            {
				SummonLasers();
			}
			target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
            {
				SummonLasers();
			}
			target.ExoDebuffs();
        }

		private void SummonLasers()
		{
			if (projectile.ai[1] == 0f)
			{
				float xStart = projectile.position.X + (float)Main.rand.Next(-400, 400);
				float yStart = projectile.position.Y - (float)Main.rand.Next(500, 800);
				Vector2 startPos = new Vector2(xStart, yStart);
				Vector2 velocity = projectile.Center - startPos;
				velocity.X += (float)Main.rand.Next(-100, 101);
				float speed = 6f;
				float targetDist = velocity.Length();
				targetDist = speed / targetDist;
				velocity.X *= targetDist;
				velocity.Y *= targetDist;
				int num19 = Projectile.NewProjectile(startPos, velocity, ModContent.ProjectileType<VividClarityBeam>(), (int)(projectile.damage * 0.7f), projectile.knockBack, projectile.owner, 0f, 0f);
				Main.projectile[num19].ai[1] = projectile.position.Y;
			}
			if (projectile.ai[1] == 1f)
			{
                int boom = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<SupernovaBoom>(), (int)(projectile.damage * 2f), projectile.knockBack, projectile.owner, 0f, 0f);
				Main.projectile[boom].Calamity().forceMagic = true;
			}
			if (projectile.ai[1] == 2f)
			{
				float spread = 30f * 0.01f * MathHelper.PiOver2;
				double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
				double deltaAngle = spread / 8f;
				double offsetAngle;
				if (projectile.owner == Main.myPlayer)
				{
					for (int i = 0; i < 4; i++)
					{
						offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<VividLaser2>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<VividLaser2>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
					}
				}
			}
		}
    }
}
