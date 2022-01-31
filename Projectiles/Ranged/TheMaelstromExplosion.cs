using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class TheMaelstromExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 75;
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = -1;
            projectile.scale = 0.1f;

            // Hardcoded spaghetti appearss to make the scale setting above tamper with the hitbox.
            projectile.width = projectile.height = (int)(60 / projectile.scale);
            projectile.timeLeft = 60;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 16;
        }

        public override void AI()
        {
            projectile.scale = MathHelper.Lerp(projectile.scale, 1f, 0.125f);
            projectile.Opacity = projectile.scale * Utils.InverseLerp(0f, 30f, projectile.timeLeft, true);

            // Emit sparks and dust.
            if (Main.netMode != NetmodeID.Server)
            {
                int sparkLifetime = Main.rand.Next(22, 36);
                float sparkScale = Main.rand.NextFloat(1f, 1.3f);
                Color sparkColor = Color.Lerp(Color.Cyan, Color.DarkBlue, Main.rand.NextFloat(0.7f));
                Vector2 sparkVelocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(3f, 8f);

                SparkParticle spark = new SparkParticle(projectile.Center, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                GeneralParticleHandler.SpawnParticle(spark);

                Vector2 dustSpawnOffset = Main.rand.NextVector2Circular(projectile.width, projectile.height) * projectile.scale * 0.4f;
                Dust electricity = Dust.NewDustPerfect(projectile.Center + dustSpawnOffset, 267);
                electricity.color = Color.Cyan;
                electricity.color.A = 84;
                electricity.scale *= Main.rand.NextFloat(0.7f, 1.2f);
                electricity.velocity = dustSpawnOffset.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.PiOver2).RotatedByRandom(0.37f);
                electricity.velocity *= Main.rand.NextFloat(2f, 6f);
                electricity.noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 origin = texture.Size() * 0.5f;

            Color frontAfterimageColor = projectile.GetAlpha(Color.Lerp(Color.Cyan, Color.DarkBlue, projectile.identity / 7f % 0.8f)) * 0.2f;
            frontAfterimageColor.A = 0;
            for (int i = 0; i < 12; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 12f + projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 2f;
                Vector2 afterimageDrawPosition = projectile.Center + drawOffset - Main.screenPosition;
                spriteBatch.Draw(texture, afterimageDrawPosition, null, frontAfterimageColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CalamityUtils.CircularHitboxCollision(projectile.Center, projectile.Size.Length() * projectile.scale / 1.414f, targetHitbox);
        }

        public override bool CanDamage() => projectile.Opacity > 0.4f;
    }
}
