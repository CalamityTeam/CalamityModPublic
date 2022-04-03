using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class Vortex : ModProjectile
    {
        public float TargetCheckCooldown
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public float Time
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        public int TargetIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public const float AngularMovementSpeed = 0.1f;
        public const float Acceleration = 0.0025f;
        public const float TargetCheckInterval = 30f;
        public const float MaximumTargetDistance = 600f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Subsuming Vortex");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = 115 * Projectile.extraUpdates;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // At the very beginning, start without a target.
            if (Time == 0f)
            {
                TargetIndex = -1;
            }
            Time++;
            if (Projectile.localAI[1] > 10f && Main.rand.NextBool(3))
            {
                VisualEffects();
            }
            Movement(HandleTargeting());
            Projectile.rotation += Projectile.velocity.X * 0.1f;
        }

        public bool HandleTargeting()
        {
            float targetCheckDistance = 350f * Projectile.ai[1];

            // Reduce a cooldown. Nothing special here.
            if (TargetCheckCooldown > 0f)
            {
                TargetCheckCooldown--;
            }

            // Attempt to find a target if the projectile has none.
            if (TargetIndex == -1 && TargetCheckCooldown <= 0f)
            {
                NPC potentialTarget = Projectile.Center.ClosestNPCAt(targetCheckDistance, true, true);
                if (potentialTarget != null)
                    TargetIndex = potentialTarget.whoAmI;
                Projectile.netUpdate = true;
            }
            if (TargetCheckCooldown <= 0f && TargetIndex == -1)
            {
                TargetCheckCooldown = TargetCheckInterval;
            }
            bool stillCanReachTarget = false;
            // Ensure that the target is still in reach if there is indeed a target.
            if (TargetIndex != -1)
            {
                stillCanReachTarget = Projectile.Distance(Main.npc[TargetIndex].Center) < MaximumTargetDistance;
                if (!stillCanReachTarget)
                {
                    TargetIndex = -1;
                    Projectile.netUpdate = true;
                }
            }
            return stillCanReachTarget;
        }

        public void VisualEffects()
        {
            // Generate idle circular dust and light after 10 frames.
            if (!Main.dedServ)
            {
                int dustCount = 5;
                for (int i = 0; i < dustCount; i++)
                {
                    Vector2 spawnPosition = Projectile.Center + Projectile.Size.RotatedBy(i / (float)dustCount * MathHelper.TwoPi) * 0.333f;
                    Vector2 velocity = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(6f, 16f);
                    Dust dust = Dust.NewDustPerfect(spawnPosition, 66, velocity, 0, Main.DiscoColor, 0.7f);
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.velocity = -Projectile.velocity;
                }
            }
            // Fade in.
            Projectile.alpha -= 5;
            if (Projectile.alpha < 50)
            {
                Projectile.alpha = 50;
            }
            // And make rainbow light.
            Lighting.AddLight(Projectile.Center / 16, Main.DiscoColor.ToVector3());
        }

        public void Movement(bool stillCanReachTarget)
        {
            // Homing sharply towards the target in a circular fashion.
            // The way this type of homing is by determining the offset between the angle to the target and the velocity's angle.
            // This offset can be thought of as how much, in radians, the velocity would need to rotate to move towards the target.
            // Then, the velocity is rotated by that offset (which, as established, would give perfect homing) multiplied by a <1 factor.
            // This causes the velocity to constantly rotate sharply towards the target.
            if (stillCanReachTarget)
            {
                float angleOffsetToTarget = Projectile.AngleTo(Main.npc[TargetIndex].Center) - Projectile.velocity.ToRotation();
                angleOffsetToTarget = MathHelper.WrapAngle(angleOffsetToTarget); // Ensure the offset is in the range of -pi to pi.
                Projectile.velocity = Projectile.velocity.RotatedBy(angleOffsetToTarget * AngularMovementSpeed);
            }
            // Accelerate constantly with time.
            float oldSpeed = Projectile.velocity.Length();
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * (oldSpeed + Acceleration);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.ExoDebuffs();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
