using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class BloodRay : ModProjectile
    {
        public const int Lifetime = 150;
        public const float MaxExponentialDamageBoost = 3f;
        public static readonly float ExponentialDamageBoost = (float)Math.Pow(MaxExponentialDamageBoost, 1f / Lifetime);
        public ref float Time => ref Projectile.ai[0];
        public ref float InitialDamage => ref Projectile.ai[1];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Ray");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 10;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = Lifetime;
        }

        public override void AI()
        {
            // Just doing damage = (int)(damage * scalar) wouldn't work here.
            // The exponential base would be too small for a weapon like this, and the
            // cast (which removes the fractional part) would overtake any increases before the damage can rise.
            if (InitialDamage == 0f)
            {
                InitialDamage = Projectile.damage;
                Projectile.netUpdate = true;
            }

            Time++;
            Projectile.damage = (int)(InitialDamage * Math.Pow(ExponentialDamageBoost, Time));

            if (Time >= 12f)
            {
                for (int i = 0; i < 2; i++)
                {
                    int dustType = Main.rand.NextBool(4) ? 182 : (int)CalamityDusts.Brimstone;
                    Vector2 dustSpawnPos = Projectile.position - Projectile.velocity * i / 2f;
                    Dust crimtameMagic = Dust.NewDustPerfect(dustSpawnPos, dustType);
                    crimtameMagic.scale = Main.rand.NextFloat(0.96f, 1.04f) * MathHelper.Lerp(1f, 1.7f, Time / Lifetime);
                    crimtameMagic.noGravity = true;
                    crimtameMagic.velocity *= 0.1f;
                }
            }
        }
    }
}
