using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class DevilsSunriseCyclone : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        private int red = 0;
        private int greenAndBlue = 100;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            if (Projectile.ai[1] < 510f && Projectile.ai[1] != -1f)
                Projectile.ai[1] += 1f;

            if (Main.myPlayer == Projectile.owner)
            {
                if (!Main.player[Projectile.owner].channel)
                {
                    Projectile.ai[1] = -1f;
                    Projectile.damage = Projectile.originalDamage;
                }
            }

            if (Projectile.ai[1] >= 255f)
                Projectile.damage = 2 * Projectile.originalDamage;

            red = 64 + (int)(Projectile.ai[1] * 0.75f);
            if (red > 255)
                red = 255;

            int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 66, 0f, 0f, 100, new Color(red, greenAndBlue, greenAndBlue), 1f);
            Main.dust[dust].velocity *= 0.3f;
            Main.dust[dust].noGravity = true;

            Vector2 projDirection = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float x = Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2) - projDirection.X;
            float y = Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2) - projDirection.Y;
            float distanceFromOwner = (float)Math.Sqrt((double)(x * x + y * y));

            float speed = 25f;
            if (distanceFromOwner > 300f)
                speed -= (distanceFromOwner - 300f) * 0.5f; // 350 units is about the max distance before traveling back or stopping
            if (speed < 2f)
                speed = 2f;

            if (speed <= 2f || Projectile.ai[1] >= 510f || Projectile.ai[1] == -1f)
            {
                float returnSpeedMax = 30f;
                float returnSpeed = 6f;
                distanceFromOwner = returnSpeedMax / distanceFromOwner;
                x *= distanceFromOwner;
                y *= distanceFromOwner;

                if (Projectile.velocity.X < x)
                {
                    Projectile.velocity.X = Projectile.velocity.X + returnSpeed;
                    if (Projectile.velocity.X < 0f && x > 0f)
                        Projectile.velocity.X = Projectile.velocity.X + returnSpeed;
                }
                else if (Projectile.velocity.X > x)
                {
                    Projectile.velocity.X = Projectile.velocity.X - returnSpeed;
                    if (Projectile.velocity.X > 0f && x < 0f)
                        Projectile.velocity.X = Projectile.velocity.X - returnSpeed;
                }
                if (Projectile.velocity.Y < y)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + returnSpeed;
                    if (Projectile.velocity.Y < 0f && y > 0f)
                        Projectile.velocity.Y = Projectile.velocity.Y + returnSpeed;
                }
                else if (Projectile.velocity.Y > y)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - returnSpeed;
                    if (Projectile.velocity.Y > 0f && y < 0f)
                        Projectile.velocity.Y = Projectile.velocity.Y - returnSpeed;
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    Rectangle rectangle = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
                    Rectangle playerPos = new Rectangle((int)Main.player[Projectile.owner].position.X, (int)Main.player[Projectile.owner].position.Y, Main.player[Projectile.owner].width, Main.player[Projectile.owner].height);
                    if (rectangle.Intersects(playerPos))
                        Projectile.Kill();
                }
            }
            else if (Main.myPlayer == Projectile.owner && Projectile.ai[0] <= 0f)
            {
                if (Main.player[Projectile.owner].channel)
                {
                    Vector2 projTravel = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float mouseDestX = (float)Main.mouseX + Main.screenPosition.X - projTravel.X;
                    float mouseDestY = (float)Main.mouseY + Main.screenPosition.Y - projTravel.Y;

                    if (Main.player[Projectile.owner].gravDir == -1f)
                        mouseDestY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - projTravel.Y;

                    if (Projectile.ai[0] < 0f)
                        Projectile.ai[0] += 1f;

                    float mouseDistance = (float)Math.Sqrt((double)(mouseDestX * mouseDestX + mouseDestY * mouseDestY));
                    mouseDistance = (float)Math.Sqrt((double)(mouseDestX * mouseDestX + mouseDestY * mouseDestY));
                    if (mouseDistance > speed)
                    {
                        mouseDistance = speed / mouseDistance;
                        mouseDestX *= mouseDistance;
                        mouseDestY *= mouseDistance;
                        int projMouseSpeedX = (int)(mouseDestX * 1000f);
                        int projMouseSpeedXVel = (int)(Projectile.velocity.X * 1000f);
                        int projMouseSpeedY = (int)(mouseDestY * 1000f);
                        int projMouseSpeedYVel = (int)(Projectile.velocity.Y * 1000f);

                        if (projMouseSpeedX != projMouseSpeedXVel || projMouseSpeedY != projMouseSpeedYVel)
                            Projectile.netUpdate = true;

                        Projectile.velocity.X = mouseDestX;
                        Projectile.velocity.Y = mouseDestY;
                    }
                    else
                    {
                        int projMouseSpeedyX = (int)(mouseDestX * 1000f);
                        int projMouseSpeedyXVel = (int)(Projectile.velocity.X * 1000f);
                        int projMouseSpeedyY = (int)(mouseDestY * 1000f);
                        int projMouseSpeedyYVel = (int)(Projectile.velocity.Y * 1000f);

                        if (projMouseSpeedyX != projMouseSpeedyXVel || projMouseSpeedyY != projMouseSpeedyYVel)
                            Projectile.netUpdate = true;

                        Projectile.velocity.X = mouseDestX;
                        Projectile.velocity.Y = mouseDestY;
                    }
                }
                else if (Projectile.ai[0] <= 0f)
                {
                    Projectile.netUpdate = true;
                    Vector2 projDirect = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float miceX = (float)Main.mouseX + Main.screenPosition.X - projDirect.X;
                    float miceY = (float)Main.mouseY + Main.screenPosition.Y - projDirect.Y;

                    if (Main.player[Projectile.owner].gravDir == -1f)
                        miceY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - projDirect.Y;

                    float miceDistancing = (float)Math.Sqrt((double)(miceX * miceX + miceY * miceY));
                    if (miceDistancing == 0f || Projectile.ai[0] < 0f)
                    {
                        projDirect = new Vector2(Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2), Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2));
                        miceX = Projectile.position.X + (float)Projectile.width * 0.5f - projDirect.X;
                        miceY = Projectile.position.Y + (float)Projectile.height * 0.5f - projDirect.Y;
                        miceDistancing = (float)Math.Sqrt((double)(miceX * miceX + miceY * miceY));
                    }

                    miceDistancing = speed / miceDistancing;
                    miceX *= miceDistancing;
                    miceY *= miceDistancing;
                    Projectile.velocity.X = miceX;
                    Projectile.velocity.Y = miceY;

                    if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                        Projectile.Kill();

                    Projectile.ai[0] = 1f;
                }
            }

            Lighting.AddLight(Projectile.Center, (float)((double)red * 0.001), 0.1f, 0.1f);

            Projectile.rotation += 0.5f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(red, greenAndBlue, greenAndBlue, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item88, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 64;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            int dustAmt = 36;
            for (int i = 0; i < dustAmt; i++)
            {
                Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                rotate = rotate.RotatedBy((double)((float)(i - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                Vector2 faceDirection = rotate - Projectile.Center;
                int cycloneDust = Dust.NewDust(rotate + faceDirection, 0, 0, 66, faceDirection.X, faceDirection.Y, 100, new Color(red, greenAndBlue, greenAndBlue), 1f);
                Main.dust[cycloneDust].noGravity = true;
                Main.dust[cycloneDust].noLight = true;
                Main.dust[cycloneDust].velocity = faceDirection;
            }
            Projectile.Damage();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
        }
    }
}
