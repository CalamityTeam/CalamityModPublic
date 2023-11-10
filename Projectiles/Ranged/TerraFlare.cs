using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class TerraFlare : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/ExtraTextures/SmallGreyscaleCircle";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 2;
            Projectile.timeLeft = 90 * Projectile.MaxUpdates;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
        }

        public override void AI() => CalamityUtils.HomeInOnNPC(Projectile, true, 640f, 15f, 20f);

        public override void OnKill(int timeLeft)
        {
            // Circular spread of clouds
            for (int i = 0; i < 8; i++)
            {
                Vector2 smokeVel = Main.rand.NextVector2Circular(8f, 8f);
                Color smokeColor = Main.rand.NextBool() ? Color.Lime : Color.Turquoise;
                Particle smoke = new MediumMistParticle(Projectile.Center, smokeVel, smokeColor, Color.Black, Main.rand.NextFloat(0.4f, 0.8f), 200 - Main.rand.Next(60), 0.1f);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D lightTexture = ModContent.Request<Texture2D>(Texture).Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float colorInterpolation = MathF.Cos(Projectile.timeLeft / 16f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;
                Color color = Color.Lerp(Color.Lime, Color.Turquoise, colorInterpolation);
                Vector2 drawPosition = Projectile.oldPos[i] + lightTexture.Size() * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(-15f, -15f);
                Color outerColor = color;
                Color innerColor = Color.Lerp(color, Color.Gold, 0.8f) * 0.5f;
                float intensity = 0.9f + 0.15f * MathF.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);

                // Become smaller the futher along the old positions we are.
                intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);

                Vector2 outerScale = new Vector2(1.25f) * intensity;
                Vector2 innerScale = new Vector2(1.25f) * intensity * 0.7f;
                outerColor *= intensity * Projectile.scale;
                innerColor *= intensity * Projectile.scale;
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, 0f, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, 0f, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
