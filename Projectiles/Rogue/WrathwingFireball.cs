using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class WrathwingFireball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wrathwing Fireball");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.aiStyle = 1;
            aiType = ProjectileID.DD2BetsyFireball;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            // Cancel out the first ten frames of Betsy fireball gravity, and half of all gravity thereafter
            projectile.velocity.Y -= 0.1f;

            // Animation
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
                projectile.frame = 0;

            // Keep the fireball rotated the correct way because the sheet faces down, not up
            projectile.rotation += MathHelper.Pi;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int frameY = frameHeight * projectile.frame;
            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 0; i < projectile.oldPos.Length; i++)
                {
                    Vector2 drawPos = projectile.oldPos[i] + projectile.Size / 2f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
                    Color color2 = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - i) / (float)projectile.oldPos.Length);
                    Main.spriteBatch.Draw(texture, drawPos, new Microsoft.Xna.Framework.Rectangle?(rectangle), color2, projectile.rotation, rectangle.Size() / 2f, projectile.scale, spriteEffects, 0f);
                }
            }

            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameY, texture.Width, frameHeight)),
                projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)frameHeight / 2f), projectile.scale, spriteEffects, 0f);

            return false;
        }

        // Expand hitbox and explode on hit.
        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 144);

            // Allow infinite piercing and ignoring iframes for this one extra AoE hit
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.Damage();

            for (int i = 0; i < 2; i++)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 55, 0f, 0f, 50, default, 1.5f);

            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 55, 0f, 0f, 0, default, 2.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 3f;
                d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 55, 0f, 0f, 50, default, 1.5f);
                Main.dust[d].velocity *= 2f;
                Main.dust[d].noGravity = true;
            }
        }
    }
}
