using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class TyphoonBubble : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/BrinyTyphoonBubble";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.timeLeft = 1;
        }

        public override void AI()
        {
            if (projectile.ai[1] > 0f)
            {
                int num625 = (int)projectile.ai[1] - 1;
                if (num625 < 255)
                {
                    projectile.localAI[0] += 1f;
                    if (projectile.localAI[0] > 10f)
                    {
                        int num626 = 6;
                        for (int num627 = 0; num627 < num626; num627++)
                        {
                            Vector2 vector45 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                            vector45 = vector45.RotatedBy((double)(num627 - (num626 / 2 - 1)) * 3.1415926535897931 / (double)(float)num626, default) + projectile.Center;
                            Vector2 value15 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                            int num628 = Dust.NewDust(vector45 + value15, 0, 0, 172, value15.X * 2f, value15.Y * 2f, 100, default, 1.4f);
                            Main.dust[num628].noGravity = true;
                            Main.dust[num628].noLight = true;
                            Main.dust[num628].velocity /= 4f;
                            Main.dust[num628].velocity -= projectile.velocity;
                        }
                        projectile.alpha -= 5;
                        if (projectile.alpha < 100)
                        {
                            projectile.alpha = 100;
                        }
                        projectile.rotation += projectile.velocity.X * 0.1f;
                        projectile.frame = (int)(projectile.localAI[0] / 3f) % 3;
                    }
                    Vector2 value16 = Main.player[num625].Center - projectile.Center;
                    float num629 = 4f;
                    num629 += projectile.localAI[0] / 20f;
                    projectile.velocity = Vector2.Normalize(value16) * num629;
                    if (value16.Length() < 50f)
                    {
                        projectile.Kill();
                    }
                }
            }
            else
            {
                float num630 = 0.209439516f;
                float num631 = 4f;
                float num632 = (float)(Math.Cos((double)(num630 * projectile.ai[0])) - 0.5) * num631;
                projectile.velocity.Y = projectile.velocity.Y - num632;
                projectile.ai[0] += 1f;
                num632 = (float)(Math.Cos((double)(num630 * projectile.ai[0])) - 0.5) * num631;
                projectile.velocity.Y = projectile.velocity.Y + num632;
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] > 10f)
                {
                    projectile.alpha -= 5;
                    if (projectile.alpha < 100)
                    {
                        projectile.alpha = 100;
                    }
                    projectile.rotation += projectile.velocity.X * 0.1f;
                    projectile.frame = (int)(projectile.localAI[0] / 3f) % 3;
                }
            }
            if (projectile.wet)
            {
                projectile.position.Y = projectile.position.Y - 16f;
                projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCDeath19, projectile.position);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 172, vector7.X * 2f, vector7.Y * 2f, 100, default, 1.4f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
            if (projectile.owner == Main.myPlayer)
            {
                if (projectile.ai[1] < 1f)
                {
                    if (projectile.localAI[1] == 1f)
                    {
                        int num230 = Projectile.NewProjectile(projectile.Center.X - (float)(projectile.direction * 30), projectile.Center.Y - 4f, (float)-(float)projectile.direction * 0.01f, 0f, ModContent.ProjectileType<SeasSearingSpout>(), projectile.damage, 3f, projectile.owner, 16f, 8f);
                        Main.projectile[num230].netUpdate = true;
                    }
                    else
                    {
                        int num230 = Projectile.NewProjectile(projectile.Center.X - (float)(projectile.direction * 30), projectile.Center.Y - 4f, (float)-(float)projectile.direction * 0.01f, 0f, ModContent.ProjectileType<WaterSpout>(), projectile.damage, 3f, projectile.owner, 16f, 8f);
                        Main.projectile[num230].netUpdate = true;
                    }
                }
            }
        }
    }
}
