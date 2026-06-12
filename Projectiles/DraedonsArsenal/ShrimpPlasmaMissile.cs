using System;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class ShrimpPlasmaMissile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public ref float time => ref Projectile.ai[0];
        public NPC closestNPC;
        public Vector2 offset = Vector2.Zero;
        public bool hitTile = false;
        public float randomRate = 0;
        public float randomSize = 0;
        public override void SetStaticDefaults()
        {
            //ProjectileID.Sets.SummonTagDamageMultiplier[Type] = 0f; // The balance people will say if this is needed
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 3;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            if (time == 0)
            {
                randomRate = Main.rand.NextFloat(1, 3);
                randomSize = Main.rand.NextFloat(1, 5);
            }
            float sine = (float)Math.Sin(time * 0.075f / MathHelper.Pi * randomRate);

            if (time > 0 && Projectile.numHits == 0)
            {
                if (Utils.Distance(Owner.Center, Projectile.Center) < 1400)
                {
                    Particle spark = new CustomSpark(Projectile.Center, Projectile.velocity * -0.01f, "CalamityMod/Particles/BloomCircle", false, 12, 0.18f, Effects.ArsenalEffects.ArsenalPlasmaColor, new Vector2(0.3f, 1), true, true, 0, false, false, 0.3f, glowCenterScale: 0.5f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                if (Projectile.ai[2] == 0)
                {
                    if (time > 10)
                    {
                        if (closestNPC == null || closestNPC.life <= 0 || !closestNPC.CanBeChasedBy())
                            closestNPC = Projectile.Center.ClosestNPCAt(900);
                        if (closestNPC != null)
                        {
                            Projectile.timeLeft++;
                            float distMult = ((float)Math.Pow(Utils.GetLerpValue(140, 80, Projectile.Center.Distance(closestNPC.Center), true), 3));
                            CalamityUtils.HomeInOnSelectedNPC(Projectile, closestNPC, true, 0.2f + 0.3f * distMult, 8, 0.99f - 0.1f * distMult, accelerate: true);
                        }
                    }
                }
                else
                {
                    Vector2 goalPosition = Owner.Calamity().mouseWorld + offset;
                    int fallTime = 300 - (int)Projectile.ai[2] * Projectile.extraUpdates;
                    offset = Vector2.UnitX * sine * 15 * randomSize;
                    if (time == fallTime)
                    {
                        Projectile.timeLeft = 600;
                        Projectile.extraUpdates = 6;
                        Projectile.scale = 1.5f;
                    }
                    if (time > fallTime)
                    {
                        float distMult = (float)Math.Pow(Utils.GetLerpValue(480, 340, time, true), 3);

                        Vector2 moveTo = (goalPosition - Projectile.Center).SafeNormalize(Vector2.UnitX);

                        if (Projectile.velocity.Y < 8)
                            Projectile.velocity.Y += 0.1f;
                        Projectile.velocity.X += moveTo.X * 0.5f;
                        Projectile.velocity.X *= 0.975f;
                    }
                    else
                        Projectile.timeLeft++;
                }

                if (Main.rand.NextBool(5))
                {
                    int dustStyle = Effects.ArsenalEffects.ArsenalPlasmaDust;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, dustStyle);
                    dust.scale = Main.rand.NextFloat(0.5f, 0.7f);
                    dust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.1f, 0.3f);
                    dust.noGravity = false;
                    dust.color = Effects.ArsenalEffects.ArsenalPlasmaColor;
                    dust.fadeIn = -0.2f;
                }
            }
            if (Collision.SolidCollision(Projectile.Center, 8, 8) && Projectile.numHits == 0 && Projectile.ai[2] > 0 && Projectile.Center.Y > Owner.Calamity().mouseWorld.Y && Projectile.scale > 1)
            {
                hitTile = true;
                Projectile.numHits = 1;
                Explode();
            }
            time++;
        }
        public void Explode()
        {
            float lifetimeMult = (hitTile ? 5 : 1);
            if (hitTile)
                Projectile.localNPCHitCooldown = 8;
            Projectile.extraUpdates = 0;
            Projectile.timeLeft = (int)(12 * lifetimeMult);
            Projectile.velocity = Vector2.Zero;
            for (int i = 0; i < (int)(12 * Projectile.scale); i++)
            {
                bool noFall = !Main.rand.NextBool(5);
                int dustStyle = noFall ? ModContent.DustType<SquashDust>() : Effects.ArsenalEffects.ArsenalPlasmaDust;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, dustStyle);
                dust.scale = Main.rand.NextFloat(0.9f, 1.7f) * (noFall ? 2 : 0.75f) * Projectile.scale;
                dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(6.4f, 7.5f) * (noFall ? 0.7f : 1.2f) * Projectile.scale;
                dust.noGravity = noFall;
                dust.color = Effects.ArsenalEffects.ArsenalPlasmaColor;
                dust.fadeIn = (noFall ? 4.5f * Projectile.scale : 1f);
            }
            SoundEngine.PlaySound(AqueousHunterDrone.Hit with { Volume = 0.4f * Projectile.scale, Pitch = Main.rand.NextFloat(-0.1f, 0.1f), MaxInstances = 30 }, Projectile.Center);
            Particle bolt2 = new CustomPulse(Projectile.Center, Vector2.Zero, Effects.ArsenalEffects.ArsenalPlasmaColor * 0.95f, "CalamityMod/Particles/SmokeExplosion", Vector2.One, Main.rand.NextFloat(-10f, 10f), 0f, 0.1f * Projectile.scale, 14);
            GeneralParticleHandler.SpawnParticle(bolt2);

            Particle bolt3 = new CustomPulse(Projectile.Center, Vector2.Zero, Effects.ArsenalEffects.ArsenalPlasmaColor * 0.75f, "CalamityMod/Particles/WaterFoam", Vector2.One, Main.rand.NextFloat(-10f, 10f), 0f, 0.77f * Projectile.scale, (int)(10 * lifetimeMult));
            GeneralParticleHandler.SpawnParticle(bolt3);

            Particle spark = new CustomSpark(Projectile.Center, Vector2.Zero, "CalamityMod/Particles/BloomCircle", false, (int)(17 * lifetimeMult), 0.6f * Projectile.scale, Effects.ArsenalEffects.ArsenalPlasmaColor, new Vector2(1, 1), true, true, 0, false, false);
            GeneralParticleHandler.SpawnParticle(spark);
            if (hitTile)
            {
                Particle spark2 = new CustomSpark(Projectile.Center, Vector2.Zero, "CalamityMod/Particles/BloomCircle", false, (int)(20 * lifetimeMult), 0.6f * Projectile.scale * lifetimeMult, Effects.ArsenalEffects.ArsenalPlasmaColor * 0.35f, new Vector2(1, 1), true, true, 0, false, false, glowOpacity: 0.35f);
                GeneralParticleHandler.SpawnParticle(spark2);
            }
            if (Projectile.scale > 1)
            {
                Player Owner = Main.player[Projectile.owner];
                Owner.SetScreenshake(2.5f);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits == 0)
            {
                Explode();
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 0)
            {
                float minMult = 0.5f;
                int hitsToMinMult = 8;
                float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
                modifiers.SourceDamage *= damageMult * 0.5f;
            }
        }
        public override bool? CanDamage() => ((time > 5 && Projectile.ai[2] == 0) || (Projectile.scale > 1)) ? null : false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width * (Projectile.numHits > 0 ? 10 : 1) * Projectile.scale, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
