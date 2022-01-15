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
        //How much damage reduction applies on uncharged attacks
        public const float initialDamage = 0.2f;
        //How much extra damage applies when fully charged. This goes ontop of the initial damage, so a max empowerment boost of 2 with an initial damage of 0.1 makes the total damage multiplier be 1.9
        public float maxEmpowermentBoost = 2f;
        public float throwTimer => throwOutTime - projectile.timeLeft;

        public Particle smear;
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
                        hasMadeSound = 1f;
                        Main.PlaySound(SoundID.Item71);
                    }
                }

                if (Empowerment / maxEmpowerment >= 0.5)
                {

                    if ((Empowerment + OverEmpowerment) % (40 + (int)(40 * (1 - Empowerment / maxEmpowerment))) == 9)
                    {
                        Vector2 shotDirection = Main.rand.NextVector2CircularEdge(15f, 15f);
                        if (lastTarget != null && lastTarget.active) //If you've got an actual target, angle your shots towards them
                        {
                            shotDirection = (shotDirection.ToRotation().AngleTowards(Owner.AngleTo(lastTarget.Center), MathHelper.PiOver2)).ToRotationVector2() * 15f;
                        }
                        Projectile.NewProjectile(Owner.Center, shotDirection, ProjectileType<SwordsmithsPrideBeam>(), (int)(projectile.damage * 0.5f), 0f, Owner.whoAmI);
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
            damage = (int)(damage * (initialDamage + (maxEmpowermentBoost * Empowerment / maxEmpowerment)));

            if (CurrentState != 1)
                return;

            lastTarget = target;

            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ProjectileType<PurityProjectionSigil>() && proj.owner == Owner.whoAmI)
                {
                    //Reset the timeleft on the sigil & give it its new target (or the same, it doesnt matter really.
                    proj.ai[0] = target.whoAmI;
                    proj.timeLeft = 1200;
                    return;
                }
            }
            Projectile sigil = Projectile.NewProjectileDirect(target.Center, Vector2.Zero, ProjectileType<PurityProjectionSigil>(), 0, 0, Owner.whoAmI, target.whoAmI);
            sigil.timeLeft = 1200;
        }

        public override void Kill(int timeLeft)
        {
            if (smear != null)
                smear.Kill();
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
        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_PurityProjection";

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

            else if ((projectile.Center - target.Center).Length() >= (projectile.Center + projectile.velocity - target.Center).Length() && CalamityUtils.AngleBetween(projectile.velocity, target.Center - projectile.Center) < MathHelper.PiOver2 * 1.5f) //Home in
            {
                projectile.timeLeft = 70; //Remain alive
                float angularTurnSpeed = MathHelper.ToRadians(MathHelper.Lerp(25, 10.5f, MathHelper.Clamp(projectile.Distance(target.Center) / 10f, 0f, 1f)));
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
    }

    //Mercurial tides

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

                    Projectile.NewProjectile(Owner.Center, Vector2.Zero, ProjectileType<MercurialTidesBlast>(), (int)(projectile.damage * 0.6f), 10f, Owner.whoAmI, 1f + CurrentIndicator * 0.15f);
                    Main.PlaySound(SoundID.DD2_WitherBeastAuraPulse, projectile.Center);
                    CurrentIndicator++;
                    OverCharge = 20f;
                }

                if (Charge >= MaxCharge)
                {
                    Charge = MaxCharge;
                    if (Owner.whoAmI == Main.myPlayer && CurrentIndicator < 4f)
                    {
                        Projectile.NewProjectile(Owner.Center, Vector2.Zero, ProjectileType<MercurialTidesBlast>(), (int)(projectile.damage * 0.6f), 10f, Owner.whoAmI, 1f + CurrentIndicator * 0.15f);
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

            if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 5)
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = 5;

            Projectile.NewProjectile(Owner.Center + (direction * 120 * projectile.scale), -direction, ProjectileType<MercurialTidesMonolith>(), projectile.damage * 2, 10f, Owner.whoAmI, Main.rand.Next(4), 1f);

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
                Projectile proj = Projectile.NewProjectileDirect(projPosition, -monolithRotation, ProjectileType<MercurialTidesMonolith>(), projectile.damage * 2, 10f, Owner.whoAmI, Main.rand.Next(4), projSize);
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
            Owner.GiveIFrames(30);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * (1f + Charge / MaxCharge));
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
                Projectile proj = Projectile.NewProjectileDirect(projPosition, -monolithRotation, ProjectileType<MercurialTidesMonolith>(), projectile.damage * 2, 10f, Owner.whoAmI, Main.rand.Next(4), projSize);
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
}


