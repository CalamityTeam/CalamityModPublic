using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class Nanotech : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nanoblade");
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
        }

        public override bool? CanHitNPC(NPC target) => projectile.ai[1] >= 30f && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, new Vector3(0.075f, 0.4f, 0.15f));

            projectile.rotation += projectile.velocity.X * 0.2f;
            if (projectile.velocity.X > 0f)
                projectile.rotation += 0.08f;
            else
                projectile.rotation -= 0.08f;

            projectile.ai[1] += 1f;
            if (projectile.ai[1] > 60f)
            {
                projectile.alpha += 5;
                if (projectile.alpha >= 255)
                {
                    projectile.alpha = 255;
                    projectile.Kill();
                    return;
                }
            }

            if (projectile.ai[1] >= 30f)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 200f, 12f, 20f);
        }

        // Reduce damage of projectiles if more than the cap are active
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            int projectileCount = Main.player[projectile.owner].ownedProjectileCounts[projectile.type];
            int cap = 5;
            int oldDamage = damage;
            if (projectileCount > cap)
            {
                damage -= (int)(oldDamage * ((projectileCount - cap) * 0.05));
                if (damage < 1)
                    damage = 1;
            }
        }

        public override void Kill(int timeLeft)
        {
            int num3;
            for (int num191 = 0; num191 < 2; num191 = num3 + 1)
            {
                int num192 = (int)(10f * projectile.scale);
                int num193 = Dust.NewDust(projectile.Center - Vector2.One * (float)num192, num192 * 2, num192 * 2, 107, 0f, 0f, 0, default, 1f);
                Dust dust20 = Main.dust[num193];
                Vector2 value8 = Vector2.Normalize(dust20.position - projectile.Center);
                dust20.position = projectile.Center + value8 * (float)num192 * projectile.scale;
                if (num191 < 30)
                {
                    dust20.velocity = value8 * dust20.velocity.Length();
                }
                else
                {
                    dust20.velocity = value8 * (float)Main.rand.Next(45, 91) / 10f;
                }
                dust20.color = Main.hslToRgb((float)(0.40000000596046448 + Main.rand.NextDouble() * 0.20000000298023224), 0.9f, 0.5f);
                dust20.color = Color.Lerp(dust20.color, Color.White, 0.3f);
                dust20.noGravity = true;
                dust20.scale = 0.7f;
                num3 = num191;
            }
        }
    }
}
