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
            projectile.width = 28;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.alpha = 0;
            projectile.penetrate = -1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 100;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.timeLeft < 50)
                projectile.alpha += 5;
            if (projectile.timeLeft < 75)
                projectile.velocity *= 0.95f;

            if (Main.rand.NextBool(150))
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 89, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f, 100, default, 2f);
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
                    int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 89, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.15f;
                }
            }
        }
    }
}
