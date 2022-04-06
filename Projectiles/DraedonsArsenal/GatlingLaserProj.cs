using CalamityMod.Items;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class GatlingLaserProj : ModProjectile
    {
        private SoundEffectInstance gatlingLaserLoop;
        private bool fireLasers = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gatling Laser");
        }

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
            Player player = Main.player[Projectile.owner];
            float num = MathHelper.PiOver2;
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
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
                        gatlingLaserLoop = SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/GatlingLaserFireLoop"), (int)Projectile.position.X, (int)Projectile.position.Y);
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
                        Vector2 value2 = vector;
                        Vector2 value3 = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY) - value2;
                        if (player.gravDir == -1f)
                        {
                            value3.Y = Main.screenHeight - Main.mouseY + Main.screenPosition.Y - value2.Y;
                        }
                        Vector2 vector3 = Vector2.Normalize(value3);
                        if (float.IsNaN(vector3.X) || float.IsNaN(vector3.Y))
                        {
                            vector3 = -Vector2.UnitY;
                        }
                        vector3 *= scaleFactor;
                        if (vector3.X != Projectile.velocity.X || vector3.Y != Projectile.velocity.Y)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity = vector3;
                        int type = ModContent.ProjectileType<GatlingLaserShot>();
                        float velocity = 3f;
                        value2 = Projectile.Center;
                        Vector2 spinningpoint = Vector2.Normalize(Projectile.velocity) * velocity;

                        double spread = Math.PI / 32D;
                        spinningpoint = spinningpoint.RotatedBy(Main.rand.NextDouble() * 2D * spread - spread);

                        if (float.IsNaN(spinningpoint.X) || float.IsNaN(spinningpoint.Y))
                        {
                            spinningpoint = -Vector2.UnitY;
                        }
                        Vector2 velocity2 = new Vector2(spinningpoint.X, spinningpoint.Y);
                        if (velocity2.Length() > 5f)
                        {
                            velocity2.Normalize();
                            velocity2 *= 5f;
                        }
                        float SpeedX = velocity2.X + Main.rand.Next(-1, 2) * 0.005f;
                        float SpeedY = velocity2.Y + Main.rand.Next(-1, 2) * 0.005f;
                        float ai0 = Projectile.ai[0] - 2f; // 0, 1, or 2

                        // Use charge when firing a laser.
                        modItem.Charge -= GatlingLaser.HoldoutChargeUse;
                        Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), value2.X, value2.Y, SpeedX, SpeedY, type, currentDamage, Projectile.knockBack, Projectile.owner, ai0, 0f);
                    }
                    else
                    {
                        if (gatlingLaserLoop != null)
                            gatlingLaserLoop.Stop();

                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/GatlingLaserFireEnd"), (int)Projectile.position.X, (int)Projectile.position.Y);
                        Projectile.Kill();
                    }
                }
            }
            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + num;
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
