using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class MajesticSparkle : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];
        public ref float ColorSpectrumHue => ref Projectile.ai[1];
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
            Projectile.width = 72;
            Projectile.height = 72;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Lifetime;
            Projectile.scale = 0.001f;
        }

        public override void AI()
        {
            if (Time == 1f)
            {
                Projectile.scale = Main.rand.NextFloat(0.3f, 0.75f);
                CalamityGlobalProjectile.ExpandHitboxBy(Projectile, (int)(Projectile.scale * 72f));

                ColorSpectrumHue = Main.rand.NextFloat(0f, 0.9999f);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.netUpdate = true;
            }

            Projectile.velocity *= 0.96f;
            Projectile.rotation = Projectile.rotation.AngleLerp(MathHelper.PiOver2, 0.085f);

            // Go 33% across the color spectrum throughout the sparkle's lifetime instead of using a static color.
            ColorSpectrumHue = (ColorSpectrumHue + 0.333f / Lifetime) % 0.999f;

            Projectile.Opacity = Utils.GetLerpValue(0f, FadeinTime, Time, true) * Utils.GetLerpValue(Lifetime, Lifetime - FadeoutTime, Time, true);
            Projectile.velocity = Projectile.velocity.RotatedBy(Math.Sin(Time / 30f) * 0.0125f);

            Time++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sparkleTexture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Color sparkleColor = Main.hslToRgb(ColorSpectrumHue, 1f, 0.5f) * Projectile.Opacity * 0.5f;
            sparkleColor.A = 0;

            sparkleColor *= MathHelper.Lerp(1f, 1.5f, Utils.GetLerpValue(Lifetime * 0.5f - 15f, Lifetime * 0.5f + 15f, Time, true));

            Color orthogonalSparkleColor = Color.Lerp(sparkleColor, Color.White, 0.5f) * 0.5f;

            Vector2 origin = sparkleTexture.Size() * 0.5f;

            Vector2 sparkleScale = new Vector2(0.3f, 1f) * Projectile.Opacity * Projectile.scale;
            Vector2 orthogonalsparkleScale = new Vector2(0.3f, 2f) * Projectile.Opacity * Projectile.scale;

            Main.EntitySpriteDraw(sparkleTexture, drawPosition, null, sparkleColor, MathHelper.PiOver2 + Projectile.rotation, origin, orthogonalsparkleScale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(sparkleTexture, drawPosition, null,sparkleColor, Projectile.rotation, origin, sparkleScale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(sparkleTexture, drawPosition, null, orthogonalSparkleColor, MathHelper.PiOver2 + Projectile.rotation, origin, orthogonalsparkleScale * 0.6f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(sparkleTexture, drawPosition, null, orthogonalSparkleColor, Projectile.rotation, origin, sparkleScale * 0.6f, SpriteEffects.None, 0);
            return false;
        }
    }
}
