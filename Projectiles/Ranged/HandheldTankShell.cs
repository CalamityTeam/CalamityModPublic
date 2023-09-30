using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class HandheldTankShell : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 11;
            Projectile.timeLeft = 1800;
        }

        public override void AI()
        {
            DrawOffsetX = -8;
            DrawOriginOffsetY = 0;
            DrawOriginOffsetX = -2;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 pos = Projectile.Center;
            Projectile.localAI[0] += 1f;

            if (Projectile.localAI[0] > 7f) // projectile has had more than 8 updates, draw fire trail
            {
                for (int i = 0; i < 5; ++i)
                {
                    pos -= Projectile.velocity * ((float)i * 0.25f);
                    int idx = Dust.NewDust(pos, 1, 1, 158, 0f, 0f, 0, default, 1f);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].position = pos;
                    Main.dust[idx].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[idx].velocity *= 0.3f;
                }
                return;
            }
            else // projectile has had less than 8 updates, draw smoke
            {
                for (int i = 0; i < 30; ++i)
                {
                    // Pick a random type of smoke (there's a little fire mixed in)
                    int dustID;
                    switch (Main.rand.Next(6))
                    {
                        case 0:
                            dustID = 55;
                            break;
                        case 1:
                        case 2:
                            dustID = 54;
                            break;
                        default:
                            dustID = 53;
                            break;
                    }

                    // Choose a random speed and angle to belch out the smoke
                    float dustSpeed = Main.rand.NextFloat(3.0f, 13.0f);
                    float angleRandom = 0.06f;
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                    // Pick a size for the smoke particle
                    float scale = Main.rand.NextFloat(0.5f, 1.6f);

                    // Actually spawn the smoke
                    int idx = Dust.NewDust(pos, 1, 1, dustID, dustVel.X, dustVel.Y, 0, default, scale);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].position = pos;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            // Grenade Launcher + Lunar Flare sounds for maximum meaty explosion
            SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);

            // Transform the projectile's hitbox into a big explosion
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 140;
            Projectile.position.X = Projectile.position.X - Projectile.width / 2;
            Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;

            // Rocket III type explosion is now a utility for convenience
            Projectile.LargeFieryExplosion();

            // Make the explosion cause damage to nearby targets (makes projectile hit twice)
            Projectile.Damage();
        }
    }
}
