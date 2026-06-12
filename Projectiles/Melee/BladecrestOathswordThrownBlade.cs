using System;
using System.IO;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Melee
{
    public class BladecrestOathswordThrownBlade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Items/Weapons/Melee/BladecrestOathsword";

        public int ChargeupTime => (int)MathHelper.Clamp(Owner.HeldItem.useTime / 2.8f, 1, 100);
        public static int Lifetime = 100;
        public float OverallProgress => 1 - Projectile.timeLeft / (float)Lifetime;
        public float ThrowProgress => 1 - Projectile.timeLeft / (float)(Lifetime);
        public float ChargeProgress => 1 - (Projectile.timeLeft - Lifetime) / (float)(ChargeupTime);
        public Player Owner => Main.player[Projectile.owner];

        // Real variables here
        public ref float time => ref Projectile.ai[0];
        public ref float stabOrder => ref Projectile.ai[1];
        public ref NPC stabbedTarget => ref Main.npc[(int)Projectile.ai[2]];

        [Flags]
        public enum State : byte
        {
            None = 0,
            HasSpawned = 1,
            Thrown = 2,
            HitTarget = 4,
            StuckInTarget = 8,
            LeftTarget = 16,
            StuckInGround = 32,
        }

        public State CurrentState;

        public static int fadeOutTime = 60;
        public int stuckTimer = 0;
        public Vector2 impalePos;
        public int bounces = 0;
        public bool fadingOut => Projectile.timeLeft <= (Lifetime - fadeOutTime);

        private Vector2 GetAimDirection()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 toMouse = (Owner.Calamity().mouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX * Owner.direction);
                float aimAngle = toMouse.ToRotation();
                if (Math.Abs(MathHelper.WrapAngle(aimAngle - Projectile.localAI[1])) > 0.02f)
                {
                    Projectile.localAI[1] = aimAngle;
                    if (Projectile.timeLeft % 6 == 0)
                        Projectile.netUpdate = true;
                }
                return toMouse;
            }
            return Projectile.localAI[1].ToRotationVector2();
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime + ChargeupTime;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.noEnchantmentVisuals = true;
        }

        public override bool ShouldUpdatePosition() => ChargeProgress >= 1 && (CurrentState & (State.StuckInTarget | State.StuckInGround)) == 0;

        //Swing animation keys
        public CurveSegment pullback = new(EasingType.PolyOut, 0f, 0f, MathHelper.PiOver4 * -1.2f, 2);
        public CurveSegment throwout = new(EasingType.PolyOut, 0.7f, MathHelper.PiOver4 * -1.2f, MathHelper.PiOver4 * 1.2f + MathHelper.PiOver2, 3);
        internal float ArmAnticipationMovement() => PiecewiseAnimation(ChargeProgress, [pullback, throwout]);

        public override void AI()
        {
            if (time == 0)
                Projectile.scale = Owner.GetMeleeScale();
            if ((CurrentState & State.HasSpawned) == 0)
            {
                Projectile.timeLeft = Lifetime + ChargeupTime;
                CurrentState |= State.HasSpawned;
            }

            if ((CurrentState & State.HitTarget) != 0)
            {
                foreach (var target in Main.ActiveNPCs)
                {
                    if ((CurrentState & State.LeftTarget) == 0 && CircularHitboxCollision(Projectile.Center, Projectile.width / ((CurrentState & State.LeftTarget) != 0 ? 0.5f : 1) * Projectile.scale, target.getRect()))
                    {
                        SoundStyle stuck = new("CalamityMod/Sounds/Item/DemonSwordImpact", 2);
                        SoundEngine.PlaySound(stuck with { Volume = 0.75f, Pitch = Main.rand.NextFloat(-0.1f, 0.1f), MaxInstances = 3 }, Projectile.Center);

                        Projectile.ai[2] = target.whoAmI;

                        if (target.Calamity().demonSwordImpales < 0)
                            target.Calamity().demonSwordImpales = 0;
                        if (target.Calamity().demonSwordImpales >= 3)
                        {
                            float bladeValue = -1;
                            Projectile ejectedBlade = null;
                            for (int x = 0; x < Main.maxProjectiles; x++)
                            {
                                Projectile projectile = Main.projectile[x];
                                if (projectile.owner == Projectile.owner && projectile.type == Projectile.type && Projectile.localAI[0] != 5 && projectile.ai[2] == Projectile.ai[2] && projectile.timeLeft > (Lifetime - fadeOutTime) && (bladeValue == -1 || bladeValue > stabOrder))
                                {
                                    bladeValue = projectile.ai[1];
                                    ejectedBlade = projectile;
                                }
                            }
                            if (ejectedBlade != null)
                            {
                                ejectedBlade.localAI[0] = 5;
                                ejectedBlade.ai[1] += 1000;
                                ejectedBlade.velocity = Projectile.velocity.RotatedByRandom(0.2f);
                                ejectedBlade.ForceNetUpdate();
                            }

                            for (int i = 0; i < 8; i++)
                            {
                                Particle spark2 = new SparkParticle(target.Center, Projectile.velocity.RotatedByRandom(0.6) * Main.rand.NextFloat(0.2f, 1f), false, 30, Main.rand.NextFloat(0.3f, 0.7f), Main.rand.NextBool(3) ? Color.Red : Color.Crimson);
                                GeneralParticleHandler.SpawnParticle(spark2);

                                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), Projectile.velocity.RotatedByRandom(0.4) * Main.rand.NextFloat(0.3f, 0.8f), 0, default, Main.rand.NextFloat(1.3f, 1.6f));
                                dust.noGravity = true;
                                dust.noLight = true;
                                dust.color = Main.rand.NextBool(3) ? Color.Red : Color.Crimson;
                            }
                        }
                        target.Calamity().demonSwordImpales++;

                        CurrentState |= State.StuckInTarget;
                        impalePos = Projectile.Center - stabbedTarget.Center;
                        stuckTimer = 3600;
                        break;
                    }
                }

                CurrentState &= ~State.HitTarget;
            }

            if ((CurrentState & State.Thrown) != 0 && (CurrentState & (State.StuckInGround | State.StuckInTarget)) == 0)
                Projectile.extraUpdates = 1;

            float playerDist = Vector2.Distance(Owner.Center, Projectile.Center);

            Projectile.spriteDirection = Projectile.direction;
            Vector3 Light = Color.Firebrick.ToVector3();
            Lighting.AddLight(Projectile.Center, Light * 0.5f);

            if (Projectile.timeLeft == Lifetime && Main.myPlayer == Projectile.owner)
            {
                // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
                Vector2 toMouse = GetAimDirection();
                Projectile.velocity = toMouse * 14;
                Projectile.Center = Owner.MountedCenter + toMouse * 12f * Projectile.scale;
                Projectile.spriteDirection = Projectile.direction;
                CurrentState |= State.Thrown;
                time = 0;
                Projectile.extraUpdates = 1;
                Projectile.tileCollide = true;
                Projectile.netUpdate = true;
            }

            if (Projectile.velocity.X > 0)
                Projectile.direction = 1;
            else
                Projectile.direction = -1;

            if ((CurrentState & State.Thrown) != 0)
            {
                Projectile.spriteDirection = Projectile.direction;
                if ((CurrentState & State.StuckInGround) == 0 && (CurrentState & State.LeftTarget) != 0)
                    Projectile.rotation += 0.35f * (MathF.Abs(Projectile.velocity.Y) * 0.03f + 0.85f) * Main.rand.NextFloat(0.7f, 1f) * Projectile.direction * Projectile.Opacity;
                else
                    Projectile.rotation = (Projectile.velocity.ToRotation() + MathHelper.PiOver4 * (Projectile.direction == 1 ? 1 : 3));

                if (time > fadeOutTime * 0.7f)
                    Projectile.velocity *= 0.93f;

                if ((CurrentState & State.StuckInTarget) != 0)
                {
                    Projectile.tileCollide = false;
                    time--;
                    Projectile.timeLeft++;
                    if (stuckTimer > 0)
                        stuckTimer--;
                    else
                    {
                        Projectile.velocity *= 0.01f;
                        stabbedTarget.Calamity().demonSwordImpales--;
                        CurrentState &= ~State.StuckInTarget;
                        fadeOutEffect();
                    }

                    Projectile.Center = stabbedTarget.Center + impalePos;
                    if ((stabbedTarget.life <= 0) || stabbedTarget == null || Projectile.localAI[0] == 5 || !stabbedTarget.active)
                    {
                        Projectile.timeLeft = Lifetime;
                        stabbedTarget.Calamity().demonSwordImpales--;
                        CurrentState &= ~State.StuckInTarget;
                        CurrentState |= State.LeftTarget;
                        Projectile.tileCollide = true;
                        Projectile.rotation += Main.rand.NextFloat(-1.5f, 1.5f);
                        for (int i = 0; i < Main.maxNPCs; i++)
                            Projectile.localNPCImmunity[i] = 0;
                        Projectile.numHits = 0;
                        SoundStyle unstuck = new("CalamityMod/Sounds/NPCHit/PerfLargeHit", 3);
                        SoundEngine.PlaySound(unstuck with { Volume = 0.85f, Pitch = Main.rand.NextFloat(0.3f, 0.4f), MaxInstances = 3 }, Projectile.Center);
                    }
                }

                if ((CurrentState & State.LeftTarget) != 0 && (CurrentState & State.StuckInGround) == 0)
                {
                    if (Projectile.velocity.Y < 14)
                        Projectile.velocity.Y += 0.1f * (bounces > 0 ? 4 : 1);
                }

                if (fadingOut)
                    fadeOutEffect();
            }

            if (!fadingOut && (CurrentState & State.StuckInTarget) == 0 && (CurrentState & State.StuckInGround) == 0 && ChargeProgress >= 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 safeVel = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                    Vector2 dustVel = (CurrentState & State.LeftTarget) != 0 ? Vector2.One.RotatedByRandom(MathHelper.Pi) : safeVel.RotatedBy(MathHelper.ToRadians(105 * (i == 0 ? 1 : -1))).RotatedByRandom(0.1f) * Main.rand.NextFloat(3, 4);
                    if (Main.rand.NextBool(3))
                    {
                        Dust dust = Dust.NewDustPerfect(
                            Projectile.Center + safeVel.RotatedBy((CurrentState & State.LeftTarget) != 0 ? Projectile.rotation - MathHelper.ToRadians(Projectile.direction * 45) : 0) * ((CurrentState & State.LeftTarget) != 0 ? 45 : 60) * Projectile.scale,
                            ModContent.DustType<SquashDust>(),
                            dustVel,
                            0,
                            default,
                            ((CurrentState & State.LeftTarget) != 0 ? 1.5f : 1) * Main.rand.NextFloat(0.8f, 0.9f) * Projectile.scale);
                        dust.noGravity = true;
                        dust.color = Main.rand.NextBool() ? Color.Red : Color.Crimson;
                        dust.noLight = true;
                        dust.noLightEmittence = true;
                        dust.alpha = 100;
                        dust.fadeIn = Projectile.scale - 1;
                    }
                }
                if (Main.rand.NextBool(5))
                {
                    Particle spark2 = new CustomSpark(
                        relativePosition: Projectile.Center,
                        velocity: -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.2f, 0.6f),
                        texture: "CalamityMod/Particles/DemonSigilParticle",
                        affectedByGravity: false,
                        lifetime: 17,
                        scale: Main.rand.NextFloat(0.2f, 0.3f) * Projectile.scale,
                        color: Color.Lerp(Color.Crimson, Color.Red, Main.rand.NextFloat(0, 0.7f)) * 0.6f,
                        stretch: new Vector2(1f, 1f),
                        extraRotation: Main.rand.NextFloat(-1f, 1f));
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
            }

            //Anticipation animation. Make the player look like they're holding the item
            if (ChargeProgress < 1)
            {
                throwAnimation();
                if (ChargeProgress >= 0.6f && time == 0)
                {
                    SoundStyle swing = new("CalamityMod/Sounds/Item/DemonSwordSwing", 2);
                    SoundEngine.PlaySound(swing with { Volume = 0.65f, Pitch = Main.rand.NextFloat(-0.1f, 0.1f) }, Projectile.Center);
                    time++;
                }
                return;
            }
            else
                time++;
        }

        public void fadeOutEffect()
        {
            Projectile.tileCollide = false;
            if (Projectile.timeLeft > Lifetime - fadeOutTime)
                Projectile.timeLeft = Lifetime - fadeOutTime;
            Projectile.Opacity = Utils.GetLerpValue(0, Lifetime - fadeOutTime, Projectile.timeLeft, true);

            Vector2 dustVel = new Vector2(10, 10).RotatedByRandom(Math.PI) * Main.rand.NextFloat(0.2f, 1f) * Projectile.Opacity;
            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>(), dustVel, 0, default, Main.rand.NextFloat(1.1f, 1.4f) * Projectile.scale);
                dust.noGravity = true;
                dust.color = Main.rand.NextBool() ? Color.Red : Color.Crimson;
                dust.noLight = true;
                dust.noLightEmittence = true;
                dust.alpha = 100;
                dust.velocity += Projectile.velocity;
                dust.fadeIn = Projectile.scale - 1;
            }
        }

        public void throwAnimation()
        {
            Vector2 aimDirection = GetAimDirection();
            Owner.ChangeDir(MathF.Sign(aimDirection.X));

            float armRotation = ArmAnticipationMovement() * Owner.direction;

            Owner.heldProj = Projectile.whoAmI;
            Projectile.spriteDirection = Owner.direction;
            Projectile.direction = Owner.direction;

            Projectile.Center = Owner.MountedCenter + Vector2.UnitY.RotatedBy(armRotation * Owner.gravDir) * -45f * Owner.gravDir * Projectile.scale;
            Projectile.rotation = (-MathHelper.PiOver4 * Projectile.direction + armRotation) * Owner.gravDir;

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi + armRotation);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CurrentState |= State.HitTarget;
            Projectile.netUpdate = true;
            Projectile.netSpam = 0;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.4f;
            int hitsToMinMult = 8;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult * ((CurrentState & State.LeftTarget) != 0 ? 1.15f : 1f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if ((CurrentState & State.LeftTarget) != 0)
            {
                if (bounces >= 1)
                {
                    impaleGround(oldVelocity);
                }
                else
                {
                    if (Projectile.velocity.X != oldVelocity.X)
                    {
                        Projectile.velocity.X = -oldVelocity.X;
                    }
                    if (Projectile.velocity.Y != oldVelocity.Y)
                    {
                        Projectile.velocity.Y = -oldVelocity.Y;
                    }
                    if (Projectile.velocity.Length() < 8)
                        Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 8;
                    Projectile.velocity.Y *= 0.85f;
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/CeramicImpact", 2) { Volume = 0.35f, Pitch = Main.rand.NextFloat(-0.4f, -0.5f) }, Projectile.Center);
                    bounces++;
                    Projectile.timeLeft = Lifetime;
                }
            }
            else
            {
                impaleGround(oldVelocity);
            }

            return false;
        }

        public void impaleGround(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            CurrentState |= State.StuckInGround;
            Projectile.timeLeft = (int)(Lifetime - fadeOutTime * 0.3f);
            Projectile.tileCollide = false;
            SoundStyle stuck = new("CalamityMod/Sounds/Item/DemonSwordImpact", 2);
            SoundEngine.PlaySound(stuck with { Volume = 0.75f, Pitch = Main.rand.NextFloat(0.2f, 0.3f), MaxInstances = 3 }, Projectile.Center);
            Projectile.netUpdate = true;
        }

        public override bool? CanDamage()
        {
            if (ChargeProgress < 1 || fadingOut || (CurrentState & State.StuckInGround) != 0 || (CurrentState & State.StuckInTarget) != 0 || (Projectile.numHits > 0 && (CurrentState & State.LeftTarget) == 0))
                return false;

            return base.CanDamage();
        }

        // After exiting a target, the hitbox is larger
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            => (Projectile.numHits <= 0 || (CurrentState & State.LeftTarget) != 0) && CircularHitboxCollision(Projectile.Center, Projectile.width / ((CurrentState & State.LeftTarget) != 0 ? 0.5f : 1) * Projectile.scale, targetHitbox);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D centerTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/BladecrestOathsword").Value;

            float fadeScale = 1f - Projectile.Opacity;
            for (int i = 0; i < 16; i++)
            {
                Color auraColor = Color.Crimson with { A = 0 } * 0.4f * fadeScale * Projectile.Opacity;
                Vector2 drawOffset = (MathHelper.TwoPi * i / 16f).ToRotationVector2() * 9f * fadeScale;
                Main.EntitySpriteDraw(
                    centerTexture,
                    Projectile.Center - Main.screenPosition + drawOffset,
                    null,
                    auraColor,
                    Projectile.rotation,
                    centerTexture.Size() * 0.5f,
                    Projectile.scale,
                    Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }

            Main.EntitySpriteDraw(centerTexture,
                Projectile.Center - Main.screenPosition,
                null,
                Color.Lerp(Color.Red with { A = 0 }, lightColor, Projectile.Opacity) * Projectile.Opacity,
                Projectile.rotation,
                centerTexture.Size() * 0.5f,
                Projectile.scale,
                Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((byte)CurrentState);
            writer.Write(Projectile.rotation);
            writer.Write(Projectile.localAI[1]);
            writer.Write(impalePos.X);
            writer.Write(impalePos.Y);
            writer.Write(stuckTimer);
            writer.Write((short)bounces);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            CurrentState = (State)reader.ReadByte();
            Projectile.rotation = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            impalePos = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            stuckTimer = reader.ReadInt32();
            bounces = reader.ReadInt16();
        }
    }
}
