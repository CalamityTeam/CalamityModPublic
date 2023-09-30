using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class ChickenRocket : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 2;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            float speed = Projectile.velocity.Length();
            if (speed >= 12f)
            {
                // If the rocket is going fast enough, emit some dust.
                for (int i = 0; i < 2; i++)
                {
                    float dx = i == 1 ? Projectile.velocity.X * 0.5f : 0f;
                    float dy = i == 1 ? Projectile.velocity.Y * 0.5f : 0f;
                    int d = Dust.NewDust(new Vector2(Projectile.position.X + 3f + dx, Projectile.position.Y + 3f + dy) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 6, 0f, 0f, 100, default, 1f);
                    Main.dust[d].scale *= 2f + Main.rand.NextFloat();
                    Main.dust[d].velocity *= 0.2f;
                    Main.dust[d].noGravity = true;
                    d = Dust.NewDust(new Vector2(Projectile.position.X + 3f + dx, Projectile.position.Y + 3f + dy) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 244, 0f, 0f, 100, default, 0.5f);
                    Main.dust[d].fadeIn = 1f + Main.rand.NextFloat(0.5f);
                    Main.dust[d].velocity *= 0.05f;
                }

                // Exponentially accelerate if not going fast enough yet.
                if (speed < 18f)
                    Projectile.velocity *= 1.006f;

                // When going at very high speed, emit even more dust.
                else if (Main.rand.NextBool())
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 1f);
                    Main.dust[d].scale = 0.1f + Main.rand.NextFloat(0.5f);
                    Main.dust[d].fadeIn = 1.5f + Main.rand.NextFloat(0.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].position = Projectile.Center + new Vector2(0f, (float)(-(float)Projectile.height / 2)).RotatedBy(Projectile.rotation) * 1.1f;
                    Main.rand.Next(2);
                    d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 1f);
                    Main.dust[d].scale = 1f + Main.rand.NextFloat(0.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].position = Projectile.Center + new Vector2(0f, (float)(-(float)Projectile.height / 2 - 6)).RotatedBy(Projectile.rotation) * 1.1f;
                }
            }

            Projectile.ai[0] += 1f;
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

            // Slight gravity, equivalent to Plasma Grenade.
            Projectile.velocity.Y += 0.09f;
        }

        // Instead of dying instantly on collision, fly straight up for a moment.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.X = 0f;
            Projectile.velocity.Y = -15f;

            // If there isn't much time left anyway, just explode immediately on collision.
            if (Projectile.timeLeft > 20)
                Projectile.timeLeft = 20;
            else
                return true;

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 1040;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ChickenExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 40; i++)
            {
                int d = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 2f);
                Main.dust[d].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[d].scale = 0.5f;
                    Main.dust[d].fadeIn = 1f + Main.rand.NextFloat();
                }
            }
            for (int i = 0; i < 70; i++)
            {
                int d = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 5f;
                d = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[d].velocity *= 2f;
            }
        }
    }
}
