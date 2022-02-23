using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SeraphimProjectile : ModProjectile
    {
        public const float InitialSpeed = 64f;
        public const float SlowdownSpeed = 7f;
        public const int SlowdownTime = 50;
        public static readonly float SlowdownFactor = (float)Math.Pow(SlowdownSpeed / InitialSpeed, 1f / SlowdownTime);

        public ref float Time => ref projectile.ai[0];

        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Seraphim";

        public override void SetStaticDefaults() => DisplayName.SetDefault("Seraphim");

        public override void SetDefaults()
        {
            projectile.width = 82;
            projectile.height = 82;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = SlowdownTime;
            projectile.alpha = 0;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 14;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // Very rapidly slow down and fade out, transforming into light.
            if (Time <= SlowdownTime)
            {
                projectile.Opacity = (float)Math.Pow(1f - Time / SlowdownTime, 2D);
                projectile.velocity *= SlowdownFactor;

                int lightDustCount = (int)MathHelper.Lerp(8f, 1f, projectile.Opacity);
                for (int i = 0; i < lightDustCount; i++)
                {
                    Vector2 dustSpawnPosition = projectile.Center + Main.rand.NextVector2Unit() * (1f - projectile.Opacity) * 45f;
                    Dust light = Dust.NewDustPerfect(dustSpawnPosition, 267);
                    light.color = Color.Lerp(Color.Gold, Color.White, Main.rand.NextFloat(0.5f, 1f));
                    light.velocity = Main.rand.NextVector2Circular(10f, 10f);
                    light.scale = MathHelper.Lerp(1.3f, 0.8f, projectile.Opacity) * Main.rand.NextFloat(0.8f, 1.2f);
                    light.noGravity = true;
                }
            }

            Time++;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer != projectile.owner)
                return;

            // Create a light projectile that explodes into a laserbeam.
            int damage = projectile.damage;
            float kb = projectile.knockBack;
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<SeraphimAngelicLight>(), damage, kb, projectile.owner);

            // As well as a fan of light daggers.
            NPC potentialTarget = projectile.Center.ClosestNPCAt(1600f);
            for (int i = 0; i < Seraphim.SplitDaggerCount; i++)
            {
                float offsetAngle = MathHelper.Lerp(-0.9f, 0.9f, i / (float)(Seraphim.SplitDaggerCount - 1f));

                // Make the fan point away from nearby targets.
                if (potentialTarget != null)
                    offsetAngle -= projectile.AngleTo(potentialTarget.Center);

                Vector2 fanVelocity = offsetAngle.ToRotationVector2() * 10f;
                Projectile.NewProjectile(projectile.Center, fanVelocity, ModContent.ProjectileType<SeraphimDagger>(), damage / 2, kb, projectile.owner, i);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.velocity *= 0.4f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 baseDrawPosition = projectile.Center - Main.screenPosition;

            float endFade = Utils.InverseLerp(0f, 12f, projectile.timeLeft, true);
            Color mainColor = Color.White * projectile.Opacity * endFade * 1.5f;
            mainColor.A = (byte)(255 - projectile.alpha);
            Color afterimageLightColor = Color.White * endFade;
            afterimageLightColor.A = (byte)(255 - projectile.alpha);

            // Distribute many knives as they dissipate into light.
            for (int i = 0; i < 18; i++)
            {
                Vector2 drawPosition = baseDrawPosition + (MathHelper.TwoPi * i / 18f).ToRotationVector2() * (1f - projectile.Opacity) * 16f;
                spriteBatch.Draw(texture, drawPosition, null, afterimageLightColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }

            // Draw specialized afterimages.
            for (int i = 0; i < 8; i++)
            {
                Vector2 drawPosition = baseDrawPosition - projectile.velocity * i * 0.3f;
                Color afterimageColor = mainColor * (1f - i / 8f);
                spriteBatch.Draw(texture, drawPosition, null, afterimageColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}
