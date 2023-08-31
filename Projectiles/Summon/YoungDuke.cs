using System;
using System.IO;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class YoungDuke : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer ModdedOwner => Owner.Calamity();
        public NPC Target => Projectile.Center.MinionHoming(MutatedTruffle.EnemyDistanceDetection, Owner);
        public ref float AITimer => ref Projectile.ai[0];
        public Vector2 VortexPosition;

        public enum AIState
        {
            Idle,
            Dashing,
            Toothball,
            Vortex
        }
        public AIState State
        {
            get => (AIState)Projectile.ai[2];
            set => Projectile.ai[2] = (int)value;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 16;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.minionSlots = 3;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = -1;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = 82;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WritePackedVector2(VortexPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            VortexPosition = reader.ReadPackedVector2();
        }

        public override void AI()
        {
            CheckMinionExistence();
            DoAnimation();
            DoRotation();

            switch (State)
            {
                case AIState.Idle:
                    IdleState();
                    break;
                case AIState.Dashing:
                    DashingState();
                    break;
                case AIState.Toothball:
                    ToothballState();
                    break;
                case AIState.Vortex:
                    VortexState();
                    break;
            }
        }

        #region AI Methods

        private void CheckMinionExistence()
        {
            Owner.AddBuff(ModContent.BuffType<YoungDukeBuff>(), 2);
            if (Type != ModContent.ProjectileType<YoungDuke>())
                return;

            if (Owner.dead)
                ModdedOwner.youngDuke = false;
            if (ModdedOwner.youngDuke)
                Projectile.timeLeft = 2;
        }

        private void DoAnimation()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 4 == 0)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            }
        }

        private void DoRotation()
        {
            // While idle, the minion will just look left or right.
            if (State == AIState.Idle)
            {
                Projectile.direction = MathF.Sign(Projectile.velocity.X);
                Projectile.rotation = 0f;
            }

            // While doing any of these states, the minion looks at the direction it's heading towards.
            else if (State == AIState.Dashing || State == AIState.Vortex)
            {
                Projectile.direction = 1;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            // And while shooting at a target, look at the target.
            else if (State == AIState.Toothball)
            {
                Projectile.direction = MathF.Sign(Target.Center.X - Projectile.Center.X);
                Projectile.rotation = Projectile.DirectionTo(Target.Center).ToRotation();
            }
        }

        private void IdleState()
        {
            // If within a short range of the player, just hover around him.
            if (!Projectile.WithinRange(Owner.Center, 240f))
            {
                Projectile.velocity = (Projectile.velocity + Projectile.SafeDirectionTo(Owner.Center)) * 0.9f;
                Projectile.netUpdate = true;
            }

            // Follow the player if they're at a medium range, to catch up.
            if (!Projectile.WithinRange(Owner.Center, 400f))
            {
                Projectile.velocity = Projectile.DirectionTo(Owner.Center) * MathF.Max(Owner.velocity.Length(), 10f);
                Projectile.netUpdate = true;
            }

            // And when sufficiently far away, teleport to the player.
            if (!Projectile.WithinRange(Owner.Center, MutatedTruffle.EnemyDistanceDetection))
            {
                Projectile.Center = Owner.Center;
                Projectile.netUpdate = true;
            }

            if (Target is not null)
                SwitchState(AIState.Dashing);
        }

        private void DashingState()
        {
            if (Target is not null)
            {
                AITimer++;

                if (!Projectile.WithinRange(Target.Center, 480f) || Projectile.velocity.Length() < MutatedTruffle.DashSpeed - 10f)
                {
                    Projectile.velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, Target, MutatedTruffle.DashSpeed);
                    Projectile.netUpdate = true;
                }

                if (AITimer > MutatedTruffle.DashTime)
                    SwitchState(AIState.Toothball);
            }
            else
                SwitchState(AIState.Idle);
        }

        private void ToothballState()
        {
            if (Target is not null)
            {
                Vector2 spinPosition = Target.Center + -Vector2.UnitY.RotatedBy(Main.GlobalTimeWrappedHourly * 2f) * 400f;
                Projectile.velocity = (Projectile.velocity * 20f + Projectile.SafeDirectionTo(spinPosition) * 40f) / 21f;

                if (AITimer % MutatedTruffle.ToothballFireRate == 0f && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, Target, 30f), ModContent.ProjectileType<YoungDukeToothball>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                    Projectile.netUpdate = true;
                }

                if (AITimer > MutatedTruffle.ToothballsUntilNextState * MutatedTruffle.ToothballFireRate)
                    SwitchState(AIState.Vortex);

                AITimer++;
            }
            else
                SwitchState(AIState.Idle);
        }

        private void VortexState()
        {
            if (Target is not null)
            {
                AITimer++;

                Vector2 spinPosition = VortexPosition + -Vector2.UnitY.RotatedBy(Main.GlobalTimeWrappedHourly * 5f) * 400f;
                Projectile.velocity = (Projectile.velocity * 20f + Projectile.SafeDirectionTo(spinPosition) * 40f) / 21f;

                if (AITimer >= MutatedTruffle.VortexTimeUntilNextState)
                    SwitchState(AIState.Dashing);
            }
            else
                SwitchState(AIState.Idle);
        }

        private void SwitchState(AIState state)
        {
            State = state;
            AITimer = 0f;

            if (state == AIState.Vortex)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Target.Center, Vector2.Zero, ModContent.ProjectileType<YoungDukeVortex>(), Projectile.damage * 10, Projectile.knockBack, Projectile.whoAmI);
                VortexPosition = Target.Center;
            }

            Projectile.netUpdate = true;
        }

        #endregion

        public override bool? CanDamage() => (State == AIState.Dashing) ? null : false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;

            if (CalamityConfig.Instance.Afterimages && State == AIState.Dashing)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Color afterimageDrawColor = Color.Green with { A = 25 } * Projectile.Opacity * (1f - i / (float)Projectile.oldPos.Length);
                    Vector2 afterimageDrawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                    Main.EntitySpriteDraw(texture, afterimageDrawPosition, frame, afterimageDrawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
