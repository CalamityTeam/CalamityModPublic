using System;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class SnakeEyesSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";

        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer ModdedOwner => Owner.Calamity();
        public NPC Target => Owner.Center.MinionHoming(SnakeEyes.EnemyDistanceDetection, Owner);

        public enum AIState
        {
            Idle,
            Targetting,
            Redirecting
        }
        public AIState State
        {
            get => (AIState)Projectile.ai[0];
            set => Projectile.ai[0] = (int)value;
        }
        public ref float AITimer => ref Projectile.ai[1];
        public ref float EyeAngle => ref Projectile.localAI[0];
        public ref float EyeOutwardness => ref Projectile.localAI[1];

        public bool HasShot = false;
        public bool HasTeleported = false;
        public Vector2 RandomPosition;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.width = Projectile.height = 22;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            // Checks if the minion can still exist given the player's circumstance.
            CheckMinionExistence();

            // The properties of the minion's pupil, it looks around the target if there's one and to the mouse if there isn't any.
            EyeAngle = Projectile.SafeDirectionTo((Target is not null) ? Target.Center : Main.MouseWorld).ToRotation();
            EyeOutwardness = Utils.Remap(Projectile.Distance((Target is not null) ? Target.Center : Main.MouseWorld), 0f, 300f, 0f, 5f);

            // Random flavor pulse.
            if (Main.rand.NextBool(100))
            {
                Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.DarkCyan, Vector2.One, 0f, 0.05f, 0.4f + Main.rand.NextFloat(0.2f), 30);
                GeneralParticleHandler.SpawnParticle(pulse);
            }

            switch (State)
            {
                case AIState.Idle:
                    IdleState();
                    break;
                case AIState.Targetting:
                    TargettingState();
                    break;
                case AIState.Redirecting:
                    RedirectingState();
                    break;
            }
        }

        #region AI Methods

        private void IdleState()
        {
            AITimer += .01f;

            // The projectile oscillates in place.
            Projectile.velocity.Y -= MathF.Cos(AITimer) / 350f;

            // If the player's far enough from the minion, he'll teleport somewhere around the player.
            if (!Projectile.WithinRange(Owner.Center, 400f))
            {
                Projectile.Center = Owner.Center + Main.rand.NextVector2Circular(300f, 300f);
                DoVFXPulse();
            }

            if (Target is not null)
                SwitchAIState(AIState.Targetting);
        }

        private void TargettingState()
        {
            if (Target is not null)
            {
                // The minion decides a random position around the target.
                if (!HasTeleported)
                {
                    RandomPosition = Target.Center + Main.rand.NextVector2CircularEdge(400f, 400f);
                    Projectile.Center = RandomPosition;

                    DoVFXPulse();

                    HasTeleported = true;
                    Projectile.netUpdate = true;
                }

                else
                {
                    // The projectile will stay on that position.
                    Projectile.Center = RandomPosition;

                    // When the delay is over, shoot at the target.
                    if (!HasShot && AITimer >= SnakeEyes.TimeToShoot)
                        ShootProjectile();

                    // When the minion has shot, when the projectile redirects towards the target,
                    // the minion will teleport on that moment on top of the projectile,
                    // so it looks like the minion deflected his own projectile.
                    if (HasShot)
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile proj = Main.projectile[i];
                            if (proj is not null && proj.active && proj.type == ModContent.ProjectileType<SnakeEyesProjectile>() && proj.owner == Owner.whoAmI && proj.ModProjectile<SnakeEyesProjectile>().MinionID == Projectile.whoAmI && proj.ModProjectile<SnakeEyesProjectile>().HasRedirected)
                            {
                                Projectile.Center = proj.Center;
                                SwitchAIState(AIState.Redirecting);
                            }
                        }
                    }

                    // If the projectile failed to hit it's enemy and therefore couldn't deflect,
                    // repeat this process.
                    if (AITimer >= SnakeEyes.TimeToShoot + 120f)
                        SwitchAIState(AIState.Targetting);

                    AITimer++;
                }
            }
            else
                SwitchAIState(AIState.Idle);
        }

        private void ShootProjectile()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, Target, SnakeEyes.ProjectileSpeed), ModContent.ProjectileType<SnakeEyesProjectile>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, Projectile.whoAmI, Target.whoAmI);

                DoVFXPulse();

                SoundEngine.PlaySound(SoundID.Item91 with { Volume = .8f, Pitch = .5f, PitchVariance = .1f }, Projectile.Center);

                HasShot = true;
                Projectile.netUpdate = true;
            }
        }

        private void RedirectingState()
        {
            if (Target is not null)
            {
                // The projectile will idle on the spot where he "deflected" the projectile to restart the process.
                if (AITimer < SnakeEyes.TimeToRestart)
                    AITimer++;
                else
                    SwitchAIState(AIState.Targetting);
            }
            else
                SwitchAIState(AIState.Idle);
        }

        private void SwitchAIState(AIState state)
        {
            State = state;
            AITimer = 0f;
            HasTeleported = false;
            HasShot = false;
            RandomPosition = Vector2.Zero;

            Projectile.velocity = Vector2.Zero;

            DoVFXPulse(state == AIState.Redirecting);

            Projectile.netUpdate = true;
        }

        private void CheckMinionExistence()
        {
            Owner.AddBuff(ModContent.BuffType<SnakeEyesBuff>(), 2);
            if (Type != ModContent.ProjectileType<SnakeEyesSummon>())
                return;

            if (Owner.dead)
                ModdedOwner.snakeEyes = false;
            if (ModdedOwner.snakeEyes)
                Projectile.timeLeft = 2;
        }

        private void DoVFXPulse(bool empowered = false)
        {
            int dustAmount = 40;
            for (int dustIndex = 0; dustIndex < dustAmount; dustIndex++)
            {
                float angle = MathHelper.TwoPi / dustAmount * dustIndex;
                Vector2 velocity = angle.ToRotationVector2() * 9f;
                Dust pulseDust = Dust.NewDustPerfect(Projectile.Center, empowered ? 261 : 226, velocity, Scale: empowered ? 2f : .5f);
                pulseDust.noGravity = true;
            }

            Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, empowered ? Color.White : Color.DarkCyan * 1.2f, Vector2.One, 0f, 0.05f, 0.5f + Main.rand.NextFloat(0.2f), 20);
            GeneralParticleHandler.SpawnParticle(pulse);
        }

        #endregion

        public override void OnSpawn(IEntitySource source) => DoVFXPulse();

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Texture2D eyeTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/SnakeEye").Value;
            Vector2 eyeDrawPosition = Projectile.Center - Main.screenPosition + EyeAngle.ToRotationVector2() * EyeOutwardness;
            Main.EntitySpriteDraw(eyeTexture, eyeDrawPosition, null, Color.White, 0f, eyeTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);

            return false;
        }
    }
}
