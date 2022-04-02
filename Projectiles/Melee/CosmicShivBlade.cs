using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Melee
{
    public class CosmicShivBlade : ModProjectile
    {
        public const int penetrateMax = 12;
        public const float maxScale = 1.8f;
        public bool initialized = false;
        public float startYVelSign = 0f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 42;
            projectile.friendly = true;
            projectile.penetrate = penetrateMax;
            projectile.tileCollide = false;
            projectile.timeLeft = 290;
        }
        public override void AI()
        {
            if (!initialized)
            {
                startYVelSign = (float)Math.Sign(projectile.velocity.Y) * 0.35f;
                initialized = true;
            }
            if (projectile.penetrate == penetrateMax && projectile.timeLeft < 245)
            {
                projectile.velocity.Y -= startYVelSign; //arc on the X axis (the amount of pressure on this tiny detail made me want to tear my head off)
                if (Math.Abs(projectile.velocity.X) < 30f)
                {
                    projectile.velocity.X *= 1.04f;
                }
            }
            if (projectile.velocity.Length() != 0)
            {
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.Pi / 4;
            }
            else
            {
                projectile.rotation += MathHelper.Pi / 6f / (projectile.scale * 2.5f);
            }
            projectile.ai[0]++;
            if (projectile.ai[1] >= 0)
            {
                projectile.ai[1]--;
                if (projectile.ai[1] == 0)
                {
                    projectile.velocity *= -0.9f;
                }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.penetrate == penetrateMax)
            {
                projectile.damage /= 15;
            }
            else
            {
                projectile.scale += (maxScale - 1) / (float)penetrateMax;
            }
            projectile.ai[1] = 5 + Main.rand.Next(-2, 3);
            target.immune[projectile.owner] = 0; //so that all blades can hit the enemy
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 60);
            target.AddBuff(BuffID.Frostburn, 60);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 60);
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 60);
        }

        public override void Kill(int timeLeft)
        {
            Vector2 randomCirclePointVector = Vector2.One.RotatedByRandom(MathHelper.ToRadians(32f));
            float lerpStart = (float)Main.rand.Next(12, 17);
            float lerpEnd = (float)Main.rand.Next(3, 7);
            for (float i = 0; i < 9f; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    Vector2 randomCirclePointRotated = randomCirclePointVector.RotatedBy((j == 0 ? 1 : -1) * MathHelper.TwoPi / 18);
                    for (float k = 0f; k < 20f; ++k)
                    {
                        Vector2 randomCirclePointLerped = Vector2.Lerp(randomCirclePointVector, randomCirclePointRotated, k / 20f);
                        float lerpMultiplier = MathHelper.Lerp(lerpStart, lerpEnd, k / 20f) * 4f;
                        int dustIndex = Dust.NewDust(projectile.Center, 0, 0,
                            173,
                            0f, 0f, 100, default, 1.1f);
                        Main.dust[dustIndex].velocity *= 0.1f;
                        Main.dust[dustIndex].noGravity = true;
                        Main.dust[dustIndex].velocity += randomCirclePointLerped * lerpMultiplier;
                    }
                }

                randomCirclePointVector = randomCirclePointVector.RotatedBy(MathHelper.TwoPi / 9);
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(124, Main.DiscoG, 255, projectile.alpha);
        }
    }
}