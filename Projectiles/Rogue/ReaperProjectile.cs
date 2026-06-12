using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Rogue
{
    [PierceResistException]
    public class ReaperProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public int time = 0;
        public int ChargeupTime = 50;
        public int Lifetime = 500;
        public bool spinning = false;
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/TheOldReaper";
        public float OverallProgress => 1 - Projectile.timeLeft / (float)Lifetime;
        public float ThrowProgress => 1 - Projectile.timeLeft / (float)(Lifetime);
        public float ChargeProgress => 1 - (Projectile.timeLeft - Lifetime) / (float)(ChargeupTime);
        public Player Owner => Main.player[Projectile.owner];
        public SlotId SpinSoundSlot;

        Vector2 squash = Vector2.One;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = Lifetime + ChargeupTime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15 * Projectile.MaxUpdates;
            Projectile.DamageType = RogueDamageClass.Instance;
        }
        public override bool ShouldUpdatePosition()
        {
            return ChargeProgress >= 1;
        }

        public override bool? CanDamage()
        {
            //We don't want the anticipation to deal damage.
            if (ChargeProgress < 1)
                return false;

            return base.CanDamage();
        }
        //Swing animation keys
        public CurveSegment pullback = new CurveSegment(EasingType.PolyOut, 0f, 0f, MathHelper.PiOver4 * -1.2f, 2);
        public CurveSegment throwout = new CurveSegment(EasingType.PolyOut, 0.7f, MathHelper.PiOver4 * -1.2f, MathHelper.PiOver4 * 1.2f + MathHelper.PiOver2, 3);
        internal float ArmAnticipationMovement() => PiecewiseAnimation(ChargeProgress, new CurveSegment[] { pullback, throwout });
        public override void AI()
        {
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            Projectile.spriteDirection = Projectile.direction;

            if (SoundEngine.TryGetActiveSound(SpinSoundSlot, out var SpinSound) && SpinSound.IsPlaying)
                SpinSound.Position = Projectile.Center;

            //Anticipation animation. Make the player look like theyre holding the weapon
            if (ChargeProgress < 1)
            {
                Owner.ChangeDir(MathF.Sign(Main.MouseWorld.X - Owner.Center.X));

                float armRotation = ArmAnticipationMovement() * Owner.direction;

                Owner.heldProj = Projectile.whoAmI;
                Projectile.spriteDirection = Owner.direction;
                Projectile.direction = Owner.direction;

                Projectile.Center = Owner.MountedCenter + Vector2.UnitY.RotatedBy(armRotation * Owner.gravDir) * -70f * Owner.gravDir + new Vector2(14 * Owner.direction, 0);
                Projectile.rotation = (-MathHelper.PiOver4 * Projectile.direction + armRotation) * Owner.gravDir;

                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi + armRotation);

                time++;
                return;
            }

            //Play the throw sound when the throw ACTUALLY BEGINS.
            //Additionally, make the projectile collide and set its speed and velocity
            if (Projectile.timeLeft == Lifetime)
            {
                Projectile.netUpdate = true;

                SoundStyle fire = new("CalamityMod/Sounds/Item/SwingMid");
                SoundEngine.PlaySound(fire with { Volume = 0.8f, Pitch = Main.rand.NextFloat(0.2f, 0.3f) }, Projectile.Center);

                Projectile.Center = Owner.MountedCenter + Projectile.velocity * 4f;
                if (Projectile.Calamity().stealthStrike)
                {
                    Projectile.velocity = new Vector2(0.5f * Owner.direction, -1) * 18;
                }
                else
                {
                    // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for initial throw direction
                    Projectile.velocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX * Owner.direction) * 15;
                }
                Projectile.spriteDirection = Projectile.direction;
                SpinSoundSlot = SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/SpinningWoosh") with { Pitch = -0.3f, Volume = 0.75f }, Projectile.Center);
                
                time = 0;
                spinning = true;
            }

            if (Projectile.velocity.X > 0)
                Projectile.direction = 1;
            else
                Projectile.direction = -1;

            if (spinning)
            {
                if (Projectile.velocity.Length() < 15)
                    Projectile.velocity *= 1.01f;
                if (time == 0)
                    Projectile.rotation += Main.rand.NextFloat(0, 10) * Projectile.direction;
                squash = new Vector2(1.3f, 0.8f);
                Projectile.rotation += 0.2f * Projectile.direction;

                if (targetDist < 1400)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        float rot = Main.rand.NextFloat(-5.5f, 5.5f);
                        float scale = Main.rand.NextFloat(1f, 1.15f);
                        Particle Smear = new CustomPulse(Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 18, Projectile.velocity, Color.Chartreuse * Main.rand.NextFloat(0.68f, 0.75f), "CalamityMod/Particles/CircularSmearSmokey", squash.RotatedBy(rot), Projectile.velocity.ToRotation() + MathHelper.ToRadians(150f) - rot, scale, scale, 3);
                        GeneralParticleHandler.SpawnParticle(Smear);
                    }

                    if (time % 7 == 0)
                    {
                        Particle trail = new SparkParticle(Projectile.Center + Main.rand.NextVector2Circular(80, 80), -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.8f), false, 60, Main.rand.NextFloat(0.9f, 1.5f), Color.Chartreuse);
                        GeneralParticleHandler.SpawnParticle(trail);
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        float rot2 = (Projectile.rotation * Projectile.direction);
                        Vector2 dustPos = Projectile.Center + (i * MathHelper.Pi + Projectile.rotation * 0.3f + MathHelper.PiOver2).ToRotationVector2() * 70f;
                        Dust dust = Dust.NewDustPerfect(dustPos, DustID.RainbowMk2);
                        dust.noGravity = true;
                        dust.scale = 0.8f;
                        dust.color = Color.Chartreuse;
                        dust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.45f, 0.6f);
                    }
                    if (Main.rand.NextBool())
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(23, 23), Main.rand.NextBool(7) ? 28 : 215);
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(0.9f, 1.3f);
                        dust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.7f);
                    }
                }

                if (Projectile.Calamity().stealthStrike)
                {
                    if (time == 30)
                    {
                        SpinSound?.Stop();
                        SoundStyle fire = new("CalamityMod/Sounds/Item/RadiationBurst");
                        SpinSoundSlot = SoundEngine.PlaySound(fire with { Volume = 1f, Pitch = 0.3f }, Projectile.Center);
                    }
                    if (time >= 150)
                    {
                        NPC target = Owner.ClampedMouseWorld().ClosestNPCAt(2000);
                        if (time == 150)
                        {
                            Projectile.extraUpdates = 25;
                        }
                        if (target != null)
                        {
                            if (Projectile.numHits <= 0)
                                CalamityUtils.HomeInOnSelectedNPC(Projectile, target, true, 0.85f, 15, 0.97f);
                        }
                        else if (time == 150)
                        {
                            // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for immediate direction impulse
                            Projectile.velocity = (Owner.Calamity().mouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX * Owner.direction) * 15;
                        }
                        Particle spark = new GlowSparkParticle(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(-2, -1), -Projectile.velocity * 0.3f, false, 7, 0.13f, Color.Lerp(Color.Green, Color.Chartreuse, 0.8f) * 0.65f, new Vector2(1, 0.3f), true, false, 1.3f);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    else if (time > 30)
                    {
                        Projectile.velocity *= 0.955f;
                        float fade = Utils.GetLerpValue(150, 0, time);
                        float numberOfDusts = 2f;
                        float rotFactor = 360f / numberOfDusts;
                        for (int i = 0; i < numberOfDusts; i++)
                        {
                            float rot = MathHelper.ToRadians(i * rotFactor);
                            Vector2 velOffset = CalamityUtils.RandomVelocity(100f, 70f, 250f, 0.04f);
                            velOffset *= Main.rand.NextFloat(25, 45) * fade;
                            Particle energy = new GlowOrbParticle(Projectile.Center + velOffset * 2.5f, -velOffset * Main.rand.NextFloat(0.08f, 0.12f) * 1.5f, false, (int)(14 - (5 * fade)), Main.rand.NextFloat(1.1f, 1.25f) - 0.5f * fade, Color.Chartreuse);
                            GeneralParticleHandler.SpawnParticle(energy);
                            Dust dust = Dust.NewDustPerfect(Projectile.Center + velOffset * 2.5f, DustID.FireworksRGB, -velOffset * Main.rand.NextFloat(0.08f, 0.12f) * 1.5f, 0, default, Main.rand.NextFloat(0.4f, 0.6f));
                            dust.noGravity = true;
                            dust.color = Color.Chartreuse;
                        }
                    }
                }
            }

            time++;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.Calamity().stealthStrike && time <= 151)
                Projectile.numHits--;
            if (Projectile.numHits == 0)
            {
                if (Projectile.Calamity().stealthStrike) // Rainstorm
                {
                    Vector2 rainSpot = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 490;
                    SoundStyle fire = new("CalamityMod/Sounds/Item/RadiationRain");
                    SoundEngine.PlaySound(fire with { Volume = 1f, Pitch = 0 }, rainSpot);
                    SoundStyle fire2 = new("CalamityMod/Sounds/Item/ViperSpit");
                    SoundEngine.PlaySound(fire2 with { Volume = 1f, Pitch = -0.3f }, rainSpot);
                    for (int i = 0; i < 37; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(rainSpot, DustID.FireworksRGB);
                        chargefull.velocity = Projectile.velocity.RotatedByRandom(100) * Main.rand.NextFloat(1, 8);
                        chargefull.scale = Main.rand.NextFloat(0.45f, 0.75f);
                        chargefull.noGravity = true;
                        chargefull.color = Color.Lerp(Color.White, Main.rand.NextBool(4) ? Color.Green : Color.Chartreuse, 0.7f);
                    }
                    Particle Smear = new CustomPulse(rainSpot, Vector2.Zero, Color.Chartreuse * 0.7f, "CalamityMod/Particles/DustyCircleHardEdge", Vector2.One, Main.rand.NextFloat(-5, 5), 0, 0.35f, 12);
                    GeneralParticleHandler.SpawnParticle(Smear);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), rainSpot, Vector2.Zero, ModContent.ProjectileType<RadiationRain>(), (int)(Projectile.damage * 0.13), 0f, Projectile.owner, 0, 0, 100);
                }
                else // Radiation Burst
                {
                    SoundStyle fire = new("CalamityMod/Sounds/Item/RadiationBurst");
                    SoundEngine.PlaySound(fire with { Volume = 1f, Pitch = 0, MaxInstances = -1 }, Projectile.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + target.velocity * 32, Vector2.Zero, ModContent.ProjectileType<RadiationBurst>(), (int)(Projectile.damage), Projectile.knockBack * 3, Projectile.owner, 0, 0, 0);
                }
            }
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 90);

            float minMult = 0.1f;
            int hitsToMinMult = 5;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;
        }

        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(SpinSoundSlot, out var SpinSound))
                SpinSound?.Stop();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition + (!spinning ? new Vector2(0, Owner.gfxOffY) : Vector2.Zero);
            Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
            Main.EntitySpriteDraw(tex.Value, drawPos, null, spinning ? Color.White : Color.Lerp(lightColor, Color.White, Utils.GetLerpValue(0, ChargeupTime, time, true)), Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

            Asset<Texture2D> p = ModContent.Request<Texture2D>("CalamityMod/Particles/VerticalSmear");
            
            if (spinning)
            {
                Main.EntitySpriteDraw(p.Value, drawPos + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 4, null, Color.Chartreuse with { A = 0 } * 0.45f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, p.Size() * 0.5f, new Vector2(0.9f - 0.3f * Utils.GetLerpValue(25, 0, time, true), 1 + 0.6f * Utils.GetLerpValue(25, 0, time, true)) * Main.rand.NextFloat(1.25f, 1.4f), SpriteEffects.None);
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], Color.Chartreuse * 0.5f, 1);
            }
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 70, targetHitbox);
    }
}
