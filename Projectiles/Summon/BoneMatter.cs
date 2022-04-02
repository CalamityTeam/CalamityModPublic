using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class BoneMatter : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bone Matter");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.tileCollide = true;
            projectile.penetrate = 1;
        }

        public override void AI()
        {
            if (projectile.frameCounter++ % 9 == 8)
            {
                projectile.frame++;
                if (projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.Kill();
                }
            }
        }
    }
}
