using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class SandBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1200;
            Projectile.Opacity = 0f;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

            Projectile.Opacity += 0.1f;
            if (Projectile.Opacity > 1f)
                Projectile.Opacity = 1f;

            int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 85, 0f, 0f, 100, default, 0.8f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int dust = 0; dust <= 3; dust++)
            {
                float rando1 = Main.rand.Next(-10, 11);
                float rando2 = Main.rand.Next(-10, 11);
                float rando3 = Main.rand.Next(3, 9);
                float randoAdjuster = (float)Math.Sqrt(rando1 * rando1 + rando2 * rando2);
                randoAdjuster = rando3 / randoAdjuster;
                rando1 *= randoAdjuster;
                rando2 *= randoAdjuster;
                int sandyDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 85, 0f, 0f, 100, default, 1.2f);
                Main.dust[sandyDust].noGravity = true;
                Main.dust[sandyDust].position.X = Projectile.Center.X;
                Main.dust[sandyDust].position.Y = Projectile.Center.Y;
                Dust expr_149DF_cp_0 = Main.dust[sandyDust];
                expr_149DF_cp_0.position.X += Main.rand.Next(-10, 11);
                Dust expr_14A09_cp_0 = Main.dust[sandyDust];
                expr_14A09_cp_0.position.Y += Main.rand.Next(-10, 11);
                Main.dust[sandyDust].velocity.X = rando1;
                Main.dust[sandyDust].velocity.Y = rando2;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = (byte)(255 * Projectile.Opacity);
            lightColor.G = (byte)(255 * Projectile.Opacity);
            lightColor.B = (byte)(255 * Projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
