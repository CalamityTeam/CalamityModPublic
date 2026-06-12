using System;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    [PierceResistException]
    public class HellkiteHoldout : BaseCustomUseStyleProjectile, ILocalizedModType
    {
        public override int AssignedItemID => ModContent.ItemType<Hellkite>();

        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Hellkite>();
        public override string Texture => "CalamityMod/Items/Weapons/Melee/Hellkite";
        public override float HitboxOutset => 100;

        public override Vector2 HitboxSize => new Vector2(185, 185);
        public override float HitboxRotationOffset => MathHelper.ToRadians(-45);

        public override Vector2 SpriteOrigin => new(-3, 124);
        public Vector2 mousePos;
        public Vector2 aimVel;
        public bool doSwing = false;
        public bool postSwing = false;
        public float fadeIn = 0; // Used to make particle effects and outer glow on the sword fade in and out
        public int useAnim; // Used as your use time stat since checking the held item use time gets jank if your attack speed changes mid swing
        public int storedUseAnim; // Used to check your use time when you began using the item and to reset use time when needed
        public int swingCount = 0;
        public int pierceReduction = 0; // Used to reduce damage when striking many enemies with a single swing

        public bool chargedSwing = false; // True if you have a charged swing fully charged
        public int chargeTimer = 0; // Timer for charging the blade with right click
        public int chargeTimerMax = 240; // This is set to be base don use time on spawn
        public bool playSwingSound = true;

        public SlotId AudSlot;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
        }
        public override void WhenSpawned()
        {
            CanHit = false;
            Projectile.knockBack = 0;
            Projectile.ai[1] = -1;

            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, as Hellkite has no projectiles
            mousePos = Owner.Calamity().mouseWorld;
            aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
            useAnim = Owner.itemAnimationMax;
            storedUseAnim = useAnim;

            chargeTimerMax = useAnim * 5; // Max charge time is set here

            if (mousePos.X < Owner.Center.X) Owner.direction = -1;
            else Owner.direction = 1;

            FlipAsSword = Owner.direction == -1 ? true : false;
        }
        public override void UseStyle()
        {
            AnimationProgress = Animation % (chargedSwing ? (int)(storedUseAnim * 0.7f) : storedUseAnim);

            if (Owner.Calamity().mouseRight)
                Projectile.ai[2] = 5;

            DrawUnconditionally = false;
            bool cantUse = (Owner == null || !Owner.active || Owner.dead || (Projectile.ai[2] == 0 && !Owner.channel) || (Projectile.ai[2] == 5 && !Owner.Calamity().mouseRight) || Owner.CCed || Owner.noItems);

            if (CanHit || postSwing)
                mousePos = Owner.Center - aimVel;
            else
            {
                mousePos = Owner.Calamity().mouseWorld;
            }

            if (CanHit)
                fadeIn = MathHelper.Lerp(fadeIn, 1, (chargedSwing ? 1.5f : 1) * 0.23f * Owner.GetAttackSpeed<MeleeDamageClass>());
            else
                fadeIn = MathHelper.Lerp(fadeIn, 0, 0.3f);
            if (chargeTimer > 0)
                fadeIn = Utils.Remap(chargeTimer, 0, chargeTimerMax, 0, 1f);

            // If you are no longer holding the charge, then stop charge counter so you can swing
            if (cantUse)
            {
                chargeTimer = 0;
                if (Projectile.ai[2] == 5)
                {
                    Owner.itemAnimation = Owner.itemAnimationMax;
                    Projectile.timeLeft = Owner.itemAnimation;
                }
                Projectile.ai[2] = 0;
            }

            if (!doSwing)
            {
                playSwingSound = true;
                mousePos = Owner.Calamity().mouseWorld;
                aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                CanHit = false;
                if (mousePos.X < Owner.Center.X) Owner.direction = -1;
                else Owner.direction = 1;
                FlipAsSword = Owner.direction == -1 ? true : false;

                Vector2 bladePos = new Vector2(60, 0);
                Vector2 particlePos = Owner.Center + (bladePos).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)) * Projectile.scale;

                if (Projectile.ai[2] == 5)
                {
                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(120f * Projectile.ai[1] * Owner.direction), 0.05f);

                    float rotationValue = 45f + (25 * Utils.GetLerpValue(0, chargeTimerMax, chargeTimer, true)) * (FlipAsSword ? 1 : -1) * -Projectile.ai[1];
                    Projectile.rotation = Projectile.rotation.AngleLerp(Owner.AngleTo(mousePos) + MathHelper.ToRadians(rotationValue), 0.3f);
                    Animation = 0;
                    Owner.itemAnimation++;
                    Projectile.timeLeft++;

                    if (chargeTimer < chargeTimerMax && !chargedSwing)
                        chargeTimer++;

                    Vector2 particleVel = (Owner.Center - particlePos).SafeNormalize(Vector2.UnitX) * -15;
                    particlePos += Main.rand.NextVector2Circular(20, 20);

                    Dust dust2 = Dust.NewDustPerfect(particlePos, DustID.RainbowMk2, particleVel * Main.rand.NextFloat(0.2f, 1));
                    dust2.scale = Main.rand.NextFloat(0.65f, 1.15f) * fadeIn;
                    dust2.noGravity = true;
                    dust2.color = Main.rand.NextBool(3) ? Color.Orange : Color.OrangeRed;
                    Particle spark3 = new GlowOrbParticle(particlePos, particleVel * Main.rand.NextFloat(0.2f, 1.5f), false, 22, Main.rand.NextFloat(0.2f, 0.4f) * fadeIn, Main.rand.NextBool(3) ? Color.Red : Color.OrangeRed);
                    GeneralParticleHandler.SpawnParticle(spark3);

                    if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                    {
                        ChargeSound.Position = Projectile.Center;
                        ChargeSound.Pitch = Utils.Remap(chargeTimer, 0, chargeTimerMax, -0.4f, 0f);
                        ChargeSound.Volume = Utils.Remap(chargeTimer, 0, chargeTimerMax, 0f, 0.5f) * 100;
                    }
                    else if (!chargedSwing)
                    {
                        AudSlot = SoundEngine.PlaySound(Hellkite.ChargeSound with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
                    }
                }
                if (chargeTimer == chargeTimerMax)
                {
                    particlePos = Owner.Center + (bladePos).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)) * Projectile.scale;
                    SoundEngine.PlaySound(Hellkite.FullChargeSound with { Volume = 0.9f, PitchVariance = 0.2f }, Projectile.Center);
                    chargedSwing = true;
                    useAnim = storedUseAnim / 3;
                    chargeTimer++;

                    for (int i = 0; i < 20; i++)
                    {
                        Particle spark2 = new LineParticle(particlePos, new Vector2(8, 8).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1f), false, 30, Main.rand.NextFloat(0.3f, 0.8f) * Projectile.scale, Main.rand.NextBool(3) ? Color.Red : Color.OrangeRed);
                        GeneralParticleHandler.SpawnParticle(spark2);
                        Dust dust2 = Dust.NewDustPerfect(particlePos, DustID.RainbowMk2, new Vector2(8, 8).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1f));
                        dust2.scale = Main.rand.NextFloat(0.65f, 1.15f) * fadeIn;
                        dust2.noGravity = true;
                        dust2.color = Main.rand.NextBool(3) ? Color.Orange : Color.OrangeRed;
                    }
                }

                if (chargeTimer == 0)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                        Projectile.localNPCImmunity[i] = 0;

                    Projectile.numHits = 0;
                    pierceReduction = 0;
                    doSwing = true;
                }
            }
            else if (chargeTimer == 0)
            {
                if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound))
                    ChargeSound?.Stop();

                if (!CanHit && !postSwing)
                {
                    if (mousePos.X < Owner.Center.X) Owner.direction = -1;
                    else Owner.direction = 1;
                }
                else
                {
                    if ((Owner.Center - aimVel).X < Owner.Center.X) Owner.direction = -1;
                    else Owner.direction = 1;
                }
                
                Projectile.rotation = Projectile.rotation.AngleLerp(Owner.AngleTo(mousePos) + MathHelper.ToRadians(45f), 0.1f);

                if (AnimationProgress < (useAnim / 1.5f))
                {
                    if (Projectile.ai[2] == 5 && !chargedSwing)
                        doSwing = false;

                    aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                    CanHit = false;
                    postSwing = false;
                    if (AnimationProgress == 0)
                    {
                        Animation = 0;
                        doSwing = false;
                        chargeTimer = 0;
                        chargedSwing = false;
                        useAnim = storedUseAnim;
                        Projectile.ai[1] = -Projectile.ai[1];
                    }

                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(120f * Projectile.ai[1] * Owner.direction * (1 + (Utils.GetLerpValue(useAnim * 0.35f, useAnim * 0.6f, Animation, true)) * 0.5f)), 0.2f);
                    FlipAsSword = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX).X > 0 ? true : false;
                }
                else
                {
                    float time = (AnimationProgress) - (useAnim / 3);
                    float timeMax = useAnim - (useAnim / 3);

                    if (time >= (int)(timeMax * (chargedSwing ? 0.2f : 0.4f)) && playSwingSound)
                    {
                        if (!chargedSwing)
                        {
                            SoundEngine.PlaySound(Hellkite.SwingSound with { Volume = 0.8f, PitchVariance = 0.25f }, Projectile.Center);
                        }
                        else
                        {
                            SoundEngine.PlaySound(Hellkite.SwingSound with { Volume = 0.9f, Pitch = 0.2f }, Projectile.Center);
                            SoundEngine.PlaySound(Hellkite.SwingSoundBig with { Volume = 1f, Pitch = 0f }, Projectile.Center);
                        }
                        swingCount++;
                        playSwingSound = false;
                    }
                    if (time > (int)(timeMax * (chargedSwing ? 0.2f : 0.4f)) && time < (int)(timeMax * (chargedSwing ? 0.9f : 0.7f)))
                    {
                        CanHit = true;

                        Vector2 particleVel = new Vector2(0, 10 * -Projectile.ai[1] * Owner.direction).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));
                        Vector2 particlePos = Owner.Center + (new Vector2(Main.rand.Next(30, 170), 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45))) * Projectile.scale;
                        if (chargedSwing)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                particleVel = (new Vector2(0, 15 * -Projectile.ai[1] * Owner.direction) * Main.rand.NextFloat(0.3f, 1f)).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));
                                particlePos = Owner.Center + (new Vector2(Main.rand.Next(30, 170), 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45))) * Projectile.scale;
                                GeneralParticleHandler.SpawnParticle(new AltSparkParticle(particlePos, -particleVel.RotatedByRandom(0.4f), false, 24, Main.rand.NextFloat(0.3f, 0.7f), Main.rand.NextBool(3) ? Color.OrangeRed : Color.DarkRed));
                                GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(particlePos, -particleVel.RotatedByRandom(0.4f), Main.rand.NextBool(4) ? Color.Red : Color.DarkRed, 23, Main.rand.NextFloat(0.5f, 1f), 0.65f));
                                GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(particlePos, -particleVel.RotatedByRandom(0.4f) * 2, Main.rand.NextBool(4) ? Color.Crimson : Color.Red, 23, Main.rand.NextFloat(0.5f, 1f), 0.65f, 0, true));
                            }
                        }
                        else
                        {
                            GeneralParticleHandler.SpawnParticle(new AltSparkParticle(particlePos, -particleVel.RotatedByRandom(0.2f), false, 24, Main.rand.NextFloat(0.3f, 0.7f), Main.rand.NextBool(3) ? Color.OrangeRed : Color.DarkRed));
                            GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(particlePos, -particleVel.RotatedByRandom(0.2f) * 2, Main.rand.NextBool(4) ? Color.Red : Color.DarkRed, 23, Main.rand.NextFloat(0.5f, 1f), 0.65f));
                        }
                    }
                    else
                    {
                        CanHit = false;
                    }

                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(MathHelper.Lerp(150f * Projectile.ai[1] * Owner.direction, 120f * -Projectile.ai[1] * Owner.direction, CalamityUtils.ExpInOutEasing(time / timeMax, 1))),
                        0.2f);

                    if (time < (int)(timeMax * 0.9f))
                    {
                        postSwing = true;
                    }

                    if (CanHit)
                    {
                        if (chargedSwing)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                float randRot = Main.rand.NextFloat(-10, -45);
                                Vector2 dustVel = (new Vector2(0, 15 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));
                                GeneralParticleHandler.SpawnParticle(new PointParticle(Owner.Center + (new Vector2(170 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot)).RotatedByRandom(0.4f)), -dustVel * Main.rand.NextFloat(0.4f, 0.7f), false, Main.rand.Next(15, 18 + 1), Main.rand.NextFloat(0.7f, 1.2f), (Main.rand.NextBool(4) ? Color.Orange : Color.OrangeRed) * 0.8f));
                            }
                            for (int i = 0; i < 4; i++)
                            {
                                float randRot = Main.rand.NextFloat(-30, -60);
                                Vector2 dustVel = (new Vector2(0, 15 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));
                                Dust dust2 = Dust.NewDustPerfect(Owner.Center + (new Vector2(170 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f)), DustID.FireworksRGB, dustVel * Main.rand.NextFloat(0.3f, 0.9f));
                                dust2.scale = Main.rand.NextFloat(0.65f, 0.95f);
                                dust2.noGravity = true;
                                dust2.color = Main.rand.NextBool(3) ? Color.Orange : Color.OrangeRed;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                float randRot = Main.rand.NextFloat(-30, -60);
                                Vector2 dustVel = (new Vector2(0, 15 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));
                                Dust dust2 = Dust.NewDustPerfect(Owner.Center + (new Vector2(170 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f)), DustID.FireworksRGB, dustVel * Main.rand.NextFloat(0.1f, 0.5f));
                                dust2.scale = Main.rand.NextFloat(0.55f, 0.85f);
                                dust2.noGravity = true;
                                dust2.color = Main.rand.NextBool(3) ? Color.Orange : Color.OrangeRed;
                            }
                        }
                    }   
                }
            }

            ArmRotationOffset = MathHelper.ToRadians(-140f);
            ArmRotationOffsetBack = MathHelper.ToRadians(-140f);
        }
        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound))
                ChargeSound?.Stop();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // If you are hitting an armored target or kill a target, don't reduce damage based on enemy hits (which uses Projectile.numHits)
            if ((damageDone <= 2 || (target.life <= 0 && target.realLife == -1)) && pierceReduction > 0)
            {
                pierceReduction -= 1;
            }

            if (!chargedSwing)
            {
                if (Projectile.numHits == 0)
                {
                    SoundEngine.PlaySound(Hellkite.HitSoundSmall with { Volume = 0.85f, PitchVariance = 0.25f }, Projectile.Center);
                    Owner.SetScreenshake(4.5f);
                }
                for (int i = 0; i < MathHelper.Clamp(8 - Projectile.numHits * 2, 2, 8); i++)
                {
                    Particle spark2 = new LineParticle(target.Center, ((Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitY) * -20).RotatedByRandom(0.7) * Main.rand.NextFloat(0.2f, 1f), false, 40, Main.rand.NextFloat(0.3f, 1f), Main.rand.NextBool(3) ? Color.Orange : Color.OrangeRed);
                    GeneralParticleHandler.SpawnParticle(spark2);
                    if (Main.rand.NextBool())
                    {
                        Particle spark3 = new AltLineParticle(target.Center, ((Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitY) * -20).RotatedByRandom(0.7) * Main.rand.NextFloat(0.2f, 1f), false, 40, Main.rand.NextFloat(0.3f, 1f), Color.DarkRed);
                        GeneralParticleHandler.SpawnParticle(spark3);
                    }
                }
            }
            else
            {
                if (Projectile.numHits == 0)
                {
                    SoundEngine.PlaySound(Hellkite.HitSoundBig with { Volume = 1f }, Projectile.Center);
                    Owner.SetScreenshake(8.5f);
                    bool photos = CalamityClientConfig.Instance.Photosensitivity;

                    if (!photos)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Particle blastRing = new CustomPulse(target.Center, Vector2.Zero, Color.OrangeRed, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 2f * (i + 1), 1f, 18, true);
                            GeneralParticleHandler.SpawnParticle(blastRing);
                            Particle blastRing2 = new CustomPulse(target.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 1f * (i + 1), 0.5f, 18, true);
                            GeneralParticleHandler.SpawnParticle(blastRing2);
                        }
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        Particle spark = new GlowSparkParticle(target.Center, (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitY) * -25 * (i == 0 ? -1 : 1), false, 12, 0.08f, Color.OrangeRed, new Vector2(3, 0.8f), true);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                    for (int i = 0; i < 15; i++)
                    {
                        Particle spark2 = new SparkParticle(target.Center, ((Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitY) * -40).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1f), true, 40, Main.rand.NextFloat(0.5f, 1.2f), Main.rand.NextBool(3) ? Color.Orange : Color.OrangeRed);
                        GeneralParticleHandler.SpawnParticle(spark2);
                        if (Main.rand.NextBool())
                        {
                            Particle spark3 = new AltSparkParticle(target.Center, ((Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitY) * -40).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1f), true, 40, Main.rand.NextFloat(0.5f, 1.2f), Color.DarkRed);
                            GeneralParticleHandler.SpawnParticle(spark3);
                        }
                        Dust dust2 = Dust.NewDustPerfect(target.Center, DustID.FireworksRGB, new Vector2(20, 20).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1));
                        dust2.scale = Main.rand.NextFloat(0.55f, 0.85f);
                        dust2.noGravity = true;
                        dust2.color = Main.rand.NextBool(3) ? Color.Orange : Color.OrangeRed;
                    }
                }
            }

            Vector2 launchVel = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
            target.MoveNPC(launchVel, (chargedSwing ? 24 : 19), true, Owner);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (chargedSwing)
                modifiers.ApplyScalingForcedCrit(Projectile);

            float minMult = 0.25f;
            int hitsToMinMult = 10;
            float damageMult = Utils.Remap(pierceReduction, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= (chargedSwing ? 2.9f : 1) * damageMult;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // Only draw the projectile if the projectile's owner is currently using the item this projectile is attached to.
            if ((useAnim > 0 || DrawUnconditionally) && (Owner.ItemAnimationActive || Owner.Calamity().mouseRight))
            {
                Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
                Asset<Texture2D> glowTex = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/HellkiteGlow");
                Asset<Texture2D> shineTex = ModContent.Request<Texture2D>("CalamityMod/Particles/Sparkle");
                Asset<Texture2D> swoosh = ModContent.Request<Texture2D>("CalamityMod/Particles/VerticalSmearRagged");

                float r = FlipAsSword ? MathHelper.ToRadians(90) : 0f;
                Vector2 generalDrawPos = Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY);
                SpriteEffects sEffects = spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

                for (int i = 0; i < 25; i++)
                {
                    Texture2D centerTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/HellkiteGhost").Value;
                    Color auraColor = Color.OrangeRed with { A = 0 } * 0.15f * fadeIn;
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 25f).ToRotationVector2() * (chargeTimer > 0 ? 4 : 7) * fadeIn;
                    Main.EntitySpriteDraw(centerTexture, Projectile.Center - Main.screenPosition + drawOffset + new Vector2(0, Owner.gfxOffY), centerTexture.Frame(1, FrameCount, 0, Frame), auraColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                }

                if (swingCount > 0 && Projectile.ai[2] != 5 && !playSwingSound)
                    Main.EntitySpriteDraw(swoosh.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), null, Color.Lerp(Color.DarkRed, Color.OrangeRed, 0.25f) with { A = 0 } * fadeIn * 0.9f, (FinalRotation + MathHelper.ToRadians(45)) + MathHelper.ToRadians(swingCount % 2 == 0 ? -80 : 80) * -Owner.direction, swoosh.Size() * 0.5f, Projectile.scale * 2.35f / 4, swingCount % 2 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

                Main.EntitySpriteDraw(tex.Value, generalDrawPos, tex.Frame(1, FrameCount, 0, Frame), lightColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, sEffects);

                if (chargeTimer > 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Color auraColor = (Color.Lerp(Color.DarkRed, Color.Red, Utils.GetLerpValue(0, 5, i)) * 0.4f * fadeIn) with { A = 0 };
                        Vector2 rotationalDrawOffset = (MathHelper.TwoPi * i / 7f + Main.GlobalTimeWrappedHourly * 17f).ToRotationVector2();
                        rotationalDrawOffset *= MathHelper.Lerp(3f, 5.25f, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 13f) * 0.5f);
                        Main.EntitySpriteDraw(glowTex.Value, Projectile.Center - Main.screenPosition + rotationalDrawOffset + new Vector2(0, Owner.gfxOffY), glowTex.Value.Frame(1, FrameCount, 0, Frame), auraColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, sEffects);
                    }
                }
                Main.EntitySpriteDraw(glowTex.Value, generalDrawPos, glowTex.Frame(1, FrameCount, 0, Frame), chargeTimer > 0 ? Color.Lerp(Color.White, Color.Gold, fadeIn) : Color.White, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(glowTex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, sEffects);
            }
            return false;
        }
        public override void ResetStyle()
        {
        }
    }
}
