using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class DuststormCloudStealth : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/DuststormCloud";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cloud");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 3600;
            projectile.Calamity().rogue = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 4;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            projectile.velocity *= 0.998f;
            projectile.ai[1] += 1f;
            if (projectile.ai[1] >= 120f)
            {
                if (projectile.alpha < 255)
                {
                    projectile.alpha += 3;
                    if (projectile.alpha > 255)
                    {
                        projectile.alpha = 255;
                    }
                }
                else if (projectile.owner == Main.myPlayer)
                {
                    projectile.Kill();
                }
            }
            else if (projectile.alpha > 80)
            {
                projectile.alpha -= 30;
                if (projectile.alpha < 80)
                {
                    projectile.alpha = 80;
                }
            }
        }
    }
}
