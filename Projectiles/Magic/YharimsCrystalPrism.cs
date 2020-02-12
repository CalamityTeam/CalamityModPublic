using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class YharimsCrystalPrism : ModProjectile
    {
        private const int AimUpdateFrameInterval = 1;
        private const int SoundInterval = 20;
        private const float AimResponsiveness = 0.92f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yermes Christal");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

            float manaConsumptionFrameInterval = 30f;
            if (projectile.ai[0] > 90f) // 3 instances of 30 frames
                manaConsumptionFrameInterval = 15f;
            if (projectile.ai[0] > 120f) // 3 instances of 30 frames + 2 instances of 15 frames
                manaConsumptionFrameInterval = 5f;

            // Update beam damage based on curent magic damage stat (so Mana Sickness affects it)
            // DEFECT -- Doesn't scale with allDamage.
            projectile.damage = (int)((player.HeldItem?.damage ?? 0) * player.magicDamage);

            // ai[0] is the overall frame counter.
            projectile.ai[0] += 1f;

            // ai[1] is the frame counter for how frequently to update the aim of the crystal
            projectile.ai[1] += 1f;

            // Every ramp up interval, set these flags to true
            bool consumeManaThisFrame = projectile.ai[0] % manaConsumptionFrameInterval == 0f;
            bool shouldCastBeams = projectile.ai[0] % manaConsumptionFrameInterval == 0f;

            // Gently adjusts the aim vector of the crystal to point towards the mouse.
            if (projectile.ai[1] >= AimUpdateFrameInterval)
            {
                projectile.ai[1] = 0f;
                shouldCastBeams = true;
                if (projectile.owner == Main.myPlayer)
                {
                    // Scale seemingly never changes, so this just scales with shoot speed (Yharim's Crystal is 30 by default)
                    float shootSpeedScaledFactor = player.inventory[player.selectedItem].shootSpeed * projectile.scale;
                    Vector2 aimVector = Vector2.Normalize(Main.MouseWorld - rrp);
                    if (aimVector.HasNaNs())
                        aimVector = -Vector2.UnitY;
                    aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(projectile.velocity), AimResponsiveness));
                    aimVector *= shootSpeedScaledFactor;

                    if (aimVector != projectile.velocity)
                        projectile.netUpdate = true;
                    projectile.velocity = aimVector;
                }
            }

            // Animate the crystal as it is used
            projectile.frameCounter++;
            int framesPerAnimationUpdate = projectile.ai[0] >= 180f ? 2 : projectile.ai[0] >= 120f ? 3 : 4;
            if (projectile.frameCounter >= framesPerAnimationUpdate)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 6)
                    projectile.frame = 0;
            }

            // Make sound intermittently while the crystal is in use
            if (projectile.soundDelay <= 0)
            {
                projectile.soundDelay = SoundInterval;
                // Don't play the continuous beam sound the first time around
                if (projectile.ai[0] > 1f)
                    Main.PlaySound(SoundID.Item15, projectile.position);
            }

            // Update the crystal's existence: project beams if no beams exist yet, and despawn if out of mana.
            if (shouldCastBeams && Main.myPlayer == projectile.owner)
            {
                // CheckMana returns true if the mana cost can be paid. If mana isn't consumed this frame, the CheckMana short-circuits out of being evaluated.
                bool allowContinuedUse = !consumeManaThisFrame || player.CheckMana(player.inventory[player.selectedItem].mana, true, false);
                bool crystalStillInUse = player.channel && allowContinuedUse && !player.noItems && !player.CCed;

                // The beams are only projected once (on frame 1).
                if (crystalStillInUse && projectile.ai[0] == 1f)
                {
                    Vector2 beamVelocity = Vector2.Normalize(projectile.velocity);
                    if (beamVelocity.HasNaNs())
                        beamVelocity = -Vector2.UnitY;

                    int damage = projectile.damage;
                    float kb = projectile.knockBack; // should always be 0

                    // CHANGED: Yharim's Crystal used to fire 7 lasers, it now fires 6
                    for (int b = 0; b < 6; b++)
                        Projectile.NewProjectile(projectile.Center, beamVelocity, ModContent.ProjectileType<YharimsCrystalBeam>(), damage, kb, projectile.owner, b, projectile.whoAmI);
                    projectile.netUpdate = true;
                }
                else if (!crystalStillInUse)
                    projectile.Kill();
            }

            // Place the projectile directly into the player's hand at all times
            projectile.position = rrp - projectile.Size / 2f;
            // The beam comes out of the tip of the crystal, not the side
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.spriteDirection = projectile.direction;
            // Ensures the crystal will disappear immediately if the left mouse button is not held anymore
            projectile.timeLeft = 2;
            // This is a holdout projectile, so change the player's variables to reflect that
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * projectile.direction, projectile.velocity.X * projectile.direction);
        }

        // Completely custom drawcode
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Point enclosingTile = projectile.Center.ToTileCoordinates();
            Color localLight = Lighting.GetColor(enclosingTile.X, enclosingTile.Y);

            Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;

            // If the projectile is totally hidden, use the lighting on the player's center. Not sure why this is needed.
            if (projectile.hide && !ProjectileID.Sets.DontAttachHideToAlpha[projectile.type])
            {
                Point playerMountCenterTile = mountedCenter.ToTileCoordinates();
                localLight = Lighting.GetColor(playerMountCenterTile.X, playerMountCenterTile.Y);
            }

            SpriteEffects eff = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D tex = Main.projectileTexture[projectile.type];
            int frameHeight = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int texYOffset = frameHeight * projectile.frame;

            Vector2 sheetInsertVec = (projectile.Center + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition).Floor();

            // Draw the crystal itself
            Main.spriteBatch.Draw(tex, sheetInsertVec, new Rectangle?(new Rectangle(0, texYOffset, tex.Width, frameHeight)), projectile.GetAlpha(localLight), projectile.rotation, new Vector2(tex.Width / 2f, frameHeight / 2f), projectile.scale, eff, 0f);
            return false;
        }

        // public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}
