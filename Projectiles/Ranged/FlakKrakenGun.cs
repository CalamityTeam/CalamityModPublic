using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class FlakKrakenGun : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/FlakKraken";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flak Kraken");
        }

        public override void SetDefaults()
        {
            projectile.width = 152;
            projectile.height = 58;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            float num = 0f;
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            if (projectile.spriteDirection == -1)
            {
                num = 3.14159274f;
            }
            float num26 = 30f;
            if (projectile.ai[0] > 90f)
            {
                num26 = 15f;
            }
            if (projectile.ai[0] > 120f)
            {
                num26 = 5f;
            }
            projectile.damage = (int)(player.ActiveItem().damage * player.RangedDamage());
            projectile.ai[0] += 1f;
            projectile.ai[1] += 1f;
            int num27 = 10;
            bool flag10 = false;
            if (projectile.ai[0] % num26 == 0f)
            {
                flag10 = true;
            }
            if (projectile.ai[1] >= 1f)
            {
                projectile.ai[1] = 0f;
                flag10 = true;
                if (Main.myPlayer == projectile.owner)
                {
                    float scaleFactor5 = player.ActiveItem().shootSpeed * projectile.scale;
                    Vector2 value12 = vector;
                    Vector2 value13 = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY) - value12;
                    if (player.gravDir == -1f)
                    {
                        value13.Y = (float)(Main.screenHeight - Main.mouseY) + Main.screenPosition.Y - value12.Y;
                    }
                    Vector2 vector11 = Vector2.Normalize(value13);
                    if (float.IsNaN(vector11.X) || float.IsNaN(vector11.Y))
                    {
                        vector11 = -Vector2.UnitY;
                    }
                    vector11 = Vector2.Normalize(Vector2.Lerp(vector11, Vector2.Normalize(projectile.velocity), 0.92f)); //0.92
                    vector11 *= scaleFactor5;
                    if (vector11.X != projectile.velocity.X || vector11.Y != projectile.velocity.Y)
                    {
                        projectile.netUpdate = true;
                    }
                    projectile.velocity = vector11;
                }
            }
            if (projectile.soundDelay <= 0)
            {
                projectile.soundDelay = num27;
                projectile.soundDelay *= 2;
                if (projectile.ai[0] != 1f && projectile.ai[0] <= 500f)
                {
                    Main.PlaySound(SoundID.Item15, projectile.position);
                }
            }
            if (flag10 && Main.myPlayer == projectile.owner)
            {
                bool flag12 = player.channel && !player.noItems && !player.CCed;
                if (flag12)
                {
                    if (projectile.ai[0] == 1f)
                    {
                        Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
                        float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
                        float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
                        if (player.gravDir == -1f)
                        {
                            num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
                        }
                        if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
                        {
                            num78 = (float)player.direction;
                            num79 = 0f;
                        }
                        vector2 += new Vector2(num78, num79);
                        int num29 = projectile.damage;
                        Projectile.NewProjectile(vector2.X, vector2.Y, 0f, 0f, ModContent.ProjectileType<FlakKrakenProj>(), num29, projectile.knockBack, projectile.owner, 0f, (float)projectile.whoAmI);
                        projectile.netUpdate = true;
                    }
                }
                else
                {
                    projectile.Kill();
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

        public override bool CanDamage() => false;
    }
}
