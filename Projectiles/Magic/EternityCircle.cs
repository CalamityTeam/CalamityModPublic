using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EternityCircle : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public int TargetNPCIndex
        {
            get => (int)projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public float SinusoidalPositionAngle
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        public float SinusoidalOffsetAngle
        {
            get => projectile.localAI[0];
            set => projectile.localAI[0] = value;
        }
        public int HeldBookIndex
        {
            get => (int)projectile.localAI[1];
            set => projectile.localAI[1] = value;
        }
        public const float TargetOffsetRadius = 490f;
        public const float SinusoidalOffsetAngleIncrement = 0.54f;
        public static readonly float SinusoidalPositionAngleIncrement = MathHelper.ToRadians(3.5f);
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eternity");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = EternityHex.Lifetime;
            projectile.alpha = 255;
            projectile.magic = true;
        }

        public override void AI()
        {
            NPC target = Main.npc[TargetNPCIndex];

            // Delete the circle if any necessary components are incorrect/would cause errors.
            if (HeldBookIndex >= Main.projectile.Length || SinusoidalOffsetAngle < 0)
            {
                projectile.Kill();
                return;
            }
            if (!Main.projectile[HeldBookIndex].active)
            {
                projectile.Kill();
                return;
            }

            // Delete the circle if the target cannot be damaged.
            if (!target.active || target.dontTakeDamage)
            {
                projectile.active = false;
            }

            // Assign an offset angle if it has a default value.
            if (SinusoidalOffsetAngle == 0f)
            {
                SinusoidalOffsetAngle = Main.rand.NextFloat(MathHelper.TwoPi);
            }

            // Circle around the enemy.
            projectile.position = target.Center + SinusoidalPositionAngle.ToRotationVector2() * TargetOffsetRadius;

            SinusoidalPositionAngle += SinusoidalPositionAngleIncrement;
            SinusoidalOffsetAngle += SinusoidalOffsetAngleIncrement;

            // Generate helical dust based on a time based sine.
            float pulse = (float)Math.Sin(SinusoidalOffsetAngle);
            float radius = 8f;
            Vector2 offset = Vector2.UnitY * pulse * radius;

            Dust dust = Dust.NewDustPerfect(projectile.Center + offset, 132, Vector2.Zero);
            dust.noGravity = true;

            dust = Dust.NewDustPerfect(projectile.Center - offset, 133, Vector2.Zero);
            dust.noGravity = true;
        }
    }
}
