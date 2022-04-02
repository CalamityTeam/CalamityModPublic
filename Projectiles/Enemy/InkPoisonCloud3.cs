using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class InkPoisonCloud3 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ink Cloud");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 48;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 3600;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 9)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

            projectile.velocity *= 0.999f;

            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 120f)
            {
                if (projectile.alpha < 255)
                {
                    projectile.alpha += 5;
                    if (projectile.alpha > 255)
                        projectile.alpha = 255;
                }
                else if (projectile.owner == Main.myPlayer)
                    projectile.Kill();
            }
            else if (projectile.alpha > 80)
            {
                projectile.alpha -= 30;
                if (projectile.alpha < 80)
                    projectile.alpha = 80;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, 20f, targetHitbox);

        public override bool CanHitPlayer(Player target) => projectile.alpha == 80;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.alpha == 80)
                target.AddBuff(BuffID.Darkness, 300, true);
        }
    }
}
