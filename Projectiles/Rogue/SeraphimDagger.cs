using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class SeraphimDagger : ModProjectile
    {
        public const int SlowdownTime = 45;
        public const int AimTime = 25;
        public ref float Time => ref Projectile.ai[0];
        public override void SetStaticDefaults() => DisplayName.SetDefault("Seraphim");

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 150;
            Projectile.MaxUpdates = 2;
            Projectile.alpha = 0;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 15;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            // Initialize rotation.
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.ai[1] = Main.rand.NextFloat(-0.8f, 0.8f);
                Projectile.localAI[0] = 1f;
            }

            // Handle fade effects.
            Projectile.Opacity = Utils.InverseLerp(0f, 8f, Time, true) * Utils.InverseLerp(0f, 8f, Projectile.timeLeft, true);

            // Very rapidly slow down and spin.
            if (Time <= SlowdownTime)
            {
                float angularVelocityInterpolant = (float)Math.Pow(1f - Utils.InverseLerp(0f, SlowdownTime, Time, true), 2D);
                float angularVelocity = angularVelocityInterpolant * MathHelper.Pi / 6f;
                Projectile.rotation += angularVelocity;
                Projectile.velocity *= 0.95f;
            }

            // Aim at nearby targets.
            else if (Time <= SlowdownTime + AimTime)
            {
                if (Time == SlowdownTime + 25f)
                {
                    SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, Projectile.Center);
                }

                NPC potentialTarget = Projectile.Center.ClosestNPCAt(1600f);
                float idealRotation = Projectile.AngleTo(potentialTarget?.Center ?? Projectile.Center - Vector2.UnitY) + MathHelper.PiOver4;
                Projectile.rotation = Projectile.rotation.AngleLerp(idealRotation, 0.08f).AngleTowards(idealRotation, 0.1f);
                Projectile.velocity *= 0.9f;

                // And fire.
                if (Time == SlowdownTime + AimTime)
                {
                    Projectile.rotation = idealRotation;
                    Projectile.velocity = (idealRotation - MathHelper.PiOver4).ToRotationVector2() * 14f;
                }
            }

            // Accelerate.
            else if (Projectile.velocity.Length() < 28f)
                Projectile.velocity *= 1.045f;

            Time++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[Projectile.type];
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 baseDrawPosition = Projectile.Center - Main.screenPosition;

            float endFade = Utils.InverseLerp(0f, 12f, Projectile.timeLeft, true);
            Color mainColor = Color.Goldenrod * Projectile.Opacity * endFade * 1.5f;
            mainColor.A = 74;
            Color afterimageLightColor = Color.White * endFade;
            afterimageLightColor.A = 74;

            // Distribute many knives as they dissipate into light.
            for (int i = 0; i < 12; i++)
            {
                Vector2 drawPosition = baseDrawPosition + (MathHelper.TwoPi * i / 12f).ToRotationVector2() * (1f - Projectile.Opacity) * 6f;
                spriteBatch.Draw(texture, drawPosition, null, afterimageLightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            }

            // Draw specialized afterimages.
            for (int i = 0; i < 10; i++)
            {
                Vector2 drawPosition = baseDrawPosition - Projectile.velocity * i * 0.45f;
                Color afterimageColor = mainColor * (1f - i / 10f);
                spriteBatch.Draw(texture, drawPosition, null, afterimageColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}
