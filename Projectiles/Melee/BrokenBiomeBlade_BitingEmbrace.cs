using CalamityMod.Buffs.StatDebuffs;
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


namespace CalamityMod.Projectiles.Melee
{
    public class BitingEmbrace : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_BitingEmbraceSmall";

        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public float rotation;
        public ref float SwingMode => ref Projectile.ai[0]; //0 = Up-Down small slash, 1 = Down-Up large slash, 2 = Thrust
        public ref float MaxTime => ref Projectile.ai[1];
        public float Timer => MaxTime - Projectile.timeLeft;

        public int SwingDirection
        {
            get
            {
                switch (SwingMode)
                {
                    case 0:
                        return -1 * Math.Sign(direction.X);
                    case 1:
                        return 1 * Math.Sign(direction.X);
                    default:
                        return 0;

                }
            }
        }
        public float SwingWidth
        {
            get
            {
                switch (SwingMode)
                {
                    case 0:
                        return 2.3f;
                    default:
                        return 1.8f;
                }
            }
        }

        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Biting Embrace");
            Main.projFrames[Projectile.type] = 6; //The true trolling is that we only really use this for the third swing.
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 75;
            Projectile.width = Projectile.height = 75;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //The hitbox is simplified into a line collision.
            float collisionPoint = 0f;
            float bladeLenght = 0f;
            Vector2 displace = Vector2.Zero;
            switch (SwingMode)
            {
                case 0:
                    bladeLenght = 90f * Projectile.scale;
                    break;
                case 1:
                    bladeLenght = 110f * Projectile.scale;
                    break;
                case 2:
                    bladeLenght = Projectile.frame <= 2 ? 85f : 150f; //Only use the extended hitbox after the blade actually extends. For realism.
                    bladeLenght *= Projectile.scale;
                    displace = direction * ThrustDisplaceRatio() * 60f;
                    break;

            }
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center + displace, Owner.Center + displace + (rotation.ToRotationVector2() * bladeLenght), 24, ref collisionPoint);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            if (SwingMode == 2)
                target.AddBuff(BuffType<GlacialState>(), 20);
        }

        public override void AI()
        {
            if (!initialized) //Initialization
            {
                Projectile.timeLeft = (int)MaxTime;
                switch (SwingMode)
                {
                    case 0:
                        Projectile.width = Projectile.height = 100;
                        SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, Projectile.Center);
                        break;
                    case 1:
                        Projectile.width = Projectile.height = 140;
                        SoundEngine.PlaySound(SoundID.DD2_OgreSpit, Projectile.Center);
                        Projectile.damage = (int)(Projectile.damage * BiomeBlade.ColdAttunement_SecondSwingBoost);
                        break;
                    case 2:
                        Projectile.width = Projectile.height = 130;
                        SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Projectile.Center);
                        Projectile.damage = (int)(Projectile.damage * BiomeBlade.ColdAttunement_ThirdSwingBoost);
                        break;
                }

                //Take the direction the sword is swung. FUCK not controlling the swing direction more than just left/right :|
                direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                direction.Normalize();
                Projectile.rotation = direction.ToRotation();

                initialized = true;
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            //Manage position and rotation
            Projectile.Center = Owner.Center + (direction * 30);
            //rotation = projectile.rotation + MathHelper.SmoothStep(SwingWidth / 2 * SwingDirection, -SwingWidth / 2 * SwingDirection, Timer / MaxTime);
            float factor = 1 - (float)Math.Pow((double)-(Timer / MaxTime) + 1, 2d);
            rotation = Projectile.rotation + MathHelper.Lerp(SwingWidth / 2 * SwingDirection, -SwingWidth / 2 * SwingDirection, factor);
            Projectile.scale = 1f + ((float)Math.Sin(Timer / MaxTime * MathHelper.Pi) * 0.6f); //SWAGGER

            Lighting.AddLight(Owner.MountedCenter, new Vector3(0.75f, 1f, 1f) * (float)Math.Sin(Timer / MaxTime * MathHelper.Pi));

            //Add the thrust motion & animation for the third combo state
            if (SwingMode == 2)
            {
                Projectile.scale = 1f + (ThrustScaleRatio() * 0.6f);
                Projectile.Center = Owner.Center + (direction * ThrustDisplaceRatio() * 60);

                Projectile.frameCounter++;
                if (Projectile.frameCounter % 5 == 0 && Projectile.frame + 1 < Main.projFrames[Projectile.type])
                    Projectile.frame++;

                if (Main.rand.NextBool())
                {
                    Particle mist = new MediumMistParticle(Owner.Center + direction * 40 + Main.rand.NextVector2Circular(30f, 30f), Vector2.Zero, new Color(172, 238, 255), new Color(145, 170, 188), Main.rand.NextFloat(0.5f, 1.5f), 245 - Main.rand.Next(50), 0.02f);
                    mist.Velocity = (mist.Position - Owner.Center) * 0.2f + Owner.velocity;
                    GeneralParticleHandler.SpawnParticle(mist);
                }

            }

            else if (Main.rand.NextFloat(0f, 1f) > 0.75f)
            {
                Vector2 particlePosition = Owner.Center + (rotation.ToRotationVector2() * 100f * Projectile.scale);
                Particle snowflake = new SnowflakeSparkle(particlePosition, rotation.ToRotationVector2() * 3f, Color.White, new Color(75, 177, 250), Main.rand.NextFloat(0.3f, 1.5f), 40, 0.5f);
                GeneralParticleHandler.SpawnParticle(snowflake);
            }

            //Make the owner look like theyre holding the sword bla bla
            Owner.heldProj = Projectile.whoAmI;
            Owner.direction = Math.Sign(rotation.ToRotationVector2().X);
            Owner.itemRotation = rotation;
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= MathHelper.Pi;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
        }

        //Animation keys
        public CurveSegment anticipation = new CurveSegment(EasingType.SineBump, 0f, 0f, -0.15f);
        public CurveSegment thrust = new CurveSegment(EasingType.PolyInOut, 0.2f, 0f, 0.9f, 3);
        public CurveSegment hold = new CurveSegment(EasingType.SineBump, 0.35f, 0.9f, 0.1f);
        public CurveSegment retract = new CurveSegment(EasingType.PolyInOut, 0.7f, 0.9f, -0.9f, 3);
        internal float ThrustDisplaceRatio() => PiecewiseAnimation(Timer / MaxTime, new CurveSegment[] { anticipation, thrust, hold, retract });

        //Animation keys
        public CurveSegment expandSize = new CurveSegment(EasingType.ExpIn, 0f, 0f, 1f);
        public CurveSegment holdSize = new CurveSegment(EasingType.Linear, 0.1f, 1f, 0f);
        public CurveSegment shrinkSize = new CurveSegment(EasingType.ExpIn, 0.85f, 1f, -1f);
        internal float ThrustScaleRatio() => PiecewiseAnimation(Timer / MaxTime, new CurveSegment[] { expandSize, holdSize, shrinkSize });

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D handle = GetTexture("CalamityMod/Items/Weapons/Melee/BiomeBlade");

            if (SwingMode != 2)
            {
                Texture2D blade = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_BitingEmbrace" + (SwingMode == 0 ? "Small" : "Big"));
                float drawAngle = rotation;
                float drawRotation = rotation + MathHelper.PiOver4;
                Vector2 drawOrigin = new Vector2(0f, handle.Height);
                Vector2 drawOffset = Owner.Center + drawAngle.ToRotationVector2() * 10f - Main.screenPosition;

                spriteBatch.Draw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, Projectile.scale, 0f, 0f);
                //Turn on additive blending
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                //Update the parameters
                drawOrigin = new Vector2(0f, blade.Height);
                drawOffset = Owner.Center + (drawAngle.ToRotationVector2() * 28f * Projectile.scale) - Main.screenPosition;
                spriteBatch.Draw(blade, drawOffset, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.8f, drawRotation, drawOrigin, Projectile.scale, 0f, 0f);
                //Back to normal
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            else
            {
                Texture2D blade = GetTexture("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_BitingEmbraceThrust");
                Vector2 thrustDisplace = direction * (ThrustDisplaceRatio() * 60);

                float drawAngle = rotation;
                float drawRotation = rotation + MathHelper.PiOver4;
                Vector2 drawOrigin = new Vector2(0f, handle.Height);
                Vector2 drawOffset = Owner.Center + drawAngle.ToRotationVector2() * 10f - Main.screenPosition;

                spriteBatch.Draw(handle, drawOffset + thrustDisplace, null, lightColor, drawRotation, drawOrigin, Projectile.scale, 0f, 0f);
                //Turn on additive blending
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                //Update the parameters

                drawOrigin = new Vector2(0f, 114f);
                drawOffset = Owner.Center + (direction * 28f * Projectile.scale) - Main.screenPosition;
                //Anim stuff
                Rectangle frameRectangle = new Rectangle(0, 0 + (116 * Projectile.frame), 114, 114);

                spriteBatch.Draw(blade, drawOffset + thrustDisplace, frameRectangle, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, Projectile.scale, 0f, 0f);
                //Back to normal
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
            writer.WriteVector2(direction);
            writer.Write(rotation);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            direction = reader.ReadVector2();
            rotation = reader.ReadSingle();
        }
    }
}
