using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SlimeBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public bool Empowered = false;
        public int countdown = 90;
        public bool postBounce = false;
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            if (countdown < 81)
            {
                Particle orb = new GenericBloom(Projectile.Center, Vector2.Zero, Color.SkyBlue, Empowered ? 0.22f : 0.17f, postBounce ? 2 : 5, true);
                GeneralParticleHandler.SpawnParticle(orb);
            }
            if (countdown > -55 && Main.rand.NextBool(postBounce ? 2 : 7))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10), Main.rand.NextBool(3) ? 16 : 20);
                dust.scale = Main.rand.NextFloat(0.3f, 0.7f);
                dust.velocity = -Projectile.velocity * 0.7f;
            }
            countdown--;
            if (countdown <= 0)
                Projectile.velocity *= 0.97f;
            Vector3 DustLight = new Vector3(0.100f, 0.150f, 0.200f);
            Lighting.AddLight(Projectile.Center, DustLight * 3);
            if (countdown == -55)
            {
                Projectile.damage *= 2;
                float numberOfDusts = 10;
                float rotFactor = 360f / numberOfDusts;
                for (int i = 0; i < numberOfDusts; i++)
                {
                    float rot = MathHelper.ToRadians(i * rotFactor);
                    Vector2 offset = new Vector2(Main.rand.NextFloat(3, 3.1f), 0).RotatedBy(rot);
                    Vector2 velOffset = new Vector2(Main.rand.NextFloat(3, 3.1f), 0).RotatedBy(rot);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, Main.rand.NextBool(3) ? 59 : 20, new Vector2(velOffset.X, velOffset.Y));
                    dust.noGravity = true;
                    dust.velocity = velOffset;
                    dust.scale = Main.rand.NextFloat(1.2f, 1.9f);
                }
            }
            if (countdown < -55 && Projectile.timeLeft % 10 == 0)
            {
                Empowered = true;
                SparkParticle spark1 = new SparkParticle(Projectile.Center, new Vector2(0, 2), false, 7, 0.9f, Color.SkyBlue);
                GeneralParticleHandler.SpawnParticle(spark1);
                SparkParticle spark2 = new SparkParticle(Projectile.Center, new Vector2(0, -2), false, 7, 0.9f, Color.SkyBlue);
                GeneralParticleHandler.SpawnParticle(spark2);
                SparkParticle spark3 = new SparkParticle(Projectile.Center, new Vector2(2, 0), false, 7, 0.9f, Color.SkyBlue);
                GeneralParticleHandler.SpawnParticle(spark3);
                SparkParticle spark4 = new SparkParticle(Projectile.Center, new Vector2(-2, 0), false, 7, 0.9f, Color.SkyBlue);
                GeneralParticleHandler.SpawnParticle(spark4);

            }
            if (Empowered)
            {
                Projectile.penetrate = 1;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X * (postBounce ? 1f : 3f);
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y * (postBounce ? 1f : 3f);
            }
            postBounce = true;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.SkyBlue, new Vector2(1f, 1f), Main.rand.NextFloat(5, -5), 0f, 0.4f, 25);
            GeneralParticleHandler.SpawnParticle(pulse);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slimed, Empowered ? 480 : 180);
            if (Empowered)
            {
                Particle pulse2 = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.SkyBlue, new Vector2(1f, 1f), Main.rand.NextFloat(5, -5), 0f, 0.65f, 35);
                GeneralParticleHandler.SpawnParticle(pulse2);
                float numberOfDusts = 12;
                float rotFactor = 360f / numberOfDusts;
                for (int i = 0; i < numberOfDusts; i++)
                {
                    float rot = MathHelper.ToRadians(i * rotFactor);
                    Vector2 offset = new Vector2(Main.rand.NextFloat(1, 3.1f), 0).RotatedBy(rot);
                    Vector2 velOffset = new Vector2(Main.rand.NextFloat(1, 3.1f), 0).RotatedBy(rot);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, Main.rand.NextBool(3) ? 34 : 59, new Vector2(velOffset.X, velOffset.Y));
                    dust.noGravity = false;
                    dust.alpha = 130;
                    dust.velocity = velOffset;
                    dust.scale = dust.type == 20 ? Main.rand.NextFloat(0.9f, 1.9f) : Main.rand.NextFloat(1.6f, 2.2f);
                }
            }
        }
    }
}
