using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class Shockblast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0f, 0.25f);
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
            while (timerCounter < projTimer)
            {
                float sizeMultiplier = (Projectile.ai[1] * 0.5f) + 1f;
                float rand1 = Main.rand.Next(-10, 11) * sizeMultiplier;
                float rand2 = Main.rand.Next(-10, 11) * sizeMultiplier;
                float rand3 = Main.rand.Next(3, 9) * sizeMultiplier;
                float randAdjust = (float)Math.Sqrt(rand1 * rand1 + rand2 * rand2);
                randAdjust = rand3 / randAdjust;
                rand1 *= randAdjust;
                rand2 *= randAdjust;
                int shockDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 185);
                Dust dust = Main.dust[shockDust];
                dust.scale = sizeMultiplier;
                dust.noGravity = true;
                dust.position.X = Projectile.Center.X;
                dust.position.Y = Projectile.Center.Y;
                dust.position.X += Main.rand.Next(-10, 11);
                dust.position.Y += Main.rand.Next(-10, 11);
                dust.velocity.X = rand1;
                dust.velocity.Y = rand2;
                timerCounter++;
            }
        }
    }
}
