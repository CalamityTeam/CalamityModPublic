using System;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class CoralSpike : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public ref float ChargeCompletion => ref Projectile.ai[0];

        public static int DustPick => Main.rand.NextBool() ? 255 : Main.rand.NextBool() ? 282 : Main.rand.NextBool() ? 281 : 280;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.96f;
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;

            if (Projectile.velocity.Length() < 2f)
                Projectile.Kill();

            int dustOpacity = (int)(200 * (1 - ChargeCompletion));
            float dustScale = Main.rand.NextFloat(1f, 1.4f) * (0.6f + 0.4f * ChargeCompletion);
            Vector2 dustVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.PiOver4 * 0.6f) * 4f + Main.rand.NextVector2Circular(4f, 4f);
            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustPick, dustVelocity, Alpha: dustOpacity, Scale : dustScale);
            dust.noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft != 0)
                SoundEngine.PlaySound(SoundID.Dig with { Volume = SoundID.Dig.Volume * 0.4f}, Projectile.Center);

            for (int i = 0; i < 3; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);

                Particle spike = new UrchinSpikeParticle(Projectile.Center + angle.ToRotationVector2() * 2f, angle.ToRotationVector2() * 6f, angle + MathHelper.PiOver2, Main.rand.NextFloat(1f, 1.3f), lifetime: Main.rand.Next(10) + 25);
                GeneralParticleHandler.SpawnParticle(spike);
            }

            int dustCount = Main.rand.Next(7);
            for (int i = 0; i < dustCount; i++)
            {
                int dustOpacity = (int)(200 * (1 - ChargeCompletion));
                float dustScale = Main.rand.NextFloat(1f, 1.4f) * (0.6f + 0.4f * ChargeCompletion);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustPick, Main.rand.NextVector2Circular(7f, 7f), Alpha: dustOpacity, Scale: dustScale);
                dust.noGravity = true;
            }
        }
    }
}
