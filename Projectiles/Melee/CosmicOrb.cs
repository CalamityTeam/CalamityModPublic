using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class CosmicOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orb");
        }

        public override void SetDefaults()
        {
            projectile.extraUpdates = 0;
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, new Vector3(0.075f, 0.5f, 0.15f));

            projectile.velocity *= 0.985f;
            projectile.rotation += projectile.velocity.X * 0.2f;

            if (projectile.velocity.X > 0f)
            {
                projectile.rotation += 0.08f;
            }
            else
            {
                projectile.rotation -= 0.08f;
            }

            projectile.ai[1] += 1f;
            if (projectile.ai[1] > 30f)
            {
                projectile.alpha += 10;
                if (projectile.alpha >= 255)
                {
                    projectile.alpha = 255;
                    projectile.Kill();
                    return;
                }
            }

            CalamityGlobalProjectile.MagnetSphereHitscan(projectile, 300f, 6f, 8f, 5, ModContent.ProjectileType<CosmicBolt>());
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item54, projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int num192 = (int)(10f * projectile.scale);
                int d = Dust.NewDust(projectile.Center - Vector2.One * (float)num192, num192 * 2, num192 * 2, 242, 0f, 0f, 0, default, 1f);
                Dust dust = Main.dust[d];
                Vector2 offset = Vector2.Normalize(dust.position - projectile.Center);
                dust.position = projectile.Center + offset * (float)num192 * projectile.scale;
                if (i < 30)
                {
                    dust.velocity = offset * dust.velocity.Length();
                }
                else
                {
                    dust.velocity = offset * Main.rand.NextFloat(4.5f, 9f);
                }
                dust.color = Main.hslToRgb(0.95f, 0.41f + Main.rand.NextFloat() * 0.2f, 0.93f);
                dust.color = Color.Lerp(dust.color, Color.White, 0.3f);
                dust.noGravity = true;
                dust.scale = 0.7f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200 - projectile.alpha, 200 - projectile.alpha, 200 - projectile.alpha, 200 - projectile.alpha);
        }
    }
}
