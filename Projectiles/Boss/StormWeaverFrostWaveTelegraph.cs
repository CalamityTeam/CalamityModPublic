using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class StormWeaverFrostWaveTelegraph : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 4;
            Projectile.Opacity = 1f;
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() < Projectile.ai[1])
            {
                Projectile.velocity *= 1.01f;
                if (Projectile.velocity.Length() > Projectile.ai[1])
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= Projectile.ai[1];
                }
            }

            if (Projectile.timeLeft < 60)
                Projectile.Opacity = Projectile.timeLeft / 60f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D pulseTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SmallGreyscaleCircle").Value;

            // Aura drawing.
            for (int i = 0; i < 5; i++)
            {
                Vector2 offset = (i / 5f * MathHelper.TwoPi).ToRotationVector2() * 24f;
                float time = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1.8f);
                float angle = time * MathHelper.Pi + Main.GlobalTimeWrappedHourly * 2.1f;
                float scale = 1.1f + time * 0.2f;
                Main.EntitySpriteDraw(pulseTexture, Projectile.Center + offset - Main.screenPosition, null, Color.LightCyan * 0.3f * Projectile.Opacity, angle, pulseTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);
            }

            return false;
        }
    }
}
