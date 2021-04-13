using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class DevilsSunriseCyclone : ModProjectile
    {
        private int red = 0;
        private int greenAndBlue = 100;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devil's Sunrise");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            if (projectile.ai[1] < 510f && projectile.ai[1] != -1f)
                projectile.ai[1] += 1f;

            if (Main.myPlayer == projectile.owner)
            {
                if (!Main.player[projectile.owner].channel)
                {
                    projectile.ai[1] = -1f;
                    projectile.damage = projectile.Calamity().defDamage;
                }
            }

            if (projectile.ai[1] >= 255f)
                projectile.damage = (int)((double)projectile.Calamity().defDamage * 2.0);

            red = 64 + (int)(projectile.ai[1] * 0.75f);
            if (red > 255)
                red = 255;

            int dust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 66, 0f, 0f, 100, new Color(red, greenAndBlue, greenAndBlue), 1f);
            Main.dust[dust].velocity *= 0.3f;
            Main.dust[dust].noGravity = true;

            Vector2 vector2 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
            float x = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector2.X;
            float y = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - vector2.Y;
            float distanceFromOwner = (float)Math.Sqrt((double)(x * x + y * y));

            float speed = 25f;
            if (distanceFromOwner > 300f)
                speed -= (distanceFromOwner - 300f) * 0.5f; // 350 units is about the max distance before traveling back or stopping
            if (speed < 2f)
                speed = 2f;

            if (speed <= 2f || projectile.ai[1] >= 510f || projectile.ai[1] == -1f)
            {
                float returnSpeedMax = 30f;
                float returnSpeed = 6f;
                distanceFromOwner = returnSpeedMax / distanceFromOwner;
                x *= distanceFromOwner;
                y *= distanceFromOwner;

                if (projectile.velocity.X < x)
                {
                    projectile.velocity.X = projectile.velocity.X + returnSpeed;
                    if (projectile.velocity.X < 0f && x > 0f)
                        projectile.velocity.X = projectile.velocity.X + returnSpeed;
                }
                else if (projectile.velocity.X > x)
                {
                    projectile.velocity.X = projectile.velocity.X - returnSpeed;
                    if (projectile.velocity.X > 0f && x < 0f)
                        projectile.velocity.X = projectile.velocity.X - returnSpeed;
                }
                if (projectile.velocity.Y < y)
                {
                    projectile.velocity.Y = projectile.velocity.Y + returnSpeed;
                    if (projectile.velocity.Y < 0f && y > 0f)
                        projectile.velocity.Y = projectile.velocity.Y + returnSpeed;
                }
                else if (projectile.velocity.Y > y)
                {
                    projectile.velocity.Y = projectile.velocity.Y - returnSpeed;
                    if (projectile.velocity.Y > 0f && y < 0f)
                        projectile.velocity.Y = projectile.velocity.Y - returnSpeed;
                }

                if (Main.myPlayer == projectile.owner)
                {
                    Rectangle rectangle = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
                    Rectangle value2 = new Rectangle((int)Main.player[projectile.owner].position.X, (int)Main.player[projectile.owner].position.Y, Main.player[projectile.owner].width, Main.player[projectile.owner].height);
                    if (rectangle.Intersects(value2))
                        projectile.Kill();
                }
            }
            else if (Main.myPlayer == projectile.owner && projectile.ai[0] <= 0f)
            {
                if (Main.player[projectile.owner].channel)
                {
                    Vector2 vector10 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num116 = (float)Main.mouseX + Main.screenPosition.X - vector10.X;
                    float num117 = (float)Main.mouseY + Main.screenPosition.Y - vector10.Y;

                    if (Main.player[projectile.owner].gravDir == -1f)
                        num117 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector10.Y;

                    if (projectile.ai[0] < 0f)
                        projectile.ai[0] += 1f;

                    float num118 = (float)Math.Sqrt((double)(num116 * num116 + num117 * num117));
                    num118 = (float)Math.Sqrt((double)(num116 * num116 + num117 * num117));
                    if (num118 > speed)
                    {
                        num118 = speed / num118;
                        num116 *= num118;
                        num117 *= num118;
                        int num119 = (int)(num116 * 1000f);
                        int num120 = (int)(projectile.velocity.X * 1000f);
                        int num121 = (int)(num117 * 1000f);
                        int num122 = (int)(projectile.velocity.Y * 1000f);

                        if (num119 != num120 || num121 != num122)
                            projectile.netUpdate = true;

                        projectile.velocity.X = num116;
                        projectile.velocity.Y = num117;
                    }
                    else
                    {
                        int num123 = (int)(num116 * 1000f);
                        int num124 = (int)(projectile.velocity.X * 1000f);
                        int num125 = (int)(num117 * 1000f);
                        int num126 = (int)(projectile.velocity.Y * 1000f);

                        if (num123 != num124 || num125 != num126)
                            projectile.netUpdate = true;

                        projectile.velocity.X = num116;
                        projectile.velocity.Y = num117;
                    }
                }
                else if (projectile.ai[0] <= 0f)
                {
                    projectile.netUpdate = true;
                    Vector2 vector11 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num128 = (float)Main.mouseX + Main.screenPosition.X - vector11.X;
                    float num129 = (float)Main.mouseY + Main.screenPosition.Y - vector11.Y;

                    if (Main.player[projectile.owner].gravDir == -1f)
                        num129 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector11.Y;

                    float num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
                    if (num130 == 0f || projectile.ai[0] < 0f)
                    {
                        vector11 = new Vector2(Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2), Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2));
                        num128 = projectile.position.X + (float)projectile.width * 0.5f - vector11.X;
                        num129 = projectile.position.Y + (float)projectile.height * 0.5f - vector11.Y;
                        num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
                    }

                    num130 = speed / num130;
                    num128 *= num130;
                    num129 *= num130;
                    projectile.velocity.X = num128;
                    projectile.velocity.Y = num129;

                    if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                        projectile.Kill();

                    projectile.ai[0] = 1f;
                }
            }

            Lighting.AddLight(projectile.Center, (float)((double)red * 0.001), 0.1f, 0.1f);

            projectile.rotation += 0.5f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(red, greenAndBlue, greenAndBlue, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item88, (int)projectile.position.X, (int)projectile.position.Y);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 64;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 66, vector7.X, vector7.Y, 100, new Color(red, greenAndBlue, greenAndBlue), 1f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
            projectile.Damage();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
        }
    }
}
