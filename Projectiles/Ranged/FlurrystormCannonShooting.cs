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
    public class FlurrystormCannonShooting : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<FlurrystormCannon>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.coldDamage = true;
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
            if (Projectile.frame >= 2)
            {
                Projectile.frame = 0;
            }
            Projectile.ai[0] += 1f;
            int fireRate = 0;
            if (Projectile.ai[0] >= 60f)
            {
                fireRate++;
            }
            if (Projectile.ai[0] >= 120f)
            {
                fireRate++;
            }
            if (Projectile.ai[0] >= 180f)
            {
                fireRate++;
            }
            if (Projectile.ai[0] >= 240f)
            {
                fireRate++;
            }
            if (Projectile.ai[0] >= 300f)
            {
                fireRate++;
            }
            if (Projectile.ai[0] >= 360f)
            {
                fireRate++;
            }
            if (Projectile.ai[0] >= 420f)
            {
                fireRate++;
            }
            if (Projectile.ai[0] >= 480f) //full speed after 8 seconds
            {
                fireRate++;
            }
            int initialRate = 24;
            int fireRateMult = 2;
            Projectile.ai[1] -= 1f;
            bool shouldShoot = false;
            if (Projectile.ai[1] <= 0f)
            {
                Projectile.ai[1] = (float)(initialRate - fireRateMult * fireRate);
                shouldShoot = true;
            }
            bool canShoot = player.channel && player.HasAmmo(player.ActiveItem()) && !player.noItems && !player.CCed;
            if (Projectile.localAI[0] > 0f)
            {
                Projectile.localAI[0] -= 1f;
            }
            if (Projectile.soundDelay <= 0 && canShoot)
            {
                Projectile.soundDelay = initialRate - fireRateMult * fireRate;
                if (Projectile.ai[0] != 1f)
                {
                    SoundEngine.PlaySound(SoundID.Item11, Projectile.position);
                }
                Projectile.localAI[0] = 12f;
            }
            if (shouldShoot && Main.myPlayer == Projectile.owner)
            {
                int projType = ProjectileID.SnowBallFriendly;
                float speedMult2 = 14f;
                int dmg = player.GetWeaponDamage(player.ActiveItem());
                float kBack = player.ActiveItem().knockBack;
                if (canShoot)
                {
                    player.PickAmmo(player.ActiveItem(), out projType, out speedMult2, out dmg, out kBack, out _);
                    kBack = player.GetWeaponKnockback(player.ActiveItem(), kBack);
                    float shootSpeed = player.ActiveItem().shootSpeed * Projectile.scale;
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
                    if (speedMult.X != Projectile.velocity.X || speedMult.Y != Projectile.velocity.Y)
                    {
                        Projectile.netUpdate = true;
                    }
                    Projectile.velocity = speedMult * 0.55f;

                    Vector2 snowballVel = Vector2.Normalize(Projectile.velocity) * speedMult2 * (0.6f + Main.rand.NextFloat(0f, 0.15f));
                    if (float.IsNaN(snowballVel.X) || float.IsNaN(snowballVel.Y))
                    {
                        snowballVel = -Vector2.UnitY;
                    }
                    Vector2 sourceS = source + Utils.RandomVector2(Main.rand, -5f, 5f);
                    snowballVel.X += Main.rand.NextFloat(-2.25f, 2.25f);
                    snowballVel.Y += Main.rand.NextFloat(-2.25f, 2.25f);
                    int snowball = Projectile.NewProjectile(Projectile.GetSource_FromThis(), sourceS, snowballVel, projType, dmg, kBack, Projectile.owner);
                    if (snowball.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[snowball].noDropItem = true;
                        Main.projectile[snowball].DamageType = DamageClass.Ranged;
                        Main.projectile[snowball].extraUpdates += Main.rand.Next(0,2);
                    }

                    if (Main.rand.NextBool(5)) //ice chunk
                    {
                        Vector2 chunkVel = Vector2.Normalize(Projectile.velocity) * speedMult2 * (0.6f + Main.rand.NextFloat() * 0.8f);
                        if (float.IsNaN(chunkVel.X) || float.IsNaN(chunkVel.Y))
                        {
                            chunkVel = -Vector2.UnitY;
                        }
                        Vector2 sourceC = source + Utils.RandomVector2(Main.rand, -15f, 15f);
                        int iceChunk = Projectile.NewProjectile(Projectile.GetSource_FromThis(), sourceC, chunkVel, ModContent.ProjectileType<FlurrystormIceChunk>(), (int)(dmg * 1.5), (int)(kBack * 1.5), Projectile.owner, 0f, chunkVel.Y);
                        Main.projectile[iceChunk].extraUpdates += fireRate / 2; //0 to 2
                    }
                }
                else
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
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
        }

        public override bool? CanDamage() => false;
    }
}
