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
    public class ButcherGun : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Butcher>();
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/Butcher";

        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0] += 1f;
            int incrementAmt = 0;
            float spreadMult = 0.15f;
            if (Projectile.ai[0] >= 80f)
            {
                incrementAmt++;
                spreadMult = 0.13f;
            }
            if (Projectile.ai[0] >= 160f)
            {
                incrementAmt++;
                spreadMult = 0.11f;
            }
            if (Projectile.ai[0] >= 240f)
            {
                incrementAmt++;
                spreadMult = 0.09f;
            }
            if (Projectile.ai[0] >= 320f)
            {
                incrementAmt++;
                spreadMult = 0.07f;
            }
            if (Projectile.ai[0] >= 400f)
            {
                incrementAmt++;
                spreadMult = 0.05f;
            }
            if (Projectile.ai[0] >= 480f)
            {
                incrementAmt++;
                spreadMult = 0.04f;
            }
            if (Projectile.ai[0] >= 560f)
            {
                incrementAmt++;
                spreadMult = 0.03f;
            }
            if (Projectile.ai[0] >= 640f) //8
            {
                incrementAmt++;
                spreadMult = 0.02f;
            }
            int shootDelayBase = 40;
            int incrementMult = 3;
            Projectile.ai[1] -= 1f;
            bool willShoot = false;
            if (Projectile.ai[1] <= 0f)
            {
                Projectile.ai[1] = (float)(shootDelayBase - incrementMult * incrementAmt);
                willShoot = true;
            }
            bool canShoot = player.channel && player.HasAmmo(player.ActiveItem()) && !player.noItems && !player.CCed;
            if (Projectile.localAI[0] > 0f)
            {
                Projectile.localAI[0] -= 1f;
            }
            if (Projectile.soundDelay <= 0 && canShoot)
            {
                Projectile.soundDelay = shootDelayBase - incrementMult * incrementAmt;
                if (Projectile.ai[0] != 1f)
                {
                    SoundEngine.PlaySound(SoundID.Item38, Projectile.position);
                }
                Projectile.localAI[0] = 12f;
            }
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            if (willShoot && Main.myPlayer == Projectile.owner)
            {
                int projType = ProjectileID.Bullet;
                float speedMult = 14f;
                int damage = player.GetWeaponDamage(player.ActiveItem());
                float kback = player.ActiveItem().knockBack;
                if (canShoot)
                {
                    player.PickAmmo(player.ActiveItem(), out projType, out speedMult, out damage, out kback, out _);
                    kback = player.GetWeaponKnockback(player.ActiveItem(), kback);
                    float speed = player.ActiveItem().shootSpeed * Projectile.scale;
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
                    if (velMult.X != Projectile.velocity.X || velMult.Y != Projectile.velocity.Y)
                    {
                        Projectile.netUpdate = true;
                    }
                    Projectile.velocity = velMult * 0.55f;
                    int randomBulletCount = Main.rand.Next(3, 5); //3 to 4 bullets
                    for (int projIndex = 0; projIndex < randomBulletCount; projIndex++)
                    {
                        Vector2 bulletVel = Vector2.Normalize(Projectile.velocity) * speedMult * (0.6f + Main.rand.NextFloat() * spreadMult);
                        if (float.IsNaN(bulletVel.X) || float.IsNaN(bulletVel.Y))
                        {
                            bulletVel = -Vector2.UnitY;
                        }
                        source += Utils.RandomVector2(Main.rand, -5f, 5f);
                        bulletVel.X += (float)Main.rand.Next(-15, 16) * spreadMult;
                        bulletVel.Y += (float)Main.rand.Next(-15, 16) * spreadMult;
                        int bullet = Projectile.NewProjectile(Projectile.GetSource_FromThis(), source, bulletVel, projType, damage, kback, Projectile.owner, 0f, 0f);
                        Main.projectile[bullet].noDropItem = true;
                        Main.projectile[bullet].extraUpdates += incrementAmt / 2; //0 to 4
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }
            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            float rotationAmt = 0f;
            if (Projectile.spriteDirection == -1)
            {
                rotationAmt = MathHelper.Pi;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + rotationAmt;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(Projectile.velocity.Y * (float)Projectile.direction), (double)(Projectile.velocity.X * (float)Projectile.direction));
        }

        public override bool? CanDamage() => false;
    }
}
