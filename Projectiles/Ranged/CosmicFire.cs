using System;
using CalamityMod.Dusts;
using CalamityMod.Enums;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

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

        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 13;
            Projectile.timeLeft = 45 * Projectile.MaxUpdates;
        }

        public override void AI()
        {
            Time++;

            Lighting.AddLight(Projectile.Center, InnerColor.ToVector3() * 0.2f);
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center); //used for some drawing prevention for when it's offscreen since it makes a fuck load of particles

            if (Main.rand.NextBool(5) && Time > 12f && targetDist < 1400f)
            {
                Particle orb = new GenericBloom(Projectile.Center + Main.rand.NextVector2CircularEdge(5, 5), Projectile.velocity * Main.rand.NextFloat(0.05f, 0.5f), Color.Black, Main.rand.NextFloat(0.2f, 0.4f), Main.rand.Next(9, 12), true, false);
                GeneralParticleHandler.SpawnParticle(orb);

                int dustStyle = ModContent.DustType<VoidDustInverted>();
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10), dustStyle);
                dust.scale = Main.rand.NextFloat(0.6f, 1.2f);
                dust.velocity = new Vector2(0, Main.rand.NextFloat(0.1f, 5));
                dust.noGravity = false;
                dust.color = Color.LightGreen;
            }

            if (Projectile.timeLeft % 2 == 0 && Time > 12f && targetDist < 1400f)
            {
                Particle spark = new CustomSpark(Projectile.Center, -Projectile.velocity * 0.05f, "CalamityMod/Particles/GlowSpark2", false, 17, 0.052f, Color.Black, new Vector2(0.6f, 1.3f), false);
                GeneralParticleHandler.SpawnParticle(spark);
                Particle spark2 = new CustomSpark(Projectile.Center, -Projectile.velocity * 0.05f, "CalamityMod/Particles/GlowSpark", false, 17, 0.027f, Color.LightGreen, new Vector2(0.6f, 1.3f), true, false);
                GeneralParticleHandler.SpawnParticle(spark2, false, GeneralDrawLayer.AfterEverything);
            }

            if (Time == 9f)
            {
                for (int i = 0; i <= 10; i++)
                {
                    float variance = Main.rand.NextFloat(-0.7f, 0.7f);
                    int dustStyle = ModContent.DustType<VoidDustInverted>();
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, dustStyle);
                    dust2.scale = Main.rand.NextFloat(1.7f, 1.9f) - Math.Abs(variance);
                    dust2.velocity = (Projectile.velocity * 2).RotatedBy(variance) * Main.rand.NextFloat(0.35f, 1f) * (1 - Math.Abs(variance));
                    dust2.noGravity = true;
                    dust2.color = InnerColor;
                }
            }
            if (BounceHits > 0f)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(6) ? 278 : 263, -Projectile.velocity);
                dust.scale = dust.type == 278 ? Main.rand.NextFloat(0.3f, 0.6f) : Main.rand.NextFloat(0.6f, 1.4f);
                dust.velocity = -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.3f, 1.7f);
                dust.noGravity = true;
                dust.color = InnerColor;

                CalamityUtils.HomeInOnNPC(Projectile, false, 600f, 7f, 20f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(DeadSunsWind.Explosion with { Pitch = HitCount * 0.05f }, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DeadSunExplosion>(), (int)(Projectile.damage * 1.5f), 4f, Projectile.owner, HitCount * 10, BounceHits > 0f ? 5 : 0);
            if (HitCount >= 15)
                HitCount = 6;
            else
                HitCount++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 45 * Projectile.MaxUpdates;

            for (int i = 0; i < 2; i++)
            {
                Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Black, "CalamityMod/Particles/LargeBloom", Vector2.One, Main.rand.NextFloat(-10, 10), 0.35f, 0.4f, 38, false);
                GeneralParticleHandler.SpawnParticle(blastRing);
            }
            for (int i = 0; i < 3; i++)
            {
                Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, InnerColor, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 0.48f, 0.52f, 38);
                GeneralParticleHandler.SpawnParticle(blastRing, false, GeneralDrawLayer.AfterEverything);
            }

            float numberOfDusts = 10;
            for (int i = 0; i < numberOfDusts; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<VoidDustInverted>());
                dust.noGravity = true;
                dust.velocity = new Vector2(5, 5).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 1f);
                dust.scale = Main.rand.NextFloat(1.6f, 2.2f);
                dust.color = InnerColor;
            }

            if (BounceHits == 0f)
                SoundEngine.PlaySound(DeadSunsWind.Ricochet, Projectile.Center);

            BounceHits++;

            if (BounceHits >= 4f)
                Projectile.Kill();

            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }
    }
}
