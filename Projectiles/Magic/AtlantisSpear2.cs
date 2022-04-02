using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class AtlantisSpear2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/AtlantisSpear";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Atlantis Spear");
        }

        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 52;
            projectile.aiStyle = 4;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.magic = true;
            aiType = ProjectileID.CrystalVileShardShaft;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
            if (projectile.ai[0] == 0f)
            {
                projectile.alpha -= 50;
                if (projectile.alpha <= 0)
                {
                    projectile.alpha = 0;
                    projectile.ai[0] = 1f;
                    if (projectile.ai[1] == 0f)
                    {
                        projectile.ai[1] += 1f;
                        projectile.position += projectile.velocity * 1f;
                    }
                }
            }
            else
            {
                if (projectile.alpha < 170 && projectile.alpha + 5 >= 170)
                {
                    for (int num55 = 0; num55 < 8; num55++)
                    {
                        int num56 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 206, projectile.velocity.X * 0.005f, projectile.velocity.Y * 0.005f, 200, default, 1f);
                        Main.dust[num56].noGravity = true;
                        Main.dust[num56].velocity *= 0.5f;
                    }
                }
                projectile.alpha += 7;
                if (projectile.alpha >= 255)
                {
                    projectile.Kill();
                }
            }
            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 206, projectile.velocity.X * 0.005f, projectile.velocity.Y * 0.005f);
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, projectile.alpha);

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 206, projectile.oldVelocity.X * 0.005f, projectile.oldVelocity.Y * 0.005f);
            }
        }
    }
}
