using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class NorfleetCannon : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Norfleet");
        }

        public override void SetDefaults()
        {
            projectile.width = 140;
            projectile.height = 42;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.ai[1]--;
            if (projectile.ai[0] >= 0f)
            {
                projectile.ai[0] += 1f;
                switch ((int)projectile.ai[0])
                {
                    case 90:
                    case 180:
                    case 270:
                    case 360:
                    case 450:
                        projectile.localAI[0] += 1f;
                        break;
                    case 540:
                        projectile.localAI[0] += 1f;
                        projectile.ai[0] = -1f;
                        Main.PlaySound(SoundID.Item14, projectile.position);
                        int num226 = 36;
                        for (int num227 = 0; num227 < num226; num227++)
                        {
                            Vector2 vector6 = Vector2.Normalize(projectile.velocity) * 9f;
                            vector6 = vector6.RotatedBy((num227 - (num226 / 2 - 1)) * 6.28318548f / num226, default) + player.Center;
                            Vector2 vector7 = vector6 - player.Center;
                            int num228 = Dust.NewDust(vector6 + vector7, 0, 0, Main.rand.NextBool(2) ? 221 : 244, 0f, 0f, 0, default, 4f);
                            Main.dust[num228].noGravity = true;
                            Main.dust[num228].velocity = vector7;
                        }
                        break;
                }
            }
            int baseUseTime = 75;
            int modifier = 2;
            bool timeToFire = false;
            if (projectile.ai[1] <= 0f)
            {
                projectile.ai[1] = baseUseTime - modifier * projectile.localAI[0];
                timeToFire = true;
            }
            bool canFire = player.channel && player.HasAmmo(player.ActiveItem(), true) && !player.noItems && !player.CCed;
            if (projectile.soundDelay <= 0 && canFire)
            {
                projectile.soundDelay = baseUseTime - modifier * (int)projectile.localAI[0];
                if (projectile.ai[0] != 1f)
                    Main.PlaySound(SoundID.Item92, projectile.position);
            }
            player.phantasmTime = 2;
            if (timeToFire && Main.myPlayer == projectile.owner)
            {
                if (canFire)
                {
                    int type = ProjectileID.FallingStar; //Gets changed below anyways
                    float scaleFactor = 30f;
                    int damage = player.GetWeaponDamage(player.ActiveItem());
                    float knockBack = player.ActiveItem().knockBack;
                    for (int i = 0; i < 5; i++)
                    {
                        player.PickAmmo(player.ActiveItem(), ref type, ref scaleFactor, ref canFire, ref damage, ref knockBack, false);
                        knockBack = player.GetWeaponKnockback(player.ActiveItem(), knockBack);
                        Vector2 playerPosition = player.RotatedRelativePoint(player.MountedCenter, true);
                        projectile.velocity = Main.screenPosition - playerPosition;
                        projectile.velocity.X += Main.mouseX;
                        projectile.velocity.Y += Main.mouseY;
                        if (player.gravDir == -1f)
                            projectile.velocity.Y = Main.screenHeight - Main.mouseY + Main.screenPosition.Y - playerPosition.Y;
                        projectile.velocity.Normalize();
                        float variation = (1f + projectile.localAI[0]) * 3f;
                        Vector2 position = playerPosition + Utils.RandomVector2(Main.rand, -variation, variation);
                        Vector2 speed = projectile.velocity * scaleFactor * (0.6f + Main.rand.NextFloat() * 0.6f);
                        type = ModContent.ProjectileType<NorfleetComet>();
                        speed.X += (float)Main.rand.Next(-30, 31) * 0.05f;
                        speed.Y += (float)Main.rand.Next(-30, 31) * 0.05f;
                        Projectile.NewProjectile(position, speed, type, damage, knockBack, projectile.owner, 0f, 0f);
                        speed.X += (float)Main.rand.Next(-30, 31) * 0.05f;
                        speed.Y += (float)Main.rand.Next(-30, 31) * 0.05f;
                        Projectile.NewProjectile(position, speed, type, damage, knockBack, projectile.owner, 0f, 0f);
                        projectile.netUpdate = true;
                    }
                }
                else
                {
                    projectile.Kill();
                }
            }
            projectile.rotation = projectile.velocity.ToRotation();
            Vector2 displayOffset = new Vector2(32, 0).RotatedBy(projectile.rotation);
            projectile.Center = player.RotatedRelativePoint(player.MountedCenter, true) + displayOffset;
            if (projectile.spriteDirection == -1)
                projectile.rotation += 3.14159274f;
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
