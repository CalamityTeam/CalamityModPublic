using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class DualpoonProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dualpoon");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.ranged = true;
            projectile.extraUpdates = 0;
            projectile.timeLeft = 300;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (player.dead)
            {
                projectile.Kill();
                return;
            }
            if (projectile.alpha == 0)
            {
                if (projectile.Center.X > player.Center.X)
                {
                    player.ChangeDir(1);
                }
                else
                {
                    player.ChangeDir(-1);
                }
            }
            if (projectile.ai[0] == 0f)
            {
                projectile.extraUpdates = 0;
            }
            else
            {
                projectile.extraUpdates = 1;
            }
            Vector2 playerVector = player.Center - projectile.Center;
            float playerDist = playerVector.Length();
            if (projectile.ai[0] == 0f)
            {
                if (playerDist > 700f)
                {
                    projectile.ai[0] = 1f;
                }
                else if (playerDist > 350f)
                {
                    projectile.ai[0] = 1f;
                }
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + MathHelper.PiOver2;
                projectile.ai[1] += 1f;
                if (projectile.ai[1] > 5f)
                {
                    projectile.alpha = 0;
                }
                if (projectile.ai[1] > 8f)
                {
                    projectile.ai[1] = 8f;
                }
                if (projectile.ai[1] >= 10f)
                {
                    projectile.ai[1] = 15f;
                    projectile.velocity.Y += 0.3f;
                }
            }
            else if (projectile.ai[0] == 1f)
            {
                projectile.tileCollide = false;
                projectile.rotation = (float)Math.Atan2((double)playerVector.Y, (double)playerVector.X) - MathHelper.PiOver2;
                float returnSpeed = 20f;
                if (playerDist < 50f)
                {
                    projectile.Kill();
                }
                playerDist = returnSpeed / playerDist;
                playerVector.X *= playerDist;
                playerVector.Y *= playerDist;
                projectile.velocity.X = playerVector.X;
                projectile.velocity.Y = playerVector.Y;
            }
        }
    }
}
