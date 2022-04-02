using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class TotalMeltdown : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
            Main.projFrames[projectile.type] = 13;
        }

        public override void SetDefaults()
        {
            projectile.width = 120;
            projectile.height = 122;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.penetrate = -1;
            projectile.timeLeft = Main.projFrames[projectile.type] * 5;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            if (projectile.timeLeft % 5f == 4f)
                projectile.frame++;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}
