using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class PrismRocket : ModProjectile
    {
        public float ExponentialAccelerationFactor => Projectile.Calamity().stealthStrike ? 1.027f : 1.015f;
        public float MaxHomingSpeed => Projectile.Calamity().stealthStrike ? 26f : 21f;
        public const int Lifetime = 150;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prism Rocket");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            NPC potentialTarget = Projectile.Center.ClosestNPCAt(800f, true);
            if (potentialTarget != null)
                AttackTarget(potentialTarget);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            EmitDust();
        }

        public void AttackTarget(NPC target)
        {
            float newSpeed = Projectile.velocity.Length() * ExponentialAccelerationFactor;
            if (newSpeed > MaxHomingSpeed)
                newSpeed = MaxHomingSpeed;

            if (!Projectile.WithinRange(target.Center, 30f))
            {
                Projectile.velocity = (Projectile.velocity * 5f + Projectile.SafeDirectionTo(target.Center) * newSpeed) / 6f;
                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(Projectile.AngleTo(target.Center), 0.15f).ToRotationVector2() * newSpeed;
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

                Dust dust = Dust.NewDustPerfect(Projectile.Center - (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 10f, 261);
                dust.color = Color.Cyan;
                dust.velocity = Main.rand.NextVector2Unit();
                dust.scale = Main.rand.NextFloat(0.75f, 1.05f);
                dust.noGravity = true;
            }
        }
    }
}
