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
            projectile.width = (int)radius * 2;
            projectile.height = (int)radius * 2;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 9;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.timeLeft >= 5)
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

                    int circle = Dust.NewDust(projectile.Center, 1, 1, dustType, circleVelocity.X, circleVelocity.Y, 0, default, 2f);
                    Main.dust[circle].noGravity = true;
                    Main.dust[circle].velocity = circleVelocity;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float dist1 = Vector2.Distance(projectile.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(projectile.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(projectile.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(projectile.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= radius;
        }
    }
}
