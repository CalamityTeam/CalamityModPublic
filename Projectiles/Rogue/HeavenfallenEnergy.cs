using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class HeavenfallenEnergy : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public bool raining => Projectile.ai[1] == 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heavenfallen Energy");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            int num154 = 14;
            int coolDust;
            Projectile.ai[0]++;
            if (Projectile.ai[0] % 2 == 0)
            {
                if (Projectile.ai[0] % 4 == 0)
                {
                    coolDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width - num154 * 2, Projectile.height - num154 * 2, ModContent.DustType<AstralBlue>(), 0f, 0f, 100, default, 1.5f);
                }
                else
                {
                    coolDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width - num154 * 2, Projectile.height - num154 * 2, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                }
                Main.dust[coolDust].noGravity = true;
                Main.dust[coolDust].velocity *= 0.1f;
                Main.dust[coolDust].velocity += Projectile.velocity * 0.5f;
            }

            float homingRange = raining ? 480f : 240f;

            if (Projectile.ai[0] >= 15f || raining)
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, homingRange, 12f, 20f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        
        public override void OnHitPvp(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        
        public override bool? CanDamage() => Projectile.ai[0] >= 15f || raining;
    }
}
