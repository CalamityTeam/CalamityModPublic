using System;
using System.IO;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class AtlasMunitionsAutocannonHeld : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public bool HasInitialized;

        public float HeatInterpolant;

        public ThanatosSmokeParticleSet SmokeDrawer = new(-1, 3, 0f, 16f, 1.5f);

        public Player Owner => Main.player[Projectile.owner];

        public bool BeingHeld
        {
            // Zero is used as the true value instead of one so that the default state of the weapon at the time of creation indicates that it's being held, because the cannon
            // should only be spawned in the hands of a player.
            get => Projectile.ai[0] == 0f;
            set => Projectile.ai[0] = value ? 0f : 1f;
        }

        // An ai value is used in this context instead of a direct property as a means of mitigating the need to handle headaches with mouse syncing.
        public bool IsFiring
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value.ToInt();
        }

        public bool OwnerCanHold
        {
            get
            {
                // Don't let the owner hold the cannon if they have no-use-item debuff effects, such as Cursed.
                if (Owner.noItems || Owner.CCed)
                    return false;

                // Drop the cannon if the player has clicked the right mouse button after the first frame.
                if (Main.myPlayer == Projectile.owner && Main.mouseRightRelease && Main.mouseRight && HasInitialized)
                    return false;

                // Drop the cannon if it's too hot to hold.
                if (HeatInterpolant >= 1f)
                    return false;

                // Drop the cannon if the player is not holding the appropriate item.
                if (Owner.ActiveItem().type != ModContent.ItemType<AtlasMunitionsBeacon>())
                    return false;

                return true;
            }
        }

        public ref float CannonDroppedTimer => ref Projectile.localAI[0];

        public ref float ShootTimer => ref Projectile.localAI[1];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.sentry = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90000;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.Opacity = 1f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HeatInterpolant);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HeatInterpolant = reader.ReadSingle();
        }

        public override void AI()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);

            // Decide whether the cannon should interact with tiles.
            Projectile.tileCollide = !BeingHeld;

            // Determine frames.
            DetermineFrames();

            // Update the smoke drawer.
            SmokeDrawer.ParticleSpawnRate = HeatInterpolant > 0.7f ? 3 : int.MaxValue;
            SmokeDrawer.BaseMoveRotation = MathHelper.PiOver2 + Projectile.spriteDirection * (Projectile.position.X - Projectile.oldPosition.X) * 0.04f;
            SmokeDrawer.Update();

            // Cool down if not being held.
            if (!BeingHeld)
                HeatInterpolant = MathHelper.Clamp(HeatInterpolant - 1f / AtlasMunitionsBeacon.HeatDissipationTime, 0f, 1f);

            if (BeingHeld)
            {
                // Drop the cannon if the player can no longer hold it due to side effects (such as Cursed or Frozen debuffs), the cannon has overheated too much, or the player simply dropped it manually.
                if (!OwnerCanHold)
                {
                    BeingHeld = false;
                    Projectile.velocity = new Vector2(Projectile.spriteDirection * 3f, -4f);
                    Projectile.netUpdate = true;
                    if (HeatInterpolant >= 1f)
                        CombatText.NewText(Owner.Hitbox, Color.OrangeRed, CalamityUtils.GetTextValue("Misc.AutocannonHot"), true);

                    return;
                }
                
                // Reset opacity and the dropped timer. This is done to undo any potential fadeout effects from the dropped state.
                CannonDroppedTimer = 0f;
                Projectile.Opacity = 1f;
                Projectile.gfxOffY = 0;

                DetermineFiringStatus();
                if (IsFiring)
                    ShootProjectiles();

                // Reset the shoot timer if not firing.
                else
                    ShootTimer = 0f;

                UpdateProjectileHeldVariables(armPosition);
                ManipulatePlayerVariables();

                // Trigger the initialization state. The cannon will always be held at the time of being naturally spawned.
                HasInitialized = true;
                return;
            }

            // If the cannon is not being held, it falls to the ground and cools off. After enough time has passed, it fades away, similar to gores.
            // Players may pick it up again in this state if they want.
            Projectile.rotation = Projectile.rotation.AngleTowards(0f, 0.3f);
            Projectile.velocity.X *= 0.96f;
            Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y + 0.25f, -16f, 15.9f);
            Projectile.gfxOffY = 10;

            // A cannon that is not held by anyone does not fire.
            IsFiring = false;

            // Increment the dropped timer.
            CannonDroppedTimer++;

            // Fade out once on the ground for long enough.
            if (CannonDroppedTimer >= AtlasMunitionsBeacon.HeldCannonMaxDropTime - AtlasMunitionsBeacon.HeldCannonFadeoutTime)
            {
                Projectile.Opacity = Utils.GetLerpValue(0f, -AtlasMunitionsBeacon.HeldCannonFadeoutTime, CannonDroppedTimer - AtlasMunitionsBeacon.HeldCannonMaxDropTime, true);

                // Delete the cannon and associated pad if it has faded out completely.
                if (Projectile.Opacity <= 0f)
                {
                    int podID = ModContent.ProjectileType<AtlasMunitionsDropPod>();
                    int podUpperID = ModContent.ProjectileType<AtlasMunitionsDropPodUpper>();
                    int cannonID = ModContent.ProjectileType<AtlasMunitionsAutocannon>();
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        bool validID = Main.projectile[i].type == podID || Main.projectile[i].type == podUpperID || Main.projectile[i].type == cannonID;
                        if (Main.projectile[i].active && validID && Main.projectile[i].owner == Projectile.owner)
                            Main.projectile[i].Kill();
                    }

                    Projectile.Kill();
                    return;
                }
            }

            // Be picked up by nearby players if they right click and are holding the appropriate item.
            bool rightClick = Main.mouseRight && Main.mouseRightRelease;
            bool correctItem = Main.LocalPlayer.ActiveItem().type == ModContent.ItemType<AtlasMunitionsBeacon>();
            if (Main.LocalPlayer.WithinRange(Projectile.Center, AtlasMunitionsBeacon.PickupRange) && rightClick && correctItem && HeatInterpolant < 0.5f)
            {
                BeingHeld = true;
                Projectile.owner = Main.myPlayer;
                Projectile.netUpdate = true;
            }
        }

        public void DetermineFrames()
        {
            int minFrame = 4;
            int maxFrame = 5;
            bool framesShouldLoop = true;

            // Not firing frames -> Firing frames.
            if (IsFiring && (Projectile.frame < 4 || Projectile.frame > 5))
            {
                if (Projectile.frame > 5)
                    Projectile.frame = 0;
                minFrame = 0;
                maxFrame = 4;
                framesShouldLoop = false;
            }

            // Firing frames -> Not firing frames.
            else if (!IsFiring)
            {
                minFrame = 6;
                maxFrame = 9;
                framesShouldLoop = false;
            }

            Projectile.frameCounter++;
            if (Projectile.frame < minFrame)
                Projectile.frame = minFrame;
            if (Projectile.frameCounter % 5 == 4)
                Projectile.frame++;
            if (Projectile.frame > maxFrame)
                Projectile.frame = framesShouldLoop ? minFrame : maxFrame;
        }

        public void DetermineFiringStatus()
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            // Determine whether the cannon is being fired based on the left mouse state.
            bool wasFiring = IsFiring;
            IsFiring = Main.mouseLeft && !Owner.mouseInterface;

            // Notify other clients and the server if the cannon's firing state has changed. This sync cannot be blocked by the net spam threshold.
            if (wasFiring != IsFiring)
            {
                Projectile.netSpam = 0;
                Projectile.netUpdate = true;
            }
        }

        public void ShootProjectiles()
        {
            ShootTimer++;

            // Periodically shoot lasers.
            if (ShootTimer >= AtlasMunitionsBeacon.HeldCannonShootRate)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound with { Volume = 0.4f }, Projectile.Center);
                if (Main.myPlayer == Projectile.owner)
                {
                    int laserCount = 3;
                    int laserDamage = (int)(Projectile.damage * AtlasMunitionsBeacon.OverdriveProjectileDamageFactor);
                    int laserID = ModContent.ProjectileType<AtlasMunitionsLaserOverdrive>();
                    for (int i = 0; i < laserCount; i++)
                    {
                        Vector2 laserVelocity = Projectile.velocity.RotatedByRandom(AtlasMunitionsBeacon.OverdriveProjectileAngularRandomness) * 9.25f;
                        Vector2 laserSpawnOffset = Projectile.velocity * 66f + (MathHelper.TwoPi * i / laserCount + MathHelper.PiOver2 / laserCount).ToRotationVector2() * 10f;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + laserSpawnOffset, laserVelocity, laserID, laserDamage, 0f, Projectile.owner);
                    }

                    // Add heat to the cannon.
                    HeatInterpolant = MathHelper.Clamp(HeatInterpolant + 1f / AtlasMunitionsBeacon.ShotsNeededToReachMaxHeat, 0f, 1f);
                    Projectile.netUpdate = true;
                }
                ShootTimer = 0;
            }
        }

        public void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            // Update the cannon direction.
            if (Main.myPlayer == Projectile.owner)
            {
                float interpolant = Utils.GetLerpValue(16f, 56f, Projectile.Distance(Main.MouseWorld), true) * Utils.GetLerpValue(3f, 10f, MathHelper.Distance(Main.MouseWorld.X, Owner.Center.X), true);
                Vector2 oldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.MouseWorld), interpolant).SafeNormalize(Vector2.UnitX * Owner.direction);

                Projectile.direction = (Projectile.velocity.X > 0f).ToDirectionInt();
                Projectile.spriteDirection = Projectile.direction;
                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }

            Vector2 cannonEndOffset = Projectile.velocity * 26f + Projectile.velocity.RotatedBy(MathHelper.PiOver2 * Projectile.spriteDirection) * 2f;
            Projectile.position = armPosition - Projectile.Size * 0.5f + cannonEndOffset;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;
        }

        public void ManipulatePlayerVariables()
        {
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.spriteDirection * -0.4f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Draw smoke.
            SmokeDrawer.DrawSet(Projectile.Center - Projectile.velocity * 24f);

            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/AtlasMunitionsAutocannonHeld").Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/AtlasMunitionsAutocannonHeldGlow").Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            SpriteEffects direction = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            for (int i = 0; i < 12; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 12f + Main.GlobalTimeWrappedHourly * 2.3f).ToRotationVector2() * (float)Math.Pow(HeatInterpolant, 2.3) * 6f;
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, frame, AtlasMunitionsBeacon.HeatGlowColor * Projectile.Opacity * 0.5f, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, direction, 0);
            }

            Main.EntitySpriteDraw(texture, drawPosition, frame, Color.Lerp(lightColor, AtlasMunitionsBeacon.HeatGlowColor, HeatInterpolant * 0.45f) * Projectile.Opacity, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, direction, 0);
            Main.EntitySpriteDraw(glowmask, drawPosition, frame, Color.White * Projectile.Opacity, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, direction, 0);
            return false;
        }

        // The cannon does not get destroyed by tile collisions. This only applies when in the dropped state.
        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        // The cannon itself does not do damage, but it does store damage for the lasers that it fires.
        public override bool? CanDamage() => false;
    }
}
