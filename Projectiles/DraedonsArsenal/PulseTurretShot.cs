using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PulseTurretShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public const int SpiralPrecision = 36;
        public const int SpiralRings = 6;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pulse Bolt");
            ProjectileID.Sets.SentryShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.ai[0] += MathHelper.ToRadians(6f);
            if (!Main.dedServ)
            {
                for (int i = 0; i < 3; i++)
                {
                    float angle = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    float pulse = (float)Math.Sin(projectile.ai[0] + MathHelper.TwoPi / 3f * i);
                    Vector2 offset = angle.ToRotationVector2().RotatedBy(MathHelper.TwoPi / 3f * i) * pulse * 7f;
                    Dust.NewDustPerfect(projectile.Center + offset, 234, Vector2.Zero).noGravity = true;
                    Dust.NewDustPerfect(projectile.Center - offset, 234, Vector2.Zero).noGravity = true;
                }
            }
            if (projectile.ai[1] == 1f)
            {
                NPC potentialTarget = projectile.Center.MinionHoming(850f, Main.player[projectile.owner], false);
                if (potentialTarget != null)
                {
                    float speed = projectile.velocity.Length();
                    float inertia = MathHelper.Lerp(18f, 6f, 1f - projectile.timeLeft / 300f);
                    projectile.velocity = (projectile.velocity * inertia + projectile.SafeDirectionTo(potentialTarget.Center) * speed) / (inertia + 1f);
                    projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitX) * speed;
                }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!Main.dedServ)
            {
                // Spirals
                for (int i = 0; i < SpiralPrecision; i++)
                {
                    for (int direction = -1; direction <= 1; direction += 2)
                    {
                        for (int j = 0; j < SpiralRings; j++)
                        {
                            Dust dust = Dust.NewDustPerfect(projectile.Center +
                                Vector2.UnitY.RotatedBy(j / (float)SpiralRings * MathHelper.TwoPi * direction).RotatedBy(i / (float)SpiralPrecision * MathHelper.TwoPi / SpiralRings * direction) *
                                36f * i / SpiralPrecision, 173);
                            dust.velocity = projectile.SafeDirectionTo(dust.position) * 2.4f;
                            dust.scale = 1.6f;
                            dust.noGravity = true;

                            dust = Dust.NewDustPerfect(projectile.Center +
                                Vector2.UnitY.RotatedBy(j / (float)SpiralRings * MathHelper.TwoPi * direction).RotatedBy(i / (float)SpiralPrecision * MathHelper.TwoPi / SpiralRings * direction) *
                                36f * i / SpiralPrecision, 173);
                            dust.velocity = projectile.DirectionFrom(dust.position) * 4f;
                            dust.scale = 1.6f;
                            dust.noGravity = true;
                        }
                    }
                }
            }
        }
    }
}
