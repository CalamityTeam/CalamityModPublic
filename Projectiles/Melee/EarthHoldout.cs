using System;
using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    [PierceResistException]
    public class EarthHoldout : BaseCustomUseStyleProjectile, ILocalizedModType
    {
        public override int AssignedItemID => ModContent.ItemType<Earth>();

        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Earth>();
        public override string Texture => "CalamityMod/Items/Weapons/Melee/Earth";
        public int size = 186 + 30;
        public override float HitboxOutset => size * 0.85f;
        public override Vector2 HitboxSize => new Vector2(size, size) * (1 + bladeFade * 1.25f);
        public override Vector2 SpriteOrigin => new(0, size - 30);
        public override float HitboxRotationOffset => MathHelper.ToRadians(-45);

        public Vector2 mousePos;
        public Vector2 aimVel;
        public bool doSwing = true;
        public bool postSwing = false;
        public float fadeIn = 0;
        public int useAnim;
        public int swingCount;
        public bool spawnBoom = true;
        public Color mainColor = Color.OrangeRed;
        public bool finalFlip = false;
        public int pause = 0;
        public bool playSwingSound = true;
        public bool allowSecondHit = true;
        public float bladeFade = 0;
        public int armoredHits = 0;
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
            Projectile.ai[1] = 1;

            // 14NOV2024: Ozzatron: clamped mouse position unnecessary, it does not influence Earth's projectile spawning
            mousePos = Owner.Calamity().mouseWorld;
            aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
            useAnim = Owner.itemAnimationMax * 2;

            if (mousePos.X < Owner.Center.X) Owner.direction = -1;
            else Owner.direction = 1;

            FlipAsSword = Owner.direction == -1 ? true : false;
        }

        public override void UseStyle()
        {
            if (pause > 0)
            {
                pause--;
                Animation--;
                return;
            }

            AnimationProgress = Animation % useAnim;
            DrawUnconditionally = false;

            float rate = Main.GlobalTimeWrappedHourly * 12;
            List<Color> earthColors = new List<Color>()
            {
                Color.OrangeRed,
                Color.MediumTurquoise,
                Color.LimeGreen
            };

            int colorIndex = (int)(rate / 2 % earthColors.Count);
            Color currentColor = earthColors[colorIndex];
            Color nextColor = earthColors[(colorIndex + 1) % earthColors.Count];
            mainColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);

            if (CanHit || postSwing)
                mousePos = Owner.Center - aimVel;
            else
            {
                mousePos = Owner.Calamity().mouseWorld;
            }

            if (CanHit)
                fadeIn = MathHelper.Lerp(fadeIn, 1, 0.2f);
            else
                fadeIn = MathHelper.Lerp(fadeIn, 0, 0.28f);

            if (Projectile.ai[1] == -1)
                bladeFade = MathHelper.Lerp(bladeFade, 1, 0.15f);
            else
                bladeFade = MathHelper.Lerp(bladeFade, 0, 0.045f);

            if (!doSwing)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                    Projectile.localNPCImmunity[i] = 0;

                allowSecondHit = true;
                playSwingSound = true;
                spawnBoom = true;
                Projectile.numHits = 0;
                mousePos = Owner.Calamity().mouseWorld;
                aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                CanHit = false;
                if (mousePos.X < Owner.Center.X) Owner.direction = -1;
                else Owner.direction = 1;
                FlipAsSword = Owner.direction == -1 ? true : false;
                if (swingCount % 2 == 0)
                {
                    useAnim = Owner.itemAnimationMax * 2;
                }
                else
                {
                    useAnim = Owner.itemAnimationMax;
                }

                doSwing = true;
                finalFlip = false;
                armoredHits = 0;
            }
            else
            {
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
                if (AnimationProgress < (useAnim / (swingCount % 2 == 0 ? 1.3f : 7)))
                {
                    aimVel = (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                    CanHit = false;
                    postSwing = false;
                    if (AnimationProgress == 0)
                    {
                        Animation = 0;
                        doSwing = false;
                        Projectile.ai[1] = -Projectile.ai[1];
                    }
                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(120f * Projectile.ai[1] * Owner.direction * (1 + (Utils.GetLerpValue(useAnim * 0.35f, useAnim * 0.6f, Animation, true)) * 0.25f)), 0.2f);
                }
                else
                {
                    if (!finalFlip)
                    {
                        FlipAsSword = Owner.direction < 0 ? true : false;
                    }

                    float time = (AnimationProgress) - (useAnim / 2.5f);
                    float timeMax = useAnim - (useAnim / 2.5f);

                    if (time >= (int)(timeMax * 0.4f) && playSwingSound)
                    {
                        SoundStyle swing = new("CalamityMod/Sounds/Item/SwingMid");
                        SoundEngine.PlaySound(swing with { Volume = 0.8f, Pitch = (Projectile.ai[1] == 1 ? -0.4f : -0.1f) }, Projectile.Center);
                        SoundStyle swing2 = new("CalamityMod/Sounds/Item/HellkiteSwing", 2);
                        SoundEngine.PlaySound(swing2 with { Volume = 0.8f, Pitch = (Projectile.ai[1] == 1 ? 0.4f : 0.7f) }, Projectile.Center);
                        swingCount++;
                        playSwingSound = false;
                    }
                    if ((int)(time) % 2 == 0 && Projectile.ai[1] == 1 && !Main.dedServ)
                    {
                        SoundStyle swoosh = new("CalamityMod/Sounds/Item/SwooshMid");
                        SoundEngine.PlaySound(swoosh with { Volume = 1f, Pitch = -0.4f, MaxInstances = -1 }, Projectile.Center);
                    }
                    if (time > (int)(timeMax * 0.45f) && time < (int)(timeMax * 0.9f))
                    {
                        CanHit = true;

                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 particleVel = new Vector2(0, 10 * -Projectile.ai[1] * Owner.direction).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));
                            Vector2 particlePos = Owner.Center + (new Vector2(Main.rand.Next(30, (int)(170 * (1 + bladeFade * 0.6f))), 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45))) * Projectile.scale;
                            GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(particlePos, -particleVel.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.75f, 0.9f), false, 14, Main.rand.NextFloat(0.06f, 0.03f) * Projectile.scale, mainColor, new Vector2(1.3f, 0.2f), true, false, 0.55f));
                        }
                        for (int i = 0; i < 6; i++)
                        {
                            Vector2 particleVel = new Vector2(0, 10 * -Projectile.ai[1] * Owner.direction).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));
                            Vector2 particlePos = Owner.Center + (new Vector2(Main.rand.Next(30, (int)(270 * (1 + bladeFade * 0.6f))), 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45))) * Projectile.scale;
                            if (Main.rand.NextBool(3))
                            {
                                Particle sparker = new CustomSpark(particlePos + Main.rand.NextVector2Circular(15, 15), -particleVel * Main.rand.NextFloat(0.4f, 0.8f), "CalamityMod/Particles/Sparkle", false, 30, Main.rand.NextFloat(1.2f, 2.2f) * Projectile.scale, mainColor, new Vector2(0.4f, Main.rand.NextFloat(0.9f, 1.4f)), true, true);
                                GeneralParticleHandler.SpawnParticle(sparker);
                            }
                            else
                            {
                                GeneralParticleHandler.SpawnParticle(new CustomSpark(particlePos, -particleVel.RotatedByRandom(0.2f) * 2, "CalamityMod/Particles/LargeBloom", false, Main.rand.Next(7, 9 + 1), Main.rand.NextFloat(0.3f, 0.35f) * Projectile.scale, mainColor * 0.65f, new Vector2(1f, 1.2f), true, false, 0, false, false, 0.45f));
                            }
                        }
                    }
                    else
                        CanHit = false;

                    float start = swingCount % 2 != 0 ? (150 * Projectile.ai[1] * Owner.direction) : (150f * Projectile.ai[1] * Owner.direction);
                    float end = swingCount % 2 != 0 ? ((270) * -Projectile.ai[1] * Owner.direction) : (120f * -Projectile.ai[1] * Owner.direction);
                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(MathHelper.Lerp(start, end, CalamityUtils.ExpInOutEasing(time / timeMax, 1))), 0.2f);
                    if (time > timeMax * 0.8f)
                    {
                        RotationOffset = Utils.AngleLerp(RotationOffset, MathHelper.ToRadians(MathHelper.Lerp(start, end, CalamityUtils.ExpInOutEasing(time / timeMax, 1))), 0.2f);
                    }
                    if (time >= timeMax)
                        doSwing = false;
                    if (time < (int)(timeMax * 0.7f))
                    {
                        postSwing = true;
                    }
                    if (CanHit)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            float randRot = Main.rand.NextFloat(-30, -60);
                            Vector2 dustVel = (new Vector2(0, 10 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));

                            GenericSparkle sparker = new GenericSparkle(Owner.Center + (new Vector2(270 * (1 + bladeFade * 0.6f), 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f)) * Projectile.scale, Vector2.Zero, Color.White, mainColor, Main.rand.NextFloat(0.5f, 0.7f) * Projectile.scale, 11, Main.rand.NextFloat(-0.1f, 0.1f), 2.68f);
                            GeneralParticleHandler.SpawnParticle(sparker);

                            Dust dust2 = Dust.NewDustPerfect(Owner.Center + (new Vector2(270 * (1 + bladeFade * 0.6f), 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f)) * Projectile.scale, DustID.FireworksRGB, dustVel);
                            dust2.scale = Main.rand.NextFloat(0.65f, 1.05f);
                            dust2.noGravity = true;
                            dust2.color = Color.Lerp(Color.White, mainColor, 0.5f);
                        }
                    }   
                }
            }

            ArmRotationOffset = MathHelper.ToRadians(-140f);
            ArmRotationOffsetBack = MathHelper.ToRadians(-140f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 600);
            if ((target.life <= 0 && target.realLife == -1) && Projectile.numHits > 0)
                Projectile.numHits -= 1;
            if (damageDone <= 2)
                armoredHits++;

            Vector2 launchVel = (Projectile.ai[1] != 1 ? Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld) : Utils.DirectionTo(Owner.Center, target.Center));
            target.MoveNPC(launchVel, 37, true, Owner);

            if (spawnBoom)
            {
                // This was used to give some "Hitstop" on hit, but it looks a bit strange so it's on hold
                //pause = 6;
                for (int i = 0; i < 5; i++)
                {
                    Particle spark = new GlowSparkParticle(target.Center, (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitY) * (-25), false, 12, 0.12f - i * 0.025f, mainColor, new Vector2(3.75f, 0.9f), true, false, 1.15f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                for (int i = 0; i < 15; i++)
                {
                    float power = Main.rand.NextFloat(0, 0.6f);
                    Particle sparker = new CustomSpark(target.Center, (Owner.Center - Owner.Calamity().mouseWorld).SafeNormalize(Vector2.UnitY).RotatedByRandom(power) * (-Main.rand.NextFloat(18.5f, 50) * (1 - power)), "CalamityMod/Particles/Sparkle", false, 38, Main.rand.NextFloat(2.2f, 4.8f), mainColor, new Vector2(0.4f, Main.rand.NextFloat(0.9f, 1.4f)), true, true);
                    GeneralParticleHandler.SpawnParticle(sparker);
                }
                SoundStyle hit2 = new("CalamityMod/Sounds/Item/FinalDawnSlash");
                SoundEngine.PlaySound(hit2 with { Volume = 0.85f, Pitch = Main.rand.NextFloat(0.2f, 0.3f) }, Projectile.Center);
                SoundStyle fire = new("CalamityMod/Sounds/NPCHit/ThanatosHitOpen1");
                SoundEngine.PlaySound(fire with { Volume = 0.75f, Pitch = 0.2f }, Projectile.Center);
                SoundStyle fire2 = new("CalamityMod/Sounds/Item/ExobladeBeamSlash");
                SoundEngine.PlaySound(fire2 with { Volume = 0.35f, Pitch = Main.rand.NextFloat(0.5f, 0.7f) }, Projectile.Center);

                if (Projectile.ai[1] == -1)
                {
                    NPC chosenTarget = Owner.ClampedMouseWorld().ClosestNPCAt(1000);
                    Vector2 spawnSpot;
                    if (chosenTarget == null || !chosenTarget.active || chosenTarget.life <= 0)
                        spawnSpot = target.Center + new Vector2(Main.rand.NextFloat(-450, 450), Main.rand.NextFloat(-450, -650));
                    else
                        spawnSpot = chosenTarget.Center + new Vector2(Main.rand.NextFloat(-450, 450), Main.rand.NextFloat(-450, -650));

                    Projectile meteor = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnSpot, Vector2.Zero, ModContent.ProjectileType<EarthMeteor>(), (int)(Projectile.damage * 1.2f), Projectile.knockBack, Projectile.owner, 0, chosenTarget == null ? 0 : chosenTarget.whoAmI, 2);
                    meteor.scale = Projectile.scale;
                }

                spawnBoom = false;
            }

            int healPower = Projectile.ai[1] == -1 ? 60 : 50;
            int heal = (int)(MathHelper.Clamp(healPower - Projectile.numHits * 35, 1, healPower));
            if (Projectile.numHits < 10)
            {
                Owner.DoLifestealDirect(target, heal, 0.2f);
            }

            if (Projectile.numHits <= 2)
            {
                float scaleFactor = 1 - Projectile.numHits * 0.2f;

                int points = 6;
                float radians = MathHelper.TwoPi / points;
                Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f)).RotatedByRandom(100);
                for (int k = 0; k < points; k++)
                {
                    Vector2 velocity = spinningPoint.RotatedBy(radians * k).RotatedBy(-0.45f);
                    if (k % 2 == 0)
                        velocity *= 5;
                    Particle spark = new GlowSparkParticle((target.Center + velocity * 7.5f), velocity * 0.5f, false, 11, 0.08f * scaleFactor, mainColor, new Vector2(1f, 0.4f), true);
                    GeneralParticleHandler.SpawnParticle(spark);
                    Particle spark2 = new GlowSparkParticle((target.Center + velocity * 7.5f), velocity * 0.5f, false, 35, 0.06f * scaleFactor, mainColor, new Vector2(0.4f, 1.1f), false, false);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
                Particle blastRing = new CustomPulse(target.Center, Vector2.Zero, mainColor, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 2f * scaleFactor, 1f * scaleFactor, 15, true);
                GeneralParticleHandler.SpawnParticle(blastRing);
                Particle blastRing2 = new CustomPulse(target.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 1f * scaleFactor, 0.5f * scaleFactor, 15, true);
                GeneralParticleHandler.SpawnParticle(blastRing2);

                for (int i = 0; i < MathHelper.Clamp(10 - Projectile.numHits * 2, 2, 10); i++)
                {
                    float power = Main.rand.NextFloat(0.2f, 0.8f);
                    Dust dust2 = Dust.NewDustPerfect(target.Center, DustID.FireworksRGB, (Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld)).RotatedByRandom(power) * (Main.rand.NextFloat(10f, 35f) * ( 1 - power)));
                    dust2.scale = Main.rand.NextFloat(0.55f, 0.85f) * scaleFactor;
                    dust2.noGravity = true;
                    dust2.color = Color.Lerp(Color.White, mainColor, 0.5f);
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 1f;
            int hitsToMinMult = 1;
            float damageMult = Utils.Remap(Projectile.numHits - armoredHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // Only draw the projectile if the projectile's owner is currently using the item this projectile is attached to.
            if ((useAnim > 0 || DrawUnconditionally) && Owner.ItemAnimationActive)
            {
                Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
                Asset<Texture2D> glowTex = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/EarthGlow");

                float r = (FlipAsSword ? MathHelper.ToRadians(90) : 0f);

                for (int i = 0; i < 20; i++)
                {
                    Texture2D centerTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/EarthGhost").Value;
                    Color auraColor = mainColor with { A = 0 } * 0.15f * fadeIn;
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 20f).ToRotationVector2() * 6 * fadeIn;
                    Main.EntitySpriteDraw(centerTexture, Projectile.Center - Main.screenPosition + drawOffset + new Vector2(0, Owner.gfxOffY), centerTexture.Frame(1, FrameCount, 0, Frame), auraColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                }
                Asset<Texture2D> swoosh = ModContent.Request<Texture2D>("CalamityMod/Particles/VerticalSmearLarge");

                if (swingCount > 0)
                {
                    if (swingCount % 2 != 0)
                    {
                        Main.EntitySpriteDraw(swoosh.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), null, mainColor with { A = 0 } * fadeIn * 0.9f, (FinalRotation + MathHelper.ToRadians(45)) + MathHelper.ToRadians(swingCount % 2 != 0 ? -80 : 80) * -Owner.direction, swoosh.Size() * 0.5f, Projectile.scale * 0.89f, SpriteEffects.None);
                        Main.EntitySpriteDraw(swoosh.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), null, mainColor with { A = 0 } * fadeIn * 0.9f, (FinalRotation + MathHelper.ToRadians(45)) + MathHelper.ToRadians(swingCount % 2 != 0 ? -80 : 80) * -Owner.direction, swoosh.Size() * 0.5f, Projectile.scale * 0.89f, SpriteEffects.FlipVertically);
                    }
                    else
                        Main.EntitySpriteDraw(swoosh.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), null, mainColor with { A = 0 } * fadeIn * 0.9f, (FinalRotation + MathHelper.ToRadians(45)) + MathHelper.ToRadians(swingCount % 2 != 0 ? -85 : 85) * -Owner.direction, swoosh.Size() * 0.5f, Projectile.scale * 1.423f, SpriteEffects.None);
                }

                Asset<Texture2D> tex2 = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
                int draws = 45;

                for (int i = 0; i < draws; i++)
                {
                    bool swordTip = i > draws * 0.73f;
                    float swordTipScale = (swordTip ? Utils.Remap(i, (int)(draws * 0.73f), draws, 0.9f, 0.35f) : 1);
                    Vector2 offsetDir = Vector2.One.RotatedBy(Projectile.rotation + RotationOffset + MathHelper.ToRadians(90));

                    Color bladeColor = Color.Lerp(Color.MediumTurquoise, Color.Lerp(Color.LimeGreen, Color.OrangeRed, (i * 0.8f) / draws), i / (draws * 0.6f));
                    Color auraColor = Color.Lerp(bladeColor, Color.White, 0.2f) with { A = 0 } * 0.3f * bladeFade;
                    Vector2 drawOffset = -offsetDir * 5 * i * bladeFade * Projectile.scale;
                    float bladeHiltMult = (MathHelper.Lerp(1.55f, 1, 1 - MathF.Pow(Utils.GetLerpValue(i > draws * 0.08f ? draws * 0.55f : 0, draws * 0.04f, i, true), 5)));
                    Main.EntitySpriteDraw(tex2.Value, Projectile.Center - offsetDir * 70 * Projectile.scale - Main.screenPosition + drawOffset + new Vector2(0, Owner.gfxOffY) + Main.rand.NextVector2Circular(2, 2), tex2.Frame(1, FrameCount, 0, Frame), auraColor, RotationOffset + Projectile.rotation + MathHelper.ToRadians(45), tex2.Size() * 0.5f, new Vector2(0.7f * swordTipScale * MathF.Pow(bladeHiltMult, 0.6f), 0.5f / bladeHiltMult) * (0.75f * swordTipScale) * 0.9f * bladeFade * bladeHiltMult * Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                }

                Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), tex.Frame(1, FrameCount, 0, Frame), lightColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                Main.EntitySpriteDraw(glowTex.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), glowTex.Frame(1, FrameCount, 0, Frame), Color.White, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(glowTex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));

            }
            return false;
        }
        public override void ResetStyle()
        {
        }
    }
}
