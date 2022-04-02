using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class PrismRocket : ModProjectile
    {
        public float ExponentialAccelerationFactor => projectile.Calamity().stealthStrike ? 1.027f : 1.015f;
        public float MaxHomingSpeed => projectile.Calamity().stealthStrike ? 26f : 21f;
        public const int Lifetime = 150;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prism Rocket");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = Lifetime;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 9;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            NPC potentialTarget = projectile.Center.ClosestNPCAt(800f, true);
            if (potentialTarget != null)
                AttackTarget(potentialTarget);

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            EmitDust();
        }

        public void AttackTarget(NPC target)
        {
            float newSpeed = projectile.velocity.Length() * ExponentialAccelerationFactor;
            if (newSpeed > MaxHomingSpeed)
                newSpeed = MaxHomingSpeed;

            if (!projectile.WithinRange(target.Center, 30f))
            {
                projectile.velocity = (projectile.velocity * 5f + projectile.SafeDirectionTo(target.Center) * newSpeed) / 6f;
                projectile.velocity = projectile.velocity.ToRotation().AngleTowards(projectile.AngleTo(target.Center), 0.15f).ToRotationVector2() * newSpeed;
            }
        }

        public void EmitDust()
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 2; i++)
            {
                if (!Main.rand.NextBool(3))
                    continue;

                Dust dust = Dust.NewDustPerfect(projectile.Center - (projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 10f, 261);
                dust.color = Color.Cyan;
                dust.velocity = Main.rand.NextVector2Unit();
                dust.scale = Main.rand.NextFloat(0.75f, 1.05f);
                dust.noGravity = true;
            }
        }
    }
}
