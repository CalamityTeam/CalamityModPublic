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
    public class ArkoftheCosmosBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/RendingScissorsRight"; //Umm actually the rending scissors are for aote mr programmer what the hel.. it gets changed in predraw anywyas

        private bool initialized = false;
        public ref float Charge => ref Projectile.ai[0];

        public bool Dashing
        {
            get
            {
                return Projectile.ai[1] == 1;
            }

            set
            {
                Projectile.ai[1] = value ? 1f : 0f;
            }
        }

        const int maxStitches = 8;
        public int CurrentStitches => (int)Math.Ceiling((1 - (float)Math.Sqrt(1f - (float)Math.Pow(MathHelper.Clamp(StitchProgress * 3f, 0f, 1f), 2f))) * maxStitches);
        public float[] StitchRotations = new float[maxStitches];
        public float[] StitchLifetimes = new float[maxStitches];

        const float MaxTime = 70;
        const float SnapTime = 25f;
        const float HoldTime = 15f;

        public float SnapTimer => MaxTime - Projectile.timeLeft;
        public float HoldTimer => MaxTime - Projectile.timeLeft - SnapTime;
        public float StitchTimer => MaxTime - Projectile.timeLeft - SnapTime - (HoldTime / 2f);

        public float SnapProgress => MathHelper.Clamp(SnapTimer / SnapTime, 0, 1);
        public float HoldProgress => MathHelper.Clamp(HoldTimer / HoldTime, 0, 1);
        public float StitchProgress => MathHelper.Clamp(StitchTimer / (MaxTime - (SnapTime + (HoldTime / 2f))), 0, 1);

        public int CurrentAnimation => (MaxTime - Projectile.timeLeft) <= SnapTime ? 0 : (MaxTime - Projectile.timeLeft) <= SnapTime + HoldTime ? 1 : 2;

        public Vector2 scissorPosition => Projectile.Center + ThrustDisplaceRatio() * Projectile.velocity * 200f;

        public Player Owner => Main.player[Projectile.owner];

        public Particle PolarStar;


        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.width = Projectile.height = 300;
            Projectile.width = Projectile.height = 300;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = (int)MaxTime + 2;
        }

        public override bool? CanDamage()
        {
            return HoldProgress > 0;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (HoldProgress == 0)
                return false;

            //The hitbox is simplified into a line collision.
            float collisionPoint = 0f;
            float bladeLength = ThrustDisplaceRatio() * 242f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (Projectile.velocity * bladeLength), 30, ref collisionPoint);
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (!initialized) //Initialization
            {
                Projectile.timeLeft = (int)MaxTime;
                var sound = SoundEngine.PlaySound(SoundID.Item84 with { Volume = SoundID.Item84.Volume * 0.3f }, Projectile.Center);

                Projectile.velocity.Normalize();
                Projectile.rotation = Projectile.velocity.ToRotation();

                initialized = true;
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            //Manage position and rotation
            Projectile.scale = 1.4f;

            HandleParticles();

            //Spawn particles when the line appears
            if (HoldTimer == 1)
            {
                //Check for the up arrow once more so if the player has good reflexes they can avoid the dash. Gives them more control
                if (Owner.controlUp && Charge >= 5)
                {
                    Owner.GiveIFrames(ArkoftheCosmos.DashIframes);
                    Dashing = true;

                }

                for (int i = 0; i < 20; i++)
                {
                    float positionAlongLine = MathHelper.Lerp(0f, ThrustDisplaceRatio() * 242f, Main.rand.NextFloat(0f, 1f));
                    Vector2 particlePosition = Projectile.Center + Projectile.velocity * positionAlongLine;
                    Color particleColor = Main.rand.NextBool() ? Color.OrangeRed : Main.rand.NextBool() ? Color.White : Color.Orange;
                    float particleScale = Main.rand.NextFloat(0.05f, 0.4f) * (0.4f + 0.6f * (float)Math.Sin(positionAlongLine / (ThrustDisplaceRatio() * 242f) * MathHelper.Pi));

                    int particleType = Main.rand.Next(3);
                    Particle particle;

                    switch (particleType)
                    {
                        case 0:
                            particle = new StrongBloom(particlePosition, Vector2.UnitY * Main.rand.NextFloat(-4f, -1f), particleColor, particleScale, Main.rand.Next(20) + 10);
                            GeneralParticleHandler.SpawnParticle(particle);
                            break;
                        case 1:
                            particle = new GenericBloom(particlePosition, Vector2.UnitY * Main.rand.NextFloat(-4f, -1f), particleColor, particleScale, Main.rand.Next(20) + 10);
                            GeneralParticleHandler.SpawnParticle(particle);
                            break;
                        case 2:
                            particle = new CritSpark(particlePosition, Vector2.UnitY * Main.rand.NextFloat(-10f, -1f), Color.White, particleColor, particleScale * 7f, Main.rand.Next(20) + 10, 0.1f, 3);
                            GeneralParticleHandler.SpawnParticle(particle);
                            break;
                    }
                }
            }


            Owner.Calamity().LungingDown = false;

            if (Dashing)
            {
                Owner.Calamity().LungingDown = true;
                Owner.fallStart = (int)(Owner.position.Y / 16f);
                Owner.velocity = Owner.SafeDirectionTo(scissorPosition, Vector2.Zero) * 60f;

                if (Owner.Distance(scissorPosition) < 60f)
                {
                    Dashing = false;
                    Owner.velocity *= 0.1f; //Abrupt stop
                    SoundEngine.PlaySound(CommonCalamitySounds.MeatySlashSound, Projectile.Center);

                    if (Owner.whoAmI == Main.myPlayer)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            var source = Projectile.GetSource_FromThis();
                            Projectile blast = Projectile.NewProjectileDirect(source, Owner.Center, Main.rand.NextVector2CircularEdge(28, 28), ProjectileType<EonBolt>(), (int)(ArkoftheCosmos.SlashBoltsDamageMultiplier * Projectile.damage), 0f, Owner.whoAmI, 0.55f, MathHelper.Pi * 0.07f);
                            {
                                blast.timeLeft = 100;
                            }
                        }
                    }
                }
            }
        }

        public void HandleParticles()
        {
            if (PolarStar == null)
            {
                PolarStar = new GenericSparkle(Projectile.Center, Vector2.Zero, Color.White, Color.CornflowerBlue, Projectile.scale * 2f, 2, 0.1f, 5f, true);
                GeneralParticleHandler.SpawnParticle(PolarStar);
            }
            else if (HoldProgress <= 0.4f)
            {
                PolarStar.Time = 0;
                PolarStar.Position = scissorPosition;
                PolarStar.Scale = Projectile.scale * 2f;
            }

            //Update stitches
            for (int i = 0; i < CurrentStitches; i++)
            {
                if (StitchRotations[i] == 0)
                {
                    StitchRotations[i] = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) + MathHelper.PiOver2;

                    SoundStyle sewSound = i % 3 == 0 ? SoundID.Item63 : i % 3 == 1 ? SoundID.Item64 : SoundID.Item65;
                    SoundEngine.PlaySound(sewSound with { Volume = sewSound.Volume * 0.5f }, Owner.Center);

                    float positionAlongLine = (ThrustDisplaceRatio() * 242f / (float)maxStitches * 0.5f) + MathHelper.Lerp(0f, ThrustDisplaceRatio() * 242f, i / (float)maxStitches);
                    Vector2 stitchCenter = Projectile.Center + Projectile.velocity * positionAlongLine;


                    Particle spark = new CritSpark(stitchCenter, Vector2.Zero, Color.White, Color.Cyan, 3f, 8, 0.1f, 3);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                StitchLifetimes[i]++;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Color pulseColor = Main.rand.NextBool() ? (Main.rand.NextBool() ? Color.Orange : Color.Coral) : (Main.rand.NextBool() ? Color.OrangeRed : Color.Gold);
            Particle pulse = new PulseRing(target.Center, Vector2.Zero, pulseColor, 0.05f, 0.2f + Main.rand.NextFloat(0f, 1f), 30);
            GeneralParticleHandler.SpawnParticle(pulse);

            for (int i = 0; i < 10; i++)
            {
                Vector2 particleSpeed = Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.8f) * Main.rand.NextFloat(2.6f, 4f);
                Particle energyLeak = new SquishyLightParticle(target.Center, particleSpeed, Main.rand.NextFloat(0.3f, 0.6f), Color.Red, 60, 1, 1.5f, hueShift: 0.002f);
                GeneralParticleHandler.SpawnParticle(energyLeak);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //Add some damage falloff
            modifiers.SourceDamage *= (float)Math.Pow(1 - ArkoftheCosmos.blastFalloffStrenght, Projectile.numHits * ArkoftheCosmos.blastFalloffSpeed);
        }

        public override void OnKill(int timeLeft)
        {
            if (Dashing)
            {
                Owner.velocity *= 0.1f; //Abrupt stop if the player was still dashing
            }

            Owner.Calamity().LungingDown = false;
        }

        //Animation keys
        public CurveSegment anticipation = new CurveSegment(EasingType.SineBump, 0f, 0.2f, -0.1f);
        public CurveSegment thrust = new CurveSegment(EasingType.PolyOut, 0.3f, 0.2f, 3f, 3);
        internal float ThrustDisplaceRatio() => PiecewiseAnimation(SnapProgress, new CurveSegment[] { anticipation, thrust });


        public CurveSegment openMore = new CurveSegment(EasingType.SineBump, 0f, 0f, -0.15f);
        public CurveSegment close = new CurveSegment(EasingType.PolyIn, 0.35f, 0f, 1f, 4);
        public CurveSegment stayClosed = new CurveSegment(EasingType.Linear, 0.5f, 1f, 0f);
        internal float RotationRatio() => PiecewiseAnimation(SnapProgress, new CurveSegment[] { openMore, close, stayClosed });

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D sliceTex = Request<Texture2D>("CalamityMod/Particles/BloomLine").Value;
            Color sliceColor = Color.Lerp(Color.OrangeRed, Color.White, SnapProgress);
            float rot = Projectile.rotation + MathHelper.PiOver2;
            Vector2 sliceScale = new Vector2(0.2f * (1 - SnapProgress) ,ThrustDisplaceRatio() * 242f);
            Main.EntitySpriteDraw(sliceTex, Projectile.Center - Main.screenPosition, null, sliceColor, rot, new Vector2(sliceTex.Width / 2f, sliceTex.Height), sliceScale, 0f, 0);

            //Draw the scissors
            if (HoldProgress <= 0.4f)
            {
                Texture2D frontBlade = Request<Texture2D>("CalamityMod/Projectiles/Melee/SunderingScissorsLeft").Value;
                Texture2D backBlade = Request<Texture2D>("CalamityMod/Projectiles/Melee/SunderingScissorsRight").Value;

                float snippingRotation = Projectile.rotation + MathHelper.PiOver4;
                float drawRotation = MathHelper.Lerp(snippingRotation - MathHelper.PiOver4, snippingRotation, RotationRatio());
                float drawRotationBack = MathHelper.Lerp(snippingRotation + MathHelper.PiOver4, snippingRotation, RotationRatio());

                Vector2 drawOrigin = new Vector2(33, 86); //Right on the hole
                Vector2 drawOriginBack = new Vector2(44f, 86); //Right on the hole
                Vector2 drawPosition = scissorPosition - Main.screenPosition;

                float opacity = (0.4f - HoldProgress) / 0.4f;
                Color drawColor = Color.Tomato * opacity * 0.9f;
                Color drawColorBack = Color.DeepSkyBlue * opacity * 0.9f;

                Main.EntitySpriteDraw(backBlade, drawPosition, null, drawColorBack, drawRotationBack, drawOriginBack, Projectile.scale, 0f, 0);
                Main.EntitySpriteDraw(frontBlade, drawPosition, null, drawColor * opacity, drawRotation, drawOrigin, Projectile.scale, 0f, 0);
            }

            //Draw the rip
            if (HoldProgress > 0)
            {
                Texture2D lineTex = Request<Texture2D>("CalamityMod/Particles/ThinEndedLine").Value;

                Vector2 Shake = HoldProgress > 0.2f ? Vector2.Zero : Vector2.One.RotatedByRandom(MathHelper.TwoPi) * (1 - HoldProgress * 5f) * 0.5f;
                float raise = (float)Math.Sin(HoldProgress * MathHelper.PiOver2);

                Vector2 origin = new Vector2(lineTex.Width / 2f, lineTex.Height);
                float ripWidth = StitchProgress < 0.75f ? 0.2f : (1 - (StitchProgress - 0.75f) * 4f) * 0.2f;
                Vector2 scale = new Vector2(ripWidth, (ThrustDisplaceRatio() * 242f) / lineTex.Height);
                float lineOpacity = StitchProgress < 0.75f ? 1f : 1 - (StitchProgress - 0.75f) * 4f;

                Main.EntitySpriteDraw(lineTex, Projectile.Center - Main.screenPosition + Shake, null, Color.Lerp(Color.White, Color.OrangeRed * 0.7f, raise) * lineOpacity, rot, origin, scale, SpriteEffects.None, 0);

                //Draw the stitches
                if (StitchProgress > 0)
                {
                    for (int i = 0; i < CurrentStitches; i++)
                    {
                        float positionAlongLine = (ThrustDisplaceRatio() * 242f / (float)maxStitches * 0.5f) + MathHelper.Lerp(0f, ThrustDisplaceRatio() * 242f, i / (float)maxStitches);
                        Vector2 stitchCenter = Projectile.Center + Projectile.velocity * positionAlongLine;

                        rot = Projectile.rotation + MathHelper.PiOver2 + StitchRotations[i];
                        origin = new Vector2(lineTex.Width / 2f, lineTex.Height / 2f);

                        float stitchLength = (float)Math.Sin(i / (float)(maxStitches - 1) * MathHelper.Pi) * 0.5f + 0.5f;
                        float stitchScale = (1f + (float)Math.Sin(MathHelper.Clamp(StitchLifetimes[i] / 7f, 0f, 1f) * MathHelper.Pi) * 0.3f) * 0.85f;
                        if (CurrentStitches == maxStitches)
                        {
                            stitchScale *= 1 - ((StitchTimer - (MaxTime - SnapTime - HoldTime * 0.5f) * 0.3f) / (MaxTime - SnapTime - HoldTime * 0.5f) * 0.7f) * 0.8f;
                        }
                        scale = new Vector2(0.2f, stitchLength) * stitchScale;

                        Color stitchColor = Color.Lerp(Color.White, Color.CornflowerBlue * 0.7f, (float)Math.Sin(MathHelper.Clamp(StitchLifetimes[i] / 7f, 0f, 1f) * MathHelper.PiOver2));

                        Main.EntitySpriteDraw(lineTex, stitchCenter - Main.screenPosition + Shake, null, stitchColor, rot, origin, scale, SpriteEffects.None, 0);
                    }
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
        }
    }
}
