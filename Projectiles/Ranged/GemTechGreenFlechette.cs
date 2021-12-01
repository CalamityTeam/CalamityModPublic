using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class GemTechGreenFlechette : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gem Tech Flechette");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.Opacity = Utils.InverseLerp(180f, 174f, projectile.timeLeft, true);

            if (projectile.localAI[0] == 0f)
            {
                // Create a circular puff of green dust.
                float initialSpeed = Main.rand.NextFloat(2.5f, 4.5f);
                for (int i = 0; i < 12; i++)
                {
                    Dust crystalShard = Dust.NewDustPerfect(projectile.Center, 267);
                    crystalShard.velocity = (MathHelper.TwoPi * i / 12f).ToRotationVector2() * initialSpeed * Main.rand.NextFloat(0.6f, 1f);
                    crystalShard.velocity = crystalShard.velocity.RotatedByRandom(0.37f);
                    crystalShard.scale = 1.25f;
                    crystalShard.color = Color.ForestGreen;
                    crystalShard.noGravity = true;
                }
                projectile.localAI[0] = 1f;
            }
        }

        public override void Kill(int timeLeft)
        {
            // Play a shatter sound.
            Main.PlaySound(SoundID.Item27, projectile.Center);

            // Create a circular puff of green dust.
            float initialSpeed = Main.rand.NextFloat(2.5f, 4.5f);
            for (int i = 0; i < 16; i++)
            {
                Dust crystalShard = Dust.NewDustPerfect(projectile.Center, 267);
                crystalShard.velocity = (MathHelper.TwoPi * i / 16f).ToRotationVector2() * initialSpeed;
                crystalShard.scale = 1.25f;
                crystalShard.color = Color.ForestGreen;
                crystalShard.noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            lightColor = Color.Lerp(lightColor, Color.White, 0.5f);
            lightColor.A = 84;
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor);
            return false;
        }
    }
}
