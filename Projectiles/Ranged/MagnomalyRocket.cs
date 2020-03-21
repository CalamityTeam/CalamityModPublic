using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class MagnomalyRocket : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nuke");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.ranged = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            //Animation
            projectile.frameCounter++;
            if (projectile.frameCounter > 7)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 4)
            {
                projectile.frame = 0;
            }

            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * projectile.direction;

			int num297 = Main.rand.NextBool(2) ? 107 : 234;
			if (Main.rand.NextBool(4))
			{
				num297 = 269;
			}
        	if (projectile.owner == Main.myPlayer && projectile.timeLeft == 300)
        	{
            	Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<MagnomalyAura>(), (int)((double)projectile.damage * 0.5f), (int)((double)projectile.knockBack * 0.5f), projectile.owner, projectile.identity, 0f);
			}
			float num247 = projectile.velocity.X * 0.5f;
			float num248 = projectile.velocity.Y * 0.5f;
			if (Main.rand.NextBool(2))
			{
				int num249 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, num297, 0f, 0f, 100, default, 0.5f);
				Main.dust[num249].scale *= (float)Main.rand.Next(10) * 0.1f;
				Main.dust[num249].velocity *= 0.2f;
				Main.dust[num249].noGravity = true;
			}
			else
			{
				int dust2 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, num297, 0f, 0f, 100, default, 0.25f);
				Main.dust[dust2].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
				Main.dust[dust2].velocity *= 0.05f;
				Main.dust[dust2].noGravity = true;
			}
			CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 600f, 16f, 20f);
        }

        public override void Kill(int timeLeft)
        {
			if (projectile.owner == Main.myPlayer)
			{
				projectile.position = projectile.Center;
				projectile.width = projectile.height = 192;
				projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
				projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
				Main.PlaySound(2, (int)projectile.Center.X, (int)projectile.Center.Y, 14);
				//DO NOT REMOVE THIS PROJECTILE
				Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<MagnomalyExplosion>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);

				int num298 = Main.rand.NextBool(2) ? 107 : 234;
				if (Main.rand.NextBool(4))
				{
					num298 = 269;
				}
				for (int num621 = 0; num621 < 30; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num298, 0f, 0f, 100, default, 1f);
					Main.dust[num622].velocity *= 3f;
					Main.dust[num622].noGravity = true;
					if (Main.rand.NextBool(2))
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 40; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num298, 0f, 0f, 100, default, 0.5f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num298, 0f, 0f, 100, default, 0.75f);
					Main.dust[num624].velocity *= 2f;
				}
				for (int num625 = 0; num625 < 10; num625++)
				{
					float scaleFactor10 = 0.33f;
					if (num625 == 1)
					{
						scaleFactor10 = 0.66f;
					}
					if (num625 == 2)
					{
						scaleFactor10 = 1f;
					}
					int num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
					Gore gore = Main.gore[num626];
					gore.velocity *= scaleFactor10;
					gore.velocity.X += 1f;
					gore.velocity.Y += 1f;
					num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
					gore.velocity *= scaleFactor10;
					gore.velocity.X -= 1f;
					gore.velocity.Y += 1f;
					num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
					gore.velocity *= scaleFactor10;
					gore.velocity.X += 1f;
					gore.velocity.Y -= 1f;
					num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(61, 64), 1f);
					gore.velocity *= scaleFactor10;
					gore.velocity.X -= 1f;
					gore.velocity.Y -= 1f;
				}
			}
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (projectile.owner == Main.myPlayer)
			{
				float random = Main.rand.Next(30, 90);
				float spread = random * 0.0174f;
				double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
				double deltaAngle = spread / 8f;
				for (int i = 0; i < 4; i++)
				{
					double offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
					int proj1 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<MagnomalyBeam>(), projectile.damage / 4, projectile.knockBack / 4, projectile.owner, 0f, 1f);
					int proj2 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<MagnomalyBeam>(), projectile.damage / 4, projectile.knockBack / 4, projectile.owner, 0f, 1f);
				}
			}
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 30);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
            target.AddBuff(BuffID.CursedInferno, 120);
            target.AddBuff(BuffID.Frostburn, 120);
            target.AddBuff(BuffID.OnFire, 120);
            target.AddBuff(BuffID.Ichor, 120);
        }
    }
}
