using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class EmpyreanBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 5;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.35f / 255f, (255 - Projectile.alpha) * 0.05f / 255f, (255 - Projectile.alpha) * 0.25f / 255f);
            bool xflag = false;
            bool yflag = false;
            if (Projectile.velocity.X < 0f && Projectile.position.X < Projectile.ai[0])
            {
                xflag = true;
            }
            if (Projectile.velocity.X > 0f && Projectile.position.X > Projectile.ai[0])
            {
                xflag = true;
            }
            if (Projectile.velocity.Y < 0f && Projectile.position.Y < Projectile.ai[1])
            {
                yflag = true;
            }
            if (Projectile.velocity.Y > 0f && Projectile.position.Y > Projectile.ai[1])
            {
                yflag = true;
            }
            if (xflag && yflag)
            {
                Projectile.Kill();
            }
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
                float rand1 = (float)Main.rand.Next(-10, 11);
                float rand2 = (float)Main.rand.Next(-10, 11);
                float rand3 = (float)Main.rand.Next(3, 9);
                float randAdjust = (float)Math.Sqrt((double)(rand1 * rand1 + rand2 * rand2));
                randAdjust = rand3 / randAdjust;
                rand1 *= randAdjust;
                rand2 *= randAdjust;
                int ourpleDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 62, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[ourpleDust];
                dust.noGravity = true;
                dust.position.X = Projectile.Center.X;
                dust.position.Y = Projectile.Center.Y;
                dust.position.X += (float)Main.rand.Next(-10, 11);
                dust.position.Y += (float)Main.rand.Next(-10, 11);
                dust.velocity.X = rand1;
                dust.velocity.Y = rand2;
                timerCounter++;
            }
        }
    }
}
