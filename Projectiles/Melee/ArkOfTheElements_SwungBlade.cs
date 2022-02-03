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
        const float ThrowReach = 500;
        public float ThrowTimer => MaxThrowTime - projectile.timeLeft;
        public float ThrowCompletion => ThrowTimer / MaxThrowTime;

        const float SnapWindowStart = 0.4f;
        const float SnapWindowEnd = 0.6f;
        public float SnapEndTime => (MaxThrowTime - (MaxThrowTime * SnapWindowEnd)) /2f;
        public float SnapEndCompletion => (SnapEndTime - projectile.timeLeft) / SnapEndTime;

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
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float bladeLenght = 160f * projectile.scale;

            if (Thrown)
            {
                return Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - Vector2.One * bladeLenght  / 2f, Vector2.One * bladeLenght);
            }

            float collisionPoint = 0f;
            Vector2 holdPoint = DistanceFromPlayer.Length() * projectile.rotation.ToRotationVector2();

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + holdPoint, Owner.Center + holdPoint + projectile.rotation.ToRotationVector2() * bladeLenght, 24, ref collisionPoint);
        }

        //Swing animation keys
        public CurveSegment anticipation = new CurveSegment(EasingType.ExpOut, 0f, 0f, 0.15f);
        public CurveSegment thrust = new CurveSegment(EasingType.PolyInOut, 0.1f, 0.15f, 0.85f, 3);
        public CurveSegment hold = new CurveSegment(EasingType.Linear, 0.5f, 1f, 0.2f);
        public CurveSegment retract = new CurveSegment(EasingType.PolyInOut, 0.7f, 0.9f, -0.9f, 3);
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
                    sound.Volume *= 2.5f;
                direction = projectile.velocity;
                direction.Normalize();
                projectile.rotation = direction.ToRotation();

                initialized = true;
                projectile.netUpdate = true;
                projectile.netSpam = 0;
            }

            if (!Thrown)
            {
                //Manage position and rotation
                projectile.Center = Owner.Center + DistanceFromPlayer;

                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Lerp(SwingWidth / 2 * SwingDirection, -SwingWidth / 2 * SwingDirection, SwingRatio()) - (Combo == 1 ? MathHelper.PiOver4 : 0f);

                projectile.scale = 1.2f + ((float)Math.Sin(SwingRatio() * MathHelper.Pi) * 0.6f) + (Charge / 10f) * 0.6f;

                if (Owner.whoAmI == Main.myPlayer && SwingRatio() > 0.5f && HasFired == 0f && Charge > 0)
                {
                    Projectile.NewProjectile(Owner.Center + direction * 30f, projectile.velocity * 2f, ProjectileType<TrueAncientBeam>(), projectile.damage, 2f, Owner.whoAmI);
                    HasFired = 1f;
                }


                
                if (Charge > 0 && Main.rand.Next(2) == 0)
                {
                    Vector2 particleOrigin = projectile.Center + projectile.rotation.ToRotationVector2() * 75 * projectile.scale;
                    Vector2 particleSpeed = projectile.rotation.ToRotationVector2().RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1.2f, 2f);
                    Particle energyLeak = new CritSpark(particleOrigin, particleSpeed + Owner.velocity, Color.White, Color.Cyan, Main.rand.NextFloat(0.6f, 1.6f), 20 + Main.rand.Next(10), 0.1f, 1.5f, hueShift: 0.02f);
                    GeneralParticleHandler.SpawnParticle(energyLeak);
                }
            }

            else
            {
                projectile.Center = Owner.Center + direction * ThrowRatio() * ThrowReach;
                projectile.rotation -= MathHelper.PiOver4 * 0.3f;
                projectile.scale = 1f + ThrowScaleRatio() * 0.5f;


                //Snip
                if (!OwnerCanShoot && Combo == 2 && ThrowCompletion >= SnapWindowStart && ThrowCompletion < SnapWindowEnd)
                {
                    if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;
                    Combo = 3f; //Mark the end of the regular throw
                    projectile.velocity = projectile.rotation.ToRotationVector2();
                    projectile.timeLeft = (int)SnapEndTime;
                }

                if (Combo == 3f)
                {
                    //Slow down the projectile's retraction
                    float curveDownGently = MathHelper.Lerp(1f, 0.8f, 1f - (float)Math.Sqrt(1f - (float)Math.Pow(SnapEndCompletion , 2f)));
                    projectile.Center = Owner.Center + direction * ThrowReach * curveDownGently;
                    projectile.scale = 1.5f;

                    float orientateProperly = (float)Math.Sqrt(1f - (float)Math.Pow(MathHelper.Clamp(SnapEndCompletion + 0.2f, 0f, 1f) - 1f, 2f));

                    float extraRotations = (direction.ToRotation() + MathHelper.PiOver4 > projectile.velocity.ToRotation()) ? -MathHelper.TwoPi : 0f;

                    projectile.rotation = MathHelper.Lerp(projectile.velocity.ToRotation(), direction.ToRotation() + MathHelper.PiOver4 * 0.4f + extraRotations, orientateProperly);

                }
            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = projectile.whoAmI;
            Owner.direction = Math.Sign(projectile.velocity.X);
            Owner.itemRotation = projectile.rotation;
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= 3.14f;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);

        }

        public override void Kill(int timeLeft)
        {
            if (Combo == 3f)
            {
                Main.PlaySound(SoundID.Item84, projectile.Center);

                Vector2 sliceDirection = direction.RotatedBy(MathHelper.ToRadians(5)) * 140;
                Particle SliceLine = new LineVFX(projectile.Center - sliceDirection, sliceDirection * 2f, 0.2f, Color.LightCoral * 0.7f)
                {
                    Lifetime = 6
                };
                GeneralParticleHandler.SpawnParticle(SliceLine);

            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (!Thrown)
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
                        //Color color = projectile.GetAlpha(lightColor) * (1f - (i / (float)projectile.oldRot.Length));
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
                    Color smearColor = Main.hslToRgb(((SwingTimer - MaxSwingTime * 0.5f) / (MaxSwingTime * 0.5f)) * 0.15f, 1, 0.6f);

                    spriteBatch.Draw(smear, Owner.Center - Main.screenPosition, null, smearColor * 0.5f * opacity, projectile.velocity.ToRotation() + MathHelper.Pi + rotation, smear.Size() / 2f, projectile.scale * 2.3f, SpriteEffects.None, 0);

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                }
            }

            else
            {
                Texture2D sword = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsRight");
                Texture2D glowmask = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsRightGlow");


                if (Combo == 3f)
                {
                    Texture2D thrownSword = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsLeft");
                    Texture2D thrownGlowmask = GetTexture("CalamityMod/Projectiles/Melee/RendingScissorsLeftGlow");

                    Vector2 drawPos2 = Vector2.SmoothStep(Owner.Center, projectile.Center, MathHelper.Clamp(SnapEndCompletion + 0.1f, 0f, 1f));
                    Vector2 drawOrigin2 = new Vector2(22, 99); //Right on the hole
                    float drawRotation2 = direction.ToRotation() + MathHelper.PiOver2;

                    spriteBatch.Draw(thrownSword, drawPos2 - Main.screenPosition, null, lightColor, drawRotation2, drawOrigin2, projectile.scale, 0f, 0f);
                    spriteBatch.Draw(thrownGlowmask, drawPos2 - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation2, drawOrigin2, projectile.scale, 0f, 0f);
                }


                Vector2 drawPos = projectile.Center;
                Vector2 drawOrigin = new Vector2(51, 86); //Right on the hole
                float drawRotation = projectile.rotation + MathHelper.PiOver4;

                spriteBatch.Draw(sword, drawPos - Main.screenPosition, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
                spriteBatch.Draw(glowmask, drawPos - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.75f), drawRotation, drawOrigin, projectile.scale, 0f, 0f);
            }


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
}