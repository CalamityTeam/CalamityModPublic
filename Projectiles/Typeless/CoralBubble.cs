using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Typeless
{
    public class CoralBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 360;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] > 2f)
            {
                Projectile.alpha -= 5;
                if (Projectile.alpha < 100)
                {
                    Projectile.alpha = 100;
                }
            }
            else
            {
                Projectile.localAI[0] += 1f;
            }
            if (Projectile.ai[1] > 30f)
            {
                if (Projectile.velocity.Y > -1.5f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - 0.05f;
                }
            }
            else
            {
                Projectile.ai[1] += 1f;
            }
            if (Projectile.wet)
            {
                if (Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y * 0.98f;
                }
                if (Projectile.velocity.Y > -1f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - 0.2f;
                }
            }
            int closestPlayer = (int)Player.FindClosest(Projectile.Center, 1, 1);
            Vector2 distance = Main.player[closestPlayer].Center - Projectile.Center;
            if (Projectile.Distance(Main.player[closestPlayer].Center) < 14f)
            {
                Main.player[closestPlayer].AddBuff(BuffID.Gills, 90);
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int num214 = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int size = 12;
                int dustIndex = Dust.NewDust(Projectile.Center - Vector2.One * size, size * 2, size * 2, 212);
                Dust dust = Main.dust[dustIndex];
                Vector2 value14 = Vector2.Normalize(dust.position - Projectile.Center);
                dust.position = Projectile.Center + value14 * size;
                dust.velocity = value14 * dust.velocity.Length();
                dust.color = Main.hslToRgb((float)(0.4000000059604645 + Main.rand.NextDouble() * 0.20000000298023224), 1f, 0.7f);
                dust.color = Color.Lerp(dust.color, Color.White, 0.3f);
                dust.noGravity = true;
                dust.scale = 0.7f;
            }
        }
    }
}
