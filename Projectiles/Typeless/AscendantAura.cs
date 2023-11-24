using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class AscendantAura : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public Player Owner => Main.player[Projectile.owner];
        public float beamWidth = 1.04f;
        public bool beamsize = false;
        public CalamityPlayer moddedOwner => Owner.Calamity();

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.width = Projectile.height = 78;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (beamWidth <= 0.96f)
            {
                beamsize = true;
            }
            if (beamWidth >= 1.04f)
            {
                beamsize = false;
            }
            beamWidth += (beamsize ? 0.015f : -0.015f);

            Projectile.scale = beamWidth;

            if (Projectile.timeLeft >= 240)
            {
                int dustAmount = 200;
                for (int d = 0; d < dustAmount; d++)
                {
                    float angle = MathHelper.TwoPi / dustAmount * d;
                    Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(5f, 40f);

                    Dust spawnDust = Dust.NewDustPerfect(Projectile.Center, 204, velocity);
                    spawnDust.noGravity = true;
                    spawnDust.scale = velocity.Length() * 0.05f;
                    spawnDust.velocity *= 0.4f;
                }
            }
            // Stay on the player's head
            Projectile.Center = (Owner.MountedCenter + new Vector2(0, -45));

            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(5, 5);
            int lifetime = Main.rand.Next(3, 6);
            float scale = Main.rand.NextFloat(0.5f, 0.9f);
            Color color = Main.rand.NextBool(3) ? Color.LightGreen : Color.Khaki;
            SparkParticle spark1 = new SparkParticle(spawnPos, new Vector2(Main.rand.NextFloat(7, 12), Main.rand.NextFloat(7, 12)), false, lifetime, scale, color);
            GeneralParticleHandler.SpawnParticle(spark1);
            SparkParticle spark2 = new SparkParticle(spawnPos, new Vector2(Main.rand.NextFloat(-7, -12), Main.rand.NextFloat(7, 12)), false, lifetime, scale, color);
            GeneralParticleHandler.SpawnParticle(spark2);
            SparkParticle spark3 = new SparkParticle(spawnPos, new Vector2(Main.rand.NextFloat(7, 12), Main.rand.NextFloat(-7, -12)), false, lifetime, scale, color);
            GeneralParticleHandler.SpawnParticle(spark3);
            SparkParticle spark4 = new SparkParticle(spawnPos, new Vector2(Main.rand.NextFloat(-7, -12), Main.rand.NextFloat(-7, -12)), false, lifetime, scale, color);
            GeneralParticleHandler.SpawnParticle(spark4);
            SparkParticle spark5 = new SparkParticle(Projectile.Center + new Vector2(20, 0), new Vector2(Main.rand.NextFloat(7, 12), 0), false, lifetime, scale, color);
            GeneralParticleHandler.SpawnParticle(spark5);
            SparkParticle spark6 = new SparkParticle(Projectile.Center + new Vector2(-20, 0), new Vector2(Main.rand.NextFloat(-7, -12), 0), false, lifetime, scale - 0.3f, color);
            GeneralParticleHandler.SpawnParticle(spark6);

            // Emit some light
            Vector3 Light = new Vector3(0.251f, 0.255f, 0.219f);
            Lighting.AddLight(Projectile.Center, Light * 5);
        }
        public override void OnKill(int timeLeft)
        {
            float numberOfDusts = 40f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(8f, 0).RotatedBy(rot);
                Vector2 velOffset = new Vector2(2.5f, 0).RotatedBy(rot);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, 204, new Vector2(velOffset.X, velOffset.Y));
                dust.noGravity = true;
                dust.velocity = velOffset * Main.rand.NextFloat(0.9f, 1.1f);
                dust.scale = 2.5f;
            }
        }

        public override bool? CanDamage() => false;
    }
}
