using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class HandheldTankShell : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tank Shell");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 11;
            projectile.timeLeft = 1800;
        }

        public override void AI()
        {
            drawOffsetX = -8;
            drawOriginOffsetY = 0;
            drawOriginOffsetX = -2;
            projectile.rotation = projectile.velocity.ToRotation();
            Vector2 pos = projectile.Center;
            projectile.localAI[0] += 1f;

            if (projectile.localAI[0] > 7f) // projectile has had more than 8 updates, draw fire trail
            {
                for (int i = 0; i < 5; ++i)
                {
                    pos -= projectile.velocity * ((float)i * 0.25f);
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
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(projectile.velocity.ToRotation());
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

        public override void Kill(int timeLeft)
        {
            // Grenade Launcher + Lunar Flare sounds for maximum meaty explosion
            Main.PlaySound(SoundID.Item62, projectile.Center);
            Main.PlaySound(SoundID.Item88, projectile.Center);

            // Transform the projectile's hitbox into a big explosion
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 140;
            projectile.position.X = projectile.position.X - projectile.width / 2;
            projectile.position.Y = projectile.position.Y - projectile.height / 2;

            // Rocket III type explosion is now a utility for convenience
            projectile.LargeFieryExplosion();

            // Make the explosion cause damage to nearby targets (makes projectile hit twice)
            projectile.Damage();
        }
    }
}
