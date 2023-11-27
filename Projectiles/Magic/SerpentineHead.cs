using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class SerpentineHead : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
	    Projectile.ArmorPenetration = 15;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.55f / 255f, (255 - Projectile.alpha) * 0.55f / 255f);
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 40;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            int seaDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 68, 0f, 0f, 100, default, 1.25f);
            Dust dust = Main.dust[seaDust];
            dust.velocity *= 0.3f;
            Main.dust[seaDust].position.X = Projectile.position.X + (float)(Projectile.width / 2) + 4f + (float)Main.rand.Next(-4, 5);
            Main.dust[seaDust].position.Y = Projectile.position.Y + (float)(Projectile.height / 2) + (float)Main.rand.Next(-4, 5);
            Main.dust[seaDust].noGravity = true;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57079637f;
            int direction = Projectile.direction;
            Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0f) ? 1 : -1;
            if (direction != Projectile.direction)
            {
                Projectile.netUpdate = true;
            }
            float scaleClamp = MathHelper.Clamp(Projectile.localAI[0], 0f, 50f);
            Projectile.position = Projectile.Center;
            Projectile.scale = 1f + scaleClamp * 0.01f;
            Projectile.width = Projectile.height = (int)(10 * Projectile.scale);
            Projectile.Center = Projectile.position;

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 20f && Projectile.ai[0] < 40f)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.3f;
            }
            else if (Projectile.ai[0] >= 40f && Projectile.ai[0] < 60f)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - 0.3f;
            }
            else if (Projectile.ai[0] >= 60f)
            {
                Projectile.ai[0] = 0f;
            }
            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] <= 0f)
            {
                if (Main.player[Projectile.owner].channel)
                {
                    float chaseMouseDist = 18f;
                    Vector2 projDirection = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float mouseX = (float)Main.mouseX + Main.screenPosition.X - projDirection.X;
                    float mouseY = (float)Main.mouseY + Main.screenPosition.Y - projDirection.Y;
                    if (Main.player[Projectile.owner].gravDir == -1f)
                    {
                        mouseY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - projDirection.Y;
                    }
                    float mouseDist = (float)Math.Sqrt((double)(mouseX * mouseX + mouseY * mouseY));
                    mouseDist = (float)Math.Sqrt((double)(mouseX * mouseX + mouseY * mouseY));
                    if (mouseDist > chaseMouseDist)
                    {
                        mouseDist = chaseMouseDist / mouseDist;
                        mouseX *= mouseDist;
                        mouseY *= mouseDist;
                        int xVelocity = (int)(mouseX * 1000f);
                        int exaggeratedXVelocity = (int)(Projectile.velocity.X * 1000f);
                        int yVelocity = (int)(mouseY * 1000f);
                        int exaggeratedYVelocity = (int)(Projectile.velocity.Y * 1000f);
                        if (xVelocity != exaggeratedXVelocity || yVelocity != exaggeratedYVelocity)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity.X = mouseX;
                        Projectile.velocity.Y = mouseY;
                    }
                    else
                    {
                        int xVel = (int)(mouseX * 1000f);
                        int exagXVel = (int)(Projectile.velocity.X * 1000f);
                        int yVel = (int)(mouseY * 1000f);
                        int exagYVel = (int)(Projectile.velocity.Y * 1000f);
                        if (xVel != exagXVel || yVel != exagYVel)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity.X = mouseX;
                        Projectile.velocity.Y = mouseY;
                    }
                }
                else if (Projectile.ai[0] <= 0f)
                {
                    Projectile.netUpdate = true;
                    Vector2 faceDirection = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float miceX = (float)Main.mouseX + Main.screenPosition.X - faceDirection.X;
                    float miceY = (float)Main.mouseY + Main.screenPosition.Y - faceDirection.Y;
                    if (Main.player[Projectile.owner].gravDir == -1f)
                    {
                        miceY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - faceDirection.Y;
                    }
                    float miceDist = (float)Math.Sqrt((double)(miceX * miceX + miceY * miceY));
                    if (miceDist == 0f || Projectile.ai[0] < 0f)
                    {
                        faceDirection = new Vector2(Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2), Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2));
                        miceX = Projectile.position.X + (float)Projectile.width * 0.5f - faceDirection.X;
                        miceY = Projectile.position.Y + (float)Projectile.height * 0.5f - faceDirection.Y;
                        miceDist = (float)Math.Sqrt((double)(miceX * miceX + miceY * miceY));
                    }
                    miceDist = 12f / miceDist;
                    miceX *= miceDist;
                    miceY *= miceDist;
                    Projectile.velocity.X = miceX;
                    Projectile.velocity.Y = miceY;
                    if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                    {
                        Projectile.Kill();
                    }
                    Projectile.ai[0] = 1f;
                }
            }
            if (Projectile.velocity.X != 0f || Projectile.velocity.Y != 0f)
            {
                Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
            }
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 8; k++)
            {
                int seaDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 68, 0f, 0f, 100, default, 1.25f);
                Dust dust = Main.dust[seaDust];
                dust.velocity *= 0.3f;
                Main.dust[seaDust].position.X = Projectile.position.X + (float)(Projectile.width / 2) + 4f + (float)Main.rand.Next(-4, 5);
                Main.dust[seaDust].position.Y = Projectile.position.Y + (float)(Projectile.height / 2) + (float)Main.rand.Next(-4, 5);
                Main.dust[seaDust].noGravity = true;
            }
        }
    }
}
