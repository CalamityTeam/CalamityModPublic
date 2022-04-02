using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class TarraEnergy : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tarra Energy");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 200;
            projectile.extraUpdates = 1;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 170 && target.CanBeChasedBy(projectile);

        // Reduce damage of projectiles if more than the cap are active
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            int projectileCount = Main.player[projectile.owner].ownedProjectileCounts[projectile.type];
            int cap = 2;
            int oldDamage = damage;
            if (projectileCount > cap)
            {
                damage -= (int)(oldDamage * ((projectileCount - cap) * 0.05));
                if (damage < 1)
                    damage = 1;
            }
        }

        public override void AI()
        {
            for (int num136 = 0; num136 < 2; num136++)
            {
                float x2 = projectile.position.X - projectile.velocity.X / 10f * (float)num136;
                float y2 = projectile.position.Y - projectile.velocity.Y / 10f * (float)num136;
                Vector2 dspeed = projectile.velocity * Main.rand.NextFloat(0.7f, 0.4f);
                int num137 = Dust.NewDust(new Vector2(x2, y2), 1, 1, 107, 0f, 0f, 0, default, 1f);
                Main.dust[num137].alpha = projectile.alpha;
                Main.dust[num137].position.X = x2;
                Main.dust[num137].position.Y = y2;
                Main.dust[num137].velocity = dspeed;
                Main.dust[num137].noGravity = true;
                Main.dust[num137].noLight = true;
            }

            if (projectile.timeLeft < 170)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 600f, 9f, 20f);
        }
    }
}
