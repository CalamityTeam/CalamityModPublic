using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Patreon
{
    public class Orbacle : ModProjectile
    {
        private static int Lifetime = 40;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = Lifetime;

            projectile.alpha = 80;

            // Auric orbs never hit the same enemy more than once.
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            // Produces golden dust while in flight
            int dustType = (Main.rand.NextBool(3)) ? 244 : 246;
            float scale = 0.8f + Main.rand.NextFloat(0.6f);
            int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType);
            Main.dust[idx].noGravity = true;
            Main.dust[idx].velocity = projectile.velocity / 3f;
            Main.dust[idx].scale = scale;

            projectile.alpha += 4;
            projectile.velocity *= 0.88f;
        }
    }
}
