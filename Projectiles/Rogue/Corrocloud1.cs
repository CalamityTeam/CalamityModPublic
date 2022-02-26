using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class Corrocloud1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corrocloud");
            Main.projFrames[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = 4;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 16;
        }

        public override void AI()
        {
            projectile.velocity *= 0.99f;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] % 7f == 6f)
            {
                projectile.frame++;
            }
            if (projectile.frame >= 8)
                projectile.Kill();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid);
            }
        }
    }
}
