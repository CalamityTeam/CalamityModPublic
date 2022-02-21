using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;
using Microsoft.Xna.Framework.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class AndromedasStride : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalaxiaExtra";
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
            DisplayName.SetDefault("Andromeda's Stride");
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
                        CalamityUtils.SafeVolumeChange(ref dashSound, 0.5f);
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
                direction = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                direction.Normalize();
                projectile.Center = Owner.Center + (direction * 70f * ChargeDisplacement());

                Charge++;
                OverCharge--;
                projectile.timeLeft = 2;
                if ((Charge / MaxCharge >= 0.25f && CurrentIndicator == 0f) || (Charge / MaxCharge >= 0.5f && CurrentIndicator == 1f) || (Charge / MaxCharge >= 0.75f && CurrentIndicator == 2f) && Owner.whoAmI == Main.myPlayer)
                {
                    //WOahhh do a ring of projectiles!! woahhh
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile blast = Projectile.NewProjectileDirect(Owner.Center, (MathHelper.TwoPi * i / 5f).ToRotationVector2() * 10f, ProjectileType<GalaxiaBolt>(), (int)(projectile.damage * FourSeasonsGalaxia.AndromedaAttunement_BoltsDamageReduction), 0f, Owner.whoAmI, 0.75f, MathHelper.Pi * 0.02f);
                        {
                            blast.timeLeft = 30;
                        }
                    }

                    Main.PlaySound(SoundID.Item79, projectile.Center);
                    CurrentIndicator++;
                    OverCharge = 20f;
                }

                if (Charge >= MaxCharge)
                {
                    Charge = MaxCharge;

                    if (Main.rand.NextBool())
                    {
                        Vector2 smokeSpeed = direction.RotatedByRandom(MathHelper.PiOver4 * 0.3f) * Main.rand.NextFloat(10f, 30f) * 0.9f;
                        Particle smoke = new HeavySmokeParticle(projectile.Center + direction * 50f, smokeSpeed + Owner.velocity, Color.Lerp(Color.Purple, Color.Indigo, (float)Math.Sin(Main.GlobalTime * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f), 0.8f, 0, false, 0, true);
                        GeneralParticleHandler.SpawnParticle(smoke);

                        if (Main.rand.Next(3) == 0)
                        {
                            Particle smokeGlow = new HeavySmokeParticle(projectile.Center + direction * 50f, smokeSpeed + Owner.velocity, Main.hslToRgb(0.85f, 1, 0.8f), 20, Main.rand.NextFloat(0.4f, 0.7f), 0.8f, 0, true, 0.01f, true);
                            GeneralParticleHandler.SpawnParticle(smokeGlow);
                        }
                    }


                    if (Owner.whoAmI == Main.myPlayer && CurrentIndicator < 4f)
                    {
                        //Projectiles!!! wah!!!
                        for (int i = 0; i < 9; i++)
                        {
                            Projectile blast = Projectile.NewProjectileDirect(Owner.Center, (MathHelper.TwoPi * i / 9f).ToRotationVector2() * 10f, ProjectileType<GalaxiaBolt>(), (int)(projectile.damage * FourSeasonsGalaxia.AndromedaAttunement_BoltsDamageReduction), 0f, Owner.whoAmI, 0.75f, MathHelper.Pi * 0.02f);
                            {
                                blast.timeLeft = 50;
                            }
                        }

                        OverCharge = 20f;
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AstralBeaconUse"), projectile.Center);
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
            if (Owner.whoAmI != Main.myPlayer)
                return;

            if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 15)
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = 15;

            Projectile proj = Projectile.NewProjectileDirect(Owner.Center + (direction * 120 * projectile.scale), -direction * 16f, ProjectileType<GalaxiaBolt>(), projectile.damage, 10f, Owner.whoAmI, 0.75f, MathHelper.Pi / 25f);
            proj.scale = 3f;
            proj.timeLeft = 50;

            SideSprouts(1, 150f, 1f * Charge / MaxCharge);
            SideSprouts(-1, 150f, 1f * Charge / MaxCharge);

            SoundEffectInstance slamSound = Main.PlaySound(SoundID.DD2_MonkStaffGroundImpact, projectile.Center);
            CalamityUtils.SafeVolumeChange(ref slamSound, 1.5f);
        }

        public bool SideSprouts(float facing, float distance, float projSize)
        {
            float widestAngle = 0f;
            float widestSurfaceAngle = 0f;
            bool validPositionFound = false;
            for (float i = 0f; i < 1; i += 1 / distance)
            {
                Vector2 positionToCheck = Owner.Center + (direction * 120 * projectile.scale) + direction.RotatedBy((i * MathHelper.PiOver2 + MathHelper.PiOver4) * facing) * distance;

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
                Vector2 projPosition = Owner.Center + (direction * 120 * projectile.scale) + direction.RotatedBy((widestSurfaceAngle * MathHelper.PiOver2 + MathHelper.PiOver4) * facing) * distance;
                Vector2 monolithRotation = direction.RotatedBy(Utils.AngleLerp(widestSurfaceAngle * -facing, 0f, projSize));
                Projectile proj = Projectile.NewProjectileDirect(projPosition, -monolithRotation, ProjectileType<AndromedasStrideBoltSpawner>(), (int)(projectile.damage * FourSeasonsGalaxia.AndromedaAttunement_MonolithDamageBoost), 10f, Owner.whoAmI, Main.rand.Next(4), projSize);
                if (proj.modProjectile is AndromedasStrideBoltSpawner spawner)
                {
                    spawner.WaitTimer = (1 - projSize) * 34f;
                    spawner.OriginDirection = direction;
                    spawner.Facing = facing;
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
            Texture2D sword = GetTexture("CalamityMod/Items/Weapons/Melee/GalaxiaExtra");

            float drawAngle = direction.ToRotation();
            float drawRotation = drawAngle + MathHelper.PiOver4;

            Vector2 drawOrigin = new Vector2(0f, sword.Height);
            Vector2 drawOffset = projectile.Center - Main.screenPosition;

            CalamityUtils.EnterShaderRegion(spriteBatch);

            if (OverCharge < 0)
                OverCharge = 0f;
            //When the blink is
            GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(OverCharge / 20f);
            GameShaders.Misc["CalamityMod:BasicTint"].UseColor(new Color(255, 129, 153));
            GameShaders.Misc["CalamityMod:BasicTint"].Apply();

            spriteBatch.Draw(sword, drawOffset, null, lightColor, drawRotation, drawOrigin, projectile.scale, 0f, 0f);

            CalamityUtils.ExitShaderRegion(spriteBatch);

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
}