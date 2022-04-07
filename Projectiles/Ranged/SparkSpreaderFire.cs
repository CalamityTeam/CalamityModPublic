using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SparkSpreaderFire : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.05f / 255f, (255 - Projectile.alpha) * 0.45f / 255f, (255 - Projectile.alpha) * 0.05f / 255f);

            if (Projectile.wet && !Projectile.lavaWet)
            {
                Projectile.Kill();
            }

            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 35, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 80, default, 0.75f);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 55, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 50, default, 0.75f);
            }

            if (Projectile.ai[0]++ > 7f)
            {
                float dustScaleSize = 1f;
                if (Projectile.ai[0] == 8f)
                {
                    dustScaleSize = 0.25f;
                }
                else if (Projectile.ai[0] == 9f)
                {
                    dustScaleSize = 0.5f;
                }
                else if (Projectile.ai[0] == 10f)
                {
                    dustScaleSize = 0.75f;
                }
                Projectile.ai[0] += 1f;
                int dustType = 6;
                for (int i = 0; i < 2; i++)
                {
                    int fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 10, default, 0.75f);
                    Dust dust = Main.dust[fire];
                    if (Main.rand.Next(3) == 0)
                    {
                        dust.noGravity = true;
                        dust.scale *= 1.75f;
                        dust.velocity.X *= 2f;
                        dust.velocity.Y *= 2f;
                    }
                    else
                    {
                        dust.noGravity = true;
                        dust.scale *= 0.5f;
                    }
                    dust.velocity.X *= 1.2f;
                    dust.velocity.Y *= 1.2f;
                    dust.scale *= dustScaleSize;
                    dust.velocity += Projectile.velocity;
                }
            }

            Projectile.rotation += 0.3f * (float)Projectile.direction;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(1, 4));
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(1, 4));
        }
    }
}
