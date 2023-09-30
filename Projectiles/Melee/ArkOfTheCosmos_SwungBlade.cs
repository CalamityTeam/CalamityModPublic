using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Melee
{
    public class ArkoftheCosmosSwungBlade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/SunderingScissorsRight";

        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Combo => ref Projectile.ai[0];
        public ref float Charge => ref Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];

        Particle smear;

        #region Regular swing variables

        public float MaxSwingTime => SwirlSwing ? 55 : 35;
        public int SwingDirection
        {
            get
            {
                switch (Combo)
                {
                    case 0:
                        return 1 * Math.Sign(direction.X);
                    case 1:
                        return -1 * Math.Sign(direction.X);
                    default:
                        return 0;

                }
            }
        }

        public bool SwirlSwing => Combo == 1;

        private float SwingWidth = MathHelper.PiOver2 * 1.5f;
        public Vector2 DistanceFromPlayer => direction * 30;
        public float SwingTimer => MaxSwingTime - Projectile.timeLeft;
        public float SwingCompletion => SwingTimer / MaxSwingTime;
        public ref float HasFired => ref Projectile.localAI[0];
        #endregion

        #region Throw variables
        //Only used for the long range throw
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed && Owner.HeldItem.type == ItemType<ArkoftheCosmos>();
        public bool Thrown => Combo == 2 || Combo == 3;
        public const float MaxThrowTime = 140;
        public float ThrowReach;
        public float ThrowTimer => MaxThrowTime - Projectile.timeLeft;
        public float ThrowCompletion => ThrowTimer / MaxThrowTime;

        public const float SnapWindowStart = 0.2f;
        public const float SnapWindowEnd = 0.75f;
        public float SnapEndTime => (MaxThrowTime - (MaxThrowTime * SnapWindowEnd));
        public float SnapEndCompletion => (SnapEndTime - Projectile.timeLeft) / SnapEndTime;
        public ref float ChanceMissed => ref Projectile.localAI[1];

        #endregion

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.width = Projectile.height = 60;
            Projectile.width = Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Thrown ? 10 : (int)(MaxSwingTime);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float bladeLength = 172f * Projectile.scale;

            if (Thrown)
            {
                bool mainCollision = Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - Vector2.One * bladeLength / 2f, Vector2.One * bladeLength);
                if (Combo == 2f)
                    return mainCollision;

                else
                {
                    Vector2 thrownBladeStart = Vector2.SmoothStep(Owner.Center, Projectile.Center, MathHelper.Clamp(SnapEndCompletion + 0.25f, 0f, 1f));
                    bool thrownScissorCollision = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), thrownBladeStart, thrownBladeStart + direction * bladeLength);
                    return mainCollision || thrownScissorCollision;
                }
            }

            float collisionPoint = 0f;
            Vector2 holdPoint = DistanceFromPlayer.Length() * Projectile.rotation.ToRotationVector2();

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + holdPoint, Owner.Center + holdPoint + Projectile.rotation.ToRotationVector2() * bladeLength, 24, ref collisionPoint);
        }

        //Swing animation keys
        public CurveSegment anticipation = new CurveSegment(EasingType.ExpOut, 0f, 0f, 0.15f);
        public CurveSegment thrust = new CurveSegment(EasingType.PolyInOut, 0.1f, 0.15f, 0.85f, 3);
        public CurveSegment hold = new CurveSegment(EasingType.Linear, 0.5f, 1f, 0.2f);
        internal float SwingRatio() => PiecewiseAnimation(SwingCompletion, new CurveSegment[] { anticipation, thrust, hold });

        //Wide swing animation keys
        public CurveSegment startup = new CurveSegment(EasingType.SineIn, 0f, 0f, 0.25f);
        public CurveSegment swing = new CurveSegment(EasingType.SineOut, 0.1f, 0.25f, 0.75f);
        internal float SwirlRatio() => PiecewiseAnimation(SwingCompletion, new CurveSegment[] { startup, swing });

        //Throw animation keys. This one is on the esoteric side, since the first 2 anim segments get used to determine the strenght of the cursor homing of the scissor blade, while the retract part is actually used for the retraction
        public CurveSegment shoot = new CurveSegment(EasingType.PolyIn, 0f, 1f, -0.2f, 3);
        public CurveSegment remain = new CurveSegment(EasingType.Linear, SnapWindowStart, 0.8f, 0f);
        public CurveSegment retract = new CurveSegment(EasingType.SineIn, SnapWindowEnd, 1f, -1f);
        internal float ThrowRatio() => PiecewiseAnimation(ThrowCompletion, new CurveSegment[] { shoot, remain, retract });


        public CurveSegment sizeCurve = new CurveSegment(EasingType.SineBump, 0f, 0f, 1f);
        internal float ThrowScaleRatio() => PiecewiseAnimation(ThrowCompletion, new CurveSegment[] { sizeCurve });

        public override void AI()
        {
            if (!initialized) //Initialization
            {
                Projectile.timeLeft = Thrown ? (int) MaxThrowTime : (int)MaxSwingTime;
                SoundStyle sound = (Charge > 0 || Thrown) ? CommonCalamitySounds.LouderPhantomPhoenix : SoundID.Item71;
                SoundEngine.PlaySound(sound, Projectile.Center);
                direction = Projectile.velocity;
                direction.Normalize();
                Projectile.velocity = direction;
                Projectile.rotation = direction.ToRotation();

                if (SwirlSwing)
                    Projectile.localNPCHitCooldown = (int)(Projectile.localNPCHitCooldown / 4f);

                initialized = true;
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            if (!Thrown)
            {
                //Manage position and rotation
                Projectile.Center = Owner.Center + DistanceFromPlayer;

                //Baby swing
                if (!SwirlSwing)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(SwingWidth / 2 * SwingDirection, -SwingWidth / 2 * SwingDirection, SwingRatio());
                }

                //Chungal swing
                else
                {
                    float startRot = (MathHelper.Pi - MathHelper.PiOver4) * SwingDirection;
                    float endRot = -(MathHelper.TwoPi + MathHelper.PiOver4 * 1.5f) * SwingDirection;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(startRot, endRot, SwirlRatio());
                    DoParticleEffects(true);

                    //Really important to use projectile.timeLeft -1 instead of simply projectile.timeLeft because if it spawns a bolt on its last frame of existence the primitive drawer will break and shit itself or something idk
                    if (Owner.whoAmI == Main.myPlayer && (Projectile.timeLeft - 1) % Math.Ceiling(MaxSwingTime / ArkoftheCosmos.SwirlBoltAmount) == 0f)
                    {
                        //Slightly shift the blasts up so the final close shots don't go BELOW the cursor and instead go right on it.
                        float adjustedBlastRotation = Projectile.rotation - MathHelper.PiOver4 * 1.15f * Owner.direction;

                         var source = Projectile.GetSource_FromThis();
                         Projectile blast = Projectile.NewProjectileDirect(source, Owner.Center + adjustedBlastRotation.ToRotationVector2() * 10f, adjustedBlastRotation.ToRotationVector2() * 20f, ProjectileType<EonBolt>(), (int)(ArkoftheCosmos.SwirlBoltDamageMultiplier / ArkoftheCosmos.SwirlBoltAmount * Projectile.damage), 0f, Owner.whoAmI, 0.55f, MathHelper.Pi * 0.05f);
                         {
                            blast.timeLeft = 100;
                         }
                    }
                }

                Projectile.scale = 1.2f + ((float)Math.Sin(SwingRatio() * MathHelper.Pi) * 0.6f) + (Charge / 10f) * 0.2f;
            }

            else
            {
                //Telegraph the start of the snap window with a bit of leeway
                if (Math.Abs(ThrowCompletion - SnapWindowStart + 0.1f) <= 0.005f && ChanceMissed == 0f && Main.myPlayer == Owner.whoAmI)
                {
                    Particle pulse = new PulseRing(Projectile.Center, Vector2.Zero, Color.OrangeRed, 0.05f, 1.8f, 8);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    SoundEngine.PlaySound(SoundID.Item4);

                    //Spawn a constellation link

                    var source = Projectile.GetSource_FromThis();
                    Projectile chain = Projectile.NewProjectileDirect(source, Owner.Center, Vector2.Zero, ProjectileType<ArkoftheCosmosConstellation>(), (int)(Projectile.damage * ArkoftheCosmos.chainDamageMultiplier), 0, Owner.whoAmI, (int)(Projectile.timeLeft / 2f));
                    chain.timeLeft = (int)(Projectile.timeLeft / 2f);
                }

                //Rotate the blade towards the cursor
                Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Calamity().mouseWorld, 0.025f * ThrowRatio());
                Projectile.Center = Projectile.Center.MoveTowards(Owner.Calamity().mouseWorld, 20f * ThrowRatio());

                if ((Projectile.Center - Owner.Center).Length() > ArkoftheCosmos.MaxThrowReach)
                    Projectile.Center = Owner.Center + Owner.DirectionTo(Projectile.Center) * ArkoftheCosmos.MaxThrowReach;

                Projectile.rotation -= MathHelper.PiOver4 * 0.3f;
                Projectile.scale = 1f + ThrowScaleRatio() * 0.5f;

                //Come back
                if (Math.Abs(ThrowCompletion - SnapWindowEnd) <= 0.005f)
                    direction = Projectile.Center - Owner.Center;

                if (ThrowCompletion > SnapWindowEnd)
                    Projectile.Center = Owner.Center + direction * ThrowRatio();


                //Snip
                if (!OwnerCanShoot && Combo == 2 && ThrowCompletion >= (SnapWindowStart - 0.1f) && ThrowCompletion < SnapWindowEnd && ChanceMissed == 0f)
                {
                    Particle snapSpark = new GenericSparkle(Projectile.Center, Owner.velocity - Utils.SafeNormalize(Projectile.velocity, Vector2.Zero), Color.White, Color.OrangeRed, Main.rand.NextFloat(1f, 2f), 10 + Main.rand.Next(10), 0.1f, 3f);
                    GeneralParticleHandler.SpawnParticle(snapSpark);

                    if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;

                    if (Owner.whoAmI == Main.myPlayer)
                    {
                        float rotationOffset = MathHelper.TwoPi * Main.rand.NextFloat();

                        for (int i = 0; i < 3; i++)
                        {
                            var source = Projectile.GetSource_FromThis();
                            Projectile blast = Projectile.NewProjectileDirect(source, Projectile.Center + (MathHelper.TwoPi * (i / 3f) + rotationOffset).ToRotationVector2() * 30f, (MathHelper.TwoPi * (i / 3f) + rotationOffset).ToRotationVector2() * 20f, ProjectileType<EonBolt>(), (int)(ArkoftheCosmos.SnapBoltsDamageMultiplier * Projectile.damage), 0f, Owner.whoAmI, 0.55f, MathHelper.Pi * 0.05f);
                            {
                                blast.timeLeft = 100;
                            }
                        }

                        //Reset local immunity so that the snap can do damage
                        for (int i = 0; i < Main.maxNPCs; ++i)
                            Projectile.localNPCImmunity[i] = 0;
                    }

                    Combo = 3f; //Mark the end of the regular throw
                    direction = Projectile.Center - Owner.Center; //At this point direction becomes also a marker for the last position it was in before snapping
                    Projectile.velocity = Projectile.rotation.ToRotationVector2();
                    Projectile.timeLeft = (int)SnapEndTime;
                    Projectile.localNPCHitCooldown = (int)SnapEndTime; //Only snap the enemies ONCE
                }


                else if (!OwnerCanShoot && Combo == 2 && ChanceMissed == 0f)
                    ChanceMissed = 1f;

                if (Combo == 3f)
                {
                    //Slow down the projectile's retraction
                    float curveDownGently = MathHelper.Lerp(1f, 0.8f, 1f - (float)Math.Sqrt(1f - (float)Math.Pow(SnapEndCompletion , 2f)));
                    Projectile.Center = Owner.Center + direction * curveDownGently;
                    Projectile.scale = 1.5f;

                    float orientateProperly = (float)Math.Sqrt(1f - (float)Math.Pow(MathHelper.Clamp(SnapEndCompletion + 0.2f, 0f, 1f) - 1f, 2f));

                    float extraRotations = (direction.ToRotation() + MathHelper.PiOver4 > Projectile.velocity.ToRotation()) ? -MathHelper.TwoPi : 0f;

                    Projectile.rotation = MathHelper.Lerp(Projectile.velocity.ToRotation(), direction.ToRotation()  + extraRotations, orientateProperly);
                }

                //Sharticles
                DoParticleEffects(false);
            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = Projectile.whoAmI;
            Owner.direction = Math.Sign(Projectile.velocity.X);
            Owner.itemRotation = Projectile.rotation;
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= MathHelper.Pi;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);

        }

        public void DoParticleEffects(bool swirlSwing)
        {
            if (swirlSwing)
            {
                //This specific scale setting isn't actually used for the blade itself at all since it gets set to something else further down. This is just to use for the particles
                Projectile.scale = 1.6f + ((float)Math.Sin(SwirlRatio() * MathHelper.Pi) * 1f) + (Charge / 10f) * 0.05f;


                Color currentColor = Color.Chocolate * (MathHelper.Clamp((float)Math.Sin((SwirlRatio() - 0.2f) * MathHelper.Pi), 0f, 1f) * 0.8f);

                if (smear == null)
                {
                    smear = new CircularSmearSmokeyVFX(Owner.Center, currentColor, Projectile.rotation, Projectile.scale * 2.4f);
                    GeneralParticleHandler.SpawnParticle(smear);
                }
                //Update the variables of the smear
                else
                {
                    smear.Rotation = Projectile.rotation + MathHelper.PiOver4 + (Owner.direction < 0 ? MathHelper.PiOver4 * 4f : 0f);
                    smear.Time = 0;
                    smear.Position = Owner.Center;
                    smear.Scale = MathHelper.Lerp(2.6f, 3.5f, (Projectile.scale - 1.6f) / 1f);
                    smear.Color = currentColor;
                }

                if (Main.rand.NextBool())
                {
                    float maxDistance = Projectile.scale * 78f;
                    Vector2 distance = Main.rand.NextVector2Circular(maxDistance, maxDistance);
                    Vector2 angularVelocity = Utils.SafeNormalize(distance.RotatedBy(MathHelper.PiOver2 * Owner.direction), Vector2.Zero) * 2 * (1f + distance.Length() / 15f);
                    Particle glitter = new CritSpark(Owner.Center + distance, Owner.velocity + angularVelocity, Main.rand.Next(3) == 0 ? Color.Turquoise : Color.Coral, currentColor, 1f + 1 * (distance.Length() / maxDistance), 10, 0.05f, 3f);
                    GeneralParticleHandler.SpawnParticle(glitter);
                }


                float Opacity = MathHelper.Clamp(MathHelper.Clamp((float)Math.Sin((SwirlRatio() - 0.2f) * MathHelper.Pi), 0f, 1f) * 2f, 0, 1) * 0.25f;
                float scaleFactor = MathHelper.Clamp(MathHelper.Clamp((float)Math.Sin((SwirlRatio() - 0.2f) * MathHelper.Pi), 0f, 1f), 0, 1);

                if (Main.rand.NextBool())
                {
                    for (float i = 0f; i <= 1; i += 0.5f)
                    {
                        Vector2 smokepos = Owner.Center + (Projectile.rotation.ToRotationVector2() * (30 + 50 * i) * Projectile.scale) + Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2) * 30f * scaleFactor * Main.rand.NextFloat();
                        Vector2 smokespeed = Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2 * Owner.direction) * 20f * scaleFactor + Owner.velocity;

                        Particle smoke = new HeavySmokeParticle(smokepos, smokespeed, Color.Lerp(Color.DodgerBlue, Color.MediumVioletRed, i), 6 + Main.rand.Next(5), scaleFactor * Main.rand.NextFloat(2.8f, 3.1f), Opacity + Main.rand.NextFloat(0f, 0.2f), 0f, false, 0, true);
                        GeneralParticleHandler.SpawnParticle(smoke);

                        if (Main.rand.Next(3) == 0)
                        {
                            Particle smokeGlow = new HeavySmokeParticle(smokepos, smokespeed, Main.rand.Next(5) == 0 ? Color.Gold : Color.Chocolate, 5, scaleFactor * Main.rand.NextFloat(2f, 2.4f), Opacity * 2.5f, 0f, true, 0.004f, true);
                            GeneralParticleHandler.SpawnParticle(smokeGlow);
                        }
                    }
                }
            }

            else
            {
                //Wait why is this using **swing timers** for something in the throw? What? Well it looks good anyways so let's call this intentional :)
                Color smearColor = Main.hslToRgb(((SwingTimer - MaxSwingTime * 0.5f) / (MaxSwingTime * 0.5f)) * 0.15f, 1, 0.8f);
                float opacity = (Combo == 3f ? (float)Math.Sin(SnapEndCompletion * MathHelper.PiOver2 + MathHelper.PiOver2) : (float)Math.Sin(ThrowCompletion * MathHelper.Pi)) * 0.5f;

                if (smear == null)
                {
                    if (Charge <= 0)
                        smear = new TrientCircularSmear(Projectile.Center, smearColor * opacity, Projectile.rotation, Projectile.scale * 1.7f);
                    else
                        smear = new CircularSmearSmokeyVFX(Projectile.Center, smearColor * opacity, Projectile.rotation, Projectile.scale * 1.7f);
                    GeneralParticleHandler.SpawnParticle(smear);
                }
                //Update the variables of the smear
                else
                {
                    smear.Rotation = Projectile.rotation - 3.5f * MathHelper.PiOver4;
                    smear.Time = 0;
                    smear.Position = Projectile.Center;
                    smear.Scale = Projectile.scale * 1.65f;
                    smear.Color = smearColor * opacity;
                }


                if (Combo == 2f)
                {
                    if (Main.rand.NextBool())
                    {
                        float maxDistance = Projectile.scale * 78f;
                        Vector2 distance = Main.rand.NextVector2Circular(maxDistance, maxDistance);
                        Vector2 angularVelocity = Utils.SafeNormalize(distance.RotatedBy(-MathHelper.PiOver2), Vector2.Zero) * 2 * (1f + distance.Length() / 15f);
                        Color glitterColor = Main.hslToRgb(Main.rand.NextFloat(), 1, 0.5f);
                        Particle glitter = new CritSpark(Projectile.Center + distance, Owner.velocity + angularVelocity, Color.White , glitterColor, 1f + 1 * (distance.Length() / maxDistance), 10, 0.05f, 3f);
                        GeneralParticleHandler.SpawnParticle(glitter);
                    }


                    opacity = 0.25f;
                    float scaleFactor = 0.7f;

                    if (Main.rand.NextBool())
                    {
                        for (float i = 0.5f; i <= 1; i += 0.5f)
                        {
                            Vector2 smokepos = Projectile.Center + (Projectile.rotation.ToRotationVector2() * (60 * i) * Projectile.scale) + Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2) * 30f * scaleFactor * Main.rand.NextFloat();
                            Vector2 smokespeed = Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * 20f * scaleFactor + Owner.velocity;

                            Particle smoke = new HeavySmokeParticle(smokepos, smokespeed, Color.Lerp(Color.DodgerBlue, Color.MediumVioletRed, i), 10 + Main.rand.Next(5), scaleFactor * Main.rand.NextFloat(2.8f, 3.1f), opacity + Main.rand.NextFloat(0f, 0.2f), 0f, false, 0, true);
                            GeneralParticleHandler.SpawnParticle(smoke);

                            if (Main.rand.Next(3) == 0)
                            {
                                Particle smokeGlow = new HeavySmokeParticle(smokepos, smokespeed, Main.rand.Next(5) == 0 ? Color.Gold : Color.Chocolate, 7, scaleFactor * Main.rand.NextFloat(2f, 2.4f), opacity * 2.5f, 0f, true, 0.004f, true);
                                GeneralParticleHandler.SpawnParticle(smokeGlow);
                            }
                        }
                    }
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Combo == 3f)
                modifiers.SourceDamage *= ArkoftheElements.snapDamageMultiplier;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 particleSpeed = Utils.SafeNormalize(target.Center - Projectile.Center , Vector2.One).RotatedByRandom(MathHelper.PiOver4 * 0.8f) * Main.rand.NextFloat(3.6f, 8f);
                Particle energyLeak = new SquishyLightParticle(target.Center, particleSpeed, Main.rand.NextFloat(0.3f, 0.6f), Color.OrangeRed, 60, 2, 2.5f, hueShift: 0.06f);
                GeneralParticleHandler.SpawnParticle(energyLeak);
            }

            if (Combo == 3f)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.ScissorGuillotineSnapSound with { Volume =CommonCalamitySounds.ScissorGuillotineSnapSound.Volume * 1.3f }, Projectile.Center);

                if (Charge <= 1)
                {
                    ArkoftheCosmos sword = (Owner.HeldItem.ModItem as ArkoftheCosmos);
                    if (sword != null)
                        sword.Charge = 2f;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Combo == 3f)
            {
                if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                    Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;

                SoundEngine.PlaySound(SoundID.Item84, Projectile.Center);

                Vector2 sliceDirection = Utils.SafeNormalize(direction, Vector2.One) * 40;
                Particle SliceLine = new LineVFX(Projectile.Center - sliceDirection, sliceDirection * 2f, 0.2f, Color.Orange * 0.7f, expansion : 250f)
                {
                    Lifetime = 10
                };
                GeneralParticleHandler.SpawnParticle(SliceLine);

            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Thrown)
            {
                if (Charge > 0)
                    DrawSwungScissors(lightColor);
                else
                    DrawSingleSwungScissorBlade(lightColor);
            }
            else
            {
                if (Charge > 0)
                    DrawThrownScissors(lightColor);
                else
                    DrawSingleThrownScissorBlade(lightColor);
            }
            return false;
        }

        public void DrawSingleSwungScissorBlade(Color lightColor)
        {
            Texture2D sword = Request<Texture2D>(Combo == 0 ? "CalamityMod/Projectiles/Melee/SunderingScissorsRight" : "CalamityMod/Projectiles/Melee/SunderingScissorsLeft").Value;
            Texture2D glowmask = Request<Texture2D>(Combo == 0 ? "CalamityMod/Projectiles/Melee/SunderingScissorsRightGlow" : "CalamityMod/Projectiles/Melee/SunderingScissorsLeftGlow").Value;

            bool flipped = Owner.direction < 0;
            SpriteEffects flip = flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float extraAngle = Owner.direction < 0 ? MathHelper.PiOver2 : 0f;

            float drawAngle = Projectile.rotation;
            float angleShift = MathHelper.PiOver4;
            float drawRotation = Projectile.rotation + angleShift + extraAngle;

            Vector2 drawOrigin = new Vector2(flipped ? sword.Width : 0f, sword.Height);
            Vector2 drawOffset = Owner.Center + drawAngle.ToRotationVector2() * 10f - Main.screenPosition;

            if (CalamityConfig.Instance.Afterimages && SwingTimer > ProjectileID.Sets.TrailCacheLength[Projectile.type] && Combo == 0f)
            {
                for (int i = 1; i < Projectile.oldRot.Length; ++i)
                {
                    Color color = Main.hslToRgb((i / (float)Projectile.oldRot.Length) * 0.1f, 1, 0.6f + (Charge > 0 ? 0.3f : 0f));
                    float afterimageRotation = Projectile.oldRot[i] + angleShift + extraAngle;
                    Main.spriteBatch.Draw(glowmask, drawOffset, null, color * 0.05f, afterimageRotation, drawOrigin, Projectile.scale - 0.2f * ((i / (float)Projectile.oldRot.Length)), flip, 0f);
                }
            }

            Main.EntitySpriteDraw(sword, drawOffset, null, lightColor, drawRotation, drawOrigin, Projectile.scale, flip, 0);
            Main.EntitySpriteDraw(glowmask, drawOffset, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, Projectile.scale, flip, 0);

            if (SwingCompletion > 0.5f && Combo == 0f)
            {
                Texture2D smear = Request<Texture2D>("CalamityMod/Particles/TrientCircularSmear").Value;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                float opacity = (float)Math.Sin(SwingCompletion * MathHelper.Pi);
                float rotation = (-MathHelper.PiOver4 * 0.5f + MathHelper.PiOver4 * 0.5f * SwingCompletion + (Combo == 1f ? MathHelper.PiOver4 : 0)) * SwingDirection;
                Color smearColor = Main.hslToRgb(((SwingTimer - MaxSwingTime * 0.5f) / (MaxSwingTime * 0.5f)) * 0.15f + ((Combo == 1f) ? 0.85f : 0f), 1, 0.6f);

                Main.EntitySpriteDraw(smear, Owner.Center - Main.screenPosition, null, smearColor * 0.5f * opacity, Projectile.velocity.ToRotation() + MathHelper.Pi + rotation, smear.Size() / 2f, Projectile.scale * 2.3f, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        public void DrawSwungScissors(Color lightColor)
        {
            Texture2D frontBlade = Request<Texture2D>("CalamityMod/Projectiles/Melee/SunderingScissorsLeft").Value;
            Texture2D frontBladeGlow = Request<Texture2D>("CalamityMod/Projectiles/Melee/SunderingScissorsLeftGlow").Value;
            Texture2D backBlade = Request<Texture2D>("CalamityMod/Projectiles/Melee/SunderingScissorsRight").Value;
            Texture2D backBladeGlow = Request<Texture2D>("CalamityMod/Projectiles/Melee/SunderingScissorsRightGlow").Value;

            //Front blade
            bool flipped = Owner.direction < 0;
            SpriteEffects flip = flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float extraAngle = flipped ? MathHelper.PiOver2 : 0f;

            float drawAngle = Projectile.rotation;
            float angleShift = MathHelper.PiOver4;
            float drawRotation = Projectile.rotation + angleShift + extraAngle;

            Vector2 drawOrigin = new Vector2(flipped ? frontBlade.Width : 0f, frontBlade.Height);
            Vector2 drawOffset = Owner.Center + drawAngle.ToRotationVector2() * 10f - Main.screenPosition;

            //Back blade
            Vector2 backScissorOrigin = new Vector2(flipped ? 90 : 44f, 86); //Lined up with the hole in the scissor blade

            Vector2 backScissorDrawPosition = Owner.Center + drawAngle.ToRotationVector2() * 10f + (drawAngle.ToRotationVector2() * 56f + (drawAngle - MathHelper.PiOver2).ToRotationVector2() * 11 * Owner.direction) * Projectile.scale - Main.screenPosition; //Lined up with the hole in the front blade

            if (CalamityConfig.Instance.Afterimages && SwingTimer > ProjectileID.Sets.TrailCacheLength[Projectile.type])
            {
                Texture2D afterimage = Request<Texture2D>("CalamityMod/Projectiles/Melee/SunderingScissorsGlow").Value;

                for (int i = 1; i < Projectile.oldRot.Length; ++i)
                {
                    Color color = Main.hslToRgb((i / (float)Projectile.oldRot.Length) * 0.1f, 1, 0.6f + (Charge > 0 ? 0.3f : 0f));
                    float afterimageRotation = Projectile.oldRot[i] + angleShift + extraAngle;

                    Main.EntitySpriteDraw(afterimage, drawOffset, null, color * 0.15f, afterimageRotation, drawOrigin, Projectile.scale - 0.2f * ((i / (float)Projectile.oldRot.Length)), flip, 0);
                }
            }

            Main.EntitySpriteDraw(backBlade, backScissorDrawPosition, null, lightColor, drawRotation, backScissorOrigin, Projectile.scale, flip, 0);
            Main.EntitySpriteDraw(backBladeGlow, backScissorDrawPosition, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, backScissorOrigin, Projectile.scale, flip, 0);

            Main.EntitySpriteDraw(frontBlade, drawOffset, null, lightColor, drawRotation, drawOrigin, Projectile.scale, flip, 0);
            Main.EntitySpriteDraw(frontBladeGlow, drawOffset, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, Projectile.scale, flip, 0);

            if (SwingCompletion > 0.5f && Combo == 0f)
            {
                Texture2D smear = Request<Texture2D>("CalamityMod/Particles/TrientCircularSmear").Value;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                float opacity = (float)Math.Sin(SwingCompletion * MathHelper.Pi);
                float rotation = (-MathHelper.PiOver4 * 0.5f + MathHelper.PiOver4 * 0.5f * SwingCompletion + (Combo == 1f ? MathHelper.PiOver4 : 0)) * SwingDirection;
                Color smearColor = Main.hslToRgb(((SwingTimer - MaxSwingTime * 0.5f) / (MaxSwingTime * 0.5f)) * 0.15f + ((Combo == 1f) ? 0.85f : 0f), 1, 0.6f);

                Main.EntitySpriteDraw(smear, Owner.Center - Main.screenPosition, null, smearColor * 0.5f * opacity, Projectile.velocity.ToRotation() + MathHelper.Pi + rotation, smear.Size() / 2f, Projectile.scale * 2.3f, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        public void DrawSingleThrownScissorBlade(Color lightColor)
        {
            Texture2D sword = Request<Texture2D>("CalamityMod/Projectiles/Melee/SunderingScissorsLeft").Value;
            Texture2D glowmask = Request<Texture2D>("CalamityMod/Projectiles/Melee/SunderingScissorsLeftGlow").Value;

            if (Combo == 3f)
            {
                Texture2D thrownSword = Request<Texture2D>("CalamityMod/Projectiles/Melee/SunderingScissorsRight").Value;
                Texture2D thrownGlowmask = Request<Texture2D>("CalamityMod/Projectiles/Melee/SunderingScissorsRightGlow").Value;

                Vector2 drawPos2 = Vector2.SmoothStep(Owner.Center, Projectile.Center, MathHelper.Clamp(SnapEndCompletion + 0.25f, 0f, 1f));
                float drawRotation2 = direction.ToRotation() + MathHelper.PiOver4;
                Vector2 drawOrigin2 = new Vector2(44, 86);

                Main.EntitySpriteDraw(thrownSword, drawPos2 - Main.screenPosition, null, lightColor, drawRotation2, drawOrigin2, Projectile.scale, 0f, 0);
                Main.EntitySpriteDraw(thrownGlowmask, drawPos2 - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation2, drawOrigin2, Projectile.scale, 0f, 0);
            }

            Vector2 drawPos = Projectile.Center;
            float drawRotation = Projectile.rotation + MathHelper.PiOver4;
            Vector2 drawOrigin = new Vector2(32, 86); //Right on the hole. Well tbh here its not the hole theres a gem on it but you get me.

            Main.EntitySpriteDraw(sword, drawPos - Main.screenPosition, null, lightColor, drawRotation, drawOrigin, Projectile.scale, 0f, 0);
            Main.EntitySpriteDraw(glowmask, drawPos - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, Projectile.scale, 0f, 0);
        }

        public void DrawThrownScissors(Color lightColor)
        {
            Texture2D frontBlade = Request<Texture2D>("CalamityMod/Projectiles/Melee/SunderingScissorsLeft").Value;
            Texture2D frontBladeGlow = Request<Texture2D>("CalamityMod/Projectiles/Melee/SunderingScissorsLeftGlow").Value;
            Texture2D backBlade = Request<Texture2D>("CalamityMod/Projectiles/Melee/SunderingScissorsRight").Value;
            Texture2D backBladeGlow = Request<Texture2D>("CalamityMod/Projectiles/Melee/SunderingScissorsRightGlow").Value;

            Vector2 drawPos = Projectile.Center;
            Vector2 drawOrigin = new Vector2(32, 86);
            float drawRotation = Projectile.rotation + MathHelper.PiOver4;

            Vector2 drawOrigin2 = new Vector2(44, 86); //Right on the hole
            float drawRotation2 = Projectile.rotation + MathHelper.Lerp(MathHelper.PiOver4, MathHelper.PiOver2 * 1.33f, MathHelper.Clamp(ThrowCompletion * 2f, 0f, 1f));

            if (Combo == 3f)
                drawRotation2 = Projectile.rotation + MathHelper.Lerp(MathHelper.PiOver2 * 1.33f, MathHelper.PiOver4, MathHelper.Clamp(SnapEndCompletion + 0.5f, 0f, 1f));

            Main.EntitySpriteDraw(backBlade, drawPos - Main.screenPosition, null, lightColor, drawRotation2, drawOrigin2, Projectile.scale, 0f, 0);
            Main.EntitySpriteDraw(backBladeGlow, drawPos - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation2, drawOrigin2, Projectile.scale, 0f, 0);

            Main.EntitySpriteDraw(frontBlade, drawPos - Main.screenPosition, null, lightColor, drawRotation, drawOrigin, Projectile.scale, 0f, 0);
            Main.EntitySpriteDraw(frontBladeGlow, drawPos - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, Projectile.scale, 0f, 0);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
            writer.WriteVector2(direction);
            writer.Write(ChanceMissed);
            writer.Write(ThrowReach);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            direction = reader.ReadVector2();
            ChanceMissed = reader.ReadSingle();
            ThrowReach = reader.ReadSingle();
        }
    }
}
