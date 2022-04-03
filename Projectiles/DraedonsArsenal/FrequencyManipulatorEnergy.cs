using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class FrequencyManipulatorEnergy : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public const int Lifetime = 360;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pulse Energy");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.Calamity().rogue = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = Lifetime;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Time++;
            GenerateIdleDust();

            NPC potentialTarget = Projectile.Center.ClosestNPCAt(920f);
            if (potentialTarget != null)
            {
                float oldSpeed = Projectile.velocity.Length();
                float inertia = MathHelper.Lerp(15f, 4f, Utils.InverseLerp(Lifetime, Lifetime - 60f, Projectile.timeLeft, true));
                Projectile.velocity = (Projectile.velocity * inertia + Projectile.SafeDirectionTo(potentialTarget.Center, -Vector2.UnitY) * oldSpeed) / (inertia + 1f);
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * oldSpeed;
            }
        }

        public void GenerateIdleDust()
        {
            if (!Main.dedServ)
            {
                // Generate a helical group of dust particles that pulsate with time.
                for (int i = 0; i < 3; i++)
                {
                    float angle = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    float pulse = (float)Math.Sin(Time / 110f + MathHelper.TwoPi / 3f * i);
                    Vector2 offset = angle.ToRotationVector2().RotatedBy(MathHelper.TwoPi / 3f * i) * pulse * 6f;

                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, 234);
                    dust.velocity = Vector2.Zero;
                    dust.noGravity = true;

                    dust = Dust.NewDustPerfect(Projectile.Center + offset, 234);
                    dust.velocity = Vector2.Zero;
                    dust.noGravity = true;
                }
            }
        }
    }
}
