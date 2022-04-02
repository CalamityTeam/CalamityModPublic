using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FlurrystormCannonShooting : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flurrystorm Cannon");
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 68;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.coldDamage = true;
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
            if (projectile.frame >= 2)
            {
                projectile.frame = 0;
            }
            projectile.ai[0] += 1f;
            int fireRate = 0;
            if (projectile.ai[0] >= 60f)
            {
                fireRate++;
            }
            if (projectile.ai[0] >= 120f)
            {
                fireRate++;
            }
            if (projectile.ai[0] >= 180f)
            {
                fireRate++;
            }
            if (projectile.ai[0] >= 240f)
            {
                fireRate++;
            }
            if (projectile.ai[0] >= 300f)
            {
                fireRate++;
            }
            if (projectile.ai[0] >= 360f)
            {
                fireRate++;
            }
            if (projectile.ai[0] >= 420f)
            {
                fireRate++;
            }
            if (projectile.ai[0] >= 480f) //full speed after 8 seconds
            {
                fireRate++;
            }
            int initialRate = 24;
            int fireRateMult = 2;
            projectile.ai[1] -= 1f;
            bool shouldShoot = false;
            if (projectile.ai[1] <= 0f)
            {
                projectile.ai[1] = (float)(initialRate - fireRateMult * fireRate);
                shouldShoot = true;
            }
            bool canShoot = player.channel && player.HasAmmo(player.ActiveItem(), true) && !player.noItems && !player.CCed;
            if (projectile.localAI[0] > 0f)
            {
                projectile.localAI[0] -= 1f;
            }
            if (projectile.soundDelay <= 0 && canShoot)
            {
                projectile.soundDelay = initialRate - fireRateMult * fireRate;
                if (projectile.ai[0] != 1f)
                {
                    Main.PlaySound(SoundID.Item11, projectile.position);
                }
                projectile.localAI[0] = 12f;
            }
            if (shouldShoot && Main.myPlayer == projectile.owner)
            {
                int projType = ProjectileID.SnowBallFriendly;
                float speedMult2 = 14f;
                int dmg = player.GetWeaponDamage(player.ActiveItem());
                float kBack = player.ActiveItem().knockBack;
                if (canShoot)
                {
                    player.PickAmmo(player.ActiveItem(), ref projType, ref speedMult2, ref canShoot, ref dmg, ref kBack, false);
                    kBack = player.GetWeaponKnockback(player.ActiveItem(), kBack);
                    float shootSpeed = player.ActiveItem().shootSpeed * projectile.scale;
                    Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
                    Vector2 direction = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY) - source;
                    if (player.gravDir == -1f)
                    {
                        direction.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - source.Y;
                    }
                    Vector2 speedMult = Vector2.Normalize(direction);
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

                    Vector2 snowballVel = Vector2.Normalize(projectile.velocity) * speedMult2 * (0.6f + Main.rand.NextFloat(0f, 0.15f));
                    if (float.IsNaN(snowballVel.X) || float.IsNaN(snowballVel.Y))
                    {
                        snowballVel = -Vector2.UnitY;
                    }
                    Vector2 sourceS = source + Utils.RandomVector2(Main.rand, -5f, 5f);
                    snowballVel.X += Main.rand.NextFloat(-2.25f, 2.25f);
                    snowballVel.Y += Main.rand.NextFloat(-2.25f, 2.25f);
                    int snowball = Projectile.NewProjectile(sourceS, snowballVel, projType, dmg, kBack, projectile.owner);
                    if (snowball.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[snowball].noDropItem = true;
                        Main.projectile[snowball].Calamity().forceRanged = true;
                        Main.projectile[snowball].thrown = false;
                        Main.projectile[snowball].extraUpdates += Main.rand.Next(0,2);
                    }

                    if (Main.rand.NextBool(5)) //ice chunk
                    {
                        Vector2 chunkVel = Vector2.Normalize(projectile.velocity) * speedMult2 * (0.6f + Main.rand.NextFloat() * 0.8f);
                        if (float.IsNaN(chunkVel.X) || float.IsNaN(chunkVel.Y))
                        {
                            chunkVel = -Vector2.UnitY;
                        }
                        Vector2 sourceC = source + Utils.RandomVector2(Main.rand, -15f, 15f);
                        int iceChunk = Projectile.NewProjectile(sourceC, chunkVel, ModContent.ProjectileType<FlurrystormIceChunk>(), (int)(dmg * 1.5), (int)(kBack * 1.5), projectile.owner, 0f, chunkVel.Y);
                        Main.projectile[iceChunk].extraUpdates += fireRate / 2; //0 to 2
                    }
                }
                else
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
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * projectile.direction, projectile.velocity.X * projectile.direction);
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
