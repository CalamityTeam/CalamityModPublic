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


namespace CalamityMod.Projectiles.Melee
{
    public class ArkoftheElementsSwungBlade : ModProjectile //"Kill la kill reference i wish"? Stop wishing, start creating!
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/RendingScissorsRight";

        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public ref float Combo => ref projectile.ai[0];
        public ref float Charge => ref projectile.ai[1];
        public Player Owner => Main.player[projectile.owner];


        #region Regular swing variables
        const float MaxSwingTime = 35;
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

        private float SwingWidth = MathHelper.PiOver2 * 1.5f;
        public Vector2 DistanceFromPlayer => direction * 30;
        public float SwingTimer => MaxSwingTime - projectile.timeLeft;
        public float SwingCompletion => SwingTimer / MaxSwingTime;
        public ref float HasFired => ref projectile.localAI[0];
        #endregion

        #region Throw variables
        //Only used for the long range throw
        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed && Owner.HeldItem.type == ItemType<ArkoftheElements>();
        public bool Thrown => Combo == 2 || Combo == 3;
        const float MaxThrowTime = 80;
        const float ThrowReachMax = 500;
        const float ThrowReachMin = 200;
        public float ThrowReach;
        public float ThrowTimer => MaxThrowTime - projectile.timeLeft;
        public float ThrowCompletion => ThrowTimer / MaxThrowTime;

        const float SnapWindowStart = 0.4f;
        const float SnapWindowEnd = 0.6f;
        public float SnapEndTime => (MaxThrowTime - (MaxThrowTime * SnapWindowEnd));
        public float SnapEndCompletion => (SnapEndTime - projectile.timeLeft) / SnapEndTime;
        public ref float ChanceMissed => ref projectile.localAI[1];

        #endregion

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ark of the Elements");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 60;
            projectile.width = projectile.height = 60;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = Thrown ? 10 : (int)MaxSwingTime;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float bladeLenght = 142f * projectile.scale;

            if (Thrown)
            {
                bool mainCollision = Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - Vector2.One * bladeLenght / 2f, Vector2.One * bladeLenght);
                if (Combo == 2f)
                    return mainCollision;

                else
                {
                    Vector2 thrownBladeStart = Vector2.SmoothStep(Owner.Center, projectile.Center, MathHelper.Clamp(SnapEndCompletion + 0.25f, 0f, 1f));
                    bool thrownScissorCollision = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), thrownBladeStart, thrownBladeStart + direction * bladeLenght);
                    return mainCollision || thrownScissorCollision;
                }
            }

            float collisionPoint = 0f;
            Vector2 holdPoint = DistanceFromPlayer.Length() * projectile.rotation.ToRotationVector2();

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + holdPoint, Owner.Center + holdPoint + projectile.rotation.ToRotationVector2() * bladeLenght, 24, ref collisionPoint);
        }

        //Swing animation keys
        public CurveSegment anticipation = new CurveSegment(EasingType.ExpOut, 0f, 0f, 0.15f);
        public CurveSegment thrust = new CurveSegment(EasingType.PolyInOut, 0.1f, 0.15f, 0.85f, 3);
        public CurveSegment hold = new CurveSegment(EasingType.Linear, 0.5f, 1f, 0.2f);
        internal float SwingRatio() => PiecewiseAnimation(SwingCompletion, new CurveSegment[] { anticipation, thrust, hold });

        //Throw animation keys
        public CurveSegment shoot = new CurveSegment(EasingType.CircOut, 0f, 0f, 1f);
        public CurveSegment remain = new CurveSegment(EasingType.Linear, SnapWindowStart, 1f, 0f);
        public CurveSegment goback = new CurveSegment(EasingType.CircIn, SnapWindowEnd, 1f, -1f);
        internal float ThrowRatio() => PiecewiseAnimation(ThrowCompletion, new CurveSegment[] { shoot, remain, goback });


        public CurveSegment sizeCurve = new CurveSegment(EasingType.SineBump, 0f, 0f, 1f);
        internal float ThrowScaleRatio() => PiecewiseAnimation(ThrowCompletion, new CurveSegment[] { sizeCurve });


        public override void AI()
        {
            if (!initialized) //Initialization
            {
                projectile.timeLeft = Thrown ? (int) MaxThrowTime : (int)MaxSwingTime;
                var sound = Main.PlaySound((Charge > 0 || Thrown) ? SoundID.DD2_PhantomPhoenixShot : SoundID.Item71, projectile.Center);
                if (Charge > 0)
                    CalamityUtils.SafeVolumeChange(ref sound, 2.5f);
                direction = projectile.velocity;
                direction.Normalize();
                projectile.rotation = direction.ToRotation();

                if (Thrown)
                    ThrowReach = MathHelper.Clamp((Owner.Center - Owner.Calamity().mouseWorld).Length(), ThrowReachMin, ThrowReachMax);

                initialized = true;
                projectile.netUpdate = true;
                projectile.netSpam = 0;
            }

            if (!Thrown)
            {
                //Manage position and rotation
                projectile.Center = Owner.Center + DistanceFromPlayer;

                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Lerp(SwingWidth / 2 * SwingDirection, -SwingWidth / 2 * SwingDirection, SwingRatio()) - (Combo == 1 ? MathHelper.PiOver4 : 0f);

                projectile.scale = 1.2f + ((float)Math.Sin(SwingRatio() * MathHelper.Pi) * 0.6f) + (Charge / 10f) * 0.2f;
            }

            else
            {
                Vector2 sparklePosition = projectile.Center + projectile.rotation.ToRotationVector2() * 90 * projectile.scale + (projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * 20 * projectile.scale;
                Particle sparkle = new CritSpark(sparklePosition, projectile.rotation.ToRotationVector2() * 7f, Color.White, Color.OrangeRed, Main.rand.NextFloat(1f, 2f), 10 + Main.rand.Next(10), 0.1f, 3f, Main.rand.NextFloat(0f, 0.01f));
                GeneralParticleHandler.SpawnParticle(sparkle);


                //Telegraph the start of the snap window with a bit of leeway
                if (Math.Abs(ThrowCompletion - SnapWindowStart + 0.1f) <= 0.005f && ChanceMissed == 0f && Main.myPlayer == Owner.whoAmI)
                {
                    Particle pulse = new PulseRing(projectile.Center, Vector2.Zero, Color.OrangeRed, 0.05f, 1.8f, 8);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    Main.PlaySound(SoundID.Item4);
                }

                projectile.Center = Owner.Center + direction * ThrowRatio() * ThrowReach;
                projectile.rotation -= MathHelper.PiOver4 * 0.3f;
                projectile.scale = 1f + ThrowScaleRatio() * 0.5f;


                //Snip
                if (!OwnerCanShoot && Combo == 2 && ThrowCompletion >= (SnapWindowStart - 0.1f) && ThrowCompletion < SnapWindowEnd && ChanceMissed == 0f)
                {
                    Particle snapSpark = new GenericSparkle(projectile.Center, Owner.velocity - Utils.SafeNormalize(projectile.velocity, Vector2.Zero), Color.White, Color.OrangeRed, Main.rand.NextFloat(1f, 2f), 10 + Main.rand.Next(10), 0.1f, 3f);
                    GeneralParticleHandler.SpawnParticle(snapSpark);

                    if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;

                    if (Owner.whoAmI == Main.myPlayer)
                    {
                        //Reset local immunity so that the snap can do damage
                        for (int i = 0; i < Main.maxNPCs; ++i)
                            projectile.localNPCImmunity[i] = 0;
                    }

                    Combo = 3f; //Mark the end of the regular throw
                    projectile.velocity = projectile.rotation.ToRotationVector2();
                    projectile.timeLeft = (int)SnapEndTime;
                    projectile.localNPCHitCooldown = (int)SnapEndTime; //Only snap the enemies ONCE
                }

                else if (!OwnerCanShoot && Combo == 2 && ChanceMissed == 0f)
                    ChanceMissed = 1f;

                if (Combo == 3f)
                {
                    //Slow down the projectile's retraction
                    float curveDownGently = MathHelper.Lerp(1f, 0.8f, 1f - (float)Math.Sqrt(1f - (float)Math.Pow(SnapEndCompletion , 2f)));
                    projectile.Center = Owner.Center + direction * ThrowReach * curveDownGently;
                    projectile.scale = 1.5f;

                    float orientateProperly = (float)Math.Sqrt(1f - (float)Math.Pow(MathHelper.Clamp(SnapEndCompletion + 0.2f, 0f, 1f) - 1f, 2f));

                    float extraRotations = (direction.ToRotation() + MathHelper.PiOver4 > projectile.velocity.ToRotation()) ? -MathHelper.TwoPi : 0f;

                    projectile.rotation = MathHelper.Lerp(projectile.velocity.ToRotation(), direction.ToRotation() + MathHelper.PiOver4 * 0.2f + extraRotations, orientateProperly);

                }
            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            Owner.direction = Math.Sign(projectile.velocity.X);
            Owner.itemRotation = projectile.rotation;
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= MathHelper.Pi;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);

        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Combo == 3f)
                damage = (int)(damage * ArkoftheElements.snapDamageMultiplier);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 particleSpeed = Utils.SafeNormalize(target.Center - projectile.Center , Vector2.One).RotatedByRandom(MathHelper.PiOver4 * 0.8f) * Main.rand.NextFloat(3.6f, 8f);
                Particle energyLeak = new SquishyLightParticle(target.Center, particleSpeed, Main.rand.NextFloat(0.3f, 0.6f), Color.OrangeRed, 60, 2, 2.5f, hueShift: 0.06f);
                GeneralParticleHandler.SpawnParticle(energyLeak);
            }

            if (Combo == 3f)
            {
                var snapSound = Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ScissorGuillotineSnap"), projectile.Center);
                SafeVolumeChange(ref snapSound, 1.3f);

                if (Charge <= 1)
                {
                    ArkoftheElements sword = (Owner.HeldItem.modItem as ArkoftheElements);
                    if (sword != null)
                        sword.Charge = 2f;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Combo == 3f)
            {
                if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                    Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;

                Main.PlaySound(SoundID.Item84, projectile.Center);

                Vector2 sliceDirection = direction * 40;
                Particle SliceLine = new LineVFX(projectile.Center - sliceDirection, sliceDirection * 2f, 0.2f, Color.Orange * 0.7f, expansion : 250f)
                {
                    Lifetime = 10
                };
                GeneralParticleHandler.SpawnParticle(SliceLine);

            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (!Thrown)
            {
                if (Charge > 0)
                    DrawSwungScissors(spriteBatch, lightColor);
                else
                    DrawSingleSwungScissorBlade(spriteBatch, lightColor);
            }
            else
            {
                if (Charge > 0)
                    DrawThrownScissors(spriteBatch, lightColor);
                else
                    DrawSingleThrownScissorBlade(spriteBatch, lightColor);
            }
            return false;
        }

        public void DrawSingleSwungScissorBlade(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D sword = GetTexture(Combo == 0 ? "CalamityMod/Projectiles/Melee/RendingScissorsRight" : "CalamityMod/Projectiles/Melee/RendingScissorsLeft");
            Texture2D glowmask = GetTexture(Combo == 0 ? "CalamityMod/Projectiles/Melee/RendingScissorsRightGlow" : "CalamityMod/Projectiles/Melee/RendingScissorsLeftGlow");

            bool flipped = Owner.direction < 0;
            SpriteEffects flip = flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float extraAngle = Owner.direction < 0 ? MathHelper.PiOver2 : 0f;

            float drawAngle = projectile.rotation;
            float angleShift = Combo == 0 ? MathHelper.PiOver4 : MathHelper.PiOver2;
            float drawRotation = projectile.rotation + angleShift + extraAngle;

            Vector2 drawOrigin = new Vector2(Combo == 1 ? sword.Width / 2f : flipped ? sword.Width : 0f, sword.Height);
            Vector2 drawOffset = Owner.Center + drawAngle.ToRotationVector2() * 10f - Main.screenPosition;

            if (CalamityConfig.Instance.Afterimages && SwingTimer > ProjectileID.Sets.TrailCacheLength[projectile.type])
            {
                for (int i = 0; i < projectile.oldRot.Length; ++i)
                {
                    Color color = Main.hslToRgb((i / (float)projectile.oldRot.Length) * 0.1f, 1, 0.6f + (Charge > 0 ? 0.3f : 0f));
                    float afterimageRotation = projectile.oldRot[i] + angleShift + extraAngle;
                    Main.spriteBatch.Draw(glowmask, drawOffset, null, color * 0.15f, afterimageRotation, drawOrigin, projectile.scale - 0.2f * ((i / (float)projectile.oldRot.Length)), flip, 0f);
                }
            }

            spriteBatch.Draw(sword, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, flip, 0f);
            spriteBatch.Draw(glowmask, drawOffset, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, projectile.scale, flip, 0f);

            if (SwingCompletion > 0.5f)
            {
                Texture2D smear = GetTexture("CalamityMod/Particles/TrientCircularSmear");

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                float opacity = (float)Math.Sin(SwingCompletion * MathHelper.Pi);
                float rotation = (-MathHelper.PiOver4 * 0.5f + MathHelper.PiOver4 * 0.5f * SwingCompletion + (Combo == 1f ? MathHelper.PiOver4 : 0)) * SwingDirection;
                Color smearColor = Main.hslToRgb(((SwingTimer - MaxSwingTime * 0.5f) / (MaxSwingTime * 0.5f)) * 0.15f + ((Combo == 1f) ? 0.85f : 0f), 1, 0.6f);

                spriteBatch.Draw(smear, Owner.Center - Main.screenPosition, null, smearColor * 0.5f * opacity, projectile.velocity.ToRotation() + MathHelper.Pi + rotation, smear.Size() / 2f, projectile.scale * 2.3f, SpriteEffects.None, 0);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        public void DrawSwungScissors(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D frontBlade = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsRight");
            Texture2D frontBladeGlow = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsRightGlow");
            Texture2D backBlade = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsLeft");
            Texture2D backBladeGlow = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsLeftGlow");

            //Front blade
            bool flipped = Owner.direction < 0;
            SpriteEffects flip = flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float extraAngle = flipped ? MathHelper.PiOver2 : 0f;
            float drawAngle = projectile.rotation;
            float angleShift = (Combo == 0 || !flipped) ? MathHelper.PiOver4 : MathHelper.PiOver2 * 1.5f;
            float drawRotation = projectile.rotation + angleShift + extraAngle;
            Vector2 drawOrigin = new Vector2(flipped ? frontBlade.Width : 0f, frontBlade.Height);
            Vector2 drawOffset = Owner.Center + drawAngle.ToRotationVector2() * 10f - Main.screenPosition;

            //Back blade
            float functionalDrawAngle = drawAngle + (Combo == 1f && flipped ? MathHelper.PiOver2 : 0f);
            Vector2 backScissorOrigin = new Vector2(flipped ? 11 : 20f, 109); //Lined up with the hole in the scissor blade
            Vector2 backScissorDrawPosition = Owner.Center + drawAngle.ToRotationVector2() * 10f + functionalDrawAngle.ToRotationVector2() * 70f * projectile.scale - Main.screenPosition; //Lined up with the hole in the front blade
            float backScissorRotation = drawRotation + (Combo == 1 ? (!flipped ? MathHelper.PiOver4 * 0.75f : MathHelper.PiOver4 * -0.75f) : 0f);

            if (CalamityConfig.Instance.Afterimages && SwingTimer > ProjectileID.Sets.TrailCacheLength[projectile.type])
            {
                for (int i = 0; i < projectile.oldRot.Length; ++i)
                {
                    Color color = Main.hslToRgb((i / (float)projectile.oldRot.Length) * 0.1f, 1, 0.6f + (Charge > 0 ? 0.3f : 0f));
                    float afterimageRotation = projectile.oldRot[i] + angleShift + extraAngle;
                    float afterimageBackRotation = afterimageRotation + (backScissorRotation - drawRotation);

                    spriteBatch.Draw(backBladeGlow, backScissorDrawPosition, null, color * 0.15f, afterimageBackRotation, backScissorOrigin, projectile.scale - 0.2f * ((i / (float)projectile.oldRot.Length)), flip, 0f);
                    spriteBatch.Draw(frontBladeGlow, drawOffset, null, color * 0.15f, afterimageRotation, drawOrigin, projectile.scale - 0.2f * ((i / (float)projectile.oldRot.Length)), flip, 0f);
                }
            }
            
            spriteBatch.Draw(backBlade, backScissorDrawPosition, null, lightColor, backScissorRotation, backScissorOrigin, projectile.scale, flip, 0f);
            spriteBatch.Draw(backBladeGlow, backScissorDrawPosition, null, Color.Lerp(lightColor, Color.White, 0.75f), backScissorRotation, backScissorOrigin, projectile.scale, flip, 0f);

            spriteBatch.Draw(frontBlade, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, flip, 0f);
            spriteBatch.Draw(frontBladeGlow, drawOffset, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, projectile.scale, flip, 0f);

            if (SwingCompletion > 0.5f)
            {
                Texture2D smear = GetTexture("CalamityMod/Particles/TrientCircularSmear");

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                float opacity = (float)Math.Sin(SwingCompletion * MathHelper.Pi);
                float rotation = (-MathHelper.PiOver4 * 0.5f + MathHelper.PiOver4 * 0.5f * SwingCompletion + (Combo == 1f ? MathHelper.PiOver4 : 0)) * SwingDirection;
                Color smearColor = Main.hslToRgb(((SwingTimer - MaxSwingTime * 0.5f) / (MaxSwingTime * 0.5f)) * 0.15f + ((Combo == 1f) ? 0.85f : 0f), 1, 0.6f);

                spriteBatch.Draw(smear, Owner.Center - Main.screenPosition, null, smearColor * 0.5f * opacity, projectile.velocity.ToRotation() + MathHelper.Pi + rotation, smear.Size() / 2f, projectile.scale * 2.3f, SpriteEffects.None, 0);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        public void DrawSingleThrownScissorBlade(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D sword = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsRight");
            Texture2D glowmask = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsRightGlow");


            if (Combo == 3f)
            {
                Texture2D thrownSword = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsLeft");
                Texture2D thrownGlowmask = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsLeftGlow");

                Vector2 drawPos2 = Vector2.SmoothStep(Owner.Center, projectile.Center, MathHelper.Clamp(SnapEndCompletion + 0.25f, 0f, 1f));
                Vector2 drawOrigin2 = new Vector2(22, 109); //Right on the hole
                float drawRotation2 = direction.ToRotation() + MathHelper.PiOver2;

                spriteBatch.Draw(thrownSword, drawPos2 - Main.screenPosition, null, lightColor, drawRotation2, drawOrigin2, projectile.scale, 0f, 0f);
                spriteBatch.Draw(thrownGlowmask, drawPos2 - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation2, drawOrigin2, projectile.scale, 0f, 0f);
            }


            Vector2 drawPos = projectile.Center;
            Vector2 drawOrigin = new Vector2(51, 86); //Right on the hole
            float drawRotation = projectile.rotation + MathHelper.PiOver4;

            spriteBatch.Draw(sword, drawPos - Main.screenPosition, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            spriteBatch.Draw(glowmask, drawPos - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            Texture2D smear = GetTexture("CalamityMod/Particles/TrientCircularSmear");

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            float opacity = Combo == 3f ? (float)Math.Sin(SnapEndCompletion * MathHelper.PiOver2 + MathHelper.PiOver2) : (float)Math.Sin(ThrowCompletion * MathHelper.Pi);
            float rotation = drawRotation + MathHelper.PiOver2 * 1.5f;
            Color smearColor = Main.hslToRgb(((SwingTimer - MaxSwingTime * 0.5f) / (MaxSwingTime * 0.5f)) * 0.15f, 1, 0.6f);

            spriteBatch.Draw(smear, projectile.Center - Main.screenPosition, null, smearColor * 0.5f * opacity, rotation, smear.Size() / 2f, projectile.scale * 1.4f, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public void DrawThrownScissors(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D frontBlade = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsRight");
            Texture2D frontBladeGlow = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsRightGlow");
            Texture2D backBlade = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsLeft");
            Texture2D backBladeGlow = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsLeftGlow");

            Vector2 drawPos = projectile.Center;
            Vector2 drawOrigin = new Vector2(51, 86); //Right on the hole
            float drawRotation = projectile.rotation + MathHelper.PiOver4;

            Vector2 drawOrigin2 = new Vector2(22, 109); //Right on the hole
            float drawRotation2 = projectile.rotation + MathHelper.Lerp(0f, -MathHelper.PiOver4 * 0.33f, MathHelper.Clamp(ThrowCompletion * 2f, 0f, 1f));

            if (Combo == 3f)
                drawRotation2 = projectile.rotation + MathHelper.Lerp(-MathHelper.PiOver4 * 0.33f, MathHelper.PiOver2 * 0.85f, MathHelper.Clamp(SnapEndCompletion + 0.5f, 0f, 1f));

            spriteBatch.Draw(backBlade, drawPos - Main.screenPosition, null, lightColor, drawRotation2, drawOrigin2, projectile.scale, 0f, 0f);
            spriteBatch.Draw(backBladeGlow, drawPos - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation2, drawOrigin2, projectile.scale, 0f, 0f);

            spriteBatch.Draw(frontBlade, drawPos - Main.screenPosition, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            spriteBatch.Draw(frontBladeGlow, drawPos - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, projectile.scale, 0f, 0f);


            //Smear stuff
            Texture2D smear = GetTexture("CalamityMod/Particles/TrientCircularSmear");

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            float opacity = Combo == 3f ? (float)Math.Sin(SnapEndCompletion * MathHelper.PiOver2 + MathHelper.PiOver2) : (float)Math.Sin(ThrowCompletion * MathHelper.Pi);
            float rotation = drawRotation + MathHelper.PiOver2 * 1.5f;
            Color smearColor = Main.hslToRgb(((SwingTimer - MaxSwingTime * 0.5f) / (MaxSwingTime * 0.5f)) * 0.15f, 1, 0.6f);

            spriteBatch.Draw(smear, projectile.Center - Main.screenPosition, null, smearColor * 0.5f * opacity, rotation, smear.Size() / 2f, projectile.scale * 1.4f, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
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