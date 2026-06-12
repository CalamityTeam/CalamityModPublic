using System;
using System.Collections.Generic;
using CalamityMod.Cooldowns;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Player;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class CountermeasureMittHoldout : BaseGunHoldoutProjectile
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override int AssociatedItemID => ModContent.ItemType<CountermeasureMitt>();

        public float offsetBase = 0;
        public override float MaxOffsetLengthFromArm => offsetBase;
        public override float RecoilResolveSpeed => 0.4f;
        public override float OffsetXUpwards => 0f;
        public override float BaseOffsetY => 0f;
        public override float OffsetYDownwards => 0f;
        public override float WeaponTurnSpeed => 0.5f;
        public bool forcePalm => Projectile.ai[2] != 0; // If you're performing a palm blast or not
        public ref float shootingTimer => ref Projectile.ai[0];
        public ref float fireRate => ref Projectile.ai[1];
        public int time = 0;

        // Finger lerps and rotations
        #region Fingers
        public float rotSpeed = 0;
        public float f1l => 0.02f * rotSpeed; // Finger Lerp Speed
        public float f1r = 0; // Finger Rotation
        public float f1x = 0; // Finger Effects Multiplier
        public float f1d = -0.95f; // Finger Aim Direction
        public SlotId AudSlot1; // Finger Laser Audio Slot
        public float f2l => 0.016f * rotSpeed;
        public float f2r = 0;
        public float f2x = 0;
        public float f2d = -0.95f;
        public SlotId AudSlot2;
        public float f3l => 0.012f * rotSpeed;
        public float f3r = 0;
        public float f3x = 0;
        public float f3d = -0.95f;
        public SlotId AudSlot3;
        public float f4l => 0.008f * rotSpeed;
        public float f4r = 0;
        public float f4x = 0;
        public float f4d = -0.95f;
        public SlotId AudSlot4;
        public float f5l => 0.004f * rotSpeed;
        public float f5r = 0;
        public float f5x = 0;
        public float f5d = -0.95f;
        public SlotId AudSlot5;
        #endregion

        public int palmAnimTime = 20; // Animation length for the palm attack
        public int cooldownGiven = 400; // Arsenal Cooldown from the palm attack
        public int attackTime = 100; // Attack cycle length
        public int attackCycles = 0;
        public bool failedManaCheck = false;
        public bool canHit = false;
        public float alteredRotationRot = 0.7f; // Alteration to the holdout rotation so it's not directly to the mouse, changes with the attack cycle
        public float alteredRotation => Projectile.rotation + (f2r * alteredRotationRot * -Projectile.direction);
        public Vector2 palmBlastPos => Owner.Center + alteredRotation.ToRotationVector2() * 55 * Projectile.scale;
        public int palmManaCost => HeldItem.mana * 15;
        public int hitTimer = 0;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 55;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }
        public override void KillHoldoutLogic()
        {

        }
        public override bool? CanDamage() => null;
        public override bool? CanHitNPC(NPC target)
        {
            return (canHit ? null : false);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!canHit) return false;
            float realRot = alteredRotation;
            Vector2 handPos = Owner.GetBackHandPosition(CompositeArmStretchAmount.None, Owner.compositeBackArm.rotation) + (Owner.compositeBackArm.rotation + MathHelper.PiOver2).ToRotationVector2() * 9;
            
            // Every laser has collision for the single hitbox, so it works more like a large wide piercing single beam
            float beamLength = 2000 * Projectile.scale;
            float beamThickness = 20 * Projectile.scale;
            float fingerLength = 6 * Projectile.scale;
            float _ = float.NaN;

            bool colCheck = false;

            #region 5 Collision Line Checks
            for (int t = 0; t < 5; t++)
            {
                float rot = t switch
                {
                    1 => f4r * Projectile.direction,
                    2 => f3r * Projectile.direction,
                    3 => f2r * Projectile.direction,
                    4 => f1r * Projectile.direction,
                    _ => -f5r * 0.3f * Projectile.direction,
                };
                float fx = t switch
                {
                    1 => f4x, 2 => f3x, 3 => f2x, 4 => f1x, _ => f5x,
                };

                Vector2 basePosition = handPos + alteredRotation.ToRotationVector2() * (t == 0 ? 2.5f : 3.3f) * Projectile.scale + (alteredRotation + rot).ToRotationVector2() * fingerLength + (alteredRotation + MathHelper.PiOver2 * -Projectile.direction).ToRotationVector2() * (t == 0 ? -3 : 3) * Projectile.scale;
                if (fx > 0.2f)
                    colCheck = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), basePosition, basePosition + (realRot + rot).ToRotationVector2() * (fingerLength + beamLength), beamThickness, ref _);
                if (colCheck)
                    t = 5;
            }
            #endregion

            return (colCheck);
        }
        public void EndSounds()
        {
            if (SoundEngine.TryGetActiveSound(AudSlot1, out var s1))
                s1?.Stop();
            if (SoundEngine.TryGetActiveSound(AudSlot2, out var s2))
                s2?.Stop();
            if (SoundEngine.TryGetActiveSound(AudSlot3, out var s3))
                s3?.Stop();
            if (SoundEngine.TryGetActiveSound(AudSlot4, out var s4))
                s4?.Stop();
            if (SoundEngine.TryGetActiveSound(AudSlot5, out var s5))
                s5?.Stop();
        }
        public override void HoldoutAI()
        {
            if (hitTimer > 0)
                hitTimer--;

            if (Owner.dead)
            {
                EndSounds();
                Projectile.Kill();
                return;
            }

            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (alteredRotation + MathHelper.PiOver2 * -Projectile.direction) + (Owner.direction == -1 ? MathHelper.Pi : 0));

            Vector2 handPos = Owner.GetBackHandPosition(CompositeArmStretchAmount.None, Owner.compositeBackArm.rotation) + (Owner.compositeBackArm.rotation + MathHelper.PiOver2).ToRotationVector2() * 4;
            float armRotation = (Utils.DirectionTo(Owner.Center, handPos).ToRotation() - MathHelper.PiOver2) * Owner.gravDir + (Owner.gravDir == -1 ? MathHelper.Pi : 0f);
            Owner.SetCompositeArmFront(true, FrontArmStretch, armRotation + ExtraFrontArmRotation * Projectile.direction);

            // Below you see the sacrifice of coding 5 individual fingers
            if (time == 0 && forcePalm)
            {
                rotSpeed = 7f;
                alteredRotationRot = 0;
            }
            if (time == 1)
                rotSpeed = 0;

            if (forcePalm) alteredRotationRot = MathHelper.Lerp(alteredRotationRot, 0, 0.1f);
            else if (shootingTimer > -10) alteredRotationRot = MathHelper.Lerp(alteredRotationRot, 0.7f, 0.1f);

            #region The Price of 5 Fingers
            float fxPower = 5;
            if (!forcePalm)
            {
                if (shootingTimer == 10) f1d *= -1;
                if (shootingTimer == 20) f2d *= -1;
                if (shootingTimer == 30) f3d *= -1;
                if (shootingTimer == 40) f4d *= -1;
                if (shootingTimer == 50) f5d *= -1;
                if (attackCycles == 0)
                {
                    if (shootingTimer == 10) f1x = fxPower;
                    if (shootingTimer == 20) f2x = fxPower;
                    if (shootingTimer == 30) f3x = fxPower;
                    if (shootingTimer == 40) f4x = fxPower;
                    if (shootingTimer == 50) f5x = fxPower;
                }
            }

            f1r = MathHelper.Lerp(f1r, f1d, f1l * rotSpeed);
            f2r = MathHelper.Lerp(f2r, f2d * 0.775f, f2l * rotSpeed);
            f3r = MathHelper.Lerp(f3r, f3d * 0.55f, f3l * rotSpeed);
            f4r = MathHelper.Lerp(f4r, f4d * 0.325f, f4l * rotSpeed);
            f5r = MathHelper.Lerp(f5r, f5d * 2.3f - 2, f5l * 4 * rotSpeed);

            float goalValue = (canHit ? 1 : 0);
            float lerpPower = (forcePalm ? 0.35f : canHit ? 0.2f : 0.25f);
            f1x = MathHelper.Lerp(f1x, (f1x > 0.05f ? goalValue : 0), lerpPower);
            f2x = MathHelper.Lerp(f2x, (f2x > 0.05f ? goalValue : 0), lerpPower);
            f3x = MathHelper.Lerp(f3x, (f3x > 0.05f ? goalValue : 0), lerpPower);
            f4x = MathHelper.Lerp(f4x, (f4x > 0.05f ? goalValue : 0), lerpPower);
            f5x = MathHelper.Lerp(f5x, (f5x > 0.05f ? goalValue : 0), lerpPower);

            float sine = (float)Math.Sin(time * 0.2f / MathHelper.Pi) * 0.1f;
            float maxVol = 0.4f;
            float minPitch = -0.8f;
            if (SoundEngine.TryGetActiveSound(AudSlot1, out var f1s) && f1s.IsPlaying)
            {
                f1s.Position = Projectile.Center;
                f1s.Pitch = Utils.Remap(f1x, 0.7f, 1, 0f + minPitch, 0f) + sine;
                f1s.Volume = Utils.Remap(f1x, 0, 1, 0f, maxVol) * 100;
            }
            else if (f1x > 1)
            {
                SoundStyle beam = new("CalamityMod/Sounds/Item/MittWelding/Weld1");
                AudSlot1 = SoundEngine.PlaySound(beam with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
            }
            if (SoundEngine.TryGetActiveSound(AudSlot2, out var f2s) && f2s.IsPlaying)
            {
                f2s.Position = Projectile.Center;
                f2s.Pitch = Utils.Remap(f2x, 0.7f, 1, 0.2f + minPitch, 0.2f) + sine;
                f2s.Volume = Utils.Remap(f2x, 0, 1, 0f, maxVol) * 100;
            }
            else if (f2x > 1)
            {
                SoundStyle beam = new("CalamityMod/Sounds/Item/MittWelding/Weld2");
                AudSlot2 = SoundEngine.PlaySound(beam with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
            }
            if (SoundEngine.TryGetActiveSound(AudSlot3, out var f3s) && f3s.IsPlaying)
            {
                f3s.Position = Projectile.Center;
                f3s.Pitch = Utils.Remap(f3x, 0.7f, 1, 0.4f + minPitch, 0.4f) + sine;
                f3s.Volume = Utils.Remap(f3x, 0, 1, 0f, maxVol) * 100;
            }
            else if (f3x > 1)
            {
                SoundStyle beam = new("CalamityMod/Sounds/Item/MittWelding/Weld3");
                AudSlot3 = SoundEngine.PlaySound(beam with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
            }
            if (SoundEngine.TryGetActiveSound(AudSlot4, out var f4s) && f4s.IsPlaying)
            {
                f4s.Position = Projectile.Center;
                f4s.Pitch = Utils.Remap(f4x, 0.7f, 1, 0.6f + minPitch, 0.6f) + sine;
                f4s.Volume = Utils.Remap(f4x, 0, 1, 0f, maxVol) * 100;
            }
            else if (f4x > 1)
            {
                SoundStyle beam = new("CalamityMod/Sounds/Item/MittWelding/Weld4");
                AudSlot4 = SoundEngine.PlaySound(beam with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
            }
            if (SoundEngine.TryGetActiveSound(AudSlot5, out var f5s) && f5s.IsPlaying)
            {
                f5s.Position = Projectile.Center;
                f5s.Pitch = Utils.Remap(f5x, 0.7f, 1, 0.8f + minPitch, 0.8f) + sine;
                f5s.Volume = Utils.Remap(f5x, 0, 1, 0f, maxVol) * 100;
            }
            else if (f5x > 1)
            {
                SoundStyle beam = new("CalamityMod/Sounds/Item/MittWelding/Weld5");
                AudSlot5 = SoundEngine.PlaySound(beam with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
            }
            #endregion

            Lighting.AddLight(Projectile.Center, Color.Lerp(Effects.ArsenalEffects.ArsenalLaserColor, Color.White, 0.3f).ToVector3() * MathHelper.Clamp((f1x + f2x + f3x + f4x + f5x - 4f) * 2f, 0f, 0.7f));
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 1, 0.1f);

            if (time == 0)
            {
                if (!forcePalm)
                    shootingTimer = -40;
                if (Projectile.ai[2] != 0)
                    Projectile.ai[2] = palmAnimTime;
                fireRate = attackTime;
                OffsetLengthFromArm = offsetBase;
            }
            if (shootingTimer < 0)
            {
                canHit = false;
                // Light cooldown before firing
                if (rotSpeed > 0)
                    rotSpeed -= 0.05f;

                if (shootingTimer == -1 || forcePalm)
                {
                    if ((!Main.mouseLeft && !forcePalm) || failedManaCheck)
                    {
                        EndSounds();
                        Projectile.Kill();
                        return;
                    }

                    attackCycles = 0;
                    fireRate = attackTime;
                }

            }
            if (Owner.Calamity().mouseRight && Owner.Calamity().arsenalCooldown <= 0 && !forcePalm && !failedManaCheck)
            {
                Projectile.ai[2] = 5;
                shootingTimer = 0;
            }
            if (forcePalm)
            {
                if (!Owner.CheckMana(HeldItem, palmManaCost, false, false))
                {
                    failedManaCheck = true;
                    SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = -0.5f }, Projectile.Center);
                    Projectile.ai[2] = 0;
                    return;
                }
                rotSpeed = 5;

                f1d = -Math.Abs(f1d); f2d = -Math.Abs(f2d); f3d = -Math.Abs(f3d); f4d = -Math.Abs(f4d); f5d = -Math.Abs(f5d);

                if (shootingTimer <= palmAnimTime)
                {
                    if (shootingTimer == 1)
                    {
                        SoundStyle aud = new("CalamityMod/Sounds/Item/MittReadyPalm");
                        SoundEngine.PlaySound(aud with { Volume = 1f, Pitch = 0 }, Projectile.Center);
                    }
                    if (shootingTimer == (int)(palmAnimTime * 0.8f))
                    {
                        SoundStyle aud = new("CalamityMod/Sounds/Item/MittThrust");
                        SoundEngine.PlaySound(aud with { Volume = 1f, Pitch = 0 }, Projectile.Center);
                    }
                    if (shootingTimer == palmAnimTime)
                    {
                        Shoot();
                        Projectile.ai[2] = 0;
                        shootingTimer = -30;
                        rotSpeed = 0;
                    }
                    Lighting.AddLight(palmBlastPos, Color.Lerp(Effects.ArsenalEffects.ArsenalLaserColor, Color.White, 0.3f).ToVector3() * (float)Math.Pow(Utils.GetLerpValue(0, palmAnimTime, shootingTimer), 2));
                }
                if (shootingTimer > palmAnimTime * 0.8f)
                    Projectile.scale *= 1.25f;
                canHit = false;
            }
            else if (shootingTimer >= 0)
            {
                if (!Main.mouseLeft)
                    shootingTimer = fireRate;

                if (shootingTimer < fireRate && !failedManaCheck)
                {
                    canHit = true;
                    if (time % 3 == 0)
                    {
                        if (Owner.CheckMana(HeldItem, -1, true, false))
                        {
                            if (rotSpeed < 1.1f)
                                rotSpeed += 0.1f;
                        }
                        else
                        {
                            SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = -0.5f }, Projectile.Center);
                            failedManaCheck = true;
                        }
                    }
                }
                else
                {
                    if (attackCycles < 1 && !failedManaCheck)
                    {
                        attackCycles++;
                        shootingTimer = 0;
                    }
                    else
                        shootingTimer = -30;
                }
            }

            time++;
            shootingTimer++;
        }
        public void Shoot()
        {
            bool hitAnything = false;
            bool oneFx = true;
            float damageMult = 25;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.dontTakeDamage && Utils.Distance(npc.Center, palmBlastPos) <= (80 + Math.Max(npc.width / 2, npc.height / 2)))
                {
                    hitAnything = true;
                    Vector2 pos = Vector2.Lerp(npc.Center, Projectile.Center, 0.35f);
                    Vector2 vel = Vector2.Lerp(Projectile.Center.DirectionTo(npc.Center), alteredRotation.ToRotationVector2(), 0.5f);
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, vel, ModContent.ProjectileType<CountermeasurePalmBlast>(), (int)(Projectile.damage * damageMult), 0, Projectile.owner, 0, npc.whoAmI);
                    if (oneFx)
                    {
                        for (int i = 0; i < 25; i++)
                        {
                            float size = Main.rand.NextFloat(0.4f, 1f);
                            Dust dust = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(10, 10), ModContent.DustType<SquashDust>(), -vel.SafeNormalize(Vector2.UnitX).RotatedBy(0.95f * (Main.rand.NextBool() ? 1 : -1)) * Main.rand.NextFloat(4f, 18f) * size, 0, default, Main.rand.NextFloat(1.1f, 1.9f) + (1 - size));
                            dust.noGravity = true;
                            dust.color = Effects.ArsenalEffects.ArsenalLaserColor;
                            dust.fadeIn = 0.7f;
                        }
                        for (int i = 0; i < 25; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(20, 20), Effects.ArsenalEffects.ArsenalLaserDust, vel.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(10f, 45f), 0, default, Main.rand.NextFloat(1f, 1.8f));
                            dust.noGravity = true;
                            dust.color = Effects.ArsenalEffects.ArsenalLaserColor;
                            dust.alpha = 100;
                            dust.fadeIn = 20;
                            if (i % 2 == 0)
                            {
                                Dust dust2 = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(20, 20), Effects.ArsenalEffects.ArsenalLaserDust, vel.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(40f, 65f), 0, default, Main.rand.NextFloat(0.6f, 1.1f));
                                dust2.noGravity = true;
                                dust2.color = Effects.ArsenalEffects.ArsenalLaserColor;
                                dust2.alpha = 100;
                                dust2.fadeIn = 10;
                            }
                        }
                        for (int i = 0; i < 2; i++)
                        {
                            Particle pulse = new CustomSpark(pos, vel.SafeNormalize(Vector2.UnitX) * 2, "CalamityMod/Particles/BloomRing", false, 20, 1f, Effects.ArsenalEffects.ArsenalLaserColor, new Vector2(2f, 0.7f), shrinkSpeed: -0.2f);
                            GeneralParticleHandler.SpawnParticle(pulse);
                            Particle pulse1 = new CustomSpark(pos, vel.SafeNormalize(Vector2.UnitX) * 18, "CalamityMod/Particles/BloomRing", false, 18, 0.8f, Effects.ArsenalEffects.ArsenalLaserColor, new Vector2(2f, 0.7f), shrinkSpeed: -0.2f);
                            GeneralParticleHandler.SpawnParticle(pulse1);
                            Particle pulse2 = new CustomSpark(pos, vel.SafeNormalize(Vector2.UnitX) * 34, "CalamityMod/Particles/BloomRing", false, 16, 0.6f, Effects.ArsenalEffects.ArsenalLaserColor, new Vector2(2f, 0.7f), shrinkSpeed: -0.2f);
                            GeneralParticleHandler.SpawnParticle(pulse2);
                        }
                        Particle bloom = new CustomSpark(pos, vel.SafeNormalize(Vector2.UnitX) * 8, "CalamityMod/Particles/BloomCircle", false, 22, 0.7f, Effects.ArsenalEffects.ArsenalLaserColor, new Vector2(1f, 8f), true, true, shrinkSpeed: -0.5f, extraRotation: MathHelper.PiOver2);
                        GeneralParticleHandler.SpawnParticle(bloom);
                        oneFx = false;
                    }
                    damageMult -= 6;
                    if (damageMult < 1)
                        damageMult = 1;
                }
            }
            if (hitAnything && Owner.CheckMana(HeldItem, palmManaCost, true, false))
            {
                Owner.Calamity().arsenalCooldown = cooldownGiven;
                Owner.AddCooldown(ArsenalPower.ID, cooldownGiven);

                SoundStyle aud = new("CalamityMod/Sounds/Item/MittHit");
                for (int i = 0; i < 2; i++)
                    SoundEngine.PlaySound(aud with { Volume = 0.8f, Pitch = 0, MaxInstances = 2 }, Projectile.Center);
                Owner.SetScreenshake(8f);
            }
            else
            {
                Owner.Calamity().arsenalCooldown = (int)(cooldownGiven / 10);
                Owner.AddCooldown(ArsenalPower.ID, (int)(cooldownGiven / 10));

                for (int i = 0; i < 15; i++)
                {
                    Dust dust = Dust.NewDustPerfect(palmBlastPos, ModContent.DustType<SquashDust>(), Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(4f, 7f), 0, default, Main.rand.NextFloat(0.7f, 0.85f));
                    dust.noGravity = false;
                    dust.color = Effects.ArsenalEffects.ArsenalLaserColor;
                }
                for (int i = 0; i < 2; i++)
                {
                    Particle bloom = new CustomSpark(palmBlastPos, Vector2.Zero, "CalamityMod/Particles/BloomCircle", false, 15, 0.5f, Effects.ArsenalEffects.ArsenalLaserColor, new Vector2(1f, 1f), true, true);
                    GeneralParticleHandler.SpawnParticle(bloom);
                }
                SoundStyle aud = new("CalamityMod/Sounds/Item/MittFail");
                SoundEngine.PlaySound(aud with { Volume = 1f, Pitch = 0 }, Projectile.Center);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (hitTimer == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(target.Center, Effects.ArsenalEffects.ArsenalLaserDust, Owner.Center.DirectionTo(target.Center).RotatedByRandom(0.3f) * Main.rand.NextFloat(9f, 13f), 0, default, Main.rand.NextFloat(0.6f, 1.1f));
                    dust.noGravity = true;
                    dust.color = Effects.ArsenalEffects.ArsenalLaserColor;
                    dust.alpha = 100;
                    dust.fadeIn = 10;
                }
                hitTimer = Projectile.localNPCHitCooldown;
            }
            else
            {
                Dust dust = Dust.NewDustPerfect(target.Center, Effects.ArsenalEffects.ArsenalLaserDust, Owner.Center.DirectionTo(target.Center).RotatedByRandom(0.2f) * Main.rand.NextFloat(6f, 9f), 0, default, Main.rand.NextFloat(0.3f, 0.7f));
                dust.noGravity = true;
                dust.color = Effects.ArsenalEffects.ArsenalLaserColor;
                dust.alpha = 100;
                dust.fadeIn = 10;
                modifiers.SourceDamage *= 0.2f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 handPos = Owner.GetBackHandPosition(CompositeArmStretchAmount.None, Owner.compositeBackArm.rotation) + (Owner.compositeBackArm.rotation + MathHelper.PiOver2).ToRotationVector2() * 9 * Projectile.scale;
            Projectile.Center = handPos;

            if (time < 2)
                return false;

            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/CountermeasureMittHoldout").Value;

            Texture2D finger = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/CountermeasureMittFinger2").Value;
            Texture2D laser = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLineFade").Value;
            Texture2D laser2 = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomLineThick").Value;
            Texture2D bloom = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            Texture2D diamond = ModContent.Request<Texture2D>("CalamityMod/Particles/SquareRotated").Value;

            Vector2 drawPosition = handPos - Main.screenPosition;
            Vector2 vel = alteredRotation.ToRotationVector2();

            float randSize = Main.rand.NextFloat(0.8f, 1.3f);
            float fadeIn = (float)Math.Pow(Utils.GetLerpValue(0, palmAnimTime, shootingTimer), 2);
            Color drawColor = Projectile.GetAlpha(lightColor);
            float drawRotation = alteredRotation + (Projectile.direction == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            float exRot = (Projectile.direction == -1 ? MathHelper.PiOver2 : 0);
            SpriteEffects flipSprite = Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float thrustCuttoff = palmAnimTime * 0.8f;
            float pullBack = MathHelper.Lerp(0.2f, 1f, (float)Math.Pow(Utils.GetLerpValue(thrustCuttoff, 0, shootingTimer, true), 4));
            float pushOut = MathHelper.Lerp(0.2f, 1.5f, (float)Math.Pow(Utils.GetLerpValue(thrustCuttoff, palmAnimTime, shootingTimer, true), 5));
            float thrustMult = (float)(forcePalm ? (shootingTimer <= thrustCuttoff ? pullBack : pushOut) : 1);
            Vector2 thrustAddition = vel * 10 * thrustMult - vel * 10;

            Main.EntitySpriteDraw(texture, drawPosition + thrustAddition, null, drawColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            
            for (int i = 0; i < 5; i++)
            {
                float fingerLength = 3f * Projectile.scale;
                Vector2 fingerScale = new Vector2(1f, 1f) * Projectile.scale;
                Vector2 fingerOrgin = new Vector2(0, finger.Height / 2);

                float rot = i switch
                {
                    1 => f4r * Projectile.direction,
                    2 => f3r * Projectile.direction,
                    3 => f2r * Projectile.direction,
                    4 => f1r * Projectile.direction,
                    _ => -f5r * 0.3f * Projectile.direction,
                };
                float fx = i switch
                {
                    1 => f4x, 2 => f3x, 3 => f2x, 4 => f1x, _ => f5x,
                };
                Color fingerColor = Color.Lerp(drawColor, Color.Black, (4 - i) * 0.2f);
                if (i == 0)
                {
                    fingerScale = new Vector2(0.9f, 1f) * Projectile.scale;
                    fingerLength = 1f * Projectile.scale;
                    fingerColor = drawColor;
                }

                Vector2 basePosition = handPos + vel * (i == 0 ? 2.5f : 3.3f) * Projectile.scale + thrustAddition + (alteredRotation + rot).ToRotationVector2() * fingerLength + (alteredRotation + MathHelper.PiOver2 * -Projectile.direction).ToRotationVector2() * (i == 0 ? -3 : 3) * Projectile.scale;
                Main.EntitySpriteDraw(finger, basePosition - Main.screenPosition, null, fingerColor, alteredRotation + rot, fingerOrgin, fingerScale, SpriteEffects.None);

                Vector2 velocity = (alteredRotation + rot).ToRotationVector2();
                float laserLength = 2.5f;
                Main.EntitySpriteDraw(laser, basePosition - Main.screenPosition + velocity * 542 * laserLength * Projectile.scale, null, Effects.ArsenalEffects.ArsenalLaserColor with { A = 0 } * fx, velocity.ToRotation() + MathHelper.PiOver2, laser.Size() / 2, new Vector2(2.5f * randSize * fx, 55 * laserLength) * Projectile.scale * 0.01f, SpriteEffects.FlipVertically);
                Main.EntitySpriteDraw(laser2, basePosition - Main.screenPosition + velocity * 542 * laserLength * Projectile.scale, null, Color.Lerp(Effects.ArsenalEffects.ArsenalLaserColor, Color.White, 0.3f) with { A = 0 } * fx, velocity.ToRotation() + MathHelper.PiOver2, laser2.Size() / 2, new Vector2(0.4f * Math.Min(fx, 1), 55 * laserLength) * Projectile.scale * 0.01f, SpriteEffects.FlipVertically);

                for (int y = 0; y < 2; y++)
                    Main.EntitySpriteDraw(bloom, basePosition - Main.screenPosition + velocity * 7 * Projectile.scale, null, Color.Lerp(Effects.ArsenalEffects.ArsenalLaserColor, Color.White, y) with { A = 0 } * (fx - 1), velocity.ToRotation() + MathHelper.PiOver2, bloom.Size() / 2, new Vector2(2 + 0.05f * (fx + 25), 1) * Projectile.scale * MathHelper.Lerp(fx, 1, 0.7f) * (0.03f - 0.01f * y), SpriteEffects.FlipVertically);
            }
            if (forcePalm)
            {
                for (int y = 0; y < 8; y++)
                    Main.EntitySpriteDraw(bloom, palmBlastPos - Main.screenPosition + thrustAddition, null, Color.Lerp(Effects.ArsenalEffects.ArsenalLaserColor, Color.White, y * 0.1f) with { A = 0 } * fadeIn * 0.25f, alteredRotation + Main.rand.NextFloat(-4, 4), bloom.Size() / 2, new Vector2(0.4f, 1.8f) * Projectile.scale * (0.3f - 0.02f * y) * randSize, SpriteEffects.FlipVertically);
                Main.EntitySpriteDraw(diamond, palmBlastPos - Main.screenPosition + thrustAddition, null, Color.White with { A = 0 } * fadeIn, 0, diamond.Size() / 2, new Vector2(1.7f * Main.rand.NextFloat(1, 1.6f) * (float)Math.Pow(fadeIn, 3), 0.06f) * Projectile.scale * 0.3f, SpriteEffects.FlipVertically);
                Main.EntitySpriteDraw(bloom, palmBlastPos - Main.screenPosition + thrustAddition, null, Effects.ArsenalEffects.ArsenalLaserColor with { A = 0 } * fadeIn * 0.35f, 0, bloom.Size() / 2, new Vector2(1.8f, 0.5f) * Projectile.scale * 0.4f * randSize, SpriteEffects.FlipVertically);
            }
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
    }
}
