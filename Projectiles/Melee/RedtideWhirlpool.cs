using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class RedtideWhirlpool : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/BrinyTyphoonBubble";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Whirlpool");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.scale = 0.5f;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 2;
            projectile.melee = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0f, projectile.scale * 1.5f);
            int num626 = 6;
            for (int num627 = 0; num627 < num626; num627++)
            {
                Vector2 vector45 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                vector45 = vector45.RotatedBy((double)(num627 - (num626 / 2 - 1)) * 3.1415926535897931 / (double)(float)num626, default) + projectile.Center;
                Vector2 value15 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                int num628 = Dust.NewDust(vector45 + value15, 0, 0, 172, value15.X * 2f, value15.Y * 2f, 100, default, projectile.scale);
                Main.dust[num628].noGravity = true;
                Main.dust[num628].noLight = true;
                Main.dust[num628].velocity /= 4f;
                Main.dust[num628].velocity -= projectile.velocity;
            }
            if (projectile.scale <= 1f)
            {
                projectile.scale *= 1.015f;
            }
            if (projectile.scale >= 1f)
            {
                projectile.Kill();
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 3)
            {
                projectile.frame = 0;
            }
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
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
        }
    }
}
