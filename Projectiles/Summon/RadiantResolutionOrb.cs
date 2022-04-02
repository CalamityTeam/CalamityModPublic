using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class RadiantResolutionOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sun");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 62;
            projectile.height = 64;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.alpha = 255;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 4;
        }

        public override void AI()
        {
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 9;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }
            Player player = Main.player[projectile.owner];
            projectile.frameCounter++;
            if (projectile.frameCounter > 8)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            if (projectile.ai[0]++ < 30f)
            {
                projectile.velocity *= 1.01f;
            }
            else
            {
                NPC potentialTarget = projectile.Center.MinionHoming(2000f, player);
                if (potentialTarget != null)
                    projectile.velocity = (projectile.velocity * 7f + projectile.SafeDirectionTo(potentialTarget.Center) * 19f) / 8f;
            }
        }
    }
}
