using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class HighExplosivePeanutShell : ModProjectile
    {
        private const int Lifetime = 180;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("High Explosive Peanut Shell");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.extraUpdates = 4;
            projectile.timeLeft = Lifetime;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            projectile.spriteDirection = 1;

            // Lighting
            Lighting.AddLight(projectile.Center, 0.75f, 0.65f, 0.08f);

            // Dirty dust, done dirt cheap
            {
                int dustID = 7; // wood flakes
                float scale = Main.rand.NextFloat(1f, 1.4f);
                Dust d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, dustID);
                d.noGravity = true;
                d.scale = scale;

                // Dust velocity is a complicated flaking function taken from Holy Fire Bullets
                d.velocity *= 0.2f;
                float angleDeviation = 0.17f;
                float angle = Main.rand.NextFloat(-angleDeviation, angleDeviation);
                Vector2 sprayVelocity = projectile.velocity.RotatedBy(angle) * 0.6f;
                d.velocity += sprayVelocity;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            // Grenade Launcher + Lunar Flare sounds for maximum meaty explosion
            Main.PlaySound(SoundID.Item62, projectile.Center);
            Main.PlaySound(SoundID.Item88, projectile.Center);

            // Massively inflate the projectile's hitbox
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 140;
            projectile.position.X = projectile.position.X - projectile.width / 2;
            projectile.position.Y = projectile.position.Y - projectile.height / 2;

            // Allow infinite piercing and ignoring iframes for this one extra hit
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;

            // Rocket III type explosion is now a utility for convenience
            projectile.LargeFieryExplosion();

            // Deal damage again. The explosion deals half the damage of the direct hit.
            projectile.damage /= 2;
            projectile.Damage();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            return true; // the projectile does indeed die on collision
        }
    }
}
