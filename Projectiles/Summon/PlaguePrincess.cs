using CalamityMod.CalPlayer;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using CalamityMod.Sounds;
using CalamityMod.Particles;
using CalamityMod.NPCs.PlaguebringerGoliath;

namespace CalamityMod.Projectiles.Summon
{
    public class PlaguePrincess : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public enum ViriliAIState
        {
            HoverNearOwner,
            ChargeAtEnemies,
            BombardEnemiesWithRockets,
            SummonPlagueBeesOnEnemies
        }

        public bool UseAfterimages;

        public ViriliAIState CurrentState
        {
            get => (ViriliAIState)Projectile.ai[0];
            set => Projectile.ai[0] = (int)value;
        }

        public Player Owner => Main.player[Projectile.owner];

        public ref float AITimer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 116;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = InfectedRemote.MinionSlotRequirement;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = InfectedRemote.DefaultIframes;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            // Decide whether the minion should still exist.
            HandleMinionBools();

            // Decide frames.
            DecideFrames();

            // Reset afterimages, extra updates, and i-frames.
            UseAfterimages = false;
            Projectile.MaxUpdates = 1;
            Projectile.localNPCHitCooldown = InfectedRemote.DefaultIframes;

            NPC potentialTarget = Projectile.Center.MinionHoming(InfectedRemote.EnemyTargetingRange, Owner);
            switch (CurrentState)
            {
                case ViriliAIState.HoverNearOwner:
                    DoBehavior_HoverNearOwner(potentialTarget);
                    break;
                case ViriliAIState.ChargeAtEnemies:
                    DoBehavior_ChargeAtEnemies(potentialTarget);
                    break;
                case ViriliAIState.BombardEnemiesWithRockets:
                    DoBehavior_BombardEnemiesWithRockets(potentialTarget);
                    break;
                case ViriliAIState.SummonPlagueBeesOnEnemies:
                    DoBehavior_SummonPlagueBeesOnEnemies(potentialTarget);
                    break;
            }
            AITimer++;
        }

        public void HandleMinionBools()
        {
            Owner.AddBuff(ModContent.BuffType<ViriliBuff>(), 3600);
            if (Projectile.type == ModContent.ProjectileType<PlaguePrincess>())
            {
                if (Owner.dead)
                    Owner.Calamity().virili = false;

                if (Owner.Calamity().virili)
                    Projectile.timeLeft = 2;
            }
        }

        public void DecideFrames()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 6 % Main.projFrames[Projectile.type];
        }

        public void DoBehavior_HoverNearOwner(NPC potentialTarget)
        {
            if (Projectile.WithinRange(Owner.Center, 160f))
                return;

            // Hover near the owner.
            Projectile.velocity = (Projectile.velocity * 34f + Projectile.SafeDirectionTo(Owner.Center) * 17f) / 35f;

            // Teleport to the owner if sufficiently far away.
            if (!Projectile.WithinRange(Owner.Center, 2500f))
            {
                Projectile.Center = Owner.Center;
                Projectile.velocity *= 0.3f;
                Projectile.netUpdate = true;
            }

            if (Math.Abs(Projectile.velocity.X) > 0.2f)
                Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);

            if (potentialTarget is not null)
            {
                CurrentState = ViriliAIState.ChargeAtEnemies;
                AITimer = 0f;
                Projectile.netUpdate = true;
            }

            // Stop rotating.
            Projectile.rotation = Projectile.rotation.AngleTowards(0f, 0.1f);
        }

        public void DoBehavior_ChargeAtEnemies(NPC target)
        {
            int hoverTime = 18;
            int chargeTime = 16;
            int slowdownTime = 15;
            int chargeCount = 6;
            float hoverSpeed = 17f;

            // Exit the attack state if Virili no longer has a valid target.
            if (target is null)
            {
                ReturnToIdleState();
                return;
            }

            // Use more extra updates and less i-frames.
            Projectile.MaxUpdates = InfectedRemote.MaxUpdatesWhenCharging;
            Projectile.localNPCHitCooldown = InfectedRemote.ChargeIframes;

            float wrappedAttackTimer = AITimer % (hoverTime + chargeTime + slowdownTime);

            // Hover into position, to the top left/right of the target.
            if (wrappedAttackTimer < hoverTime)
            {
                // Look at the target.
                Projectile.spriteDirection = (target.Center.X > Projectile.Center.X).ToDirectionInt();
                HoverToPosition(target.Center + new Vector2(Projectile.spriteDirection * -270f, -150f), hoverSpeed);
            }
            else
            {
                UseAfterimages = true;
                if (wrappedAttackTimer < hoverTime + chargeTime)
                {
                    // Create anime-esque streaks during the dash.
                    if (wrappedAttackTimer % 2f == 1f && wrappedAttackTimer >= hoverTime + 3f)
                    {
                        Vector2 particleVelocity = -Projectile.velocity.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(6f, 10f);
                        var energyLeak = new SquishyLightParticle(Projectile.Center + Main.rand.NextVector2Circular(50f, 50f) + Projectile.velocity * 5f, particleVelocity, Main.rand.NextFloat(0.45f, 0.87f), Color.ForestGreen, 30, 3.4f, 4.5f);
                        GeneralParticleHandler.SpawnParticle(energyLeak);
                    }

                    if (Projectile.velocity.Length() < InfectedRemote.RegularChargeSpeed)
                        Projectile.velocity *= 1.1f;
                }
            }

            // Dash extremely quickly at the target.
            if (wrappedAttackTimer == hoverTime)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.ELRFireSound, Projectile.Center);
                Projectile.velocity = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, target, InfectedRemote.RegularChargeSpeed * 0.55f, 8);
                Projectile.netUpdate = true;
            }

            // Slow down rapidly to a crawl after the charge.
            if (wrappedAttackTimer >= hoverTime + chargeTime)
                Projectile.velocity *= 0.825f;

            // Determine rotation.
            Projectile.rotation = Projectile.velocity.X * 0.014f;

            // Transition to the next attack state once enough charges have happened.
            if (AITimer >= (hoverTime + chargeTime + slowdownTime) * chargeCount)
            {
                AITimer = 0f;
                Projectile.velocity *= 0.3f;
                CurrentState = ViriliAIState.BombardEnemiesWithRockets;
                Projectile.netUpdate = true;
            }
        }

        public void DoBehavior_BombardEnemiesWithRockets(NPC target)
        {
            int hoverTime = 50;
            int chargeTime = 50;
            int chargeCount = 6;
            float hoverSpeed = 17f;
            float rocketShootSpeed = 7f;

            // Exit the attack state if Virili no longer has a valid target.
            if (target is null)
            {
                ReturnToIdleState();
                return;
            }

            // Use more extra updates.
            Projectile.MaxUpdates = InfectedRemote.MaxUpdatesWhenCharging;

            float wrappedAttackTimer = AITimer % (hoverTime + chargeTime);
            
            // Hover into position, to the top left/right of the target.
            if (wrappedAttackTimer < hoverTime)
            {
                // Look at the target.
                Projectile.spriteDirection = (target.Center.X > Projectile.Center.X).ToDirectionInt();

                Vector2 hoverDestination = target.Center + new Vector2(Projectile.spriteDirection * -480f, -280f);
                HoverToPosition(hoverDestination, hoverSpeed);
                if (Projectile.WithinRange(hoverDestination, 150f))
                {
                    Projectile.velocity *= 0.85f;
                    Projectile.Center = Vector2.Lerp(Projectile.Center, hoverDestination, 0.03f);
                }
            }
            else
            {
                UseAfterimages = true;
                if (wrappedAttackTimer < hoverTime + chargeTime)
                {
                    // Create anime-esque streaks during the dash.
                    if (wrappedAttackTimer % 2f == 1f && wrappedAttackTimer >= hoverTime + 3f)
                    {
                        Vector2 particleVelocity = -Projectile.velocity.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(6f, 10f);
                        var energyLeak = new SquishyLightParticle(Projectile.Center + Main.rand.NextVector2Circular(50f, 50f) + Projectile.velocity * 5f, particleVelocity, Main.rand.NextFloat(0.45f, 0.87f), Color.ForestGreen, 30, 3.4f, 4.5f);
                        GeneralParticleHandler.SpawnParticle(energyLeak);
                    }

                    // Release rockets.
                    if (Main.myPlayer == Projectile.owner && wrappedAttackTimer % InfectedRemote.RocketShootRate == InfectedRemote.RocketShootRate - 1f)
                    {
                        Vector2 rocketSpawnPosition = Projectile.Center + Vector2.UnitY * Projectile.scale * 48f;
                        Vector2 rocketVelocity = (target.Center - rocketSpawnPosition).SafeNormalize(Vector2.UnitY) * rocketShootSpeed;
                        int rocket = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, rocketVelocity, ModContent.ProjectileType<MK2RocketHoming>(), (int)(Projectile.damage * InfectedRemote.RocketDamageFactor), 3f, Projectile.owner);
                        if (Main.projectile.IndexInRange(rocket))
                            Main.projectile[rocket].originalDamage = (int)(Projectile.originalDamage * InfectedRemote.RocketDamageFactor);
                    }

                    if (Projectile.velocity.Length() < InfectedRemote.HorizontalRocketChargeSpeed)
                        Projectile.velocity *= 1.1f;
                }
            }

            // Dash extremely quickly above the target.
            if (wrappedAttackTimer == hoverTime)
            {
                SoundEngine.PlaySound(PlaguebringerGoliath.BarrageLaunchSound, Projectile.Center);
                Projectile.velocity = Vector2.UnitX * InfectedRemote.HorizontalRocketChargeSpeed * Projectile.spriteDirection * 0.55f;
                Projectile.netUpdate = true;
            }

            // Transition to the next attack state once enough charges have happened.
            if (AITimer >= (hoverTime + chargeTime) * chargeCount)
            {
                AITimer = 0f;
                Projectile.velocity *= 0.3f;
                CurrentState = ViriliAIState.SummonPlagueBeesOnEnemies;
                Projectile.netUpdate = true;
            }
        }

        public void DoBehavior_SummonPlagueBeesOnEnemies(NPC target)
        {
            int shootTime = 300;
            float hoverSpeed = 23f;

            // Exit the attack state if Virili no longer has a valid target.
            if (target is null)
            {
                ReturnToIdleState();
                return;
            }

            // Hover above the target.
            Vector2 hoverDestination = target.Center - Vector2.UnitY * 350f;
            HoverToPosition(hoverDestination, hoverSpeed);
            if (Projectile.WithinRange(hoverDestination, 240f))
            {
                Projectile.velocity *= 0.7f;
                Projectile.Center = Vector2.Lerp(Projectile.Center, hoverDestination, 0.04f);

                // Release bees.
                if (Main.myPlayer == Projectile.owner && AITimer % InfectedRemote.BeeShootRate == InfectedRemote.BeeShootRate - 1f)
                {
                    int smallBee = ModContent.ProjectileType<PlagueBeeSmall>();
                    int bigBee = ModContent.ProjectileType<BabyPlaguebringer>();
                    int projType = smallBee;
                    if (Owner.ownedProjectileCounts[bigBee] <= 0 && Main.rand.NextBool())
                        projType = bigBee;

                    if (Main.myPlayer == Projectile.owner && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, target.Center, 0, 0))
                    {
                        int beeCount = projType == bigBee ? 1 : 4;
                        for (int i = 0; i < beeCount; i++)
                        {
                            Vector2 beeVelocity = Projectile.SafeDirectionTo(target.Center) * 6f;
                            int bee = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, beeVelocity, projType, (int)(Projectile.damage * InfectedRemote.BeeDamageFactor), 0f, Projectile.owner);
                            if (Main.projectile.IndexInRange(bee))
                            {
                                if (projType == bigBee)
                                    Main.projectile[bee].frame = 2;
                            }
                            Projectile.netUpdate = true;
                        }
                    }
                }
            }

            if (AITimer >= shootTime)
            {
                AITimer = 0f;
                Projectile.velocity *= 0.3f;
                CurrentState = ViriliAIState.ChargeAtEnemies;
                Projectile.netUpdate = true;
            }
        }

        public void HoverToPosition(Vector2 hoverDestination, float hoverSpeed)
        {
            Vector2 baseHoverVelocity = Projectile.SafeDirectionTo(hoverDestination) * hoverSpeed;

            // If not close to the hover destination, rapidly zoom towards it. Otherwise, hover in place.
            if (!Projectile.WithinRange(hoverDestination, 150f))
            {
                float hyperspeedInterpolant = Utils.GetLerpValue(Projectile.Distance(hoverDestination), 500f, 960f, true);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Lerp(baseHoverVelocity * 1.4f, (hoverDestination - Projectile.Center) * 0.1f, hyperspeedInterpolant), 0.2f);
            }
            else
            {
                Projectile.velocity = (Projectile.velocity * 29f + baseHoverVelocity) / 30f;
                Projectile.velocity = Projectile.velocity.MoveTowards(baseHoverVelocity, hoverSpeed / 11f);
            }
        }

        public void ReturnToIdleState()
        {
            CurrentState = ViriliAIState.HoverNearOwner;
            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (UseAfterimages)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Color afterimageDrawColor = Color.ForestGreen with { A = 25 } * Projectile.Opacity * (1f - i / (float)Projectile.oldPos.Length);
                    Vector2 afterimageDrawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                    Main.EntitySpriteDraw(texture, afterimageDrawPosition, frame, afterimageDrawColor, Projectile.rotation, origin, Projectile.scale, direction, 0);
                }
            }
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Plague>(), 180);

        public override bool? CanDamage() => CurrentState == ViriliAIState.ChargeAtEnemies ? null : false;
    }
}
