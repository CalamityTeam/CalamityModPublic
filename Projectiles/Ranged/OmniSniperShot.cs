using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class OmniSniperShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public const int DoubleDamageTime = 90;
        public static int Lifetime = 600;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 6;
            Projectile.light = 0.5f;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 7;
            Projectile.scale = 1.18f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.penetrate = 5;
            Projectile.timeLeft = Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.damage += Projectile.originalDamage / DoubleDamageTime;
            if (Projectile.timeLeft == Lifetime - 2)
            {
                for (int i = 0; i <= 4; i++) //Dragon's Breath shot particles my beloved
                {
                    Vector2 sparkVelocity = Projectile.velocity * 0.5f;

                    float sparkScale1 = Main.rand.NextFloat(0.3f, 0.8f);
                    Vector2 sparkvelocity1 = sparkVelocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(0.4f, 0.95f);
                    SparkParticle spark1 = new SparkParticle(Projectile.Center, sparkvelocity1, false, 6, sparkScale1, Main.rand.NextBool() ? Color.Goldenrod : Color.DarkGoldenrod);
                    GeneralParticleHandler.SpawnParticle(spark1);

                    float sparkScale2 = Main.rand.NextFloat(0.4f, 1f);
                    Vector2 sparkvelocity2 = sparkVelocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(1.1f, 3.1f);
                    SparkParticle spark2 = new SparkParticle(Projectile.Center, sparkvelocity2, false, 6, sparkScale2, Main.rand.NextBool() ? Color.Goldenrod : Color.DarkGoldenrod);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
            }
            if (Projectile.timeLeft < Lifetime - 3 && Projectile.timeLeft > Lifetime - 150)
            {
                SparkParticle spark = new SparkParticle(Projectile.Center, -Projectile.velocity * 0.05f, false, 9, 1.2f, Color.Coral * 0.3f);
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.Center, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 6; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? 158 : 55, new Vector2(5, 5).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1.5f), 0, default, Main.rand.NextFloat(1.6f, 2.2f));
                dust.noGravity = true;
                dust.fadeIn = 0.5f;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //Always critically strikes
            modifiers.SetCrit();
            for (int i = 0; i <= 2; i++)
            {
                LineParticle spark = new LineParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(0.18f, 0.44f)) * Main.rand.NextFloat(0.4f, 2.5f), false, 8, 0.9f, Main.rand.NextBool() ? Color.Goldenrod : Color.Gold);
                GeneralParticleHandler.SpawnParticle(spark);
                LineParticle spark2 = new LineParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.18f, -0.44f)) * Main.rand.NextFloat(0.4f, 2.5f), false, 8, 0.9f, Main.rand.NextBool() ? Color.Goldenrod : Color.Gold);
                GeneralParticleHandler.SpawnParticle(spark2);
            }

            for (int i = 0; i <= 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? 158 : 55, new Vector2(5, 5).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1.5f), 0, default, Main.rand.NextFloat(1.6f, 2.2f));
                dust.noGravity = true;
                dust.fadeIn = 0.5f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor);
            return false;
        }
    }
}
