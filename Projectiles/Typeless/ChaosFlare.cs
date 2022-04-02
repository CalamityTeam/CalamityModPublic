using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class ChaosFlare : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flare");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 105 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.5f, 0.25f, 0f);

            if (projectile.localAI[0] == 0f)
            {
                Main.PlaySound(SoundID.Item20, projectile.position);
                projectile.localAI[0] += 1f;
            }

            for (int i = 0; i < 6; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 127, 0f, 0f, 100, default, 1.2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.5f;
                Main.dust[d].velocity += projectile.velocity * 0.1f;
            }

            if (projectile.timeLeft < 105)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 450f, 12f, 20f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
        }
    }
}
