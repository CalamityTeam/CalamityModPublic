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
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 90;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.3f, 0f, 0f);
            if (Projectile.ai[0] > 7f)
            {
                float scalar = 1f;
                if (Projectile.ai[0] == 8f)
                {
                    scalar = 0.25f;
                }
                else if (Projectile.ai[0] == 9f)
                {
                    scalar = 0.5f;
                }
                else if (Projectile.ai[0] == 10f)
                {
                    scalar = 0.75f;
                }
                Projectile.ai[0] += 1f;
                int dustType = (int)CalamityDusts.Brimstone;
                if (Main.rand.NextBool(2))
                {
                    int fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 100, default, 1f);
                    Dust dust = Main.dust[fire];
                    if (Main.rand.NextBool(3))
                    {
                        dust.noGravity = true;
                        dust.scale *= 3f;
                    }
                    else
                    {
                        dust.scale *= 1.5f;
                    }
                    dust.scale *= scalar;
                }
            }
            else
            {
                Projectile.ai[0] += 1f;
            }
            Projectile.rotation += 0.3f * (float)Projectile.direction;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
        }
    }
}
