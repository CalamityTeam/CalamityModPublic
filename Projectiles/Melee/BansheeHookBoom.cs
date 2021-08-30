using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class BansheeHookBoom : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boom");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 6;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            projectile.ai[1] += 0.01f;
            projectile.scale = projectile.ai[1];
            projectile.ai[0] += 1f;
            projectile.alpha -= 63;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            Lighting.AddLight(projectile.Center, 1.5f, 0f, 0.15f);
            if (projectile.ai[0] == 1f)
            {
                projectile.position = projectile.Center;
                projectile.width = projectile.height = (int)(52f * projectile.scale);
                projectile.Center = projectile.position;
                projectile.Damage();
                for (int num1000 = 0; num1000 < 2; num1000++)
                {
                    int num1001 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                    Main.dust[num1001].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                }
                for (int num1002 = 0; num1002 < 5; num1002++)
                {
                    int num1003 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 200, default, 2.7f);
                    Main.dust[num1003].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                    Main.dust[num1003].noGravity = true;
                    Main.dust[num1003].velocity *= 3f;
                    num1003 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                    Main.dust[num1003].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                    Main.dust[num1003].velocity *= 2f;
                    Main.dust[num1003].noGravity = true;
                    Main.dust[num1003].fadeIn = 2.5f;
                }
                for (int num1004 = 0; num1004 < 2; num1004++)
                {
                    int num1005 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, 0f, 0f, 0, default, 2.7f);
                    Main.dust[num1005].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 2f;
                    Main.dust[num1005].noGravity = true;
                    Main.dust[num1005].velocity *= 3f;
                }
                for (int num1006 = 0; num1006 < 5; num1006++)
                {
                    int num1007 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 180, 0f, 0f, 0, default, 1.5f);
                    Main.dust[num1007].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 2f;
                    Main.dust[num1007].noGravity = true;
                    Main.dust[num1007].velocity *= 3f;
                }
            }
        }
    }
}
