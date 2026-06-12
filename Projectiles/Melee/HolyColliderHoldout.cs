using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Boss;
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
    public class HolyColliderHoldout : BaseCustomUseStyleProjectile, ILocalizedModType
    {
        public override int AssignedItemID => ModContent.ItemType<HolyCollider>();

        public override LocalizedText DisplayName => CalamityUtils.GetItemName<HolyCollider>();
        public override string Texture => "CalamityMod/Items/Weapons/Melee/HolyCollider";
        public int size = 146;
        public override float HitboxOutset => size * 0.85f;
        public override Vector2 HitboxSize => new Vector2(size, size);
        public override Vector2 SpriteOrigin => new(0, size);
        public override float HitboxRotationOffset => MathHelper.ToRadians(-45);
        public Vector2 mousePos;
        public Vector2 aimVel;
        public bool doSwing = false;
        public bool postSwing = false;
        public float fadeIn = 0; // Used to make particle effects and outer glow on the sword fade in and out
        public int useAnim; // Used as your use time stat since checking the held item use time gets jank if your attack speed changes mid swing
        public int storedUseAnim; // Used to check your use time when you began using the item and to reset use time when needed
        public int swingCount = -1; // Runs counting code first, so it has to be one below

        public int pierceReduction = 0; // Used to reduce damage when striking many enemies with a single swing

        public bool chargedSwing = false; // True if you have a charged swing fully charged
        public int chargeTimer = 0; // Timer for charging the blade
        public int chargeTimerMax = 240; // This is set to be base don use time on spawn

        public Color mainColor1 = Color.Goldenrod;
        public Color mainColor2 = Color.OrangeRed;
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
            Projectile.knockBack = 0;
            Projectile.ai[1] = -1;

            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, it does not influence Holy Collider's projectile spawning
            mousePos = Owner.Calamity().mouseWorld;
            aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
            useAnim = Owner.itemAnimationMax;
            storedUseAnim = useAnim;

            chargeTimerMax = (int)(useAnim * 1.1f); // Max charge time is set here

            if (mousePos.X < Owner.Center.X) Owner.direction = -1;
            else Owner.direction = 1;

            FlipAsSword = Owner.direction == -1 ? true : false;
        }
        public override void UseStyle()
        {
            AnimationProgress = Animation % (chargedSwing ? (int)(storedUseAnim * 1.2f) : storedUseAnim);

            DrawUnconditionally = false;
            bool cantUse = ( Owner == null || !Owner.active || Owner.dead || Main.mouseLeftRelease || Owner.CCed || Owner.noItems);

            if (CanHit || postSwing)
                mousePos = Owner.Center - aimVel;
            else
            {
                mousePos = Owner.Calamity().mouseWorld;
            }

            if (CanHit)
                fadeIn = MathHelper.Lerp(fadeIn, 1, 0.1f);
            else
                fadeIn = MathHelper.Lerp(fadeIn, 0, 0.15f);
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
            else
                Projectile.ai[2] = 5;

            if (!doSwing)
            {
                mousePos = Owner.Calamity().mouseWorld;
                aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                CanHit = false;
                if (mousePos.X < Owner.Center.X) Owner.direction = -1;
                else Owner.direction = 1;
                FlipAsSword = Owner.direction == -1 ? true : false;

                Vector2 bladePos = new Vector2(76 * Projectile.scale, 0);
                Vector2 particlePos = Owner.Center + (bladePos).RotatedBy(FinalRotation + MathHelper.ToRadians(-45) - 0.1f * (FlipAsSword ? 1 : -1) * -Projectile.ai[1]);

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

                    Vector2 particleVel = (Owner.Center - particlePos).SafeNormalize(Vector2.UnitX) * -9 * Projectile.scale;

                    Dust dust2 = Dust.NewDustPerfect(particlePos, ModContent.DustType<SquashDust>(), particleVel.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.5f, 1.4f));
                    dust2.scale = Main.rand.NextFloat(1.45f, 1.95f) * fadeIn * Projectile.scale;
                    dust2.noGravity = true;
                    dust2.color = Main.rand.NextBool(3) ? mainColor2 : mainColor1;
                    dust2.fadeIn = Projectile.scale * 1.5f;

                    Particle spark3 = new LineParticle(particlePos, particleVel.RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 0.6f), false, 18, Main.rand.NextFloat(0.5f, 0.8f) * fadeIn * Projectile.scale, Main.rand.NextBool(3) ? mainColor2 : mainColor1);
                    GeneralParticleHandler.SpawnParticle(spark3);

                    if (SoundEngine.TryGetActiveSound(AudSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                    {
                        ChargeSound.Position = Projectile.Center;
                        ChargeSound.Pitch = Utils.Remap(chargeTimer, 0, chargeTimerMax, -0.4f, 0f);
                        ChargeSound.Volume = Utils.Remap(chargeTimer, 0, chargeTimerMax, 0f, 0.5f) * 100;
                    }
                    else if (!chargedSwing)
                    {
                        SoundStyle burn = new("CalamityMod/Sounds/Custom/Providence/ProvidenceBurnLoop");
                        AudSlot = SoundEngine.PlaySound(burn with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
                    }
                }
                if (chargeTimer == chargeTimerMax)
                {
                    particlePos = Owner.Center + (bladePos).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));

                    SoundStyle fullCharge = new("CalamityMod/Sounds/Custom/Providence/ProvidenceBurn");
                    SoundEngine.PlaySound(fullCharge with { Volume = 0.7f, Pitch = 0.5f }, Projectile.Center);

                    chargedSwing = true;
                    useAnim = storedUseAnim / 2;
                    chargeTimer++;

                    for (int i = 0; i < 20; i++)
                    {
                        Particle spark2 = new LineParticle(particlePos, new Vector2(8, 8).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1f), false, 30, Main.rand.NextFloat(0.3f, 0.8f) * Projectile.scale, Main.rand.NextBool(3) ? mainColor2 : mainColor1);
                        GeneralParticleHandler.SpawnParticle(spark2);
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

                if (AnimationProgress < (useAnim / 1.6f))
                {
                    if (Projectile.ai[2] == 5 && !chargedSwing)
                        doSwing = false;

                    playSwingSound = true;
                    aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                    CanHit = false;
                    postSwing = false;
                    if (AnimationProgress == 0)
                    {
                        Animation = 0;
                        doSwing = false;
                        chargeTimer = 0;
                        chargedSwing = false;
                        swingCount++;
                        useAnim = storedUseAnim;
                        Projectile.ai[1] = -Projectile.ai[1];
                    }
                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(120f * Projectile.ai[1] * Owner.direction * (1 + (Utils.GetLerpValue(useAnim * 0.15f, useAnim * 0.35f, Animation, true)) * 0.35f)), 0.2f);
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
                            SoundStyle swing = new("CalamityMod/Sounds/Custom/Providence/ProvidenceHolyBlastShoot");
                            SoundEngine.PlaySound(swing with { Volume = 0.7f, Pitch = 0.35f }, Projectile.Center);
                            SoundStyle swing2 = new("CalamityMod/Sounds/Item/SwingMid");
                            SoundEngine.PlaySound(swing2 with { Volume = 0.55f, Pitch = -0.15f }, Projectile.Center);
                        }
                        else
                        {
                            SoundStyle swing = new("CalamityMod/Sounds/Custom/ProfanedGuardians/GuardianShieldDeactivate");
                            SoundEngine.PlaySound(swing with { Volume = 0.8f, Pitch = -0.35f }, Projectile.Center);
                            SoundStyle swing2 = new("CalamityMod/Sounds/Item/SwingMid");
                            SoundEngine.PlaySound(swing2 with { Volume = 0.9f, Pitch = -0.55f }, Projectile.Center);
                        }
                        playSwingSound = false;
                    }
                    if (time > (int)(timeMax * (chargedSwing ? 0.1f : 0.3f)) && time < (int)(timeMax * (chargedSwing ? 0.95f : 0.85f)))
                    {
                        CanHit = true;

                        Vector2 particleVel = new Vector2(0, 2 * -Projectile.ai[1] * Owner.direction).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));
                        Vector2 particlePos = Owner.Center + (new Vector2(Main.rand.Next(30, 170) * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)));
                        if (!chargedSwing)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                particleVel = (new Vector2(0, 15 * -Projectile.ai[1] * Owner.direction) * Main.rand.NextFloat(0.3f, 1f)).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));
                                particlePos = Owner.Center + (new Vector2(Main.rand.Next(30, 170) * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)));
                                if (i < 2)
                                {
                                    Dust dust2 = Dust.NewDustPerfect(particlePos, ModContent.DustType<SquashDust>(), -particleVel.RotatedByRandom(0.3f));
                                    dust2.scale = Main.rand.NextFloat(0.95f, 1.45f) * Projectile.scale;
                                    dust2.noGravity = true;
                                    dust2.color = Main.rand.NextBool(3) ? mainColor2 : mainColor1;
                                    dust2.fadeIn = Projectile.scale - 1;
                                }
                                else
                                {
                                    Particle spark = new CustomSpark(particlePos, (-particleVel * 0.2f).RotatedByRandom(0.3f), "CalamityMod/Particles/ProvidenceMarkParticle", false, 17, Main.rand.NextFloat(0.9f, 1.1f) * Projectile.scale, Color.Lerp(Color.Orchid, Color.White, Main.rand.NextFloat(0, 0.7f)), new Vector2(1.3f, 0.5f), true, false, 0, false, false, Main.rand.NextFloat(0.1f, 0.2f));
                                    GeneralParticleHandler.SpawnParticle(spark);
                                }
                            }
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
                                GeneralParticleHandler.SpawnParticle(new CustomSpark(Owner.Center + (new Vector2(170 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot)).RotatedByRandom(0.4f)), -dustVel * Main.rand.NextFloat(0.4f, 0.7f), "CalamityMod/Particles/LargeBloom", false, Main.rand.Next(7, 9 + 1), Main.rand.NextFloat(0.3f, 0.35f) * Projectile.scale, (Main.rand.NextBool(4) ? Color.DarkGoldenrod : Color.Goldenrod), new Vector2(1f, 1.2f), true, false, 0, false, false, 0.45f));
                                if (i % 3 == 0)
                                {
                                    Particle spark = new CustomSpark(Owner.Center + (new Vector2(170 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot)).RotatedByRandom(0.4f)), -dustVel * Main.rand.NextFloat(0.4f, 0.7f), "CalamityMod/Particles/ProvidenceMarkParticle", false, 27, Main.rand.NextFloat(1.15f, 1.3f) * Projectile.scale, Main.rand.NextBool(4) ? Color.Khaki : Color.Orange, new Vector2(1.3f, 0.5f), true, false, 0, false, false, Main.rand.NextFloat(0.1f, 0.2f));
                                    GeneralParticleHandler.SpawnParticle(spark);
                                }
                            }
                            for (int i = 0; i < 6; i++)
                            {
                                float randRot = Main.rand.NextFloat(-30, -60);
                                Vector2 dustVel = (new Vector2(0, 35 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));
                                Dust dust2 = Dust.NewDustPerfect(Owner.Center + (new Vector2(170 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f)), ModContent.DustType<SquashDust>(), dustVel * Main.rand.NextFloat(0.2f, 0.6f));
                                dust2.scale = Main.rand.NextFloat(1.35f, 1.85f) * Projectile.scale;
                                dust2.noGravity = true;
                                dust2.color = Main.rand.NextBool(3) ? mainColor2 : mainColor1;
                                dust2.fadeIn = Projectile.scale * 0.3f;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                float randRot = Main.rand.NextFloat(-30, -60);
                                Vector2 dustVel = -(new Vector2(0, 25 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));
                                Dust dust2 = Dust.NewDustPerfect(Owner.Center + (new Vector2(170 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.15f)), ModContent.DustType<SquashDust>(), dustVel * Main.rand.NextFloat(0.1f, 0.5f));
                                dust2.scale = Main.rand.NextFloat(0.75f, 0.9f) * Projectile.scale;
                                dust2.noGravity = true;
                                dust2.color = Main.rand.NextBool(3) ? mainColor2 : mainColor1;
                                dust2.fadeIn = Projectile.scale - 1;
                            }
                        }

                        if (chargedSwing)
                        {
                            for (int x = 0; x < Main.maxProjectiles; x++)
                            {
                                Projectile projectile = Main.projectile[x];
                                bool isProviProj = (projectile.type == ModContent.ProjectileType<HolyFire>() ||
                                    projectile.type == ModContent.ProjectileType<HolyFire2>() ||
                                    projectile.type == ModContent.ProjectileType<HolyFlare>() ||
                                    projectile.type == ModContent.ProjectileType<HolyBomb>() ||
                                    projectile.type == ModContent.ProjectileType<HolyBlast>());
                                bool isAFireball = (projectile.active && projectile.type == ModContent.ProjectileType<HolyColliderHolyFire>() && projectile.ai[0] != 10) || isProviProj;
                                if (Vector2.Distance(Owner.Center + (new Vector2(60 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45))), projectile.Center) <= 150 * Projectile.scale + Math.Max(projectile.width, projectile.height) && projectile.active && isAFireball)
                                {
                                    if (isProviProj)
                                    {
                                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), projectile.Center, (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitY) * -2.5f, ModContent.ProjectileType<HolyColliderHolyFire>(), (int)((Projectile.damage * 0.1) + projectile.damage), Projectile.knockBack, Projectile.owner, 0, 15, 5);
                                        proj.localAI[0] = Projectile.scale;
                                        Particle orb2 = new CustomPulse(projectile.Center, Vector2.Zero, Color.Goldenrod, "CalamityMod/Particles/BloomRing", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0, 0.8f * Projectile.scale, 11);
                                        GeneralParticleHandler.SpawnParticle(orb2);
                                        if (projectile.type == ModContent.ProjectileType<HolyBlast>())
                                            projectile.active = false;
                                        else
                                            projectile.Kill();
                                    }
                                    else
                                    {
                                        projectile.ai[2] = 5;
                                        projectile.owner = Owner.whoAmI;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int x = 0; x < Main.maxProjectiles; x++)
                            {
                                Projectile projectile = Main.projectile[x];
                                bool isAFireball = (projectile.active && projectile.type == ModContent.ProjectileType<HolyColliderHolyFire>() && projectile.ai[0] != 10);
                                if (Vector2.Distance(Owner.Center + (new Vector2(60 * Projectile.scale, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45))), projectile.Center) <= 150 * Projectile.scale + Math.Max(projectile.width, projectile.height) && isAFireball)
                                {
                                    Vector2 launch = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitY) * -2.5f;
                                    projectile.velocity += launch;
                                    projectile.timeLeft = 300;
                                    projectile.owner = Owner.whoAmI;
                                    Particle orb2 = new CustomPulse(projectile.Center, Vector2.Zero, Color.Goldenrod, "CalamityMod/Particles/BloomRing", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0, 0.8f * Projectile.scale, 11);
                                    GeneralParticleHandler.SpawnParticle(orb2);
                                }
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
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 90);

            // If you are hitting an armored target or kill a target, don't reduce damage based on enemy hits (which uses Projectile.numHits)
            if ((damageDone <= 2 || (target.life <= 0 && target.realLife == -1)) && pierceReduction > 0)
            {
                pierceReduction -= 1;
            }

            if (!chargedSwing)
            {
                if (Projectile.numHits == 0)
                {
                    SoundStyle hitSound = new("CalamityMod/Sounds/Item/HolyColliderSmallHit");
                    SoundEngine.PlaySound(hitSound with { Volume = 0.85f, PitchVariance = 0.25f }, Projectile.Center);

                    for (int i = 0; i < 6; i++)
                    {
                        // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                        Projectile fadedFire = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, ((Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitY) * -11).RotatedByRandom(0.6f) * Main.rand.NextFloat(0.3f, 1f), ModContent.ProjectileType<HolyColliderHolyFire>(), (int)(Projectile.damage * 0.01), Projectile.knockBack, Projectile.owner, 10, target.whoAmI);
                        fadedFire.timeLeft = Main.rand.Next(40, 55 + 1);
                        fadedFire.localAI[0] = Projectile.scale;
                    }
                }
            }
            else
            {
                if (Projectile.numHits == 0)
                {
                    SoundStyle hitSound = new("CalamityMod/Sounds/Item/HolyColliderBigHit");
                    SoundEngine.PlaySound(hitSound with { Volume = 1f, PitchVariance = 0.15f }, Projectile.Center);

                    float starAngle = MathHelper.ToRadians(45f);
                    for (int i = 0; i < 4; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB);
                        Vector2 vel = (MathHelper.TwoPi * i / 4f).ToRotationVector2().RotatedBy(starAngle) * 4f;

                        Particle pulse = new CustomSpark(target.Center, vel, "CalamityMod/Particles/BloomCircle", false, 12, 1.2f * Projectile.scale, Color.Orange, new Vector2(3.2f, 0.9f), true, true, shrinkSpeed: 0.95f);
                        GeneralParticleHandler.SpawnParticle(pulse);
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, (new Vector2(0, -35).RotatedBy(MathHelper.ToRadians(45f))).RotatedBy(MathHelper.ToRadians(90f) * i), ModContent.ProjectileType<HolyColliderHolyFire>(), (int)(Projectile.damage * 0.1), Projectile.knockBack, Projectile.owner, 0);
                        proj.localAI[0] = Projectile.scale;
                    }
                }
            }

            Vector2 launchVel = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
            target.MoveNPC(launchVel, (chargedSwing ? 35 : 23), true, Owner);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.25f;
            int hitsToMinMult = 10;
            float damageMult = Utils.Remap(pierceReduction, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= (chargedSwing ? 1f : 0.2f) * damageMult;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // Only draw the projectile if the projectile's owner is currently using the item this projectile is attached to.
            if ((useAnim > 0 || DrawUnconditionally) && (Owner.ItemAnimationActive))
            {
                Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
                Asset<Texture2D> glowTex = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/HolyColliderGlow");
                bool flipAsSword = (swingCount % 2 == 0 ? !FlipAsSword : FlipAsSword);
                float r = flipAsSword ? MathHelper.ToRadians(90) : 0f;
                Vector2 generalDrawPos = Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY);
                SpriteEffects sEffects = spriteEffects != SpriteEffects.None ? spriteEffects : (flipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

                for (int i = 0; i < 25; i++)
                {
                    Texture2D centerTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/HolyColliderGhost").Value;
                    Color auraColor = mainColor1 with { A = 0 } * 0.15f * fadeIn;
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 25f).ToRotationVector2() * 6 * fadeIn;
                    Main.EntitySpriteDraw(centerTexture, Projectile.Center - Main.screenPosition + drawOffset + new Vector2(0, Owner.gfxOffY), centerTexture.Frame(1, FrameCount, 0, Frame), auraColor, Projectile.rotation + RotationOffset + r, flipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (flipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                }

                Main.EntitySpriteDraw(tex.Value, generalDrawPos, tex.Frame(1, FrameCount, 0, Frame), lightColor, Projectile.rotation + RotationOffset + r, flipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, sEffects);
                Main.EntitySpriteDraw(glowTex.Value, generalDrawPos, glowTex.Frame(1, FrameCount, 0, Frame), Color.White, Projectile.rotation + RotationOffset + r, flipAsSword ? new Vector2(glowTex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, sEffects);
            }
            else
            {
                chargeTimer = 0;
                chargedSwing = false;
            }
            return false;
        }
        public override void ResetStyle()
        {
        }
    }
}
