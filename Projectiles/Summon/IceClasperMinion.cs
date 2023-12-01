using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class IceClasperMinion : BaseMinionProjectile
    {
        public override int AssociatedProjectileTypeID => ModContent.ProjectileType<IceClasperMinion>();
        public override int AssociatedBuffTypeID => ModContent.BuffType<IceClasperBuff>();
        public override ref bool AssociatedMinionBool => ref ModdedOwner.IceClasperBool;

        public enum AIState { Follow, Ram }
        public AIState State
        {
            get => (AIState)Projectile.ai[0];
            set
            {
                Projectile.ai[0] = (int)value;
                SyncVariables();
            }
        }

        public ref float TimerForShooting => ref Projectile.ai[1];

        public ref float AfterimageInterpolant => ref Projectile.localAI[0];

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.coldDamage = true;
            Projectile.width = Projectile.height = 62;
        }

        public override void MinionAI()
        {
            switch (State)
            {
                case AIState.Follow:
                    FollowState();
                    break;
                case AIState.Ram:
                    RamState();
                    break;
            }

            Projectile.MinionAntiClump(0.5f);

            if (!Main.dedServ)
            {
                if (Main.rand.NextBool(10))
                {
                    Dust ghostDust = Dust.NewDustPerfect(Projectile.Center, 56, -Projectile.rotation.ToRotationVector2().RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(2f, 3f));
                    ghostDust.customData = false;
                    ghostDust.noLight = true;
                    ghostDust.noLightEmittence = true;
                }

                Lighting.AddLight(Projectile.Center, Color.Cyan.ToVector3());
            }
        }

        #region AI Methods

        public void FollowState()
        {
            // If the minion starts to get far, force the minion to go to you.
            if (!Projectile.WithinRange(Owner.Center, AncientIceChunk.MaxDistanceFromOwner))
            {
                Projectile.velocity = (Projectile.velocity + Projectile.SafeDirectionTo(Owner.Center)) * 0.9f;
                SyncVariables();
            }

            // Teleport to the owner if sufficiently far away.
            else if (!Projectile.WithinRange(Owner.Center, 1200f))
            {
                Projectile.Center = Owner.Center;
                SyncVariables();
            }

            // If the target is not null but not in range to dash: shoot.
            // If in range to dash: dash.
            if (Target != null)
            {
                if (Owner.WithinRange(Target.Center, AncientIceChunk.DistanceToDash))
                    State = AIState.Ram;
                else
                    ShootTarget();

                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(Target.Center), .15f);
            }
            else
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.velocity.ToRotation(), .15f);
        }

        public void RamState()
        {
            if (Target is not null && Owner.WithinRange(Projectile.Center, AncientIceChunk.DistanceToStopDash))
            {
                // The distance to the target plus a small number so it's not 0, it'd break calculations.
                float distanceToTarget = Projectile.Distance(Target.Center) + .01f;

                // The minion will head towards it's rotation.
                // If the target's close, the minion'll speed up, and viceversa, so it doesn't circle around the target doing nothing.
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * (AncientIceChunk.MinVelocity + (12f / (distanceToTarget * .01f)));
                Projectile.velocity = Vector2.Clamp(Projectile.velocity, Vector2.One * -25f, Vector2.One * 25f);
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(Target.Center), .001f * distanceToTarget);
            }

            // If there's no target while dashing or the player's gone far enough from the target: back to shooting.
            else
                State = AIState.Follow;
        }

        public void ShootTarget()
        {
            ++TimerForShooting;
            if (TimerForShooting >= AncientIceChunk.TimeToShoot && Projectile.owner == Main.myPlayer)
            {
                Vector2 velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, Target, 25f);

                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    velocity,
                    ModContent.ProjectileType<IceClasperSummonProjectile>(),
                    (int)(Projectile.damage * AncientIceChunk.ProjectileDMGMultiplier),
                    Projectile.knockBack,
                    Projectile.owner);

                // Flavor recoil effect.
                Projectile.velocity -= velocity * .1f;

                if (!Main.dedServ)
                {
                    // For the fucking love of any god you can think of, this sound sucks but I can't find another one that fits better.
                    SoundEngine.PlaySound(SoundID.Item28, Projectile.Center);
                }

                TimerForShooting = 0f;
                SyncVariables();
            }
        }

        public void SyncVariables()
        {
            Projectile.netUpdate = true;
            if (Projectile.netSpam >= 10)
                Projectile.netSpam = 9;
        }

        #endregion

        public override void OnSpawn(IEntitySource source)
        {
            IFrames = AncientIceChunk.IFrames;
            TrailingMode = 2;
            TrailCacheLength = 6;

            if (!Main.dedServ)
            {
                int dustAmount = 45;
                for (int dustIndex = 0; dustIndex < dustAmount; dustIndex++)
                {
                    float angle = MathHelper.TwoPi / dustAmount * dustIndex;
                    Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(3f, 7f);
                    Dust spawnDust = Dust.NewDustPerfect(Projectile.Center, 56, velocity);
                    spawnDust.customData = false;
                    spawnDust.noGravity = true;
                    spawnDust.velocity *= .75f;
                    spawnDust.scale = velocity.Length() * .2f;
                }
            }
        }

        public override bool? CanDamage() => (State == AIState.Ram) ? null : false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;

            // Flavor fade-in-and-out for afterimages.
            AfterimageInterpolant += ((Target is not null && Owner.WithinRange(Target.Center, 450f)) || State == AIState.Ram) ? .05f : -.05f;
            AfterimageInterpolant = MathHelper.Clamp(AfterimageInterpolant, 0f, 1f);
            float AfterimageFade = MathHelper.Lerp(0f, 1f, AfterimageInterpolant);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Color afterimageDrawColor = new Color(0.05f, 0.33f, 0.63f) with { A = 25 } * Projectile.Opacity * (1f - i / (float)Projectile.oldPos.Length) * AfterimageFade;
                    Vector2 afterimageDrawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                    Main.EntitySpriteDraw(texture, afterimageDrawPosition, frame, afterimageDrawColor, Projectile.rotation - MathHelper.PiOver2, origin, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation - MathHelper.PiOver2, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
