using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class GhastlySubBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghast Blast");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 420;
            projectile.magic = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            bool flag60 = false;
            bool flag61 = false;
            int num3 = projectile.type;
            int num1024 = ModContent.ProjectileType<GhastlyBlast>();
            float num1025 = 420f;
            float x3 = 0.15f;
            float y3 = 0.15f;
            if (flag61)
            {
                int num1026 = (int)projectile.ai[1];
                if (!Main.projectile[num1026].active || Main.projectile[num1026].type != num1024)
                {
                    projectile.Kill();
                    return;
                }
                projectile.timeLeft = 2;
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] < num1025)
            {
                bool flag62 = true;
                int num1027 = (int)projectile.ai[1];
                if (Main.projectile[num1027].active && Main.projectile[num1027].type == num1024)
                {
                    if (!flag60 && Main.projectile[num1027].oldPos[1] != Vector2.Zero)
                    {
                        projectile.position += Main.projectile[num1027].position - Main.projectile[num1027].oldPos[1];
                    }
                    if (projectile.Center.HasNaNs())
                    {
                        projectile.Kill();
                        return;
                    }
                }
                else
                {
                    projectile.ai[0] = num1025;
                    flag62 = false;
                    projectile.Kill();
                }
                if (flag62 && !flag60)
                {
                    projectile.velocity += new Vector2((float)Math.Sign(Main.projectile[num1027].Center.X - projectile.Center.X), (float)Math.Sign(Main.projectile[num1027].Center.Y - projectile.Center.Y)) * new Vector2(x3, y3);
                    if (projectile.velocity.Length() > 6f)
                    {
                        projectile.velocity *= 6f / projectile.velocity.Length();
                    }
                }
                if (Main.rand.NextBool(2))
                {
                    int num1028 = Dust.NewDust(projectile.Center, 8, 8, 60, 0f, 0f, 0, default, 1f);
                    Main.dust[num1028].position = projectile.Center;
                    Main.dust[num1028].velocity = projectile.velocity;
                    Main.dust[num1028].noGravity = true;
                    Main.dust[num1028].scale = 1.5f;
                    if (flag62)
                    {
                        Main.dust[num1028].customData = Main.projectile[(int)projectile.ai[1]];
                    }
                }
                projectile.alpha = 255;
                return;
            }
        }

        public override void Kill(int timeLeft)
        {
            projectile.ai[0] = 60f;
            for (int num114 = 0; num114 < 10; num114++)
            {
                int num115 = Dust.NewDust(projectile.position, projectile.width, projectile.height, (int)projectile.ai[0], projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 0, default, 0.5f);
                Dust dust;
                Main.dust[num115].scale = 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
                Main.dust[num115].noGravity = true;
                dust = Main.dust[num115];
                dust.velocity *= 1.25f;
                dust = Main.dust[num115];
                dust.velocity -= projectile.oldVelocity / 10f;
            }
        }
    }
}
