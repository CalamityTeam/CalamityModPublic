using System;
using System.IO;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PlantationStaffSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer ModdedOwner => Owner.Calamity();
        public NPC Target => Owner.Center.MinionHoming(PlantationStaff.EnemyDistanceDetection, Owner);

        public enum AIState
        {
            Idle,
            Thornball,
            Seeds,
            Ramming
        }
        public AIState State
        {
            get => (AIState)Projectile.ai[0];
            set => Projectile.ai[0] = (int)value;
        }
        public ref float AITimer => ref Projectile.ai[1];
        public ref float ShootSeedsTimer => ref Projectile.ai[2];
        public ref float SeedBurstsShot => ref Projectile.localAI[0];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 3f;
            Projectile.localNPCHitCooldown = 10;
            Projectile.width = Projectile.height = 48;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.netImportant = true;
        }

        #region Variable Syncing

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(SeedBurstsShot);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SeedBurstsShot = reader.ReadSingle();
        }

        #endregion

        public override void AI()
        {
            CheckMinionExistence();
            DoAnimation();

            switch (State)
            {
                case AIState.Idle:
                    IdleState();
                    break;
                case AIState.Thornball:
                    ThornballState();
                    break;
                case AIState.Seeds:
                    SeedsState();
                    break;
                case AIState.Ramming:
                    RammingState();
                    break;
            }
        }

        #region AI Methods

        private void IdleState()
        {
            // The projectile points towards its direction.
            Projectile.rotation = Projectile.velocity.ToRotation();

            // The minion will hover around the owner at a certain distance.
            if (!Projectile.WithinRange(Owner.Center, 160f))
            {
                Projectile.velocity = (Projectile.velocity + Projectile.SafeDirectionTo(Owner.Center)) * 0.9f;
                Projectile.netUpdate = true;
            }

            // If the owner starts to get far, the minion will follow straight to the owner.
            if (!Projectile.WithinRange(Owner.Center, 320f))
            {
                Projectile.velocity = Projectile.SafeDirectionTo(Owner.Center) * MathF.Max(5f, Owner.velocity.Length());
                Projectile.netUpdate = true;
            }

            // If the owner's far enough, teleport the minion on the owner.
            if (!Projectile.WithinRange(Owner.Center, PlantationStaff.EnemyDistanceDetection))
            {
                Projectile.Center = Owner.Center;
                Projectile.netUpdate = true;
            }

            if (Target is not null)
                SwitchState(AIState.Thornball);
        }

        private void ThornballState()
        {
            if (Target is not null)
            {
                Vector2 targetDirection = Projectile.SafeDirectionTo(Target.Center);

                ShootingMovement();

                AITimer++;
                if (AITimer % PlantationStaff.ThornballFireRate == 0f && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, targetDirection * PlantationStaff.ThornballSpeed, ModContent.ProjectileType<PlantationStaffThornball>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);

                    // Flavor recoil effect.
                    Projectile.velocity -= targetDirection * 3f;

                    ShootEffect();

                    SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);

                    Projectile.netUpdate = true;
                }

                if (AITimer >= PlantationStaff.ThornballAmount * PlantationStaff.ThornballFireRate)
                    SwitchState(AIState.Seeds);
            }
            else
                SwitchState(AIState.Idle);
        }

        private void SeedsState()
        {
            if (Target is not null)
            {
                ShootingMovement();

                AITimer++;
                if (AITimer >= PlantationStaff.SeedBurstDelay)
                {
                    ShootSeedsTimer++;

                    if (ShootSeedsTimer % PlantationStaff.SeedBetweenBurstDelay == 0f && Main.myPlayer == Projectile.owner)
                    {
                        Vector2 velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, Target, PlantationStaff.SeedSpeed);

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<PlantationStaffSeed>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, Main.rand.Next(2));

                        // Flavor recoil effect.
                        Projectile.velocity -= velocity.SafeNormalize(Vector2.Zero) * 2f;

                        ShootEffect();

                        SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);

                        Projectile.netUpdate = true;
                    }

                    if (ShootSeedsTimer >= PlantationStaff.SeedBetweenBurstDelay * PlantationStaff.SeedAmountPerBurst)
                    {
                        AITimer = 0f;
                        ShootSeedsTimer = 0f;
                        SeedBurstsShot++;
                        Projectile.netUpdate = true;
                    }
                }

                if (SeedBurstsShot >= PlantationStaff.SeedBurstAmount)
                    SwitchState(AIState.Ramming);
            }
            else
                SwitchState(AIState.Idle);
        }

        private void RammingState()
        {
            if (Target is not null)
            {
                AITimer++;
                if (AITimer <= PlantationStaff.TimeBeforeRamming)
                {
                    // The minion slows down to give a flavor effect of preparation.
                    Projectile.velocity *= 0.985f;
                }
                else
                {
                    // If the minion's not withing a certain range of the target, go towards it.
                    // When it reaches that range, the velocity will not change anymore until they hit
                    // the outside range, repeating this process and giving the effect of ramming.
                    if (!Projectile.WithinRange(Target.Center, 400f))
                        RamMovement();

                    // But, if the minion was already inside the target when it appeared,
                    // meaning it doesn't have the velocity to go to the outside range.
                    // We'll make it ram once to be start the effect of ramming,
                    // with an amount of grace so the minion doesn't change it's velocity constantly.
                    else if (Projectile.velocity.Length() < PlantationStaff.ChargingSpeed - 5f)
                        RamMovement();

                    if (AITimer >= PlantationStaff.RamTime + PlantationStaff.TimeBeforeRamming)
                        SwitchState(AIState.Thornball);
                }
            }
            else
                SwitchState(AIState.Idle);
        }

        private void ShootingMovement()
        {
            Vector2 targetDirection = Projectile.SafeDirectionTo(Target.Center);

            // The minion will look towards the target.
            Projectile.rotation = targetDirection.ToRotation();

            // If the minion is not within a certain range of the target, go towards it.
            if (!Projectile.WithinRange(Target.Center, 480f))
            {
                Projectile.velocity = (Projectile.velocity * 30f + targetDirection * PlantationStaff.ChargingSpeed) / 31f;
                Projectile.netUpdate = true;
            }

            // But if the minion gets to close, separate it from the target.
            if (Projectile.WithinRange(Target.Center, 400f))
            {
                Projectile.velocity = (Projectile.velocity * 30f + -targetDirection * PlantationStaff.ChargingSpeed) / 31f;
                Projectile.netUpdate = true;
            }
        }

        private void RamMovement()
        {
            Projectile.velocity = Projectile.SafeDirectionTo(Target.Center) * PlantationStaff.ChargingSpeed;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.netUpdate = true;
        }

        private void SwitchState(AIState state)
        {
            State = state;
            AITimer = 0f;
            ShootSeedsTimer = 0f;
            SeedBurstsShot = 0f;

            if (state == AIState.Ramming)
            {
                int sporeCloudAmount = 12;
                for (int sporeCloudIndex = 0; sporeCloudIndex < sporeCloudAmount; sporeCloudIndex++)
                {
                    float angle = MathHelper.TwoPi / sporeCloudAmount * sporeCloudIndex;
                    Vector2 velocity = angle.ToRotationVector2() * PlantationStaff.SporeStartVelocity;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<PlantationStaffSporeCloud>(), Projectile.damage, 10f, Owner.whoAmI, Main.rand.Next(3));
                }

                int tentacleAmount = 6;
                for (int tentacleIndex = 0; tentacleIndex < tentacleAmount; tentacleIndex++)
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PlantationStaffTentacle>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, tentacleIndex, Projectile.whoAmI);

                SoundEngine.PlaySound(SoundID.Roar with { Volume = .3f, Pitch = 1f, PitchVariance = .1f }, Projectile.Center);
            }

            Projectile.netUpdate = true;
        }

        private void CheckMinionExistence()
        {
            Owner.AddBuff(ModContent.BuffType<PlantationStaffBuff>(), 2);
            if (Type != ModContent.ProjectileType<PlantationStaffSummon>())
                return;

            if (Owner.dead)
                ModdedOwner.PlantationSummon = false;
            if (ModdedOwner.PlantationSummon)
                Projectile.timeLeft = 2;
        }

        private void DoAnimation()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;

                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];

                if (State == AIState.Ramming && Projectile.frame < 4)
                    Projectile.frame = 4;

                if (State != AIState.Ramming && Projectile.frame > 3)
                    Projectile.frame = 0;
            }
        }

        private void ShootEffect()
        {
            for (int i = 0; i < 5; i++)
                Dust.NewDustPerfect(Projectile.Center, 40, Projectile.rotation.ToRotationVector2().RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(3f, 5f));
        }

        #endregion

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (State == AIState.Ramming)
                modifiers.SourceDamage *= 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;

            if (State == AIState.Ramming && CalamityConfig.Instance.Afterimages)
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor);

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
