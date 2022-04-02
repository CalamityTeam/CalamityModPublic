using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneFire : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Fire");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 90;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.3f, 0f, 0f);
            if (projectile.ai[0] > 7f)
            {
                float scalar = 1f;
                if (projectile.ai[0] == 8f)
                {
                    scalar = 0.25f;
                }
                else if (projectile.ai[0] == 9f)
                {
                    scalar = 0.5f;
                }
                else if (projectile.ai[0] == 10f)
                {
                    scalar = 0.75f;
                }
                projectile.ai[0] += 1f;
                int dustType = (int)CalamityDusts.Brimstone;
                if (Main.rand.NextBool(2))
                {
                    int fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 100, default, 1f);
                    Dust dust = Main.dust[fire];
                    if (Main.rand.NextBool(3))
                    {
                        dust.noGravity = true;
                        dust.scale *= 3f;
                        dust.velocity.X *= 1.5f;
                        dust.velocity.Y *= 1.5f;
                    }
                    else
                    {
                        dust.scale *= 1.5f;
                    }
                    dust.velocity.X *= 1.2f;
                    dust.velocity.Y *= 1.2f;
                    dust.scale *= scalar;
                }
            }
            else
            {
                projectile.ai[0] += 1f;
            }
            projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }
    }
}
