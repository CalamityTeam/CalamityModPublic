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
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles.Astral;

namespace CalamityMod.Projectiles.Melee
{
    public class ExtantAbhorrence : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_ExtantAbhorrence";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public Player Owner => Main.player[Projectile.owner];
        public ref float Charge => ref Projectile.ai[0]; //Charge
        public ref float State => ref Projectile.ai[1]; //State 0 is "charging", State 1 is "thrusting"
        public ref float CurrentIndicator => ref Projectile.localAI[0]; //What "indicator" stage are you on.
        public ref float OverCharge => ref Projectile.localAI[1];

        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;
        const float MaxCharge = 500;

        public Vector2 lastDisplacement;
        public float dashDuration;

        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 70;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
        }

        public override bool? CanDamage()
        {
            return (State == 1);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLength = 120 * Projectile.scale;
            float bladeWidth = 20 * Projectile.scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Owner.Center + (direction * bladeLength), bladeWidth, ref collisionPoint);
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
                Projectile.velocity = Vector2.Zero;
                SoundEngine.PlaySound(SoundID.Item101, Projectile.Center);
                initialized = true;
            }

            if (!OwnerCanShoot)
            {
                if (State == 0f)
                {
                    if (Charge / MaxCharge < 0.25f)
                    {
                        SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                        Projectile.Kill();
                        return;
                    }
                    else
                    {

                        SoundEngine.PlaySound(SoundID.Item120 with { Volume = SoundID.Item120.Volume * 0.5f }, Projectile.Center);
                        State = 1f;
                        Projectile.timeLeft = (7 + (int)((Charge / MaxCharge - 0.25f) * 20)) * 2; //Keep that even, if its an odd number itll fuck off and wont reset the players velocity on death
                        dashDuration = Projectile.timeLeft;
                        lastDisplacement = Projectile.Center - Owner.Center;
                        Projectile.netUpdate = true;
                        Projectile.netSpam = 0;
                    }
                }
            }

            if (State == 0f)
            {
                direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                direction.Normalize();
                Projectile.Center = Owner.Center + (direction * 70f * ChargeDisplacement());

                Charge++;
                OverCharge--;
                Projectile.timeLeft = 2;

                if ((Charge / MaxCharge >= 0.25f && CurrentIndicator == 0f) || (Charge / MaxCharge >= 0.5f && CurrentIndicator == 1f) || (Charge / MaxCharge >= 0.75f && CurrentIndicator == 2f))
                {
                    SoundEngine.PlaySound(DeusRitualDrama.PulseSound with { Pitch = DeusRitualDrama.PulseSound.Pitch - 0.2f + 0.1f * CurrentIndicator }, Projectile.Center);
                    CurrentIndicator++;
                    OverCharge = 20f;
                }

                if (Charge >= MaxCharge)
                {
                    Charge = MaxCharge;
                    if (Owner.whoAmI == Main.myPlayer && CurrentIndicator < 4f)
                    {
                        OverCharge = 20f;
                        SoundEngine.PlaySound(AstralBeacon.UseSound, Projectile.Center);
                        CurrentIndicator++;
                    }
                }
            }

            if (State == 1f)
            {
                Projectile.Center = Owner.Center + Vector2.Lerp(lastDisplacement, direction * 40f, MathHelper.Clamp(((dashDuration - Projectile.timeLeft) / dashDuration) * 2f, 0f, 1f));
                Owner.fallStart = (int)(Owner.position.Y / 16f);

                Owner.Calamity().LungingDown = true;

                if (Collision.SolidCollision(Owner.Center + (direction * 120 * Projectile.scale) - Vector2.One * 5f, 10, 10))
                {
                    SlamDown();
                    Projectile.timeLeft = 0;
                    Owner.Calamity().LungingDown = false;
                    Projectile.active = false;
                    Projectile.netUpdate = true;
                    Projectile.netSpam = 0;
                }

                Owner.velocity = direction * 30f;

                float variation = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
                float strength = (float)Math.Sin(variation * 2f + MathHelper.PiOver2);
                Particle Sparkle = new CritSpark(Projectile.Center, Owner.velocity - direction.RotatedBy(variation) * (1 + strength) * 2f * Main.rand.NextFloat(7.5f, 20f), Color.White, Main.rand.NextBool() ? Color.MediumTurquoise : Color.DarkOrange, 0.1f + Main.rand.NextFloat(0f, 1.5f), 20 + Main.rand.Next(30), 1, 3f);
                GeneralParticleHandler.SpawnParticle(Sparkle);
            }

            Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.56f, 0.56f) * Charge / MaxCharge);

            //Manage position and rotation
            Projectile.rotation = direction.ToRotation();

            //Scaling based on charge
            Projectile.scale = 1f + (Charge / MaxCharge * 0.3f);

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
            var slamSound = SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { Volume = SoundID.DD2_MonkStaffGroundImpact.Volume * 1.5f }, Projectile.Center);

            if (Owner.whoAmI != Main.myPlayer || Owner.velocity.Y == 0f)
                return;

            if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 5)
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = 5;

            //Only create the central monolith if over half charge
            if (Charge / MaxCharge < 0.5f)
                return;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center + (direction * 120 * Projectile.scale), -direction, ProjectileType<ExtantAbhorrenceMonolith>(), (int)(Projectile.damage * TrueBiomeBlade.AstralAttunement_MonolithDamageBoost), 10f, Owner.whoAmI, Main.rand.Next(4), 1f);

            //Only create the side monoliths if over 3/4th charge
            if (Charge / MaxCharge < 0.75f)
                return;

            if (SideSprouts(1, 150f, 0.8f) && Charge >= MaxCharge)
                SideSprouts(1, 250f, 0.5f);

            if (SideSprouts(-1, 150f, 0.8f) && Charge >= MaxCharge)
                SideSprouts(-1, 250f, 0.5f);

        }

        public bool SideSprouts(float facing, float distance, float projSize)
        {
            float widestAngle = 0f;
            float widestSurfaceAngle = 0f;
            bool validPositionFound = false;
            for (float i = 0f; i < 1; i += 1 / distance)
            {
                Vector2 positionToCheck = Owner.Center + (direction * 120 * Projectile.scale) + direction.RotatedBy((i * MathHelper.PiOver2 + MathHelper.PiOver4) * facing) * distance;

                if (Main.tile[(int)(positionToCheck.X / 16), (int)(positionToCheck.Y / 16)].IsTileSolid())
                    widestAngle = i;

                else if (widestAngle != 0)
                {
                    validPositionFound = true;
                    widestSurfaceAngle = widestAngle;
                }
            }

            if (validPositionFound)
            {
                Vector2 projPosition = Owner.Center + (direction * 120 * Projectile.scale) + direction.RotatedBy((widestSurfaceAngle * MathHelper.PiOver2 + MathHelper.PiOver4) * facing) * distance;
                Vector2 monolithRotation = direction.RotatedBy(Utils.AngleLerp(widestSurfaceAngle * -facing, 0f, projSize));
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), projPosition, -monolithRotation, ProjectileType<ExtantAbhorrenceMonolith>(), (int)(Projectile.damage * TrueBiomeBlade.AstralAttunement_MonolithDamageBoost), 10f, Owner.whoAmI, Main.rand.Next(4), projSize);
                if (proj.ModProjectile is ExtantAbhorrenceMonolith monolith)
                    monolith.WaitTimer = (1 - projSize) * 34f;
            }

            return validPositionFound;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Owner.GiveIFrames(TrueBiomeBlade.AstralAttunement_DashHitIFrames);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 1f + TrueBiomeBlade.AstralAttunement_FullChargeBoost * (float)Math.Pow(Charge / MaxCharge, 2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D handle = Request<Texture2D>("CalamityMod/Projectiles/Melee/MendedBiomeBlade").Value;
            Texture2D blade = Request<Texture2D>("CalamityMod/Projectiles/Melee/MendedBiomeBlade_ExtantAbhorrence").Value;

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, handle.Height);
            Vector2 drawOffset = Projectile.Center - Main.screenPosition;

            Main.EntitySpriteDraw(handle, drawOffset, null, lightColor, drawRotation, drawOrigin, Projectile.scale, 0f, 0);

            //Turn on additive blending
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            //Just in case
            if (OverCharge < 0)
                OverCharge = 0f;
            //When the blink is
            GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(OverCharge / 20f);
            GameShaders.Misc["CalamityMod:BasicTint"].UseColor(new Color(154, 244, 240));
            GameShaders.Misc["CalamityMod:BasicTint"].Apply();

            //Update the parameters
            drawOrigin = new Vector2(0f, blade.Height);

            Main.EntitySpriteDraw(blade, drawOffset, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, Projectile.scale, 0f, 0);

            //Back to normal
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            //Cut the velocity short if dashing
            if (State == 1f)
                Owner.velocity *= 0.33f;

            Owner.Calamity().LungingDown = false;

            Projectile.active = false;
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
}
