using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class SHPExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 500;
            Projectile.height = 500;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            float lights = (float)Main.rand.Next(90, 111) * 0.01f;
            lights *= Main.essScale;
            Lighting.AddLight(Projectile.Center, 5f * lights, 1f * lights, 4f * lights);
            float projTimer = 25f;
            if (Projectile.ai[0] > 180f)
            {
                projTimer -= (Projectile.ai[0] - 180f) / 2f;
            }
            if (projTimer <= 0f)
            {
                projTimer = 0f;
                Projectile.Kill();
            }
            projTimer *= 0.7f;
            Projectile.ai[0] += 4f;
            int timerCounter = 0;
            while ((float)timerCounter < projTimer)
            {
                float rando1 = (float)Main.rand.Next(-40, 41);
                float rando2 = (float)Main.rand.Next(-40, 41);
                float rando3 = (float)Main.rand.Next(12, 36);
                float randoAdjust = (float)Math.Sqrt((double)(rando1 * rando1 + rando2 * rando2));
                randoAdjust = rando3 / randoAdjust;
                rando1 *= randoAdjust;
                rando2 *= randoAdjust;
                int randomDust = Main.rand.Next(3);
                if (randomDust == 0)
                {
                    randomDust = 246;
                }
                else if (randomDust == 1)
                {
                    randomDust = 73;
                }
                else
                {
                    randomDust = 187;
                }
                int EXPLODE = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDust, 0f, 0f, 100, default, 2f);
                Main.dust[EXPLODE].noGravity = true;
                Main.dust[EXPLODE].position.X = Projectile.Center.X;
                Main.dust[EXPLODE].position.Y = Projectile.Center.Y;
                Dust expr_149DF_cp_0 = Main.dust[EXPLODE];
                expr_149DF_cp_0.position.X += (float)Main.rand.Next(-10, 11);
                Dust expr_14A09_cp_0 = Main.dust[EXPLODE];
                expr_14A09_cp_0.position.Y += (float)Main.rand.Next(-10, 11);
                Main.dust[EXPLODE].velocity.X = rando1;
                Main.dust[EXPLODE].velocity.Y = rando2;
                timerCounter++;
            }
        }
    }
}
