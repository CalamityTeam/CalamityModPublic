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
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 6;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.ai[1] += 0.01f;
            Projectile.scale = Projectile.ai[1];
            Projectile.ai[0] += 1f;
            Projectile.alpha -= 63;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            Lighting.AddLight(Projectile.Center, 1.5f, 0f, 0.15f);
            if (Projectile.ai[0] == 1f)
            {
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = (int)(52f * Projectile.scale);
                Projectile.Center = Projectile.position;
                Projectile.Damage();
                for (int num1000 = 0; num1000 < 2; num1000++)
                {
                    int num1001 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                    Main.dust[num1001].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                }
                for (int num1002 = 0; num1002 < 5; num1002++)
                {
                    int num1003 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 200, default, 2.7f);
                    Main.dust[num1003].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                    Main.dust[num1003].noGravity = true;
                    Main.dust[num1003].velocity *= 3f;
                    num1003 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                    Main.dust[num1003].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                    Main.dust[num1003].velocity *= 2f;
                    Main.dust[num1003].noGravity = true;
                    Main.dust[num1003].fadeIn = 2.5f;
                }
                for (int num1004 = 0; num1004 < 2; num1004++)
                {
                    int num1005 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 0, default, 2.7f);
                    Main.dust[num1005].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 2f;
                    Main.dust[num1005].noGravity = true;
                    Main.dust[num1005].velocity *= 3f;
                }
                for (int num1006 = 0; num1006 < 5; num1006++)
                {
                    int num1007 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 180, 0f, 0f, 0, default, 1.5f);
                    Main.dust[num1007].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 2f;
                    Main.dust[num1007].noGravity = true;
                    Main.dust[num1007].velocity *= 3f;
                }
            }
        }
    }
}
