using System;
using System.IO;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Tiles.Astral;
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
    public class AndromedasStride : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalaxiaExtra";
        private bool initialized = false;
        Vector2 direction = Vector2.Zero;
        public Player Owner => Main.player[Projectile.owner];
        public ref float Charge => ref Projectile.ai[0]; //Charge
        public ref float State => ref Projectile.ai[1]; //State 0 is "charging", State 1 is "thrusting"
        public ref float CurrentIndicator => ref Projectile.localAI[0]; //What "indicator" stage are you on.
        public ref float OverCharge => ref Projectile.localAI[1];

        const float MaxCharge = 360;

        public Vector2 lastDisplacement;
        public float dashDuration;

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

        public override bool? CanDamage() => State == 1f ? (bool?)null : false;

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
            if (!initialized) //Initialization. Here its litterally just playing a sound tho lmfao
            {
                Projectile.velocity = Vector2.Zero;
                SoundEngine.PlaySound(SoundID.Item101, Projectile.Center);
                initialized = true;
            }

            if (Owner.CantUseHoldout())
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
                        float screenshakeLevel = 4f + CurrentIndicator * 2f;
                        Main.LocalPlayer.SetScreenshake(screenshakeLevel);
                        CustomPulse shatter = new(Owner.Center, Vector2.Zero, Color.HotPink, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.0075f * CurrentIndicator, 0.075f * CurrentIndicator, 30);
                        GeneralParticleHandler.SpawnParticle(shatter);

                        State = 1f;
                        Projectile.timeLeft = (7 + (int)((Charge / MaxCharge - 0.25f) * 20)) * 2; // Keep that even, if it's an odd number it'll fuck off and won't reset the player's velocity on death
                        dashDuration = Projectile.timeLeft;
                        lastDisplacement = Projectile.Center - Owner.Center;
                        Projectile.ForceNetUpdate();
                    }
                }
            }

            if (State == 0f)
            {
                // 14NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                direction.Normalize();
                Projectile.Center = Owner.Center + (direction * 70f * ChargeDisplacement());

                Charge++;
                OverCharge--;
                Projectile.timeLeft = 2;
                if ((Charge / MaxCharge >= 0.25f && CurrentIndicator == 0f) || (Charge / MaxCharge >= 0.5f && CurrentIndicator == 1f) || (Charge / MaxCharge >= 0.75f && CurrentIndicator == 2f) && Owner.whoAmI == Main.myPlayer)
                {
                    // Spawn a ring of stars
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 velocity = direction.RotatedByRandom(MathHelper.PiOver4) * 10f;
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, velocity, ProjectileType<GalaxiaBolt>(), (int)(Projectile.damage * FourSeasonsGalaxia.AndromedaAttunement_ChargeupBoltDamageMultiplier), 0f, Owner.whoAmI, 0.75f, MathHelper.Pi * 0.02f, 1f);
                    }

                    SoundEngine.PlaySound(SoundID.Item79, Projectile.Center);
                    CurrentIndicator++;
                    OverCharge = 20f;
                }

                if (Charge >= MaxCharge)
                {
                    Charge = MaxCharge;

                    if (Main.rand.NextBool())
                    {
                        Vector2 smokeSpeed = direction.RotatedByRandom(MathHelper.PiOver4 * 0.3f) * Main.rand.NextFloat(10f, 30f) * 0.9f;
                        Particle smoke = new HeavySmokeParticle(Projectile.Center + direction * 50f, smokeSpeed + Owner.velocity, Color.Lerp(Color.Purple, Color.Indigo, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f), 0.8f, 0, false, 0, true);
                        GeneralParticleHandler.SpawnParticle(smoke);

                        if (Main.rand.NextBool(3))
                        {
                            Particle smokeGlow = new HeavySmokeParticle(Projectile.Center + direction * 50f, smokeSpeed + Owner.velocity, Main.hslToRgb(0.85f, 1, 0.8f), 20, Main.rand.NextFloat(0.4f, 0.7f), 0.8f, 0, true, 0.01f, true);
                            GeneralParticleHandler.SpawnParticle(smokeGlow);
                        }
                    }


                    if (Owner.whoAmI == Main.myPlayer && CurrentIndicator < 4f)
                    {
                        // Spawn more stars
                        for (int i = 0; i < 9; i++)
                        {
                            Vector2 velocity = direction.RotatedByRandom(MathHelper.PiOver4) * 10f;
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Owner.Center, velocity, ProjectileType<GalaxiaBolt>(), (int)(Projectile.damage * FourSeasonsGalaxia.AndromedaAttunement_ChargeupBoltDamageMultiplier), 0f, Owner.whoAmI, 0.75f, MathHelper.Pi * 0.02f, 1f);
                        }

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

                // Immediately cancel the lunge if you hit a tile
                // Unlike its downgrades, there are NO effects from hitting tiles - it's too impractical at this stage, why would there be?
                if (Collision.SolidCollision(Owner.Center + (direction * 120 * Projectile.scale) - Vector2.One * 5f, 10, 10))
                {
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 17APR2024: Ozzatron: Galaxia's dash gives iframes when striking enemies in a similar manner to a ram dash.
            // This is a fixed and intentionally very low number of iframes, and is not boosted by Cross Necklace.
            Owner.GiveUniversalIFrames(FourSeasonsGalaxia.AndromedaAttunement_DashHitIFrames);

            // ai[0] is NPC index to stay locked onto it, ai[1] is charge level to determine how many stars are rained
            if (!CalamityUtils.AnyProjectiles(ProjectileType<AndromedasStrideBoltSpawner>()))
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<AndromedasStrideBoltSpawner>(), (int)(Projectile.damage * FourSeasonsGalaxia.AndromedaAttunement_StarDamageMultiplier), Projectile.knockBack, Owner.whoAmI, target.whoAmI, CurrentIndicator);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Lunge always critically strikes
            modifiers.SetCrit();
            modifiers.SourceDamage *= (FourSeasonsGalaxia.AndromedaAttunement_FullChargeMult * (float)Math.Pow(Charge / MaxCharge, 2));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sword = Request<Texture2D>("CalamityMod/Items/Weapons/Melee/GalaxiaExtra").Value;

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, sword.Height);
            Vector2 drawOffset = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.EnterShaderRegion();

            if (OverCharge < 0)
                OverCharge = 0f;
            //When the blink is
            GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(OverCharge / 20f);
            GameShaders.Misc["CalamityMod:BasicTint"].UseColor(new Color(255, 129, 153));
            GameShaders.Misc["CalamityMod:BasicTint"].Apply();

            Main.EntitySpriteDraw(sword, drawOffset, null, lightColor, drawRotation, drawOrigin, Projectile.scale, 0f, 0);
            Main.spriteBatch.ExitShaderRegion();

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
