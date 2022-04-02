using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class HalleysComet : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Halley's Comet");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = 10;
            projectile.extraUpdates = 10;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (projectile.scale <= 1.5f)
            {
                projectile.scale *= 1.01f;
            }

            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.35f / 255f, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.45f / 255f);

            if (projectile.ai[0]++ > 5f)
            {
                float dustScaleSize = 1f;
                if (projectile.ai[0] == 6f)
                {
                    dustScaleSize = 0.25f;
                }
                else if (projectile.ai[0] == 7f)
                {
                    dustScaleSize = 0.5f;
                }
                else if (projectile.ai[0] == 8f)
                {
                    dustScaleSize = 0.75f;
                }
                projectile.ai[0] += 1f;
                int dustType = 176;
                for (int i = 0; i < 3; i++)
                {
                    int fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 1, default, 1f);
                    Dust dust = Main.dust[fire];
                    if (Main.rand.NextBool(3))
                    {
                        dust.noGravity = true;
                        dust.scale *= 1.75f;
                        dust.velocity.X *= 2f;
                        dust.velocity.Y *= 2f;
                    }
                    else
                    {
                        dust.scale *= 0.5f;
                    }
                    dust.velocity.X *= 1.2f;
                    dust.velocity.Y *= 1.2f;
                    dust.scale *= dustScaleSize;
                    dust.velocity += projectile.velocity;
                    if (!dust.noGravity)
                    {
                        dust.velocity *= 0.5f;
                    }
                }
            }

            projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 240);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 240);
        }
    }
}
