using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
	public class TerraBulletMain : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Bullet");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.alpha = 255;
            projectile.extraUpdates = 3;
			projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
		}

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.4f, 0f);

            // localAI is used as an invisibility counter. The bullet fades into existence after 15 startup frames.
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                // After 15 frames, the alpha will be exactly 0
                if (projectile.alpha > 0)
                    projectile.alpha -= 17;

                // Dust type 74, scale 0.8, no gravity, no light, no velocity
                Vector2 pos = projectile.Center - projectile.velocity * 0.1f;
                Dust d = Dust.NewDustDirect(pos, 0, 0, 74, Scale: 0.8f);
                d.position = pos;
                d.velocity = Vector2.Zero;
                d.noGravity = true;
                d.noLight = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(projectile, 0, lightColor);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                for (int b = 0; b < 2; b++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<TerraBulletSplit>(), (int)(projectile.damage * 0.3), 0f, projectile.owner, 0f, 0f);
                }
            }
            Main.PlaySound(SoundID.Item118, projectile.position);
        }
    }
}
