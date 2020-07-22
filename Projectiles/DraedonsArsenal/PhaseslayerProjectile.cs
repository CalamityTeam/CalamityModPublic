using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PhaseslayerProjectile : ModProjectile
    {
        public bool IsSmall => Main.player[projectile.owner].ActiveItem().Calamity().CurrentCharge / (float)Main.player[projectile.owner].ActiveItem().Calamity().ChargeMax < 0.5f;
        public float AngularSwingMomentum
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public float FadeoutTime
        {
            get => projectile.localAI[0];
            set => projectile.localAI[0] = value;
        }
        public int BladeFrameX => IsSmall ? 1 : projectile.frame / 7;
        public int BladeFrameY => IsSmall ? projectile.frame % 3 : projectile.frame % 7;

        // This is rotation EVERY FRAME. Max damage occurs when you can swing it 360 degrees in half a second.
        // That speed is 12 degrees per frame, or pi/15 radians per frame.
        public const float MaxAngularDamage = MathHelper.Pi / 15f;
        public const float DamageUpdateResponsiveness = 0.08f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phaseslayer");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 46; // Collision logic is done manually.
            projectile.height = 46;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.friendly = true;
            projectile.localNPCHitCooldown = 3;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            float rotationAdjusted = MathHelper.WrapAngle(projectile.rotation) + MathHelper.Pi; // Within the bounds of 0 and 2pi instead of -pi and pi for convenience with absolute values.
            float oldRotationAdjusted = MathHelper.WrapAngle(projectile.oldRot[1]) + MathHelper.Pi;
            float deltaAngle = Math.Abs(rotationAdjusted - oldRotationAdjusted);

            ManipulatePlayer(player);
            AdjustDamageBasedOnRotation(player, deltaAngle);
            ManipulateFrames();
            PlaySounds(player, deltaAngle);
            HandleCooldowns();
        }

        public void AdjustDamageBasedOnRotation(Player player, float deltaAngle)
        {
            AngularSwingMomentum = MathHelper.Lerp(AngularSwingMomentum, deltaAngle, DamageUpdateResponsiveness);

            float damageInterpolationValue = 0.16f + (float)Math.Log(AngularSwingMomentum / MaxAngularDamage + 1.5f, 3f);
            float baseDamageFromSpeed = MathHelper.Lerp(Phaseslayer.MinDamage, Phaseslayer.MaxDamage, damageInterpolationValue);

            float statDamageMultiplier = player.MeleeDamage() * (IsSmall ? Phaseslayer.SmallPhaseDamageMultiplier : 1f);
            projectile.damage = (int)(baseDamageFromSpeed * statDamageMultiplier);
        }

        public void ManipulatePlayer(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                // Smoothly rotate towards the mouse. This effect is weaker if the mouse is close to the player, to prevent jittery movement.
                if (player.channel && !player.noItems && !player.CCed && player.ActiveItem().type == ModContent.ItemType<Phaseslayer>())
                {
                    float mouseDistance = projectile.Distance(Main.MouseWorld);

                    // Ranges from 0 (your mouse is on the player) to 1 (your mouse is at the max range considered, or any further distance).
                    float distRatio = Utils.InverseLerp(0f, 360f, mouseDistance, true);

                    float aimResponsiveness = Utils.InverseLerp(0.035f, 1f, distRatio);
                    projectile.rotation = projectile.rotation.AngleLerp(player.AngleTo(Main.MouseWorld), aimResponsiveness);
                }
                else if (FadeoutTime == 0f)
                {
                    if (!player.channel)
                        FadeoutTime = 10f; // Fade out if the player is no longer charging the weapon.
                    else projectile.Kill(); // But if something happened to the player, such as death, just kill the projectile immediately.
                }
                projectile.Center = player.MountedCenter + projectile.rotation.ToRotationVector2() * 36f;
            }
            projectile.direction = (Math.Cos(projectile.rotation) > 0).ToDirectionInt();
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = projectile.rotation * projectile.direction;
        }

        public void ManipulateFrames()
        {
            projectile.frame = 0;

            if (IsSmall)
            {
                // Fadeout frames.
                if (FadeoutTime > 5)
                    projectile.frame = 1;
                else if (FadeoutTime > 0)
                    projectile.frame = 2;
            }
            else
            {
                projectile.frameCounter++;
                int adjustFrameCounter = projectile.frameCounter % 120;

                // Idle lightning frames.
                if (adjustFrameCounter >= 50 && adjustFrameCounter <= 78)
                    projectile.frame = (int)MathHelper.Lerp(1, 9, Utils.InverseLerp(50, 75, adjustFrameCounter, true));
                if (adjustFrameCounter >= 90 && adjustFrameCounter <= 120)
                    projectile.frame = (int)MathHelper.Lerp(10, 18, Utils.InverseLerp(90, 117, adjustFrameCounter, true));

                // Fadeout frames.
                if (FadeoutTime > 5)
                    projectile.frame = 19;
                else if (FadeoutTime > 0)
                    projectile.frame = 20;
            }
        }

        public void PlaySounds(Player player, float deltaAngle)
        {
            if (projectile.soundDelay <= 0 && deltaAngle >= 0.055f)
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    Vector2 velocity = projectile.rotation.ToRotationVector2() * 20f;
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<PhaseslayerBeam>(), (int)(projectile.damage * 0.45), 0f, player.whoAmI);
                }
                bool wasBig = !IsSmall;
                if (Main.rand.NextBool(10) && player.ActiveItem().Calamity().CurrentCharge > 0)
                {
                    // This needs to be done manually instead of via ActiveItem. If it isn't, the charge reductions don't work if the item is held via the mouse.
                    if (Main.myPlayer == projectile.owner)
                    {
                        if (Main.mouseItem.active)
                            Main.mouseItem.Calamity().CurrentCharge--;
                        else player.HeldItem.Calamity().CurrentCharge--;
                    }

                    // Make some visuals if the sword becomes small as a result of charging.
                    if (IsSmall && wasBig)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/MechGaussRifle"), projectile.Center);
                        if (!Main.dedServ)
                        {
                            for (int i = 0; i < 60; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.Brimstone);
                                dust.velocity = Main.rand.NextVector2Circular(20f, 20f);
                                dust.scale = 2.5f;
                                dust.fadeIn = 1.2f;
                                dust.noGravity = true;
                            }
                        }
                    }
                }
                projectile.soundDelay = 15 - (int)MathHelper.Clamp(deltaAngle / MathHelper.ToRadians(36f) * 9f, 0f, 9f);
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/ELRFire"), projectile.Center);
            }
        }

        public void HandleCooldowns()
        {
            if (FadeoutTime > 0)
            {
                FadeoutTime--;
                if (FadeoutTime <= 0f)
                    projectile.Kill();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D bladeTexture = ModContent.GetTexture("CalamityMod/Projectiles/DraedonsArsenal/PhaseslayerBlade");
            Texture2D hiltTexture = ModContent.GetTexture(Texture);
            if (IsSmall)
                bladeTexture = ModContent.GetTexture("CalamityMod/Projectiles/DraedonsArsenal/PhaseslayerBladeSmall");
            Vector2 bladeOffset = projectile.rotation.ToRotationVector2() * (IsSmall ? 90f : 132f) * projectile.scale;
            Vector2 origin = bladeTexture.Size() * 0.5f;
            origin /= IsSmall ? new Vector2(1f, 3f) : new Vector2(3f, 7f);
            Rectangle frame = IsSmall ? bladeTexture.Frame(1, 3, 0, BladeFrameY) : bladeTexture.Frame(3, 7, BladeFrameX, BladeFrameY);

            // TODO: Update this afterimage drawcode to be more cool.
            for (int i = 1; i < projectile.oldRot.Length; i++)
            {
                float angleDelta = MathHelper.Clamp(projectile.rotation - projectile.oldRot[i], -0.26f, 0.26f);
                float angle = projectile.rotation + angleDelta;
                angle += MathHelper.PiOver2;
                Color color = Color.White * (float)Math.Pow(1f - i / (float)projectile.oldRot.Length, 3f);
                Rectangle afterimageFrame = IsSmall ? bladeTexture.Frame(1, 3, 0, 2) : bladeTexture.Frame(3, 7, 2, 6);
                spriteBatch.Draw(bladeTexture, projectile.Center + bladeOffset - Main.screenPosition, afterimageFrame, color, angle, origin, projectile.scale, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(bladeTexture, projectile.Center + bladeOffset - Main.screenPosition, frame, Color.White, projectile.rotation + MathHelper.PiOver2, origin, projectile.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(hiltTexture, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation + MathHelper.PiOver2, hiltTexture.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            Vector2 start = projectile.Center + projectile.rotation.ToRotationVector2() * 28f;
            Vector2 end = start + projectile.rotation.ToRotationVector2() * (IsSmall ? 202f : 254f) * projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 60f * projectile.scale, ref _);
        }
    }
}
