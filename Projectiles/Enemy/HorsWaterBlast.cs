using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Enemy
{
    public class HorsWaterBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Enemy";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0f)
            {
                for (int i = 0; i < 20; i++)
                {
                    int waterDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 2f);
                    Main.dust[waterDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[waterDust].scale = 0.5f;
                        Main.dust[waterDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.35f / 255f);
            for (int j = 0; j < 10; j++)
            {
                int watery = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 1.2f);
                Main.dust[watery].noGravity = true;
                Main.dust[watery].velocity *= 0.5f;
                Main.dust[watery].velocity += Projectile.velocity * 0.1f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
            for (int dust = 0; dust <= 40; dust++)
            {
                float rando1 = (float)Main.rand.Next(-10, 11);
                float rando2 = (float)Main.rand.Next(-10, 11);
                float rando3 = (float)Main.rand.Next(3, 9);
                float randoAdjuster = (float)Math.Sqrt((double)(rando1 * rando1 + rando2 * rando2));
                randoAdjuster = rando3 / randoAdjuster;
                rando1 *= randoAdjuster;
                rando2 *= randoAdjuster;
                int killWaterDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 33, 0f, 0f, 100, default, 1.2f);
                Main.dust[killWaterDust].noGravity = true;
                Main.dust[killWaterDust].position.X = Projectile.Center.X;
                Main.dust[killWaterDust].position.Y = Projectile.Center.Y;
                Dust expr_149DF_cp_0 = Main.dust[killWaterDust];
                expr_149DF_cp_0.position.X += (float)Main.rand.Next(-10, 11);
                Dust expr_14A09_cp_0 = Main.dust[killWaterDust];
                expr_14A09_cp_0.position.Y += (float)Main.rand.Next(-10, 11);
                Main.dust[killWaterDust].velocity.X = rando1;
                Main.dust[killWaterDust].velocity.Y = rando2;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(BuffID.Wet, 300);
        }
    }
}
