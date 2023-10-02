using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ChickenCannonHeld : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<ChickenCannon>();
        static float FireRate = 33f;
        //The first shot from the holdout doesnt consume ammo, this is because the ammo is already consumed by the fact the player needs to consume ammo to shoot it
        public ref float FreeShotLoaded => ref Projectile.ai[0];
        public ref float FramesTillNextShot => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 126;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }


            FramesTillNextShot--;
            bool shouldShoot = false;
            if (FramesTillNextShot <= 0f)
            {
                FramesTillNextShot = FireRate;
                shouldShoot = true;
            }
            bool canShoot = player.channel && (player.HasAmmo(player.ActiveItem()) || FreeShotLoaded > 0) && !player.noItems && !player.CCed;
            if (Projectile.soundDelay <= 0 && canShoot)
            {
                Projectile.soundDelay = (int)FireRate;
                SoundEngine.PlaySound(SoundID.Item61, Projectile.position);
            }

            if (Main.myPlayer == Projectile.owner)
            {
                int projType = ModContent.ProjectileType<ChickenRocket>(); // PickAmmo screws up with Rockets as per usual so this variable is reset right afterward
                float speedMult2 = 14.5f;
                int dmg = player.GetWeaponDamage(player.ActiveItem());
                float kBack = player.ActiveItem().knockBack;
                if (canShoot)
                {
                    Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
                    Vector2 direction = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY) - source;
                    if (player.gravDir == -1f)
                    {
                        direction.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - source.Y;
                    }
                    Vector2 speedMult = Vector2.Normalize(direction);
                    float shootSpeed = player.ActiveItem().shootSpeed * Projectile.scale;
                    if (float.IsNaN(speedMult.X) || float.IsNaN(speedMult.Y))
                    {
                        speedMult = -Vector2.UnitY;
                    }
                    speedMult *= shootSpeed;
                    if (speedMult.X != Projectile.velocity.X || speedMult.Y != Projectile.velocity.Y)
                    {
                        Projectile.netUpdate = true;
                    }
                    Projectile.velocity = speedMult * 0.55f;

                    if (shouldShoot)
                    {
                        if (FreeShotLoaded > 0)
                            FreeShotLoaded--;
                        else
                            player.PickAmmo(player.ActiveItem(), out projType, out speedMult2, out dmg, out kBack, out _);

                        projType = ModContent.ProjectileType<ChickenRocket>();
                        kBack = player.GetWeaponKnockback(player.ActiveItem(), kBack);

                        Vector2 velocity = Vector2.Normalize(Projectile.velocity) * speedMult2;
                        if (float.IsNaN(velocity.X) || float.IsNaN(velocity.Y))
                        {
                            velocity = -Vector2.UnitY;
                        }
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), source, velocity, projType, dmg, kBack, Projectile.owner);
                    }
                }
                else if (!canShoot)
                {
                    Projectile.Kill();
                }
            }

            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            if (player.itemTime < 2)
                player.itemTime = 2;
            if (player.itemAnimation < 2)
                player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
        }

        public override bool? CanDamage() => false;
    }
}
