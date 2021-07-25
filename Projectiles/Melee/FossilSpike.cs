using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class FossilSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spike");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.melee = true;
			projectile.usesIDStaticNPCImmunity = true;
			projectile.idStaticNPCHitCooldown = 10;
		}

        public override void AI()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 32, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);

            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;
        }
    }
}
