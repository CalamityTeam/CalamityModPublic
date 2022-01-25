using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{

    // General cosmic bolt projectile that gets used for a lot of the other ones

    public class GalaxiaBolt : ModProjectile
    {
        public NPC target;
        public Player Owner => Main.player[projectile.owner];

        public ref float Hue => ref projectile.ai[0];
        public ref float HomingStrenght => ref projectile.ai[1];

        Particle Head;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galaxia Bolt");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 80;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Head == null)
            {
                Head = new GenericSparkle(projectile.Center, Vector2.Zero, Color.White, Main.hslToRgb(Hue, 100, 50), 1.2f, 2, 0.06f, 3);
                GeneralParticleHandler.SpawnParticle(Head);
            }
            else
            {
                Head.Position = projectile.Center + projectile.velocity * 0.5f;
                Head.Time = 0;
                Head.Scale += (float)Math.Sin(Main.GlobalTime * 6) * 0.02f * projectile.scale;
            }


            if (target == null)
                target = projectile.Center.ClosestNPCAt(812f, true);

            else if (CalamityUtils.AngleBetween(projectile.velocity, target.Center - projectile.Center) < MathHelper.PiOver2) //Home in
            {
                float idealDirection = projectile.AngleTo(target.Center);
                float updatedDirection = projectile.velocity.ToRotation().AngleTowards(idealDirection, HomingStrenght);
                projectile.velocity = updatedDirection.ToRotationVector2() * projectile.velocity.Length() * 0.995f;
            }


            Lighting.AddLight(projectile.Center, 0.75f, 1f, 0.24f);

            if (Main.rand.Next(2) == 0)
            {
                Particle smoke = new HeavySmokeParticle(projectile.Center, projectile.velocity * 0.5f, Color.Lerp(Color.MidnightBlue, Color.Indigo, (float)Math.Sin(Main.GlobalTime * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f) * projectile.scale, 0.8f, 0, false, 0, true);
                GeneralParticleHandler.SpawnParticle(smoke);

                if (Main.rand.Next(3) == 0)
                {
                    Particle smokeGlow = new HeavySmokeParticle(projectile.Center, projectile.velocity * 0.5f, Main.hslToRgb(Hue, 1, 0.7f), 20, Main.rand.NextFloat(0.4f, 0.7f) * projectile.scale, 0.8f, 0, true, 0.005f, true);
                    GeneralParticleHandler.SpawnParticle(smokeGlow);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            //Shit out a lot of particles or smth
        }
    }


    //Phoenix's pride aka Whirlwind attunement

    public class PhoenixsPride : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalaxiaExtra2"; //The blue one, since this is long range
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float CurrentState => ref projectile.ai[0];
        public Player Owner => Main.player[projectile.owner];
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;
        public const float throwOutTime = 90f;
        public const float throwOutDistance = 440f;

        public static float snapPoint = 0.45f;
        public float snapTimer => (throwTimer / throwOutTime) < snapPoint ? 0 : ((throwTimer / throwOutTime) - snapPoint) / (1f - snapPoint);
        public static float retractionPoint = 0.6f;
        public float retractionTimer => (throwTimer / throwOutTime) < retractionPoint ? 0 : ((throwTimer / throwOutTime) - retractionPoint) / (1f - retractionPoint);
        public ref float Empowerment => ref projectile.ai[1];
        public float OverEmpowerment = 0f; //Used to keep cooldowns working when the spin is full
        public ref float hasMadeSound => ref projectile.localAI[0];
        public ref float hasMadeChargeSound => ref projectile.localAI[1];

        public const float maxEmpowerment = 600f;
        public float throwTimer => throwOutTime - projectile.timeLeft;


        public float AngleReset = 0f;
        public bool CanDirectFire = true;
        public Particle smear;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pohenix's Pride");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 74;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = FourSeasonsGalaxia.WhirlwindAttunement_LocalIFrames;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 145 * projectile.scale;
            float bladeWidth = 25 * projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + (direction * bladeLenght), bladeWidth, ref collisionPoint);
        }


        public CurveSegment launch = new CurveSegment(EasingType.CircOut, 0f, 0f, 1f, 4);
        public CurveSegment hold = new CurveSegment(EasingType.Linear, snapPoint, 1f, 0f);
        public CurveSegment retract = new CurveSegment(EasingType.PolyInOut, retractionPoint, 1f, -1.05f, 3);
        internal float ThrowCurve() => PiecewiseAnimation((throwOutTime - projectile.timeLeft) / throwOutTime, new CurveSegment[] { launch, hold, retract });

        public override void AI()
        {
            if (!initialized) //Initialization. Here its litterally just playing a sound tho lmfao
            {
                Main.PlaySound(SoundID.Item90, projectile.Center);
                projectile.velocity = Vector2.Zero;
                direction = Owner.DirectionTo(Main.MouseWorld);
                direction.Normalize();
                initialized = true;
            }

            projectile.rotation = direction.ToRotation(); //Only done for afterimages

            if (!OwnerCanShoot)
            {
                if (CurrentState == 2f || (CurrentState == 0f && Empowerment / maxEmpowerment < 0.5))
                {
                    Main.PlaySound(SoundID.Item77, projectile.Center);
                    projectile.Kill();
                    return;
                }

                else if (CurrentState == 0f)
                {
                    CurrentState = 1f;
                    if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;

                    Main.PlaySound(SoundID.Item80, projectile.Center);
                    direction = Owner.DirectionTo(Main.MouseWorld);
                    //PARTICLES LOTS OF PARTICLES LOTS OF SPARKLES YES YES MH YES YES 
                    for (int i = 0; i <= 8; i++)
                    {
                        float variation = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
                        float strength = (float)Math.Sin(variation * 2f + MathHelper.PiOver2);
                        Particle Sparkle = new CritSpark(projectile.Center, Owner.velocity + direction.RotatedBy(variation) * (1 + strength) * 2f * Main.rand.NextFloat(7.5f, 20f), Color.Coral, Color.Chocolate, 2f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 2f);
                        GeneralParticleHandler.SpawnParticle(Sparkle);
                    }

                    //Actually do projectiles
                    for (int i = 0; i <= 5; i++)
                    {
                        float angle = direction.ToRotation() + MathHelper.Lerp(-MathHelper.PiOver4, MathHelper.PiOver4, i / 5f);
                        Projectile.NewProjectile(Owner.Center, angle.ToRotationVector2() * 30f, ProjectileType<GalaxiaBolt>(), (int)(projectile.damage * FourSeasonsGalaxia.WhirlwindAttunement_BoltThrowDamageMultiplier), 0f, Owner.whoAmI, 0.1f, MathHelper.Pi * 0.02f);
                    }
                }
            }

            if (CurrentState == 0f)
            {
                if (Owner.whoAmI == Main.myPlayer)
                {
                    if (hasMadeChargeSound == 0f && Empowerment / maxEmpowerment >= 0.5)
                    {
                        hasMadeChargeSound = 1f;
                        Main.PlaySound(SoundID.Item76);
                    }

                    float rotation = direction.ToRotation();
                    if (rotation > -MathHelper.PiOver2 - MathHelper.PiOver4 && rotation < -MathHelper.PiOver2 + MathHelper.PiOver4 && hasMadeSound == 1f)
                        hasMadeSound = 0f;

                    else if (rotation > MathHelper.PiOver2 - MathHelper.PiOver4 && rotation < MathHelper.PiOver2 + MathHelper.PiOver4 && hasMadeSound == 0f)
                    {
                        CanDirectFire = true;
                        hasMadeSound = 1f;
                        Main.PlaySound(SoundID.Item71);
                    }
                }

                if (Empowerment / maxEmpowerment >= 0.5) //Double the charge speed when under half speed
                {
                    Empowerment++;
                }


                if (Empowerment / maxEmpowerment >= 0.5)
                {

                    if (Main.rand.Next(2) == 0)

                    {

                        float Opacity = MathHelper.Clamp((Empowerment / maxEmpowerment - 0.5f) * 3f, 0, 1) * 0.75f;
                        float scaleFactor = MathHelper.Clamp((Empowerment / maxEmpowerment - 0.5f) * 3f, 0, 1);

                        for (float i = 0f; i <= 1; i += 0.5f)
                        {
                            Vector2 smokepos = projectile.Center + (direction * (60 + 40 * i) * projectile.scale) + direction.RotatedBy(-MathHelper.PiOver2) * 30f * scaleFactor * Main.rand.NextFloat();


                            Particle smoke = new HeavySmokeParticle(smokepos, direction.RotatedBy(-MathHelper.PiOver2) * 20f * scaleFactor + Owner.velocity, Color.Lerp(Color.MidnightBlue, Color.Indigo, i), 10 + Main.rand.Next(5), scaleFactor * Main.rand.NextFloat(2.8f, 3.1f), Opacity + Main.rand.NextFloat(0f, 0.2f), 0f, false, 0, true);
                            GeneralParticleHandler.SpawnParticle(smoke);

                            if (Main.rand.Next(3) == 0)
                            {
                                Particle smokeGlow = new HeavySmokeParticle(smokepos, direction.RotatedBy(-MathHelper.PiOver2) * 20f * scaleFactor + Owner.velocity, Color.OrangeRed, 7, scaleFactor * Main.rand.NextFloat(2f, 2.4f), Opacity * 2, 0f, true, 0.001f, true);
                                GeneralParticleHandler.SpawnParticle(smokeGlow);
                            }
                        }
                    }

                    if ((Empowerment + OverEmpowerment) % 30 == 29)
                    {
                        Vector2 shotDirection = Main.rand.NextVector2CircularEdge(15f, 15f);
                        Projectile.NewProjectile(Owner.Center, shotDirection, ProjectileType<GalaxiaBolt>(), (int)(projectile.damage * FourSeasonsGalaxia.WhirlwindAttunement_BoltDamageReduction), 0f, Owner.whoAmI, 0.1f, MathHelper.Pi * 0.02f);
                    }
                }


                if (Empowerment / maxEmpowerment >= 0.75)
                {
                    Color currentColor = Color.Chocolate * (((Empowerment / maxEmpowerment) - 0.75f) / 0.25f * 0.8f);
                    if (smear == null)
                    {
                        smear = new CircularSmearSmokeyVFX(Owner.Center, currentColor, direction.ToRotation(), projectile.scale * 1.5f);
                        GeneralParticleHandler.SpawnParticle(smear);
                    }
                    //Update the variables of the smear
                    else
                    {
                        smear.Rotation = direction.ToRotation() + MathHelper.PiOver2;
                        smear.Time = 0;
                        smear.Position = Owner.Center;
                        smear.Scale = projectile.scale * 1.9f;
                        smear.Color = currentColor;
                    }

                    float rotationAdjusted = MathHelper.WrapAngle(projectile.rotation) + MathHelper.Pi;
                    float mouseAngleAdjusted = MathHelper.WrapAngle(Owner.DirectionTo(Main.MouseWorld).ToRotation()) + MathHelper.Pi;
                    float deltaAngleShoot = Math.Abs(MathHelper.WrapAngle(rotationAdjusted - mouseAngleAdjusted));

                    if (CanDirectFire && deltaAngleShoot < 0.1f)
                    {
                        Projectile.NewProjectile(Owner.Center, Owner.DirectionTo(Main.MouseWorld) * 15f, ProjectileType<GalaxiaBolt>(), (int)(projectile.damage * FourSeasonsGalaxia.WhirlwindAttunement_BoltDamageReduction), 0f, Owner.whoAmI, 0.1f, MathHelper.Pi * 0.02f);
                        CanDirectFire = false;
                        AngleReset = Owner.DirectionTo(Main.MouseWorld).ToRotation();
                    }


                    if (Main.rand.NextBool())
                    {
                        float maxDistance = projectile.scale * 1.9f * 78f;
                        Vector2 distance = Main.rand.NextVector2Circular(maxDistance, maxDistance);
                        Vector2 angularVelocity = Vector2.Normalize(distance.RotatedBy(MathHelper.PiOver2)) * 2f * (1f + distance.Length() / 15f);
                        Particle glitter = new CritSpark(Owner.Center + distance, Owner.velocity + angularVelocity, Main.rand.Next(3) == 0 ? Color.Turquoise : Color.Coral, currentColor, 1f + 1 * (distance.Length() / maxDistance), 10, 0.05f, 3f);
                        GeneralParticleHandler.SpawnParticle(glitter);
                    }
                }


                //Manage position and rotation
                projectile.scale = 1 + Empowerment / maxEmpowerment * 1.5f;

                direction = direction.RotatedBy(MathHelper.Clamp(Empowerment / maxEmpowerment, 0.4f, 1f) * MathHelper.PiOver4 * 0.20f);
                direction.Normalize();
                projectile.rotation = direction.ToRotation();
                projectile.Center = Owner.Center + (direction * projectile.scale * 10);
                projectile.timeLeft = (int)throwOutTime + 1;
                Empowerment++;
                if (Empowerment > maxEmpowerment)
                {
                    Empowerment = maxEmpowerment;
                    OverEmpowerment++;
                }
            }

            if (CurrentState == 1f)
            {
                projectile.Center = Owner.Center + (direction * projectile.scale * 10) + (direction * throwOutDistance * ThrowCurve());
                projectile.scale = (1 + Empowerment / maxEmpowerment * 1.5f) * MathHelper.Clamp(1 - retractionTimer, 0.3f, 1f);
            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            Owner.direction = Math.Sign(direction.X);
            Owner.itemRotation = direction.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * (OmegaBiomeBlade.WhirlwindAttunement_BaseDamageReduction + (OmegaBiomeBlade.WhirlwindAttunement_FullChargeDamageBoost * Empowerment / maxEmpowerment)));
        }

        public override void Kill(int timeLeft)
        {
            if (smear != null)
                smear.Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D sword = GetTexture("CalamityMod/Items/Weapons/Melee/GalaxiaExtra2");

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, sword.Height);
            Vector2 drawOffset = projectile.Center - Main.screenPosition;

            //Afterimages
            if (CalamityConfig.Instance.Afterimages && CurrentState == 0f && Empowerment / maxEmpowerment > 0.4f)
            {
                for (int i = 0; i < projectile.oldRot.Length; ++i)
                {
                    Color color = projectile.GetAlpha(lightColor) * (1f - (i / (float)projectile.oldRot.Length));
                    float afterimageRotation = projectile.oldRot[i] + MathHelper.PiOver4;
                    Main.spriteBatch.Draw(sword, drawOffset, null, color * MathHelper.Lerp(0f, 0.5f, MathHelper.Clamp((Empowerment - maxEmpowerment * 0.4f) / (maxEmpowerment * 0.6f), 0f, 1f)), afterimageRotation, drawOrigin, projectile.scale - 0.2f * ((i / (float)projectile.oldRot.Length)), 0f, 0f);
                }
            }

            spriteBatch.Draw(sword, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            //Turn on additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //Update the parameters

            //Don't draw the glowing blade after the retraction
            float opacityFade = CurrentState == 0f ? 1f : 1f - retractionTimer;
            //Add basic tint (my beloved) during the snap
            if (snapTimer > 0 && retractionTimer <= 0)
            {
                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(MathHelper.Clamp(0.8f - snapTimer, 0f, 1f));
                GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Color.White);
                GameShaders.Misc["CalamityMod:BasicTint"].Apply();
            }

            if (CurrentState == 1f && snapTimer > 0)
            {
                drawChain(spriteBatch, snapTimer, retractionTimer);
            }

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public void drawChain(SpriteBatch spriteBatch, float snapProgress, float retractProgress)
        {
            Texture2D chainTex = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_HeavensMightChain");

            float opacity = retractProgress < 0.5 ? 1 : (retractProgress - 0.5f) / 0.5f;

            Vector2 Shake = retractProgress > 0 ? Vector2.Zero : Vector2.One.RotatedByRandom(MathHelper.TwoPi) * (1f - snapProgress) * 10f;

            int dist = (int)Vector2.Distance(Owner.Center, projectile.Center) / 16;
            Vector2[] Nodes = new Vector2[dist + 1];
            Nodes[0] = Owner.Center;
            Nodes[dist] = projectile.Center;

            for (int i = 1; i < dist + 1; i++)
            {
                Rectangle frame = new Rectangle(0, 0 + 18 * (i % 2), 12, 18);
                Vector2 positionAlongLine = Vector2.Lerp(Owner.Center, projectile.Center, i / (float)dist); //Get the position of the segment along the line, as if it were a flat line
                Nodes[i] = positionAlongLine + Shake * (float)Math.Sin(i / (float)dist * MathHelper.Pi);

                float rotation = (Nodes[i] - Nodes[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(Nodes[i], Nodes[i - 1]) / frame.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                Vector2 scale = new Vector2(1, yScale);

                Color chainLightColor = Lighting.GetColor((int)Nodes[i].X / 16, (int)Nodes[i].Y / 16); //Lighting of the position of the chain segment

                Vector2 origin = new Vector2(frame.Width / 2, frame.Height); //Draw from center bottom of texture
                spriteBatch.Draw(chainTex, Nodes[i] - Main.screenPosition, frame, chainLightColor * opacity * 0.7f, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
            writer.WriteVector2(direction);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            direction = reader.ReadVector2();
        }
    }


    //Polaris's gaze aka Super Pogo attunement

    public class PolarissGaze : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalaxiaExtra"; //Red cuz close range yget the deal
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref projectile.ai[0]; //Charge, basically
        public ref float HitChargeCooldown => ref projectile.ai[1];
        public float Bounce(float x) => x <= 50 ? x / 50f : x <= 65 ? 1 + 0.15f * (float)Math.Sin((x - 50f) / 15f * MathHelper.Pi) : 1f;
        public float ShredRatio => MathHelper.Clamp(Shred / (maxShred * 0.5f), 0f, 1f);
        public Player Owner => Main.player[projectile.owner];
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;

        public const float maxShred = 750; //How much shred you get

        public Projectile Wheel;
        public bool Dashing;
        public Vector2 DashStart;

        public Particle[] Rings = new Particle[3];
        public Particle PolarStar;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polaris's Gaze");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 70;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = FourSeasonsGalaxia.SuperPogoAttunement_LocalIFrames;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 145 * projectile.scale;
            float bladeWidth = 86 * projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Owner.Center + (direction * bladeLenght), bladeWidth, ref collisionPoint);
        }

        public override void AI()
        {
            if (!initialized) //Initialization. 
            {
                Main.PlaySound(SoundID.Item90, projectile.Center);
                initialized = true;

                foreach(Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ProjectileType<PolarissGazeStar>() && proj.owner == Owner.whoAmI)
                    {
                        if (CalamityUtils.AngleBetween(Owner.Center - Main.MouseWorld, Owner.Center - proj.Center) > MathHelper.PiOver4)
                        {
                            proj.Kill();
                            break;
                        }

                        Wheel = proj;
                        Dashing = true;
                        DashStart = Owner.Center;
                        Wheel.timeLeft = 60;
                        Owner.GiveIFrames(FourSeasonsGalaxia.SuperPogoAttunement_SlashIFrames);
                        break;
                    }
                }
            }

            if (!OwnerCanShoot)
            {
                projectile.Kill();
                return;
            }


            #region sparkles and particles

            float bladeLenght = 120 * projectile.scale;
            for (int i = 0; i < 3; i ++)
            {
                if (Rings[i] == null)
                {
                    Rings[i] = new ConstellationRingVFX(Owner.Center + (direction * (25 + bladeLenght * 0.33f * i)), Color.DarkOrchid, direction.ToRotation(), projectile.scale * 0.25f * i, new Vector2(0.5f, 1f), spinSpeed: 7, starAmount: 3 + i, important : true) ;
                    GeneralParticleHandler.SpawnParticle(Rings[i]);
                }
                else
                {
                    Rings[i].Time = 0;
                    Rings[i].Position = Owner.Center + Vector2.Lerp(Vector2.Zero, ((0.9f + 0.1f * (float)Math.Sin(Main.GlobalTime * 10f - i * 0.5f)) * direction * (25 + bladeLenght * 0.33f * (i + 1))), Bounce(MathHelper.Clamp(Shred - 30 * i, 0, maxShred)));
                    Rings[i].Rotation = direction.ToRotation();
                    Rings[i].Scale = projectile.scale * 0.25f * (i + 1);
                    (Rings[i] as ConstellationRingVFX).Opacity = 0.5f + 0.5f * ShredRatio;
                }
            }

            if (PolarStar == null)
            {
                PolarStar = new GenericSparkle(Owner.Center + direction, Vector2.Zero, Color.White, Color.CornflowerBlue, projectile.scale, 2, 0.05f, 5f);
                GeneralParticleHandler.SpawnParticle(PolarStar);
            }
            else
            {
                PolarStar.Time = 0;
                PolarStar.Position = Owner.Center + direction * 46 * projectile.scale;
                PolarStar.Rotation += (1 + (float)Math.Sin(Main.GlobalTime * 4f)) * 0.02f;
                PolarStar.Scale = projectile.scale * 2f ;
            }

            if (Main.rand.NextBool())
            { 
                Vector2 smokeSpeed = direction.RotatedByRandom(MathHelper.PiOver4 * 0.3f) * Main.rand.NextFloat(10f, 30f) * (ShredRatio * 0.5f + 1f);
                Particle smoke = new HeavySmokeParticle(projectile.Center, smokeSpeed + Owner.velocity, Color.Lerp(Color.Purple, Color.Indigo, (float)Math.Sin(Main.GlobalTime * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f) * (ShredRatio * 0.5f + 1f), 0.8f, 0, false, 0, true);
                GeneralParticleHandler.SpawnParticle(smoke);

                if (Main.rand.Next(3) == 0)
                {
                    Particle smokeGlow = new HeavySmokeParticle(projectile.Center, smokeSpeed + Owner.velocity, Main.hslToRgb(0.55f, 1, 0.5f + 0.2f * ShredRatio), 20, Main.rand.NextFloat(0.4f, 0.7f) * (ShredRatio * 0.5f + 1f), 0.8f, 0, true, 0.01f, true);
                    GeneralParticleHandler.SpawnParticle(smokeGlow);
                }
            }
            #endregion

            if (Shred >= maxShred)
                Shred = maxShred;
            if (Shred < 0)
                Shred = 0;

            //Manage position and rotation
            direction = Owner.DirectionTo(Main.MouseWorld);
            direction.Normalize();
            projectile.rotation = direction.ToRotation();
            projectile.Center = Owner.Center + (direction * 60);

            //Scaling based on shred
            projectile.localNPCHitCooldown = FourSeasonsGalaxia.SuperPogoAttunement_LocalIFrames - (int)(MathHelper.Lerp(0, FourSeasonsGalaxia.SuperPogoAttunement_LocalIFrames - FourSeasonsGalaxia.SuperPogoAttunement_LocalIFramesCharged, ShredRatio)); //Increase the hit frequency
            projectile.scale = 1f + (ShredRatio * 1f); //SWAGGER


            if ((Wheel == null || !Wheel.active) && Dashing)
            {
                Dashing = false;
                Owner.velocity *= 0.1f; //Abrupt stop

                for (int i = 0; i < 5; i++)
                {
                    Projectile blast = Projectile.NewProjectileDirect(Owner.Center, Main.rand.NextVector2CircularEdge(15, 15), ProjectileType<GalaxiaBolt>(), (int)(FourSeasonsGalaxia.SuperPogoAttunement_SlashBoltsDamage * Owner.meleeDamage), 0f, Owner.whoAmI, 0.55f, MathHelper.Pi * 0.02f);
                    {
                        blast.timeLeft = 100;
                    }
                }

                Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/MeatySlash"), projectile.Center);
                Projectile proj = Projectile.NewProjectileDirect(Owner.Center - DashStart / 2f, Vector2.Zero, ProjectileType<PolarissGazeDash>(), (int)(projectile.damage * FourSeasonsGalaxia.SuperPogoAttunement_SlashDamageBoost), 0, Owner.whoAmI);
                if (proj.modProjectile is PolarissGazeDash dash)
                {
                    dash.DashStart = DashStart;
                    dash.DashEnd = Owner.Center;
                }

            }

            Owner.Calamity().LungingDown = false;

            if (Dashing)
            {
                Owner.Calamity().LungingDown = true;
                Owner.fallStart = (int)(Owner.position.Y / 16f);
                Owner.velocity = Owner.DirectionTo(Wheel.Center) * 60f;

                if (Owner.Distance(Wheel.Center) < 60f)
                    Wheel.active = false;
            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            Owner.direction = Math.Sign(direction.X);
            Owner.itemRotation = direction.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            Shred += 1;
            HitChargeCooldown--;
            projectile.timeLeft = 2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => ShredTarget();
        public override void OnHitPvp(Player target, int damage, bool crit) => ShredTarget();

        private void ShredTarget()
        {
            if (Main.myPlayer != Owner.whoAmI)
                return;

            Owner.fallStart = (int)(Owner.position.Y / 16f);
            // get lifted up
            if (HitChargeCooldown <= 0)
            {
                Main.PlaySound(SoundID.NPCHit30, projectile.Center); //Sizzle
                Shred += 80; //Augment the shredspeed
                if (Owner.velocity.Y > 0)
                    Owner.velocity.Y = -2f; //Get "stuck" into the enemy partly
                Owner.GiveIFrames(FourSeasonsGalaxia.SuperPogoAttunement_ShredIFrames); // i framez.
                HitChargeCooldown = 20;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCHit43, projectile.Center);
            if (ShredRatio > 0.85 && Owner.whoAmI == Main.myPlayer) 
            {
                Projectile.NewProjectile(projectile.Center, direction * 16f, ProjectileType<PolarissGazeStar>(), (int)(projectile.damage * FourSeasonsGalaxia.SuperPogoAttunement_ShotDamageBoost) , projectile.knockBack, Owner.whoAmI, Shred);
            }
            Owner.Calamity().LungingDown = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D sword = GetTexture("CalamityMod/Items/Weapons/Melee/GalaxiaExtra");

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, sword.Height);
            Vector2 drawOffset = Owner.Center + direction * 10f - Main.screenPosition;

            spriteBatch.Draw(sword, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
            writer.WriteVector2(direction);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            direction = reader.ReadVector2();
        }
    }

    public class PolarissGazeStar : ModProjectile 
    {
        public override string Texture => "CalamityMod/Particles/Sparkle";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref projectile.ai[0];
        public float ShredRatio => MathHelper.Clamp(Shred / (PolarissGaze.maxShred * 0.5f), 0f, 1f);
        public Player Owner => Main.player[projectile.owner];

        public float Timer => MaxTime - projectile.timeLeft;

        public const float MaxTime = 120;

        public Particle PolarStar; //Using a particle ontop of it since the smoke particles would otherwise go over it


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Northern Star");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 45;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity *= 0.95f;
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 84 * projectile.scale;
            float bladeWidth = 76 * projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - direction * bladeLenght / 2, projectile.Center + direction * bladeLenght / 2, bladeWidth, ref collisionPoint);
        }

        public override void AI()
        {
            if (!initialized)
            {
                Main.PlaySound(SoundID.Item90, projectile.Center);
                initialized = true;

                direction = Owner.DirectionTo(Main.MouseWorld);
                direction.Normalize();
                projectile.rotation = direction.ToRotation();

                projectile.timeLeft = (int)MaxTime;
                projectile.velocity = direction * 16f;

                projectile.scale = 1f + ShredRatio ; //SWAGGER
                projectile.netUpdate = true;

            }

            projectile.velocity *= 0.96f;
            projectile.position += projectile.velocity;


            if (PolarStar == null)
            {
                PolarStar = new GenericSparkle(projectile.Center, Vector2.Zero, Color.White, Color.CornflowerBlue, projectile.scale * 2f, 2, 0.1f, 5f);
                GeneralParticleHandler.SpawnParticle(PolarStar);
            }
            else
            {
                PolarStar.Time = 0;
                PolarStar.Position = projectile.Center;
                PolarStar.Scale = projectile.scale * 2f;
            }


            Vector2 smokeSpeed = Main.rand.NextVector2Circular(10f, 10f);
            Particle smoke = new HeavySmokeParticle(projectile.Center, smokeSpeed + projectile.velocity / 2, Color.Lerp(Color.Purple, Color.Indigo, (float)Math.Sin(Main.GlobalTime * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f), 0.8f, 0, false, 0, true);
            GeneralParticleHandler.SpawnParticle(smoke);

            if (Main.rand.Next(3) == 0)
            {
                Particle smokeGlow = new HeavySmokeParticle(projectile.Center, smokeSpeed + projectile.velocity / 2, Main.hslToRgb(0.55f, 1, 0.5f), 20, Main.rand.NextFloat(0.4f, 0.7f), 0.8f, 0, true, 0.01f, true);
                GeneralParticleHandler.SpawnParticle(smokeGlow);
            }

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            var tex = GetTexture("CalamityMod/Particles/Sparkle");
            float opacityFade = projectile.timeLeft > 15 ? 1 : projectile.timeLeft / 15f;

            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.5f * opacityFade, Main.GlobalTime * 10f + MathHelper.PiOver4, tex.Size() / 2f, projectile.scale * 1.5f, 0f, 0f);

            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.8f * opacityFade, Main.GlobalTime * 10f, tex.Size() / 2f, projectile.scale * 2f, 0f, 0f);
            
            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item60, projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Particle Sparkle = new CritSpark(projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(17.5f, 25f) * projectile.scale, Color.White, Main.rand.NextBool() ? Color.CornflowerBlue : Color.MediumSlateBlue, 0.4f + Main.rand.NextFloat(0f, 3.5f), 20 + Main.rand.Next(30), 1, 3f);
                GeneralParticleHandler.SpawnParticle(Sparkle);

                Vector2 smokeSpeed = Main.rand.NextVector2Circular(20f, 20f);
                Particle smoke = new HeavySmokeParticle(projectile.Center, smokeSpeed + projectile.velocity / 2, Color.Lerp(Color.DarkRed, Color.Indigo, (float)Math.Sin(Main.GlobalTime * 6f)), 30, Main.rand.NextFloat(1.5f, 2.2f), 0.8f, 0, false, 0, true);
                GeneralParticleHandler.SpawnParticle(smoke);

                if (Main.rand.Next(3) == 0)
                {
                    Particle smokeGlow = new HeavySmokeParticle(projectile.Center, smokeSpeed + projectile.velocity / 2, Main.hslToRgb(0.55f, 1, 0.5f), 20, Main.rand.NextFloat(1.4f, 1.5f), 0.8f, 0, true, 0.01f, true);
                    GeneralParticleHandler.SpawnParticle(smokeGlow);
                }
            }
        }

    }

    public class PolarissGazeDash : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[projectile.owner];
        public float Timer => 20 - projectile.timeLeft;

        public Vector2 DashStart;
        public Vector2 DashEnd;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shredding Lunge");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 8;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 60;
            projectile.timeLeft = 20;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), DashStart, DashEnd);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            crit = true;

            Particle bloom = new StrongBloom(target.Center, target.velocity, Color.Crimson * 0.5f, 1f, 30);
            GeneralParticleHandler.SpawnParticle(bloom);

            for (int i = 0; i < 3; i++)
            {
                Vector2 sparkSpeed = target.DirectionTo(Owner.Center).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * 9f;
                Particle Spark = new CritSpark(target.Center, sparkSpeed, Color.White, Color.CornflowerBlue, 1f + Main.rand.NextFloat(0, 1f), 30, 0.4f, 0.6f);
                GeneralParticleHandler.SpawnParticle(Spark);
            }

            //Explode into cosmic bolts
            for (int i = 0; i < 3; i++)
            {
                Projectile blast = Projectile.NewProjectileDirect(Owner.Center, Owner.DirectionTo(target.Center).RotatedByRandom(MathHelper.PiOver4) * 30f, ProjectileType<GalaxiaBolt>(), (int)(FourSeasonsGalaxia.SuperPogoAttunement_SlashBoltsDamage * Owner.meleeDamage), 0f, Owner.whoAmI, 0.55f, MathHelper.Pi * 0.01f);
                {
                    blast.timeLeft = 100;
                }
            }
        }



        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) //OMw to reuse way too much code from the entangling vines
        {

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D lineTex = GetTexture("CalamityMod/Particles/ThinEndedLine");

            Vector2 Shake = projectile.timeLeft < 15 ? Vector2.Zero : Vector2.One.RotatedByRandom(MathHelper.TwoPi) * (15 - projectile.timeLeft / 5f) * 0.5f;
            float bump = (float)Math.Sin(((20f - projectile.timeLeft) / 20f) * MathHelper.Pi);
            float raise = (float)Math.Sin(((20f - projectile.timeLeft) / 20f) * MathHelper.PiOver2);

            float rot = (DashEnd - DashStart).ToRotation() + MathHelper.PiOver2;
            Vector2 origin = new Vector2(lineTex.Width / 2f, lineTex.Height);
            Vector2 scale = new Vector2(0.2f, (DashEnd - DashStart).Length() / lineTex.Height);

            spriteBatch.Draw(lineTex, DashStart - Main.screenPosition + Shake, null, Color.Lerp(Color.White, Color.CornflowerBlue * 0.7f, raise), rot, origin, scale, SpriteEffects.None, 0);

            Texture2D sparkTexture = GetTexture("CalamityMod/Particles/ThinSparkle");
            Texture2D bloomTexture = GetTexture("CalamityMod/Particles/BloomCircle");
            //Ajust the bloom's texture to be the same size as the star's
            float properBloomSize = (float)sparkTexture.Width / (float)bloomTexture.Height;

            
            Rectangle frame = new Rectangle(0, 0, 14, 14);

            spriteBatch.Draw(bloomTexture, DashEnd - Main.screenPosition, null, Color.CornflowerBlue * bump * 0.5f, 0, bloomTexture.Size() / 2f, bump * 6f * properBloomSize, SpriteEffects.None, 0);
            spriteBatch.Draw(sparkTexture, DashEnd - Main.screenPosition, frame, Color.Lerp(Color.White, Color.CornflowerBlue, raise) * bump, raise * MathHelper.TwoPi, frame.Size() / 2f, bump * 3f, SpriteEffects.None, 0);

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }

    //Mercurial tides aka Shockwave attunement

    public class AndromedasStride : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalaxiaExtra";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public Player Owner => Main.player[projectile.owner];
        public ref float Charge => ref projectile.ai[0]; //Charge
        public ref float State => ref projectile.ai[1]; //State 0 is "charging", State 1 is "thrusting"
        public ref float CurrentIndicator => ref projectile.localAI[0]; //What "indicator" stage are you on. 
        public ref float OverCharge => ref projectile.localAI[1];

        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;
        const float MaxCharge = 500;

        public Vector2 lastDisplacement;
        public float dashDuration;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Andromeda's Stride");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 70;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 16;
        }

        public override bool CanDamage()
        {
            return (State == 1);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 145 * projectile.scale;
            float bladeWidth = 25 * projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Owner.Center + (direction * bladeLenght), bladeWidth, ref collisionPoint);
        }

        public CurveSegment QuickOut = new CurveSegment(EasingType.PolyIn, 0f, 0f, 0.2f, 3);
        public CurveSegment Bump = new CurveSegment(EasingType.SineBump, 0.06f, 0.2f, 0.1f);
        public CurveSegment QuickDraw = new CurveSegment(EasingType.Linear, 0.25f, 0.2f, -0.45f);
        public CurveSegment SlowDrawOut = new CurveSegment(EasingType.PolyIn, 0.50f, -0.25f, -0.2f, 3);
        public CurveSegment OverShoot = new CurveSegment(EasingType.SineBump, 0.93f, -0.45f, -0.1f);

        internal float ChargeDisplacement() => PiecewiseAnimation(Charge / MaxCharge, new CurveSegment[] { QuickOut, Bump, QuickDraw, SlowDrawOut, OverShoot });

        public override void AI()
        {
            if (!initialized) //Initialization. Here its litterally just playing a sound tho lmfao
            {
                projectile.velocity = Vector2.Zero;
                Main.PlaySound(SoundID.Item101, projectile.Center);
                initialized = true;
            }

            if (!OwnerCanShoot)
            {
                if (State == 0f)
                {
                    if (Charge / MaxCharge < 0.25f)
                    {
                        Main.PlaySound(SoundID.Item109, projectile.Center);
                        projectile.Kill();
                        return;
                    }
                    else
                    {

                        var dashSound = Main.PlaySound(SoundID.Item120, projectile.Center);
                        if (dashSound != null)
                            dashSound.Volume *= 0.5f;
                        State = 1f;
                        projectile.timeLeft = (7 + (int)((Charge / MaxCharge - 0.25f) * 20)) * 2; //Keep that even, if its an odd number itll fuck off and wont reset the players velocity on death
                        dashDuration = projectile.timeLeft;
                        lastDisplacement = projectile.Center - Owner.Center;
                        projectile.netUpdate = true;
                        projectile.netSpam = 0;
                    }
                }
            }

            if (State == 0f)
            {
                direction = Owner.DirectionTo(Main.MouseWorld);
                direction.Normalize();
                projectile.Center = Owner.Center + (direction * 70f * ChargeDisplacement());

                Charge++;
                OverCharge--;
                projectile.timeLeft = 2;
                if ((Charge / MaxCharge >= 0.25f && CurrentIndicator == 0f) || (Charge / MaxCharge >= 0.5f && CurrentIndicator == 1f) || (Charge / MaxCharge >= 0.75f && CurrentIndicator == 2f))
                {
                    //WOahhh do a ring of projectiles!! woahhh
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile blast = Projectile.NewProjectileDirect(Owner.Center, (MathHelper.TwoPi * i / 5f).ToRotationVector2() * 10f, ProjectileType<GalaxiaBolt>(), (int)(projectile.damage * FourSeasonsGalaxia.ShockwaveAttunement_BoltsDamageReduction), 0f, Owner.whoAmI, 0.75f, MathHelper.Pi * 0.02f);
                        {
                            blast.timeLeft = 30;
                        }
                    }

                    Main.PlaySound(SoundID.Item79, projectile.Center);
                    CurrentIndicator++;
                    OverCharge = 20f;
                }

                if (Charge >= MaxCharge)
                {
                    Charge = MaxCharge;

                    if (Main.rand.NextBool())
                    {
                        Vector2 smokeSpeed = direction.RotatedByRandom(MathHelper.PiOver4 * 0.3f) * Main.rand.NextFloat(10f, 30f) * 0.9f;
                        Particle smoke = new HeavySmokeParticle(projectile.Center + direction * 50f, smokeSpeed + Owner.velocity, Color.Lerp(Color.Purple, Color.Indigo, (float)Math.Sin(Main.GlobalTime * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f), 0.8f, 0, false, 0, true);
                        GeneralParticleHandler.SpawnParticle(smoke);

                        if (Main.rand.Next(3) == 0)
                        {
                            Particle smokeGlow = new HeavySmokeParticle(projectile.Center + direction * 50f, smokeSpeed + Owner.velocity, Main.hslToRgb(0.85f, 1, 0.8f), 20, Main.rand.NextFloat(0.4f, 0.7f), 0.8f, 0, true, 0.01f, true);
                            GeneralParticleHandler.SpawnParticle(smokeGlow);
                        }
                    }


                    if (Owner.whoAmI == Main.myPlayer && CurrentIndicator < 4f)
                    {
                        //Projectiles!!! wah!!!
                        for (int i = 0; i < 9; i++)
                        {
                            Projectile blast = Projectile.NewProjectileDirect(Owner.Center, (MathHelper.TwoPi * i / 9f).ToRotationVector2() * 10f, ProjectileType<GalaxiaBolt>(), (int)(projectile.damage * FourSeasonsGalaxia.ShockwaveAttunement_BoltsDamageReduction), 0f, Owner.whoAmI, 0.75f, MathHelper.Pi * 0.02f);
                            {
                                blast.timeLeft = 50;
                            }
                        }


                        OverCharge = 20f;
                        Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/CorvinaScream"), projectile.Center);
                        CurrentIndicator++;
                    }
                }
            }

            if (State == 1f)
            {
                projectile.Center = Owner.Center + Vector2.Lerp(lastDisplacement, direction * 40f, MathHelper.Clamp(((dashDuration - projectile.timeLeft) / dashDuration) * 2f, 0f, 1f));
                Owner.fallStart = (int)(Owner.position.Y / 16f);

                Owner.Calamity().LungingDown = true;

                if (Collision.SolidCollision(Owner.Center + (direction * 120 * projectile.scale) - Vector2.One * 5f, 10, 10))
                {
                    SlamDown();
                    projectile.timeLeft = 0;
                    Owner.Calamity().LungingDown = false;
                    projectile.active = false;
                    projectile.netUpdate = true;
                    projectile.netSpam = 0;
                }

                Owner.velocity = direction * 30f;

                float variation = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
                float strength = (float)Math.Sin(variation * 2f + MathHelper.PiOver2);
                Particle Sparkle = new CritSpark(projectile.Center, Owner.velocity - direction.RotatedBy(variation) * (1 + strength) * 2f * Main.rand.NextFloat(7.5f, 20f), Color.White, Main.rand.NextBool() ? Color.MediumTurquoise : Color.DarkOrange, 0.1f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                GeneralParticleHandler.SpawnParticle(Sparkle);
            }

            Lighting.AddLight(projectile.Center, new Vector3(1f, 0.56f, 0.56f) * Charge / MaxCharge);

            //Manage position and rotation
            projectile.rotation = direction.ToRotation();

            //Scaling based on charge
            projectile.scale = 1f + (Charge / MaxCharge * 0.3f);

            Owner.direction = Math.Sign(direction.X);
            Owner.itemRotation = direction.ToRotation();

            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }

            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        public void SlamDown()
        {
            var slamSound = Main.PlaySound(SoundID.DD2_MonkStaffGroundImpact, projectile.Center);
            if (slamSound != null)
                slamSound.Volume *= 1.5f;

            if (Owner.whoAmI != Main.myPlayer)
                return;

            if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 15)
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = 15;

            Projectile proj = Projectile.NewProjectileDirect(Owner.Center + (direction * 120 * projectile.scale), -direction * 16f, ProjectileType<GalaxiaBolt>(), projectile.damage, 10f, Owner.whoAmI, 0.75f, MathHelper.Pi / 25f);
            proj.scale = 3f;
            proj.timeLeft = 50;


            SideSprouts(1, 150f, 1f * Charge / MaxCharge);
            SideSprouts(-1, 150f, 1f * Charge / MaxCharge);

        }

        public bool SideSprouts(float facing, float distance, float projSize)
        {
            float widestAngle = 0f;
            float widestSurfaceAngle = 0f;
            bool validPositionFound = false;
            for (float i = 0f; i < 1; i += 1 / distance)
            {
                Vector2 positionToCheck = Owner.Center + (direction * 120 * projectile.scale) + direction.RotatedBy((i * MathHelper.PiOver2 + MathHelper.PiOver4) * facing) * distance;

                if (Main.tile[(int)(positionToCheck.X / 16), (int)(positionToCheck.Y / 16)].IsTileSolidGround())
                    widestAngle = i;

                else if (widestAngle != 0)
                {
                    validPositionFound = true;
                    widestSurfaceAngle = widestAngle;
                }
            }

            if (validPositionFound)
            {
                Vector2 projPosition = Owner.Center + (direction * 120 * projectile.scale) + direction.RotatedBy((widestSurfaceAngle * MathHelper.PiOver2 + MathHelper.PiOver4) * facing) * distance;
                Vector2 monolithRotation = direction.RotatedBy(Utils.AngleLerp(widestSurfaceAngle * -facing, 0f, projSize));
                Projectile proj = Projectile.NewProjectileDirect(projPosition, -monolithRotation, ProjectileType<AndromedasStrideBoltSpawner>(), (int)(projectile.damage * FourSeasonsGalaxia.ShockwaveAttunement_MonolithDamageBoost), 10f, Owner.whoAmI, Main.rand.Next(4), projSize);
                if (proj.modProjectile is AndromedasStrideBoltSpawner spawner)
                {
                    spawner.WaitTimer = (1 - projSize) * 34f;
                    spawner.OriginDirection = direction;
                    spawner.Facing = facing;
                }
            }

            return validPositionFound;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Owner.GiveIFrames(OmegaBiomeBlade.ShockwaveAttunement_DashHitIFrames);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * (1f + OmegaBiomeBlade.ShockwaveAttunement_FullChargeBoost * Charge / MaxCharge));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D sword = GetTexture("CalamityMod/Items/Weapons/Melee/GalaxiaExtra");

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, sword.Height);
            Vector2 drawOffset = projectile.Center - Main.screenPosition;

            CalamityUtils.EnterShaderRegion(spriteBatch);
            
            if (OverCharge < 0)
                OverCharge = 0f;
            //When the blink is
            GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(OverCharge / 20f);
            GameShaders.Misc["CalamityMod:BasicTint"].UseColor(new Color(255, 129, 153));
            GameShaders.Misc["CalamityMod:BasicTint"].Apply();

            spriteBatch.Draw(sword, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            CalamityUtils.ExitShaderRegion(spriteBatch);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            //Cut the velocity short if dashing
            if (State == 1f)
                Owner.velocity *= 0.33f;

            Owner.Calamity().LungingDown = false;

            projectile.active = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
            writer.WriteVector2(direction);
            writer.Write(CurrentIndicator);
            writer.WriteVector2(lastDisplacement);
            writer.Write(dashDuration);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            direction = reader.ReadVector2();
            CurrentIndicator = reader.ReadSingle();
            lastDisplacement = reader.ReadVector2();
            dashDuration = reader.ReadSingle();
        }
    }

    public class AndromedasStrideBoltSpawner : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[projectile.owner];
        public ref float Size => ref projectile.ai[1]; //Yes

        public float WaitTimer; //How long until the monoliths appears
        public Vector2 OriginDirection; //The direction of the original strike
        public float Facing; //The direction of the original strike

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Andromeda Shock");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 70;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 30;
            projectile.hide = true;
        }

        public override bool CanDamage() => false;

        public override void AI()
        {
            if (projectile.velocity != Vector2.Zero)
            {
                SurfaceUp();
                projectile.rotation = projectile.velocity.ToRotation();
                projectile.velocity = Vector2.Zero;
            }

            if (WaitTimer > 0)
            {
                projectile.timeLeft = 30;
                WaitTimer--;
            }

            if (projectile.timeLeft == 29)
            {
                if (Size * 0.8 > 0.4 && Facing != 0)
                  SideSprouts(Facing, 150f, Size * 0.8f);
            }

            if (projectile.timeLeft < 29)
            {
               if (Main.rand.Next(3) == 0)
                {
                    Vector2 particleDirection = (projectile.rotation - MathHelper.PiOver4).ToRotationVector2();
                    Vector2 flyDirection = particleDirection.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4 / 2f, MathHelper.PiOver4 / 2f)) * Main.rand.NextFloat(15f, 35f);

                    Particle smoke = new HeavySmokeParticle(projectile.Center, flyDirection, Color.Lerp(Color.MidnightBlue, Color.Indigo, (float)Math.Sin(Main.GlobalTime * 6f)), 30, Main.rand.NextFloat(0.4f, 1.3f) * projectile.scale, 0.8f, 0, false, 0, true);
                    GeneralParticleHandler.SpawnParticle(smoke);

                    if (Main.rand.Next(3) == 0)
                    {
                        Particle smokeGlow = new HeavySmokeParticle(projectile.Center, flyDirection, Color.Red, 20, Main.rand.NextFloat(0.1f, 0.7f) * projectile.scale, 0.8f, 0, true, 0.01f, true);
                        GeneralParticleHandler.SpawnParticle(smokeGlow);
                    }

                }
            }

            if (projectile.timeLeft == 2)
            {
                //New bolt
                Projectile proj = Projectile.NewProjectileDirect(projectile.position, projectile.rotation.ToRotationVector2() * 18f, ProjectileType<GalaxiaBolt>(), projectile.damage, 10f, Owner.whoAmI, 0.75f, MathHelper.Pi / 25f);
                proj.scale = Size * 3f;
                proj.timeLeft = 50;

                Vector2 particleDirection = (projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
                Main.PlaySound(SoundID.DD2_EtherianPortalDryadTouch, projectile.Center);

                for (int i = 0; i < 8; i++)
                {
                    Vector2 hitPositionDisplace = particleDirection.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0f, 10f);
                    Vector2 flyDirection = particleDirection.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * Main.rand.NextFloat(5f, 15f);

                    Particle smoke = new HeavySmokeParticle(projectile.Center + hitPositionDisplace, flyDirection, Color.Lerp(Color.MidnightBlue, Color.Indigo, (float)Math.Sin(Main.GlobalTime * 6f)), 30, Main.rand.NextFloat(1.6f, 2.2f) * projectile.scale, 0.8f, 0, false, 0, true);
                    GeneralParticleHandler.SpawnParticle(smoke);

                    if (Main.rand.Next(3) == 0)
                    {
                        Particle smokeGlow = new HeavySmokeParticle(projectile.Center + hitPositionDisplace, flyDirection, Color.Red, 20, Main.rand.NextFloat(1.4f, 1.7f) * projectile.scale, 0.8f, 0, true, 0.005f, true);
                        GeneralParticleHandler.SpawnParticle(smokeGlow);
                    }
                }
            }
        }

        //Go up to the "surface" so you're not stuck in the middle of the ground like a complete moron.
        public void SurfaceUp()
        {
            for (float i = 0; i < 40; i += 0.5f)
            {
                Vector2 positionToCheck = projectile.Center + projectile.velocity * i;
                if (!Main.tile[(int)(positionToCheck.X / 16), (int)(positionToCheck.Y / 16)].IsTileSolidGround())
                {
                    projectile.Center = projectile.Center + projectile.velocity * i;
                    return;
                }
            }
            projectile.Center = projectile.Center + projectile.velocity * 40f;
        }

        public bool SideSprouts(float facing, float distance, float projSize)
        {
            float widestAngle = 0f;
            float widestSurfaceAngle = 0f;
            bool validPositionFound = false;
            for (float i = 0f; i < 1; i += 1 / distance)
            {
                Vector2 positionToCheck = projectile.Center + OriginDirection.RotatedBy((i * MathHelper.PiOver2 + MathHelper.PiOver4) * facing) * distance;

                if (Main.tile[(int)(positionToCheck.X / 16), (int)(positionToCheck.Y / 16)].IsTileSolidGround())
                    widestAngle = i;

                else if (widestAngle != 0)
                {
                    validPositionFound = true;
                    widestSurfaceAngle = widestAngle;
                }
            }

            if (validPositionFound)
            {
                Vector2 projPosition = projectile.Center + OriginDirection.RotatedBy((widestSurfaceAngle * MathHelper.PiOver2 + MathHelper.PiOver4) * facing) * distance;
                Vector2 monolithRotation = OriginDirection.RotatedBy(Utils.AngleLerp(widestSurfaceAngle * -facing, 0f, projSize));
                Projectile proj = Projectile.NewProjectileDirect(projPosition, -monolithRotation, ProjectileType<AndromedasStrideBoltSpawner>(), projectile.damage, 10f, Owner.whoAmI, Main.rand.Next(4), projSize);
                if (proj.modProjectile is AndromedasStrideBoltSpawner spawner)
                {
                    spawner.WaitTimer = (float)Math.Sqrt(1.0 - Math.Pow(projSize - 1.0, 2)) * 3f;
                    spawner.OriginDirection = OriginDirection;
                    spawner.Facing = facing;
                }
            }

            return validPositionFound;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(WaitTimer);
            writer.Write(Facing);
            writer.WriteVector2(OriginDirection);

        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            WaitTimer = reader.ReadSingle();
            Facing = reader.ReadSingle();
            OriginDirection = reader.ReadVector2();
        }
    }

    //Aries's Wrath aka FlailBlade attunement

    public class AriessWrath : ModProjectile
    {
        private NPC[] excludedTargets = new NPC[4];
        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalaxiaExtra2";
        public Player Owner => Main.player[projectile.owner];
        public ref float ChainSwapTimer => ref projectile.ai[0];
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;

        const float MaxProjReach = 500f; //How far away do you check for enemies for the extra projs from crits be

        public Particle smear;
        public Projectile lastConstellation;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aries's Wrath");

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;

        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 80;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = FourSeasonsGalaxia.FlailBladeAttunement_LocalIFrames;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - Vector2.One * 50 * projectile.scale, Vector2.One * 100 * projectile.scale);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 sparkSpeed = Owner.DirectionTo(target.Center).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * 9f;
                Particle Spark = new CritSpark(target.Center, sparkSpeed, Color.White, Color.HotPink, 1f + Main.rand.NextFloat(0, 1f), 30, 0.4f, 1f);
                GeneralParticleHandler.SpawnParticle(Spark);
            }

            Vector2 sliceDirection = Main.rand.NextVector2CircularEdge(50f, 100f);
            Particle SliceLine = new LineVFX(target.Center - sliceDirection, sliceDirection * 2f, 0.2f, Color.HotPink * 0.6f)
            {
                Lifetime = 6
            };
            GeneralParticleHandler.SpawnParticle(SliceLine);

             excludedTargets[0] = target;
             for (int i = 0; i < 3; i++)
             {
                  NPC potentialTarget = TargetNext(target.Center, i);
                  if (potentialTarget == null)
                       break;
                  Projectile proj = Projectile.NewProjectileDirect(target.Center, target.DirectionTo(potentialTarget.Center) * 25f, ProjectileType<GalaxiaBolt>(), (int)(damage * FourSeasonsGalaxia.FlailBladeAttunement_OnHitBoltDamageReduction), 0, Owner.whoAmI, 0.9f, MathHelper.PiOver4 * 0.4f);
                  proj.scale = 2f;
             }
             Array.Clear(excludedTargets, 0, 3);
            
        }

        public NPC TargetNext(Vector2 hitFrom, int index)
        {
            float longestReach = MaxProjReach;
            NPC target = null;
            for (int i = 0; i < 200; ++i)
            {
                NPC npc = Main.npc[i];
                if (!excludedTargets.Contains(npc) && npc.CanBeChasedBy() && !npc.friendly && !npc.townNPC)
                {
                    float distance = Vector2.Distance(hitFrom, npc.Center);
                    if (distance < longestReach)
                    {
                        longestReach = distance;
                        target = npc;
                    }
                }
            }
            if (index < 3)
                excludedTargets[index + 1] = target;
            return target;
        }

        //Animation keys
        public CurveSegment slowIn = new CurveSegment(EasingType.PolyIn, 0f, 0.2f, 1f, 3);
        public CurveSegment bounce = new CurveSegment(EasingType.SineBump, 0.3f, 1f, 0.2f);
        public CurveSegment remain = new CurveSegment(EasingType.SineBump, 0.6f, 1f, -0.1f);
        internal float ThrowDisplace() => PiecewiseAnimation(MathHelper.Clamp(ChainSwapTimer / 40f, 0, 1), new CurveSegment[] { slowIn, bounce, remain });

        //Animation keys
        public CurveSegment scaleUp = new CurveSegment(EasingType.PolyIn, 0f, 0.2f, 1f, 3);
        public CurveSegment scaleDown = new CurveSegment(EasingType.SineBump, 0.3f, 1f, 0.2f);
        internal float ScaleEquation() => PiecewiseAnimation(MathHelper.Clamp(ChainSwapTimer / 30f, 0, 1), new CurveSegment[] { scaleUp, scaleDown});


        public override void AI()
        {
            if (!OwnerCanShoot)
            {
                if ((Owner.Center - projectile.Center).Length() < 30f)
                    projectile.Kill();

                else
                {
                    if (projectile.timeLeft <= 2)
                    {
                        projectile.velocity *= 10f;
                    }
                    if (projectile.velocity.AngleBetween(Owner.Center - projectile.Center) > MathHelper.PiOver4)
                        projectile.velocity = (projectile.velocity.ToRotation().AngleTowards(projectile.DirectionTo(Owner.Center).ToRotation(), MathHelper.Pi / 20f)).ToRotationVector2() * projectile.velocity.Length() * 0.98f;
                    else
                        projectile.velocity = (projectile.velocity.ToRotation().AngleTowards(projectile.DirectionTo(Owner.Center).ToRotation(), MathHelper.Pi)).ToRotationVector2() * projectile.velocity.Length() * 1.05f;
                    projectile.rotation = Main.GlobalTime * 25f;
                    projectile.scale = MathHelper.Clamp((Owner.Center - projectile.Center).Length() / (FourSeasonsGalaxia.FlailBladeAttunement_Reach * 0.5f) , 0.3f, 2f);
                    projectile.timeLeft = 4;
                }
                return;
            }

            if (ChainSwapTimer == 0f)
            {
                projectile.Center = Owner.Center;
                var scream = Main.PlaySound(SoundID.Item120, projectile.Center);
                if (scream != null)
                    scream.Volume *= 0.5f;

                if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                    Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;
            }

            projectile.scale = 1f + ScaleEquation();
            projectile.timeLeft = 2;

            projectile.Center = Vector2.Lerp(projectile.Center, Main.MouseWorld, 0.05f * ThrowDisplace());
            projectile.Center = projectile.Center.MoveTowards(Main.MouseWorld, 40f * ThrowDisplace());

            if ((projectile.Center - Owner.Center).Length() > FourSeasonsGalaxia.FlailBladeAttunement_Reach)
                projectile.Center = Owner.Center + Owner.DirectionTo(projectile.Center) * FourSeasonsGalaxia.FlailBladeAttunement_Reach;

            projectile.rotation = Main.GlobalTime * 25f;
            //Make the owner look like theyre "holding" the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            projectile.velocity = Owner.DirectionTo(projectile.Center);
            Owner.direction = Math.Sign(projectile.velocity.X);
            Owner.itemRotation = projectile.velocity.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            if (smear == null)
            {
                smear = new CircularSmearSmokeyVFX(projectile.Center, Color.MediumOrchid, projectile.rotation, projectile.scale);
                GeneralParticleHandler.SpawnParticle(smear);
            }
            if (smear != null)
            {
                smear.Position = projectile.Center;
                smear.Rotation = projectile.rotation + MathHelper.PiOver2 + MathHelper.PiOver4;
                smear.Time = 0;
                smear.Scale = projectile.scale;
                smear.Color.A = (byte)(255 * MathHelper.Clamp(ChainSwapTimer / 50f, 0, 1));
            }

            if (Main.rand.NextBool())
            {
                float maxDistance = projectile.scale * 82f;
                Vector2 distance = Main.rand.NextVector2Circular(maxDistance, maxDistance);
                Vector2 angularVelocity = Vector2.Normalize(distance.RotatedBy(MathHelper.PiOver2)) * 2f * (1f + distance.Length() / 15f);
                Particle glitter = new CritSpark(projectile.Center + distance, angularVelocity, Main.rand.Next(3) == 0 ? Color.HotPink : Color.Plum, Color.DarkOrchid, 1f + 1 * (distance.Length() / maxDistance), 10, 0.05f, 3f);
                GeneralParticleHandler.SpawnParticle(glitter);
            }

            float smokeDistance = projectile.scale * 62f;
            Vector2 smokePos = Main.rand.NextVector2Circular(smokeDistance, smokeDistance);
            Vector2 smokeSpeed = Vector2.Normalize(smokePos.RotatedBy(MathHelper.PiOver2)) * 0.1f * (1f + smokePos.Length() / 15f);
            Particle smoke = new HeavySmokeParticle(projectile.Center + smokePos, smokeSpeed, Color.Lerp(Color.Navy, Color.Indigo, (float)Math.Sin(Main.GlobalTime * 6f)), 30, Main.rand.NextFloat(0.4f, 1f) * projectile.scale, 0.8f, 0, false, 0, true);
            GeneralParticleHandler.SpawnParticle(smoke);

            if (Main.rand.Next(3) == 0)
            {
               Particle smokeGlow = new HeavySmokeParticle(projectile.Center + smokePos, smokeSpeed, Main.hslToRgb(0.85f, 1, 0.5f), 20, Main.rand.NextFloat(0.4f, 1f) * projectile.scale, 0.8f, 0, true, 0.01f, true);
               GeneralParticleHandler.SpawnParticle(smokeGlow);
            }

            if (ChainSwapTimer % (20 - Math.Ceiling(14 * MathHelper.Clamp((projectile.position - projectile.oldPosition).Length() / 30f, 0, 1))) == 1)
            {
                if (lastConstellation != null && lastConstellation.active)
                    lastConstellation.Kill();

                lastConstellation = Projectile.NewProjectileDirect(Owner.Center, Vector2.Zero, ProjectileType<AriessWrathConstellation>(), (int)(projectile.damage * FourSeasonsGalaxia.FlailBladeAttunement_ChainDamageReduction), 0, Owner.whoAmI);
                if (lastConstellation.modProjectile is AriessWrathConstellation constellation)
                {
                    constellation.SizeVector = projectile.Center - Owner.Center;
                }
            }

            ChainSwapTimer++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D sword = GetTexture("CalamityMod/Items/Weapons/Melee/GalaxiaExtra2");

            Vector2 drawPos = projectile.Center;
            Vector2 drawOrigin = sword.Size() / 2f;
            float drawRotation = projectile.rotation + MathHelper.PiOver4;

            spriteBatch.Draw(sword, drawPos - Main.screenPosition, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            return false;
        }
    }

    public class AriessWrathConstellation : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[projectile.owner];
        public float Timer => 20 - projectile.timeLeft;

        public Vector2 SizeVector;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Constellation");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 8;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
            projectile.timeLeft = 20;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + SizeVector, 30f, ref collisionPoint);
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                projectile.ai[0] = 1;
                Vector2 previousStar = projectile.Center;
                Vector2 offset;
                Particle Line;
                Particle Star = new GenericSparkle(previousStar, Vector2.Zero, Color.White, Color.Plum, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                GeneralParticleHandler.SpawnParticle(Star);


                for (float i = 0 + Main.rand.NextFloat(0.2f, 0.5f); i < 1; i += Main.rand.NextFloat(0.2f, 0.5f))
                {
                    offset = Main.rand.NextFloat(-50f, 50f) * Vector2.Normalize(SizeVector.RotatedBy(MathHelper.PiOver2));
                    Star = new GenericSparkle(projectile.Center + SizeVector * i + offset, Vector2.Zero, Color.White, Color.Plum, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                    GeneralParticleHandler.SpawnParticle(Star);

                    Line = new BloomLineVFX(previousStar, projectile.Center + SizeVector * i + offset - previousStar, 0.8f, Color.MediumVioletRed * 0.75f, 20, true, true);
                    GeneralParticleHandler.SpawnParticle(Line);

                    if (Main.rand.Next(3) == 0)
                    {
                        offset = Main.rand.NextFloat(-50f, 50f) * Vector2.Normalize(SizeVector.RotatedBy(MathHelper.PiOver2));
                        Star = new GenericSparkle(projectile.Center + SizeVector * i + offset, Vector2.Zero, Color.White, Color.Plum, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                        GeneralParticleHandler.SpawnParticle(Star);

                        Line = new BloomLineVFX(previousStar, projectile.Center + SizeVector * i + offset - previousStar, 0.8f, Color.MediumVioletRed * 0.75f, 20, true, true);
                        GeneralParticleHandler.SpawnParticle(Line);
                    }

                    previousStar = projectile.Center + SizeVector * i + offset;
                }

                Star = new GenericSparkle(projectile.Center + SizeVector, Vector2.Zero, Color.White, Color.Plum, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                GeneralParticleHandler.SpawnParticle(Star);

                Line = new BloomLineVFX(previousStar, projectile.Center + SizeVector - previousStar, 0.8f, Color.MediumVioletRed * 0.75f, 20, true);
                GeneralParticleHandler.SpawnParticle(Line);
            }
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(SizeVector);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SizeVector = reader.ReadVector2();
        }

    }
}


