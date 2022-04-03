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
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
            Projectile.extraUpdates = 1;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 170 && target.CanBeChasedBy(Projectile);

        // Reduce damage of projectiles if more than the cap are active
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            int projectileCount = Main.player[Projectile.owner].ownedProjectileCounts[Projectile.type];
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
                float x2 = Projectile.position.X - Projectile.velocity.X / 10f * (float)num136;
                float y2 = Projectile.position.Y - Projectile.velocity.Y / 10f * (float)num136;
                Vector2 dspeed = Projectile.velocity * Main.rand.NextFloat(0.7f, 0.4f);
                int num137 = Dust.NewDust(new Vector2(x2, y2), 1, 1, 107, 0f, 0f, 0, default, 1f);
                Main.dust[num137].alpha = Projectile.alpha;
                Main.dust[num137].position.X = x2;
                Main.dust[num137].position.Y = y2;
                Main.dust[num137].velocity = dspeed;
                Main.dust[num137].noGravity = true;
                Main.dust[num137].noLight = true;
            }

            if (Projectile.timeLeft < 170)
                CalamityGlobalProjectile.HomeInOnNPC(Projectile, true, 600f, 9f, 20f);
        }
    }
}
