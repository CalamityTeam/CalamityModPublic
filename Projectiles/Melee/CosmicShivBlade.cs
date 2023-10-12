using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Melee
{
    public class CosmicShivBlade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public const int penetrateMax = 12;
        public const float maxScale = 1.8f;
        public bool initialized = false;
        public float startYVelSign = 0f;
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.penetrate = penetrateMax;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 290;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 7;
        }
        public override void AI()
        {
            if (!initialized)
            {
                startYVelSign = (float)Math.Sign(Projectile.velocity.Y) * 0.35f;
                initialized = true;
            }
            if (Projectile.penetrate == penetrateMax && Projectile.timeLeft < 245)
            {
                Projectile.velocity.Y -= startYVelSign; //arc on the X axis (the amount of pressure on this tiny detail made me want to tear my head off)
                if (Math.Abs(Projectile.velocity.X) < 30f)
                {
                    Projectile.velocity.X *= 1.04f;
                }
            }
            if (Projectile.velocity.Length() != 0)
            {
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.Pi / 4;
            }
            else
            {
                Projectile.rotation += MathHelper.Pi / 6f / (Projectile.scale * 2.5f);
            }
            Projectile.ai[0]++;
            if (Projectile.ai[1] >= 0)
            {
                Projectile.ai[1]--;
                if (Projectile.ai[1] == 0)
                {
                    Projectile.velocity *= -0.9f;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.scale += (maxScale - 1) / (float)penetrateMax;
            Projectile.ai[1] = 5 + Main.rand.Next(-2, 3);
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override void OnKill(int timeLeft)
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
                        int dustIndex = Dust.NewDust(Projectile.Center, 0, 0,
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
            return new Color(124, Main.DiscoG, 255, Projectile.alpha);
        }
    }
}
