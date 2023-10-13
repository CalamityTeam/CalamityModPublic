using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Typeless;

namespace CalamityMod.Projectiles.Ranged
{
    public class CosmicFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public ref int HitCount => ref Main.player[Projectile.owner].Calamity().deadSunCounter;
        public int Time = 0;
        public int bounceKill = 0;
        public bool Homing = false;
        public Color color1 = Color.Turquoise;
        public Color color2 = Color.Indigo;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 9;
            Projectile.timeLeft = 500;
        }

        public override void AI()
        {
            Time++;
            Lighting.AddLight(Projectile.Center, 0.117f, 0.155f, 0.159f);
            
            if (Projectile.timeLeft % 3 == 0 && Time > 12)
            {
                //SparkParticle spark = new SparkParticle(Projectile.Center, -Projectile.velocity * 0.05f, false, 20, 2.3f, Main.rand.NextBool() ? color2 : color1);
                //GeneralParticleHandler.SpawnParticle(spark);
                AltSparkParticle spark = new AltSparkParticle(Projectile.Center, -Projectile.velocity * 0.05f, false, 20, 2.3f, Color.Black);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            if (Projectile.timeLeft % 2 == 0 && Time > 12)
            {
                SparkParticle spark2 = new SparkParticle(Projectile.Center, -Projectile.velocity * 0.05f, false, 20, 0.9f, color1);
                GeneralParticleHandler.SpawnParticle(spark2);
            }
            if (Time == 7)
            {
                for (int i = 0; i <= 18; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 191 : Main.rand.NextBool(4) ? 229 : 156, Projectile.velocity);
                    dust.scale = Main.rand.NextFloat(1.5f, 2.3f);
                    dust.velocity = Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.3f, 2.1f);
                    dust.noGravity = true;
                }
            }
            if (Homing)
            {
                for (int i = 0; i <= 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(4) ? 229 : 156, -Projectile.velocity);
                    dust.scale = Main.rand.NextFloat(0.7f, 1.3f);
                    dust.velocity = -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.3f, 1.7f);
                    dust.noGravity = true;
                }
                CalamityUtils.HomeInOnNPC(Projectile, false, 600f, 12f, 20f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(DeadSunsWind.Explosion with { Pitch = HitCount * 0.05f }, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DeadSunExplosion>(), Projectile.damage / 2, 4f, Projectile.owner, HitCount * 10, Homing ? 5 : 0);
            if (HitCount >= 15)
                HitCount = 6;
            else
                HitCount++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 500;
            float numberOfDusts = 25;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(Main.rand.NextFloat(1, 3.1f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                Vector2 velOffset = new Vector2(Main.rand.NextFloat(1, 3.1f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, Main.rand.NextBool(6) ? 229 : 156, new Vector2(velOffset.X, velOffset.Y));
                dust.noGravity = true;
                dust.velocity = velOffset;
                dust.scale = Main.rand.NextFloat(2.6f, 3.2f);
            }
            Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, color1, new Vector2(1f, 1f), Main.rand.NextFloat(5, -5), 0.1f, 0.9f - (bounceKill * 0.25f), 25);
            GeneralParticleHandler.SpawnParticle(pulse);
            Particle pulse2 = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, color2, new Vector2(1f, 1f), Main.rand.NextFloat(5, -5), 0.05f, 0.8f - (bounceKill * 0.25f), 25);
            GeneralParticleHandler.SpawnParticle(pulse2);
            Homing = true;
            if (bounceKill == 0)
                SoundEngine.PlaySound(DeadSunsWind.Ricochet, Projectile.Center);

            bounceKill++;
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            if (bounceKill == 4)
                Projectile.Kill();
            return false;
        }
    }
}
