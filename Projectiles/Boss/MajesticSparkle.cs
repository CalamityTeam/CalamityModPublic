using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class MajesticSparkle : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public ref float ColorSpectrumHue => ref projectile.ai[1];
        public const int Lifetime = 90;
        public const int FadeinTime = 18;
        public const int FadeoutTime = 18;
        public override string Texture => "CalamityMod/Projectiles/StarProj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Majestic Sparkle");
        }

        public override void SetDefaults()
        {
            projectile.width = 72;
            projectile.height = 72;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = Lifetime;
            projectile.scale = 0.001f;
        }

        public override void AI()
        {
            if (Time == 1f)
            {
                projectile.scale = Main.rand.NextFloat(0.3f, 0.75f);
                CalamityGlobalProjectile.ExpandHitboxBy(projectile, (int)(projectile.scale * 72f));

                ColorSpectrumHue = Main.rand.NextFloat(0f, 0.9999f);
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                projectile.netUpdate = true;
            }

            projectile.velocity *= 0.96f;
            projectile.rotation = projectile.rotation.AngleLerp(MathHelper.PiOver2, 0.085f);

            // Go 33% across the color spectrum throughout the sparkle's lifetime instead of using a static color.
            ColorSpectrumHue = (ColorSpectrumHue + 0.333f / Lifetime) % 0.999f;

            projectile.Opacity = Utils.InverseLerp(0f, FadeinTime, Time, true) * Utils.InverseLerp(Lifetime, Lifetime - FadeoutTime, Time, true);
            projectile.velocity = projectile.velocity.RotatedBy(Math.Sin(Time / 30f) * 0.0125f);

            Time++;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D sparkleTexture = ModContent.GetTexture(Texture);

            Vector2 drawPosition = projectile.Center - Main.screenPosition + Vector2.UnitY * projectile.gfxOffY;
            Color sparkleColor = Main.hslToRgb(ColorSpectrumHue, 1f, 0.5f) * projectile.Opacity * 0.5f;
            sparkleColor.A = 0;

            sparkleColor *= MathHelper.Lerp(1f, 1.5f, Utils.InverseLerp(Lifetime * 0.5f - 15f, Lifetime * 0.5f + 15f, Time, true));

            Color orthogonalSparkleColor = Color.Lerp(sparkleColor, Color.White, 0.5f) * 0.5f;

            Vector2 origin = sparkleTexture.Size() * 0.5f;

            Vector2 sparkleScale = new Vector2(0.3f, 1f) * projectile.Opacity * projectile.scale;
            Vector2 orthogonalsparkleScale = new Vector2(0.3f, 2f) * projectile.Opacity * projectile.scale;

            spriteBatch.Draw(sparkleTexture, drawPosition, null, sparkleColor, MathHelper.PiOver2 + projectile.rotation, origin, orthogonalsparkleScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(sparkleTexture, drawPosition, null,sparkleColor, projectile.rotation, origin, sparkleScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(sparkleTexture, drawPosition, null, orthogonalSparkleColor, MathHelper.PiOver2 + projectile.rotation, origin, orthogonalsparkleScale * 0.6f, SpriteEffects.None, 0f);
            spriteBatch.Draw(sparkleTexture, drawPosition, null, orthogonalSparkleColor, projectile.rotation, origin, sparkleScale * 0.6f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
