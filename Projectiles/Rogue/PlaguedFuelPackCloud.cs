using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class PlaguedFuelPackCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plagued Cloud");
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 100;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 50)
                Projectile.alpha += 5;
            if (Projectile.timeLeft < 75)
                Projectile.velocity *= 0.95f;

            if (Main.rand.NextBool(150))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 89, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 100, default, 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity.Y -= 0.15f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
        }

        public override void Kill(int timeLeft)
        {
            if (timeLeft != 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 89, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.15f;
                }
            }
        }
    }
}
