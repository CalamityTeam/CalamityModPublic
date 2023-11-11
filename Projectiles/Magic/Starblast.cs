using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class Starblast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 5f)
            {
                Projectile.alpha -= 25;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;

                    if (Main.rand.NextBool(4))
                    {
                        int stardust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 180, 0f, 0f, 100, default, 1f);
                        Main.dust[stardust].position = Projectile.Center;
                        Main.dust[stardust].scale += (float)Main.rand.Next(50) * 0.01f;
                        Main.dust[stardust].noGravity = true;
                        Dust expr_835F_cp_0 = Main.dust[stardust];
                        expr_835F_cp_0.velocity.Y -= 2f;
                    }

                    if (Main.rand.NextBool(6))
                    {
                        int stardustier = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 176, 0f, 0f, 100, default, 1f);
                        Main.dust[stardustier].position = Projectile.Center;
                        Main.dust[stardustier].scale += 0.3f + (float)Main.rand.Next(50) * 0.01f;
                        Main.dust[stardustier].noGravity = true;
                        Main.dust[stardustier].velocity *= 0.1f;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 176, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 180, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
