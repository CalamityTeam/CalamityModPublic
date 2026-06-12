using CalamityMod.Particles;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class HeliumFlashBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private static float ExplosionRadius = 500.0f;
        private static float ParticleRadius = 210.0f;
        private Player Owner => Main.player[Projectile.owner];
        public int frameX = 0;
        public int frameY = 0;
        private const int horizontalFrames = 5;
        private const int verticalFrames = 4;
        public int time = 0;
        public int currentFrame = 0;
        private const int frameLength = 2;
        public bool damageFrame = false;
        public float starAngle = 0;


        public override void SetDefaults()
        {
            // Width and height don't actually do anything because the explosion uses custom collision
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (currentFrame == 19)
                damageFrame = true;
            else
                damageFrame = false;
            if (currentFrame == 0)
            {
                for (int i = 0; i < 6; i++)
                { 
                    Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 3f, 1f, 35, true);
                    GeneralParticleHandler.SpawnParticle(blastRing);
                    Particle blastRing2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 1.5f, 0.5f, 35, true);
                    GeneralParticleHandler.SpawnParticle(blastRing2);
                }
                Particle p = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Orange * 0.5f, "CalamityMod/Particles/BloomRing", Vector2.One, Main.rand.NextFloat(-5, 5), 5.5f, 0f, 20);
                GeneralParticleHandler.SpawnParticle(p);
            }
            if (currentFrame == 5)
            {
                Particle p = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Orange * 0.5f, "CalamityMod/Particles/BloomRing", Vector2.One, Main.rand.NextFloat(-5, 5), 4.5f, 0f, 15);
                GeneralParticleHandler.SpawnParticle(p);
            }
            if (currentFrame == 10)
            {
                starAngle = Main.rand.NextFloat(-0.9f, 0.9f);
                for (int i = 0; i < 4; i++)
                {
                    Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB);
                    Vector2 vel = (MathHelper.TwoPi * i / 4f).ToRotationVector2().RotatedBy(starAngle) * 8f;

                    Particle pulse = new GlowSparkParticle(Projectile.Center, vel, false, 6, 0.12f, Color.Orange, new Vector2(1.5f, 0.9f), true, true, 2);
                    GeneralParticleHandler.SpawnParticle(pulse);
                }
            }
            if (currentFrame == 19)
            {
                Owner.SetScreenshake(8f);
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/HeliumFlashExplodeNoMetal") { Volume = 1f }, Projectile.Center);
                for (int i = 0; i < 12; i++)
                {
                    Particle explosion = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Lerp(Color.Red, Color.Orange, Utils.GetLerpValue(0, 12, i, true)), "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, Main.rand.NextFloat(-5, 5), 0f, ParticleRadius * 0.0006f + 0.08f + 0.04f * i, (int)(30 - i * 1.3f));
                    GeneralParticleHandler.SpawnParticle(explosion);
                }
                for (int i = 0; i < 3; i++)
                {
                    Particle explosion = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Lerp(Color.Red, Color.OrangeRed, Utils.GetLerpValue(0, 3, i, true)), "CalamityMod/Particles/FlameExplosion", Vector2.One, Main.rand.NextFloat(-5, 5), 0f, ParticleRadius * 0.0006f + 0.093f + 0.04f * i * 4, (int)(30 - i * 2f));
                    GeneralParticleHandler.SpawnParticle(explosion);
                }

                for (int i = 0; i < 2; i++)
                {
                    Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 1f, 5f, 35, true);
                    GeneralParticleHandler.SpawnParticle(blastRing);
                    Particle blastRing2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 0.5f, 2f, 35, true);
                    GeneralParticleHandler.SpawnParticle(blastRing2);
                }
                for (int i = 0; i < 4; i++)
                {
                    Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB);
                    Vector2 vel = (MathHelper.TwoPi * i / 4f).ToRotationVector2().RotatedBy(starAngle) * 8f;

                    Particle pulse = new GlowSparkParticle(Projectile.Center, vel, false, 10, 0.22f, Color.Orange, new Vector2(1.5f, 0.9f), true, true, 1);
                    GeneralParticleHandler.SpawnParticle(pulse);
                }
                for (int i = 0; i < 35; i++)
                {
                    Vector2 randVel = new Vector2(50, 50).RotatedByRandom(100) * Main.rand.NextFloat(0.8f, 1.6f);
                    Particle smoke = new HeavySmokeParticle(Projectile.Center + randVel, randVel, Color.SlateGray, Main.rand.Next(25, 35 + 1), Main.rand.NextFloat(0.8f, 1.3f), 0.5f);
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
                for (int i = 0; i < 60; i++)
                {
                    Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB);
                    chargefull.velocity = new Vector2(25, 25).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 2f);
                    chargefull.scale = Main.rand.NextFloat(0.65f, 1.25f);
                    chargefull.noGravity = true;
                    chargefull.color = Color.Lerp(Color.White, Main.rand.NextBool(4) ? Color.Orange : Color.OrangeRed, 0.7f);
                }
            }
            currentFrame++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 300);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ApplyScalingForcedCrit(Projectile);
            if (Projectile.numHits > 0)
                Projectile.damage = (int)(Projectile.damage * 0.85f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, ExplosionRadius, targetHitbox);
        public override bool? CanDamage() => damageFrame ? null : false;
    }
}


