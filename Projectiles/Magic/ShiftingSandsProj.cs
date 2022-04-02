using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ShiftingSandsProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shifting Sands");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.penetrate = 5;
            projectile.magic = true;
            projectile.extraUpdates = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (projectile.soundDelay == 0 && Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) > 2f)
            {
                projectile.soundDelay = 10;
                Main.PlaySound(SoundID.Item20, projectile.Center);
            }
            int sand = Dust.NewDust(projectile.position, projectile.width, projectile.height, 85, 0f, 0f, 100, default, 2f);
            Dust dust = Main.dust[sand];
            dust.velocity *= 0.3f;
            dust.position.X = projectile.Center.X + 4f + (float)Main.rand.Next(-4, 5);
            dust.position.Y = projectile.Center.Y + (float)Main.rand.Next(-4, 5);
            dust.noGravity = true;

            if (Main.myPlayer == projectile.owner && projectile.ai[0] <= 0f)
            {
                Player player = Main.player[projectile.owner];
                if (player.channel)
                {
                    float speed = 18f;
                    float mouseDistX = (float)Main.mouseX + Main.screenPosition.X - projectile.Center.X;
                    float mouseDistY = (float)Main.mouseY + Main.screenPosition.Y - projectile.Center.Y;
                    if (player.gravDir == -1f)
                    {
                        mouseDistY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - projectile.Center.Y;
                    }
                    Vector2 mouseVec = new Vector2(mouseDistX, mouseDistY);
                    float mouseDist = mouseVec.Length();
                    if (projectile.ai[0] < 0f)
                    {
                        projectile.ai[0] += 1f;
                    }
                    if (mouseDist > speed)
                    {
                        mouseDist = speed / mouseDist;
                        mouseVec.X *= mouseDist;
                        mouseVec.Y *= mouseDist;
                        int mouseSpeedX = (int)(mouseVec.X * 1000f);
                        int projSpeedX = (int)(projectile.velocity.X * 1000f);
                        int mouseSpeedY = (int)(mouseVec.Y * 1000f);
                        int projSpeedY = (int)(projectile.velocity.Y * 1000f);
                        if (mouseSpeedX != projSpeedX || mouseSpeedY != projSpeedY)
                        {
                            projectile.netUpdate = true;
                        }
                        projectile.velocity.X = mouseVec.X;
                        projectile.velocity.Y = mouseVec.Y;
                    }
                    else
                    {
                        int mouseSpeedX = (int)(mouseVec.X * 1000f);
                        int projSpeedX = (int)(projectile.velocity.X * 1000f);
                        int mouseSpeedY = (int)(mouseVec.Y * 1000f);
                        int projSpeedY = (int)(projectile.velocity.Y * 1000f);
                        if (mouseSpeedX != projSpeedX || mouseSpeedY != projSpeedY)
                        {
                            projectile.netUpdate = true;
                        }
                        projectile.velocity.X = mouseVec.X;
                        projectile.velocity.Y = mouseVec.Y;
                    }
                }
                else if (projectile.ai[0] <= 0f)
                {
                    projectile.netUpdate = true;
                    float speed = 12f;
                    Vector2 projCenter = projectile.Center;
                    float mouseDistX = (float)Main.mouseX + Main.screenPosition.X - projCenter.X;
                    float mouseDistY = (float)Main.mouseY + Main.screenPosition.Y - projCenter.Y;
                    if (player.gravDir == -1f)
                    {
                        mouseDistY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - projCenter.Y;
                    }
                    Vector2 mouseVec = new Vector2(mouseDistX, mouseDistY);
                    float mouseDist = mouseVec.Length();
                    if (mouseDist == 0f || projectile.ai[0] < 0f)
                    {
                        projCenter = player.Center;
                        mouseVec = projectile.Center - projCenter;
                        mouseDist = mouseVec.Length();
                    }
                    mouseDist = speed / mouseDist;
                    mouseVec.X *= mouseDist;
                    mouseVec.Y *= mouseDist;
                    projectile.velocity.X = mouseVec.X;
                    projectile.velocity.Y = mouseVec.Y;
                    if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                    {
                        projectile.Kill();
                    }
                    projectile.ai[0] = 1f;
                }
            }
            if (projectile.velocity.X != 0f || projectile.velocity.Y != 0f)
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 64;
            projectile.Center = projectile.position;
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.Item14, projectile.Center);
            int dustAmt = 36;
            for (int index = 0; index < dustAmt; index++)
            {
                Vector2 dustPos = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                dustPos = dustPos.RotatedBy((double)((float)(index - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                Vector2 dustVel = dustPos - projectile.Center;
                int sand = Dust.NewDust(dustPos + dustVel, 0, 0, 85, dustVel.X * 1.5f, dustVel.Y * 1.5f, 100, default, 1.2f);
                Main.dust[sand].noGravity = true;
                Main.dust[sand].noLight = true;
                Main.dust[sand].velocity = dustVel;
            }
        }
    }
}
