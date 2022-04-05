using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class PartySparkle : ModProjectile
    {
        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float ColorSpectrumHue
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public const int Lifetime = 90;
        public const int FadeinTime = 18;
        public const int FadeoutTime = 18;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pretty Sparkle");
        }

        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 72;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.scale = 0.001f;
        }

        public override void AI()
        {
            if (Time == 1f)
            {
                Projectile.scale = Main.rand.NextFloat(0.4f, 1.1f);
                CalamityGlobalProjectile.ExpandHitboxBy(Projectile, (int)(72 * Projectile.scale));
                ColorSpectrumHue = Main.rand.NextFloat(0f, 0.9999f);
                Projectile.netUpdate = true;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            Time++;

            Projectile.velocity *= 0.96f;

            Projectile.rotation = Projectile.rotation.AngleLerp(MathHelper.PiOver2, 0.085f);

            ColorSpectrumHue = (ColorSpectrumHue + 0.333f / Lifetime) % 0.999f; // Go 33% across the color spectrum throughout the sparkle's life instead of using a static sprite.

            Projectile.Opacity = Utils.GetLerpValue(0f, FadeinTime, Time, true) * Utils.GetLerpValue(Lifetime, Lifetime - FadeoutTime, Time, true);
            Projectile.velocity = Projectile.velocity.RotatedBy(Math.Sin(Time / 30f) * 0.0125f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sparkleTexture = ModContent.Request<Texture2D>(Texture);

            Color sparkleColor = CalamityUtils.MulticolorLerp(ColorSpectrumHue, RainbowPartyCannon.ColorSet) * Projectile.Opacity * 0.5f;
            sparkleColor.A = 0;

            sparkleColor *= MathHelper.Lerp(1f, 1.5f, Utils.GetLerpValue(Lifetime * 0.5f - 15f, Lifetime * 0.5f + 15f, Time, true));

            Color orthogonalsparkleColor = Color.Lerp(sparkleColor, Color.White, 0.5f) * 0.5f;

            Vector2 origin = sparkleTexture.Size() / 2f;

            Vector2 sparkleScale = new Vector2(0.3f, 1f) * Projectile.Opacity * Projectile.scale;
            Vector2 orthogonalsparkleScale = new Vector2(0.3f, 2f) * Projectile.Opacity * Projectile.scale;

            Main.EntitySpriteDraw(sparkleTexture,
                             Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                             null,
                             sparkleColor,
                             MathHelper.PiOver2 + Projectile.rotation,
                             origin,
                             orthogonalsparkleScale,
                             SpriteEffects.None,
                             0f);
            Main.EntitySpriteDraw(sparkleTexture,
                             Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                             null,
                             sparkleColor,
                             Projectile.rotation,
                             origin,
                             sparkleScale,
                             SpriteEffects.None,
                             0f);
            Main.EntitySpriteDraw(sparkleTexture,
                             Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                             null,
                             orthogonalsparkleColor,
                             MathHelper.PiOver2 + Projectile.rotation,
                             origin,
                             orthogonalsparkleScale * 0.6f,
                             SpriteEffects.None,
                             0f);
            Main.EntitySpriteDraw(sparkleTexture,
                             Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                             null,
                             orthogonalsparkleColor,
                             Projectile.rotation,
                             origin,
                             sparkleScale * 0.6f,
                             SpriteEffects.None,
                             0f);
            return false;
        }
    }
}
