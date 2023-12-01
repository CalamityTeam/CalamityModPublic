using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Terraria.Graphics.Shaders;
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

namespace CalamityMod.Projectiles.Melee
{
    public class PhoenixsPride : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalaxiaExtra2"; //The blue one, since this is long range
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float CurrentState => ref Projectile.ai[0];
        public Player Owner => Main.player[Projectile.owner];
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;
        public const float throwOutTime = 90f;
        public const float throwOutDistance = 440f;

        public static float snapPoint = 0.45f;
        public float snapTimer => (throwTimer / throwOutTime) < snapPoint ? 0 : ((throwTimer / throwOutTime) - snapPoint) / (1f - snapPoint);
        public static float retractionPoint = 0.6f;
        public float retractionTimer => (throwTimer / throwOutTime) < retractionPoint ? 0 : ((throwTimer / throwOutTime) - retractionPoint) / (1f - retractionPoint);
        public ref float Empowerment => ref Projectile.ai[1];
        public float OverEmpowerment = 0f; //Used to keep cooldowns working when the spin is full
        public ref float hasMadeSound => ref Projectile.localAI[0];
        public ref float hasMadeChargeSound => ref Projectile.localAI[1];

        public const float maxEmpowerment = 600f;
        public float throwTimer => throwOutTime - Projectile.timeLeft;


        public float AngleReset = 0f;
        public bool CanDirectFire = true;
        public Particle smear;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 74;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = FourSeasonsGalaxia.PhoenixAttunement_LocalIFrames;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLength = 145 * Projectile.scale;
            float bladeWidth = 25 * Projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (direction * bladeLength), bladeWidth, ref collisionPoint);
        }


        public CurveSegment launch = new CurveSegment(EasingType.CircOut, 0f, 0f, 1f, 4);
        public CurveSegment hold = new CurveSegment(EasingType.Linear, snapPoint, 1f, 0f);
        public CurveSegment retract = new CurveSegment(EasingType.PolyInOut, retractionPoint, 1f, -1.05f, 3);
        internal float ThrowCurve() => PiecewiseAnimation((throwOutTime - Projectile.timeLeft) / throwOutTime, new CurveSegment[] { launch, hold, retract });

        public override void AI()
        {
            if (!initialized) //Initialization. Here its litterally just playing a sound tho lmfao
            {
                SoundEngine.PlaySound(SoundID.Item90, Projectile.Center);
                Projectile.velocity = Vector2.Zero;
                direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                direction.Normalize();
                initialized = true;
            }

            Projectile.rotation = direction.ToRotation(); //Only done for afterimages

            if (!OwnerCanShoot)
            {
                if (CurrentState == 2f || (CurrentState == 0f && Empowerment / maxEmpowerment < 0.5))
                {
                    SoundEngine.PlaySound(SoundID.Item77, Projectile.Center);
                    Projectile.Kill();
                    return;
                }

                else if (CurrentState == 0f)
                {
                    CurrentState = 1f;
                    if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;

                    SoundEngine.PlaySound(SoundID.Item80, Projectile.Center);
                    direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                    //PARTICLES LOTS OF PARTICLES LOTS OF SPARKLES YES YES MH YES YES
                    for (int i = 0; i <= 8; i++)
                    {
                        float variation = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
                        float strength = (float)Math.Sin(variation * 2f + MathHelper.PiOver2);
                        Particle Sparkle = new CritSpark(Projectile.Center, Owner.velocity + direction.RotatedBy(variation) * (1 + strength) * 2f * Main.rand.NextFloat(7.5f, 20f), Color.Coral, Color.Chocolate, 2f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 2f);
                        GeneralParticleHandler.SpawnParticle(Sparkle);
                    }

                    //Actually do projectiles now
                    if (Owner.whoAmI == Main.myPlayer)
                    {
                        for (int i = 0; i <= 5; i++)
                        {
                            float angle = direction.ToRotation() + MathHelper.Lerp(-MathHelper.PiOver4, MathHelper.PiOver4, i / 5f);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, angle.ToRotationVector2() * 30f, ProjectileType<GalaxiaBolt>(), (int)(Projectile.damage * FourSeasonsGalaxia.PhoenixAttunement_BoltThrowDamageMultiplier), 0f, Owner.whoAmI, 0.1f, MathHelper.Pi * 0.02f);
                        }
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
                        SoundEngine.PlaySound(SoundID.Item76);
                    }

                    float rotation = direction.ToRotation();
                    if (rotation > -MathHelper.PiOver2 - MathHelper.PiOver4 && rotation < -MathHelper.PiOver2 + MathHelper.PiOver4 && hasMadeSound == 1f)
                        hasMadeSound = 0f;

                    else if (rotation > MathHelper.PiOver2 - MathHelper.PiOver4 && rotation < MathHelper.PiOver2 + MathHelper.PiOver4 && hasMadeSound == 0f)
                    {
                        CanDirectFire = true;
                        hasMadeSound = 1f;
                        SoundEngine.PlaySound(SoundID.Item71);
                    }
                }


                if (Empowerment / maxEmpowerment >= 0.5)
                {
                    Empowerment++; //Charge faster when near the end

                    if (Main.rand.Next(2) == 0)

                    {

                        float Opacity = MathHelper.Clamp((Empowerment / maxEmpowerment - 0.5f) * 3f, 0, 1) * 0.75f;
                        float scaleFactor = MathHelper.Clamp((Empowerment / maxEmpowerment - 0.5f) * 3f, 0, 1);

                        for (float i = 0f; i <= 1; i += 0.5f)
                        {
                            Vector2 smokepos = Projectile.Center + (direction * (60 + 40 * i) * Projectile.scale) + direction.RotatedBy(-MathHelper.PiOver2) * 30f * scaleFactor * Main.rand.NextFloat();


                            Particle smoke = new HeavySmokeParticle(smokepos, direction.RotatedBy(-MathHelper.PiOver2) * 20f * scaleFactor + Owner.velocity, Color.Lerp(Color.MidnightBlue, Color.Indigo, i), 10 + Main.rand.Next(5), scaleFactor * Main.rand.NextFloat(2.8f, 3.1f), Opacity + Main.rand.NextFloat(0f, 0.2f), 0f, false, 0, true);
                            GeneralParticleHandler.SpawnParticle(smoke);

                            if (Main.rand.Next(3) == 0)
                            {
                                Particle smokeGlow = new HeavySmokeParticle(smokepos, direction.RotatedBy(-MathHelper.PiOver2) * 20f * scaleFactor + Owner.velocity, Color.OrangeRed, 7, scaleFactor * Main.rand.NextFloat(2f, 2.4f), Opacity * 2, 0f, true, 0.001f, true);
                                GeneralParticleHandler.SpawnParticle(smokeGlow);
                            }
                        }
                    }

                    if ((Empowerment + OverEmpowerment) % 30 == 29 && Owner.whoAmI == Main.myPlayer)
                    {
                        Vector2 shotDirection = Main.rand.NextVector2CircularEdge(15f, 15f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, shotDirection, ProjectileType<GalaxiaBolt>(), (int)(Projectile.damage * FourSeasonsGalaxia.PhoenixAttunement_BoltDamageReduction), 0f, Owner.whoAmI, 0.1f, MathHelper.Pi * 0.02f);
                    }
                }


                if (Empowerment / maxEmpowerment >= 0.75)
                {
                    Color currentColor = Color.Chocolate * (((Empowerment / maxEmpowerment) - 0.75f) / 0.25f * 0.8f);
                    if (smear == null)
                    {
                        smear = new CircularSmearSmokeyVFX(Owner.Center, currentColor, direction.ToRotation(), Projectile.scale * 1.5f);
                        GeneralParticleHandler.SpawnParticle(smear);
                    }
                    //Update the variables of the smear
                    else
                    {
                        smear.Rotation = direction.ToRotation() + MathHelper.PiOver2;
                        smear.Time = 0;
                        smear.Position = Owner.Center;
                        smear.Scale = Projectile.scale * 1.9f;
                        smear.Color = currentColor;
                    }

                    float rotationAdjusted = MathHelper.WrapAngle(Projectile.rotation) + MathHelper.Pi;
                    float mouseAngleAdjusted = MathHelper.WrapAngle(Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.One).ToRotation()) + MathHelper.Pi;
                    float deltaAngleShoot = Math.Abs(MathHelper.WrapAngle(rotationAdjusted - mouseAngleAdjusted));

                    if (CanDirectFire && deltaAngleShoot < 0.1f)
                    {
                        if (Owner.whoAmI == Main.myPlayer)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.One) * 15f, ProjectileType<GalaxiaBolt>(), (int)(Projectile.damage * FourSeasonsGalaxia.PhoenixAttunement_BoltDamageReduction), 0f, Owner.whoAmI, 0.1f, MathHelper.Pi * 0.02f);
                        }
                        CanDirectFire = false;
                        AngleReset = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.One).ToRotation();
                    }


                    if (Main.rand.NextBool())
                    {
                        float maxDistance = Projectile.scale * 1.9f * 78f;
                        Vector2 distance = Main.rand.NextVector2Circular(maxDistance, maxDistance);
                        Vector2 angularVelocity = Utils.SafeNormalize(distance.RotatedBy(MathHelper.PiOver2), Vector2.Zero) * 2f * (1f + distance.Length() / 15f);
                        Particle glitter = new CritSpark(Owner.Center + distance, Owner.velocity + angularVelocity, Main.rand.Next(3) == 0 ? Color.Turquoise : Color.Coral, currentColor, 1f + 1 * (distance.Length() / maxDistance), 10, 0.05f, 3f);
                        GeneralParticleHandler.SpawnParticle(glitter);
                    }
                }


                //Manage position and rotation
                Projectile.scale = 1 + Empowerment / maxEmpowerment * 1.5f;

                direction = direction.RotatedBy(MathHelper.Clamp(Empowerment / maxEmpowerment, 0.4f, 1f) * MathHelper.PiOver4 * 0.20f);
                direction.Normalize();
                Projectile.rotation = direction.ToRotation();
                Projectile.Center = Owner.Center + (direction * Projectile.scale * 10);
                Projectile.timeLeft = (int)throwOutTime + 1;
                Empowerment++;
                if (Empowerment > maxEmpowerment)
                {
                    Empowerment = maxEmpowerment;
                    OverEmpowerment++;
                }
            }

            if (CurrentState == 1f)
            {
                Projectile.Center = Owner.Center + (direction * Projectile.scale * 10) + (direction * throwOutDistance * ThrowCurve());
                Projectile.scale = (1 + Empowerment / maxEmpowerment * 1.5f) * MathHelper.Clamp(1 - retractionTimer, 0.3f, 1f);
            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Math.Sign(direction.X));
            Owner.itemRotation = direction.ToRotation();
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (CurrentState == 1f)
                modifiers.SourceDamage *= MathHelper.Lerp(1f, FourSeasonsGalaxia.PhoenixAttunement_ThrowDamageBoost, Empowerment / maxEmpowerment);
            else
                modifiers.SourceDamage *= FourSeasonsGalaxia.PhoenixAttunement_BaseDamageReduction + (FourSeasonsGalaxia.PhoenixAttunement_FullChargeDamageBoost * Empowerment / maxEmpowerment);
        }

        public override void OnKill(int timeLeft)
        {
            if (smear != null)
                smear.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sword = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/GalaxiaExtra2").Value;

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, sword.Height);
            Vector2 drawOffset = Projectile.Center - Main.screenPosition;

            //Afterimages
            if (CalamityConfig.Instance.Afterimages && CurrentState == 0f && Empowerment / maxEmpowerment > 0.4f)
            {
                for (int i = 0; i < Projectile.oldRot.Length; ++i)
                {
                    Color color = Projectile.GetAlpha(lightColor) * (1f - (i / (float)Projectile.oldRot.Length));
                    float afterimageRotation = Projectile.oldRot[i] + MathHelper.PiOver4;
                    Main.spriteBatch.Draw(sword, drawOffset, null, color * MathHelper.Lerp(0f, 0.5f, MathHelper.Clamp((Empowerment - maxEmpowerment * 0.4f) / (maxEmpowerment * 0.6f), 0f, 1f)), afterimageRotation, drawOrigin, Projectile.scale - 0.2f * ((i / (float)Projectile.oldRot.Length)), 0f, 0f);
                }
            }

            Main.EntitySpriteDraw(sword, drawOffset, null, lightColor, drawRotation, drawOrigin, Projectile.scale, 0f, 0);

            //Turn on additive blending
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
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
                drawChain(snapTimer, retractionTimer);
            }

            //Back to normal
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public void drawChain(float snapProgress, float retractProgress)
        {
            Texture2D chainTex = Request<Texture2D>("CalamityMod/Projectiles/Melee/MendedBiomeBlade_HeavensMightChain").Value;

            float opacity = retractProgress < 0.5 ? 1 : (retractProgress - 0.5f) / 0.5f;

            Vector2 Shake = retractProgress > 0 ? Vector2.Zero : Vector2.One.RotatedByRandom(MathHelper.TwoPi) * (1f - snapProgress) * 10f;

            int dist = (int)Vector2.Distance(Owner.Center, Projectile.Center) / 16;
            Vector2[] Nodes = new Vector2[dist + 1];
            Nodes[0] = Owner.Center;
            Nodes[dist] = Projectile.Center;

            for (int i = 1; i < dist + 1; i++)
            {
                Rectangle frame = new Rectangle(0, 0 + 18 * (i % 2), 12, 18);
                Vector2 positionAlongLine = Vector2.Lerp(Owner.Center, Projectile.Center, i / (float)dist); //Get the position of the segment along the line, as if it were a flat line
                Nodes[i] = positionAlongLine + Shake * (float)Math.Sin(i / (float)dist * MathHelper.Pi);

                float rotation = (Nodes[i] - Nodes[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(Nodes[i], Nodes[i - 1]) / frame.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                Vector2 scale = new Vector2(1, yScale);

                Color chainLightColor = Lighting.GetColor((int)Nodes[i].X / 16, (int)Nodes[i].Y / 16); //Lighting of the position of the chain segment

                Vector2 origin = new Vector2(frame.Width / 2, frame.Height); //Draw from center bottom of texture
                Main.EntitySpriteDraw(chainTex, Nodes[i] - Main.screenPosition, frame, chainLightColor * opacity * 0.7f, rotation, origin, scale, SpriteEffects.None, 0);
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
}
