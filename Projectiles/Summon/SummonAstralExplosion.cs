using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SummonAstralExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 44;
            projectile.height = 48;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = 2;
            projectile.timeLeft = Main.projFrames[projectile.type] * 5;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            // Bluish cyan light
            Lighting.AddLight(projectile.Center, 66f / 255f, 189f / 255f, 181f / 255f);
            if (projectile.timeLeft % 5f == 4f)
                projectile.frame++;
        }
    }
}
