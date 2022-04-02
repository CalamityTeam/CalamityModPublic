using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class ApparatusExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            Main.projFrames[projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 44;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            // Bluish cyan light
            Lighting.AddLight(projectile.Center, 0.1f, 0.5f, 1f);

            projectile.frameCounter++;
            if (projectile.frameCounter >= 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.Kill();
            }
        }
    }
}
