using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class RedirectingFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public ref float BurstIntensity => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Time >= 18f)
            {
                NPC potentialTarget = Projectile.Center.ClosestNPCAt(1100f, false);
                if (potentialTarget != null)
                    HomeInOnTarget(potentialTarget);

                float accelerationFactor = MathHelper.SmoothStep(1.025f, 1.015f, Utils.GetLerpValue(6f, 24f, Projectile.velocity.Length(), true));
                if (Projectile.velocity.Length() < 24f)
                    Projectile.velocity *= accelerationFactor;
            }
            EmitDust();
            Time++;
        }

        public void HomeInOnTarget(NPC target)
        {
            float oldSpeed = Projectile.velocity.Length();
            float delayFactor = Utils.GetLerpValue(20f, 35f, Time, true);
            float homeSpeed = MathHelper.Lerp(0f, 0.075f, delayFactor);

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(target.Center) * 16f, homeSpeed);
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * oldSpeed;
            Projectile.position += (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * MathHelper.Lerp(25f, 1f, Utils.GetLerpValue(100f, 360f, Projectile.Distance(target.Center), true)) * delayFactor;
        }

        public Dust CreateDustInstance()
        {
            int dustType = 264;

            Color brimstoneColor = Color.Lerp(Color.Red, Color.DarkViolet, (float)Math.Sin(Projectile.identity * 3f + Time / 12f) * 0.7f + 0.3f);
            Color dustColor = Color.Lerp(Color.Orange, Color.Yellow, Main.rand.NextFloat());
            dustColor = Color.Lerp(dustColor, brimstoneColor, (float)Math.Pow(BurstIntensity, 2D));

            Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType);
            dust.color = dustColor;
            dust.noGravity = true;
            dust.noLight = true;
            return dust;
        }

        public void EmitDust()
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 5; i++)
            {
                Dust dust = CreateDustInstance();
                dust.velocity = (MathHelper.TwoPi * i / 5f).ToRotationVector2();
                dust.position += Projectile.velocity;
                dust.scale = MathHelper.Lerp(1f, 1.5f, BurstIntensity) * Main.rand.NextFloat(0.7f, 1.3f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            // Emit some fire sounds.
            SoundEngine.PlaySound(SoundID.Item34, Projectile.Center);
            if (Main.dedServ)
                return;

            for (int i = 0; i < 15; i++)
            {
                Dust ectoplasm = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(30f, 30f) * BurstIntensity, 264);
                ectoplasm.velocity = Main.rand.NextVector2Circular(2f, 2f);
                ectoplasm.color = Color.Lerp(Color.Orange, Color.Yellow, Main.rand.NextFloat(0.67f));
                ectoplasm.scale = MathHelper.Lerp(1f, 1.6f, BurstIntensity);
                ectoplasm.noGravity = true;
                ectoplasm.noLight = true;
            }
        }
    }
}
