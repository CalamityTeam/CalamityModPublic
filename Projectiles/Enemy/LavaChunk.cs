using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class LavaChunk : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lava Chunk");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.hostile = true;
            projectile.timeLeft = 360;
            projectile.penetrate = 1;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 5)
            {
                projectile.frame = 0;
            }
            if (projectile.localAI[1] < 1f)
            {
                projectile.localAI[1] += 0.002f;
                projectile.scale -= 0.002f;
                projectile.width = (int)(18f * projectile.scale);
                projectile.height = (int)(18f * projectile.scale);
            }
            else
            {
                projectile.Kill();
            }
            if (projectile.scale > 0.25f)
            {
                for (int num246 = 0; num246 < 2; num246++)
                {
                    float num248 = 0f;
                    if (num246 == 1)
                    {
                        num248 = projectile.velocity.Y * 0.5f;
                    }
                    int num249 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 6, 0f, 0f, 100, default, projectile.scale);
                    Main.dust[num249].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                    Main.dust[num249].velocity *= 0.2f;
                    Main.dust[num249].noGravity = true;
                    num249 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 31, 0f, 0f, 100, default, projectile.scale * 0.5f);
                    Main.dust[num249].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                    Main.dust[num249].velocity *= 0.05f;
                }
            }
            else
            {
                projectile.damage = 0;
            }
            if (projectile.velocity.Y < 6f)
            {
                projectile.velocity.Y = projectile.velocity.Y + 0.05f;
            }
            if (projectile.wet)
            {
                if (projectile.velocity.Y < 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y * 0.98f;
                }
                if (projectile.velocity.Y < 0.5f)
                {
                    projectile.velocity.Y = projectile.velocity.Y + 0.01f;
                }
            }
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
