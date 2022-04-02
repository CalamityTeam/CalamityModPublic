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
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public const int Lifetime = 360;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pulse Energy");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = Lifetime;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Time++;
            GenerateIdleDust();

            NPC potentialTarget = projectile.Center.ClosestNPCAt(920f);
            if (potentialTarget != null)
            {
                float oldSpeed = projectile.velocity.Length();
                float inertia = MathHelper.Lerp(15f, 4f, Utils.InverseLerp(Lifetime, Lifetime - 60f, projectile.timeLeft, true));
                projectile.velocity = (projectile.velocity * inertia + projectile.SafeDirectionTo(potentialTarget.Center, -Vector2.UnitY) * oldSpeed) / (inertia + 1f);
                projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitX) * oldSpeed;
            }
        }

        public void GenerateIdleDust()
        {
            if (!Main.dedServ)
            {
                // Generate a helical group of dust particles that pulsate with time.
                for (int i = 0; i < 3; i++)
                {
                    float angle = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    float pulse = (float)Math.Sin(Time / 110f + MathHelper.TwoPi / 3f * i);
                    Vector2 offset = angle.ToRotationVector2().RotatedBy(MathHelper.TwoPi / 3f * i) * pulse * 6f;

                    Dust dust = Dust.NewDustPerfect(projectile.Center + offset, 234);
                    dust.velocity = Vector2.Zero;
                    dust.noGravity = true;

                    dust = Dust.NewDustPerfect(projectile.Center + offset, 234);
                    dust.velocity = Vector2.Zero;
                    dust.noGravity = true;
                }
            }
        }
    }
}
