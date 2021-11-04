using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class DrataliornusBow : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/Drataliornus";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Drataliornus");
        }

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 84;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.ignoreWater = true;
        }

        public override void AI() //mostly phangasm code
        {
            Lighting.AddLight(projectile.Center, 255f / 255f, 154f / 255f, 58f / 255f);
            Player player = Main.player[projectile.owner];

            projectile.ai[1]--; //usetime timer

            if (projectile.ai[0] >= 0) //spinup timer
            {
                projectile.ai[0]++;

                switch ((int)projectile.ai[0])
                {
                    case 36:
                    case 72:
                    case 108:
                    case 144:
                    case 180:
                    case 216:
                    case 252:
                    case 288:
                    case 324:
                        projectile.localAI[0]++;
                        break;

                    case 360:
                        projectile.localAI[0]++;
                        projectile.ai[0] = -1f; //fully spun up, don't need timer anymore

                        const int num226 = 36; //dusts indicate fully spun up
                        for (int num227 = 0; num227 < num226; num227++)
                        {
                            Vector2 vector6 = Vector2.Normalize(projectile.velocity) * 9f;
                            vector6 = vector6.RotatedBy((num227 - (num226 / 2 - 1)) * 6.28318548f / num226, default) + player.Center;
                            Vector2 vector7 = vector6 - player.Center;
                            int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 127, 0f, 0f, 0, default, 4f);
                            Main.dust[num228].noGravity = true;
                            Main.dust[num228].velocity = vector7;
                        }
                        break;
                }
            }

            int baseUseTime = 38;
            int modifier = 3;
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
                    Main.PlaySound(SoundID.Item5, projectile.position);
            }

            if (timeToFire && Main.myPlayer == projectile.owner)
            {
                if (canFire) //fire an angery flame
                {
                    int type = ProjectileID.WoodenArrowFriendly; //Gets changed below anyways
                    float scaleFactor = 18f;
                    int damage = player.GetWeaponDamage(player.ActiveItem());
                    float knockBack = player.ActiveItem().knockBack;

                    player.PickAmmo(player.ActiveItem(), ref type, ref scaleFactor, ref canFire, ref damage, ref knockBack, false);

                    type = ModContent.ProjectileType<DrataliornusFlame>();
                    knockBack = player.GetWeaponKnockback(player.ActiveItem(), knockBack);

                    Vector2 playerPosition = player.RotatedRelativePoint(player.MountedCenter, true);
                    projectile.velocity = Main.screenPosition - playerPosition;
                    projectile.velocity.X += Main.mouseX;
                    projectile.velocity.Y += Main.mouseY;
                    if (player.gravDir == -1f)
                        projectile.velocity.Y = Main.screenHeight - Main.mouseY + Main.screenPosition.Y - playerPosition.Y;

                    projectile.velocity.Normalize();

                    float variation = (1f + projectile.localAI[0]) * 3f; //variation increases as fire rate increases
                    Vector2 position = playerPosition + Utils.RandomVector2(Main.rand, -variation, variation);
                    Vector2 speed = projectile.velocity * scaleFactor * (0.6f + Main.rand.NextFloat() * 0.6f);

                    float ai0 = 0f;
                    if (projectile.ai[0] < 0f) //if fully spun up
                    {
                        if (Main.rand.NextBool(3)) //chance to shoot homing
                        {
                            ai0 = 2f;
                            speed /= 2f;
                        }
                        else
                        {
                            ai0 = 1f;
                        }
                    }

                    Projectile.NewProjectile(position, speed, type, damage, knockBack, projectile.owner, ai0, 0f);

                    projectile.netUpdate = true;
                }
                else
                {
                    projectile.Kill();
                }
            }

            //display projectile
            projectile.rotation = projectile.velocity.ToRotation();
            Vector2 displayOffset = new Vector2(32, 0).RotatedBy(projectile.rotation);
            projectile.Center = player.RotatedRelativePoint(player.MountedCenter, true) + displayOffset;
            if (projectile.spriteDirection == -1)
                projectile.rotation += MathHelper.Pi;
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
