using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ChickenCannonHeld : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chicken Cannon");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 126;
            projectile.height = 44;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            float fireRate = 33f;
            projectile.ai[1] -= 1f;
            bool shouldShoot = false;
            if (projectile.ai[1] <= 0f)
            {
                projectile.ai[1] = fireRate;
                shouldShoot = true;
            }
            bool canShoot = player.channel && player.HasAmmo(player.ActiveItem(), true) && !player.noItems && !player.CCed;
            if (projectile.soundDelay <= 0 && canShoot)
            {
                projectile.soundDelay = (int)fireRate;
                Main.PlaySound(SoundID.Item61, projectile.position);
            }

            if (Main.myPlayer == projectile.owner)
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
                    float shootSpeed = player.ActiveItem().shootSpeed * projectile.scale;
                    if (float.IsNaN(speedMult.X) || float.IsNaN(speedMult.Y))
                    {
                        speedMult = -Vector2.UnitY;
                    }
                    speedMult *= shootSpeed;
                    if (speedMult.X != projectile.velocity.X || speedMult.Y != projectile.velocity.Y)
                    {
                        projectile.netUpdate = true;
                    }
                    projectile.velocity = speedMult * 0.55f;

                    if (shouldShoot)
                    {
                        player.PickAmmo(player.ActiveItem(), ref projType, ref speedMult2, ref canShoot, ref dmg, ref kBack, true);
                        projType = ModContent.ProjectileType<ChickenRocket>();
                        kBack = player.GetWeaponKnockback(player.ActiveItem(), kBack);

                        Vector2 velocity = Vector2.Normalize(projectile.velocity) * speedMult2;
                        if (float.IsNaN(velocity.X) || float.IsNaN(velocity.Y))
                        {
                            velocity = -Vector2.UnitY;
                        }
                        Projectile.NewProjectile(source, velocity, projType, dmg, kBack, projectile.owner);
                    }
                }
                else if (!canShoot)
                {
                    projectile.Kill();
                }
            }

            projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - projectile.Size / 2f;
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            if (player.itemTime < 2)
                player.itemTime = 2;
            if (player.itemAnimation < 2)
                player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * projectile.direction, projectile.velocity.X * projectile.direction);
        }

        public override bool CanDamage() => false;
    }
}
