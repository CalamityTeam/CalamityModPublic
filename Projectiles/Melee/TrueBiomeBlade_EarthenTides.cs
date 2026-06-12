using System;
using System.IO;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Melee
{
    public class EarthenTides : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/TrueBiomeBlade_EarthenTides";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public Player Owner => Main.player[Projectile.owner];
        public ref float Charge => ref Projectile.ai[0]; //Charge
        public ref float State => ref Projectile.ai[1]; //State 0 is "charging", State 1 is "thrusting"
        public ref float CurrentIndicator => ref Projectile.localAI[0]; //What "indicator" stage are you on.
        public ref float OverCharge => ref Projectile.localAI[1];

        const float MaxCharge = 420;

        public Vector2 lastDisplacement;
        public float dashDuration;

        public static readonly SoundStyle FullChargeSound = new("CalamityMod/Sounds/Item/MagicRockSound");
        public static readonly SoundStyle GroundImpact = new("CalamityMod/Sounds/Item/MagicRockImpact");

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.width = Projectile.height = 70;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
        }

        // Only deal damage while charging
        public override bool? CanDamage() => State == 1f;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float bladeLength = 145 * Projectile.scale;
            float bladeWidth = 25 * Projectile.scale;

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
            if (!initialized) //Initialization. Here it's literally just playing a sound
            {
                Projectile.velocity = Vector2.Zero;
                SoundEngine.PlaySound(SoundID.Item101, Projectile.Center);
                initialized = true;
            }

            // Attempt a charge if the player stops channeling the sword
            if (Owner.CantUseHoldout())
            {
                if (State == 0f)
                {
                    // You need at least 25% charge duration to do a charge
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
                        Projectile.ForceNetUpdate();
                    }
                }
            }

            if (State == 0f)
            {
                // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                direction.Normalize();
                Projectile.Center = Owner.Center + (direction * 70f * ChargeDisplacement());

                Charge++;
                OverCharge--;
                Projectile.timeLeft = 2;
                if ((Charge / MaxCharge >= 0.25f && CurrentIndicator == 0f) || (Charge / MaxCharge >= 0.5f && CurrentIndicator == 1f) || (Charge / MaxCharge >= 0.75f && CurrentIndicator == 2f) && Owner.whoAmI == Main.myPlayer)
                {
                    for (int s = 0; s < 2; s++)
                    {
                        Vector2 swordVel = (Vector2.UnitX.RotatedBy(Projectile.rotation) * 10f).RotatedByRandom(MathHelper.Pi / 8);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, swordVel, ProjectileType<EarthenTidesBeam>(), (int)(Projectile.damage * OmegaBiomeBlade.ShockwaveAttunement_BeamDamageMult), 10f, Owner.whoAmI);
                    }

                    SoundEngine.PlaySound(SoundID.Item69 with { Pitch = -0.2f + 0.1f * CurrentIndicator }, Projectile.Center);

                    CurrentIndicator++;
                    OverCharge = 20f;
                }

                if (Charge >= MaxCharge)
                {
                    Charge = MaxCharge;
                    if (Owner.whoAmI == Main.myPlayer && CurrentIndicator < 4f)
                    {
                        for (int s = 0; s < 5; s++)
                        {
                            Vector2 swordVel = (Vector2.UnitX.RotatedBy(Projectile.rotation) * 10f).RotatedByRandom(MathHelper.Pi / 8);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, swordVel, ProjectileType<EarthenTidesBeam>(), (int)(Projectile.damage * OmegaBiomeBlade.ShockwaveAttunement_BeamDamageMult), 10f, Owner.whoAmI);
                        }
                        OverCharge = 20f;
                        SoundEngine.PlaySound(FullChargeSound, Projectile.Center);
                        CurrentIndicator++;
                    }

                    if (Main.rand.NextBool())
                    {
                        Vector2 sparkPos = Projectile.Center + direction * Main.rand.NextFloat(50f, 100f);
                        Vector2 sparkVel = direction.RotatedByRandom(MathHelper.Pi / 15f) * Main.rand.NextFloat(9f, 18f) + (Owner.velocity * 0.5f);
                        Color sparkColor = Color.Lerp(new Color(71, 191, 71), new Color(122, 213, 233), Main.rand.NextFloat());
                        CustomSprite critSpark = new(sparkPos, sparkVel, 16, "CalamityMod/Particles/CritSpark", 1.4f, sparkColor, frameCount: 4, frame: Main.rand.Next(4));
                        GeneralParticleHandler.SpawnParticle(critSpark);
                    }
                }
            }

            if (State == 1f)
            {
                Projectile.Center = Owner.Center + Vector2.Lerp(lastDisplacement, direction * 40f, MathHelper.Clamp(((dashDuration - Projectile.timeLeft) / dashDuration) * 2f, 0f, 1f));
                Owner.fallStart = (int)(Owner.position.Y / 16f);

                Owner.Calamity().LungingDown = true;

                Vector2 collisionCheckPos = Owner.Center + (direction * 120 * Projectile.scale) - Vector2.One * 5f;
                if (Collision.SolidCollision(collisionCheckPos, 10, 10))
                {
                    SlamDown(collisionCheckPos);
                    Projectile.timeLeft = 0;
                    Owner.Calamity().LungingDown = false;
                    Projectile.active = false;
                    Projectile.ForceNetUpdate();
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

            Owner.ChangeDir(Math.Sign(direction.X));
            Owner.itemRotation = direction.ToRotation();

            if (Owner.direction != 1)
            {
                Owner.itemRotation -= MathHelper.Pi;
            }

            Owner.itemRotation = MathHelper.WrapAngle(Owner.itemRotation);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        public void SlamDown(Vector2 collisionSpot)
        {
            if (Owner.whoAmI != Main.myPlayer || Owner.velocity.Y == 0f)
                return;

            // Sound and screenshake
            SoundEngine.PlaySound(GroundImpact, Projectile.Center);
            Main.LocalPlayer.SetScreenshake(15f);

            // Dust and particles from the impact
            for (int d = 0; d < 13; d++)
                Dust.NewDustPerfect(collisionSpot, Main.rand.NextBool() ? DustID.Clay : DustID.Dirt, Main.rand.NextVector2CircularEdge(6f, 6f), Scale: 1.2f);

            CustomPulse shatter = new(collisionSpot, Vector2.Zero, Color.SandyBrown, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.03f, 0.275f, 30);
            GeneralParticleHandler.SpawnParticle(shatter);
            for (int i = 0; i < 10; i++)
            {
                Vector2 rockVel = -(Owner.velocity * 0.55f).RotatedByRandom(MathHelper.Pi / 2.5f);
                StoneDebrisParticle rock = new(collisionSpot, rockVel, Color.White, 1f, 35);
                GeneralParticleHandler.SpawnParticle(rock);
            }

            // Spawn the blast spawner
            // Its duration scales with the current charge level
            int duration = 12 + (int)CurrentIndicator * 12;
            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, Vector2.Zero, ProjectileType<EarthenTidesBlastSpawner>(), (int)(Projectile.damage * OmegaBiomeBlade.ShockwaveAttunement_MonolithDamageBoost), 0f, Owner.whoAmI, duration);

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 17APR2024: Ozzatron: True Biome Blade's shockwave slam gives iframes when striking enemies in a similar manner to a ram dash.
            // This is a fixed and intentionally very low number of iframes, and is not boosted by Cross Necklace.
            Owner.GiveUniversalIFrames(OmegaBiomeBlade.ShockwaveAttunement_DashHitIFrames);

            if (!CalamityUtils.AnyProjectiles(ProjectileType<EarthenTidesShockwave>()))
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<EarthenTidesShockwave>(), (int)(Projectile.damage * 0.75f), 0f, Owner.whoAmI, 1f);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= (OmegaBiomeBlade.ShockwaveAttunement_FullChargeMult * (float)Math.Pow(Charge / MaxCharge, 2));

            if (Owner.HeldItem.ModItem is OmegaBiomeBlade sword && Main.rand.NextFloat() <= OmegaBiomeBlade.ShockwaveAttunement_SwordProc)
                sword.OnHitProc = true;

            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ProjectileType<PurityProjectionSigil>() && proj.owner == Owner.whoAmI)
                {
                    //Reset the timeLeft on the sigil & set its new target (or same target, doesn't matter)
                    proj.ai[0] = target.whoAmI;
                    proj.timeLeft = OmegaBiomeBlade.ShockwaveAttunement_SigilTime;
                    return;
                }
            }
            Projectile sigil = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<PurityProjectionSigil>(), 0, 0, Owner.whoAmI, target.whoAmI, 1f);
            sigil.timeLeft = OmegaBiomeBlade.ShockwaveAttunement_SigilTime;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D handle = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/OmegaBiomeBlade").Value;
            Texture2D blade = Request<Texture2D>("CalamityMod/Projectiles/Melee/TrueBiomeBlade_EarthenTides").Value;

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
