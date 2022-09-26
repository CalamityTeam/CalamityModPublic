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
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.ai[1] >= 30f && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.075f, 0.4f, 0.15f));

            Projectile.rotation += Projectile.velocity.X * 0.2f;
            if (Projectile.velocity.X > 0f)
                Projectile.rotation += 0.08f;
            else
                Projectile.rotation -= 0.08f;

            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] > 60f)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha >= 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                    return;
                }
            }

            if (Projectile.ai[1] >= 30f)
                CalamityUtils.HomeInOnNPC(Projectile, true, 200f, 12f, 20f);
        }

        // Reduce damage of projectiles if more than the cap are active
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            int projectileCount = Main.player[Projectile.owner].ownedProjectileCounts[Projectile.type];
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
                int num192 = (int)(10f * Projectile.scale);
                int num193 = Dust.NewDust(Projectile.Center - Vector2.One * (float)num192, num192 * 2, num192 * 2, 107, 0f, 0f, 0, default, 1f);
                Dust dust20 = Main.dust[num193];
                Vector2 value8 = Vector2.Normalize(dust20.position - Projectile.Center);
                dust20.position = Projectile.Center + value8 * (float)num192 * Projectile.scale;
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
