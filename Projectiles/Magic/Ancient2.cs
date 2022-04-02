using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class Ancient2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/Ancient";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.magic = true;
            projectile.penetrate = 4;
            projectile.extraUpdates = 12;
            projectile.timeLeft = 30;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
            projectile.scale = 0.5f;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.3f, 0.25f, 0f);
            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;
            if (projectile.ai[0] > 7f && projectile.numUpdates % 2 == 0)
            {
                int dustType = 22;
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                Dust dust = Main.dust[idx];
                if (Main.rand.NextBool(2))
                {
                    dust.noGravity = true;
                    dust.scale *= 2f;
                    dust.velocity.X *= 2f;
                    dust.velocity.Y *= 2f;
                }
                else
                {
                    dust.scale *= 1.25f;
                }
                dust.velocity.X *= 3f;
                dust.velocity.Y *= 3f;
                idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                if (Main.rand.NextBool(3))
                {
                    dust.noGravity = true;
                    dust.scale *= 2.5f;
                    dust.velocity.X *= 1.5f;
                    dust.velocity.Y *= 1.5f;
                }
                else
                {
                    dust.scale *= 1.5f;
                }
                dust.velocity.X *= 1.1f;
                dust.velocity.Y *= 1.1f;
            }
            projectile.ai[0] += 1f;
            projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 drawPos = projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
            int height = texture.Height / Main.projFrames[projectile.type];
            int frameHeight = height * projectile.frame;
            Rectangle rectangle = new Rectangle(0, frameHeight, texture.Width, height);
            Vector2 origin = new Vector2(texture.Width / 2f, height / 2f);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangle), lightColor, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);

            // Dust effects
            Circle dustCircle = new Circle(projectile.Center, projectile.width / 2);

            for (int i = 0; i < 20; i++)
            {
                // Dust
                Vector2 dustPos = dustCircle.RandomPointInCircle();
                if ((dustPos - projectile.Center).Length() > 48)
                {
                    int dustIndex = Dust.NewDust(dustPos, 1, 1, 22);
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].fadeIn = 1f;
                    Vector2 dustVelocity = projectile.Center - Main.dust[dustIndex].position;
                    float distToCenter = dustVelocity.Length();
                    dustVelocity.Normalize();
                    dustVelocity = dustVelocity.RotatedBy(MathHelper.ToRadians(-90f));
                    dustVelocity *= distToCenter * 0.04f;
                    Main.dust[dustIndex].velocity = dustVelocity;
                }
            }
            return false;
        }
    }
}
