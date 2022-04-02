using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class RedirectingFire : ModProjectile
    {
        public ref float BurstIntensity => ref projectile.ai[0];
        public ref float Time => ref projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetStaticDefaults() => DisplayName.SetDefault("Fire");

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 18;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Time >= 18f)
            {
                NPC potentialTarget = projectile.Center.ClosestNPCAt(1100f, false);
                if (potentialTarget != null)
                    HomeInOnTarget(potentialTarget);

                float accelerationFactor = MathHelper.SmoothStep(1.025f, 1.015f, Utils.InverseLerp(6f, 24f, projectile.velocity.Length(), true));
                if (projectile.velocity.Length() < 24f)
                    projectile.velocity *= accelerationFactor;
            }
            EmitDust();
            Time++;
        }

        public void HomeInOnTarget(NPC target)
        {
            float oldSpeed = projectile.velocity.Length();
            float delayFactor = Utils.InverseLerp(20f, 35f, Time, true);
            float homeSpeed = MathHelper.Lerp(0f, 0.075f, delayFactor);

            projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.SafeDirectionTo(target.Center) * 16f, homeSpeed);
            projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitY) * oldSpeed;
            projectile.position += (target.Center - projectile.Center).SafeNormalize(Vector2.Zero) * MathHelper.Lerp(25f, 1f, Utils.InverseLerp(100f, 360f, projectile.Distance(target.Center), true)) * delayFactor;
        }

        public Dust CreateDustInstance()
        {
            int dustType = 264;

            Color brimstoneColor = Color.Lerp(Color.Red, Color.DarkViolet, (float)Math.Sin(projectile.identity * 3f + Time / 12f) * 0.7f + 0.3f);
            Color dustColor = Color.Lerp(Color.Orange, Color.Yellow, Main.rand.NextFloat());
            dustColor = Color.Lerp(dustColor, brimstoneColor, (float)Math.Pow(BurstIntensity, 2D));

            Dust dust = Dust.NewDustPerfect(projectile.Center, dustType);
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
                dust.position += projectile.velocity;
                dust.scale = MathHelper.Lerp(1f, 1.5f, BurstIntensity) * Main.rand.NextFloat(0.7f, 1.3f);
            }
        }

        public override void Kill(int timeLeft)
        {
            // Emit some fire sounds.
            Main.PlaySound(SoundID.Item34, projectile.Center);
            if (Main.dedServ)
                return;

            for (int i = 0; i < 15; i++)
            {
                Dust ectoplasm = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(30f, 30f) * BurstIntensity, 264);
                ectoplasm.velocity = Main.rand.NextVector2Circular(2f, 2f);
                ectoplasm.color = Color.Lerp(Color.Orange, Color.Yellow, Main.rand.NextFloat(0.67f));
                ectoplasm.scale = MathHelper.Lerp(1f, 1.6f, BurstIntensity);
                ectoplasm.noGravity = true;
                ectoplasm.noLight = true;
            }
        }
    }
}
