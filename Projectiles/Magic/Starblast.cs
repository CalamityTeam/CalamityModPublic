using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class Starblast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
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
                Projectile.velocity.Y = Projectile.velocity.Y + 0.1f;
                Projectile.velocity.X = Projectile.velocity.X * 1.025f;
                Projectile.alpha -= 23;
                Projectile.scale = 0.8f * (255f - (float)Projectile.alpha) / 255f;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            if (Projectile.alpha >= 255 && Projectile.ai[0] > 5f)
            {
                Projectile.Kill();
            }
            if (Main.rand.NextBool(4))
            {
                int num193 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 180, 0f, 0f, 100, default, 1f);
                Main.dust[num193].position = Projectile.Center;
                Main.dust[num193].scale += (float)Main.rand.Next(50) * 0.01f;
                Main.dust[num193].noGravity = true;
                Dust expr_835F_cp_0 = Main.dust[num193];
                expr_835F_cp_0.velocity.Y -= 2f;
            }
            if (Main.rand.NextBool(6))
            {
                int num194 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 176, 0f, 0f, 100, default, 1f);
                Main.dust[num194].position = Projectile.Center;
                Main.dust[num194].scale += 0.3f + (float)Main.rand.Next(50) * 0.01f;
                Main.dust[num194].noGravity = true;
                Main.dust[num194].velocity *= 0.1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 176, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 180, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
