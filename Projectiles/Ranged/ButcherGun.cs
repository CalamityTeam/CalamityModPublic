using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ButcherGun : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/Butcher";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Butcher");
        }

        public override void SetDefaults()
        {
            projectile.width = 66;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.ai[0] += 1f;
            int incrementAmt = 0;
            float spreadMult = 0.15f;
            if (projectile.ai[0] >= 80f)
            {
                incrementAmt++;
                spreadMult = 0.13f;
            }
            if (projectile.ai[0] >= 160f)
            {
                incrementAmt++;
                spreadMult = 0.11f;
            }
            if (projectile.ai[0] >= 240f)
            {
                incrementAmt++;
                spreadMult = 0.09f;
            }
            if (projectile.ai[0] >= 320f)
            {
                incrementAmt++;
                spreadMult = 0.07f;
            }
            if (projectile.ai[0] >= 400f)
            {
                incrementAmt++;
                spreadMult = 0.05f;
            }
            if (projectile.ai[0] >= 480f)
            {
                incrementAmt++;
                spreadMult = 0.04f;
            }
            if (projectile.ai[0] >= 560f)
            {
                incrementAmt++;
                spreadMult = 0.03f;
            }
            if (projectile.ai[0] >= 640f) //8
            {
                incrementAmt++;
                spreadMult = 0.02f;
            }
            int shootDelayBase = 40;
            int incrementMult = 3;
            projectile.ai[1] -= 1f;
            bool willShoot = false;
            if (projectile.ai[1] <= 0f)
            {
                projectile.ai[1] = (float)(shootDelayBase - incrementMult * incrementAmt);
                willShoot = true;
            }
            bool canShoot = player.channel && player.HasAmmo(player.ActiveItem(), true) && !player.noItems && !player.CCed;
            if (projectile.localAI[0] > 0f)
            {
                projectile.localAI[0] -= 1f;
            }
            if (projectile.soundDelay <= 0 && canShoot)
            {
                projectile.soundDelay = shootDelayBase - incrementMult * incrementAmt;
                if (projectile.ai[0] != 1f)
                {
                    Main.PlaySound(SoundID.Item38, projectile.position);
                }
                projectile.localAI[0] = 12f;
            }
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            if (willShoot && Main.myPlayer == projectile.owner)
            {
                int projType = ProjectileID.Bullet;
                float speedMult = 14f;
                int damage = player.GetWeaponDamage(player.ActiveItem());
                float kback = player.ActiveItem().knockBack;
                if (canShoot)
                {
                    player.PickAmmo(player.ActiveItem(), ref projType, ref speedMult, ref canShoot, ref damage, ref kback);
                    kback = player.GetWeaponKnockback(player.ActiveItem(), kback);
                    float speed = player.ActiveItem().shootSpeed * projectile.scale;
                    Vector2 targetPos = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY) - source;
                    if (player.gravDir == -1f)
                    {
                        targetPos.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - source.Y;
                    }
                    Vector2 velMult = Vector2.Normalize(targetPos);
                    if (float.IsNaN(velMult.X) || float.IsNaN(velMult.Y))
                    {
                        velMult = -Vector2.UnitY;
                    }
                    velMult *= speed;
                    if (velMult.X != projectile.velocity.X || velMult.Y != projectile.velocity.Y)
                    {
                        projectile.netUpdate = true;
                    }
                    projectile.velocity = velMult * 0.55f;
                    int randomBulletCount = Main.rand.Next(3, 5); //3 to 4 bullets
                    for (int projIndex = 0; projIndex < randomBulletCount; projIndex++)
                    {
                        Vector2 bulletVel = Vector2.Normalize(projectile.velocity) * speedMult * (0.6f + Main.rand.NextFloat() * spreadMult);
                        if (float.IsNaN(bulletVel.X) || float.IsNaN(bulletVel.Y))
                        {
                            bulletVel = -Vector2.UnitY;
                        }
                        source += Utils.RandomVector2(Main.rand, -5f, 5f);
                        bulletVel.X += (float)Main.rand.Next(-15, 16) * spreadMult;
                        bulletVel.Y += (float)Main.rand.Next(-15, 16) * spreadMult;
                        int num44 = Projectile.NewProjectile(source, bulletVel, projType, damage, kback, projectile.owner, 0f, 0f);
                        Main.projectile[num44].noDropItem = true;
                        Main.projectile[num44].extraUpdates += incrementAmt / 2; //0 to 4
                    }
                }
                else
                {
                    projectile.Kill();
                }
            }
            projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - projectile.Size / 2f;
            float rotationAmt = 0f;
            if (projectile.spriteDirection == -1)
            {
                rotationAmt = MathHelper.Pi;
            }
            projectile.rotation = projectile.velocity.ToRotation() + rotationAmt;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(projectile.velocity.Y * (float)projectile.direction), (double)(projectile.velocity.X * (float)projectile.direction));
        }

        public override bool CanDamage() => false;
    }
}
