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

    //Swordsmith's pride aka Whirlwind attunement

    public class SwordsmithsPride : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_SwordsmithsPride";
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
        public Particle sightLine;
        public NPC lastTarget;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Swordsmith's Pride");
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
            projectile.localNPCHitCooldown = 16;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 140 * projectile.scale;
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
                    Main.PlaySound(SoundID.Item80, projectile.Center);
                    direction = Owner.DirectionTo(Main.MouseWorld);
                    //PARTICLES LOTS OF PARTICLES LOTS OF SPARKLES YES YES MH YES YES 
                    for (int i = 0; i <= 8; i++)
                    {
                        float variation = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
                        float strength = (float)Math.Sin(variation * 2f + MathHelper.PiOver2);
                        Particle Sparkle = new CritSpark(projectile.Center, Owner.velocity + direction.RotatedBy(variation) * (1 + strength) * 2f * Main.rand.NextFloat(7.5f, 20f), Color.White, Color.HotPink, 2f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 2f);
                        GeneralParticleHandler.SpawnParticle(Sparkle);
                    }
                    for (int i = 0; i <= 8; i++)
                    {
                        float variation = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
                        float strength = (float)Math.Sin(variation * 2f + MathHelper.PiOver2);
                        Particle Sparkle = new CritSpark(projectile.Center, Owner.velocity + direction.RotatedBy(variation) * (1 + strength) * Main.rand.NextFloat(7.5f, 20f), Color.White, Color.GreenYellow, 2f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 2f);
                        GeneralParticleHandler.SpawnParticle(Sparkle);
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

                if (Empowerment / maxEmpowerment >= 0.5)
                {

                    if ((Empowerment + OverEmpowerment) % 30 == 29)
                    {
                        Vector2 shotDirection = Main.rand.NextVector2CircularEdge(15f, 15f);
                        if (lastTarget != null && lastTarget.active) //If you've got an actual target, angle your shots towards them
                        {
                            shotDirection = (shotDirection.ToRotation().AngleTowards(Owner.AngleTo(lastTarget.Center), MathHelper.PiOver2)).ToRotationVector2() * 15f;
                        }
                        Projectile.NewProjectile(Owner.Center, shotDirection, ProjectileType<SwordsmithsPrideBeam>(), (int)(projectile.damage * OmegaBiomeBlade.WhirlwindAttunement_BeamDamageReduction), 0f, Owner.whoAmI);
                    }
                }


                if (Empowerment / maxEmpowerment >= 0.75)
                {
                    Color currentColor = Color.Lerp(Color.HotPink, Color.GreenYellow, (float)Math.Sin(Main.GlobalTime * 2f)) * (((Empowerment / maxEmpowerment) - 0.75f) / 0.25f * 0.8f);
                    if (smear == null)
                    {
                        smear = new CircularSmearVFX(Owner.Center, Color.HotPink, direction.ToRotation(), projectile.scale * 1.5f);
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

                    if (sightLine == null)
                    {
                        sightLine = new LineVFX(Owner.Center, Owner.DirectionTo(Main.MouseWorld), 0.2f, Color.HotPink, false);
                        GeneralParticleHandler.SpawnParticle(sightLine);
                    }
                    else
                    {
                        sightLine.Position = Owner.Center + Owner.DirectionTo(Main.MouseWorld) * projectile.scale * 1.88f * 40;
                        (sightLine as LineVFX).LineVector = Owner.DirectionTo(Main.MouseWorld) * projectile.scale * 1.88f * 38f;
                        sightLine.Scale = 0.2f;
                        sightLine.Time = 0;
                        sightLine.Color = currentColor * 0.7f;
                    }

                    float rotationAdjusted = MathHelper.WrapAngle(projectile.rotation) + MathHelper.Pi;
                    float mouseAngleAdjusted = MathHelper.WrapAngle(Owner.DirectionTo(Main.MouseWorld).ToRotation()) + MathHelper.Pi;
                    float deltaAngleShoot = Math.Abs(MathHelper.WrapAngle(rotationAdjusted - mouseAngleAdjusted));

                    if (CanDirectFire && deltaAngleShoot < 0.1f)
                    {
                        Particle Blink = new GenericSparkle(Owner.Center + Owner.DirectionTo(Main.MouseWorld) * projectile.scale * 1.88f * 78f, Owner.velocity, Color.White, currentColor, 1.5f, 10, 0.1f, 3f);
                        GeneralParticleHandler.SpawnParticle(Blink);

                        Projectile.NewProjectile(Owner.Center, Owner.DirectionTo(Main.MouseWorld) * 15f, ProjectileType<SwordsmithsPrideBeam>(), (int)(projectile.damage * OmegaBiomeBlade.WhirlwindAttunement_BeamDamageReduction), 0f, Owner.whoAmI);
                        CanDirectFire = false;
                        AngleReset = Owner.DirectionTo(Main.MouseWorld).ToRotation();
                    }


                    if (Main.rand.NextBool())
                    {
                        float maxDistance = projectile.scale * 1.9f * 78f;
                        Vector2 distance = Main.rand.NextVector2Circular(maxDistance, maxDistance);
                        Vector2 angularVelocity = Vector2.Normalize(distance.RotatedBy(MathHelper.PiOver2)) * 2f * (1f + distance.Length() / 15f);
                        Particle glitter = new CritSpark(Owner.Center + distance, Owner.velocity + angularVelocity, Color.White, currentColor, 1f + 1 * (distance.Length() / maxDistance), 10, 0.05f, 3f);
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

            if (CurrentState != 1)
                return;

            lastTarget = target;

            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ProjectileType<PurityProjectionSigil>() && proj.owner == Owner.whoAmI)
                {
                    //Reset the timeleft on the sigil & give it its new target (or the same, it doesnt matter really.
                    proj.ai[0] = target.whoAmI;
                    proj.timeLeft = OmegaBiomeBlade.WhirlwindAttunement_SigilTime;
                    return;
                }
            }
            Projectile sigil = Projectile.NewProjectileDirect(target.Center, Vector2.Zero, ProjectileType<PurityProjectionSigil>(), 0, 0, Owner.whoAmI, target.whoAmI);
            sigil.timeLeft = OmegaBiomeBlade.WhirlwindAttunement_SigilTime;
        }

        public override void Kill(int timeLeft)
        {
            if (smear != null)
                smear.Kill();
            if (sightLine != null)
                sightLine.Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D handle = GetTexture("CalamityMod/Items/Weapons/Melee/OmegaBiomeBlade");
            Texture2D blade = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_SwordsmithsPride");

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            Vector2 drawOffset = projectile.Center - Main.screenPosition;

            //Afterimages
            if (CalamityConfig.Instance.Afterimages && CurrentState == 0f && Empowerment / maxEmpowerment > 0.4f)
            {
                for (int i = 0; i < projectile.oldRot.Length; ++i)
                {
                    Color color = projectile.GetAlpha(lightColor) * (1f - (i / (float)projectile.oldRot.Length));
                    float afterimageRotation = projectile.oldRot[i] + MathHelper.PiOver4;
                    Main.spriteBatch.Draw(handle, drawOffset, null, color * MathHelper.Lerp(0f, 0.5f, MathHelper.Clamp(Empowerment / maxEmpowerment - 0.4f / 0.1f, 0f, 1f)), afterimageRotation, drawOrigin, projectile.scale - 0.2f * ((i / (float)projectile.oldRot.Length)), 0f, 0f);
                }

                spriteBatch.End(); //Haha sup babe what if i restarted the spritebatch way too many times haha /blushes
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                Vector2 afterimageDrawOrigin = new Vector2(0f, blade.Height);
                for (int i = 0; i < projectile.oldRot.Length; ++i)
                {
                    Color color = projectile.GetAlpha(lightColor) * (1f - (i / (float)projectile.oldRot.Length));
                    float afterimageRotation = projectile.oldRot[i] + MathHelper.PiOver4;
                    Main.spriteBatch.Draw(blade, drawOffset, null, color * MathHelper.Lerp(0f, 0.5f, MathHelper.Clamp((Empowerment / maxEmpowerment - 0.4f) / 0.1f, 0f, 1f)), afterimageRotation, afterimageDrawOrigin, projectile.scale - 0.2f * ((i / (float)projectile.oldRot.Length)), 0f, 0f);
                }
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }


            spriteBatch.Draw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            //Turn on additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //Update the parameters
            drawOrigin = new Vector2(0f, blade.Height);

            //Don't draw the glowing blade after the retraction
            float opacityFade = CurrentState == 0f ? 1f : 1f - retractionTimer;
            //Add basic tint (my beloved) during the snap
            if (snapTimer > 0 && retractionTimer <= 0)
            {
                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(MathHelper.Clamp(0.8f - snapTimer, 0f, 1f));
                GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Color.White);
                GameShaders.Misc["CalamityMod:BasicTint"].Apply();
            }

            spriteBatch.Draw(blade, drawOffset, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f * opacityFade, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

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

    public class SwordsmithsPrideBeam : ModProjectile 
    {
        public NPC target;
        public Player Owner => Main.player[projectile.owner];
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_SwordsmithsPrideBeam";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghostly projection");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 32;
            projectile.aiStyle = 27;
            aiType = ProjectileID.LightBeam;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 80;
            projectile.extraUpdates = 1;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {

            if (target == null)
            {
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ProjectileType<PurityProjectionSigil>() && proj.owner == Owner.whoAmI)
                    {
                        target = Main.npc[(int)proj.ai[0]];
                        break;
                    }
                }
            }

            else if ((projectile.Center - target.Center).Length() >= (projectile.Center + projectile.velocity - target.Center).Length() && CalamityUtils.AngleBetween(projectile.velocity, target.Center - projectile.Center) < MathHelper.PiOver4) //Home in
            {
                projectile.timeLeft = 70; //Remain alive
                float angularTurnSpeed = MathHelper.ToRadians(MathHelper.Lerp(15, 2.5f, MathHelper.Clamp(projectile.Distance(target.Center) / 10f, 0f, 1f)));
                float idealDirection = projectile.AngleTo(target.Center);
                float updatedDirection = projectile.velocity.ToRotation().AngleTowards(idealDirection, angularTurnSpeed);
                projectile.velocity = updatedDirection.ToRotationVector2() * projectile.velocity.Length();
            }

            Lighting.AddLight(projectile.Center, 0.75f, 1f, 0.24f);
            int dustParticle = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.CursedTorch, 0f, 0f, 100, default, 0.9f);
            Main.dust[dustParticle].noGravity = true;
            Main.dust[dustParticle].velocity *= 0.5f;
            Main.dust[dustParticle].velocity += projectile.velocity * 0.1f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 75)
                return false;

            DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);

            spriteBatch.End(); //Haha sup babe what if i restarted the spritebatch way too many times haha /blushes
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D tex = Main.projectileTexture[projectile.type];

            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.Lerp(Color.HotPink, Color.GreenYellow, (float)Math.Sin(Main.GlobalTime * 2f)), projectile.rotation, tex.Size() * 0.5f, 1f, 0f, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item43, projectile.Center);
            for (int i = 0; i <= 15; i++)
            {
                Vector2 displace = (projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (-0.5f + (i / 15f)) * 88f;
                int dustParticle = Dust.NewDust(projectile.Center + displace, projectile.width, projectile.height, DustID.CursedTorch, 0f, 0f, 100, default, 2f);
                Main.dust[dustParticle].noGravity = true;
                Main.dust[dustParticle].velocity = projectile.oldVelocity;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int debuffTime = 90;
            target.AddBuff(BuffType<ArmorCrunch>(), debuffTime);
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(Color.HotPink, Color.GreenYellow, (float)Math.Sin(Main.GlobalTime * 2f));
    }

    //Sanguine fury aka Super Pogo attunement

    public class SanguineFury : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_SanguineFury";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref projectile.ai[0]; //How much the attack is, attacking
        public float ShredRatio => MathHelper.Clamp(Shred / (maxShred * 0.5f), 0f, 1f);
        public ref float PogoCooldown => ref projectile.ai[1]; //Cooldown for the pogo
        public ref float BounceTime => ref projectile.localAI[0];
        public Player Owner => Main.player[projectile.owner];
        public bool CanPogo => Owner.velocity.Y != 0 && PogoCooldown <= 0; //Only pogo when in the air and if the cooldown is zero
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;

        public const float pogoStrenght = 16f; //How much the player gets pogoed up
        public const float maxShred = 500; //How much shred you get

        public Projectile Wheel;
        public bool Dashing;
        public Vector2 DashStart;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sanguine Fury");
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

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLenght = 130 * projectile.scale;
            float bladeWidth = 86 * projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Owner.Center + (direction * bladeLenght), bladeWidth, ref collisionPoint);
        }

        public void Pogo()
        {
            if (CanPogo && Main.myPlayer == Owner.whoAmI)
            {
                Owner.velocity = -direction.SafeNormalize(Vector2.Zero) * pogoStrenght; //Bounce
                Owner.fallStart = (int)(Owner.position.Y / 16f);
                PogoCooldown = 30; //Cooldown
                Main.PlaySound(SoundID.DD2_MonkStaffGroundImpact, projectile.position);

                Vector2 hitPosition = Owner.Center + (direction * 100 * projectile.scale);
                BounceTime = 20f; //Used only for animation

                for (int i = 0; i < 8; i++)
                {
                    Vector2 hitPositionDisplace = direction.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-10f, 10f);
                    Vector2 flyDirection = -direction.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));
                    Particle smoke = new SmallSmokeParticle(hitPosition + hitPositionDisplace, flyDirection * 9f, Color.Crimson, new Color(130, 130, 130), Main.rand.NextFloat(1.8f, 2.6f), 155 - Main.rand.Next(30));
                    GeneralParticleHandler.SpawnParticle(smoke);

                    Particle Glow = new StrongBloom(hitPosition - hitPositionDisplace * 3, -direction * 6 * Main.rand.NextFloat(0.5f, 1f), Color.Crimson * 0.5f, 0.01f + Main.rand.NextFloat(0f, 0.2f), 20 + Main.rand.Next(40));
                    GeneralParticleHandler.SpawnParticle(Glow);
                }
                for (int i = 0; i < 3; i++)
                {
                    Vector2 hitPositionDisplace = direction.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-10f, 10f);
                    Vector2 flyDirection = -direction.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));

                    Particle Rock = new StoneDebrisParticle(hitPosition - hitPositionDisplace * 3, flyDirection * Main.rand.NextFloat(3f, 6f), Color.Beige, 1f + Main.rand.NextFloat(0f, 0.4f), 30 + Main.rand.Next(50), 0.1f);
                    GeneralParticleHandler.SpawnParticle(Rock);
                }
            }
        }

        public override void AI()
        {
            if (!initialized) //Initialization. Here its litterally just playing a sound tho lmfao
            {
                Main.PlaySound(SoundID.Item90, projectile.Center);
                initialized = true;

                foreach(Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ProjectileType<SanguineFuryWheel>() && proj.owner == Owner.whoAmI)
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
                        Owner.GiveIFrames(OmegaBiomeBlade.SuperPogoAttunement_SlashIFrames);
                        break;
                    }
                }
            }

            if (!OwnerCanShoot)
            {
                projectile.Kill();
                return;
            }

            if (Shred >= maxShred)
                Shred = maxShred;
            if (Shred < 0)
                Shred = 0;

            Lighting.AddLight(projectile.Center, new Vector3(1f, 0.56f, 0.56f) * ShredRatio);

            //Manage position and rotation
            direction = Owner.DirectionTo(Main.MouseWorld);
            direction.Normalize();
            projectile.rotation = direction.ToRotation();
            projectile.Center = Owner.Center + (direction * 60);

            //Scaling based on shred
            projectile.localNPCHitCooldown = 16 - (int)(MathHelper.Lerp(0, 8, ShredRatio)); //Increase the hit frequency
            projectile.scale = 1f + (ShredRatio * 1f); //SWAGGER


            if ((Wheel == null || !Wheel.active) && Dashing)
            {
                Dashing = false;
                Owner.velocity *= 0.1f; //Abrupt stop

                Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/MeatySlash"), projectile.Center);
                Projectile proj = Projectile.NewProjectileDirect(Owner.Center - DashStart / 2f, Vector2.Zero, ProjectileType<SanguineFuryDash>(), (int)(projectile.damage * OmegaBiomeBlade.SuperPogoAttunement_SlashDamageBoost), 0, Owner.whoAmI);
                if (proj.modProjectile is SanguineFuryDash dash)
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


            if (Collision.SolidCollision(Owner.Center + (direction * 100 * projectile.scale) - Vector2.One * 5f, 10, 10) && !Dashing)
            {
                Pogo();
                projectile.netUpdate = true;
                projectile.netSpam = 0;
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

            Shred--;
            PogoCooldown--;
            BounceTime--;
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
            if (PogoCooldown <= 0)
            {
                Main.PlaySound(SoundID.NPCHit30, projectile.Center); //Sizzle
                Shred += 62; //Augment the shredspeed
                if (Owner.velocity.Y > 0)
                    Owner.velocity.Y = -2f; //Get "stuck" into the enemy partly
                Owner.GiveIFrames(OmegaBiomeBlade.SuperPogoAttunement_ShredIFrames); // i framez.
                PogoCooldown = 20;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCHit43, projectile.Center);
            if (ShredRatio > 0.25 && Owner.whoAmI == Main.myPlayer) 
            {
                Projectile.NewProjectile(projectile.Center, direction * 16f, ProjectileType<SanguineFuryWheel>(), (int)(projectile.damage * OmegaBiomeBlade.SuperPogoAttunement_ShotDamageBoost) , projectile.knockBack, Owner.whoAmI, Shred);
            }
            Owner.Calamity().LungingDown = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D handle = GetTexture("CalamityMod/Items/Weapons/Melee/OmegaBiomeBlade");
            Texture2D blade = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_SanguineFury");

            int bladeAmount = 4;

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            Vector2 drawOffset = Owner.Center + direction * 10f - Main.screenPosition;

            spriteBatch.Draw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            //Turn on additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(MathHelper.Clamp(BounceTime, 0f, 20f) / 20f);
            GameShaders.Misc["CalamityMod:BasicTint"].UseColor(new Color(207, 248, 255));
            GameShaders.Misc["CalamityMod:BasicTint"].Apply();

            //Update the parameters
            drawOrigin = new Vector2(0f, blade.Height);

            spriteBatch.Draw(blade, drawOffset, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);


            for (int i = 0; i < bladeAmount; i++) //Draw extra copies
            {
                blade = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_SanguineFuryExtra");

                drawAngle = direction.ToRotation();

                float circleCompletion = (float)Math.Sin(Main.GlobalTime * 5 + i * MathHelper.PiOver2);
                drawRotation = drawAngle + MathHelper.PiOver4 + (circleCompletion * MathHelper.Pi / 10f) - (circleCompletion * (MathHelper.Pi / 9f) * ShredRatio);

                drawOrigin = new Vector2(0f, blade.Height);

                Vector2 drawOffsetStraight = Owner.Center + direction * (float)Math.Sin(Main.GlobalTime * 7) * 10 - Main.screenPosition; //How far from the player
                Vector2 drawDisplacementAngle = direction.RotatedBy(MathHelper.PiOver2) * circleCompletion.ToRotationVector2().Y * (20 + 40 * ShredRatio); //How far perpendicularly
                Vector2 drawOffsetFromBounce = direction * MathHelper.Clamp(BounceTime, 0f, 20f) / 20f * 20f;

                spriteBatch.Draw(blade, drawOffsetStraight + drawDisplacementAngle + drawOffsetFromBounce, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.8f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            }

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

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

    public class SanguineFuryWheel : ModProjectile 
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_SanguineFuryExtra";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Shred => ref projectile.ai[0];
        public float ShredRatio => MathHelper.Clamp(Shred / (maxShred * 0.5f), 0f, 1f);
        public Player Owner => Main.player[projectile.owner];

        public float Timer => MaxTime - projectile.timeLeft;

        public const float maxShred = 500; //How much shred you get
        public const float MaxTime = 120;



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sanguine Wheel");
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

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < 4; i++) //Draw extra copies
            {
                var tex = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_SanguineFuryExtra");

                float transition = MathHelper.Clamp((Timer - 15 - (i * 20)) / (MaxTime * 0.2f), 0f, 1f);
                transition = MathHelper.Clamp(Timer / MaxTime, 0f, 1f);

                //float circleCompletion = (float)Math.Sin(Main.GlobalTime * 5 + i * MathHelper.PiOver2);
                //float drawAngleShot = MathHelper.WrapAngle(direction.ToRotation() + MathHelper.PiOver4 + (circleCompletion * MathHelper.Pi / 10f) - (circleCompletion * (MathHelper.Pi / 9f) * ShredRatio));
                float drawAngleWheel = MathHelper.WrapAngle(i * MathHelper.PiOver2 + (Main.GlobalTime * 6));
                //Lerp between the position in the shot and the position in  the wheel
                //float drawAngle = MathHelper.WrapAngle(Utils.AngleLerp(drawAngleShot, drawAngleWheel, transition));

                Vector2 drawOrigin = new Vector2(0f, tex.Height);


                //Vector2 drawOffsetStraight = projectile.Center + direction * (float)Math.Sin(Main.GlobalTime * 7) * 10 - Main.screenPosition; //How far from the player
                //Vector2 drawDisplacementAngle = direction.RotatedBy(MathHelper.PiOver2) * circleCompletion.ToRotationVector2().Y * (20 + 40 * ShredRatio); //How far perpendicularly
                //Vector2 drawPositionShot = drawOffsetStraight + drawDisplacementAngle;
                Vector2 drawPositionWheel = projectile.Center - drawAngleWheel.ToRotationVector2() * 20f - Main.screenPosition;

                //Vector2 drawPosition = Vector2.Lerp(drawPositionShot, drawPositionWheel, transition);

                float opacityFade = projectile.timeLeft > 15 ? 1 : projectile.timeLeft / 15f;

                spriteBatch.Draw(tex, drawPositionWheel, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.8f * opacityFade, drawAngleWheel, drawOrigin, projectile.scale * 0.85f, 0f, 0f);
            }

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
                Particle Sparkle = new CritSpark(projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(17.5f, 25f) * projectile.scale, Color.White, Main.rand.NextBool() ? Color.Crimson : Color.DarkRed, 0.4f + Main.rand.NextFloat(0f, 3.5f), 20 + Main.rand.Next(30), 1, 3f);
                GeneralParticleHandler.SpawnParticle(Sparkle);
            }
        }

    }

    public class SanguineFuryDash : ModProjectile
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
                Particle Spark = new CritSpark(target.Center, sparkSpeed, Color.White, Color.Crimson, 1f + Main.rand.NextFloat(0, 1f), 30, 0.4f, 0.6f);
                GeneralParticleHandler.SpawnParticle(Spark);
            }

            Owner.statLife += OmegaBiomeBlade.SuperPogoAttunementSlashLifesteal;
            Owner.HealEffect(OmegaBiomeBlade.SuperPogoAttunementSlashLifesteal);
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

            spriteBatch.Draw(lineTex, DashStart - Main.screenPosition + Shake, null, Color.Lerp(Color.White, Color.Crimson * 0.7f, raise), rot, origin, scale, SpriteEffects.None, 0);

            Texture2D sparkTexture = GetTexture("CalamityMod/Particles/ThinSparkle");
            Texture2D bloomTexture = GetTexture("CalamityMod/Particles/BloomCircle");
            //Ajust the bloom's texture to be the same size as the star's
            float properBloomSize = (float)sparkTexture.Width / (float)bloomTexture.Height;

            
            Rectangle frame = new Rectangle(0, 0, 14, 14);

            spriteBatch.Draw(bloomTexture, DashEnd - Main.screenPosition, null, Color.Crimson * bump * 0.5f, 0, bloomTexture.Size() / 2f, bump * 6f * properBloomSize, SpriteEffects.None, 0);
            spriteBatch.Draw(sparkTexture, DashEnd - Main.screenPosition, frame, Color.Lerp(Color.White, Color.Crimson, raise) * bump, raise * MathHelper.TwoPi, frame.Size() / 2f, bump * 3f, SpriteEffects.None, 0);

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }

    //Mercurial tides aka Shockwave attunement

    public class MercurialTides : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_MercurialTides";
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
            DisplayName.SetDefault("Mercurial Tides");
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

                    Projectile.NewProjectile(Owner.Center, Vector2.Zero, ProjectileType<MercurialTidesBlast>(), (int)(projectile.damage * OmegaBiomeBlade.ShockwaveAttunement_BlastDamageReduction), 10f, Owner.whoAmI, 1f + CurrentIndicator * 0.15f);
                    Main.PlaySound(SoundID.DD2_WitherBeastAuraPulse, projectile.Center);
                    CurrentIndicator++;
                    OverCharge = 20f;
                }

                if (Charge >= MaxCharge)
                {
                    Charge = MaxCharge;
                    if (Owner.whoAmI == Main.myPlayer && CurrentIndicator < 4f)
                    {
                        Projectile.NewProjectile(Owner.Center, Vector2.Zero, ProjectileType<MercurialTidesBlast>(), (int)(projectile.damage * OmegaBiomeBlade.ShockwaveAttunement_BlastDamageReduction), 10f, Owner.whoAmI, 1f + CurrentIndicator * 0.15f);
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

            if (Owner.whoAmI != Main.myPlayer || Owner.velocity.Y == 0f)
                return;

            if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 15)
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = 15;

            Projectile proj = Projectile.NewProjectileDirect(Owner.Center + (direction * 120 * projectile.scale), -direction, ProjectileType<MercurialTidesMonolith>(), (int)(projectile.damage * OmegaBiomeBlade.ShockwaveAttunement_MonolithDamageBoost), 10f, Owner.whoAmI, Main.rand.Next(4), 1f);
            proj.timeLeft = 81;


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
                Projectile proj = Projectile.NewProjectileDirect(projPosition, -monolithRotation, ProjectileType<MercurialTidesMonolith>(), (int)(projectile.damage * OmegaBiomeBlade.ShockwaveAttunement_MonolithDamageBoost), 10f, Owner.whoAmI, Main.rand.Next(4), projSize);
                if (proj.modProjectile is MercurialTidesMonolith monolith)
                {
                    monolith.WaitTimer = (1 - projSize) * 34f;
                    monolith.OriginDirection = direction;
                    monolith.Facing = facing;
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
            Texture2D handle = GetTexture("CalamityMod/Items/Weapons/Melee/OmegaBiomeBlade");
            Texture2D blade = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_MercurialTides");

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            Vector2 drawOffset = projectile.Center - Main.screenPosition;

            spriteBatch.Draw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            //Turn on additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            //Just in case
            if (OverCharge < 0)
                OverCharge = 0f;
            //When the blink is
            GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(OverCharge / 20f);
            GameShaders.Misc["CalamityMod:BasicTint"].UseColor(new Color(154, 244, 240));
            GameShaders.Misc["CalamityMod:BasicTint"].Apply();

            //Update the parameters
            drawOrigin = new Vector2(0f, blade.Height);

            spriteBatch.Draw(blade, drawOffset, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

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

    public class MercurialTidesMonolith : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_MercurialTidesMonolith";
        public Player Owner => Main.player[projectile.owner];
        public float Timer => (100f - projectile.timeLeft) / 100f;
        public ref float Variant => ref projectile.ai[0]; //Yes
        public ref float Size => ref projectile.ai[1]; //Yes
        public float WaitTimer; //How long until the monoliths appears
        public Vector2 OriginDirection; //The direction of the original strike
        public float Facing; //The direction of the original strike
        public const float BaseWidth = 90f;
        public const float BaseHeight = 420f;

        public CurveSegment StaySmall = new CurveSegment(EasingType.Linear, 0f, 0.2f, 0f);
        public CurveSegment GoBig = new CurveSegment(EasingType.SineOut, 0.25f, 0.2f, 1f);
        public CurveSegment GoNormal = new CurveSegment(EasingType.CircIn, 0.4f, 1.2f, -0.2f);
        public CurveSegment StayNormal = new CurveSegment(EasingType.Linear, 0.5f, 1f, 0f);

        internal float Width() => PiecewiseAnimation(Timer, new CurveSegment[] { StaySmall, GoBig, GoNormal, StayNormal }) * BaseWidth * Size;

        public CurveSegment Anticipate = new CurveSegment(EasingType.CircIn, 0f, 0f, 0.15f);
        public CurveSegment Overextend = new CurveSegment(EasingType.SineOut, 0.2f, 0.15f, 1f);
        public CurveSegment Unextend = new CurveSegment(EasingType.CircIn, 0.25f, 1.15f, -0.15f);
        public CurveSegment Hold = new CurveSegment(EasingType.ExpOut, 0.70f, 1f, -0.1f);
        internal float Height() => PiecewiseAnimation(Timer, new CurveSegment[] { Anticipate, Overextend, Unextend, Hold }) * BaseHeight * Size;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mercurial Monolith");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 70;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 102;
            projectile.hide = true;
        }

        public override bool CanDamage() => WaitTimer <= 0;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + ((projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * Height()), Width(), ref collisionPoint);
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {
            if (projectile.velocity != Vector2.Zero)
            {
                SurfaceUp();
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                projectile.velocity = Vector2.Zero;
            }

            if (WaitTimer > 0)
            {
                projectile.timeLeft = 102;
                WaitTimer--;
            }

            if (projectile.timeLeft == 100)
            {
                if (Size >= 1) //Big monoliths create sparkles even before sprouting up
                {

                    for (int i = 0; i < 20; i++)
                    {
                        Particle Sparkle = new CritSpark(projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(7.5f, 20f), Color.White, Main.rand.NextBool() ? Color.MediumTurquoise : Color.DarkOrange, 0.1f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                        GeneralParticleHandler.SpawnParticle(Sparkle);
                    }
                }

                if (Size * 0.8 > 0.4 && Facing != 0)
                {
                    SideSprouts(Facing, 150f, Size * 0.8f);
                }
            }

            if (projectile.timeLeft == 80)
            {
                Vector2 particleDirection = (projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
                Main.PlaySound(SoundID.DD2_EtherianPortalDryadTouch, projectile.Center);

                for (int i = 0; i < 8; i++)
                {
                    Vector2 hitPositionDisplace = particleDirection.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0f, 10f);
                    Vector2 flyDirection = particleDirection.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * Main.rand.NextFloat(5f, 15f);
                    Particle smoke = new SmallSmokeParticle(projectile.Center + hitPositionDisplace, flyDirection, Color.Lerp(Color.DarkOrange, Color.MediumTurquoise, Main.rand.NextFloat()), new Color(130, 130, 130), Main.rand.NextFloat(2.8f, 3.6f) * Size, 165 - Main.rand.Next(30), 0.1f);
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
                for (int i = 0; i < 6; i++)
                {
                    Vector2 hitPositionDisplace = particleDirection.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(10f, 30f);
                    Vector2 flyDirection = particleDirection.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));

                    Particle Rock = new StoneDebrisParticle(projectile.Center + hitPositionDisplace * 3, flyDirection * Main.rand.NextFloat(3f, 6f), Color.Lerp(Color.DarkSlateBlue, Color.LightSlateGray, Main.rand.NextFloat()), (1f + Main.rand.NextFloat(0f, 2.4f)) * Size, 30 + Main.rand.Next(50), 0.1f);
                    GeneralParticleHandler.SpawnParticle(Rock);
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
                Projectile proj = Projectile.NewProjectileDirect(projPosition, -monolithRotation, ProjectileType<MercurialTidesMonolith>(), projectile.damage, 10f, Owner.whoAmI, Main.rand.Next(4), projSize);
                if (proj.modProjectile is MercurialTidesMonolith monolith)
                {
                    monolith.WaitTimer = (float)Math.Sqrt(1.0 - Math.Pow(projSize - 1.0, 2)) * 3f ;
                    monolith.OriginDirection = OriginDirection;
                    monolith.Facing = facing;
                }
            }

            return validPositionFound;
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (WaitTimer > 0)
                return false;

            Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_MercurialTidesMonolith");

            Vector2 Shake = projectile.timeLeft < 70 ? Vector2.Zero : Vector2.One.RotatedByRandom(MathHelper.TwoPi) * (70 - projectile.timeLeft / 30f) * 0.05f;

            float drawAngle = projectile.rotation;
            Rectangle frame = new Rectangle(0 + (int)Variant * 94, 0, 94, 420);

            Vector2 drawScale = new Vector2(Width() / BaseWidth, Height() / BaseHeight);
            Vector2 drawPosition = projectile.Center - Main.screenPosition - (projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 26f;
            Vector2 drawOrigin = new Vector2(frame.Width / 2f, frame.Height);

            float opacity = MathHelper.Clamp(1f - ((Timer - 0.85f) / 0.15f), 0f, 1f);

            spriteBatch.Draw(tex, drawPosition + Shake, frame, lightColor * opacity, drawAngle, drawOrigin, drawScale, 0f, 0f);

            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (WaitTimer > 0)
                return;

            Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_TheirAbhorrenceMonolith_Glow");

            float drawAngle = projectile.rotation;
            Rectangle frame = new Rectangle(0 + (int)Variant * 94, 0, 94, 420);

            Vector2 drawScale = new Vector2(Width() / BaseWidth, Height() / BaseHeight);
            Vector2 drawPosition = projectile.Center - Main.screenPosition - (projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 26f;
            Vector2 drawOrigin = new Vector2(frame.Width / 2f, frame.Height);

            float opacity = MathHelper.Clamp(1f - ((Timer - 0.85f) / 0.15f), 0f, 1f);

            spriteBatch.Draw(tex, drawPosition, frame, Color.White * opacity, drawAngle, drawOrigin, drawScale, 0f, 0f);
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

    public class MercurialTidesBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_MercurialTidesShockwave";
        public Player Owner => Main.player[projectile.owner];
        public float Timer => (60f - projectile.timeLeft) / 100f;
        public ref float Variant => ref projectile.ai[0]; //Yes
        public ref float Size => ref projectile.ai[0]; //Yes

        public Particle BloomRing;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mercurial Blast");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 170;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
        }

        public override bool CanDamage() => projectile.timeLeft < 40;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center + (projHitbox.Size() * projectile.scale * 0.5f), projHitbox.Size() * projectile.scale);
        }

        public override void AI()
        {
            projectile.velocity = Vector2.Zero;
            projectile.scale = (1 + (float)Math.Sin(projectile.timeLeft / 60f * MathHelper.Pi) * 0.2f) * Size;

            if (projectile.timeLeft == 60)
            {
                Main.PlaySound(SoundID.Item79, projectile.Center);
                Particle Sparkle = new GenericSparkle(projectile.Center, Vector2.Zero, Color.White,  Main.rand.NextBool() ? Color.Aqua : Color.SpringGreen, projectile.scale, 20 ,0.2f, 2);
                GeneralParticleHandler.SpawnParticle(Sparkle);
            }



            if (projectile.timeLeft == 40)
            {
                Main.PlaySound(SoundID.DD2_ExplosiveTrapExplode, projectile.Center);

                BloomRing = new BloomRing(projectile.Center, Vector2.Zero, Color.Aqua, projectile.scale, 40);
                GeneralParticleHandler.SpawnParticle(BloomRing);

                Particle Bloom = new StrongBloom(projectile.Center, Vector2.Zero, Main.rand.NextBool() ? Color.Aqua * 0.6f : Color.SpringGreen * 0.6f, projectile.scale * (1f + Main.rand.NextFloat(0f, 1.5f)), 20);
                GeneralParticleHandler.SpawnParticle(Bloom);
                for (int i = 0; i < 10; i++)
                {
                    Particle Sparkle = new CritSpark(projectile.Center, Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(17.5f, 25f) * projectile.scale, Color.White, Main.rand.NextBool() ? Color.DarkSlateBlue : Color.Chocolate, 0.1f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                    GeneralParticleHandler.SpawnParticle(Sparkle);
                }

                for (float i = 0f; i < 1; i += 0.05f)
                {
                    float rotation = i * MathHelper.TwoPi;
                    Particle Sparkle = new CritSpark(projectile.Center + rotation.ToRotationVector2() * 65f * projectile.scale, rotation.ToRotationVector2() * 10f, Color.White, Main.rand.NextBool() ? Color.Aqua : Color.SpringGreen, 0.1f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                    GeneralParticleHandler.SpawnParticle(Sparkle);
                }
            }

            if (BloomRing != null)
            {
                BloomRing.Scale = projectile.scale;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_MercurialTidesShockwave");

            float drawAngle = projectile.rotation;
            int animFrame = 6 - (int)Math.Ceiling(projectile.timeLeft / 10f);
            Rectangle frame = new Rectangle(0, 0 + animFrame * 168, 170, 166);

            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = frame.Size() / 2f;

            spriteBatch.Draw(tex, drawPosition, frame, Color.White, drawAngle, drawOrigin, projectile.scale, 0f, 0f);

            
            return false;
        }
    }

    //Lamentations of the Chained aka FlailBlade attunement

    public class LamentationsOfTheChained : ModProjectile
    {
        private NPC[] excludedTargets = new NPC[4];

        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChained";
        private bool initialized = false;
        public Player Owner => Main.player[projectile.owner];
        public float Timer => MaxTime - projectile.timeLeft;
        public ref float ChainSwapTimer => ref projectile.ai[0];
        public ref float SnapCoyoteTime => ref projectile.ai[1];

        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;

        public float Size;
        public float flipped;

        public static readonly float MaxTime = (float)OmegaBiomeBlade.FlailBladeAttunement_FlailTime;
        const int coyoteTimeFrames = 8; //How many frames does the whip stay extended 
        const int MaxReach = 400;
        const float SnappingPoint = 0.45f; //When does the snap occur.
        const float ReelBackStrenght = 14f;

        const float MaxTangleReach = 400f; //How long can tangling vines from crits be

        //Doing your mom. Be warned that the X value of these vectors represents the angle of the whip while the Y value represents its RNG seed
        public Vector2 whip1;
        public Vector2 whip2;
        public Vector2 whip3;

        internal bool ReelingBack => Timer / MaxTime > SnappingPoint;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lamentations of the Chained");
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 80;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {

            //int numPoints = 32;
            //Vector2[] chainPositions = curve.GetPoints(numPoints).ToArray();
            //float collisionPoint = 0;
            //for (int i = 1; i < numPoints; i++)
            //{
            //    Vector2 position = chainPositions[i];
            //    Vector2 previousPosition = chainPositions[i - 1];
            //    if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
            //        return true;
            //    if (i == numPoints - 1) //Extra lenght collision for the blade itself
            //    {
            //        Vector2 projectileHalfLenght = 85 * projectile.rotation.ToRotationVector2();
            //        return (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - projectileHalfLenght, projectile.Center + projectileHalfLenght, 32, ref collisionPoint));
            //    }

            //}

            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Vector2 projectileHalfLenght = 85f * projectile.rotation.ToRotationVector2();
            float collisionPoint = 0;
            //If you hit the enemy during the coyote time with the blade of the whip, guarantee a crit
            if (Collision.CheckAABBvLineCollision(target.Hitbox.TopLeft(), target.Hitbox.Size(), projectile.Center - projectileHalfLenght, projectile.Center + projectileHalfLenght, 32, ref collisionPoint))
            {
                if (SnapCoyoteTime > 0f)
                {
                    crit = true;
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 sparkSpeed = Owner.DirectionTo(target.Center).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * 9f;
                        Particle Spark = new CritSpark(target.Center, sparkSpeed, Color.White, Color.LimeGreen, 1f + Main.rand.NextFloat(0, 1f), 30, 0.4f, 0.6f);
                        GeneralParticleHandler.SpawnParticle(Spark);
                    }

                }
            }
            else
                damage = (int)(damage * OmegaBiomeBlade.FlailBladeAttunement_ChainDamageReduction); //If the enemy is hit with the chain of the whip, the damage gets reduced
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/SwiftSlice"), projectile.Center);
                excludedTargets[0] = target;
                for (int i = 0; i < 3; i++)
                {
                    NPC potentialTarget = TargetNext(target.Center, i);
                    if (potentialTarget == null)
                        break;
                    Projectile.NewProjectile(target.Center, Vector2.Zero, ProjectileType<GhastlyChain>(), (int)(damage * OmegaBiomeBlade.FlailBladeAttunement_GhostChainDamageReduction), 0, Owner.whoAmI, target.whoAmI, potentialTarget.whoAmI);
                }
                Array.Clear(excludedTargets, 0, 3);
            }
        }

        public NPC TargetNext(Vector2 hitFrom, int index)
        {
            float longestReach = MaxTangleReach;
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

        public override void AI()
        {
            if (!initialized) //Initialization. create control points & shit)
            {
                projectile.timeLeft = (int)MaxTime;
                initialized = true;
                projectile.netUpdate = true;
                projectile.netSpam = 0;
            }

            if (!OwnerCanShoot)
            {
                projectile.Kill();
                return;
            }

            projectile.velocity = Owner.DirectionTo(Main.MouseWorld);
            projectile.velocity.Normalize();
            projectile.rotation = projectile.velocity.ToRotation();
            projectile.Center = Owner.Center + (projectile.velocity * 60);

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            Owner.direction = Math.Sign(projectile.velocity.X);
            Owner.itemRotation = projectile.rotation;
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            projectile.timeLeft = 2;

            if (ChainSwapTimer % 5 == 0)
            {
                Main.PlaySound(SoundID.DD2_OgreSpit, projectile.Center);
            }
            //if ((ChainSwapTimer - 12f) % 23 == 0)
            //{
            //    Main.PlaySound(SoundID.Item41, projectile.Center);
            //}

            ChainSwapTimer++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D handle = GetTexture("CalamityMod/Items/Weapons/Melee/OmegaBiomeBlade");
            Texture2D blade = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChained");

            CalculateChains(spriteBatch, out Vector2[] chainPositions1, out Vector2[] chainPositions2, out Vector2[] chainPositions3);

            //float drawRotation = (projBottom - chainPositions[chainPositions.Length - 2]).ToRotation() +  MathHelper.PiOver4; //Face away from the last point of the bezier curve

            Vector2 drawPos = projectile.Center - projectile.velocity * 55f;
            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            float drawRotation = projectile.rotation + MathHelper.PiOver4;

            //lightColor = Lighting.GetColor((int)(projectile.Center.X / 16f), (int)(projectile.Center.Y / 16f));

            spriteBatch.Draw(handle, drawPos - Main.screenPosition, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);


            //Turn on additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            //Draw the tip of the flails here
            Texture2D flailBlade = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChainedFlail");
            Rectangle bladeFrame = new Rectangle(0, 0, 24, 48);
            Vector2 flailOrigin = new Vector2(bladeFrame.Width / 2, bladeFrame.Height); //Draw from center bottom of texture

            float flailRotation = (chainPositions1[chainPositions1.Length - 2] - chainPositions1[chainPositions1.Length - 3]).ToRotation() + MathHelper.PiOver2;
            spriteBatch.Draw(flailBlade, chainPositions1[chainPositions1.Length - 2] - Main.screenPosition, bladeFrame, Color.White, flailRotation, flailOrigin, 1, SpriteEffects.None, 0);
    
            flailRotation = (chainPositions2[chainPositions2.Length - 2] - chainPositions2[chainPositions1.Length - 3]).ToRotation() + MathHelper.PiOver2;
            spriteBatch.Draw(flailBlade, chainPositions2[chainPositions2.Length - 2] - Main.screenPosition, bladeFrame, Color.White, flailRotation, flailOrigin, 1, SpriteEffects.None, 0);

            flailRotation = (chainPositions3[chainPositions3.Length - 2] - chainPositions3[chainPositions3.Length - 3]).ToRotation() + MathHelper.PiOver2;
            spriteBatch.Draw(flailBlade, chainPositions3[chainPositions3.Length - 2] - Main.screenPosition, bladeFrame, Color.White, flailRotation, flailOrigin, 1, SpriteEffects.None, 0);

            drawOrigin = new Vector2(0f, blade.Height);
            spriteBatch.Draw(blade, drawPos - Main.screenPosition, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            //Back to normal
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        //TLDR : https://media.discordapp.net/attachments/659100646397575208/933521303443501136/placeholder.gif 
        private void GenerateCurve(float seed, Vector2 direction, out Vector2 control0, out Vector2 control1, out Vector2 control2, out Vector2 control3, float angleShift = 0f, float necessaryOrientation = 1f)
        {
            float randomNumber = 0.5f + ((float)Math.Sin(seed * 17.07947) + (float)Math.Sin(seed * 0.2f * 25.13274)) * 0.25f ; //Ty dom for the awesome pseudo rng
            //Get new random numbers based on the same seed
            float seed1 = 0.5f + ((float)Math.Sin(randomNumber * 100f * 17.07947) + (float)Math.Sin(randomNumber * 100f * 0.2f * 25.13274)) * 0.25f;
            float seed2 = 0.5f + ((float)Math.Sin(seed1 * 50f * 17.07947) + (float)Math.Sin(seed1 * 50f * 0.2f * 25.13274)) * 0.25f;
            float seed3 = 0.5f + ((float)Math.Sin(seed2 * 17.07947) + (float)Math.Sin(seed2 * 0.2f * 25.13274)) * 0.25f;

            if ((necessaryOrientation == -1 && randomNumber >= 0.5) || (necessaryOrientation == 1 && randomNumber < 0.5))
                randomNumber = 1 - randomNumber;


            float flip = randomNumber >= 0.5 ? 1f : -1f;


            control0 = Owner.MountedCenter + direction.RotatedBy(MathHelper.ToRadians(MathHelper.Lerp(MathHelper.PiOver4 * 0.3f * flip, MathHelper.PiOver4 * flip, randomNumber))) * MathHelper.Lerp(50f, 130f, (float)Math.Sin(randomNumber * MathHelper.Pi));

            float easedShift = angleShift == 1 ? 1 : 1 - (float)Math.Pow(2, -10 * angleShift);
            float Reach = MaxReach * (0.75f + 0.75f * seed2 - 0.05f * easedShift);
            control3 = Owner.MountedCenter + (direction * Reach).RotatedBy(MathHelper.Lerp(-0.01f * flip, 0.01f * flip, easedShift)); // The last point of the curve gets put straight at the front

            //Over the double fucking bezier rainbow
            Vector2 point1 = control3 + direction.RotatedBy(MathHelper.PiOver2) * 50;
            Vector2 point2 = control3 + direction.RotatedBy(MathHelper.PiOver2) * 50  + direction * 250;
            Vector2 point3 = control3 + direction.RotatedBy(-MathHelper.PiOver2) * 50  + direction * 250;
            Vector2 point4 = control3 + direction.RotatedBy(-MathHelper.PiOver2) * 50;
            BezierCurve curve = new BezierCurve(new Vector2[] { point1, point2, point3, point4 });
            control3 = curve.Evaluate(randomNumber);

            Vector2 directionFromHead = direction.RotatedBy(MathHelper.ToRadians(MathHelper.Lerp(0, 160f * flip, (float)Math.Sin(randomNumber * MathHelper.Pi)))) * MathHelper.Lerp(130f, 200f, MathHelper.Clamp((float)Math.Sin(randomNumber * MathHelper.Pi) - 0.5f, 0f, 1f) * 2f);
            control2 = control3 + directionFromHead;

            Vector2 directionFromSecondToLastPoint = Vector2.Normalize(directionFromHead.RotatedBy(MathHelper.Pi - MathHelper.ToRadians(MathHelper.Lerp(80f * flip, 110f * flip, (float)Math.Sin(randomNumber * MathHelper.Pi))))) * MathHelper.Lerp(120f, 280f, (float)Math.Sin(randomNumber * MathHelper.Pi));
            control1 = control2 + directionFromSecondToLastPoint;

            control3 += Vector2.UnitX.RotatedBy(MathHelper.TwoPi * seed1) * seed3 * 30f;
            control2 += Vector2.UnitX.RotatedBy(MathHelper.TwoPi * seed2) * seed1 * 30f;
            control1 += Vector2.UnitX.RotatedBy(MathHelper.TwoPi * seed3) * seed2 * 30f;
        }

        private void CalculateChains(SpriteBatch spriteBatch, out Vector2[] chainPositions1, out Vector2[] chainPositions2, out Vector2[] chainPositions3)
        {

            if (ChainSwapTimer % 6 == 0 || whip1.Y == 0)
            {
                whip1.X = projectile.velocity.RotatedBy(Main.rand.NextFloat(MathHelper.PiOver4 / 16f, -MathHelper.PiOver4 / 16f)).ToRotation(); //X is the orientation
                whip1.Y = Main.rand.NextFloat(0.2f, 100f);//Y is the "seed"

                ChainSwapTimer++;
            }

            if ((ChainSwapTimer - 2) % 6 == 0 || whip2.Y == 0)
            {
                whip2.X = projectile.velocity.RotatedBy(Main.rand.NextFloat(MathHelper.Pi / 16f, -MathHelper.Pi / 16f)).ToRotation(); //X is the orientation
                whip2.Y = Main.rand.NextFloat(0.2f, 100f);//Y is the "seed"

                ChainSwapTimer++;
            }

            if ((ChainSwapTimer - 4) % 6 == 0 || whip3.Y == 0)
            {
                whip3.X = projectile.velocity.RotatedBy(Main.rand.NextFloat(MathHelper.Pi / 16f, -MathHelper.Pi / 16f)).ToRotation(); //X is the orientation
                whip3.Y = Main.rand.NextFloat(0.2f, 100f);//Y is the "seed"

                ChainSwapTimer++;
            }


            GenerateCurve(whip1.Y, whip1.X.ToRotationVector2(), out Vector2 control0, out Vector2 control1, out Vector2 control2, out Vector2 control3, (ChainSwapTimer % OmegaBiomeBlade.FlailBladeAttunement_FlailTime) / (float)OmegaBiomeBlade.FlailBladeAttunement_FlailTime, 1);
            DrawChain(spriteBatch, out chainPositions1, control0, control1, control2, control3);

            GenerateCurve(whip2.Y, whip2.X.ToRotationVector2(), out control0, out control1, out control2, out control3, ((ChainSwapTimer - OmegaBiomeBlade.FlailBladeAttunement_FlailTime / 3f) % OmegaBiomeBlade.FlailBladeAttunement_FlailTime) / (float)OmegaBiomeBlade.FlailBladeAttunement_FlailTime, -1);
            DrawChain(spriteBatch, out chainPositions2, control0, control1, control2, control3);

            GenerateCurve(whip3.Y, whip3.X.ToRotationVector2(), out control0, out control1, out control2, out control3, ((ChainSwapTimer - OmegaBiomeBlade.FlailBladeAttunement_FlailTime * 2f / 3f) % OmegaBiomeBlade.FlailBladeAttunement_FlailTime) / (float)OmegaBiomeBlade.FlailBladeAttunement_FlailTime, -1);
            DrawChain(spriteBatch, out chainPositions3, control0, control1, control2, control3);
        }

        private void DrawChain(SpriteBatch spriteBatch, out Vector2[] chainPositions, Vector2 control0, Vector2 control1, Vector2 control2, Vector2 control3)
        {

            Texture2D tex = GetTexture("CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChainedFlail");
            Rectangle chainFrame = new Rectangle(0, 70, 24, 14);
            Rectangle guardFrame = new Rectangle(0, 50, 24, 18);

            //First chain
            BezierCurve curve = new BezierCurve(new Vector2[] { Owner.MountedCenter, control0, control1, control2, control3 });
            int numPoints = 40;
            chainPositions = curve.GetPoints(numPoints).ToArray();

            //Draw each chain segment bar the very first one
            for (int i = 1; i < numPoints; i++)
            {
                Vector2 position = chainPositions[i];

                float rotation = (chainPositions[i] - chainPositions[i - 1]).ToRotation() + MathHelper.PiOver2; //Calculate rotation based on direction from last point
                Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment

                if (i < numPoints - 1)
                {
                    float yScale = Vector2.Distance(chainPositions[i], chainPositions[i - 1]) / chainFrame.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points

                    float segmentProgress = i < 30f ? i / 30f : 1 - (i - 30f) / 10f;
                    float xScale = 1f + 0.75f * (float)Math.Sin(segmentProgress * MathHelper.PiOver2);

                    Vector2 scale = new Vector2(xScale, yScale); //Remember to make it get thicker and thinner based on whatever
                    Vector2 origin = new Vector2(chainFrame.Width / 2, chainFrame.Height); //Draw from center bottom of texture
                    spriteBatch.Draw(tex, chainPositions[i] - Main.screenPosition, chainFrame, chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
                }
                else
                {
                    Vector2 origin = new Vector2(guardFrame.Width / 2, guardFrame.Height); //Draw from center bottom of texture
                    spriteBatch.Draw(tex, chainPositions[i] - Main.screenPosition, guardFrame, chainLightColor, rotation, origin, 1, SpriteEffects.None, 0);

                    if (Main.rand.Next(6) == 0)
                    {
                        Particle Spark = new CritSpark(chainPositions[i], Vector2.Zero, Color.White, Color.SteelBlue, 1f + Main.rand.NextFloat(0, 1f), 30, 0.4f, 0.6f);
                        GeneralParticleHandler.SpawnParticle(Spark);
                    }
                }
            }
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
            writer.Write(flipped);
            writer.Write(Size);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            flipped = reader.ReadSingle();
            Size = reader.ReadSingle();
        }
    }

    public class GhastlyChain : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_LamentationsOfTheChainedChain";
        public Player Owner => Main.player[projectile.owner];
        public float Timer => 20 - projectile.timeLeft;
        public NPC NPCfrom
        {
            get => Main.npc[(int)projectile.ai[0]];
            set => projectile.ai[0] = value.whoAmI;
        }
        public NPC Target
        {
            get => Main.npc[(int)projectile.ai[1]];
            set => projectile.ai[1] = value.whoAmI;
        }

        const float curvature = 16f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghastly Chain");
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

        public override bool? CanHitNPC(NPC target) => target == Target;

        public override void AI()
        {
            projectile.Center = Target.Center;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D chainTex = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_GrovetendersTouchChain");

            float opacity = projectile.timeLeft > 10 ? 1 : projectile.timeLeft / 10f;
            Vector2 Shake = projectile.timeLeft < 15 ? Vector2.Zero : Vector2.One.RotatedByRandom(MathHelper.TwoPi) * (15 - projectile.timeLeft / 5f) * 0.5f;

            Vector2 lineDirection = Vector2.Normalize(Target.Center - NPCfrom.Center);
            int dist = (int)Vector2.Distance(Target.Center, NPCfrom.Center) / 16;
            Vector2[] Nodes = new Vector2[dist + 1];
            Nodes[0] = NPCfrom.Center;
            Nodes[dist] = Target.Center;
            float pointUp = Target.Center.X > NPCfrom.Center.X ? -MathHelper.PiOver2 : MathHelper.PiOver2;

            for (int i = 1; i < dist + 1; i++)
            {
                Vector2 positionAlongLine = Vector2.Lerp(NPCfrom.Center, Target.Center, i / (float)dist); //Get the position of the segment along the line, as if it were a flat line
                float elevation = (float)Math.Sin(i / (float)dist * MathHelper.Pi) * curvature * dist / 10f;
                Nodes[i] = positionAlongLine + lineDirection.RotatedBy(pointUp) * elevation + Shake * (float)Math.Sin(i / (float)dist * MathHelper.Pi);

                float rotation = (Nodes[i] - Nodes[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(Nodes[i], Nodes[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                Vector2 scale = new Vector2(1, yScale);

                Color chainLightColor = Lighting.GetColor((int)Nodes[i].X / 16, (int)Nodes[i].Y / 16); //Lighting of the position of the chain segment

                Vector2 origin = new Vector2(chainTex.Width / 2, chainTex.Height); //Draw from center bottom of texture
                spriteBatch.Draw(chainTex, Nodes[i] - Main.screenPosition, null, chainLightColor * opacity, rotation, origin, scale, SpriteEffects.None, 0);
            }
            return false;
        }

    }

}


