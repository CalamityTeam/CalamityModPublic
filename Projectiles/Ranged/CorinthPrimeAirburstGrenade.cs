using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.NPCs.ExoMechs.Ares;
using System;

namespace CalamityMod.Projectiles.Ranged
{
    public class CorinthPrimeAirburstGrenade : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corinth Prime Airburst Shot");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 130;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            //Animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            // Gravity
            if (Projectile.velocity.Y < 12f && Projectile.timeLeft < 115)
                Projectile.velocity.Y += 0.1f;

            // Rotate the projectile
            Projectile.rotation = Projectile.velocity.ToRotation();

            Vector2 pos = Projectile.Center;

            // Belch out dust when the grenade is fired.
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 7f) // Projectile has had more than 8 updates, draw fire trail.
            {
                for (int i = 0; i < 5; i++)
                {
                    pos -= Projectile.velocity * (i * 0.25f);
                    int idx = Dust.NewDust(pos, Projectile.width, Projectile.height, Main.rand.NextBool() ? 206 : 187, 0f, 0f, 0, default, 1f);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].position = pos;
                    Main.dust[idx].scale = Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[idx].velocity *= 0.3f;
                }
            }
            else // Projectile has had less than 8 updates, draw smoke.
            {
                for (int i = 0; i < 30; i++)
                {
                    // Pick a random type of smoke (there's a little fire mixed in).
                    int dustID;
                    switch (Main.rand.Next(6))
                    {
                        case 0:
                            dustID = 186;
                            break;
                        case 1:
                        case 2:
                            dustID = 20;
                            break;
                        default:
                            dustID = 56;
                            break;
                    }

                    // Choose a random speed and angle to belch out the smoke.
                    float dustSpeed = Main.rand.NextFloat(3.0f, 13.0f);
                    float angleRandom = 0.06f;
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                    // Pick a size for the smoke particle.
                    float scale = Main.rand.NextFloat(0.5f, 1.6f);

                    // Actually spawn the smoke.
                    int idx = Dust.NewDust(pos, Projectile.width, Projectile.height, dustID, dustVel.X, dustVel.Y, 0, default, scale);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].position = pos;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            bool weakExplosion = Projectile.localAI[0] < 90f;

            // Play weaker sound if not fully charged and strong sound if fully charged.
            if (weakExplosion)
                SoundEngine.PlaySound(TeslaCannon.FireSound, Projectile.Center);
            else
                SoundEngine.PlaySound(AresGaussNuke.NukeExplosionSound, Projectile.Center);

            if (Main.myPlayer == Projectile.owner)
            {
                // Make big boom that does big damage if it's been active for 1.5 seconds.
                // Make small boom that does 1 damage if it hasn't been active for 1.5 seconds.
                if (weakExplosion)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile explosion = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CorinthPrimeAirburst>(), 1, 0f, Projectile.owner);
                        explosion.ai[1] = Main.rand.NextFloat(64f, 174f) + i * 20f; // Randomize the maximum radius.
                        explosion.localAI[1] = Main.rand.NextFloat(0.18f, 0.3f); // And the interpolation step.
                        explosion.netUpdate = true;
                    }
                }
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        Projectile explosion = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CorinthPrimeAirburst>(), (int)(Projectile.damage * 0.3), Projectile.knockBack, Projectile.owner);
                        if (explosion.whoAmI.WithinBounds(Main.maxProjectiles))
                        {
                            explosion.ai[1] = Main.rand.NextFloat(256f, 696f) + i * 45f; // Randomize the maximum radius.
                            explosion.localAI[1] = Main.rand.NextFloat(0.08f, 0.25f); // And the interpolation step.
                            explosion.Opacity = MathHelper.Lerp(0.18f, 0.6f, i / 7f) + Main.rand.NextFloat(-0.08f, 0.08f);
                            explosion.netUpdate = true;
                        }
                    }
                }
            }

            // Dust circles
            int totalDust = weakExplosion ? 100 : 400;
            for (int num640 = 0; num640 < totalDust; num640++)
            {
                int dustType = 206;
                float num641 = 32f;
                if (num640 < 300)
                {
                    dustType = 187;
                    num641 = 16f;
                }
                if (num640 < 200)
                    num641 = 8f;
                if (num640 < 100)
                    num641 = 4f;

                int num643 = Dust.NewDust(Projectile.Center, 6, 6, dustType, 0f, 0f, 100, default, 1f);
                float num644 = Main.dust[num643].velocity.X;
                float num645 = Main.dust[num643].velocity.Y;

                if (num644 == 0f && num645 == 0f)
                    num644 = 1f;

                float num646 = (float)Math.Sqrt(num644 * num644 + num645 * num645);
                num646 = num641 / num646;
                num644 *= num646;
                num645 *= num646;

                float scale = 1f;
                switch ((int)num641)
                {
                    case 4:
                        scale = 1.1f;
                        break;
                    case 8:
                        scale = 1.2f;
                        break;
                    case 16:
                        scale = 1.4f;
                        break;
                    case 32:
                        scale = 1.8f;
                        break;
                    default:
                        break;
                }

                Dust dust = Main.dust[num643];
                dust.velocity *= 0.5f;
                dust.velocity.X += num644;
                dust.velocity.Y += num645;
                dust.scale = scale;
                dust.noGravity = true;
            }
        }
    }
}

