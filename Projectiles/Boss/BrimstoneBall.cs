using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Fireball");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            projectile.rotation += 0.12f * projectile.direction;

            Lighting.AddLight(projectile.Center, 0.25f, 0f, 0f);

            for (int num468 = 0; num468 < 2; num468++)
            {
                Vector2 dspeed = -projectile.velocity * 0.7f;
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 150, default, 1.1f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity = dspeed;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
        }
    }
}
