using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class DevilsSunriseCyclone : ModProjectile
    {
        private int red = 0;
        private int greenAndBlue = 100;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devil's Sunrise");
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

            Vector2 vector2 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float x = Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2) - vector2.X;
            float y = Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2) - vector2.Y;
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
                    Rectangle value2 = new Rectangle((int)Main.player[Projectile.owner].position.X, (int)Main.player[Projectile.owner].position.Y, Main.player[Projectile.owner].width, Main.player[Projectile.owner].height);
                    if (rectangle.Intersects(value2))
                        Projectile.Kill();
                }
            }
            else if (Main.myPlayer == Projectile.owner && Projectile.ai[0] <= 0f)
            {
                if (Main.player[Projectile.owner].channel)
                {
                    Vector2 vector10 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float num116 = (float)Main.mouseX + Main.screenPosition.X - vector10.X;
                    float num117 = (float)Main.mouseY + Main.screenPosition.Y - vector10.Y;

                    if (Main.player[Projectile.owner].gravDir == -1f)
                        num117 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector10.Y;

                    if (Projectile.ai[0] < 0f)
                        Projectile.ai[0] += 1f;

                    float num118 = (float)Math.Sqrt((double)(num116 * num116 + num117 * num117));
                    num118 = (float)Math.Sqrt((double)(num116 * num116 + num117 * num117));
                    if (num118 > speed)
                    {
                        num118 = speed / num118;
                        num116 *= num118;
                        num117 *= num118;
                        int num119 = (int)(num116 * 1000f);
                        int num120 = (int)(Projectile.velocity.X * 1000f);
                        int num121 = (int)(num117 * 1000f);
                        int num122 = (int)(Projectile.velocity.Y * 1000f);

                        if (num119 != num120 || num121 != num122)
                            Projectile.netUpdate = true;

                        Projectile.velocity.X = num116;
                        Projectile.velocity.Y = num117;
                    }
                    else
                    {
                        int num123 = (int)(num116 * 1000f);
                        int num124 = (int)(Projectile.velocity.X * 1000f);
                        int num125 = (int)(num117 * 1000f);
                        int num126 = (int)(Projectile.velocity.Y * 1000f);

                        if (num123 != num124 || num125 != num126)
                            Projectile.netUpdate = true;

                        Projectile.velocity.X = num116;
                        Projectile.velocity.Y = num117;
                    }
                }
                else if (Projectile.ai[0] <= 0f)
                {
                    Projectile.netUpdate = true;
                    Vector2 vector11 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float num128 = (float)Main.mouseX + Main.screenPosition.X - vector11.X;
                    float num129 = (float)Main.mouseY + Main.screenPosition.Y - vector11.Y;

                    if (Main.player[Projectile.owner].gravDir == -1f)
                        num129 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector11.Y;

                    float num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
                    if (num130 == 0f || Projectile.ai[0] < 0f)
                    {
                        vector11 = new Vector2(Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2), Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2));
                        num128 = Projectile.position.X + (float)Projectile.width * 0.5f - vector11.X;
                        num129 = Projectile.position.Y + (float)Projectile.height * 0.5f - vector11.Y;
                        num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
                    }

                    num130 = speed / num130;
                    num128 *= num130;
                    num129 *= num130;
                    Projectile.velocity.X = num128;
                    Projectile.velocity.Y = num129;

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

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item88, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 64;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 66, vector7.X, vector7.Y, 100, new Color(red, greenAndBlue, greenAndBlue), 1f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
            Projectile.Damage();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
        }
    }
}
