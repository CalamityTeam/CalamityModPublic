using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class T1000Proj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Magic/T1000";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("T1000");
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 80;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.magic = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            float num = 0f;
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            if (projectile.type == ModContent.ProjectileType<T1000Proj>())
            {
                projectile.ai[0] += 1f;
                int num2 = 0;
                if (projectile.ai[0] >= 20f)
                {
                    num2++;
                }
                if (projectile.ai[0] >= 40f)
                {
                    num2++;
                }
                if (projectile.ai[0] >= 60f)
                {
                    num2++;
                }
                int num3 = 24;
                int num4 = 6;
                projectile.ai[1] += 1f;
                bool flag = false;
                if (projectile.ai[1] >= (float)(num3 - num4 * num2))
                {
                    projectile.ai[1] = 0f;
                    flag = true;
                }
                if (projectile.soundDelay <= 0)
                {
                    projectile.soundDelay = num3 - num4 * num2;
                    if (projectile.ai[0] != 1f)
                    {
                        Main.PlaySound(SoundID.Item91, projectile.position);
                    }
                }
                if (projectile.ai[1] == 1f && projectile.ai[0] != 1f)
                {
                    Vector2 vector2 = Vector2.UnitX * 24f;
                    vector2 = vector2.RotatedBy((double)(projectile.rotation - 1.57079637f), default);
                    Vector2 value = projectile.Center + vector2;
                    for (int i = 0; i < 2; i++)
                    {
                        int num5 = Dust.NewDust(value - Vector2.One * 8f, 16, 16, 66, projectile.velocity.X / 2f, projectile.velocity.Y / 2f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                        Main.dust[num5].velocity *= 0.66f;
                        Main.dust[num5].noGravity = true;
                        Main.dust[num5].scale = 1.4f;
                    }
                }
                if (flag && Main.myPlayer == projectile.owner)
                {
                    int weaponDamage2 = player.GetWeaponDamage(player.ActiveItem());
                    bool flag2 = player.channel && player.CheckMana(player.ActiveItem().mana, true, false) && !player.noItems && !player.CCed;
                    if (flag2)
                    {
                        float scaleFactor = player.ActiveItem().shootSpeed * projectile.scale;
                        Vector2 value2 = vector;
                        Vector2 value3 = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY) - value2;
                        if (player.gravDir == -1f)
                        {
                            value3.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - value2.Y;
                        }
                        Vector2 vector3 = Vector2.Normalize(value3);
                        if (float.IsNaN(vector3.X) || float.IsNaN(vector3.Y))
                        {
                            vector3 = -Vector2.UnitY;
                        }
                        vector3 *= scaleFactor;
                        if (vector3.X != projectile.velocity.X || vector3.Y != projectile.velocity.Y)
                        {
                            projectile.netUpdate = true;
                        }
                        projectile.velocity = vector3;
                        int num6 = ModContent.ProjectileType<T1000Laser>();
                        float scaleFactor2 = 14f;
                        int num7 = 7;
                        for (int j = 0; j < 4; j++)
                        {
                            value2 = projectile.Center + new Vector2((float)Main.rand.Next(-num7, num7 + 1), (float)Main.rand.Next(-num7, num7 + 1));
                            Vector2 spinningpoint = Vector2.Normalize(projectile.velocity) * scaleFactor2;
                            spinningpoint = spinningpoint.RotatedBy(Main.rand.NextDouble() * 0.19634954631328583 - 0.098174773156642914, default);
                            if (float.IsNaN(spinningpoint.X) || float.IsNaN(spinningpoint.Y))
                            {
                                spinningpoint = -Vector2.UnitY;
                            }
                            Projectile.NewProjectile(value2.X, value2.Y, spinningpoint.X, spinningpoint.Y, num6, weaponDamage2, projectile.knockBack, projectile.owner, 0f, 0f);
                        }
                    }
                    else
                    {
                        projectile.Kill();
                    }
                }
            }
            projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - projectile.Size / 2f;
            projectile.rotation = projectile.velocity.ToRotation() + num;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(projectile.velocity.Y * (float)projectile.direction), (double)(projectile.velocity.X * (float)projectile.direction));
        }
    }
}
