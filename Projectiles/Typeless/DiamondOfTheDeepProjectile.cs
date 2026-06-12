using System;
using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Abyss;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Sounds;
using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class DiamondOfTheDeepProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public Color bColor = Color.White; // Shifting color, used for most effects
        public Color color1 = Color.White;
        public Color color2 = Color.White;
        public bool canDamage = false;
        public bool visuals => Owner.Calamity().dOfTheDeepVisual; // Enables/disables visuals and sounds based on accessory visibility
        public ref float time => ref Projectile.ai[0];
        public ref float energyNumber => ref Projectile.ai[1]; // The number assigned to this energy, ranging from 1 to the max number of projectiles
        public bool idle => Projectile.ai[2] == 0; // Floating around the player
        public bool healing = false; // If the projectiles should heal the player. If false they will attack
        public NPC targeted; // Chosen target to home into
        public int hitCooldown = 0; // Only for VOID, time after hit that it turns around before another hit
        public ref float projType => ref Projectile.localAI[2];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NoLiquidDistortion[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        // Hello, if you're trying to use code from this thing
        // DO NOT
        // It's very messy and confusing, because it was coded in pieces over the course of months with various levels of skill
        // If I ever make another thing like this it will most certainly be 3 seperate projectiles, or at least use AIState instead
        public override void AI()
        {
            Vector2 velocity = Projectile.velocity;
            int startTime = (projType == 0 ? 70 : projType == 1 ? 140 : 210) + 15; // Time after launch when it will begin attacking/healing
            int endTime = 320; // Time after launch when it's movement code reaches it's cap on strength

            #region Color Shifting
            if (time == 0 && idle)
            {
                switch (projType) // 0 = gravel/vines, 1 = hadal/hydrothermic, 2 = void/lumenyl
                {
                    case 0:
                        {
                            color1 = Color.MediumSeaGreen;
                            color2 = Color.DarkSlateGray;
                            break;
                        }
                    case 1:
                        {
                            color1 = Color.DarkRed;
                            color2 = Color.OrangeRed;
                            break;
                        }
                    case 2:
                        {
                            color1 = Color.MediumBlue;
                            color2 = Color.DodgerBlue;
                            break;
                        }
                }
            }

            float rate = Main.GlobalTimeWrappedHourly * 5;
            List<Color> eColors = new List<Color>()
            {
                color1,
                color2
            };

            int colorIndex = (int)(rate / 2 % eColors.Count);
            Color currentColor = eColors[colorIndex];
            Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
            bColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);
            #endregion

            if (idle || time < startTime) // If not attacking/healing keep it alive
                Projectile.timeLeft++;

            if (Owner.dead || !Owner.Calamity().dOfTheDeep)
                Projectile.Kill();

            if (Utils.Distance(Owner.Center, Projectile.Center) > 1100) // If it's too far away either teleport to the player if idle (or kill it if needed)
            {
                if (idle)
                    Projectile.Center = Owner.Center;
                else if (targeted == null && !healing)
                    Projectile.Kill();
            }

            // Emit some light
            Lighting.AddLight(Projectile.Center, bColor.ToVector3() * 1.5f);

            #region Dust Trails
            float velLerp = Utils.GetLerpValue(-1.5f, 3f, Projectile.velocity.Length(), true);
            if (projType == 0 && visuals)
            {
                if (Main.rand.NextBool((int)(14 - 5 * velLerp)))
                {
                    Dust c = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6, 6) * Projectile.scale + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 15, ModContent.DustType<SquashDustHollow>());
                    c.velocity = -Projectile.velocity * Main.rand.NextFloat(0.3f, 1f) * velLerp;
                    c.scale = Main.rand.NextFloat(1f, 1.3f) * velLerp;
                    c.noGravity = true;
                    c.color = Main.rand.NextBool() ? Color.Aquamarine : bColor;
                    c.noLightEmittence = true;
                    c.fadeIn = 1;
                    c.alpha = 100;
                }
            }
            if (projType == 1 && visuals)
            {
                if (Main.rand.NextBool((int)(10 - 5 * velLerp)))
                {
                    Dust c = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 15, ModContent.DustType<SquashDust>());
                    c.velocity = -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.6f, 1.3f) * velLerp;
                    c.scale = Main.rand.NextFloat(1.2f, 1.5f) * velLerp;
                    c.noGravity = true;
                    c.color = bColor;
                    c.noLightEmittence = true;
                    c.noGravity = !Main.rand.NextBool(3);
                }
            }
            if (projType == 2 && visuals)
            {
                if (Main.rand.NextBool((int)(3 - velLerp)))
                {
                    Dust c = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6, 6) * Projectile.scale + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 15, ModContent.DustType<SquashDust>());
                    c.velocity = -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(1.4f, 3.5f) * velLerp;
                    c.scale = Main.rand.NextFloat(1.8f, 2.3f) * velLerp;
                    c.noGravity = true;
                    c.color = bColor;
                    c.noLightEmittence = true;
                    c.fadeIn = 2.3f;
                }
            }
            #endregion
            
            if (idle) // Following Ai
            {
                if (time == 0 && visuals) // On spawn fx
                {
                    for (int i = 0; i <= 10; i++)
                    {
                        float variance = Main.rand.NextFloat(-0.5f, 0.5f);
                        Vector2 vel = (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 7).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance)) * 2;
                        float scale = (Main.rand.NextFloat(1.5f, 1.7f) - Math.Abs(variance)) * 0.35f;

                        Dust dust2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>(), vel);
                        dust2.scale = scale * 3;
                        dust2.noGravity = true;
                        dust2.color = bColor;
                        dust2.noLightEmittence = true;
                    }
                }
                if (time > 80) // Track the player in a natural motion simular to swimming
                {
                    float homingSpeed = Utils.Remap(Utils.Distance(Projectile.Center, Owner.Center), 200, 600, 0.07f, 0.16f) + 0.009f * energyNumber;
                    float offsetPower = Utils.GetLerpValue(1, 5, Owner.velocity.Length(), true);
                    float sine = (float)Math.Sin((time * 0.1f) / MathHelper.Pi);
                    float sine2 = (float)Math.Sin((time * 0.04f) / MathHelper.Pi);
                    Vector2 bonusMobility = (offsetPower > 0 ? ((Utils.DirectionTo(Projectile.Center, Owner.Center) * 90) * sine2).RotatedBy(0.8f * sine) * offsetPower : Vector2.Zero);
                    Vector2 goalPosition = Owner.MountedCenter + bonusMobility + ((MathHelper.TwoPi * energyNumber) / Math.Max(Owner.ownedProjectileCounts[ModContent.ProjectileType<AmuletEnergy>()], 1)).ToRotationVector2().RotatedBy(Main.GlobalTimeWrappedHourly * 0.4f) * 20;

                    bool outOfRange = Utils.Distance(Projectile.Center, goalPosition) > 120;
                    if (Projectile.velocity.Length() < 6 && outOfRange)
                        Projectile.velocity = Projectile.velocity * 0.995f + Utils.DirectionTo(Projectile.Center, goalPosition) * homingSpeed;
                    else if (outOfRange)
                        Projectile.velocity *= 0.985f;
                    if (!outOfRange)
                        Projectile.velocity = Projectile.velocity.RotatedBy(0.0065f * (energyNumber % 2 == 0 ? -1 : 1)) * 1.004f;

                    velocity = Projectile.velocity;
                }
                else
                    Projectile.velocity *= 0.99f;
            }
            else // Attack/Healing Ai
            {
                if (Projectile.ai[2] == 5) // Effects for the moment they stop being idle
                {
                    time = 0;
                    healing = (Owner.statLife < Owner.statLifeMax2 * 0.5f);
                    Projectile.ai[2]++;
                }
                if (healing)
                    startTime = 100;
                if (time <= startTime) // Get into position before the attack/heal
                {
                    float timeLerp = Utils.GetLerpValue(0, startTime * 0.5f, time, true);
                    int distFromPlayer = (int)(160 + (healing ? 240 * Utils.GetLerpValue(0, startTime * 0.8f, time, true) : 0));
                    int moveSpeed = (int)(90 - 85 * timeLerp);
                    if (projType == 0)
                    {
                        Vector2 destination = Owner.Center + Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld).RotatedBy(MathHelper.TwoPi / 3) * distFromPlayer;
                        Projectile.velocity = (destination - Projectile.Center) / moveSpeed;
                        velocity = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
                    }
                    if (projType == 1)
                    {
                        Vector2 destination = Owner.Center + Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld) * distFromPlayer;
                        Projectile.velocity = (destination - Projectile.Center) / moveSpeed;
                        velocity = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
                    }
                    if (projType == 2)
                    {
                        Vector2 destination = Owner.Center + Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld).RotatedBy(-(MathHelper.TwoPi / 3)) * distFromPlayer;
                        Projectile.velocity = (destination - Projectile.Center) / moveSpeed;
                        velocity = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
                    }
                }

                if (healing) // Heal/Buff player (only occurs below 50% HP)
                {
                    if (time > startTime)
                    {
                        float sine = (float)Math.Sin((time * 0.3f) / MathHelper.Pi);
                        Projectile.extraUpdates = 6;

                        if (time > startTime)
                        {
                            float homingSpeed = Utils.Remap(time, startTime, endTime, 0.01f, 0.1f);

                            Vector2 goalPosition = Owner.Center;

                            if (Projectile.velocity.Length() < 5)
                                Projectile.velocity = Projectile.velocity.RotatedBy(0.02f * sine) * 0.99f + Utils.DirectionTo(Projectile.Center, goalPosition) * homingSpeed;
                            else
                                Projectile.velocity *= 0.985f;

                            if (Utils.Distance(goalPosition, Projectile.Center) < 50)
                            {
                                if (projType == 0) // Heals you for 8 flat
                                {
                                    Owner.HealPlayer(8);
                                }
                                if (projType == 1) // Gives you a temporary defense buff
                                {
                                    Owner.Calamity().dOfTheDeepDefenseBuffTimer = Owner.Calamity().dOfTheDeepDefenseBuffMax;
                                }
                                if (projType == 2) // Heals you for 5% max hp
                                {
                                    int healValue = (int)((float)(Owner.statLifeMax2) * 0.05f);
                                    Owner.HealPlayer(healValue);
                                }

                                Projectile.Kill();
                            }
                        }
                    }
                }
                else // Attacking
                {
                    if (time > startTime)
                    {
                        if (canDamage == false) // Effects for the moment they begin attacking
                        {
                            if (visuals)
                            {
                                SoundStyle fire = AstrumDeusHead.GodRaySound;
                                SoundEngine.PlaySound(fire with { Volume = 0.3f, Pitch = (Main.rand.NextFloat(-0.6f, -0.5f) + projType * 0.1f) }, Projectile.Center);
                                SoundStyle fire2 = StatisVoidSash.VoidDash;
                                SoundEngine.PlaySound(fire2 with { Volume = 0.3f, Pitch = (Main.rand.NextFloat(0.2f, 0.4f) + projType * 0.1f) }, Projectile.Center);
                            }

                            NPC tempTarget = Projectile.Center.ClosestNPCAt(1200);
                            Vector2 aimTo = (tempTarget == null ? Owner.Calamity().mouseWorld : tempTarget.Center);
                            if (projType == 0)
                            {
                                Projectile.velocity = Utils.DirectionTo(Projectile.Center, aimTo) * 2;
                                Projectile.extraUpdates = 3;
                            }
                            if (projType == 1)
                            {
                                Projectile.velocity = Utils.DirectionTo(Projectile.Center, aimTo) * 11;
                                Projectile.extraUpdates = 18;

                                if (visuals)
                                {
                                    for (float i = 1; i <= 1.4f; i += 0.4f)
                                    {
                                        Particle pulse = new CustomSpark(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitX) * 7 * i, "CalamityMod/Particles/BloomRing", false, 18, 0.5f / i, bColor, new Vector2(3f, 1f), extraRotation: MathHelper.ToRadians(90), shrinkSpeed: 0.9f);
                                        GeneralParticleHandler.SpawnParticle(pulse);
                                    }
                                }
                            }
                            if (projType == 2)
                            {
                                Projectile.velocity = Utils.DirectionTo(Projectile.Center, aimTo) * 7;
                                Projectile.extraUpdates = 7;
                                Projectile.localNPCHitCooldown = 5;
                                Projectile.timeLeft = 600;

                                if (visuals)
                                {
                                    Particle pulse = new CustomSpark(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitX) * 5, "CalamityMod/Particles/BloomRing", false, 14, 0.35f, bColor, new Vector2(3f, 1f), extraRotation: MathHelper.ToRadians(90), shrinkSpeed: 0.9f);
                                    GeneralParticleHandler.SpawnParticle(pulse);
                                }
                            }

                            if (visuals)
                            {
                                for (int i = 0; i <= 12; i++)
                                {
                                    float variance = Main.rand.NextFloat(-0.7f, 0.7f);
                                    Vector2 vel = (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 24).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance));
                                    float scale = (Main.rand.NextFloat(1.5f, 1.7f) - Math.Abs(variance)) * 0.35f;

                                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>(), vel);
                                    dust2.scale = scale * 4;
                                    dust2.noGravity = true;
                                    dust2.color = bColor;
                                    dust2.noLightEmittence = true;
                                }
                            }
                        }
                        canDamage = true;

                        float homingSpeed = Utils.Remap(time, startTime, endTime, 0.01f, 0.5f);
                        if (projType == 0)
                        {
                            targeted = Owner.Calamity().mouseWorld.ClosestNPCAt(2300);
                            CalamityUtils.HomeInOnSelectedNPC(Projectile, targeted, true, homingSpeed, 25, 0.99f, accelerate: true);

                            if (targeted == null)
                            {
                                if (Projectile.velocity.Y > 5)
                                {
                                    Projectile.velocity.Y += 0.8f * homingSpeed;
                                    Projectile.velocity.X *= 0.997f;
                                }
                            }
                            else
                                Projectile.timeLeft++;
                        }
                        if (projType == 2)
                        {
                            if (hitCooldown > 0)
                                hitCooldown--;
                            float timeLerp = Utils.GetLerpValue(400, 800, Projectile.timeLeft, true); // While a target is active, lifetime goes up, which increases homing power
                            if (targeted == null && hitCooldown == 0)
                            {
                                CalamityUtils.HomeInOnSelectedNPC(Projectile, Owner.Calamity().mouseWorld.ClosestNPCAt(2300), true, 0.5f, 10, 0.99f, accelerate: true);
                            }
                            else if (hitCooldown == 0)
                            {
                                Projectile.timeLeft += 2;
                                CalamityUtils.HomeInOnSelectedNPC(Projectile, targeted, true, 0.6f * timeLerp, 10, 0.99f, accelerate: true);
                            }
                            else
                            {
                                if (hitCooldown < 100)
                                    Projectile.velocity = Projectile.velocity.RotatedBy((Projectile.numHits % 2 == 0 ? 1 : -1) * 0.0363f) * 0.99f;
                            }
                            if (targeted != null && (targeted.life <= 0 || !targeted.CanBeChasedBy() || !targeted.active))
                                targeted = null;
                        }
                    }
                }
            }

            // The main particle trail
            float squash = Utils.GetLerpValue(1, 3, Projectile.velocity.Length(), true);
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);
            if (squash > 0.15f && visuals && targetDist < 1400)
            {
                float scale = 0.55f * Projectile.scale * (projType == 1 && canDamage ? 2 : 1);
                int lifetime = (int)(18 * (projType == 1 && canDamage ? 2.5f : 1));
                Particle fadeInfx = new CustomSpark(Projectile.Center, Projectile.velocity * 0.01f, "CalamityMod/Particles/BloomCircle", false, lifetime, scale, bColor * 0.4f * squash, new Vector2(1 - 0.15f * squash, 1f), true, true, shrinkSpeed: 0.3f * squash, glowOpacity: 0.5f, glowCenterScale: 0.45f);
                GeneralParticleHandler.SpawnParticle(fadeInfx);
            }

            Projectile.scale = MathHelper.Lerp(Projectile.scale, 0.7f, 0.1f);
            Projectile.rotation = velocity.ToRotation() + MathHelper.PiOver2;
            time++;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (projType == 0)
            {
                if (visuals)
                {
                    SoundStyle s = DeusMine.ExplodeSound;
                    SoundEngine.PlaySound(s with { Volume = 0.7f, Pitch = (Main.rand.NextFloat(0.3f, 0.4f)) }, Projectile.Center);
                    SoundStyle fire2 = StatisVoidSash.VoidDash;
                    SoundEngine.PlaySound(fire2 with { Volume = 0.4f, Pitch = Main.rand.NextFloat(-0.2f, -0.3f) }, Projectile.Center);

                    for (int i = 0; i < 15; i++)
                    {
                        Dust c = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>());
                        c.velocity = (MathHelper.TwoPi * i / 15f).ToRotationVector2() * 16f * (i % 3 == 0 ? 0.8f : 1f);
                        c.scale = Main.rand.NextFloat(1.3f, 1.6f) * 0.9f * (i % 3 == 0 ? 2.2f : 1.8f);
                        c.noGravity = true;
                        c.color = bColor;
                        c.noLightEmittence = true;
                        c.fadeIn = 0.9f;
                    }
                    Particle blastRing = new CustomPulse(target.Center, Vector2.Zero, bColor * 0.7f, "CalamityMod/Particles/BloomRing", Vector2.One, Main.rand.NextFloat(-10, 10), 0.5f, 1.8f, 13, true);
                    GeneralParticleHandler.SpawnParticle(blastRing);
                }

                float blastSize = 170;
                float minMultiplier = 0.35f;
                int hitsToMinMult = 8;
                int debuff = ModContent.BuffType<CrushDepth>();
                int debuffTime = 240;
                target.AddBuff(debuff, debuffTime);

                Projectile blast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BasicBurstExclusive>(), Projectile.damage, -11, Projectile.owner, blastSize, minMultiplier, hitsToMinMult);
                blast.timeLeft = 14;
                blast.DamageType = Projectile.DamageType;
                blast.localAI[0] = target.whoAmI;
                blast.localAI[1] = debuff;
                blast.localAI[2] = debuffTime;
                Projectile.Kill();
            }
            else
            {
                // HADAL and VOID have knockback, GRAVEL has it's knockback on the explosion
                Vector2 launchVel = Utils.DirectionTo(Projectile.Center, target.Center) - Vector2.UnitY;
                float launchPower = 7f;
                target.MoveNPC(launchVel, launchPower, false, Owner);

                if (visuals)
                {
                    for (int i = 0; i <= Math.Max(2, 8 - Projectile.numHits); i++)
                    {
                        float variance = Main.rand.NextFloat(-0.3f, 0.3f);
                        Vector2 vel = (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 24).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance));
                        float scale = (Main.rand.NextFloat(1.5f, 1.7f) - Math.Abs(variance)) * 0.35f;

                        Dust dust2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>(), vel);
                        dust2.scale = scale * 5;
                        dust2.noGravity = true;
                        dust2.color = bColor;
                        dust2.noLightEmittence = true;
                        dust2.fadeIn = 1.5f;
                    }
                }
            }
            if (projType == 1)
            {
                if (visuals)
                {
                    SoundStyle s = AuricOre.MineSound;
                    SoundEngine.PlaySound(s with { Volume = 0.5f, Pitch = (-0.3f + Projectile.numHits * 0.1f), MaxInstances = 6 }, Projectile.Center);
                }

                target.AddBuff(BuffID.OnFire3, 300); // Should be replaced with "Steamscorched" or whatever the scoria debuff will be
                float minMult = 0.25f;
                int hitsToMinMult = 10;
                float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
                modifiers.SourceDamage *= damageMult * 2.5f; // Deals extra damage
            }
            if (projType == 2)
            {
                modifiers.SourceDamage *= 0.33f; // Deals less damage but hits multiple times
                if (visuals)
                {
                    SoundStyle s = CommonCalamitySounds.VoidstoneMine with { Volume = 1 };
                    SoundEngine.PlaySound(s with { Volume = 0.6f, Pitch = (-0.3f + Projectile.numHits * 0.1f) }, Projectile.Center);
                }

                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 7;
                target.AddBuff(ModContent.BuffType<HadopelagicPressure>(), 30); // Very stong debuff need very short time
                targeted = target;
                hitCooldown = 140;
                Projectile.timeLeft = 600;
                Projectile.extraUpdates += 3; // Get faster on each hit
                if (Projectile.numHits >= 5)
                    Projectile.Kill();
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (healing && visuals)
            {
                for (int i = 0; i <= 4; i++)
                {
                    float variance = Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 vel = (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 4).RotatedBy(variance) * Main.rand.NextFloat(0.3f, 1f) * (1 - Math.Abs(variance)) * 4;
                    float scale = (Main.rand.NextFloat(1.5f, 1.7f) - Math.Abs(variance)) * 0.35f;

                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), vel);
                    dust2.scale = scale * 2;
                    dust2.noGravity = false;
                    dust2.color = bColor;
                    dust2.noLight = true;
                    dust2.noLightEmittence = true;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> orb = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
            float sine = (float)Math.Sin((Main.GlobalTimeWrappedHourly * 10) / MathHelper.Pi);
            Vector2 squash = new Vector2(Utils.Remap(Projectile.velocity.Length(), 1, 5, 1, 0.6f), Utils.Remap(Projectile.velocity.Length(), 1, 5, 1, 2f));

            Asset<Texture2D> block = TextureAssets.Item[ModContent.ItemType<AbyssGravel>()];
            switch (projType) // 0 = gravel/vines, 1 = hadal/hydrothermic, 2 = void/lumenyl
            {
                case 0:
                    {
                        block = TextureAssets.Item[ModContent.ItemType<AbyssGravel>()];
                        break;
                    }
                case 1:
                    {
                        block = TextureAssets.Item[ModContent.ItemType<PyreMantle>()];
                        break;
                    }
                case 2:
                    {
                        block = TextureAssets.Item[ModContent.ItemType<Voidstone>()];
                        break;
                    }
            }

            // The main "glowy orb" part of the projectile
            for (int i = 0; i < 6; i++)
            {
                Color orbColor = Color.Lerp(bColor, color2, (i + 1) / 6) with { A = 0 } * 0.4f;
                Vector2 scale = Projectile.scale * squash * (0.05f + i * 0.01f) * 4.3f;
                Main.EntitySpriteDraw(orb.Value, Projectile.Center - Main.screenPosition, null, Color.Lerp(orbColor, Color.White with { A = 0 }, 1 - i * 0.5f) * (visuals ? 1 : 0.15f), Projectile.rotation, orb.Size() * 0.5f, scale, SpriteEffects.None);
            }

            float velLerp = Utils.GetLerpValue(0.5f, 3f, Projectile.velocity.Length(), true);
            if (projType == 0 && visuals) // GRAVEL circle
            {
                Asset<Texture2D> ring = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomRing");
                int orbs = 9;
                for (int i = 1; i < orbs + 1; i++)
                {
                    float sine2 = (float)Math.Sin((time * (i % 3 == 0 ? 0.2f : i % 2 == 0 ? 0.4f : 1f) * 0.2f) / MathHelper.Pi);

                    Vector2 placement = Projectile.Center + ((MathHelper.TwoPi * i / orbs) + 7 * sine).ToRotationVector2() * (18 + Math.Abs(sine * (i % 3 == 0 ? 19 : 11))) * velLerp;
                    Color orbColor = Color.White * (visuals ? 1 : 0.1f) * velLerp;
                    Vector2 scale = Vector2.One * Projectile.scale;

                    for (int y = 0; y < 7; y++)
                    {
                        Vector2 drawOffset = (MathHelper.TwoPi * y / 7).ToRotationVector2() * 4.5f;
                        Main.EntitySpriteDraw(block.Value, placement - Main.screenPosition + drawOffset, null, bColor with { A = 0 } * 0.7f * velLerp, Projectile.rotation + 0.02f * MathHelper.Lerp(i, 1, 0.75f) + 0.4f, block.Size() * 0.5f, scale, i % 2 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                    }
                    Main.EntitySpriteDraw(block.Value, placement - Main.screenPosition, null, orbColor, Projectile.rotation + 0.02f * MathHelper.Lerp(i, 1, 0.75f) + 0.4f, block.Size() * 0.5f, scale, i % 2 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                }
                for (int i = 0; i < 3; i++)
                    Main.EntitySpriteDraw(ring.Value, Projectile.Center - Main.screenPosition, null, bColor with { A = 0 } * 0.9f * velLerp, Projectile.rotation, ring.Size() * 0.5f, 0.2f * velLerp * Projectile.scale + (0.15f * Math.Abs(sine)) + 0.02f * i, SpriteEffects.None);
            }
            if (projType == 1 && visuals) // HALAD spires
            {
                int orbs = 9;
                for (int i = 0; i < orbs; i++)
                {
                    bool outer = i > (2);
                    bool outest = i > (5);
                    float rotation = MathHelper.TwoPi * i / (3) + Main.GlobalTimeWrappedHourly * 6;
                    Vector2 placement = Projectile.Center + ((rotation).ToRotationVector2() * (outest ? 16 : outer ? 12 : 8) + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * (outest ? -15f : outer ? 0f : 15))) * velLerp;
                    Color orbColor = Color.White * (visuals ? 1 : 0.1f) * velLerp;
                    Vector2 scale = new Vector2(0.35f + (outest ? 0.1f : outer ? 0.3f : 0.6f), 2.3f) * Projectile.scale * (outest ? 0.8f : outer ? 0.9f : 1) * 1.5f;

                    for (int y = 0; y < 7; y++)
                    {
                        Vector2 drawOffset = (MathHelper.TwoPi * y / 7).ToRotationVector2() * 2.5f;
                        Main.EntitySpriteDraw(block.Value, placement - Main.screenPosition + drawOffset, null, bColor with { A = 0 } * velLerp, Utils.DirectionTo(placement, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 65f).ToRotation() + MathHelper.PiOver2, block.Size() * 0.5f, scale, outer ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                    }
                    Main.EntitySpriteDraw(block.Value, placement - Main.screenPosition, null, orbColor, Utils.DirectionTo(placement, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 65f).ToRotation() + MathHelper.PiOver2, block.Size() * 0.5f, scale, outer ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                }
            }
            if (projType == 2 && visuals) // VOID shards
            {
                float scalingValue = Main.GlobalTimeWrappedHourly * (90 + Projectile.numHits * 55);
                int orbs = 8;
                for (int i = 1; i < orbs + 1; i++)
                {
                    float sine3 = (float)Math.Sin((scalingValue * (i % 3 == 0 ? 0.2f : i % 2 == 0 ? 0.4f : 1f) * 0.07f) / MathHelper.Pi);
                    float sine2 = (float)Math.Sin((scalingValue * (i % 3 == 0 ? 0.2f : i % 2 == 0 ? 0.4f : 1f) * 0.2f) / MathHelper.Pi);

                    Vector2 velocity = Projectile.Center + ((MathHelper.TwoPi * i / orbs) + scalingValue * 0.04f).ToRotationVector2() * (23f + 8 * sine3) * Math.Abs(sine2) * velLerp;

                    Color orbColor = Color.White * (visuals ? 1 : 0.1f) * velLerp;
                    Vector2 scale = new Vector2(i % 2 == 0 ? 0.4f : 0.6f, i % 2 == 0 ? 1f : 1.3f) * Projectile.scale * 1.3f;

                    for (int y = 0; y < 7; y++)
                    {
                        Vector2 drawOffset = (MathHelper.TwoPi * y / 7).ToRotationVector2() * 4.5f;
                        Main.EntitySpriteDraw(block.Value, velocity - Main.screenPosition + drawOffset, null, bColor with { A = 0 } * 0.7f * velLerp, Utils.DirectionTo(velocity, Projectile.Center).ToRotation() + MathHelper.PiOver2 * sine3, block.Size() * 0.5f, scale, i % 2 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                    }
                    Main.EntitySpriteDraw(block.Value, velocity - Main.screenPosition, null, orbColor, Utils.DirectionTo(velocity, Projectile.Center).ToRotation() + MathHelper.PiOver2 * sine3, block.Size() * 0.5f, scale, i % 2 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                }
            }

            return false;
        }
        public override bool? CanDamage() => canDamage ? null : false;
        public override bool? CanHitNPC(NPC target)
        {
            if (!target.CanBeChasedBy())
                return false;
            if (projType == 2) // VOID has multihit, so it needs extra checks. Can't have it inflicting post ML debuffs and piercing
            {
                if (hitCooldown > 0 || (targeted != null && target != targeted))
                    return false;
                else
                    return null;
            }
            else
                return null;
        }
    }
}
