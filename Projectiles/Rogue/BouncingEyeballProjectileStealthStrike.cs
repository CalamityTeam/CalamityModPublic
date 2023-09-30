using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class BouncingEyeballProjectileStealthStrike : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public const float Bounciness = 1.35f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 280;
            Projectile.penetrate = -1;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Math.Abs(Projectile.velocity.X) > 23f)
            {
                Projectile.velocity.X = Math.Sign(Projectile.velocity.X) * 23f;
            }
            if (Math.Abs(Projectile.velocity.Y) > 23f)
            {
                Projectile.velocity.Y = Math.Sign(Projectile.velocity.Y) * 23f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity != oldVelocity)
            {
                Projectile.velocity = Main.rand.NextFloat(-1.15f, -0.85f) * oldVelocity * Bounciness;
            }
            SoundEngine.PlaySound(SoundID.NPCHit19 with { Volume = SoundID.NPCHit19.Volume * 0.7f}, Projectile.Center);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            // Explode into a large display of blood.
            SoundEngine.PlaySound(SoundID.NPCHit19 with { Volume = SoundID.NPCHit19.Volume * 0.7f}, Projectile.Center);
            int dustCount = Main.rand.Next(15, 26);
            for (int index = 0; index < dustCount; index++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(4f, 9f) + Projectile.velocity / 2f;
                Dust.NewDust(Projectile.Center, 4, 4, DustID.Blood, velocity.X, velocity.Y);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
