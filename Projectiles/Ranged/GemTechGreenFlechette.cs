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
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.MaxUpdates = 3;
            projectile.timeLeft = projectile.MaxUpdates * 180;
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
            int afterimageCount = ProjectileID.Sets.TrailCacheLength[projectile.type];
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;
            for (int i = 0; i < afterimageCount; i++)
            {
                if (projectile.oldPos[i] == Vector2.Zero)
                    continue;

                float scaleFactor = MathHelper.Lerp(1f, 0.6f, i / (float)(afterimageCount - 1f));
                Color drawColor = Color.Lerp(Color.LightGreen, Color.White, i / (float)(afterimageCount - 1f));
                drawColor.A = (byte)(int)MathHelper.Lerp(105f, 0f, i / (float)(afterimageCount - 1f));
                drawPosition -= projectile.velocity.SafeNormalize(Vector2.Zero) * scaleFactor * 4.5f;
                spriteBatch.Draw(texture, drawPosition, null, drawColor, projectile.rotation, origin, projectile.scale * scaleFactor, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}
