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
        public ref float Time => ref Projectile.ai[0];
        public ref float BounceHits => ref Projectile.ai[1];

        public Color InnerColor = Color.LightGreen;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 10;
            Projectile.timeLeft = 15 * Projectile.MaxUpdates;
        }

        public override void AI()
        {
            Time++;
            Lighting.AddLight(Projectile.Center, InnerColor.ToVector3() * 0.2f);
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center); //used for some drawing prevention for when it's offscreen since it makes a fuck load of particles
            if (Projectile.timeLeft % 3 == 0 && Time > 12f && targetDist < 1400f)
            {
                AltSparkParticle spark = new AltSparkParticle(Projectile.Center, -Projectile.velocity * 0.05f, false, 17, 2.3f, Color.Black);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            if (Main.rand.NextBool(3) && Time > 12f && targetDist < 1400f)
            {
                Particle orb = new GenericBloom(Projectile.Center + Main.rand.NextVector2Circular(10, 10), Projectile.velocity * Main.rand.NextFloat(0.05f, 0.5f), Color.Black, Main.rand.NextFloat(0.2f, 0.45f), Main.rand.Next(9, 12), true, false);
                GeneralParticleHandler.SpawnParticle(orb);
            }
            
            if (Projectile.timeLeft % 3 == 0 && Time > 12f && targetDist < 1400f)
            {
                LineParticle spark2 = new LineParticle(Projectile.Center, -Projectile.velocity * 0.05f, false, 17, 1.7f, InnerColor);
                GeneralParticleHandler.SpawnParticle(spark2);
            }
            if (Main.rand.NextBool(3) && Time > 12f && targetDist < 1400f)
            {
                Particle orb2 = new GenericBloom(Projectile.Center + Main.rand.NextVector2Circular(5, 5), Projectile.velocity * Main.rand.NextFloat(0.05f, 0.5f), InnerColor, Main.rand.NextFloat(0.05f, 0.3f), Main.rand.Next(9, 12), true);
                GeneralParticleHandler.SpawnParticle(orb2);
            }
            
            if (Time == 7f)
            {
                for (int i = 0; i <= 18; i++)
                {
                    int dustStyle = Main.rand.NextBool() ? 66 : 263;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 191 : dustStyle, Projectile.velocity);
                    dust.scale = Main.rand.NextFloat(1.5f, 2.3f);
                    dust.velocity = Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.3f, 2.1f);
                    dust.noGravity = true;
                    dust.color = dust.type == dustStyle ? InnerColor : default;
                }
            }
            if (BounceHits > 0f)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(6) ? 278 : 263, -Projectile.velocity);
                dust.scale = dust.type == 278 ? Main.rand.NextFloat(0.3f, 0.6f) : Main.rand.NextFloat(0.6f, 1.4f);
                dust.velocity = -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.3f, 1.7f);
                dust.noGravity = true;
                dust.color = InnerColor;

                CalamityUtils.HomeInOnNPC(Projectile, false, 600f, 12f, 20f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(DeadSunsWind.Explosion with { Pitch = HitCount * 0.05f }, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DeadSunExplosion>(), Projectile.damage / 2, 4f, Projectile.owner, HitCount * 10, BounceHits > 0f ? 5 : 0);
            if (HitCount >= 15)
                HitCount = 6;
            else
                HitCount++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 15 * Projectile.MaxUpdates;
            float numberOflines = 5;
            float rotFactorlines = 360f / numberOflines;
            for (int i = 0; i < numberOflines; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactorlines);
                Vector2 offset = new Vector2(Main.rand.NextFloat(1, 3.1f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                Vector2 velOffset = new Vector2(Main.rand.NextFloat(1, 3.1f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                AltSparkParticle spark = new AltSparkParticle(Projectile.Center + offset, velOffset, false, 20, Main.rand.NextFloat(1.9f, 2.3f), Color.Black);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            float numberOfDusts = 25;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(Main.rand.NextFloat(1, 3.1f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                Vector2 velOffset = new Vector2(Main.rand.NextFloat(1, 3.1f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, Main.rand.NextBool(3) ? 156 : 191, new Vector2(velOffset.X, velOffset.Y));
                dust.noGravity = true;
                dust.noLight = dust.type == 191 ? true : false;
                dust.velocity = dust.type == 191 ? velOffset * 2.5f : velOffset;
                dust.scale = dust.type == 191 ? Main.rand.NextFloat(0.9f, 1.9f) : Main.rand.NextFloat(2.6f, 3.2f);
            }
            Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, InnerColor, new Vector2(1f, 1f), Main.rand.NextFloat(5, -5), 0.1f, 0.9f - (BounceHits * 0.25f), 25);
            GeneralParticleHandler.SpawnParticle(pulse);

            if (BounceHits == 0f)
                SoundEngine.PlaySound(DeadSunsWind.Ricochet, Projectile.Center);

            BounceHits++;
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            if (BounceHits >= 4f)
                Projectile.Kill();
            return false;
        }
    }
}
