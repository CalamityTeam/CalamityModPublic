using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class InfernalKrisExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static float radius = 64;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infernal Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.width = (int)radius * 2;
            Projectile.height = (int)radius * 2;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 9;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.timeLeft >= 5)
            {
                int numDust = 40;
                int dustType = 6;
                float minRange = 0;
                float maxRange = radius / 10f;

                for (int i = 0; i < numDust; i++)
                {

                    Vector2 circleVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    circleVelocity.Normalize();
                    circleVelocity *= Main.rand.NextFloat(minRange, maxRange);

                    int circle = Dust.NewDust(Projectile.Center, 1, 1, dustType, circleVelocity.X, circleVelocity.Y, 0, default, 2f);
                    Main.dust[circle].noGravity = true;
                    Main.dust[circle].velocity = circleVelocity;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
    }
}
