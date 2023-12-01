using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Ranged
{
    public class BarinadeArrow : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 2;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }
        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            Vector3 DustLight = new Vector3(0.171f, 0.124f, 0.086f);
            Lighting.AddLight(Projectile.Center, DustLight * 3);

            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;

            if (Projectile.timeLeft == 300)
            {
                for (int i = 0; i <= 5; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 216 : 207, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.1f, 0.6f), 0, default, Main.rand.NextFloat(1.2f, 1.6f));
                    dust.noGravity = true;
                }
            }

            if (Projectile.timeLeft % 2 == 0 && Projectile.timeLeft < 295 && targetDist < 1400f)
            {
                SparkParticle spark = new SparkParticle(Projectile.Center - Projectile.velocity * 2f, -Projectile.velocity * 0.1f, false, 9, 1.5f, Color.White * 0.2f);
                GeneralParticleHandler.SpawnParticle(spark);
            }

        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

            for (int i = 0; i <= 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 32 : 216, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(8f)) * Main.rand.NextFloat(0.1f, 0.6f), 0, default, Main.rand.NextFloat(0.9f, 1.2f));
                dust.noGravity = false;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor * 2, 1);
            return true;
        }
    }
}
