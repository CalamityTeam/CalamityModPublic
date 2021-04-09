using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneHellfireball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Hellfireball");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
			projectile.Calamity().canBreakPlayerDefense = true;
			projectile.width = 34;
            projectile.height = 34;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            //Animation
            projectile.frameCounter++;
            if (projectile.frameCounter > 9)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 6)
            {
                projectile.frame = 0;
            }

            //Fade in
            if (projectile.alpha > 5)
                projectile.alpha -= 15;
            if (projectile.alpha < 5)
                projectile.alpha = 5;

			if (projectile.ai[0] != 0f && projectile.ai[1] != 0f)
			{
				bool flag15 = false;
				bool flag16 = false;
				if (projectile.velocity.X < 0f && projectile.position.X < projectile.ai[0])
				{
					flag15 = true;
				}
				if (projectile.velocity.X > 0f && projectile.position.X > projectile.ai[0])
				{
					flag15 = true;
				}
				if (projectile.velocity.Y < 0f && projectile.position.Y < projectile.ai[1])
				{
					flag16 = true;
				}
				if (projectile.velocity.Y > 0f && projectile.position.Y > projectile.ai[1])
				{
					flag16 = true;
				}
				if (flag15 & flag16)
				{
					projectile.Kill();
				}
			}

			//Rotation
			projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) - MathHelper.ToRadians(90) * projectile.direction;

            projectile.velocity.Y *= 1.01f;
            projectile.velocity.X *= 1.01f;
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.5f / 255f, (255 - projectile.alpha) * 0.05f / 255f, (255 - projectile.alpha) * 0.05f / 255f);
            if (projectile.localAI[0] == 0f)
            {
                Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 20);
                projectile.localAI[0] += 1f;
            }

            int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 170, default, 1.1f);
            Main.dust[num458].noGravity = true;
            Main.dust[num458].velocity *= 0.5f;
            Main.dust[num458].velocity += projectile.velocity * 0.1f;
        }

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(250, 50, 50, projectile.alpha);
		}

		public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<HellfireExplosion>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 240);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
