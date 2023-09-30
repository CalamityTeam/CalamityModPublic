using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class PeanutRocket : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/Ranged/HighExplosivePeanutShell";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = 1;

            // Lighting
            Lighting.AddLight(Projectile.Center, 0.75f, 0.65f, 0.08f);

            // Dirty dust, done dirt cheap
            {
                int dustID = 7; // wood flakes
                float scale = Main.rand.NextFloat(1f, 1.4f);
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustID);
                d.noGravity = true;
                d.scale = scale;

                // Dust velocity is a complicated flaking function taken from Holy Fire Bullets
                d.velocity *= 0.2f;
                float angleDeviation = 0.17f;
                float angle = Main.rand.NextFloat(-angleDeviation, angleDeviation);
                Vector2 sprayVelocity = Projectile.velocity.RotatedBy(angle) * 0.6f;
                d.velocity += sprayVelocity;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            // Grenade Launcher + Lunar Flare sounds for maximum meaty explosion
            SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);

            // Massively inflate the projectile's hitbox
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 140;
            Projectile.position.X = Projectile.position.X - Projectile.width / 2;
            Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;

            // Allow infinite piercing and ignoring iframes for this one extra hit
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            // Rocket III type explosion is now a utility for convenience
            Projectile.LargeFieryExplosion();

            // Deal damage again. The explosion deals half the damage of the direct hit.
            Projectile.damage /= 2;
            Projectile.Damage();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            return true; // the projectile does indeed die on collision
        }
    }
}
