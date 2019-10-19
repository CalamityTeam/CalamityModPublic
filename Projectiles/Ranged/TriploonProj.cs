using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class TriploonProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Triploon");
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
        }

        public override void AI()
        {
            if (Main.player[projectile.owner].dead)
            {
                projectile.Kill();
                return;
            }
            if (projectile.alpha == 0)
            {
                if (projectile.position.X + (float)(projectile.width / 2) > Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2))
                {
                    Main.player[projectile.owner].ChangeDir(1);
                }
                else
                {
                    Main.player[projectile.owner].ChangeDir(-1);
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
            Vector2 vector14 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
            float num166 = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector14.X;
            float num167 = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - vector14.Y;
            float num168 = (float)Math.Sqrt((double)(num166 * num166 + num167 * num167));
            if (projectile.ai[0] == 0f)
            {
                if (num168 > 700f)
                {
                    projectile.ai[0] = 1f;
                }
                else if (num168 > 350f)
                {
                    projectile.ai[0] = 1f;
                }
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
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
                    projectile.velocity.Y = projectile.velocity.Y + 0.3f;
                }
            }
            else if (projectile.ai[0] == 1f)
            {
                projectile.tileCollide = false;
                projectile.rotation = (float)Math.Atan2((double)num167, (double)num166) - 1.57f;
                float num169 = 20f;
                if (num168 < 50f)
                {
                    projectile.Kill();
                }
                num168 = num169 / num168;
                num166 *= num168;
                num167 *= num168;
                projectile.velocity.X = num166;
                projectile.velocity.Y = num167;
            }
        }
    }
}
