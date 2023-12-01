using CalamityMod.Items;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class GatlingLaserProj : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<GatlingLaser>();
        private SlotId gatlingLaserLoopID;
        private bool fireLasers = false;

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            if (SoundEngine.TryGetActiveSound(gatlingLaserLoopID, out var ShootingSound) && ShootingSound.IsPlaying)
                ShootingSound.Position = Projectile.Center;
            Player player = Main.player[Projectile.owner];
            Vector2 playerRotation = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Projectile.type == ModContent.ProjectileType<GatlingLaserProj>())
            {
                if (Projectile.ai[0] < 5f)
                    Projectile.ai[0] += 1f;

                if (Projectile.ai[0] > 4f)
                    Projectile.ai[0] = 2f;

                // The Gatling Laser shoots every other frame (effective use time of 2).
                int fireRate = 2;
                Projectile.ai[1] += 1f;
                bool shootThisFrame = false;
                if (Projectile.ai[1] >= fireRate)
                {
                    Projectile.ai[1] = 0f;
                    shootThisFrame = true;
                }

                if (Projectile.soundDelay <= 0)
                {
                    Projectile.soundDelay = fireRate * 6;
                    if (Projectile.ai[0] != 1f)
                    {
                        fireLasers = true;
                        Projectile.soundDelay *= 6;
                        gatlingLaserLoopID = SoundEngine.PlaySound(GatlingLaser.FireLoopSound, Projectile.position);
                    }
                }
                if (shootThisFrame && Main.myPlayer == Projectile.owner && fireLasers)
                {
                    Item gatling = player.ActiveItem();

                    // This grabs the weapon's current damage, taking current charge level into account.
                    int currentDamage = player.GetWeaponDamage(gatling);

                    bool stillInUse = player.channel && !player.noItems && !player.CCed;

                    // This both checks if the player has sufficient mana and consumes it if they do.
                    // If this is false, the Gatling Laser stops functioning.
                    bool hasMana = player.CheckMana(gatling, -1, true, false);

                    // Checks if the Gatling Laser has sufficient charge to fire. If this is false, it stops functioning.
                    CalamityGlobalItem modItem = gatling.Calamity();
                    bool hasCharge = modItem.Charge >= GatlingLaser.HoldoutChargeUse;

                    if (stillInUse && hasMana && hasCharge)
                    {
                        float scaleFactor = player.ActiveItem().shootSpeed * Projectile.scale;
                        Vector2 laserDirection = playerRotation;
                        Vector2 aimDirection = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY) - laserDirection;
                        if (player.gravDir == -1f)
                        {
                            aimDirection.Y = Main.screenHeight - Main.mouseY + Main.screenPosition.Y - laserDirection.Y;
                        }
                        Vector2 projVelocity = Vector2.Normalize(aimDirection);
                        if (float.IsNaN(projVelocity.X) || float.IsNaN(projVelocity.Y))
                        {
                            projVelocity = -Vector2.UnitY;
                        }
                        projVelocity *= scaleFactor;
                        if (projVelocity.X != Projectile.velocity.X || projVelocity.Y != Projectile.velocity.Y)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity = projVelocity;
                        int type = ModContent.ProjectileType<GatlingLaserShot>();
                        float velocity = 3f;
                        laserDirection = Projectile.Center;
                        Vector2 spinningpoint = Vector2.Normalize(Projectile.velocity) * velocity;

                        double spread = Math.PI / 32D;
                        spinningpoint = spinningpoint.RotatedBy(Main.rand.NextDouble() * 2D * spread - spread);

                        if (float.IsNaN(spinningpoint.X) || float.IsNaN(spinningpoint.Y))
                        {
                            spinningpoint = -Vector2.UnitY;
                        }
                        Vector2 projVelocitySpread = new Vector2(spinningpoint.X, spinningpoint.Y);
                        if (projVelocitySpread.Length() > 5f)
                        {
                            projVelocitySpread.Normalize();
                            projVelocitySpread *= 5f;
                        }
                        float SpeedX = projVelocitySpread.X + Main.rand.Next(-1, 2) * 0.005f;
                        float SpeedY = projVelocitySpread.Y + Main.rand.Next(-1, 2) * 0.005f;
                        float ai0 = Projectile.ai[0] - 2f; // 0, 1, or 2

                        // Use charge when firing a laser.
                        modItem.Charge -= GatlingLaser.HoldoutChargeUse;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), laserDirection.X, laserDirection.Y, SpeedX, SpeedY, type, currentDamage, Projectile.knockBack, Projectile.owner, ai0, 0f);
                    }
                    else
                    {
                        ActiveSound result;
                        if (SoundEngine.TryGetActiveSound(gatlingLaserLoopID, out result))
                            result.Stop();

                        SoundEngine.PlaySound(GatlingLaser.FireEndSound, Projectile.position);
                        Projectile.Kill();
                    }
                }
            }
            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
        }

        public override bool? CanDamage() => false;
    }
}
