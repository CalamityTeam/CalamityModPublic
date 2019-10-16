using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class Climax2 : ModProjectile
    {
        private double timeElapsed = 0.0;
        private double circleSize = 1.0;
        private double circleGrowth = 0.02;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Climax");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.friendly = true;
            projectile.light = 0.5f;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 420;
            projectile.magic = true;
        }

        public override void AI()
        {
            timeElapsed += 0.02;
            projectile.velocity.X = (float)(Math.Sin(timeElapsed * (double)(0.5f * projectile.ai[0])) * circleSize);
            projectile.velocity.Y = (float)(Math.Cos(timeElapsed * (double)(0.5f * projectile.ai[0])) * circleSize);
            circleSize += circleGrowth;
            if (projectile.velocity.X > 0f)
            {
                projectile.rotation += (Math.Abs(projectile.velocity.Y) + Math.Abs(projectile.velocity.X)) * 0.001f;
            }
            else
            {
                projectile.rotation -= (Math.Abs(projectile.velocity.Y) + Math.Abs(projectile.velocity.X)) * 0.001f;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 4)
                {
                    projectile.frame = 0;
                }
            }
            int num3;
            for (int num433 = 0; num433 < 1000; num433 = num3 + 1)
            {
                num3 = num433;
            }
            int[] array = new int[20];
            int num434 = 0;
            float num435 = 300f;
            bool flag14 = false;
            for (int num436 = 0; num436 < 200; num436 = num3 + 1)
            {
                if (Main.npc[num436].CanBeChasedBy(projectile, false))
                {
                    float num437 = Main.npc[num436].position.X + (float)(Main.npc[num436].width / 2);
                    float num438 = Main.npc[num436].position.Y + (float)(Main.npc[num436].height / 2);
                    float num439 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num437) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num438);
                    if (num439 < num435 && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num436].Center, 1, 1))
                    {
                        if (num434 < 20)
                        {
                            array[num434] = num436;
                            num434++;
                        }
                        flag14 = true;
                    }
                }
                num3 = num436;
            }
            if (projectile.timeLeft < 30)
            {
                flag14 = false;
            }
            if (flag14)
            {
                int num440 = Main.rand.Next(num434);
                num440 = array[num440];
                float num441 = Main.npc[num440].position.X + (float)(Main.npc[num440].width / 2);
                float num442 = Main.npc[num440].position.Y + (float)(Main.npc[num440].height / 2);
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] > 4f)
                {
                    projectile.localAI[0] = 0f;
                    float num443 = 8f;
                    Vector2 vector33 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    vector33 += projectile.velocity * 4f;
                    float num444 = num441 - vector33.X;
                    float num445 = num442 - vector33.Y;
                    float num446 = (float)Math.Sqrt((double)(num444 * num444 + num445 * num445));
                    num446 = num443 / num446;
                    num444 *= num446;
                    num445 *= num446;
                    Projectile.NewProjectile(vector33.X, vector33.Y, num444, num445, ModContent.ProjectileType<ClimaxBeam>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                    return;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 30)
            {
                float num7 = (float)projectile.timeLeft / 30f;
                projectile.alpha = (int)(255f - 255f * num7);
            }
            return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
