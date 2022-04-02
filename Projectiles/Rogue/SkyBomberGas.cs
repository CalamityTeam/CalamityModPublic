using Terraria;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.Rogue
{
    public class SkyBomberGas : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Irradiated Cloud");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.alpha = 0;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 100;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.timeLeft < 50)
                projectile.alpha += 5;
            if (projectile.timeLeft < 75)
            {
                projectile.velocity *= 0.95f;
                projectile.scale += 0.002f;
            }
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
