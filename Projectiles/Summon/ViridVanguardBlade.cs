using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class ViridVanguardBlade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public enum ViridVanguardAIState
        {
            CircleOwner,
            HorizontalSlashes,
            VerticalPierceTeleport,
            RegularPierceSlashes,
            ChargingCircle,
            PhotonRipperZenithSlashes,
            PhotonRipperZenithEndlag,
            AxeCircle
        }

        public int BladeIndex;

        public VertexStrip TrailDrawer;

        public Vector2 ChargeStartingPosition;

        public float BladeAngleForPhripperZenith = 0;

        public int CurrentViridCount => (int)MathHelper.Max(1, Owner.ownedProjectileCounts[Type]);

        public float BladeHoverOffsetAngle
        {
            get
            {
                return MathHelper.TwoPi * BladeIndex / CurrentViridCount + Owner.Calamity().ViridVanguardRotation * (Owner.Calamity().InvertExaltationLineRotationDirections ? -1 : 1);
            }
        }

        public ViridVanguardAIState CurrentState
        {
            get => (ViridVanguardAIState)Projectile.ai[0];
            set => Projectile.ai[0] = (int)value;
        }

        public static List<ViridVanguardAIState> ActiveAttackStateList = new() { ViridVanguardAIState.ChargingCircle, ViridVanguardAIState.PhotonRipperZenithSlashes, ViridVanguardAIState.PhotonRipperZenithEndlag };


        public Player Owner => Main.player[Projectile.owner];

        public ref float AITimer => ref Projectile.ai[1];

        public float ActiveTimer
        {
            get => Owner.Calamity().ViridVanguardActiveCooldown;
            set => Owner.Calamity().ViridVanguardActiveCooldown = value;
        }

        public bool IsAxe => BladeIndex % 2 == 0;

        public bool DrawAsAxe => IsInDotRange && CurrentState == ViridVanguardAIState.PhotonRipperZenithSlashes ? !ActiveAttacking : IsAxe;

        bool IsInDotRange = false;

        bool ActiveAttacking = false;

        Vector2 StoredAttackMousePos = Vector2.Zero;

        public ref float BladeGleamInterpolant => ref Projectile.localAI[0];

        #region AI consts
        public const int HorizontalSlashChargeTime = 14;

        public const float HorizontalSlashSpeed = 44f;

        public const int VerticalSlashChargeTime = 32;

        public const float VerticalSlashSpeed = 45f;

        public const float VerticalTeleportOffset = 850f;

        public const int PierceChargeAttackCycleTime = 44;

        public const float MaxTargetingDistance = 1550f;


        #endregion

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 45;
        }

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(BladeIndex);
            writer.WriteVector2(ChargeStartingPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BladeIndex = reader.ReadInt32();
            ChargeStartingPosition = reader.ReadVector2();
        }

        public override bool MinionContactDamage() => true;

        public override void AI()
        {
            // Decide whether the minion should still exist.
            HandleMinionBools();

            // Reset extra updates.
            Projectile.MaxUpdates = 1;

            // Have the gleam interpolant dissipate.
            BladeGleamInterpolant = MathHelper.Lerp(BladeGleamInterpolant, 0f, 0.1f);
            if (BladeGleamInterpolant <= 0.02f)
                BladeGleamInterpolant = 0f;

            NPC potentialTarget = Projectile.Center.MinionHoming(MaxTargetingDistance, Owner);
            switch (CurrentState)
            {
                case ViridVanguardAIState.CircleOwner:
                    DoBehavior_CircleOwner(potentialTarget);
                    break;
                case ViridVanguardAIState.HorizontalSlashes:
                    DoBehavior_HorizontalSlashes(potentialTarget);
                    break;
                case ViridVanguardAIState.VerticalPierceTeleport:
                    DoBehavior_VerticalPierceTeleport(potentialTarget);
                    break;
                case ViridVanguardAIState.RegularPierceSlashes:
                    DoBehavior_RegularPierceSlashes(potentialTarget);
                    break;
                case ViridVanguardAIState.ChargingCircle:
                    DoBehavior_PhotonRipperZenithStartup(potentialTarget);
                    break;
                case ViridVanguardAIState.PhotonRipperZenithSlashes:
                    DoBehavior_PhotonRipperZenithActive(potentialTarget);
                    break;
                case ViridVanguardAIState.PhotonRipperZenithEndlag:
                    DoBehavior_PhotonRipperZenithEndlag(potentialTarget);
                    break;
                case ViridVanguardAIState.AxeCircle:
                    DoBehavior_AxeCircle(potentialTarget);
                    break;
            }

            AITimer++;
            if (CurrentState == ViridVanguardAIState.PhotonRipperZenithEndlag)
                Projectile.MaxUpdates = 1;
        }

        public void DoBehavior_CircleOwner(NPC potentialTarget)
        {
            //Reset active cooldown if it's in the negative somehow
            if (ActiveTimer < 0)
                ActiveTimer = ViridVanguard.ActiveAttackCooldown;

            // Begin attacking if an enemy is nearby.
            var viridAmount = 300f / CurrentViridCount;
            if (potentialTarget is not null)
            {
                CurrentState = IsAxe ? ViridVanguardAIState.AxeCircle : ViridVanguardAIState.HorizontalSlashes;
                AITimer = 0f;
                Projectile.netUpdate = true;
                return;
            }
            float hoverDistMult = MathHelper.Lerp(IsAxe ? 170 : 200, IsAxe ? 200 : 170, (MathF.Sin(AITimer * 0.06f) + 1) * 0.5f);
            Vector2 hoverDestination = Owner.Center + BladeHoverOffsetAngle.ToRotationVector2() * hoverDistMult;
            Projectile.Center = Vector2.Lerp(Projectile.Center, hoverDestination, 0.04f).MoveTowards(hoverDestination, 20f);
            Projectile.velocity *= 0.8f;

            // Teleport to the owner if sufficiently far away.
            if (!Projectile.WithinRange(Owner.Center, 2500f))
            {
                Projectile.Center = hoverDestination;
                Projectile.netUpdate = true;
            }

            // Aim away from the target.
            Projectile.rotation = Projectile.AngleFrom(Owner.Center) + MathHelper.PiOver2;
        }

        public void DoBehavior_AxeCircle(NPC potentialTarget)
        {
            // Begin attacking if an enemy is nearby.
            var viridAmount = 300f / CurrentViridCount;
            if (potentialTarget is null)
            {
                ReturnToIdleState();
                return;
            }
            var offset = MathHelper.TwoPi * MathF.Floor(BladeIndex * 0.5f) / MathF.Ceiling(CurrentViridCount * 0.5f) + Owner.Calamity().ViridVanguardRotation * (Owner.Calamity().InvertExaltationLineRotationDirections ? -1 : 1);
            Vector2 hoverDestination = Owner.Center + offset.ToRotationVector2() * 200f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, hoverDestination, 0.04f).MoveTowards(hoverDestination, 32f);
            Projectile.velocity *= 0.8f;

            // Teleport to the owner if sufficiently far away.
            if (!Projectile.WithinRange(Owner.Center, 2500f))
            {
                Projectile.Center = hoverDestination;
                Projectile.netUpdate = true;
            }
            Owner.Calamity().ViridVanguardRotationToAdd = ViridVanguard.IdleCirclingSpeed * MathHelper.SmoothStep(1f, ViridVanguard.AxeCirclingSpeedMultiplier, AITimer / 150f);
            // Aim away from the target.
            Projectile.rotation = Projectile.AngleFrom(Owner.Center) + MathHelper.PiOver2;
        }

        public void DoBehavior_HorizontalSlashes(NPC target)
        {
            int hoverTime = 22;
            int chargeTime = HorizontalSlashChargeTime;

            // Exit the attack state if the blade no longer has a valid target.
            if (target is null)
            {
                ReturnToIdleState();
                return;
            }

            // Use extra updates to make the swords move faster than normal.
            Projectile.MaxUpdates = 2;

            // Use a gleam effect right before and during charges.
            float wrappedAttackTimer = AITimer % (hoverTime + chargeTime);
            float fadeIn = Utils.GetLerpValue(hoverTime - 6f, hoverTime, wrappedAttackTimer, true);
            float fadeOut = Utils.GetLerpValue(chargeTime, chargeTime - 6f, wrappedAttackTimer - hoverTime, true);
            BladeGleamInterpolant = fadeIn * fadeOut;

            // Hover to the side of the target.
            if (wrappedAttackTimer < hoverTime)
            {
                Vector2 hoverDestination = target.Center + Vector2.UnitX * (target.Center.X < Projectile.Center.X).ToDirectionInt() * 250f;
                hoverDestination.Y += (float)Math.Cos(Projectile.identity * 1.7f) * 67f;

                Projectile.Center = Vector2.Lerp(Projectile.Center, hoverDestination, 0.08f).MoveTowards(hoverDestination, 16f);
                Projectile.velocity *= 0.5f;

                // Update rotation, to look upward.
                Projectile.rotation = Projectile.rotation.AngleLerp(0f, 0.08f);
            }

            // Otherwise, if not hovering, update rotation to swing forward somewhat.
            else
                Projectile.rotation += Math.Sign(Projectile.velocity.X) * MathHelper.Pi / chargeTime * 0.36f;

            // Charge VERY quickly at the target once ready.
            if (wrappedAttackTimer == hoverTime)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.MeatySlashSound with { Pitch = 1.6f, Volume = 0.27f }, Projectile.Center);
                Projectile.oldPos = new Vector2[Projectile.oldPos.Length];
                Projectile.velocity = Vector2.UnitX * (target.Center.X > Projectile.Center.X).ToDirectionInt() * HorizontalSlashSpeed;
                Projectile.netUpdate = true;
            }

            // Switch to the next attack state once enough charges have happened.
            if (AITimer >= (hoverTime + chargeTime) * ViridVanguard.HorizontalSlashAmount)
            {
                AITimer = 0f;
                CurrentState = ViridVanguardAIState.RegularPierceSlashes;
                Projectile.netUpdate = true;
            }
        }

        public void DoBehavior_VerticalPierceTeleport(NPC target)
        {
            int hoverTime = 26;
            int chargeTime = VerticalSlashChargeTime;

            // Exit the attack state if the blade no longer has a valid target.
            if (target is null)
            {
                ReturnToIdleState();
                return;
            }

            // Use extra updates to make the swords move faster than normal.
            Projectile.MaxUpdates = 2;

            // Use a gleam effect right before and during charges.
            int localAITimer = (int)AITimer + Projectile.identity / 2;
            float wrappedAttackTimer = localAITimer % (hoverTime + chargeTime);
            float fadeIn = Utils.GetLerpValue(hoverTime - 6f, hoverTime, wrappedAttackTimer, true);
            float fadeOut = Utils.GetLerpValue(chargeTime, chargeTime - 6f, wrappedAttackTimer - hoverTime, true);
            BladeGleamInterpolant = fadeIn * fadeOut;

            // Hover above the target.
            if (wrappedAttackTimer < hoverTime)
            {
                Vector2 hoverDestination = target.Center - Vector2.UnitY * 500f;
                hoverDestination.X += (float)Math.Cos(Projectile.identity * 1.7f) * 50f;

                Projectile.Center = Vector2.Lerp(Projectile.Center, hoverDestination, 0.08f).MoveTowards(hoverDestination, 16f);
                Projectile.velocity *= 0.5f;

                // Update rotation, to look at the target.
                Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.AngleTo(target.Center) + MathHelper.PiOver2, 0.24f);
            }

            // Pierce the target once ready.
            if (wrappedAttackTimer == hoverTime)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.MeatySlashSound with { Pitch = 1.6f, Volume = 0.27f }, Projectile.Center);
                Projectile.oldPos = new Vector2[Projectile.oldPos.Length];
                Projectile.velocity = Vector2.UnitY * VerticalSlashSpeed;
                Projectile.netUpdate = true;
            }

            // Teleport above the target once sufficiently far below them.
            if (wrappedAttackTimer >= hoverTime + 5f && Projectile.Center.Y > target.Center.Y + VerticalTeleportOffset)
            {
                Projectile.oldPos = new Vector2[Projectile.oldPos.Length];
                Projectile.Center = target.Center - Vector2.UnitY * VerticalTeleportOffset * 0.7f;
                Projectile.netUpdate = true;
            }

            // Switch to the next attack state once enough charges have happened.
            if (AITimer >= (hoverTime + chargeTime) * ViridVanguard.VerticalPierceAmount)
            {
                AITimer = 0f;
                CurrentState = ViridVanguardAIState.HorizontalSlashes;
                Projectile.netUpdate = true;
            }
        }

        public void DoBehavior_RegularPierceSlashes(NPC target)
        {
            int attackCycleTime = PierceChargeAttackCycleTime;
            float upwardRiseTimeRatio = 0.4f;
            float pierceTimeRatio = 0.14f;

            // Exit the attack state if the blade no longer has a valid target.
            if (target is null)
            {
                ReturnToIdleState();
                ChargeStartingPosition = Vector2.Zero;
                return;
            }

            // Initialize the starting position on the first frame.
            if (AITimer % attackCycleTime == 1f)
            {
                ChargeStartingPosition = Projectile.Center + Main.rand.NextVector2Circular(80f, 80f);
                Projectile.netUpdate = true;
            }

            float attackCompletion = AITimer / (float)attackCycleTime % 1f;

            // Reset the trail point array if aiming at the target.
            if (attackCompletion < upwardRiseTimeRatio)
                Projectile.oldPos = new Vector2[Projectile.oldPos.Length];

            // Use extra updates to make the swords move faster than normal.
            Projectile.MaxUpdates = 2;

            // This represents the entire attack cycle in a single interpolant via a bunch of GetLerpValues and vector math. The patterns is as follows:
            // 1. Hover very quickly to the starting position.
            // 2. Rise upward somewhat.
            // 3. Lerp towards the target rapidly.
            // 4. Lerp through the target rapidly.
            // 5. Repeat.
            float offsetDistanceFactor = MathHelper.Lerp(1.61f, 3f, Projectile.identity / 7f % 1f);
            Vector2 startingPosition = ChargeStartingPosition + Vector2.UnitY * Utils.GetLerpValue(0f, upwardRiseTimeRatio, attackCompletion, true) * -200f;
            Vector2 targetOffset = target.Center - startingPosition;
            Vector2 endingPosition = target.Center + targetOffset.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(targetOffset.Length(), 60f, 240f) * offsetDistanceFactor;
            float pierceCompletion = Utils.GetLerpValue(upwardRiseTimeRatio, upwardRiseTimeRatio + pierceTimeRatio, attackCompletion, true);
            float throughTargetCompletion = Utils.GetLerpValue(upwardRiseTimeRatio + pierceTimeRatio, 1f, attackCompletion, true);

            // Update rotation and interpolate through the necessary positions.
            Projectile.rotation = Projectile.rotation.AngleTowards(targetOffset.ToRotation() + MathHelper.PiOver2, MathHelper.Pi / 5f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, Vector2.Lerp(startingPosition, target.Center, pierceCompletion), pierceCompletion * 0.5f);
            if (throughTargetCompletion > 0f)
                Projectile.Center = Vector2.Lerp(target.Center, endingPosition, throughTargetCompletion);
            Projectile.velocity = Vector2.Zero;

            // Play a slice sound once ready to charge.
            if (AITimer % attackCycleTime == (int)(attackCycleTime * upwardRiseTimeRatio))
                SoundEngine.PlaySound(CommonCalamitySounds.MeatySlashSound with { Pitch = 1.6f, Volume = 0.27f }, Projectile.Center);

            // Switch to the next attack state once enough charges have happened.
            if (AITimer >= attackCycleTime * ViridVanguard.StabAmount)
            {
                AITimer = 0f;
                CurrentState = ViridVanguardAIState.VerticalPierceTeleport;
                Projectile.netUpdate = true;
            }
        }

        public void DoBehavior_PhotonRipperZenithStartup(NPC target)
        {
            Projectile.MaxUpdates = 2;
            float completion = AITimer / (ViridVanguard.ActiveAttackStartup * (float)Projectile.MaxUpdates);
            Vector2 hoverDestination = Owner.Center + (BladeAngleForPhripperZenith * (Owner.Calamity().InvertExaltationLineRotationDirections ? -1 : 1)).ToRotationVector2() * MathHelper.Lerp(200f, 116f, completion);

            Projectile.Center = Vector2.Lerp(Projectile.Center, hoverDestination, 0.04f).MoveTowards(hoverDestination, MathHelper.Lerp(16f, 64f, completion));
            Projectile.velocity *= 0.8f;
            Owner.Calamity().ViridVanguardRotationToAdd = MathHelper.Lerp(1, ViridVanguard.ActiveAttackCirclingSpeedMultiplier, completion) * ViridVanguard.IdleCirclingSpeed;
            BladeAngleForPhripperZenith += Owner.Calamity().ViridVanguardRotationToAdd / Projectile.MaxUpdates;
            ActiveTimer = -1;
            IsInDotRange = false;

            if (completion >= 1f)
            {
                CurrentState = ViridVanguardAIState.PhotonRipperZenithSlashes;
                Projectile.netUpdate = true;
            }

            // Aim away from the target.
            Projectile.rotation = Projectile.AngleFrom(Owner.Center) + MathHelper.PiOver2;
        }
        public void DoBehavior_PhotonRipperZenithActive(NPC target)
        {
            Projectile.MaxUpdates = 2;
            Vector2 angleToMouse = Owner.DirectionTo(IsInDotRange ? StoredAttackMousePos : Owner.Calamity().mouseWorld);

            float dotResult = Vector2.Dot(angleToMouse, BladeAngleForPhripperZenith.ToRotationVector2());

            if (dotResult < -0.5f)
            {
                IsInDotRange = false;
                ActiveAttacking = false;
            }
            else if (!IsInDotRange)
            {
                IsInDotRange = true;
                StoredAttackMousePos = Owner.Calamity().mouseWorld;
                ActiveTimer--;

                if (ActiveTimer % 2 == 0)
                {
                    ActiveAttacking = true;
                    Projectile.ResetLocalNPCHitImmunity();
                }
            }
            dotResult += 1f;
            dotResult /= 2f;
            if (!ActiveAttacking)
            {
                Vector2 hoverDestination = Owner.Center + (BladeAngleForPhripperZenith * (Owner.Calamity().InvertExaltationLineRotationDirections ? -1 : 1)).ToRotationVector2() * 116f;
                Projectile.Center = hoverDestination;
                Projectile.velocity *= 0.8f;

                Projectile.rotation = Projectile.AngleFrom(Owner.Center) + MathHelper.PiOver2;
                if ((ActiveTimer < -2 * ViridVanguard.ActiveAttackSlashCount * CurrentViridCount || ActiveTimer > 0) && Projectile.FinalExtraUpdate())
                {
                    CurrentState = ViridVanguardAIState.PhotonRipperZenithEndlag;
                    ActiveTimer = ViridVanguard.ActiveAttackCooldown;
                    AITimer = 0;
                }
            }
            else
            {
                float distance = Owner.Distance(StoredAttackMousePos);
                float endSize = MathHelper.Clamp(distance * 0.25f, 164f, 300f);
                Vector2 hoverCenter = Vector2.Lerp(Owner.Center, Owner.Center + Owner.Center.DirectionTo(StoredAttackMousePos) * (distance > 164 ? distance - endSize : 0), dotResult);
                Vector2 finalHoverDestination = hoverCenter + (BladeAngleForPhripperZenith * (Owner.Calamity().InvertExaltationLineRotationDirections ? -1 : 1)).ToRotationVector2()  * MathHelper.Lerp(116f, endSize, dotResult);
                Projectile.Center = finalHoverDestination;
                Projectile.velocity *= 0.8f;
                Projectile.rotation = Projectile.AngleFrom(hoverCenter) + MathHelper.PiOver2;
            }

            Owner.Calamity().ViridVanguardRotationToAdd = ViridVanguard.ActiveAttackCirclingSpeedMultiplier * ViridVanguard.IdleCirclingSpeed;
            BladeAngleForPhripperZenith += Owner.Calamity().ViridVanguardRotationToAdd / Projectile.MaxUpdates;

        }
        public void DoBehavior_PhotonRipperZenithEndlag(NPC target)
        {

            Projectile.MaxUpdates = 1;
            float completion = 1 - AITimer / (ViridVanguard.ActiveAttackEndlag * (float)Projectile.MaxUpdates);
            if (Owner.Calamity().ViridVanguardActiveAttackerThisFrame)
            {
                AITimer = 0;
                ActiveTimer = ViridVanguard.ActiveAttackCooldown;
                completion = 1;
            }
            Vector2 hoverDestination = Owner.Center + (BladeAngleForPhripperZenith * (Owner.Calamity().InvertExaltationLineRotationDirections ? -1 : 1)).ToRotationVector2() * MathHelper.Lerp(200f, 116f, completion);
            Projectile.Center = hoverDestination;
            Projectile.velocity *= 0.8f;
            Owner.Calamity().ViridVanguardRotationToAdd = MathHelper.Lerp(1, ViridVanguard.ActiveAttackCirclingSpeedMultiplier, completion) * ViridVanguard.IdleCirclingSpeed;
            BladeAngleForPhripperZenith += Owner.Calamity().ViridVanguardRotationToAdd / Projectile.MaxUpdates;

            if (completion <= 0f)
            {
                ReturnToIdleState();
                Owner.Calamity().ViridVanguardRotation += MathHelper.WrapAngle(BladeAngleForPhripperZenith - BladeHoverOffsetAngle); //This will cause the proper rotation to be kept on completion
                Projectile.netUpdate = true;
            }

            // Aim away from the target.
            Projectile.rotation = Projectile.AngleFrom(Owner.Center) + MathHelper.PiOver2;
        }

        public void BeginSuperEpicPhotonRipperZenithKnockoffAttack()
        {

            if (CurrentState != ViridVanguardAIState.CircleOwner)
                AITimer = 0f;
            BladeAngleForPhripperZenith = BladeHoverOffsetAngle;
            CurrentState = ViridVanguardAIState.ChargingCircle;
            AITimer = 0f;
            Projectile.netUpdate = true;
        }
        public void ReturnToIdleState()
        {
            AITimer = 0f;
            Projectile.extraUpdates = 0;
            CurrentState = ViridVanguardAIState.CircleOwner;
            Projectile.netUpdate = true;
        }

        public void HandleMinionBools()
        {
            Owner.AddBuff(ModContent.BuffType<ViridVanguardBuff>(), 3600);
            if (Projectile.type == ModContent.ProjectileType<ViridVanguardBlade>())
            {
                if (Owner.dead)
                    Owner.Calamity().viridVanguard = false;

                if (Owner.Calamity().viridVanguard)
                    Projectile.timeLeft = 2;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SagePoison>(), 300);
            if (ActiveAttacking)
            {
                Vector2 vel = new Vector2(0.1f, 0.1f).RotatedByRandom(100);
                VoidSparkParticle spark2 = new VoidSparkParticle(target.Center, vel, false, 9, Main.rand.NextFloat(0.25f, 0.35f), Main.rand.NextBool() ? Color.LimeGreen : Color.Goldenrod);
                GeneralParticleHandler.SpawnParticle(spark2);
                SoundEngine.PlaySound(CommonCalamitySounds.SwiftSliceSound with { Volume = CommonCalamitySounds.SwiftSliceSound.Volume * 0.3f, MaxInstances = 0 }, Projectile.Center);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //Deals massively increased damage during the active attack
            if (ActiveAttacking)
                modifiers.SourceDamage *= ViridVanguard.ActiveAttackSlashDmgMult;
            //when not using the active, deal the right amount of damage for the swords vs axes
            else if (!ActiveAttackStateList.Contains(CurrentState))
                if (IsAxe)
                    modifiers.SourceDamage *= ViridVanguard.AxeDmgMult;
                else
                    modifiers.SourceDamage *= ViridVanguard.SwordDmgMult;
        }

        public Color TrailColorFunction(float completionRatio)
        {
            float opacity = (float)Math.Pow(Utils.GetLerpValue(1f, 0.45f, completionRatio, true), 4D) * Projectile.Opacity * (CurrentState == ViridVanguardAIState.CircleOwner ? 0.3f : 0.48f);
            return Color.Lerp(new(115, 196, 127), Color.Yellow, MathHelper.Clamp(completionRatio * 1.4f, 0f, 1f)) * opacity;
        }

        public float TrailWidthFunction(float completionRatio) => Projectile.height * (1f - completionRatio) * (DrawAsAxe ? 0.6f : 0.8f);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(2, 1, DrawAsAxe ? 0 : 1, 0);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteEffects direction = Projectile.spriteDirection == 1 ^ Owner.Calamity().InvertExaltationLineRotationDirections ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Draw the afterimage trail.
            TrailDrawer ??= new();
            GameShaders.Misc["EmpressBlade"].UseImage0("Images/Extra_201");
            GameShaders.Misc["EmpressBlade"].UseImage1("Images/Extra_193");
            GameShaders.Misc["EmpressBlade"].UseShaderSpecificData(new Vector4(1f, 0f, 0f, 0.6f));
            GameShaders.Misc["EmpressBlade"].Apply(null);
            TrailDrawer.PrepareStrip(Projectile.oldPos, Projectile.oldRot, TrailColorFunction, TrailWidthFunction, Projectile.Size * 0.5f - Main.screenPosition, Projectile.oldPos.Length, true);
            TrailDrawer.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            // Draw the blade.
            float outlineOpacity = 1;
            float outlineWidth = 1;
            Color outlineColor = Color.LimeGreen;
            if (!(ActiveAttackStateList.Contains(CurrentState) && ActiveTimer > 0))
            {
                outlineWidth = Math.Clamp(MathF.Pow(1 - ActiveTimer / ViridVanguard.ActiveAttackCooldown, 3), 0, 1);
                outlineOpacity = 0.75f;
            }
            var outlineTex = ViridVanguard.GetBladeOutlineTex();
            var rotation = Projectile.rotation + (DrawAsAxe ^ Owner.Calamity().InvertExaltationLineRotationDirections ? -0.2f : 0.2f);
            Main.EntitySpriteDraw(outlineTex, drawPosition + new Vector2(2, 0) * outlineWidth, frame, outlineColor * outlineOpacity, rotation, origin, Projectile.scale, direction, 0);
            Main.EntitySpriteDraw(outlineTex, drawPosition + new Vector2(0, 2) * outlineWidth, frame, outlineColor * outlineOpacity, rotation, origin, Projectile.scale, direction, 0);
            Main.EntitySpriteDraw(outlineTex, drawPosition + new Vector2(-2, 0) * outlineWidth, frame, outlineColor * outlineOpacity, rotation, origin, Projectile.scale, direction, 0);
            Main.EntitySpriteDraw(outlineTex, drawPosition + new Vector2(0, -2) * outlineWidth, frame, outlineColor * outlineOpacity, rotation, origin, Projectile.scale, direction, 0);
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, direction, 0);

            // Draw the gleam at the tip of the blade.
            if (ActiveTimer <= 0)
            {
                Texture2D shineTex = ModContent.Request<Texture2D>("CalamityMod/Particles/HalfStar").Value;
                Vector2 shineScale = new Vector2(1.67f, 3f) * Projectile.scale;
                shineScale *= MathHelper.Lerp(0.9f, 1.1f, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 7.4f + Projectile.identity) * 0.5f + 0.5f);

                Vector2 lensFlareWorldPosition = Projectile.Center + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * Projectile.width * Projectile.scale * 0.88f;
                Color lensFlareColor = Color.Lerp(Color.LimeGreen, Color.Yellow, 0.23f) with { A = 0 } * BladeGleamInterpolant;
                Main.EntitySpriteDraw(shineTex, lensFlareWorldPosition - Main.screenPosition, null, lensFlareColor, 0f, shineTex.Size() * 0.5f, shineScale * 0.6f, 0, 0);
                Main.EntitySpriteDraw(shineTex, lensFlareWorldPosition - Main.screenPosition, null, lensFlareColor, MathHelper.PiOver2, shineTex.Size() * 0.5f, shineScale, 0, 0);
            }
            // Reset textures for shaders, since they're only defined once at load-time in vanilla.
            GameShaders.Misc["EmpressBlade"].UseImage0("Images/Extra_209");
            GameShaders.Misc["EmpressBlade"].UseImage1("Images/Extra_210");
            return false;
        }
    }
}
