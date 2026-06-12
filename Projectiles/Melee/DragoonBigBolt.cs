using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class DragoonBigBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public int time = 0;
        public float colorValue = 0;
        public float sizeMult = 1;
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 75;
            Projectile.timeLeft = 200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            colorValue = MathHelper.Lerp(colorValue, 50, 0.025f);
            Color usedColor = Color.Lerp(Color.Cyan, Color.Orchid, Utils.GetLerpValue(0, 50, colorValue));

            if (time == 0)
            {
                colorValue += 30;
                sizeMult = Projectile.ai[1];
            }

            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (targetDist < 1400f && Projectile.timeLeft > 5)
            {
                Vector2 pos = Projectile.Center;
                if (Projectile.timeLeft % 4 == 0)
                {
                    if (time < 120)
                    {
                        float velMult = (Projectile.ai[1] == 0.5f ? 0.2f : 3 * sizeMult);
                        Particle spark3 = new CustomSpark(pos, Projectile.velocity * 1.2f * velMult, "CalamityMod/Particles/GlowSpark", false, 11, 0.15f * sizeMult * Projectile.scale, usedColor, new Vector2(2f, 0.8f), true, true, shrinkSpeed: 1f);
                        GeneralParticleHandler.SpawnParticle(spark3);
                        sizeMult *= 0.97f;
                    }
                    Particle spark2 = new BoltParticle(pos, -Projectile.velocity * 0.05f, false, 30, 0.6f * Projectile.scale, usedColor, new Vector2(1.8f, 0.8f), true, true, false, 0.3f);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
                if (Main.rand.NextBool(35))
                {
                    Particle spark2 = new BoltParticle(pos, Projectile.velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.3f, 1.9f), false, 23, Main.rand.NextFloat(0.2f, 0.25f) * Projectile.scale, usedColor, new Vector2(1.8f, 0.8f), true, true, false, 0.3f);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
                if (Main.rand.NextBool(10))
                {
                    Particle spark2 = new CustomSpark(pos, Projectile.velocity * Main.rand.NextFloat(-0.4f, 0.4f), "CalamityMod/Particles/DrainLineBloom", false, 80, Main.rand.NextFloat(1.2f, 1.3f) * sizeMult * Projectile.scale, usedColor, new Vector2(1, 4), true, true);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
                if (time % 5 == 0)
                {
                    Dust dust = Dust.NewDustPerfect(pos, DustID.FireworksRGB, new Vector2(5, 5).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1f), 0, default, Main.rand.NextFloat(0.45f, 0.6f));
                    dust.noGravity = true;
                    dust.color = usedColor;
                }
            }
            if (Projectile.ai[1] == 0.5f && Projectile.timeLeft == 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    Particle orb = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Cyan, "CalamityMod/Particles/BloomCircle", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0.1f * Projectile.scale, 1.48f * Projectile.scale, 15);
                    GeneralParticleHandler.SpawnParticle(orb);
                    Particle orb2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0.1f * Projectile.scale, 0.925f * Projectile.scale, 15);
                    GeneralParticleHandler.SpawnParticle(orb2);
                }
            }
            time++;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.15f;
            int hitsToMinMult = 3; // Damage falls off very fast, so small bolts are better multitarget
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 300);
            if (Projectile.ai[1] != 0.5f)
            {
                Player Owner = Main.player[Projectile.owner];
                if (Projectile.numHits == 0)
                {
                    Projectile.timeLeft = 5;
                    Projectile.velocity = Vector2.Zero;
                    float fxScale = 3;
                    Vector2 pos = target.Center;
                    for (int i = 0; i < (int)(7 * fxScale); i++)
                    {
                        Particle spark2 = new BoltParticle(pos, (new Vector2(4, 4) * fxScale).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 1.9f), true, 13, Main.rand.NextFloat(0.1f, 0.15f) * fxScale, Main.rand.NextBool(5) ? Color.Cyan : Color.Orchid, new Vector2(1.8f, 0.8f), true, true, false, 0.7f);
                        GeneralParticleHandler.SpawnParticle(spark2);
                        Dust dust = Dust.NewDustPerfect(pos, ModContent.DustType<LightDust>(), (new Vector2(5, 5) * fxScale).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1f), 0, default, Main.rand.NextFloat(0.4f, 0.55f) * fxScale);
                        dust.noGravity = !Main.rand.NextBool(3);
                        dust.color = Main.rand.NextBool(5) ? Color.Cyan : Color.Orchid;
                    }
                    Particle pulse2 = new CustomPulse(pos, Vector2.Zero, Color.Cyan, "CalamityMod/Particles/HighResFoggyCircleHardEdge", new Vector2(1, 1), 0, 0f, 0.0815f * fxScale, 10);
                    GeneralParticleHandler.SpawnParticle(pulse2);
                    for (int i = 0; i < 2; i++)
                    {
                        Particle orb = new CustomPulse(pos, Vector2.Zero, Color.Orchid, "CalamityMod/Particles/BloomCircle", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 1.38f * fxScale, 0.5f * fxScale, 14);
                        GeneralParticleHandler.SpawnParticle(orb);
                        Particle orb2 = new CustomPulse(pos, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0.925f * fxScale, 0.2f * fxScale, 14);
                        GeneralParticleHandler.SpawnParticle(orb2);
                    }
                }

                Vector2 launchVel = Utils.DirectionTo(Projectile.Center, target.Center);
                target.MoveNPC(launchVel, 20, true, Owner);
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float size = 45 * sizeMult * (Projectile.numHits > 0 ? 6 : 1) * Projectile.scale;
            Player Owner = Main.player[Projectile.owner];
            if (time <= 1 && Projectile.ai[1] != 0.5f)
            {
                float _ = float.NaN;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Owner.Center, size, ref _);
            }
            else
                return CalamityUtils.CircularHitboxCollision(Projectile.Center, size, targetHitbox);
        }
        public override bool? CanCutTiles() => false;
    }
}
