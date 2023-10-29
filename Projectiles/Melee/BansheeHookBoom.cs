using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class BansheeHookBoom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

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
                for (int i = 0; i < 2; i++)
                {
                    int bansheeDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                    Main.dust[bansheeDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                }
                for (int j = 0; j < 5; j++)
                {
                    int bansheeDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 200, default, 2.7f);
                    Main.dust[bansheeDust2].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                    Main.dust[bansheeDust2].noGravity = true;
                    Main.dust[bansheeDust2].velocity *= 3f;
                    bansheeDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                    Main.dust[bansheeDust2].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                    Main.dust[bansheeDust2].velocity *= 2f;
                    Main.dust[bansheeDust2].noGravity = true;
                    Main.dust[bansheeDust2].fadeIn = 2.5f;
                }
                for (int k = 0; k < 2; k++)
                {
                    int bansheeDust3 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 0, default, 2.7f);
                    Main.dust[bansheeDust3].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 2f;
                    Main.dust[bansheeDust3].noGravity = true;
                    Main.dust[bansheeDust3].velocity *= 3f;
                }
                for (int l = 0; l < 5; l++)
                {
                    int spiritDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 180, 0f, 0f, 0, default, 1.5f);
                    Main.dust[spiritDust].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 2f;
                    Main.dust[spiritDust].noGravity = true;
                    Main.dust[spiritDust].velocity *= 3f;
                }
            }
        }
    }
}
