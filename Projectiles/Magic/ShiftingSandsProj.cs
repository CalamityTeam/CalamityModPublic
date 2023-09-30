using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class ShiftingSandsProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.soundDelay == 0 && Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) > 2f)
            {
                Projectile.soundDelay = 10;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            }
            int sand = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 85, 0f, 0f, 100, default, 2f);
            Dust dust = Main.dust[sand];
            dust.velocity *= 0.3f;
            dust.position.X = Projectile.Center.X + 4f + (float)Main.rand.Next(-4, 5);
            dust.position.Y = Projectile.Center.Y + (float)Main.rand.Next(-4, 5);
            dust.noGravity = true;

            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] <= 0f)
            {
                Player player = Main.player[Projectile.owner];
                if (player.channel)
                {
                    float speed = 18f;
                    float mouseDistX = (float)Main.mouseX + Main.screenPosition.X - Projectile.Center.X;
                    float mouseDistY = (float)Main.mouseY + Main.screenPosition.Y - Projectile.Center.Y;
                    if (player.gravDir == -1f)
                    {
                        mouseDistY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - Projectile.Center.Y;
                    }
                    Vector2 mouseVec = new Vector2(mouseDistX, mouseDistY);
                    float mouseDist = mouseVec.Length();
                    if (Projectile.ai[0] < 0f)
                    {
                        Projectile.ai[0] += 1f;
                    }
                    if (mouseDist > speed)
                    {
                        mouseDist = speed / mouseDist;
                        mouseVec.X *= mouseDist;
                        mouseVec.Y *= mouseDist;
                        int mouseSpeedX = (int)(mouseVec.X * 1000f);
                        int projSpeedX = (int)(Projectile.velocity.X * 1000f);
                        int mouseSpeedY = (int)(mouseVec.Y * 1000f);
                        int projSpeedY = (int)(Projectile.velocity.Y * 1000f);
                        if (mouseSpeedX != projSpeedX || mouseSpeedY != projSpeedY)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity.X = mouseVec.X;
                        Projectile.velocity.Y = mouseVec.Y;
                    }
                    else
                    {
                        int mouseSpeedX = (int)(mouseVec.X * 1000f);
                        int projSpeedX = (int)(Projectile.velocity.X * 1000f);
                        int mouseSpeedY = (int)(mouseVec.Y * 1000f);
                        int projSpeedY = (int)(Projectile.velocity.Y * 1000f);
                        if (mouseSpeedX != projSpeedX || mouseSpeedY != projSpeedY)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity.X = mouseVec.X;
                        Projectile.velocity.Y = mouseVec.Y;
                    }
                }
                else if (Projectile.ai[0] <= 0f)
                {
                    Projectile.netUpdate = true;
                    float speed = 12f;
                    Vector2 projCenter = Projectile.Center;
                    float mouseDistX = (float)Main.mouseX + Main.screenPosition.X - projCenter.X;
                    float mouseDistY = (float)Main.mouseY + Main.screenPosition.Y - projCenter.Y;
                    if (player.gravDir == -1f)
                    {
                        mouseDistY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - projCenter.Y;
                    }
                    Vector2 mouseVec = new Vector2(mouseDistX, mouseDistY);
                    float mouseDist = mouseVec.Length();
                    if (mouseDist == 0f || Projectile.ai[0] < 0f)
                    {
                        projCenter = player.Center;
                        mouseVec = Projectile.Center - projCenter;
                        mouseDist = mouseVec.Length();
                    }
                    mouseDist = speed / mouseDist;
                    mouseVec.X *= mouseDist;
                    mouseVec.Y *= mouseDist;
                    Projectile.velocity.X = mouseVec.X;
                    Projectile.velocity.Y = mouseVec.Y;
                    if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                    {
                        Projectile.Kill();
                    }
                    Projectile.ai[0] = 1f;
                }
            }
            if (Projectile.velocity.X != 0f || Projectile.velocity.Y != 0f)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 64;
            Projectile.Center = Projectile.position;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            int dustAmt = 36;
            for (int index = 0; index < dustAmt; index++)
            {
                Vector2 dustPos = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                dustPos = dustPos.RotatedBy((double)((float)(index - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustVel = dustPos - Projectile.Center;
                int sand = Dust.NewDust(dustPos + dustVel, 0, 0, 85, dustVel.X * 1.5f, dustVel.Y * 1.5f, 100, default, 1.2f);
                Main.dust[sand].noGravity = true;
                Main.dust[sand].noLight = true;
                Main.dust[sand].velocity = dustVel;
            }
        }
    }
}
