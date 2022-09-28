using System;
using System.IO;
using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Weapons.Summon;
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
    public class ViridVanguardBlade : ModProjectile
    {
        public enum ViridVanguardAIState
        {
            CircleOwner,
            HorizontalSlashes,
            VerticalPierceTeleport,
            RegularPierceSlashes,
        }

        public int BladeIndex;

        public VertexStrip TrailDrawer;

        public Vector2 ChargeStartingPosition;

        public float BladeHoverOffsetAngle
        {
            get
            {
                float projectileCounts = Owner.ownedProjectileCounts[Type];
                if (projectileCounts <= 1f)
                    projectileCounts = 1f;

                return MathHelper.TwoPi * BladeIndex / projectileCounts + AITimer / 27f;
            }
        }

        public ViridVanguardAIState CurrentState
        {
            get => (ViridVanguardAIState)Projectile.ai[0];
            set => Projectile.ai[0] = (int)value;
        }

        public Player Owner => Main.player[Projectile.owner];

        public ref float AITimer => ref Projectile.ai[1];

        public ref float BladeGleamInterpolant => ref Projectile.localAI[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vanguard Blade");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 45;
        }

        public override void SetDefaults()
        {
            Projectile.width = 84;
            Projectile.height = 84;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.tileCollide = false;
            Projectile.minion = true;
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

            NPC potentialTarget = Projectile.Center.MinionHoming(ViridVanguard.MaxTargetingDistance, Owner);
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
            }

            AITimer++;
        }

        public void DoBehavior_CircleOwner(NPC potentialTarget)
        {
            // Begin attacking if an enemy is nearby.
            if (potentialTarget is not null)
            {
                CurrentState = ViridVanguardAIState.HorizontalSlashes;
                AITimer = 0f;
                Projectile.netUpdate = true;
                return;
            }

            Vector2 hoverDestination = Owner.Center + BladeHoverOffsetAngle.ToRotationVector2() * 200f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, hoverDestination, 0.04f).MoveTowards(hoverDestination, 16f);
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

        public void DoBehavior_HorizontalSlashes(NPC target)
        {
            int hoverTime = 22;
            int chargeTime = 14;
            int chargeCount = 7;
            float chargeSpeed = 44f;

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
                Projectile.velocity = Vector2.UnitX * (target.Center.X > Projectile.Center.X).ToDirectionInt() * chargeSpeed;
                Projectile.netUpdate = true;
            }

            // Switch to the next attack state once enough charges have happened.
            if (AITimer >= (hoverTime + chargeTime) * chargeCount)
            {
                AITimer = 0f;
                CurrentState = ViridVanguardAIState.VerticalPierceTeleport;
                Projectile.netUpdate = true;
            }
        }

        public void DoBehavior_VerticalPierceTeleport(NPC target)
        {
            int hoverTime = 26;
            int chargeTime = 32;
            int chargeCount = 7;
            float chargeSpeed = 45f;

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
                Projectile.velocity = Vector2.UnitY * chargeSpeed;
                Projectile.netUpdate = true;
            }

            // Teleport above the target once sufficiently far below them.
            if (wrappedAttackTimer >= hoverTime + 5f && Projectile.Center.Y > target.Center.Y + 850f)
            {
                Projectile.oldPos = new Vector2[Projectile.oldPos.Length];
                Projectile.Center = target.Center - Vector2.UnitY * 600f;
                Projectile.netUpdate = true;
            }

            // Switch to the next attack state once enough charges have happened.
            if (AITimer >= (hoverTime + chargeTime) * chargeCount)
            {
                AITimer = 0f;
                CurrentState = ViridVanguardAIState.RegularPierceSlashes;
                Projectile.netUpdate = true;
            }
        }

        public void DoBehavior_RegularPierceSlashes(NPC target)
        {
            int attackCycleTime = 44;
            int chargeCount = 8;
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
            int localAITimer = (int)AITimer + Projectile.identity;
            if (localAITimer % attackCycleTime == 1f)
            {
                ChargeStartingPosition = Projectile.Center + Main.rand.NextVector2Circular(80f, 80f);
                Projectile.netUpdate = true;
            }

            float attackCompletion = localAITimer / (float)attackCycleTime % 1f;

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
            if (localAITimer % attackCycleTime == (int)(attackCycleTime * upwardRiseTimeRatio))
                SoundEngine.PlaySound(CommonCalamitySounds.MeatySlashSound with { Pitch = 1.6f, Volume = 0.27f }, Projectile.Center);

            // Switch to the next attack state once enough charges have happened.
            if (localAITimer >= attackCycleTime * chargeCount)
            {
                AITimer = 0f;
                CurrentState = ViridVanguardAIState.HorizontalSlashes;
                Projectile.netUpdate = true;
            }
        }
        
        public void ReturnToIdleState()
        {
            AITimer = 0f;
            CurrentState = ViridVanguardAIState.CircleOwner;
            Projectile.netUpdate = true;
        }

        public void HandleMinionBools()
        {
            Owner.AddBuff(ModContent.BuffType<ViridVanguardBuff>(), 3600);
            if (Projectile.type == ModContent.ProjectileType<ViridVanguardBlade>())
            {
                if (Owner.dead)
                    Owner.Calamity().ViridVanguard = false;

                if (Owner.Calamity().ViridVanguard)
                    Projectile.timeLeft = 2;
            }
        }

        public Color TrailColorFunction(float completionRatio)
        {
            float opacity = (float)Math.Pow(Utils.GetLerpValue(1f, 0.45f, completionRatio, true), 4D) * Projectile.Opacity;
            return Color.Lerp(new(115, 196, 127), Color.Yellow, MathHelper.Clamp(completionRatio * 1.4f, 0f, 1f)) * opacity;
        }

        public float TrailWidthFunction(float completionRatio) => Projectile.height * (1f - completionRatio) * 0.8f;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            TrailDrawer ??= new();
            GameShaders.Misc["EmpressBlade"].UseImage0("Images/Extra_201");
            GameShaders.Misc["EmpressBlade"].UseImage1("Images/Extra_193");
            GameShaders.Misc["EmpressBlade"].UseShaderSpecificData(new Vector4(1f, 0f, 0f, 0.6f));
            GameShaders.Misc["EmpressBlade"].Apply(null);
            TrailDrawer.PrepareStrip(Projectile.oldPos, Projectile.oldRot, TrailColorFunction, TrailWidthFunction, Projectile.Size * 0.5f - Main.screenPosition, Projectile.oldPos.Length, true);
            TrailDrawer.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            // Draw the blade.
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation + 0.2f, origin, Projectile.scale, direction, 0);

            // Draw the gleam.
            Texture2D shineTex = ModContent.Request<Texture2D>("CalamityMod/Particles/HalfStar").Value;
            Vector2 shineScale = new Vector2(1.67f, 3f) * Projectile.scale;
            shineScale *= MathHelper.Lerp(0.9f, 1.1f, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 7.4f + Projectile.identity) * 0.5f + 0.5f);

            Vector2 lensFlareWorldPosition = Projectile.Center + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * Projectile.width * Projectile.scale * 0.88f;
            Color lensFlareColor = Color.Lerp(Color.LimeGreen, Color.Yellow, 0.23f) with { A = 0 } * BladeGleamInterpolant;
            Main.EntitySpriteDraw(shineTex, lensFlareWorldPosition - Main.screenPosition, null, lensFlareColor, 0f, shineTex.Size() * 0.5f, shineScale * 0.6f, 0, 0);
            Main.EntitySpriteDraw(shineTex, lensFlareWorldPosition - Main.screenPosition, null, lensFlareColor, MathHelper.PiOver2, shineTex.Size() * 0.5f, shineScale, 0, 0);

            // Reset textures for shaders.
            GameShaders.Misc["EmpressBlade"].UseImage0("Images/Extra_209");
            GameShaders.Misc["EmpressBlade"].UseImage1("Images/Extra_210");
            return false;
        }
    }
}
