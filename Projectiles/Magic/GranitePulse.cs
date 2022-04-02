using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class GranitePulse : ModProjectile
    {
        public bool initialized = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pulse");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 46;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 1200;
        }

        public override void AI()
        {
            projectile.velocity = new Vector2(0f, (float)Math.Sin((double)(MathHelper.TwoPi * projectile.ai[0] / 300f)) * 0.5f);
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 300f)
            {
                projectile.ai[0] = 0f;
                projectile.netUpdate = true;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            projectile.ai[1] += 1f;
            if (projectile.ai[1] >= 7200f)
            {
                projectile.alpha += 5;
                if (projectile.alpha > 255)
                {
                    projectile.alpha = 255;
                    projectile.Kill();
                }
            }
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= 10f)
            {
                projectile.localAI[0] = 0f;
                int projCount = 0;
                int index = 0;
                float findOldest = 0f;
                int projType = projectile.type;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.owner == projectile.owner && proj.type == projType && proj.ai[1] < 3600f)
                    {
                        projCount++;
                        if (proj.ai[1] > findOldest)
                        {
                            index = i;
                            findOldest = proj.ai[1];
                        }
                    }
                }
                if (projCount > 1)
                {
                    Main.projectile[index].netUpdate = true;
                    Main.projectile[index].ai[1] = 36000f;
                    return;
                }
            }
            if (!initialized)
            {
                Main.PlaySound(SoundID.NPCHit53, projectile.Center);
                CalamityGlobalProjectile.ExpandHitboxBy(projectile, 20);
                for (int d = 0; d < 5; d++)
                {
                    int ecto = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, default, 0.5f);
                    Main.dust[ecto].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[ecto].scale = 0.5f;
                        Main.dust[ecto].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int d = 0; d < 10; d++)
                {
                    int ecto = Dust.NewDust(projectile.position, projectile.width, projectile.height, 206, 0f, 0f, 100, default, 1f);
                    Main.dust[ecto].noGravity = true;
                    Main.dust[ecto].velocity *= 5f;
                    ecto = Dust.NewDust(projectile.position, projectile.width, projectile.height, 206, 0f, 0f, 100, default, 0.5f);
                    Main.dust[ecto].velocity *= 2f;
                }
                initialized = true;
            }
            if (projectile.timeLeft % 50 == 1 && projectile.alpha <= 0)
            {
                Main.PlaySound(SoundID.NPCHit53, projectile.Center);
                CalamityGlobalProjectile.ExpandHitboxBy(projectile, 20);
                for (int d = 0; d < 5; d++)
                {
                    int ecto = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, default, 0.5f);
                    Main.dust[ecto].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[ecto].scale = 0.5f;
                        Main.dust[ecto].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int d = 0; d < 10; d++)
                {
                    int ecto = Dust.NewDust(projectile.position, projectile.width, projectile.height, 206, 0f, 0f, 100, default, 1f);
                    Main.dust[ecto].noGravity = true;
                    Main.dust[ecto].velocity *= 5f;
                    ecto = Dust.NewDust(projectile.position, projectile.width, projectile.height, 206, 0f, 0f, 100, default, 0.5f);
                    Main.dust[ecto].velocity *= 2f;
                }
                float spread = 45f * 0.0174f;
                double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                double deltaAngle = spread / 8f;
                double offsetAngle;
                if (projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<GraniteEnergy>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<GraniteEnergy>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int height = texture.Height / Main.projFrames[projectile.type];
            int frameHeight = height * projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, height / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override bool CanDamage() => false;
    }
}
