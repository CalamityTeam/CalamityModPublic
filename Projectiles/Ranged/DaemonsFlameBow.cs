using System;
using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class DaemonsFlameBow : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<DaemonsFlame>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 116;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0.7f, 0.5f);
            Player player = Main.player[Projectile.owner];
            float pi = 0f;
            Vector2 playerRotate = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Projectile.spriteDirection == -1)
            {
                pi = 3.14159274f;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
            Projectile.ai[0] += 1f;
            int fireSpeed = 0;
            if (Projectile.ai[0] >= 30f)
            {
                fireSpeed++;
            }
            if (Projectile.ai[0] >= 60f)
            {
                fireSpeed++;
            }
            if (Projectile.ai[0] >= 90f)
            {
                fireSpeed++;
            }
            if (Projectile.ai[0] >= 120f)
            {
                fireSpeed++;
            }
            int delayCompare = 24;
            int fireSpeedCompare = 2;
            Projectile.ai[1] -= 1f;
            bool fullSpeed = false;
            if (Projectile.ai[1] <= 0f)
            {
                Projectile.ai[1] = (float)(delayCompare - fireSpeedCompare * fireSpeed);
                fullSpeed = true;
                int arg_1EF4_0 = (int)Projectile.ai[0] / (delayCompare - fireSpeedCompare * fireSpeed);
            }
            bool canUseItem = player.channel && player.HasAmmo(player.ActiveItem()) && !player.noItems && !player.CCed;
            if (Projectile.localAI[0] > 0f)
            {
                Projectile.localAI[0] -= 1f;
            }
            if (Projectile.soundDelay <= 0 && canUseItem)
            {
                Projectile.soundDelay = delayCompare - fireSpeedCompare * fireSpeed;
                if (Projectile.ai[0] != 1f)
                {
                    SoundEngine.PlaySound(SoundID.Item5, Projectile.position);
                }
                Projectile.localAI[0] = 12f;
            }
            if (fullSpeed && Main.myPlayer == Projectile.owner)
            {
                int ammoType = ProjectileID.WoodenArrowFriendly;
                float scaleFactor11 = 14f;
                int weaponDamage2 = player.GetWeaponDamage(player.ActiveItem());
                float weaponKnockback2 = player.ActiveItem().knockBack;
                if (canUseItem)
                {
                    player.PickAmmo(player.ActiveItem(), out ammoType, out scaleFactor11, out weaponDamage2, out weaponKnockback2, out _);
                    weaponKnockback2 = player.GetWeaponKnockback(player.ActiveItem(), weaponKnockback2);
                    float scaleFactor12 = player.ActiveItem().shootSpeed * Projectile.scale;
                    Vector2 shootDirection = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY) - playerRotate;
                    if (player.gravDir == -1f)
                    {
                        shootDirection.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - playerRotate.Y;
                    }
                    Vector2 normalizeShoot = Vector2.Normalize(shootDirection);
                    if (float.IsNaN(normalizeShoot.X) || float.IsNaN(normalizeShoot.Y))
                    {
                        normalizeShoot = -Vector2.UnitY;
                    }
                    normalizeShoot *= scaleFactor12;
                    if (normalizeShoot.X != Projectile.velocity.X || normalizeShoot.Y != Projectile.velocity.Y)
                    {
                        Projectile.netUpdate = true;
                    }
                    Projectile.velocity = normalizeShoot * 0.55f;
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 randNormalize = Vector2.Normalize(Projectile.velocity) * scaleFactor11 * (0.6f + Main.rand.NextFloat() * 0.8f);
                        if (float.IsNaN(randNormalize.X) || float.IsNaN(randNormalize.Y))
                        {
                            randNormalize = -Vector2.UnitY;
                        }
                        Vector2 projRandomPos = playerRotate + Utils.RandomVector2(Main.rand, -15f, 15f);
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), projRandomPos.X, projRandomPos.Y, randNormalize.X, randNormalize.Y, ammoType, weaponDamage2, weaponKnockback2, Projectile.owner, 0f, 0f);
                        Main.projectile[proj].noDropItem = true;
                    }
                    Vector2 daemonNormalize = Vector2.Normalize(Projectile.velocity) * scaleFactor11 * (0.6f + Main.rand.NextFloat() * 0.8f);
                    if (float.IsNaN(daemonNormalize.X) || float.IsNaN(daemonNormalize.Y))
                    {
                        daemonNormalize = -Vector2.UnitY;
                    }
                    float speedY = daemonNormalize.Y;
                    Vector2 daemonRandomPos = playerRotate + Utils.RandomVector2(Main.rand, -15f, 15f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), daemonRandomPos.X, daemonRandomPos.Y, daemonNormalize.X, daemonNormalize.Y, ModContent.ProjectileType<DaemonsFlameArrow>(), weaponDamage2, weaponKnockback2, Projectile.owner, 0.0f, speedY);
                }
                else
                {
                    Projectile.Kill();
                }
            }
            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + pi;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(Projectile.velocity.Y * (float)Projectile.direction), (double)(Projectile.velocity.X * (float)Projectile.direction));
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int framing = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = framing * Projectile.frame;
            Vector2 origin = new Vector2(27f, 58f);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/DaemonsFlameBowGlow").Value, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, framing)), Color.White, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
        }

        public override bool? CanDamage() => false;
    }
}
