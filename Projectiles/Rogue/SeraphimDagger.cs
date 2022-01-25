using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SeraphimDagger : ModProjectile
    {
        public const int SlowdownTime = 45;
        public const int AimTime = 25;
        public ref float Time => ref projectile.ai[0];
        public override void SetStaticDefaults() => DisplayName.SetDefault("Seraphim");

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 150;
            projectile.MaxUpdates = 2;
            projectile.alpha = 0;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = projectile.MaxUpdates * 15;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            // Initialize rotation.
            if (projectile.localAI[0] == 0f)
            {
                projectile.ai[1] = Main.rand.NextFloat(-0.8f, 0.8f);
                projectile.localAI[0] = 1f;
            }

            // Handle fade effects.
            projectile.Opacity = Utils.InverseLerp(0f, 8f, Time, true) * Utils.InverseLerp(0f, 8f, projectile.timeLeft, true);

            // Very rapidly slow down and spin.
            if (Time <= SlowdownTime)
            {
                float angularVelocityInterpolant = (float)Math.Pow(1f - Utils.InverseLerp(0f, SlowdownTime, Time, true), 2D);
                float angularVelocity = angularVelocityInterpolant * MathHelper.Pi / 6f;
                projectile.rotation += angularVelocity;
                projectile.velocity *= 0.95f;
            }

            // Aim at nearby targets.
            else if (Time <= SlowdownTime + AimTime)
            {
                if (Time == SlowdownTime + 25f)
                {
                    Main.PlaySound(SoundID.DD2_WyvernDiveDown, projectile.Center);
                    Main.PlaySound(SoundID.DD2_DarkMageHealImpact, projectile.Center);
                }

                NPC potentialTarget = projectile.Center.ClosestNPCAt(1600f);
                float idealRotation = projectile.AngleTo(potentialTarget?.Center ?? projectile.Center - Vector2.UnitY) + MathHelper.PiOver4;
                projectile.rotation = projectile.rotation.AngleLerp(idealRotation, 0.08f).AngleTowards(idealRotation, 0.1f);
                projectile.velocity *= 0.9f;

                // And fire.
                if (Time == SlowdownTime + AimTime)
                {
                    projectile.rotation = idealRotation;
                    projectile.velocity = (idealRotation - MathHelper.PiOver4).ToRotationVector2() * 14f;
                }
            }

            // Accelerate.
            else if (projectile.velocity.Length() < 28f)
                projectile.velocity *= 1.045f;

            Time++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 baseDrawPosition = projectile.Center - Main.screenPosition;

            float endFade = Utils.InverseLerp(0f, 12f, projectile.timeLeft, true);
            Color mainColor = Color.Goldenrod * projectile.Opacity * endFade * 1.5f;
            mainColor.A = 74;
            Color afterimageLightColor = Color.White * endFade;
            afterimageLightColor.A = 74;

            // Distribute many knives as they dissipate into light.
            for (int i = 0; i < 12; i++)
            {
                Vector2 drawPosition = baseDrawPosition + (MathHelper.TwoPi * i / 12f).ToRotationVector2() * (1f - projectile.Opacity) * 6f;
                spriteBatch.Draw(texture, drawPosition, null, afterimageLightColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }

            // Draw specialized afterimages.
            for (int i = 0; i < 10; i++)
            {
                Vector2 drawPosition = baseDrawPosition - projectile.velocity * i * 0.45f;
                Color afterimageColor = mainColor * (1f - i / 10f);
                spriteBatch.Draw(texture, drawPosition, null, afterimageColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}
