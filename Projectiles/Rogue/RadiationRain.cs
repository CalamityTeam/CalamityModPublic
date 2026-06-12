using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Enums;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class RadiationRain : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public NPC targetedNPC;
        public int time = 0;
        public Vector2 spawnSpot;

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 10;
            Projectile.timeLeft = 200;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30 * Projectile.MaxUpdates;
            Projectile.ArmorPenetration = 30;
            Projectile.noEnchantmentVisuals = true;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            if (time == 0)
            {
                float orbScale = 0.6f * Main.rand.NextFloat(0.8f, 1.1f);
                Color smokeColor = Color.Lerp(Color.DimGray, Color.DarkGreen, Main.rand.NextFloat(0.2f, 0.6f));
                Particle orb = new CustomPulse(Projectile.Center, Vector2.Zero, smokeColor * 0.7f, "CalamityMod/ExtraTextures/GreyscaleVortex", new Vector2(1, 1), Projectile.ai[2] * 0.45f, orbScale, orbScale * 1.1f, 12, false);
                GeneralParticleHandler.SpawnParticle(orb);
                Particle orb3 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Chartreuse, "CalamityMod/Particles/LargeBloom", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0.8f, 0.4f, 18);
                GeneralParticleHandler.SpawnParticle(orb3, false, GeneralDrawLayer.AfterEverything);
                Particle orb2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/LargeBloom", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0.4f, 0.2f, 18);
                GeneralParticleHandler.SpawnParticle(orb2, false, GeneralDrawLayer.AfterEverything);

                for (int i = 0; i < 2; i++)
                {
                    int dir = (i == 0 ? 1 : -1);
                    Particle pulse3 = new GlowSparkParticle(Projectile.Center + new Vector2(20 * dir, 0), new Vector2(10 * dir, 0), false, 12, 0.087f, Color.Chartreuse, new Vector2(1.7f, 0.8f), true, true, 0.8f);
                    GeneralParticleHandler.SpawnParticle(pulse3, false, GeneralDrawLayer.AfterEverything);
                }

                for (int i = 0; i < 3; i++)
                {
                    smokeColor = Color.Lerp(Color.DimGray, Color.DarkGreen, Main.rand.NextFloat(0.2f, 0.6f));
                    Particle smoke = new HeavySmokeParticle(Projectile.Center, new Vector2(27, 27).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1f), smokeColor, Main.rand.Next(25, 40 + 1), Main.rand.NextFloat(0.7f, 1.3f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool(), required: false);
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
                spawnSpot = Projectile.Center;

                targetedNPC = Projectile.Center.ClosestNPCAt(1200);
                if (targetedNPC != null)
                    Projectile.velocity = (targetedNPC.Center - Projectile.Center + targetedNPC.velocity * 1.5f).SafeNormalize(Vector2.UnitX) * 8;
                else
                    Projectile.velocity = (Owner.ClampedMouseWorld() - Projectile.Center).SafeNormalize(Vector2.UnitX) * 8;
            }

            if (targetDist < 1400 && time > 5)
            {
                Particle spark = new GlowSparkParticle(Projectile.Center + Main.rand.NextVector2Circular(70, 70), Projectile.velocity * Main.rand.NextFloat(1.5f, 5f), false, 2, Main.rand.NextFloat(0.04f, 0.06f), Color.Lerp(Color.Green, Color.Chartreuse, Main.rand.NextFloat(0.2f, 1f)), new Vector2(0.2f * (3 * Utils.GetLerpValue(40, 0, time, true) + 1),  1.5f), true, false, 0.3f);
                GeneralParticleHandler.SpawnParticle(spark);
                if (time % 6 == 0)
                {
                    Particle spark2 = new AltLineParticle(Projectile.Center + Main.rand.NextVector2Circular(70, 70), Projectile.velocity * Main.rand.NextFloat(4.5f, 9f), false, 12, Main.rand.NextFloat(0.9f, 1.1f) * (2 * Utils.GetLerpValue(40, 0, time, true) + 1), Color.Lerp(Color.Green, Color.Chartreuse, Main.rand.NextFloat(0.2f, 1f)));
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
                if (time % 4 == 0)
                {
                    Dust c = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(70, 70), DustID.RainbowMk2);
                    c.velocity = Projectile.velocity * Main.rand.NextFloat(0.5f, 3f);
                    c.scale = Main.rand.NextFloat(0.45f, 0.75f);
                    c.noGravity = true;
                    c.color = Color.Lerp(Color.White, Main.rand.NextBool(4) ? Color.Green : Color.Chartreuse, 0.7f);
                    c.noLight = true;
                    c.noLightEmittence = true;
                }
            }
            if (time % 10 * Projectile.MaxUpdates == 0)
            {
                Owner.SetScreenshake(1.5f);
            }

            if (Projectile.ai[2] > 0 && time == 30)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnSpot, Projectile.velocity, ModContent.ProjectileType<RadiationRain>(), (int)(Projectile.damage), 0f, Projectile.owner, 0, 0, Projectile.ai[2] - 1);

            time++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 60);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 0)
                Projectile.damage = (int)(Projectile.damage * 0.9f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 60, targetHitbox);
    }
}
