using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SeraphimProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public const float InitialSpeed = 64f;
        public const float SlowdownSpeed = 7f;
        public const int SlowdownTime = 50;
        public static readonly float SlowdownFactor = (float)Math.Pow(SlowdownSpeed / InitialSpeed, 1f / SlowdownTime);

        public ref float Time => ref Projectile.ai[0];

        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Seraphim";

        public override void SetDefaults()
        {
            Projectile.width = 82;
            Projectile.height = 82;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = SlowdownTime;
            Projectile.alpha = 0;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // Very rapidly slow down and fade out, transforming into light.
            if (Time <= SlowdownTime)
            {
                Projectile.Opacity = (float)Math.Pow(1f - Time / SlowdownTime, 2D);
                Projectile.velocity *= SlowdownFactor;

                int lightDustCount = (int)MathHelper.Lerp(8f, 1f, Projectile.Opacity);
                for (int i = 0; i < lightDustCount; i++)
                {
                    Vector2 dustSpawnPosition = Projectile.Center + Main.rand.NextVector2Unit() * (1f - Projectile.Opacity) * 45f;
                    Dust light = Dust.NewDustPerfect(dustSpawnPosition, 267);
                    light.color = Color.Lerp(Color.Gold, Color.White, Main.rand.NextFloat(0.5f, 1f));
                    light.velocity = Main.rand.NextVector2Circular(10f, 10f);
                    light.scale = MathHelper.Lerp(1.3f, 0.8f, Projectile.Opacity) * Main.rand.NextFloat(0.8f, 1.2f);
                    light.noGravity = true;
                }
            }

            Time++;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            // Create a light projectile that explodes into a laserbeam.
            int damage = Projectile.damage;
            float kb = Projectile.knockBack;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SeraphimAngelicLight>(), damage, kb, Projectile.owner);

            // As well as a fan of light daggers.
            NPC potentialTarget = Projectile.Center.ClosestNPCAt(1600f);
            for (int i = 0; i < Seraphim.SplitDaggerCount; i++)
            {
                float offsetAngle = MathHelper.Lerp(-0.9f, 0.9f, i / (float)(Seraphim.SplitDaggerCount - 1f));

                // Make the fan point away from nearby targets.
                if (potentialTarget != null)
                    offsetAngle -= Projectile.AngleTo(potentialTarget.Center);

                Vector2 fanVelocity = offsetAngle.ToRotationVector2() * 10f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, fanVelocity, ModContent.ProjectileType<SeraphimDagger>(), damage / 2, kb, Projectile.owner, i);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity *= 0.4f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 baseDrawPosition = Projectile.Center - Main.screenPosition;

            float endFade = Utils.GetLerpValue(0f, 12f, Projectile.timeLeft, true);
            Color mainColor = Color.White * Projectile.Opacity * endFade * 1.5f;
            mainColor.A = (byte)(255 - Projectile.alpha);
            Color afterimageLightColor = Color.White * endFade;
            afterimageLightColor.A = (byte)(255 - Projectile.alpha);

            // Distribute many knives as they dissipate into light.
            for (int i = 0; i < 18; i++)
            {
                Vector2 drawPosition = baseDrawPosition + (MathHelper.TwoPi * i / 18f).ToRotationVector2() * (1f - Projectile.Opacity) * 16f;
                Main.EntitySpriteDraw(texture, drawPosition, null, afterimageLightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            // Draw specialized afterimages.
            for (int i = 0; i < 8; i++)
            {
                Vector2 drawPosition = baseDrawPosition - Projectile.velocity * i * 0.3f;
                Color afterimageColor = mainColor * (1f - i / 8f);
                Main.EntitySpriteDraw(texture, drawPosition, null, afterimageColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
